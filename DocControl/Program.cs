using DocControl.AI;
using Microsoft.Extensions.DependencyInjection;

namespace DocControl
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();

            var services = new ServiceCollection();

            var aiOptions = new AiClientOptions
            {
                OpenAi = new OpenAiOptions
                {
                    ApiKey = string.Empty,
                    Model = "gpt-4.1"
                },
                Gemini = new GeminiOptions
                {
                    ApiKey = string.Empty,
                    Model = "gemini-1.5-pro"
                }
            };

            services.AddAiClients(aiOptions);
            services.AddSingleton<Form1>();

            using var provider = services.BuildServiceProvider();
            Application.Run(provider.GetRequiredService<Form1>();
        }
    }
}