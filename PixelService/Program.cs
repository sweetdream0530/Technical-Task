using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System;
using System.Linq;
using System.Net.Http;
using System.Text;

namespace PixelService
{
    public class Program
    {
        public static WebApplicationBuilder CreateHostBuilder(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            return builder;
        }
        public static void Main(string[] args)
        {
            var builder = CreateHostBuilder(args);
            var app = builder.Build();

            // Define the /track endpoint
            app.MapGet("/track", async (HttpContext context) =>
            {
                // Extract headers and IP address
                string referrer = context.Request.Headers["Referer"].FirstOrDefault() ?? "null";
                string userAgent = context.Request.Headers["User-Agent"].FirstOrDefault() ?? "null";
                string ipAddress = context.Connection.RemoteIpAddress?.ToString() ?? "null";

                // Retrieve StorageService URL from configuration
                string storageServiceUrl = builder.Configuration.GetValue<string>("StorageServiceUrl");

                // Send data to Storage Service
                using var client = new HttpClient();
                var content = new StringContent($"{DateTime.UtcNow:O}|{referrer}|{userAgent}|{ipAddress}", Encoding.UTF8, "application/x-www-form-urlencoded");
                await client.PostAsync(storageServiceUrl + "/store", content);

                // Return a 1x1 transparent GIF
                var gif = Convert.FromBase64String("R0lGODlhAQABAIAAAP///wAAACH5BAEAAAAALAAAAAABAAEAAAICRAEAOw==");           
                context.Response.ContentType = "image/gif";
                await context.Response.Body.WriteAsync(gif, 0, gif.Length);
            });
            app.Run();
        }
    }
}