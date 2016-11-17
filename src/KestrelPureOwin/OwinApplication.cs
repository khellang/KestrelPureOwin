using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Http.Features.Authentication;
using Microsoft.Extensions.ObjectPool;

namespace KestrelPureOwin
{
    public class OwinApplication : IHttpApplication<Dictionary<string, object>>
    {
        private static readonly ObjectPoolProvider PoolProvider = new DefaultObjectPoolProvider();

        public OwinApplication(AppFunc application)
        {
            Application = application;
            EnvironmentPool = PoolProvider.Create<Dictionary<string, object>>();
        }

        private AppFunc Application { get; }

        private ObjectPool<Dictionary<string, object>> EnvironmentPool { get; }

        public Dictionary<string, object> CreateContext(IFeatureCollection features)
        {
            var environment = EnvironmentPool.Get();

            var request = features.Get<IHttpRequestFeature>();
            var response = features.Get<IHttpResponseFeature>();
            var connection = features.Get<IHttpConnectionFeature>();
            var lifetime = features.Get<IHttpRequestLifetimeFeature>();
            var identifier = features.Get<IHttpRequestIdentifierFeature>();
            var authentication = features.Get<IHttpAuthenticationFeature>();

            environment.Clear();

            environment["owin.RequestBody"] = request.Body;
            environment["owin.RequestHeaders"] = new OwinHeaderDictionary(request.Headers);
            environment["owin.RequestMethod"] = request.Method;
            environment["owin.RequestPath"] = request.Path;
            environment["owin.RequestPathBase"] = request.PathBase;
            environment["owin.RequestProtocol"] = request.Protocol;
            environment["owin.RequestQueryString"] = request.QueryString.TrimStart('?');
            environment["owin.RequestScheme"] = request.Scheme;
            environment["owin.RequestUser"] = authentication?.User;
            environment["owin.RequestId"] = identifier?.TraceIdentifier;

            environment["owin.ResponseBody"] = response.Body;
            environment["owin.ResponseHeaders"] = new OwinHeaderDictionary(response.Headers);
            environment["owin.ResponseStatusCode"] = response.StatusCode;
            environment["owin.ResponseReasonPhrase"] = response.ReasonPhrase;
            environment["owin.ResponseProtocol"] = request.Protocol;

            environment["owin.CallCancelled"] = lifetime.RequestAborted;
            environment["owin.Version"] = "1.1";

            environment["server.RemoteIpAddress"] = connection?.RemoteIpAddress.ToString();
            environment["server.RemotePort"] = connection?.RemotePort.ToString();
            environment["server.LocalIpAddress"] = connection?.LocalIpAddress.ToString();
            environment["server.LocalPort"] = connection?.LocalPort.ToString();
            environment["server.User"] = authentication?.User;

            return environment;
        }

        public Task ProcessRequestAsync(Dictionary<string, object> environment)
        {
            return Application.Invoke(environment);
        }

        public void DisposeContext(Dictionary<string, object> environment, Exception exception)
        {
            EnvironmentPool.Return(environment);
        }
    }
}
