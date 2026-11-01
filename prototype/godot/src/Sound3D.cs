using Godot;

namespace EighthThrone.Demo;

// เสียงทั้งหมดสังเคราะห์จากโค้ดตอนเปิดเกม ไม่มีไฟล์เสียงจากที่อื่น
// ใช้แทนไปก่อนจนกว่าจะมีนักออกแบบเสียงทำเสียงจริง
public static class SfxBank
{
    const int Rate = 44100;
    static readonly Dictionary<string, AudioStreamWav> _all = new();
    static System.Random _rng = new(8);

    public static AudioStreamWav Get(string id)
    {
        if (_all.Count == 0) Build();
        return _all[id];
    }

    // เขียนเสียงทั้งหมดเป็นไฟล์ .wav ไว้ฟังหรือส่งให้นักออกแบบเสียงดูเป็นตัวอย่าง (--dump-sfx=<โฟลเดอร์>)
    public static void Dump(string dir)
    {
        if (_all.Count == 0) Build();
        DirAccess.MakeDirRecursiveAbsolute(dir);
        foreach (var (id, wav) in _all) wav.SaveToWav($"{dir}/{id}.wav");
    }

    static void Build()
    {
        _all["swing_light"] = Wav(Whoosh(0.2f, 700, 2600, 1.0f));
        _all["swing_heavy"] = Wav(Whoosh(0.34f, 280, 1300, 1.0f));
        _all["swing_spin"] = Wav(Mix(Whoosh(0.55f, 400, 1800, 0.9f, tremolo: 14), Whoosh(0.55f, 900, 2400, 0.4f)));
        _all["dash"] = Wav(Whoosh(0.3f, 1500, 500, 0.8f));
        _all["dodge"] = Wav(Mix(Whoosh(0.26f, 1100, 450, 0.75f), Noise(0.12f, 400, 0.25f, 0.05f)));
        _all["hit"] = Wav(Mix(Thump(0.2f, 150, 55, 1f), Crunch(0.08f, 1700, 0.55f), Click(0.6f)));
        _all["hit_heavy"] = Wav(Mix(Thump(0.32f, 120, 38, 1f), Crunch(0.14f, 1100, 0.75f), Click(0.8f)));
        _all["hurt"] = Wav(Mix(Thump(0.22f, 110, 50, 1f), Crunch(0.06f, 900, 0.4f)));
        _all["clank"] = Wav(Mix(Bell(new[] { 523f, 1187, 1789, 2633, 3410 }, new[] { 0.28f, 0.2f, 0.14f, 0.1f, 0.07f }, 0.7f), Click(0.7f)));
        _all["perfect"] = Wav(Mix(
            Bell(new[] { 1760f, 2640, 3527, 5280 }, new[] { 0.9f, 0.7f, 0.6f, 0.35f }, 0.75f),
            Thump(0.35f, 90, 45, 0.6f)));
        _all["step"] = Wav(Mix(Noise(0.06f, 900, 0.5f, 0.004f), Thump(0.06f, 95, 70, 0.35f)));
        _all["growl"] = Wav(Growl(0.45f, 210, 0.9f));
        _all["tele_gold"] = Wav(Mix(Tone(0.18f, 880, 0.5f, 0.12f), Delay(Tone(0.3f, 1320, 0.55f, 0.25f), 0.09f)));
        _all["tele_red"] = Wav(Horn(0.6f, 98, 1f));
        _all["tele_purple"] = Wav(Glide(0.45f, 700, 420, 0.7f));
        _all["slam"] = Wav(Mix(Thump(0.9f, 70, 28, 1f), Noise(0.7f, 500, 0.7f, 0.25f), Crunch(0.25f, 700, 0.5f)));
        _all["ink"] = Wav(Bubbles(0.7f, 0.8f));
        _all["stagger"] = Wav(Glide(0.6f, 1300, 480, 0.55f, vibrato: 9));
        _all["kill"] = Wav(Mix(Thump(0.4f, 90, 35, 1f), Noise(0.35f, 700, 0.4f, 0.12f)));
        _all["boss_death"] = Wav(Mix(Thump(1.8f, 55, 22, 1f), Noise(1.6f, 350, 0.8f, 0.7f), Glide(1.4f, 300, 90, 0.35f)));
        _all["banner"] = Wav(Bell(new[] { 196f, 294, 392.6f, 588 }, new[] { 1.4f, 1.1f, 0.9f, 0.6f }, 0.5f));
        _all["wind"] = Wav(Wind(8f, 0.5f), loop: true);
    }

