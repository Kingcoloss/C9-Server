using System.Globalization;

namespace C9.LocalLauncher;

public enum LauncherLanguage { Thai, English, Chinese }

/// <summary>
/// L9 display text in Thai, English and Simplified Chinese. The Thai source text is the key, so code keeps
/// reading in Thai and c9-auth's Thai error messages translate too; text without an entry is shown as given.
/// </summary>
public static class LauncherText
{
    /// <summary>Language of the running launcher (UI thread only).</summary>
    public static LauncherLanguage Language { get; set; } = LauncherLanguage.Thai;

    public static string Code(LauncherLanguage language) => language switch
    {
        LauncherLanguage.English => "en",
        LauncherLanguage.Chinese => "zh",
        _ => "th",
    };

    public static LauncherLanguage Parse(string? code) => code switch
    {
        "en" => LauncherLanguage.English,
        "zh" => LauncherLanguage.Chinese,
        _ => LauncherLanguage.Thai,
    };

    public static string T(string thai) => T(thai, Language);

    public static string T(string thai, LauncherLanguage language) =>
        language == LauncherLanguage.Thai || !Table.TryGetValue(thai, out (string English, string Chinese) text) ? thai
            : language == LauncherLanguage.English ? text.English : text.Chinese;

    /// <summary><see cref="T(string)"/> of a composite format ({0}, {1}…), then formatted.</summary>
    public static string F(string thai, params object?[] args) => string.Format(CultureInfo.InvariantCulture, T(thai), args);

    public static IReadOnlyDictionary<string, (string English, string Chinese)> Translations => Table;

