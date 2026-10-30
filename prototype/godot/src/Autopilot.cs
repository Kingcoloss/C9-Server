using Eighth.Sim;
using Godot;

namespace EighthThrone.Demo;

// โหมดสาธิต: บอทเล่นแทน เปลี่ยนค่าเครือข่ายตามลำดับ แล้วบันทึกภาพหน้าจอ
// เปิดด้วย: godot --path . -- --autoplay --shots=/โฟลเดอร์/ที่เก็บภาพ
public sealed class Autopilot
{
    readonly IGameHost _m;
    readonly FightBot _bot = new();
    readonly string? _dir;
    readonly List<Pending> _pending = new();
    readonly HashSet<string> _taken = new();
    int _frame;
    int _stage;
    int _stageFrame;
    int _quitAt = -1;

    sealed class Pending
    {
        public string Name = "";
        public int Frame;
        public Action? After;
    }

    public Autopilot(IGameHost m, string? shotDir)
    {
        _m = m;
        _dir = shotDir;
        if (_dir != null) DirAccess.MakeDirRecursiveAbsolute(_dir);
    }

    // เรียกก่อนเดินเวลาในแต่ละเฟรม
    public void Step()
    {
        _frame++;
        Capture();
        var s = _m.Session;
        _bot.Step(s.Client, s.Now, _frame);

        if (_stage == 0)
        {
            _m.SetPing(2);
            _m.SetComp(true);
            Next();
        }
        else if (_stage == 3)
        {
            if (_frame - _stageFrame == 45) Shot("03-server-ghost", 0);
            if (_frame - _stageFrame >= 120)
            {
                _m.ShowServer = false;
                Next();
            }
        }

        foreach (var e in s.Client.Enemies(s.Now))
        {
            if (_m.T < _m.BannerUntil) break;
            if (e.V.Kind != EKind.Boss || e.V.Phase != EPhase.Windup || e.V.Attack == AttackId.None) continue;
            if (Telegraph.Progress(e) < 0.7f) continue;
            var tele = Attacks.Get(e.V.Attack).Tele;
            Shot(tele == Tele.Red ? "05-boss-sweep" : "04-boss-telegraph", 0);
        }

        if (_frame > 30 * 60 * 5 && _quitAt < 0) _quitAt = _frame;
        if (_quitAt >= 0 && _frame >= _quitAt) _m.GetTree().Quit();
    }

    public void OnUi(in UiEvent ui)
    {
        switch (ui.Kind)
        {
            case UiKind.Verdict when _stage == 1 && ui.Server is Judgment.PerfectDodge or Judgment.PerfectGuard:
                Next();
                Shot("01-perfect-ping150-comp-on", 6, () => _m.SetComp(false));
                break;
            case UiKind.Verdict when _stage == 2 && ui.Server is Judgment.Hit or Judgment.GuardBreak or Judgment.Guard:
                Next();
                Shot("02-ping150-comp-off", 6, () =>
                {
                    _m.SetComp(true);
                    _m.ShowServer = true;
                    _stageFrame = _frame;
                });
                break;
            case UiKind.Stagger when IsBoss(ui.EnemyId):
                Shot("06-boss-stagger", 10);
                break;
            case UiKind.Banner when ui.Text.StartsWith("ชนะ") || ui.Text.StartsWith("พ่ายแพ้"):
                Shot("07-end", 24);
                _quitAt = _frame + 75;
                break;
        }
    }

    bool IsBoss(int id)
    {
        foreach (var e in _m.Session.Client.Latest.Enemies)
            if (e.Id == id) return e.Kind == EKind.Boss;
        return false;
    }

    void Next()
    {
        _stage++;
        _stageFrame = _frame;
    }

    void Shot(string name, int delay, Action? after = null)
    {
        if (!_taken.Add(name)) return;
        _pending.Add(new Pending { Name = name, Frame = _frame + delay, After = after });
    }

    // บันทึกภาพของเฟรมที่วาดเสร็จล่าสุด
    void Capture()
    {
        for (int i = 0; i < _pending.Count; i++)
        {
            var p = _pending[i];
            if (_frame < p.Frame) continue;
            _pending.RemoveAt(i--);
            if (_dir != null)
            {
                var img = _m.GetViewport().GetTexture().GetImage();
                var path = $"{_dir}/{p.Name}.png";
                var err = img.SavePng(path);
                GD.Print($"[autoplay] {p.Name} t={_m.Session.Now:0.0}s -> {err}");
            }
            p.After?.Invoke();
        }
    }
}