    // ---- ส่วนประกอบเสียง ----

    static int N(float sec) => (int)(sec * Rate);
    static float Rnd() => (float)(_rng.NextDouble() * 2 - 1);

    // ลมจากการเหวี่ยง: noise ผ่าน band-pass ที่กวาดความถี่
    static float[] Whoosh(float dur, float f0, float f1, float gain, float tremolo = 0)
    {
        var b = new float[N(dur)];
        var f = new Svf();
        for (int i = 0; i < b.Length; i++)
        {
            float t = (float)i / b.Length;
            float fc = f0 * Mathf.Pow(f1 / f0, t);
            float env = Mathf.Pow(Mathf.Max(0, Mathf.Sin(Mathf.Pi * Mathf.Min(1, t * 1.15f))), 1.6f);
            if (tremolo > 0) env *= 0.65f + 0.35f * Mathf.Sin(Mathf.Tau * tremolo * i / Rate);
            b[i] = f.Band(Rnd(), fc, 1.6f) * env;
        }
        return Norm(b, gain);
    }

    // เสียงกระแทกต่ำ: sine ที่เสียงตกลงเร็ว
    static float[] Thump(float dur, float f0, float f1, float gain)
    {
        var b = new float[N(dur)];
        float ph = 0;
        for (int i = 0; i < b.Length; i++)
        {
            float t = (float)i / Rate;
            float fc = f1 + (f0 - f1) * Mathf.Exp(-t * 28);
            ph += Mathf.Tau * fc / Rate;
            b[i] = Mathf.Sin(ph) * Mathf.Exp(-t * 5.5f / dur) * Mathf.Min(1, i / 60f);
        }
        return Norm(b, gain);
    }

    static float[] Crunch(float dur, float fc, float gain)
    {
        var b = new float[N(dur)];
        var f = new Svf();
        for (int i = 0; i < b.Length; i++)
        {
            float t = (float)i / b.Length;
            b[i] = f.Band(Rnd() * (Rnd() > 0.2f ? 1 : 2.5f), fc, 0.9f) * Mathf.Exp(-t * 5);
        }
        return Norm(b, gain);
    }

    static float[] Click(float gain)
    {
        var b = new float[N(0.012f)];
        for (int i = 0; i < b.Length; i++) b[i] = Rnd() * (1 - (float)i / b.Length);
        return Norm(b, gain);
    }

    static float[] Noise(float dur, float lowpass, float gain, float attack)
    {
        var b = new float[N(dur)];
        float y = 0, a = 1 - Mathf.Exp(-Mathf.Tau * lowpass / Rate);
        for (int i = 0; i < b.Length; i++)
        {
            float t = (float)i / Rate;
            y += a * (Rnd() - y);
            float env = Mathf.Min(1, t / Mathf.Max(0.001f, attack)) * Mathf.Exp(-(t - attack) * 4 / dur);
            b[i] = y * env;
        }
        return Norm(b, gain);
    }

    // โลหะ/ระฆัง: ผลรวมของความถี่ไม่ลงตัวที่จางไม่เท่ากัน
    static float[] Bell(float[] freqs, float[] decays, float gain)
    {
        float dur = 0;
        foreach (var d in decays) dur = Mathf.Max(dur, d * 4);
        var b = new float[N(dur)];
        for (int k = 0; k < freqs.Length; k++)
            for (int i = 0; i < b.Length; i++)
            {
                float t = (float)i / Rate;
                b[i] += Mathf.Sin(Mathf.Tau * freqs[k] * t) * Mathf.Exp(-t / decays[k]) / (k + 1) * Mathf.Min(1, i / 30f);
            }
        return Norm(b, gain);
    }

