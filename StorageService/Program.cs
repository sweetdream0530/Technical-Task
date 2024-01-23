using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Threading.Tasks;

namespace StorageService
{
    public class Program
    {
        public static WebApplicationBuilder CreateHostBuilder(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // You can add additional services if needed
            // builder.Services...

            return builder;
        }

        public static void Main(string[] args)
        {
            var builder = CreateHostBuilder(args);
            var app = builder.Build();

            var configuration = builder.Configuration;
            var logFilePath = configuration.GetValue<string>("LogFilePath") ?? "F:\\tmp\\visits.log";

            app.MapPost("/store", async (HttpContext context) =>
            {
                using var reader = new StreamReader(context.Request.Body);
                string content = await reader.ReadToEndAsync();

                // Ensure the directory exists
                var directory = Path.GetDirectoryName(logFilePath);
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                // Append the content to the log file
                await File.AppendAllTextAsync(logFilePath, content + Environment.NewLine);
            });

            app.Run();
        }
    }
}
