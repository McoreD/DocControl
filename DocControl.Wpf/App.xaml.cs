using System.IO;
using System.Windows;
using DocControl.AI;
using DocControl.Core.Configuration;
using DocControl.Core.Security;
using DocControl.Infrastructure.Data;
using DocControl.Infrastructure.Presentation;
using DocControl.Infrastructure.Services;
using Microsoft.Extensions.DependencyInjection;

namespace DocControl.Wpf
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private ServiceProvider? serviceProvider;

        protected override async void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var services = new ServiceCollection();

            var dbPath = Path.Combine(AppContext.BaseDirectory, "doccontrol.db");
            var settingsPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "DocControl", "settings.json");
            var dbFactory = new DbConnectionFactory(dbPath);
            var initializer = new DatabaseInitializer(dbFactory);
            await initializer.EnsureCreatedAsync();

            var configRepo = new ConfigRepository(dbFactory);
            var credentialStore = new JsonFileApiKeyStore(settingsPath);
            var configService = new ConfigService(configRepo, credentialStore);

            var documentConfig = await configService.LoadDocumentConfigAsync();
            var aiSettings = await configService.LoadAiSettingsAsync();
            var aiOptions = await AiOptionsBuilder.BuildAsync(aiSettings, credentialStore);

            services.AddSingleton(dbFactory);
            services.AddSingleton(configRepo);
            services.AddSingleton<IApiKeyStore>(credentialStore);
            services.AddSingleton(configService);

            services.AddSingleton(documentConfig);
            services.AddSingleton(aiSettings);
            services.AddSingleton(aiOptions);

            services.AddAiClients(aiOptions);

            services.AddSingleton<DatabaseInitializer>();
            services.AddSingleton<NumberAllocator>();
            services.AddSingleton<DocumentRepository>();
            services.AddSingleton<AuditRepository>();
            services.AddSingleton<CodeSeriesRepository>();

            services.AddSingleton<CodeGenerator>();
            services.AddSingleton<FileNameParser>();
            services.AddSingleton<DocumentService>();
            services.AddSingleton<ImportService>();
            services.AddSingleton<RecommendationService>();
            services.AddSingleton<NlqService>();
            services.AddSingleton<CodeImportService>();
            services.AddSingleton<DocumentImportService>();
            services.AddSingleton<MainController>();

            services.AddSingleton<MainWindow>(provider => new MainWindow(
                provider.GetRequiredService<MainController>(),
                provider.GetRequiredService<DocumentConfig>(),
                provider.GetRequiredService<AiSettings>(),
                provider.GetRequiredService<AiClientOptions>()));

            serviceProvider = services.BuildServiceProvider();

            var mainWindow = serviceProvider.GetRequiredService<MainWindow>();
            mainWindow.Show();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            serviceProvider?.Dispose();
            base.OnExit(e);
        }
    }
}
