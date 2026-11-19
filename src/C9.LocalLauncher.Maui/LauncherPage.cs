using C9.Contracts;
using C9.LocalLauncher;
using Microsoft.Maui.Controls.Shapes;

namespace C9.LocalLauncher.Maui;

/// <summary>
/// Responsive launcher screen. Wide windows show two columns (status/actions | settings/tools);
/// narrow windows stack everything in one scrolling column.
/// </summary>
public sealed class LauncherPage : ContentPage
{
    private const double WideBreakpoint = 760;
    private static readonly Color Ink = Color.FromArgb("#11171F");
    private static readonly Color CardFill = Color.FromArgb("#1A2331");
    private static readonly Color Field = Color.FromArgb("#0D131B");
    private static readonly Color Muted = Color.FromArgb("#2A3648");
    private static readonly Color Gold = Color.FromArgb("#E8BE68");
    private static readonly Color TextTone = Color.FromArgb("#F2F4F7");
    private static readonly Color Subtle = Color.FromArgb("#9AA5B4");
    private static readonly Color Good = Color.FromArgb("#7BD88F");
    private static readonly Color Bad = Color.FromArgb("#F08A7E");

    private readonly LauncherStartup startup;
    private readonly LocalLauncherRuntime runtime;
    private LauncherUserSettings settings;
    private string gameDirectory;
    private bool compatibilityTest;
    private bool busy;
    private bool initialized;
    private bool? wideLayout;

    private readonly Grid columns = new() { ColumnSpacing = 20, RowSpacing = 20 };
    private readonly VerticalStackLayout primaryColumn = new() { Spacing = 20 };
    private readonly VerticalStackLayout settingsColumn = new() { Spacing = 20 };
    private readonly Ellipse statusDot = new() { WidthRequest = 12, HeightRequest = 12, Fill = new SolidColorBrush(Color.FromArgb("#9AA5B4")), VerticalOptions = LayoutOptions.Center };
    private readonly Label statusTitle = new() { FontSize = 20, FontAttributes = FontAttributes.Bold, TextColor = TextTone, LineBreakMode = LineBreakMode.WordWrap };
    private readonly Label statusDetail = new() { FontSize = 14, TextColor = Subtle, LineBreakMode = LineBreakMode.WordWrap };
    private readonly Label clientInfo = new() { FontSize = 12, TextColor = Subtle, LineBreakMode = LineBreakMode.TailTruncation };
    private readonly ActivityIndicator activity = new() { Color = Gold, WidthRequest = 22, HeightRequest = 22, IsRunning = false, IsVisible = false };
    private readonly Label modeBadge = new() { FontSize = 12, FontAttributes = FontAttributes.Bold, TextColor = Ink };
    private readonly Border modeBadgeFrame;
    private readonly Button play;
    private readonly Button check;
    private readonly Entry directoryEntry = new() { FontSize = 14, TextColor = TextTone, PlaceholderColor = Subtle, BackgroundColor = Field, Placeholder = LauncherUserSettings.FallbackGameDirectory };
    private readonly Label directoryState = new() { FontSize = 13, LineBreakMode = LineBreakMode.WordWrap };
    private readonly Label directorySource = new() { FontSize = 12, TextColor = Subtle, LineBreakMode = LineBreakMode.WordWrap };
    private readonly Switch compatibilitySwitch = new() { OnColor = Gold, VerticalOptions = LayoutOptions.Center };
    private readonly List<View> lockWhileBusy = [];

