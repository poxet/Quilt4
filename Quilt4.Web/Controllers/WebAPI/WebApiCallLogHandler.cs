using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Quilt4.Web.Business;

//using Tharga.Quilt4Net.Web.Business;

namespace Tharga.Quilt4Net.Web.Controllers.WebAPI
{
    public class WebApiCallLogHandler : DelegatingHandler
    {
        private readonly ISettingsBusiness _settingsBusiness;

        public WebApiCallLogHandler(ISettingsBusiness settingsBusiness)
        {
            _settingsBusiness = settingsBusiness;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var requestContent = string.Empty;
            if (request.Content != null)
            {
                requestContent = await request.Content.ReadAsStringAsync();
                //if (Settings.LogAllRequests())
                if (_settingsBusiness.GetSetting<bool>("LogAllRequests"))
                    Issue.Register("Web API Request", Issue.MessageIssueLevel.Information, data: new Dictionary<string, string> { { "Request", requestContent } });
            }

            var response = await base.SendAsync(request, cancellationToken);
            if (response.Content != null)
            {
                //if (Settings.LogAllResponses())
                if (_settingsBusiness.GetSetting<bool>("LogAllResponses"))
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    Issue.Register("Web API Response", Issue.MessageIssueLevel.Information, data: new Dictionary<string, string> { { "Response", responseContent }, { "Request", requestContent } });
                }
            }

            return response;
        }
    }
}