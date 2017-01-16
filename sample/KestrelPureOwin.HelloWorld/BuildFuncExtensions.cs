using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace KestrelPureOwin.HelloWorld
{
    using MidFunc = Func<
        Func<IDictionary<string, object>, Task>,
        Func<IDictionary<string, object>, Task>>;

    using BuildFunc = Action<Func<
        Func<IDictionary<string, object>, Task>,
        Func<IDictionary<string, object>, Task>>>;

    public static class BuildFuncExtensions
    {
        public static BuildFunc UseHelloWorld(this BuildFunc builder)
        {
            return builder.Use(HelloWorldMiddleware.Create());
        }

        public static BuildFunc Use(this BuildFunc builder, MidFunc middleware)
        {
            builder(middleware);
            return builder;
        }
    }
}
