using Newtonsoft.Json.Linq;

namespace SonarQube.Client.Api.V3_30
{
    public class AuthValidateRequest : RequestBase<AuthenticationResult>, IAuthValidateRequest
    {
        protected override string Path => "api/authentication/validate";

        protected override AuthenticationResult ParseResponse(string response)
        {
            try
            {
                return new AuthenticationResult
                {
                    IsValid = (bool)JObject.Parse(response).GetValue("valid")
                };
            }
            catch
            {
                return new AuthenticationResult { IsValid = false };
            }
        }
    }
}
