namespace SonarQube.Client.Api.V2_10
{
    public class ServerVersionRequest : RequestBase<string>, IServerVersionRequest
    {
        protected override string Path => "api/server/version";

        protected override string ParseResponse(string response) => response;
    }
}