    // Account (c9-auth). The session token lives in memory only; the password is cleared after each attempt.
    private LauncherSession? session;
    private bool registerMode;
    private readonly Entry loginEntry = new() { FontSize = 14, TextColor = TextTone, PlaceholderColor = Subtle, BackgroundColor = Field, Placeholder = "ชื่อผู้ใช้ (3–32 ตัว: A–Z a–z 0–9 _ . -)", MaxLength = 32 };
    private readonly Entry passwordEntry = new() { FontSize = 14, TextColor = TextTone, PlaceholderColor = Subtle, BackgroundColor = Field, Placeholder = "รหัสผ่าน", IsPassword = true, MaxLength = 128 };
    private readonly Entry confirmEntry = new() { FontSize = 14, TextColor = TextTone, PlaceholderColor = Subtle, BackgroundColor = Field, Placeholder = "ยืนยันรหัสผ่าน (8 ตัวขึ้นไป)", IsPassword = true, MaxLength = 128, IsVisible = false };
    private readonly Entry authUrlEntry = new() { FontSize = 13, TextColor = TextTone, PlaceholderColor = Subtle, BackgroundColor = Field, Placeholder = LauncherAuthClient.DefaultUrl };
    private readonly Label accountMessage = new() { FontSize = 13, LineBreakMode = LineBreakMode.WordWrap, IsVisible = false };
    private readonly Label signedInName = new() { FontSize = 18, FontAttributes = FontAttributes.Bold, TextColor = TextTone, VerticalOptions = LayoutOptions.Center, LineBreakMode = LineBreakMode.TailTruncation };
    private readonly Label accountCaption = new() { FontSize = 12, TextColor = Subtle, LineBreakMode = LineBreakMode.WordWrap };
    private readonly VerticalStackLayout signedOutView = new() { Spacing = 10 };
    private readonly VerticalStackLayout signedInView = new() { Spacing = 6, IsVisible = false };
    private Button submitAccount = null!;
    private Button switchMode = null!;

    public LauncherPage(LauncherStartup startup)
    {
        this.startup = startup;
        runtime = new LocalLauncherRuntime(startup.LauncherDirectory, startup.Workspace);
        runtime.StatusChanged += status => MainThread.BeginInvokeOnMainThread(() =>
        {
            ShowStatus(status);
            UpdateButtons();
        });
        settings = LauncherUserSettings.Load();
        loginEntry.Text = settings.LastLogin;
        authUrlEntry.Text = settings.AuthUrl ?? LauncherAuthClient.DefaultUrl;
        gameDirectory = startup.ClientDirectoryOverride ?? settings.ResolveGameDirectory(startup.LauncherDirectory, startup.Workspace);
        compatibilityTest = startup.CompatibilityOverride ?? settings.CompatibilityTest;

        BackgroundColor = Ink;
        play = MakeButton("▶  เปิดเกม Local", Gold, Ink, primary: true);
        check = MakeButton("ตรวจความพร้อม", Muted, TextTone, primary: true);
        play.Clicked += async (_, _) => await StartGameAsync();
        check.Clicked += async (_, _) => await CheckAsync();
        modeBadgeFrame = new Border
        {
            BackgroundColor = Gold, StrokeThickness = 0, Padding = new Thickness(10, 4),
            StrokeShape = new RoundRectangle { CornerRadius = 10 }, Content = modeBadge,
            HorizontalOptions = LayoutOptions.Start, VerticalOptions = LayoutOptions.Center,
        };

        primaryColumn.Children.Add(BuildAccountCard());
        primaryColumn.Children.Add(BuildStatusCard());
        primaryColumn.Children.Add(BuildActionsCard());
        settingsColumn.Children.Add(BuildDirectoryCard());
        settingsColumn.Children.Add(BuildModeCard());
        settingsColumn.Children.Add(BuildToolsCard());
        columns.Children.Add(primaryColumn);
        columns.Children.Add(settingsColumn);

        VerticalStackLayout body = new()
        {
            Padding = new Thickness(24, 24, 24, 20), Spacing = 20, MaximumWidthRequest = 1160,
            HorizontalOptions = LayoutOptions.Fill,
            Children = { BuildHeader(), columns, BuildFooter() },
        };
        Content = new ScrollView { Content = body };
        ApplyLayout(980);
        SizeChanged += (_, _) => { if (Width > 0) ApplyLayout(Width); };

        directoryEntry.Text = gameDirectory;
        compatibilitySwitch.IsToggled = compatibilityTest;
        compatibilitySwitch.Toggled += async (_, e) => await SetCompatibilityAsync(e.Value);
        directoryEntry.Completed += async (_, _) => await ApplyDirectoryAsync(directoryEntry.Text, remember: true);
        UpdateMode();
        UpdateDirectoryState();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        if (initialized) return;
        initialized = true;
        UpdateMode();
        await CheckAsync();
    }

