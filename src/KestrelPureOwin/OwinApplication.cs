using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Http.Features.Authentication;

namespace KestrelPureOwin
{
    public class OwinApplication : IHttpApplication<Dictionary<string, object>>
    {
        public OwinApplication(AppFunc application)
        {
            Application = application;
        }

        private AppFunc Application { get; }

        public Dictionary<string, object> CreateContext(IFeatureCollection features)
        {
            var environment = new Dictionary<string, object>(StringComparer.Ordinal);

            var request = features.Get<IHttpRequestFeature>();
            var response = features.Get<IHttpResponseFeature>();
            var lifetime = features.Get<IHttpRequestLifetimeFeature>();
            var identifier = features.Get<IHttpRequestIdentifierFeature>();
            var authentication = features.Get<IHttpAuthenticationFeature>();

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

            return environment;
        }

        public Task ProcessRequestAsync(Dictionary<string, object> environment)
        {
            return Application.Invoke(environment);
        }

        public void DisposeContext(Dictionary<string, object> environment, Exception exception)
        {
            // TODO: Anything to do here?
        }
    }
}
