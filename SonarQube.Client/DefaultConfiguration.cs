using SonarQube.Client.Api;

namespace SonarQube.Client
{
    public static class DefaultConfiguration
    {
        public static void Configure(RequestFactory requestFactory)
        {
            requestFactory
                .RegisterRequest<IAuthValidateRequest, Api.V3_30.AuthValidateRequest>("3.3")
                .RegisterRequest<IServerVersionRequest, Api.V2_10.ServerVersionRequest>("2.10");
        }
    }
}
