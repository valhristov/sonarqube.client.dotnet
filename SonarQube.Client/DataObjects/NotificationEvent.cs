using System;

namespace SonarQube.Client
{
    public class NotificationEvent
    {
        public string Category { get; set; }
        public string Message { get; set; }
        public Uri Link { get; set; }
        public string ProjectKey { get; set; }
    }
}
