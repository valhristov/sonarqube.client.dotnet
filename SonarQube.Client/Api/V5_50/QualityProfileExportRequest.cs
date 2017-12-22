using System;
using System.Collections.Generic;
using System.Text;

namespace SonarQube.Client.Api.V5_50
{
    public class QualityProfileExportRequest : RequestBase<RoslynQualityProfile>, IQualityProfileExportRequest
    {
        protected override string Path => "api/qualityprofile/export";

        protected override RoslynQualityProfile ParseResponse(string response)
        {
            throw new NotImplementedException();
        }
    }
}
