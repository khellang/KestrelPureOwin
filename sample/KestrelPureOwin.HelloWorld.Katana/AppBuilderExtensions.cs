using Owin;

namespace KestrelPureOwin.HelloWorld.Katana
{
    public static class AppBuilderExtensions
    {
        public static IAppBuilder UseHelloWorld(this IAppBuilder app)
        {
            return app.Use<HelloWorldMiddleware>();
        }
    }
}