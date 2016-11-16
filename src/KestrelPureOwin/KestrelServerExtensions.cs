using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore.Server.Kestrel;

namespace KestrelPureOwin
{
    public static class KestrelServerExtensions
    {
        public static void Run(this KestrelServer server, string url, Action<BuildFunc> configure)
        {
            var done = new ManualResetEventSlim(false);

            Console.CancelKeyPress += (s, e) =>
            {
                Console.WriteLine("Application is shutting down...");

                done.Set();

                e.Cancel = true;
            };

            server.Start(url, configure);

            done.Wait();
        }

        public static void Start(this KestrelServer server, string url, Action<BuildFunc> configure)
        {
            var application = ConfigureApplication(configure);

            var addresses = server.Features.Get<IServerAddressesFeature>();

            if (addresses == null)
            {
                server.Features.Set(addresses = new ServerAddressesFeature());
            }

            addresses.Addresses.Add(url);

            foreach (var address in addresses.Addresses)
            {
                Console.WriteLine($"Now listening on: {address}");
            }

            server.Start(application);
        }

        private static OwinApplication ConfigureApplication(Action<BuildFunc> configure)
        {
            var middleware = new List<MidFunc>();

            var builder = new BuildFunc(middleware.Add);

            configure(builder);

            return BuildApplication(middleware);
        }

        private static OwinApplication BuildApplication(IEnumerable<MidFunc> middleware)
        {
            var end = new AppFunc(env => Task.CompletedTask);

            var app = middleware.Reverse().Aggregate(end, (current, next) => next(current));

            return new OwinApplication(app);
        }
    }
}