    static float[] Tone(float dur, float freq, float gain, float decay)
    {
        var b = new float[N(dur + decay * 3)];
        for (int i = 0; i < b.Length; i++)
        {
            float t = (float)i / Rate;
            float env = Mathf.Min(1, t / 0.01f) * (t < dur ? 1 : Mathf.Exp(-(t - dur) / decay));
            b[i] = (Mathf.Sin(Mathf.Tau * freq * t) + 0.3f * Mathf.Sin(Mathf.Tau * freq * 2 * t)) * env * Mathf.Exp(-t * 2);
        }
        return Norm(b, gain);
    }

    static float[] Glide(float dur, float f0, float f1, float gain, float vibrato = 0)
    {
        var b = new float[N(dur)];
        float ph = 0;
        for (int i = 0; i < b.Length; i++)
        {
            float t = (float)i / b.Length;
            float fc = f0 * Mathf.Pow(f1 / f0, t) * (1 + 0.03f * Mathf.Sin(Mathf.Tau * vibrato * i / Rate));
            ph += Mathf.Tau * fc / Rate;
            b[i] = (Mathf.Sin(ph) + 0.25f * Mathf.Sin(ph * 1.5f)) * Mathf.Sin(Mathf.Pi * t);
        }
        return Norm(b, gain);
    }

    // เตือนภัย (ท่ากันไม่ได้): เสียงแตรต่ำที่ค่อยๆ เปิดกว้าง
    static float[] Horn(float dur, float freq, float gain)
    {
        var b = new float[N(dur)];
        float y = 0;
        for (int i = 0; i < b.Length; i++)
        {
            float t = (float)i / b.Length;
            float saw = 0;
            for (int h = 1; h <= 12; h++) saw += Mathf.Sin(Mathf.Tau * freq * h * i / Rate * (1 + 0.002f * h)) / h;
            float cut = 300 + 2200 * t;
            y += (1 - Mathf.Exp(-Mathf.Tau * cut / Rate)) * (saw - y);
            b[i] = y * Mathf.Min(1, t * 8) * (t > 0.85f ? (1 - t) / 0.15f : 1);
        }
        return Norm(b, gain);
    }

    static float[] Growl(float dur, float fc, float gain)
    {
        var b = new float[N(dur)];
        var f = new Svf();
        for (int i = 0; i < b.Length; i++)
        {
            float t = (float)i / b.Length;
            float am = 0.55f + 0.45f * Mathf.Sin(Mathf.Tau * 27 * i / Rate);
            b[i] = f.Band(Rnd(), fc * (1 + 0.3f * t), 4f) * am * Mathf.Sin(Mathf.Pi * t);
        }
        return Norm(b, gain);
    }

    static float[] Bubbles(float dur, float gain)
    {
        var b = new float[N(dur)];
        float at = 0;
        while (at < dur - 0.08f)
        {
            float f0 = 180 + (Rnd() + 1) * 160;
            int s = N(at), n = N(0.07f);
            for (int i = 0; i < n && s + i < b.Length; i++)
            {
                float t = (float)i / Rate;
                b[s + i] += Mathf.Sin(Mathf.Tau * f0 * (1 + 6 * t) * t) * Mathf.Exp(-t * 45);
            }
            at += 0.035f + (Rnd() + 1) * 0.03f;
        }
        return Norm(b, gain);
    }

    // ลมพัดในซากวิหาร วนซ้ำได้ (ปลายกับต้นเฟดเข้าหากัน)
    static float[] Wind(float dur, float gain)
    {
        var b = new float[N(dur)];
        var f = new Svf();
        for (int i = 0; i < b.Length; i++)
        {
            float t = (float)i / Rate;
            float fc = 380 + 220 * Mathf.Sin(Mathf.Tau * t / dur) + 90 * Mathf.Sin(Mathf.Tau * 3 * t / dur);
            b[i] = f.Band(Rnd(), fc, 0.7f) + 0.25f * Mathf.Sin(Mathf.Tau * 55 * t) * (0.5f + 0.5f * Mathf.Sin(Mathf.Tau * 2 * t / dur));
        }
        int fade = N(0.5f);
        for (int i = 0; i < fade; i++)
        {
            float k = (float)i / fade;
            b[i] = b[i] * k + b[b.Length - fade + i] * (1 - k);
        }
        Array.Resize(ref b, b.Length - fade);
        return Norm(b, gain);
    }

