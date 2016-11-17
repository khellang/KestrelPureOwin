using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Http.Features.Authentication;
using Microsoft.Extensions.ObjectPool;
using static KestrelPureOwin.Constants;

namespace KestrelPureOwin
{
    public class OwinApplication : IHttpApplication<OwinApplication.Context>
    {
        private static readonly ObjectPoolProvider PoolProvider = new DefaultObjectPoolProvider();

        public OwinApplication(AppFunc application)
        {
            Application = application;
            EnvironmentPool = PoolProvider.Create<Dictionary<string, object>>();
        }

        private AppFunc Application { get; }

        private ObjectPool<Dictionary<string, object>> EnvironmentPool { get; }

        public Context CreateContext(IFeatureCollection features)
        {
            var environment = EnvironmentPool.Get();

            var context = new Context(features, environment);

            var request = features.Get<IHttpRequestFeature>();
            var response = features.Get<IHttpResponseFeature>();
            var connection = features.Get<IHttpConnectionFeature>();
            var lifetime = features.Get<IHttpRequestLifetimeFeature>();
            var identifier = features.Get<IHttpRequestIdentifierFeature>();
            var authentication = features.Get<IHttpAuthenticationFeature>();

            context.Clear();

            context.Set(Owin.Request.Body, request.Body);
            context.Set(Owin.Request.Headers, new OwinHeaderDictionary(request.Headers));
            context.Set(Owin.Request.Method, request.Method);
            context.Set(Owin.Request.Path, request.Path);
            context.Set(Owin.Request.PathBase, request.PathBase);
            context.Set(Owin.Request.Protocol, request.Protocol);
            context.Set(Owin.Request.QueryString, request.QueryString.TrimStart('?'));
            context.Set(Owin.Request.Scheme, request.Scheme);
            context.Set(Owin.Request.User, authentication?.User);
            context.Set(Owin.Request.Id, identifier?.TraceIdentifier);

            context.Set(Owin.Response.Body, response.Body);
            context.Set(Owin.Response.Headers, new OwinHeaderDictionary(response.Headers));
            context.Set(Owin.Response.StatusCode, response.StatusCode);
            context.Set(Owin.Response.ReasonPhrase, response.ReasonPhrase);
            context.Set(Owin.Response.Protocol, request.Protocol);

            context.Set(Owin.CallCancelled, lifetime.RequestAborted);
            context.Set(Owin.OwinVersion, "1.1");

            context.Set(Server.RemoteIpAddress, connection?.RemoteIpAddress.ToString());
            context.Set(Server.RemotePort, connection?.RemotePort.ToString());
            context.Set(Server.LocalIpAddress, connection?.LocalIpAddress.ToString());
            context.Set(Server.LocalPort, connection?.LocalPort.ToString());
            context.Set(Server.User, authentication?.User);

            return new Context(features, environment);
        }

        public async Task ProcessRequestAsync(Context context)
        {
            await Application.Invoke(context.Environment);

            var response = context.Features.Get<IHttpResponseFeature>();

            response.Body = context.Get<Stream>(Owin.Response.Body);
            // TODO: Headers...
            response.StatusCode = context.Get<int>(Owin.Response.StatusCode);
            response.ReasonPhrase = context.Get<string>(Owin.Response.ReasonPhrase);
        }

        public void DisposeContext(Context context, Exception exception)
        {
            EnvironmentPool.Return(context.Environment);
        }

        public class Context
        {
            public Context(IFeatureCollection features, Dictionary<string, object> environment)
            {
                Features = features;
                Environment = environment;
            }

            public IFeatureCollection Features { get; }

            public Dictionary<string, object> Environment { get; }

            public T Get<T>(string key)
            {
                object value;
                if (Environment.TryGetValue(key, out value) && value is T)
                {
                    return (T) value;
                }

                return default(T);
            }

            public void Set<T>(string key, T value)
            {
                Environment[key] = value;
            }

            public void Clear()
            {
                Environment.Clear();
            }
        }
    }
}
