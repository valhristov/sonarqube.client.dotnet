using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace SonarQube.Client
{
    public interface IRequestBase
    {
    }

    public interface IRequestBase<TResult> : IRequestBase
    {
        Task<TResult> InvokeAsync(HttpClient httpClient, CancellationToken token);
    }
}
