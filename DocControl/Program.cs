using DocControl.AI;
using DocControl.Configuration;
using DocControl.Data;
using DocControl.Security;
using DocControl.Services;
using Microsoft.Extensions.DependencyInjection;

namespace DocControl
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();

            var services = new ServiceCollection();

            var dbPath = Path.Combine(AppContext.BaseDirectory, "doccontrol.db");
            var dbFactory = new DbConnectionFactory(dbPath);
            var initializer = new DatabaseInitializer(dbFactory);
            initializer.EnsureCreatedAsync().GetAwaiter().GetResult();

            var configRepo = new ConfigRepository(dbFactory);
            var credentialStore = new CredentialManagerApiKeyStore();
            var configService = new ConfigService(configRepo, credentialStore);

            var documentConfig = configService.LoadDocumentConfigAsync().GetAwaiter().GetResult();
            var aiSettings = configService.LoadAiSettingsAsync().GetAwaiter().GetResult();
            var aiOptions = AiOptionsBuilder.BuildAsync(aiSettings, credentialStore).GetAwaiter().GetResult();

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

            services.AddSingleton<Form1>();

            using var provider = services.BuildServiceProvider();

            Application.Run(provider.GetRequiredService<Form1>());
        }
    }
}