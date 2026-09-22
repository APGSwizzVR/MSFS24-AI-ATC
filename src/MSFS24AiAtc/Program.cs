using MSFS24AiAtc.Core;
using MSFS24AiAtc.Data;
using MSFS24AiAtc.Services;
using MSFS24AiAtc.UI;

ApplicationConfiguration.Initialize();

var dataDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "MSFS24-AI-ATC");
Directory.CreateDirectory(dataDir);

var database = new AppDatabase(Path.Combine(dataDir, "accounts.db"));
database.Initialize();

var accountService = new AccountService(database);
var moderation = new ModerationService(database);
var airportService = new AirportService();
var simConnect = new SimConnectService();
var ai = new AiProviderManager();
var speech = new SpeechService();

using var form = new MainForm(accountService, moderation, airportService, simConnect, ai, speech);
Application.Run(form);
