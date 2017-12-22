using System;
using System.Diagnostics;
using System.Net.Http.Headers;
using System.Security;
using System.Text;

namespace SonarQube.Client.Helpers
{
    public static class AuthenticationHeaderFactory
    {
        internal const string BasicAuthCredentialSeparator = ":";

        /// <summary>
        /// Encoding used to create the basic authentication token
        /// </summary>
        internal static readonly Encoding BasicAuthEncoding = Encoding.UTF8;

        public static AuthenticationHeaderValue Create(string username, SecureString password)
        {
            return string.IsNullOrWhiteSpace(username)
                ? null
                : new AuthenticationHeaderValue("Basic", GetBasicAuthToken(username, password));
        }

        internal static string GetBasicAuthToken(string user, SecureString password)
        {
            if (!string.IsNullOrEmpty(user) && user.Contains(BasicAuthCredentialSeparator))
            {
                // See also: http://tools.ietf.org/html/rfc2617#section-2
                Debug.Fail("Invalid user name: contains ':'");
                throw new ArgumentOutOfRangeException(nameof(user));
            }

            return Convert.ToBase64String(BasicAuthEncoding.GetBytes(
                string.Join(BasicAuthCredentialSeparator, user, password.ToUnsecureString())));
        }
    }
}
