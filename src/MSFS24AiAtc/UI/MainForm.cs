using MSFS24AiAtc.Core;
using MSFS24AiAtc.Services;

namespace MSFS24AiAtc.UI;

public sealed class MainForm : Form
{
    private readonly AccountService accounts;
    private readonly ModerationService moderation;
    private readonly SimConnectService sim;
    private readonly AiProviderManager ai;
    private readonly RadioAudioService radio;
    private readonly SecureSettings settings;

    private readonly Label status = new();
    private readonly Label frequency = new();
    private readonly TextBox log = new();
    private UserAccount? user;

    public MainForm(AccountService accounts, ModerationService moderation, SimConnectService sim,
        AiProviderManager ai, RadioAudioService radio, SecureSettings settings)
    {
        this.accounts = accounts;
        this.moderation = moderation;
        this.sim = sim;
        this.ai = ai;
        this.radio = radio;
        this.settings = settings;

        Text = "MSFS24 AI ATC";
        Width = 1200;
        Height = 760;
        MinimumSize = new Size(1000, 650);
        BackColor = Color.FromArgb(15, 17, 21);
        ForeColor = Color.White;

        BuildUi();
        Shown += (_, _) => Login();
        FormClosing += (_, _) => sim.Dispose();
    }

    private void BuildUi()
    {
        var header = new Panel { Dock = DockStyle.Top, Height = 74, Padding = new Padding(18) };
        header.Controls.Add(new Label {
            Text = "MSFS24 AI ATC",
            AutoSize = true,
            Font = new Font("Segoe UI", 21, FontStyle.Bold)
        });

        status.Text = "● MSFS 2024 — Waiting";
        status.AutoSize = true;
        status.Left = 280;
        status.Top = 17;
        status.ForeColor = Color.Gold;
        header.Controls.Add(status);

        var freqPanel = new Panel { Dock = DockStyle.Top, Height = 82, Padding = new Padding(20) };
        frequency.Text = "ACTIVE FREQUENCY  —  ---";
        frequency.Font = new Font("Consolas", 20, FontStyle.Bold);
        frequency.AutoSize = true;
        freqPanel.Controls.Add(frequency);

        var nav = new FlowLayoutPanel {
            Dock = DockStyle.Left, Width = 230, Padding = new Padding(16),
            FlowDirection = FlowDirection.TopDown, WrapContents = false
        };

        AddNav(nav, "Flight");
        AddNav(nav, "Frequencies");
        AddNav(nav, "Traffic");
        AddNav(nav, "ATC Log");
        AddNav(nav, "AI Models", () => new SettingsForm(settings, ai).ShowDialog(this));
        AddNav(nav, "API & Settings", () => new SettingsForm(settings, ai).ShowDialog(this));

        var content = new Panel { Dock = DockStyle.Fill, Padding = new Padding(20) };
        log.Multiline = true;
        log.ReadOnly = true;
        log.ScrollBars = ScrollBars.Vertical;
        log.Dock = DockStyle.Fill;
        log.Font = new Font("Consolas", 11);
        content.Controls.Add(log);

        var bottom = new FlowLayoutPanel { Dock = DockStyle.Bottom, Height = 62 };
        var connect = new Button { Text = "Connect to MSFS 2024", Width = 190, Height = 42 };
        connect.Click += (_, _) => Connect();
        var settingsButton = new Button { Text = "Settings", Width = 110, Height = 42 };
        settingsButton.Click += (_, _) => new SettingsForm(settings, ai).ShowDialog(this);
        bottom.Controls.Add(connect);
        bottom.Controls.Add(settingsButton);

        Controls.Add(content);
        Controls.Add(nav);
        Controls.Add(bottom);
        Controls.Add(freqPanel);
        Controls.Add(header);
    }

    private void AddNav(FlowLayoutPanel nav, string text, Action? action = null)
    {
        var button = new Button { Text = text, Width = 195, Height = 40, FlatStyle = FlatStyle.Flat };
        button.Click += (_, _) => action?.Invoke();
        nav.Controls.Add(button);
    }

    private void Connect()
    {
        if (sim.TryConnect(Handle))
        {
            status.Text = "● MSFS 2024 — Connected";
            status.ForeColor = Color.LightGreen;
            Write("SIM", "Connected to Microsoft Flight Simulator 2024.");
        }
        else
        {
            status.Text = "● MSFS 2024 — Waiting";
            status.ForeColor = Color.Gold;
            Write("SIM", "MSFS 2024 not detected. Start the simulator and try again.");
        }
    }

    private void Write(string source, string text) =>
        log.AppendText($"[{DateTime.Now:HH:mm:ss}] [{source}] {text}{Environment.NewLine}");

    private void Login()
    {
        using var dialog = new LoginForm(accounts);
        if (dialog.ShowDialog(this) != DialogResult.OK || dialog.User is null)
        {
            Close();
            return;
        }

        user = dialog.User;
        if (user.Suspended)
        {
            MessageBox.Show("This account is suspended.", "MSFS24 AI ATC");
            Close();
        }
    }
}

internal sealed class LoginForm : Form
{
    private readonly AccountService accounts;
    private readonly TextBox username = new();
    private readonly TextBox password = new();
    public UserAccount? User { get; private set; }

    public LoginForm(AccountService accounts)
    {
        this.accounts = accounts;
        Text = "MSFS24 AI ATC Account";
        Width = 430;
        Height = 270;
        StartPosition = FormStartPosition.CenterParent;
        FormBorderStyle = FormBorderStyle.FixedDialog;

        Controls.Add(new Label { Text = "Username", Left = 25, Top = 25, AutoSize = true });
        username.SetBounds(25, 48, 360, 30);
        Controls.Add(username);

        Controls.Add(new Label { Text = "Password", Left = 25, Top = 88, AutoSize = true });
        password.SetBounds(25, 111, 360, 30);
        password.UseSystemPasswordChar = true;
        Controls.Add(password);

        var login = new Button { Text = "Log in", Left = 25, Top = 160, Width = 110 };
        login.Click += (_, _) => TryLogin();
        var create = new Button { Text = "Create account", Left = 150, Top = 160, Width = 135 };
        create.Click += (_, _) => TryCreate();

        Controls.Add(login);
        Controls.Add(create);
    }

    private void TryLogin()
    {
        try { User = accounts.Login(username.Text, password.Text); DialogResult = DialogResult.OK; }
        catch (Exception ex) { MessageBox.Show(ex.Message); }
    }

    private void TryCreate()
    {
        try { User = accounts.Create(username.Text, password.Text); DialogResult = DialogResult.OK; }
        catch (Exception ex) { MessageBox.Show(ex.Message); }
    }
}
