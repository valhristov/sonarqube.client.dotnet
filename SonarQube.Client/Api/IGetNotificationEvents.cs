using System;
using System.Collections.Generic;

namespace SonarQube.Client.Api
{
    public interface IGetNotificationEvents : IRequestBase<List<NotificationEvent>>
    {
        DateTimeOffset Since { get; set; }

        IEnumerable<string> ProjectKeys { get; set; }
    }
}
