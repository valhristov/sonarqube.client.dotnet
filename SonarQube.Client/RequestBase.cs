using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SonarQube.Client.Helpers;

namespace SonarQube.Client
{
    public abstract class RequestBase<TResponse>
    {
        [JsonIgnore]
        protected abstract string Path { get; }

        protected abstract TResponse ParseResponse(string response);

        public async Task<TResponse> InvokeAsync(HttpClient httpClient, CancellationToken token)
        {
            var query = QueryStringSerializer.ToQueryString(this);

            var pathAndQuery = string.IsNullOrEmpty(query) ? Path : $"{Path}?{query}";

            var httpRequest = new HttpRequestMessage(HttpMethod.Get, new Uri(pathAndQuery, UriKind.Relative));

            var httpResponse = await httpClient.SendAsync(httpRequest, HttpCompletionOption.ResponseHeadersRead, token)
                .ConfigureAwait(false);

            httpResponse.EnsureSuccessStatusCode();

            return await ReadResponse(httpResponse);
        }

        protected virtual async Task<TResponse> ReadResponse(HttpResponseMessage httpResponse)
        {
            var responseString = await httpResponse.Content.ReadAsStringAsync()
                .ConfigureAwait(false);

            return ParseResponse(responseString);
        }
    }
}
