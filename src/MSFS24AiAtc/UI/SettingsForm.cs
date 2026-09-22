using MSFS24AiAtc.Core;
using MSFS24AiAtc.Services;

namespace MSFS24AiAtc.UI;

public sealed class SettingsForm : Form
{
    private readonly SecureSettings settings;
    private readonly AiProviderManager ai;
    private readonly TextBox openAi = new();
    private readonly TextBox customEndpoint = new();
    private readonly TextBox customKey = new();
    private readonly NumericUpDown volume = new();
    private readonly CheckBox staticNoise = new();
    private readonly CheckBox bandpass = new();
    private readonly CheckBox compression = new();

    public SettingsForm(SecureSettings settings, AiProviderManager ai)
    {
        this.settings = settings;
        this.ai = ai;
        Text = "MSFS24 AI ATC — Settings";
        Width = 850;
        Height = 720;
        StartPosition = FormStartPosition.CenterParent;

        var tabs = new TabControl { Dock = DockStyle.Fill };
        tabs.TabPages.Add(BuildApiPage());
        tabs.TabPages.Add(BuildAudioPage());
        tabs.TabPages.Add(BuildControllerPage());

        var save = new Button { Text = "Save", Dock = DockStyle.Bottom, Height = 46 };
        save.Click += (_, _) => Save();

        Controls.Add(tabs);
        Controls.Add(save);
    }

    private TabPage BuildApiPage()
    {
        var page = new TabPage("API & AI");
        AddLabel(page, "OpenAI API key", 25, 25);
        openAi.SetBounds(25, 52, 760, 32);
        openAi.UseSystemPasswordChar = true;
        openAi.Text = settings.Get("api.openai");
        page.Controls.Add(openAi);

        AddLabel(page, "Custom OpenAI-compatible endpoint", 25, 105);
        customEndpoint.SetBounds(25, 132, 760, 32);
        customEndpoint.Text = settings.Get("api.custom.endpoint");
        page.Controls.Add(customEndpoint);

        AddLabel(page, "Custom provider API key", 25, 185);
        customKey.SetBounds(25, 212, 760, 32);
        customKey.UseSystemPasswordChar = true;
        customKey.Text = settings.Get("api.custom.key");
        page.Controls.Add(customKey);

        page.Controls.Add(new Label {
            Text = "Keys are encrypted locally with Windows DPAPI. Never commit API keys to GitHub.",
            Left = 25, Top = 270, Width = 760, Height = 50
        });

        return page;
    }

    private TabPage BuildAudioPage()
    {
        var page = new TabPage("Radio Audio");
        AddLabel(page, "Radio volume", 25, 25);
        volume.Minimum = 0; volume.Maximum = 100;
        volume.Value = decimal.Parse(settings.Get("radio.volume", "85"));
        volume.SetBounds(25, 52, 180, 30);
        page.Controls.Add(volume);

        staticNoise.Text = "VHF static / noise bursts";
        staticNoise.Checked = settings.Get("radio.static", "true") == "true";
        staticNoise.SetBounds(25, 105, 300, 30);

        bandpass.Text = "VHF band limiting";
        bandpass.Checked = settings.Get("radio.bandpass", "true") == "true";
        bandpass.SetBounds(25, 145, 300, 30);

        compression.Text = "Radio compression";
        compression.Checked = settings.Get("radio.compression", "true") == "true";
        compression.SetBounds(25, 185, 300, 30);

        page.Controls.AddRange([staticNoise, bandpass, compression]);
        return page;
    }

    private TabPage BuildControllerPage()
    {
        var page = new TabPage("Controller Models");
        var y = 20;

        foreach (var type in Enum.GetValues<ControllerType>())
        {
            AddLabel(page, type.ToString(), 25, y);
            var box = new ComboBox { Left = 180, Top = y - 3, Width = 500, DropDownStyle = ComboBoxStyle.DropDownList };
            box.Items.AddRange(ai.Models.Select(m => $"{m.Provider}/{m.Model}").ToArray());
            var selected = ai.GetModel(type);
            box.SelectedItem = $"{selected.Provider}/{selected.Model}";
            if (box.SelectedIndex < 0) box.SelectedIndex = 0;
            box.Tag = type;
            page.Controls.Add(box);
            y += 55;
        }

        return page;
    }

    private static void AddLabel(Control page, string text, int x, int y) =>
        page.Controls.Add(new Label { Text = text, Left = x, Top = y, Width = 145, Height = 28 });

    private void Save()
    {
        settings.Set("api.openai", openAi.Text.Trim());
        settings.Set("api.custom.endpoint", customEndpoint.Text.Trim());
        settings.Set("api.custom.key", customKey.Text.Trim());
        settings.Set("radio.volume", ((int)volume.Value).ToString());
        settings.Set("radio.static", staticNoise.Checked.ToString().ToLowerInvariant());
        settings.Set("radio.bandpass", bandpass.Checked.ToString().ToLowerInvariant());
        settings.Set("radio.compression", compression.Checked.ToString().ToLowerInvariant());

        foreach (Control control in Controls.OfType<TabControl>().First().TabPages[2].Controls)
        {
            if (control is ComboBox box && box.Tag is ControllerType type && box.SelectedItem is string value)
            {
                var split = value.Split('/', 2);
                if (split.Length == 2)
                    ai.SetModel(type, new AiModel(split[0], split[1]));
            }
        }

        DialogResult = DialogResult.OK;
        Close();
    }
}
