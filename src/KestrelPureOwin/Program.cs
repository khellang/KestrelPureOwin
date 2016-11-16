using Microsoft.AspNetCore.Hosting.Internal;
using Microsoft.AspNetCore.Server.Kestrel;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace KestrelPureOwin
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            var options = Options.Create(new KestrelServerOptions());

            var lifetime = new ApplicationLifetime();

            var loggerFactory = new LoggerFactory();

            using (var kestrel = new KestrelServer(options, lifetime, loggerFactory))
            {
                kestrel.Run("http://localhost:8080", Configure);
            }
        }

        private static void Configure(BuildFunc builder)
        {
            builder.UseLogging();
        }
    }
}
