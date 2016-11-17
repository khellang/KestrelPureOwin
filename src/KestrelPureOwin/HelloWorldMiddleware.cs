using System.Collections.Generic;
using System.IO;
using System.Text;
using static KestrelPureOwin.Constants;

namespace KestrelPureOwin
{
    public static class HelloWorldMiddleware
    {
        private static readonly string[] TextPlain = { "text/plain" };
        private static readonly string[] ContentLength = { "13" };

        private static readonly byte[] Payload = Encoding.UTF8.GetBytes("Hello, World!");

        public static MidFunc Create()
        {
            return next => env =>
            {
                env.Set(Owin.Response.StatusCode, 200);

                var headers = env.Get<IDictionary<string, string[]>>(Owin.Response.Headers);

                headers["Content-Type"] = TextPlain;
                headers["Content-Length"] = ContentLength;

                var stream = env.Get<Stream>(Owin.Response.Body);

                return stream.WriteAsync(Payload, 0, Payload.Length);
            };
        }
    }
}
