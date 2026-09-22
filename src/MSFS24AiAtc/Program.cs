using MSFS24AiAtc.Data;
using MSFS24AiAtc.Services;
using MSFS24AiAtc.UI;

ApplicationConfiguration.Initialize();

var dataDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "MSFS24-AI-ATC");
Directory.CreateDirectory(dataDir);

var database = new AppDatabase(Path.Combine(dataDir, "accounts.db"));
database.Initialize();

var accounts = new AccountService(database);
var moderation = new ModerationService(database);
var settings = new SecureSettings();
var ai = new AiProviderManager(settings);
var radio = new RadioAudioService();
var sim = new SimConnectService();

using var form = new MainForm(accounts, moderation, sim, ai, radio, settings);
Application.Run(form);
