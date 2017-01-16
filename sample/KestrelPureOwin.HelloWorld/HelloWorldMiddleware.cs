using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace KestrelPureOwin.HelloWorld
{
    using MidFunc = Func<
        Func<IDictionary<string, object>, Task>,
        Func<IDictionary<string, object>, Task>>;

    public static class HelloWorldMiddleware
    {
        private static readonly string[] TextPlain = { "text/plain" };
        private static readonly string[] ContentLength = { "13" };

        private static readonly byte[] Payload = Encoding.UTF8.GetBytes("Hello, World!");

        public static MidFunc Create()
        {
            return next => env =>
            {
                env.Set("owin.ResponseStatusCode", 200);

                var headers = env.Get<IDictionary<string, string[]>>("owin.ResponseHeaders");

                headers.Set("Content-Type", TextPlain);
                headers.Set("Content-Length", ContentLength);

                var stream = env.Get<Stream>("owin.ResponseBody");

                return stream.WriteAsync(Payload, 0, Payload.Length);
            };
        }
    }
}
