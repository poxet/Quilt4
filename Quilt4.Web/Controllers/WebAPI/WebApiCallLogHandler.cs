using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Quilt4.Interface;

namespace Quilt4.Web.Controllers.WebAPI
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
            throw new NotImplementedException();
            //var requestContent = string.Empty;
            //if (request.Content != null)
            //{
            //    requestContent = await request.Content.ReadAsStringAsync();
            //    if (_settingsBusiness.GetConfigSetting<bool>("LogAllRequests"))
            //        Issue.Register("Web API Request", Issue.MessageIssueLevel.Information, data: new Dictionary<string, string> { { "Request", requestContent } });
            //}

            //var response = await base.SendAsync(request, cancellationToken);
            //if (response.Content != null)
            //{
            //    if (_settingsBusiness.GetConfigSetting<bool>("LogAllResponses"))
            //    {
            //        var responseContent = await response.Content.ReadAsStringAsync();
            //        Issue.Register("Web API Response", Issue.MessageIssueLevel.Information, data: new Dictionary<string, string> { { "Response", responseContent }, { "Request", requestContent } });
            //    }
            //}

            //return response;
        }
    }
}