    static float[] Delay(float[] a, float sec)
    {
        var b = new float[a.Length + N(sec)];
        Array.Copy(a, 0, b, N(sec), a.Length);
        return b;
    }

    static float[] Mix(params float[][] parts)
    {
        int n = 0;
        foreach (var p in parts) n = Math.Max(n, p.Length);
        var b = new float[n];
        foreach (var p in parts)
            for (int i = 0; i < p.Length; i++) b[i] += p[i];
        return Norm(b, 0.9f);
    }

    static float[] Norm(float[] b, float peak)
    {
        float m = 1e-6f;
        foreach (var v in b) m = Mathf.Max(m, Mathf.Abs(v));
        for (int i = 0; i < b.Length; i++) b[i] *= peak / m;
        return b;
    }

    static AudioStreamWav Wav(float[] b, bool loop = false)
    {
        var bytes = new byte[b.Length * 2];
        for (int i = 0; i < b.Length; i++)
        {
            short v = (short)Mathf.Clamp(b[i] * 32767f, -32768f, 32767f);
            bytes[2 * i] = (byte)(v & 0xff);
            bytes[2 * i + 1] = (byte)((v >> 8) & 0xff);
        }
        var w = new AudioStreamWav { Format = AudioStreamWav.FormatEnum.Format16Bits, MixRate = Rate, Stereo = false, Data = bytes };
        if (loop)
        {
            w.LoopMode = AudioStreamWav.LoopModeEnum.Forward;
            w.LoopBegin = 0;
            w.LoopEnd = b.Length;
        }
        return w;
    }

    // state-variable filter (แบบ trapezoidal) ใช้ทำ band-pass
    sealed class Svf
    {
        float _ic1, _ic2;

        public float Band(float x, float fc, float q)
        {
            float g = Mathf.Tan(Mathf.Pi * Mathf.Min(fc, Rate * 0.45f) / Rate);
            float k = 1 / q;
            float a1 = 1 / (1 + g * (g + k)), a2 = g * a1, a3 = g * a2;
            float v3 = x - _ic2;
            float v1 = a1 * _ic1 + a2 * v3;
            float v2 = _ic2 + a2 * _ic1 + a3 * v3;
            _ic1 = 2 * v1 - _ic1;
            _ic2 = 2 * v2 - _ic2;
            return v1;
        }
    }
}

// เล่นเสียงตามตำแหน่งในโลก 3D (เสียงไกลจะเบาลง) และเสียงลมพื้นหลัง
public partial class SoundFx : Node3D
{
    readonly AudioStreamPlayer3D[] _pool = new AudioStreamPlayer3D[24];
    int _next;
    readonly RandomNumberGenerator _rng = new() { Seed = 21 };

    public override void _Ready()
    {
        // ก้องเล็กน้อยให้เหมือนอยู่ในวิหารกลางแจ้ง
        if (AudioServer.GetBusEffectCount(0) == 0)
            AudioServer.AddBusEffect(0, new AudioEffectReverb { RoomSize = 0.55f, Damping = 0.6f, Wet = 0.12f, Dry = 1f });
        for (int i = 0; i < _pool.Length; i++)
        {
            _pool[i] = new AudioStreamPlayer3D { UnitSize = 7f, MaxDistance = 70f, AttenuationModel = AudioStreamPlayer3D.AttenuationModelEnum.InverseDistance };
            AddChild(_pool[i]);
        }
        var wind = new AudioStreamPlayer { Stream = SfxBank.Get("wind"), VolumeDb = -20f, Autoplay = true };
        AddChild(wind);
    }

    public void Play(string id, Vector3 at, float db = 0f, float pitch = 1f, float jitter = 0.06f)
    {
        var p = _pool[_next];
        _next = (_next + 1) % _pool.Length;
        p.Stream = SfxBank.Get(id);
        p.GlobalPosition = at;
        p.VolumeDb = db;
        p.PitchScale = pitch * (1 + _rng.RandfRange(-jitter, jitter));
        p.Play();
    }
}
