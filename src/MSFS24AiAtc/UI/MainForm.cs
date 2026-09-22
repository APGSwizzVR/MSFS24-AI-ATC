using MSFS24AiAtc.Core;
using MSFS24AiAtc.Services;

namespace MSFS24AiAtc.UI;

public sealed class MainForm : Form
{
    private readonly AccountService _accounts;
    private readonly ModerationService _moderation;
    private readonly AirportService _airports;
    private readonly SimConnectService _sim;
    private readonly AiProviderManager _ai;
    private readonly SpeechService _speech;

    private readonly Label status = new();
    private readonly ComboBox model = new();
    private readonly ComboBox controller = new();
    private readonly TextBox transcript = new();
    private UserAccount? _user;

    public MainForm(AccountService accounts, ModerationService moderation, AirportService airports,
        SimConnectService sim, AiProviderManager ai, SpeechService speech)
    {
        _accounts = accounts;
        _moderation = moderation;
        _airports = airports;
        _sim = sim;
        _ai = ai;
        _speech = speech;

        Text = "MSFS24 AI ATC";
        Width = 1100;
        Height = 720;
        MinimumSize = new Size(900, 600);
        BackColor = Color.FromArgb(18, 20, 24);
        ForeColor = Color.White;

        BuildUi();
        Shown += (_, _) => ShowLogin();
        FormClosing += (_, _) => _sim.Dispose();
    }

    private void BuildUi()
    {
        var top = new Panel { Dock = DockStyle.Top, Height = 70, Padding = new Padding(18) };
        var title = new Label { Text = "MSFS24 AI ATC", AutoSize = true, Font = new Font("Segoe UI", 20, FontStyle.Bold) };
        status.Text = "● MSFS 2024: Not connected";
        status.AutoSize = true;
        status.Left = 280;
        status.Top = 12;
        status.ForeColor = Color.Gold;
        top.Controls.Add(title);
        top.Controls.Add(status);

        var nav = new FlowLayoutPanel { Dock = DockStyle.Left, Width = 220, FlowDirection = FlowDirection.TopDown, Padding = new Padding(16), WrapContents = false };
        foreach (var name in new[] { "Flight", "Frequencies", "AI Models", "Audio", "Settings", "Account" })
        {
            var b = new Button { Text = name, Width = 185, Height = 42, FlatStyle = FlatStyle.Flat, Margin = new Padding(0, 0, 0, 8) };
            b.Click += (_, _) => MessageBox.Show($"{name} panel is part of the current application shell.");
            nav.Controls.Add(b);
        }

        var content = new Panel { Dock = DockStyle.Fill, Padding = new Padding(20) };
        var heading = new Label { Text = "AI Controller Configuration", Font = new Font("Segoe UI", 16, FontStyle.Bold), AutoSize = true };
        heading.Top = 10;

        controller.Items.AddRange(Enum.GetNames<ControllerType>());
        controller.SelectedIndex = 0;
        controller.Left = 10;
        controller.Top = 55;
        controller.Width = 220;

        model.Items.AddRange(_ai.Models.Select(x => $"{x.Provider} / {x.Model}").ToArray());
        model.SelectedIndex = 0;
        model.Left = 250;
        model.Top = 55;
        model.Width = 300;

        transcript.Multiline = true;
        transcript.ReadOnly = true;
        transcript.ScrollBars = ScrollBars.Vertical;
        transcript.Left = 10;
        transcript.Top = 115;
        transcript.Width = 790;
        transcript.Height = 330;
        transcript.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;

        var connect = new Button { Text = "Connect to MSFS 2024", Left = 10, Top = 465, Width = 220, Height = 45 };
        connect.Click += (_, _) => Connect();

        var test = new Button { Text = "Test ATC Voice", Left = 245, Top = 465, Width = 180, Height = 45 };
        test.Click += async (_, _) => await TestVoice();

        content.Controls.AddRange([heading, controller, model, transcript, connect, test]);
        Controls.Add(content);
        Controls.Add(nav);
        Controls.Add(top);
    }

    private void Connect()
    {
        if (_sim.TryConnect(Handle))
        {
            status.Text = "● MSFS 2024: Connected";
            status.ForeColor = Color.LightGreen;
            transcript.AppendText("[SIM] Connected to MSFS 2024.
");
        }
        else
        {
            status.Text = "● MSFS 2024: Not connected";
            status.ForeColor = Color.Gold;
            transcript.AppendText("[SIM] MSFS 2024 was not detected. Start a flight and try again.
");
        }
    }

    private async Task TestVoice()
    {
        var text = "Shamrock Five Five Eight Three, Dublin Control, descend flight level two four zero.";
        transcript.AppendText($"[ATC] {text}
");
        await _speech.SpeakAsync(text);
    }

    private void ShowLogin()
    {
        using var dialog = new LoginForm(_accounts);
        if (dialog.ShowDialog(this) != DialogResult.OK || dialog.User is null)
        {
            Close();
            return;
        }

        _user = dialog.User;
        if (_user.Suspended)
        {
            MessageBox.Show("This account is suspended.", "MSFS24 AI ATC");
            Close();
        }
    }
}

internal sealed class LoginForm : Form
{
    private readonly AccountService _accounts;
    private readonly TextBox username = new();
    private readonly TextBox password = new();
    public UserAccount? User { get; private set; }

    public LoginForm(AccountService accounts)
    {
        _accounts = accounts;
        Text = "MSFS24 AI ATC Account";
        Width = 420;
        Height = 260;
        StartPosition = FormStartPosition.CenterParent;
        FormBorderStyle = FormBorderStyle.FixedDialog;

        var u = new Label { Text = "Username", Left = 25, Top = 25, AutoSize = true };
        username.SetBounds(25, 48, 350, 30);
        var p = new Label { Text = "Password", Left = 25, Top = 88, AutoSize = true };
        password.SetBounds(25, 111, 350, 30);
        password.UseSystemPasswordChar = true;

        var login = new Button { Text = "Log in", Left = 25, Top = 155, Width = 105 };
        login.Click += (_, _) => TryLogin();

        var create = new Button { Text = "Create account", Left = 145, Top = 155, Width = 130 };
        create.Click += (_, _) => TryCreate();

        Controls.AddRange([u, username, p, password, login, create]);
    }

    private void TryLogin()
    {
        try { User = _accounts.Login(username.Text, password.Text); DialogResult = DialogResult.OK; }
        catch (Exception ex) { MessageBox.Show(ex.Message); }
    }

    private void TryCreate()
    {
        try
        {
            User = _accounts.Create(username.Text, password.Text);
            MessageBox.Show("Account created.");
            DialogResult = DialogResult.OK;
        }
        catch (Exception ex) { MessageBox.Show(ex.Message); }
    }
}