    private static readonly Dictionary<string, (string English, string Chinese)> Table = new(StringComparer.Ordinal)
    {
        // Launcher screen
        ["เปิดเกมบนเซิร์ฟเวอร์ในเครื่อง"] = ("Play on the local server", "在本地服务器上启动游戏"),
        ["บัญชีผู้เล่น"] = ("Player account", "玩家账号"),
        ["ชื่อผู้ใช้ (3–32 ตัว: A–Z a–z 0–9 _ . -)"] = ("Username (3–32 characters: A–Z a–z 0–9 _ . -)", "用户名（3–32 个字符：A–Z a–z 0–9 _ . -）"),
        ["รหัสผ่าน"] = ("Password", "密码"),
        ["ยืนยันรหัสผ่าน (8 ตัวขึ้นไป)"] = ("Confirm password (8 or more characters)", "确认密码（至少 8 个字符）"),
        ["เข้าสู่ระบบ"] = ("Sign in", "登录"),
        ["ยังไม่มีบัญชี? สมัครสมาชิก"] = ("No account? Register", "没有账号？注册"),
        ["สมัครสมาชิกและเข้าสู่ระบบ"] = ("Register and sign in", "注册并登录"),
        ["มีบัญชีแล้ว? เข้าสู่ระบบ"] = ("Have an account? Sign in", "已有账号？登录"),
        ["ออกจากระบบ"] = ("Sign out", "退出登录"),
        ["เข้าสู่ระบบแล้ว • บัญชีเลขที่ {0} • หมดอายุ {1}"] = ("Signed in • Account {0} • Expires {1}", "已登录 • 账号编号 {0} • 到期 {1}"),
        ["ตั้งค่าเซิร์ฟเวอร์บัญชีไม่ถูกต้อง"] = ("The account server setting is invalid", "账号服务器设置无效"),
        ["รหัสผ่านทั้งสองช่องไม่ตรงกัน"] = ("The two passwords do not match", "两次输入的密码不一致"),
        ["รหัสผ่านต้องยาว 8 ตัวอักษรขึ้นไป"] = ("The password must be 8 or more characters", "密码至少需要 8 个字符"),
        ["เข้าสู่ระบบไม่สำเร็จ"] = ("Sign-in failed", "登录失败"),
        ["ยินดีต้อนรับ {0}"] = ("Welcome, {0}", "欢迎，{0}"),
        ["กด \"เปิดเกม Local\" เพื่อเริ่มเล่น"] = ("Press \"Play (Local)\" to start", "点击“启动游戏（本地）”开始游戏"),
        ["ออกจากระบบแล้ว"] = ("Signed out", "已退出登录"),
        ["เข้าสู่ระบบอีกครั้งเพื่อเปิดเกม"] = ("Sign in again to play", "请重新登录以启动游戏"),
        ["บันทึกเซิร์ฟเวอร์บัญชีแล้ว"] = ("Account server saved", "账号服务器已保存"),
        ["URL เซิร์ฟเวอร์บัญชีไม่ถูกต้อง"] = ("Invalid account server URL", "账号服务器 URL 无效"),
        ["สถานะ"] = ("Status", "状态"),
        ["เริ่มเล่น"] = ("Play", "开始游戏"),
        ["▶  เปิดเกม Local"] = ("▶  Play (Local)", "▶  启动游戏（本地）"),
        ["ตรวจความพร้อม"] = ("Check", "检查状态"),
        ["เกมกำลังทำงาน"] = ("Game running", "游戏运行中"),
        ["Launcher ทำงานด้วยสิทธิ์ Administrator อยู่แล้ว จะเปิด C9.exe ได้ทันที"] =
            ("The launcher already runs as Administrator, so C9.exe starts right away", "启动器已以管理员身份运行，可直接启动 C9.exe"),
        ["กดเปิดเกมแล้ว Windows จะถามสิทธิ์ Administrator (UAC) ให้กด Yes เพราะ C9.exe ต้องใช้สิทธิ์นี้"] =
            ("When you press Play, Windows asks for Administrator rights (UAC). Choose Yes, because C9.exe needs them",
             "点击启动后，Windows 会请求管理员权限（UAC），请选择“是”，因为 C9.exe 需要此权限"),
        ["Status ตัวละคร"] = ("Character status", "角色属性"),
        ["เลือกตัวละคร"] = ("Select a character", "选择角色"),
        ["บันทึก"] = ("Save", "保存"),
        ["บันทึก (ใช้ {0} point)"] = ("Save (use {0} points)", "保存（使用 {0} 点）"),
        ["ล้างที่ยังไม่บันทึก"] = ("Clear unsaved", "清除未保存"),
        ["โหลดใหม่"] = ("Reload", "重新加载"),
        ["ได้ point เมื่อ level ถึงตามตารางของเซิร์ฟเวอร์ ค่าที่บันทึกแล้วมีผลเมื่อเข้าโลกครั้งถัดไป (กลับหน้าเลือกตัวละครแล้วเข้าใหม่) และยังคืน point ไม่ได้"] =
            ("Points are earned when your level reaches the server's table. Saved values apply the next time you enter the world " +
             "(go back to character select and enter again). Points cannot be refunded yet.",
             "等级达到服务器设定时获得点数。已保存的数值在下次进入世界时生效（返回角色选择后重新进入），点数暂时无法退还。"),
        ["บัญชีนี้ยังไม่มีตัวละคร สร้างตัวละครในเกมก่อน"] = ("This account has no characters yet. Create one in the game first.", "此账号还没有角色，请先在游戏中创建角色。"),
        ["Point เหลือ {0} จาก {1}  •  Lv {2} {3}  •  ได้รับทั้งหมด {4} ใช้ไปแล้ว {5}"] =
            ("Points left {0} of {1}  •  Lv {2} {3}  •  Earned {4}, spent {5}", "剩余点数 {0} / {1}  •  Lv {2} {3}  •  共获得 {4}，已使用 {5}"),
        ["โหลด status ไม่สำเร็จ"] = ("Could not load the status", "加载属性失败"),
        ["บันทึก status ไม่สำเร็จ"] = ("Could not save the status", "保存属性失败"),
        ["บันทึกแล้ว ค่าใหม่มีผลเมื่อเข้าโลกครั้งถัดไป"] = ("Saved. The new values apply the next time you enter the world", "已保存，新数值将在下次进入世界时生效"),
        ["โฟลเดอร์เกม (Game Directory)"] = ("Game directory", "游戏文件夹"),
        ["เลือกโฟลเดอร์…"] = ("Browse…", "选择文件夹…"),
        ["ใช้ path นี้"] = ("Use this path", "使用此路径"),
        ["ค่าเริ่มต้น"] = ("Default", "默认"),
        ["พบ C9.exe"] = ("C9.exe found", "已找到 C9.exe"),
        ["ใช้ค่าจาก --client-dir (ชั่วคราว)"] = ("Using --client-dir (temporary)", "使用 --client-dir 的值（临时）"),
        ["ใช้ค่าเริ่มต้นของโปรเจกต์"] = ("Using the project default", "使用项目默认值"),
        ["ใช้โฟลเดอร์ที่เลือกไว้ (จำค่าไว้แล้ว)"] = ("Using the folder you chose (remembered)", "使用已选择的文件夹（已保存）"),
        ["เปิดหน้าต่างเลือกโฟลเดอร์ไม่ได้ (เช่นเมื่อเปิด Launcher แบบ Administrator) กรุณาพิมพ์หรือวาง path แล้วกด \"ใช้ path นี้\""] =
            ("Could not open the folder picker (for example when the launcher runs as Administrator). Type or paste the path and press \"Use this path\"",
             "无法打开文件夹选择窗口（例如以管理员身份运行启动器时），请输入或粘贴路径后点击“使用此路径”"),
        ["เครื่องมือ"] = ("Tools", "工具"),
        ["เปิดโฟลเดอร์เกม"] = ("Open game folder", "打开游戏文件夹"),
        ["เปิด VFUN Launcher (Official)"] = ("Open VFUN Launcher (official)", "打开 VFUN Launcher（官方）"),
        ["ดู Log"] = ("View log", "查看日志"),
        ["อัปเดตเกมผ่าน VFUN Launcher เท่านั้น ตัวเปิดนี้ไม่แก้ไขไฟล์ของเกม"] =
            ("Update the game only through the VFUN Launcher; this launcher never changes game files", "请仅通过 VFUN Launcher 更新游戏，本启动器不会修改游戏文件"),
        ["เซิร์ฟเวอร์บัญชี (c9-auth)"] = ("Account server (c9-auth)", "账号服务器（c9-auth）"),
        ["ใช้ URL นี้"] = ("Use this URL", "使用此 URL"),
        ["บัญชีทดสอบ: test  •  โหมดพัฒนา local server  •  ค่าที่ตั้งไว้ถูกจำใน %LOCALAPPDATA%\\C9Local\\L9"] =
            ("Test account: test  •  local server development mode  •  settings are kept in %LOCALAPPDATA%\\C9Local\\L9",
             "测试账号：test  •  本地服务器开发模式  •  设置保存在 %LOCALAPPDATA%\\C9Local\\L9"),
        ["กรุณาเข้าสู่ระบบก่อน"] = ("Please sign in first", "请先登录"),
        ["เข้าสู่ระบบหรือสมัครสมาชิกในการ์ด \"บัญชีผู้เล่น\" ก่อนเปิดเกม"] =
            ("Sign in or register in the \"Player account\" card before playing", "请先在“玩家账号”中登录或注册，再启动游戏"),
        ["กำลังขอ game ticket…"] = ("Requesting a game ticket…", "正在获取游戏票据…"),
        ["บัญชี {0}"] = ("Account {0}", "账号 {0}"),
        ["ขอ game ticket ไม่สำเร็จ"] = ("Could not get a game ticket", "获取游戏票据失败"),
        ["บันทึกการตั้งค่าไม่สำเร็จ"] = ("Could not save the settings", "保存设置失败"),
        ["ค่าที่เลือกจะใช้ได้จนกว่าจะปิด Launcher"] = ("Your choice applies until the launcher closes", "所选设置在关闭启动器前有效"),

        // Launch workflow (LocalLauncherRuntime)
        ["ปิดเกมก่อนหากต้องการเปลี่ยนโฟลเดอร์"] = ("Close the game first to change the folder", "如需更改文件夹，请先关闭游戏"),
        ["พร้อมเปิดเกม local"] = ("Ready to play (local)", "可以启动本地游戏"),
        ["ตรวจพบ client {0} และเซิร์ฟเวอร์ในเครื่อง"] = ("Found client {0} and the local server", "已检测到客户端 {0} 和本地服务器"),
        ["Client พร้อม • เซิร์ฟเวอร์ยังไม่เปิด"] = ("Client ready • server not running", "客户端就绪 • 服务器未启动"),
        ["กดเปิดเกมเพื่อเริ่มเซิร์ฟเวอร์ในเครื่องโดยอัตโนมัติ"] = ("Press Play to start the local server automatically", "点击启动游戏将自动启动本地服务器"),
        ["เกมกำลังทำงานอยู่"] = ("The game is already running", "游戏已在运行"),
        ["ปิดเกมเดิมก่อนเปิดใหม่"] = ("Close the running game before starting it again", "请先关闭正在运行的游戏再重新启动"),
        ["มีเกม C9 เปิดอยู่ กรุณาปิดเกมก่อนเปิดชุด local"] = ("C9 is already open. Close it before starting the local game", "C9 已在运行，请先关闭后再启动本地游戏"),
        ["กำลังเตรียมเซิร์ฟเวอร์…"] = ("Preparing the server…", "正在准备服务器…"),
        ["เริ่ม login และ character lobby ในเครื่อง"] = ("Starting the local login and character lobby", "正在启动本地登录和角色大厅"),
        ["รอการอนุญาตสิทธิ์ผู้ดูแลระบบ…"] = ("Waiting for Administrator permission…", "正在等待管理员权限…"),
        ["C9.exe ต้องใช้สิทธิ์ Administrator กรุณากด Yes ในหน้าต่าง UAC"] =
            ("C9.exe needs Administrator rights. Choose Yes in the UAC window", "C9.exe 需要管理员权限，请在 UAC 窗口中选择“是”"),
        ["กำลังเริ่มเกม…"] = ("Starting the game…", "正在启动游戏…"),
        ["รอเกมเตรียมระบบ หากเกมปิดตัวลง สามารถเปิดจากหน้านี้อีกครั้งได้"] =
            ("Waiting for the game to get ready. If it closes, you can start it again from here", "正在等待游戏准备就绪，如果游戏关闭，可在此重新启动"),
        ["ยังไม่ได้รับสิทธิ์ผู้ดูแลระบบ"] = ("Administrator permission was not granted", "未获得管理员权限"),
        ["C9.exe ต้องใช้สิทธิ์ Administrator กดเปิดเกมอีกครั้งแล้วเลือก Yes ในหน้าต่าง UAC"] =
            ("C9.exe needs Administrator rights. Press Play again and choose Yes in the UAC window", "C9.exe 需要管理员权限，请再次点击启动游戏并在 UAC 窗口中选择“是”"),
        ["กรุณาปิดเกมก่อนเปิด VFUN Launcher"] = ("Close the game before opening the VFUN Launcher", "请先关闭游戏再打开 VFUN Launcher"),
        ["กำลังรอเกมเตรียมระบบ…"] = ("Waiting for the game to get ready…", "正在等待游戏准备就绪…"),
        ["หากรอนาน กรุณาตรวจข้อความในหน้าต่าง C9 รวมถึงข้อผิดพลาดจาก GameGuard"] =
            ("If this takes long, check the messages in the C9 window, including GameGuard errors", "如果等待时间过长，请检查 C9 窗口中的提示，包括 GameGuard 错误"),
        ["เกมปิดตัวลงแล้ว"] = ("The game has closed", "游戏已关闭"),
        ["ดูรายละเอียดการเริ่มเกมใน L9.local.log และการเชื่อมต่อใน L9.server.log"] =
            ("See L9.local.log for the game start and L9.server.log for the connection", "游戏启动详情见 L9.local.log，连接详情见 L9.server.log"),
        ["เปิดเซิร์ฟเวอร์พอร์ต {0} ไม่สำเร็จ กรุณาตรวจ {1}"] = ("Could not start the server on port {0}. Check {1}", "无法在端口 {0} 启动服务器，请检查 {1}"),
        ["ยังเปิดเกม local ไม่ได้"] = ("Cannot start the local game yet", "暂时无法启动本地游戏"),
        ["C9.exe รุ่นนี้ยังไม่มี profile ที่รองรับ (ระบบรองรับ Client รุ่น 14 พ.ย. 2026) กรุณาอัปเดต L9"] =
            ("This C9.exe build has no supported profile (L9 supports the 14 Nov 2026 Client). Please update L9",
             "此版本的 C9.exe 没有可用的配置（L9 支持 2026 年 11 月 14 日的客户端），请更新 L9"),
        ["Windows ไม่อนุญาตให้เปิดโปรแกรม: {0}"] = ("Windows did not allow the program to start: {0}", "Windows 不允许启动该程序：{0}"),
        ["กรุณาตรวจไฟล์ติดตั้งและ L9.local.log"] = ("Please check the installed files and L9.local.log", "请检查安装文件和 L9.local.log"),

        // Game folder checks (LauncherUserSettings)
        ["กรุณาเลือกโฟลเดอร์เกม"] = ("Please choose the game folder", "请选择游戏文件夹"),
        ["ต้องเป็น path เต็ม เช่น C:\\VFUN\\C9-GL"] = ("Use a full path, for example C:\\VFUN\\C9-GL", "请使用完整路径，例如 C:\\VFUN\\C9-GL"),
        ["ไม่พบโฟลเดอร์นี้"] = ("This folder does not exist", "找不到此文件夹"),
        ["ไม่พบ C9.exe ในโฟลเดอร์นี้"] = ("C9.exe is not in this folder", "此文件夹中没有 C9.exe"),

        // Account server client (LauncherAuthClient)
        ["URL เซิร์ฟเวอร์บัญชีไม่ถูกต้อง ตัวอย่าง: http://127.0.0.1:14890"] =
            ("Invalid account server URL. Example: http://127.0.0.1:14890", "账号服务器 URL 无效。示例：http://127.0.0.1:14890"),
        ["เซิร์ฟเวอร์บัญชีที่อยู่นอกเครื่องต้องใช้ https เพื่อไม่ให้รหัสผ่านถูกส่งแบบไม่เข้ารหัส"] =
            ("An account server on another computer must use https so passwords are never sent unencrypted", "其他电脑上的账号服务器必须使用 https，以免密码以未加密方式发送"),
        ["เซิร์ฟเวอร์บัญชีตอบกลับไม่ถูกต้อง"] = ("The account server sent an invalid reply", "账号服务器返回了无效的响应"),
        ["เซิร์ฟเวอร์บัญชีตอบกลับ HTTP {0}"] = ("The account server replied HTTP {0}", "账号服务器返回 HTTP {0}"),
        ["ติดต่อเซิร์ฟเวอร์บัญชีไม่ได้ ({0}) ตรวจว่าเริ่ม platform ด้วย -GameData แล้ว"] =
            ("Cannot reach the account server ({0}). Check that the platform was started with -GameData", "无法连接账号服务器（{0}），请确认平台已使用 -GameData 启动"),

        // Messages c9-auth sends (src/C9.AuthService/Program.cs)
        ["ลองหลายครั้งเกินไป กรุณารอ 1 นาทีแล้วลองใหม่"] = ("Too many attempts. Wait 1 minute and try again", "尝试次数过多，请等待 1 分钟后重试"),
        ["ปิดรับสมัครสมาชิกใหม่อยู่"] = ("Registration is closed", "暂停注册新账号"),
        ["ชื่อผู้ใช้นี้ถูกใช้แล้ว"] = ("This username is already taken", "此用户名已被使用"),
        ["รหัสผ่านต้องยาว 8–128 ตัวอักษร"] = ("The password must be 8–128 characters", "密码长度必须为 8–128 个字符"),
        ["ชื่อผู้ใช้ต้องยาว 3–32 ตัว ใช้ได้เฉพาะ A–Z a–z 0–9 _ . -"] =
            ("The username must be 3–32 characters using only A–Z a–z 0–9 _ . -", "用户名长度必须为 3–32 个字符，只能使用 A–Z a–z 0–9 _ . -"),
        ["บัญชีนี้ถูกแบน"] = ("This account is banned", "此账号已被封禁"),
        ["บัญชีนี้ถูกระงับชั่วคราว"] = ("This account is temporarily suspended", "此账号已被暂时停用"),
        ["ชื่อผู้ใช้หรือรหัสผ่านไม่ถูกต้อง"] = ("Wrong username or password", "用户名或密码错误"),
        ["หมดเวลาเข้าสู่ระบบ กรุณาเข้าสู่ระบบใหม่"] = ("Your session has expired. Please sign in again", "登录已过期，请重新登录"),
        ["ข้อมูลการอัป status ไม่ถูกต้อง"] = ("The status upgrade data is invalid", "属性升级数据无效"),
        ["ไม่พบตัวละครหรือ status นี้"] = ("Character or status not found", "找不到该角色或属性"),
        ["status นี้ยังไม่เปิดให้อัป"] = ("This status cannot be upgraded yet", "该属性暂时无法升级"),
        ["ข้อมูลเปลี่ยนระหว่างบันทึก กรุณาโหลดใหม่"] = ("The data changed while saving. Please reload", "保存时数据已变更，请重新加载"),
        ["point ไม่พอหรือจำนวนไม่ถูกต้อง"] = ("Not enough points or an invalid amount", "点数不足或数量无效"),
        ["ระบบบัญชียังไม่ได้เปิด (เริ่ม platform ด้วย -GameData)"] =
            ("The account system is not running (start the platform with -GameData)", "账号系统尚未启动（请使用 -GameData 启动平台）"),
        ["กรุณากรอกชื่อผู้ใช้และรหัสผ่าน"] = ("Please enter a username and password", "请输入用户名和密码"),
    };
}