    // ----- layout -----

    private void ApplyLayout(double width)
    {
        bool wide = width >= WideBreakpoint;
        if (wideLayout == wide) return;
        wideLayout = wide;
        columns.ColumnDefinitions.Clear();
        columns.RowDefinitions.Clear();
        if (wide)
        {
            columns.ColumnDefinitions.Add(new ColumnDefinition(new GridLength(1.15, GridUnitType.Star)));
            columns.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Star));
            columns.RowDefinitions.Add(new RowDefinition(GridLength.Auto));
            Grid.SetColumn(primaryColumn, 0); Grid.SetRow(primaryColumn, 0);
            Grid.SetColumn(settingsColumn, 1); Grid.SetRow(settingsColumn, 0);
        }
        else
        {
            columns.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Star));
            columns.RowDefinitions.Add(new RowDefinition(GridLength.Auto));
            columns.RowDefinitions.Add(new RowDefinition(GridLength.Auto));
            Grid.SetColumn(primaryColumn, 0); Grid.SetRow(primaryColumn, 0);
            Grid.SetColumn(settingsColumn, 0); Grid.SetRow(settingsColumn, 1);
        }
    }

    private View BuildHeader()
    {
        Label title = new() { Text = "C9  LOCAL", FontSize = 34, FontAttributes = FontAttributes.Bold, TextColor = Gold, CharacterSpacing = 2 };
        Label subtitle = new() { Text = "เปิดเกมบนเซิร์ฟเวอร์ในเครื่อง", FontSize = 15, TextColor = Subtle };
        Grid header = new()
        {
            ColumnDefinitions = { new ColumnDefinition(GridLength.Star), new ColumnDefinition(GridLength.Auto) },
            RowDefinitions = { new RowDefinition(GridLength.Auto), new RowDefinition(GridLength.Auto) },
            ColumnSpacing = 12,
        };
        header.Add(title, 0, 0);
        header.Add(subtitle, 0, 1);
        header.Add(modeBadgeFrame, 1, 0);
        return header;
    }

    private View BuildAccountCard()
    {
        submitAccount = MakeButton("เข้าสู่ระบบ", Gold, Ink, primary: true);
        switchMode = MakeButton("ยังไม่มีบัญชี? สมัครสมาชิก", Muted, TextTone);
        Button signOut = MakeButton("ออกจากระบบ", Muted, TextTone);
        submitAccount.Clicked += async (_, _) => await SubmitAccountAsync();
        passwordEntry.Completed += async (_, _) => { if (!registerMode) await SubmitAccountAsync(); };
        confirmEntry.Completed += async (_, _) => await SubmitAccountAsync();
        switchMode.Clicked += (_, _) => { registerMode = !registerMode; accountMessage.IsVisible = false; UpdateAccountView(); };
        signOut.Clicked += async (_, _) => await SignOutAsync();
        lockWhileBusy.AddRange([submitAccount, switchMode, signOut, loginEntry, passwordEntry, confirmEntry]);
        submitAccount.HorizontalOptions = LayoutOptions.Fill;
        signedOutView.Children.Add(FieldFrame(loginEntry));
        signedOutView.Children.Add(FieldFrame(passwordEntry));
        signedOutView.Children.Add(FieldFrame(confirmEntry));
        signedOutView.Children.Add(accountMessage);
        signedOutView.Children.Add(submitAccount);
        signedOutView.Children.Add(Wrap(switchMode));
        Grid signedInRow = new() { ColumnDefinitions = { new ColumnDefinition(GridLength.Star), new ColumnDefinition(GridLength.Auto) }, ColumnSpacing = 12 };
        signedInRow.Add(signedInName, 0, 0);
        signedInRow.Add(signOut, 1, 0);
        signedInView.Children.Add(signedInRow);
        signedInView.Children.Add(accountCaption);
        UpdateAccountView();
        return MakeCard("บัญชีผู้เล่น", new VerticalStackLayout { Spacing = 0, Children = { signedOutView, signedInView } });
    }

    private static Border FieldFrame(View content) => new()
    {
        BackgroundColor = Field, StrokeThickness = 1, Stroke = new SolidColorBrush(Muted),
        StrokeShape = new RoundRectangle { CornerRadius = 8 }, Padding = new Thickness(6, 2), Content = content,
        IsVisible = content.IsVisible,
    };

    private void UpdateAccountView()
    {
        bool signedIn = session is not null;
        signedOutView.IsVisible = !signedIn;
        signedInView.IsVisible = signedIn;
        confirmEntry.IsVisible = registerMode;
        if (confirmEntry.Parent is View frame) frame.IsVisible = registerMode;
        submitAccount.Text = registerMode ? "สมัครสมาชิกและเข้าสู่ระบบ" : "เข้าสู่ระบบ";
        switchMode.Text = registerMode ? "มีบัญชีแล้ว? เข้าสู่ระบบ" : "ยังไม่มีบัญชี? สมัครสมาชิก";
        if (session is { } current)
        {
            signedInName.Text = current.Login;
            accountCaption.Text = $"เข้าสู่ระบบแล้ว • บัญชีเลขที่ {current.AccountId} • หมดอายุ {current.ExpiresAt.ToLocalTime():HH:mm}";
        }
        UpdateButtons();
    }

    private void ShowAccountMessage(string text, bool error)
    {
        accountMessage.Text = (error ? "✕  " : "✓  ") + text;
        accountMessage.TextColor = error ? Bad : Good;
        accountMessage.IsVisible = true;
    }

    private LauncherAuthClient? CreateAuthClient()
    {
        try { return new LauncherAuthClient(settings.AuthUrl ?? LauncherAuthClient.DefaultUrl); }
        catch (ArgumentException error)
        {
            ShowAccountMessage(error.Message, error: true);
            ShowStatus(new LauncherStatus(LauncherTone.Error, "ตั้งค่าเซิร์ฟเวอร์บัญชีไม่ถูกต้อง", error.Message));
            return null;
        }
    }

    private async Task SubmitAccountAsync()
    {
        if (busy) return;
        string login = loginEntry.Text?.Trim() ?? "";
        string password = passwordEntry.Text ?? "";
        if (login.Length == 0 || password.Length == 0) { ShowAccountMessage("กรุณากรอกชื่อผู้ใช้และรหัสผ่าน", error: true); return; }
        if (registerMode && password != (confirmEntry.Text ?? "")) { ShowAccountMessage("รหัสผ่านทั้งสองช่องไม่ตรงกัน", error: true); return; }
        if (registerMode && password.Length < 8) { ShowAccountMessage("รหัสผ่านต้องยาว 8 ตัวอักษรขึ้นไป", error: true); return; }
        SetBusy(true);
        try
        {
            using LauncherAuthClient? client = CreateAuthClient();
            if (client is null) return;
            AuthResult<LauncherSession> result = registerMode
                ? await client.RegisterAsync(login, password)
                : await client.LoginAsync(login, password);
            if (!result.Ok)
            {
                ShowAccountMessage(result.ErrorMessage ?? "เข้าสู่ระบบไม่สำเร็จ", error: true);
                runtime.Log($"account {(registerMode ? "register" : "login")} failed login={login} code={result.ErrorCode}");
                return;
            }
            session = result.Value;
            runtime.Log($"account {(registerMode ? "register" : "login")} ok login={session!.Login} account={session.AccountId}");
            registerMode = false;
            accountMessage.IsVisible = false;
            Remember(settings with { LastLogin = session.Login });
            ShowStatus(new LauncherStatus(LauncherTone.Good, $"ยินดีต้อนรับ {session.Login}", "กด \"เปิดเกม Local\" เพื่อเริ่มเล่น"));
        }
        finally
        {
            passwordEntry.Text = "";
            confirmEntry.Text = "";
            SetBusy(false);
            UpdateAccountView();
        }
    }

    private async Task SignOutAsync()
    {
        if (session is { } current)
        {
            using LauncherAuthClient? client = CreateAuthClient();
            if (client is not null) await client.LogoutAsync(current.SessionToken);
            runtime.Log($"account logout login={current.Login}");
        }
        session = null;
        UpdateAccountView();
        ShowStatus(new LauncherStatus(LauncherTone.Neutral, "ออกจากระบบแล้ว", "เข้าสู่ระบบอีกครั้งเพื่อเปิดเกม"));
    }

    private void SaveAuthUrl()
    {
        try
        {
            Uri url = LauncherAuthClient.ValidateUrl(authUrlEntry.Text);
            string value = url.GetLeftPart(UriPartial.Authority);
            Remember(settings with { AuthUrl = value == LauncherAuthClient.DefaultUrl ? null : value });
            authUrlEntry.Text = value;
            ShowStatus(new LauncherStatus(LauncherTone.Good, "บันทึกเซิร์ฟเวอร์บัญชีแล้ว", value));
        }
        catch (ArgumentException error)
        {
            ShowStatus(new LauncherStatus(LauncherTone.Error, "URL เซิร์ฟเวอร์บัญชีไม่ถูกต้อง", error.Message));
        }
    }

    private View BuildStatusCard()
    {
        Grid heading = new()
        {
            ColumnDefinitions = { new ColumnDefinition(GridLength.Auto), new ColumnDefinition(GridLength.Star), new ColumnDefinition(GridLength.Auto) },
            ColumnSpacing = 10,
        };
        heading.Add(statusDot, 0, 0);
        heading.Add(statusTitle, 1, 0);
        heading.Add(activity, 2, 0);
        return MakeCard("สถานะ", new VerticalStackLayout { Spacing = 8, Children = { heading, statusDetail, clientInfo } });
    }

    private View BuildActionsCard()
    {
        Grid actions = new()
        {
            ColumnDefinitions = { new ColumnDefinition(GridLength.Star), new ColumnDefinition(GridLength.Auto) },
            ColumnSpacing = 12,
        };
        actions.Add(play, 0, 0);
        actions.Add(check, 1, 0);
        Label hint = new()
        {
            FontSize = 12, TextColor = Subtle, LineBreakMode = LineBreakMode.WordWrap,
            Text = LocalLauncherRuntime.IsElevated
                ? "Launcher ทำงานด้วยสิทธิ์ Administrator อยู่แล้ว จะเปิด C9.exe ได้ทันที"
                : "กดเปิดเกมแล้ว Windows จะถามสิทธิ์ Administrator (UAC) ให้กด Yes เพราะ C9.exe ต้องใช้สิทธิ์นี้",
        };
        return MakeCard("เริ่มเล่น", new VerticalStackLayout { Spacing = 12, Children = { actions, hint } });
    }

    private View BuildDirectoryCard()
    {
        Button browse = MakeButton("เลือกโฟลเดอร์…", Gold, Ink);
        Button save = MakeButton("ใช้ path นี้", Muted, TextTone);
        Button reset = MakeButton("ค่าเริ่มต้น", Muted, TextTone);
        browse.Clicked += async (_, _) => await BrowseAsync();
        save.Clicked += async (_, _) => await ApplyDirectoryAsync(directoryEntry.Text, remember: true);
        reset.Clicked += async (_, _) => await ResetDirectoryAsync();
        lockWhileBusy.AddRange([browse, save, reset, directoryEntry]);
        Border field = new()
        {
            BackgroundColor = Field, StrokeThickness = 1, Stroke = new SolidColorBrush(Muted),
            StrokeShape = new RoundRectangle { CornerRadius = 8 }, Padding = new Thickness(6, 2), Content = directoryEntry,
        };
        return MakeCard("โฟลเดอร์เกม (Game Directory)", new VerticalStackLayout
        {
            Spacing = 10,
            Children = { field, directoryState, Wrap(browse, save, reset), directorySource },
        });
    }

    private View BuildModeCard()
    {
        Label name = new() { Text = "ทดสอบ Client รุ่นใหม่ (Local)", FontSize = 15, FontAttributes = FontAttributes.Bold, TextColor = TextTone, VerticalOptions = LayoutOptions.Center, LineBreakMode = LineBreakMode.WordWrap };
        Label note = new()
        {
            Text = "เปิด C9.exe รุ่น 14 พ.ย. 2026 ด้วยรูปแบบ argument ของ Build 55350 (โหมดทดลอง ระบบจะจำค่าที่เลือกไว้)",
            FontSize = 12, TextColor = Subtle, LineBreakMode = LineBreakMode.WordWrap,
        };
        Grid row = new() { ColumnDefinitions = { new ColumnDefinition(GridLength.Star), new ColumnDefinition(GridLength.Auto) }, ColumnSpacing = 12 };
        row.Add(name, 0, 0);
        row.Add(compatibilitySwitch, 1, 0);
        lockWhileBusy.Add(compatibilitySwitch);
        return MakeCard("โหมดการเปิดเกม", new VerticalStackLayout { Spacing = 6, Children = { row, note } });
    }

    private View BuildToolsCard()
    {
        Button folder = MakeButton("เปิดโฟลเดอร์เกม", Muted, TextTone);
        Button official = MakeButton("เปิด VFUN Launcher (Official)", Muted, TextTone);
        Button log = MakeButton("ดู Log", Muted, TextTone);
        folder.Clicked += (_, _) => ShowIfFailed(runtime.OpenShell(gameDirectory, Directory.Exists(gameDirectory) ? gameDirectory : null));
        official.Clicked += (_, _) => ShowIfFailed(runtime.OpenOfficialLauncher());
        log.Clicked += (_, _) => ShowIfFailed(File.Exists(runtime.LogPath) ? runtime.OpenShell(runtime.LogPath) : runtime.OpenShell(runtime.ConfigurationDirectory));
        Label note = new() { Text = "อัปเดตเกมผ่าน VFUN Launcher เท่านั้น ตัวเปิดนี้ไม่แก้ไขไฟล์ของเกม", FontSize = 12, TextColor = Subtle, LineBreakMode = LineBreakMode.WordWrap };
        Button saveAuth = MakeButton("ใช้ URL นี้", Muted, TextTone);
        saveAuth.Clicked += (_, _) => SaveAuthUrl();
        lockWhileBusy.AddRange([saveAuth, authUrlEntry]);
        Label authCaption = new() { Text = "เซิร์ฟเวอร์บัญชี (c9-auth)", FontSize = 12, TextColor = Subtle };
        Border authField = new()
        {
            BackgroundColor = Field, StrokeThickness = 1, Stroke = new SolidColorBrush(Muted),
            StrokeShape = new RoundRectangle { CornerRadius = 8 }, Padding = new Thickness(6, 2), Content = authUrlEntry,
        };
        return MakeCard("เครื่องมือ", new VerticalStackLayout { Spacing = 8, Children = { Wrap(folder, official, log), note, authCaption, authField, Wrap(saveAuth) } });
    }

    private static View BuildFooter() => new Label
    {
        Text = "บัญชีทดสอบ: test  •  โหมดพัฒนา local server  •  ค่าที่ตั้งไว้ถูกจำใน %LOCALAPPDATA%\\C9Local\\L9",
        FontSize = 11, TextColor = Color.FromArgb("#6B7585"), LineBreakMode = LineBreakMode.WordWrap,
    };

    private static Border MakeCard(string caption, View content) => new()
    {
        BackgroundColor = CardFill, StrokeThickness = 0, StrokeShape = new RoundRectangle { CornerRadius = 14 },
        Padding = new Thickness(20, 16, 20, 18),
        Content = new VerticalStackLayout
        {
            Spacing = 12,
            Children = { new Label { Text = caption.ToUpperInvariant(), FontSize = 12, FontAttributes = FontAttributes.Bold, TextColor = Gold, CharacterSpacing = 1 }, content },
        },
    };

    private static Button MakeButton(string text, Color background, Color foreground, bool primary = false) => new()
    {
        Text = text, BackgroundColor = background, TextColor = foreground, CornerRadius = 8,
        FontSize = primary ? 17 : 13, FontAttributes = primary ? FontAttributes.Bold : FontAttributes.None,
        HeightRequest = primary ? 56 : 38, Padding = new Thickness(14, 6), BorderWidth = 0,
    };

    private static FlexLayout Wrap(params View[] views)
    {
        FlexLayout layout = new() { Wrap = Microsoft.Maui.Layouts.FlexWrap.Wrap, JustifyContent = Microsoft.Maui.Layouts.FlexJustify.Start };
        foreach (View view in views)
        {
            view.Margin = new Thickness(0, 0, 8, 8);
            layout.Children.Add(view);
        }
        return layout;
    }

    // ----- behaviour -----

    private async Task CheckAsync()
    {
        if (busy) return;
        SetBusy(true);
        try { ShowStatus(await runtime.CheckAsync(gameDirectory, compatibilityTest)); }
        finally { SetBusy(false); }
    }

    private async Task StartGameAsync()
    {
        if (busy) return;
        SetBusy(true);
        try
        {
            // Login is mandatory: the game only starts with a one-time ticket from c9-auth (no anonymous/lab path in L9).
            if (session is not { } current)
            {
                ShowStatus(new LauncherStatus(LauncherTone.Warning, "กรุณาเข้าสู่ระบบก่อน", "เข้าสู่ระบบหรือสมัครสมาชิกในการ์ด \"บัญชีผู้เล่น\" ก่อนเปิดเกม"));
                return;
            }
            ShowStatus(new LauncherStatus(LauncherTone.Neutral, "กำลังขอ game ticket…", $"บัญชี {current.Login}"));
            using LauncherAuthClient? client = CreateAuthClient();
            if (client is null) return;
            AuthResult<LauncherTicket> ticket = await client.TicketAsync(current.SessionToken);
            if (!ticket.Ok)
            {
                if (ticket.ErrorCode is "session_expired") { session = null; UpdateAccountView(); }
                ShowStatus(new LauncherStatus(LauncherTone.Error, "ขอ game ticket ไม่สำเร็จ", ticket.ErrorMessage ?? ""));
                return;
            }
            LaunchIdentity identity = LaunchIdentity.From(ticket.Value!);
            Progress<LauncherStatus> progress = new(ShowStatus);
            ShowStatus(await runtime.StartGameAsync(gameDirectory, compatibilityTest, progress, identity));
        }
        finally { SetBusy(false); }
    }

    private async Task SetCompatibilityAsync(bool value)
    {
        if (value == compatibilityTest) return;
        compatibilityTest = value;
        Remember(settings with { CompatibilityTest = value });
        UpdateMode();
        await CheckAsync();
    }

    private async Task ApplyDirectoryAsync(string? path, bool remember)
    {
        string? problem = LauncherUserSettings.ValidateGameDirectory(path);
        if (problem is not null)
        {
            directoryState.Text = "✕  " + problem;
            directoryState.TextColor = Bad;
            return;
        }
        gameDirectory = LauncherUserSettings.NormalizeDirectory(path!);
        directoryEntry.Text = gameDirectory;
        if (remember) Remember(settings with { GameDirectory = gameDirectory });
        UpdateDirectoryState();
        await CheckAsync();
    }

    private async Task ResetDirectoryAsync()
    {
        Remember(settings with { GameDirectory = null });
        gameDirectory = LauncherUserSettings.DefaultGameDirectory(startup.LauncherDirectory, startup.Workspace);
        directoryEntry.Text = gameDirectory;
        UpdateDirectoryState();
        await CheckAsync();
    }

    private async Task BrowseAsync()
    {
        try
        {
            string? picked = await PickFolderAsync();
            if (picked is not null) await ApplyDirectoryAsync(picked, remember: true);
        }
        catch (Exception error)
        {
            runtime.Log($"folder picker failed: {error.GetType().Name}: {error.Message}");
            directoryState.Text = "✕  เปิดหน้าต่างเลือกโฟลเดอร์ไม่ได้ (เช่นเมื่อเปิด Launcher แบบ Administrator) กรุณาพิมพ์หรือวาง path แล้วกด \"ใช้ path นี้\"";
            directoryState.TextColor = Bad;
        }
    }

    private async Task<string?> PickFolderAsync()
    {
#if WINDOWS
        global::Windows.Storage.Pickers.FolderPicker picker = new()
        {
            SuggestedStartLocation = global::Windows.Storage.Pickers.PickerLocationId.ComputerFolder,
        };
        picker.FileTypeFilter.Add("*");
        if (Window?.Handler?.PlatformView is global::Microsoft.UI.Xaml.Window native)
            global::WinRT.Interop.InitializeWithWindow.Initialize(picker, global::WinRT.Interop.WindowNative.GetWindowHandle(native));
        global::Windows.Storage.StorageFolder? folder = await picker.PickSingleFolderAsync();
        return folder?.Path;
#else
        await Task.CompletedTask;
        return null;
#endif
    }

    private void Remember(LauncherUserSettings next)
    {
        try
        {
            next.Save();
            settings = next;
        }
        catch (Exception error) when (error is IOException or UnauthorizedAccessException or ArgumentException)
        {
            runtime.Log($"settings save failed: {error.GetType().Name}: {error.Message}");
            ShowStatus(new LauncherStatus(LauncherTone.Warning, "บันทึกการตั้งค่าไม่สำเร็จ", "ค่าที่เลือกจะใช้ได้จนกว่าจะปิด Launcher"));
        }
    }

    private void UpdateDirectoryState()
    {
        string? problem = LauncherUserSettings.ValidateGameDirectory(gameDirectory);
        directoryState.Text = problem is null ? "✓  พบ C9.exe" : "✕  " + problem;
        directoryState.TextColor = problem is null ? Good : Bad;
        directorySource.Text = startup.ClientDirectoryOverride is not null && gameDirectory == startup.ClientDirectoryOverride
            ? "ใช้ค่าจาก --client-dir (ชั่วคราว)"
            : settings.GameDirectory is null ? "ใช้ค่าเริ่มต้นของโปรเจกต์" : "ใช้โฟลเดอร์ที่เลือกไว้ (จำค่าไว้แล้ว)";
    }

    private void UpdateMode()
    {
        modeBadge.Text = compatibilityTest ? "COMPATIBILITY TEST" : "LOCAL";
        modeBadgeFrame.BackgroundColor = compatibilityTest ? Gold : Muted;
        modeBadge.TextColor = compatibilityTest ? Ink : TextTone;
        if (Window is not null) Window.Title = compatibilityTest ? "C9 Local Launcher - Compatibility Test" : "C9 Local Launcher";
    }

    private void ShowIfFailed(LauncherStatus? status)
    {
        if (status is not null) ShowStatus(status);
    }

    private void ShowStatus(LauncherStatus status)
    {
        statusTitle.Text = status.Title;
        statusDetail.Text = status.Detail;
        Color tone = status.Tone switch
        {
            LauncherTone.Good => Good,
            LauncherTone.Warning => Gold,
            LauncherTone.Error => Bad,
            _ => Subtle,
        };
        statusTitle.TextColor = status.Tone == LauncherTone.Neutral ? TextTone : tone;
        statusDot.Fill = new SolidColorBrush(tone);
        if (status.ClientHash is not null)
            clientInfo.Text = $"Client {status.ClientBuild}  •  SHA-256 {status.ClientHash[..12]}…";
    }

    private void SetBusy(bool value)
    {
        busy = value;
        activity.IsRunning = value;
        activity.IsVisible = value;
        UpdateButtons();
    }

    private void UpdateButtons()
    {
        play.IsEnabled = !busy && !runtime.GameRunning && session is not null;
        play.Text = runtime.GameRunning ? "เกมกำลังทำงาน" : "▶  เปิดเกม Local";
        check.IsEnabled = !busy;
        foreach (View view in lockWhileBusy) view.IsEnabled = !busy;
    }
}
