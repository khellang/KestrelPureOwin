using System;

namespace KestrelPureOwin
{
    public static class LoggingMiddleware
    {
        public static MidFunc Create()
        {
            return next => env =>
            {
                Console.WriteLine();

                foreach (var pair in env)
                {
                    Console.WriteLine($"{pair.Key}: {pair.Value ?? "<null>"}");
                }

                return next(env);
            };
        }
    }
}
