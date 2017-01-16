using System.Text;
using System.Threading.Tasks;
using Microsoft.Owin;

namespace KestrelPureOwin.HelloWorld.Katana
{
    public class HelloWorldMiddleware : OwinMiddleware
    {
        private const string TextPlain = "text/plain";
        private const string ContentLength = "13";

        private static readonly byte[] Payload = Encoding.UTF8.GetBytes("Hello, World!");

        public HelloWorldMiddleware(OwinMiddleware next) : base(next)
        {
        }

        public override Task Invoke(IOwinContext context)
        {
            context.Response.StatusCode = 200;

            context.Response.Headers.Set("Content-Type", TextPlain);
            context.Response.Headers.Set("Content-Length", ContentLength);

            return context.Response.WriteAsync(Payload);
        }
    }
}