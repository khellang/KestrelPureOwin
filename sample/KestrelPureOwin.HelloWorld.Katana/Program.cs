using System;
using Microsoft.Owin.Hosting;
using Owin;

namespace KestrelPureOwin.HelloWorld.Katana
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            const string url = "http://localhost:8080";

            var options = new StartOptions(url)
            {
                ServerFactory = "KestrelPureOwin"
            };

            using (WebApp.Start(options, Configure))
            {
                Console.WriteLine($"Listening on {url}...");
                Console.ReadLine();
            }
        }

        private static void Configure(IAppBuilder app)
        {
            app.UseHelloWorld();
        }
    }
}
