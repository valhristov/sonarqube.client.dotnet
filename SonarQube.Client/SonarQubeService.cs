using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Security;
using System.Threading;
using System.Threading.Tasks;
using SonarQube.Client.Api;
using SonarQube.Client.Helpers;

namespace SonarQube.Client
{
    public class SonarQubeService
    {
        private readonly HttpClient httpClient;
        private readonly RequestFactory requestFactory;

        private string sonarQubeVersion = null;
        private bool connected;

        public SonarQubeService(HttpMessageHandler messageHandler, RequestFactory requestFactory)
        {
            httpClient = new HttpClient(messageHandler);
            this.requestFactory = requestFactory;
        }

        public async Task Connect(Uri baseAddress, string username, SecureString password)
        {
            httpClient.BaseAddress = baseAddress;
            httpClient.DefaultRequestHeaders.Authorization = AuthenticationHeaderFactory.Create(username, password);

            connected = true;

            sonarQubeVersion = await InvokeRequestAsync<IServerVersionRequest, string>(
                CancellationToken.None);

            var result = await InvokeRequestAsync<IAuthValidateRequest, AuthenticationResult>(
                CancellationToken.None);

            if (!result.IsValid)
            {
                throw new InvalidOperationException("Invalid credentials");
            }
        }

        public Task<List<NotificationEvent>> GetNotificationEventsAsync(string projectKey, DateTimeOffset since, CancellationToken token) =>
            InvokeRequestAsync<IGetNotificationEvents, List<NotificationEvent>>(
                request =>
                {
                    request.ProjectKeys = new[] { projectKey };
                    request.Since = since;
                },
                token);

        private Task<TResponse> InvokeRequestAsync<TRequest, TResponse>(CancellationToken token)
            where TRequest : IRequestBase<TResponse>
        {
            return InvokeRequestAsync<TRequest, TResponse>(request => { }, token);
        }

        private async Task<TResponse> InvokeRequestAsync<TRequest, TResponse>(Action<TRequest> configure, CancellationToken token)
            where TRequest : IRequestBase<TResponse>
        {
            if (!connected)
            {
                throw new InvalidOperationException("This operation expects the service to be connected.");
            }

            var request = CreateRequest<TRequest>();
            configure(request);
            return await request.InvokeAsync(httpClient, token);
        }

        private TRequest CreateRequest<TRequest>()
            where TRequest : IRequestBase
        {
            return requestFactory.Create<TRequest>(sonarQubeVersion);
        }
    }
}
