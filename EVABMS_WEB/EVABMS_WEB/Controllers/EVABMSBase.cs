using Microsoft.AspNetCore.Mvc;
using WebCommonHelper;
using WebCommonHelper.Services.Authenticaiton;
using WebCommonHelper.Services.CallApi;

namespace EVABMS_WEB.Controllers
{
    [Authorize]
    public class EVABMSBase: ControllerBase
    {
        public IConnect connect;
        public IUserService userService;
        private readonly ILogger<AuthorizationController> _logger;
        public const string policyName = "EVABMS_WEB_POLICY";
    }
}
