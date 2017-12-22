using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace SonarQube.Client.Api.V1_00
{
    public class GetNotificationEvents : RequestBase<List<NotificationEvent>>, IGetNotificationEvents
    {
        [JsonProperty("from")]
        public DateTimeOffset Since { get; set; }

        [JsonProperty("projects")]
        public IEnumerable<string> ProjectKeys { get; set; }

        protected override string Path => "api/developers/search_events";

        protected override List<NotificationEvent> ParseResponse(string response) =>
            JObject.Parse(response)["events"]
                .Cast<JObject>()
                .Select(CreateNotificationEvent)
                .ToList();

        private NotificationEvent CreateNotificationEvent(JObject jobject) =>
            new NotificationEvent
            {
                Category = (string)jobject["category"],
                Message = (string)jobject["message"],
                Link = new Uri((string)jobject["link"]),
                ProjectKey = (string)jobject["project"],
            };
    }
}
