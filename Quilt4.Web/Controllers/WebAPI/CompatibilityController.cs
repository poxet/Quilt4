﻿using System;
using System.Reflection;
using System.Web.Http;
using Quilt4.Interface;
using Quilt4.Web.DataTransfer;

namespace Quilt4.Web.Controllers.WebAPI
{
    public class CompatibilityController : ApiController
    {
        private readonly ICompatibilityBusiness _compatibilityBusiness;

        public CompatibilityController(ICompatibilityBusiness compatibilityBusiness)
        {
            _compatibilityBusiness = compatibilityBusiness;
        }

        // GET api/compatibility/register
        [HttpPost]
        [Route("api/compatibility/register")]
        [AllowAnonymous]
        public void RegisterCompatibility([FromBody] RegisterCompatibilityRequest request)
        {
            if (string.IsNullOrEmpty(request.SupportToolkitNameVersion)) throw new ArgumentNullException("request", "No SupportToolkitNameVersion provided.");

            var version = Assembly.GetAssembly(typeof(HomeController)).GetName().Version;
            _compatibilityBusiness.RegisterToolkitCompability(version, DateTime.UtcNow, request.SupportToolkitNameVersion, request.Compatible ? Compatibility.Compable : Compatibility.Incompatible);
        }
    }
}