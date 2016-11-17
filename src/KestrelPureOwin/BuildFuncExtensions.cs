namespace KestrelPureOwin
{
    public static class BuildFuncExtensions
    {
        public static BuildFunc UseHelloWorld(this BuildFunc builder)
        {
            return builder.Use(HelloWorldMiddleware.Create());
        }

        public static BuildFunc UseLogging(this BuildFunc builder)
        {
            return builder.Use(LoggingMiddleware.Create());
        }

        public static BuildFunc Use(this BuildFunc builder, MidFunc middleware)
        {
            builder(middleware);
            return builder;
        }
    }
}
