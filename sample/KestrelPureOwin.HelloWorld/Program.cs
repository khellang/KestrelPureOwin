using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Server.Kestrel;

namespace KestrelPureOwin.HelloWorld
{
    using BuildFunc = Action<Func<
        Func<IDictionary<string, object>, Task>,
        Func<IDictionary<string, object>, Task>>>;

    public static class Program
    {
        public static void Main(string[] args)
        {
            var options = new KestrelServerOptions
            {
                AddServerHeader = false
            };

            using (var server = new KestrelOwinServer(options))
            {
                server.Run("http://localhost:8080", Configure);
            }
        }

        private static void Configure(BuildFunc builder)
        {
            builder.UseHelloWorld();
        }
    }
}
