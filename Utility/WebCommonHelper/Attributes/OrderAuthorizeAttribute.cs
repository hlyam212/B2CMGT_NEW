using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using WebCommonHelper.Entities;

namespace WebCommonHelper
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class OrderAuthorizeAttribute : Attribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            // skip authorization if action is decorated with [AllowAnonymous] attribute
            var allowAnonymous = context.ActionDescriptor.EndpointMetadata.OfType<AllowAnonymousAttribute>().Any();
            if (allowAnonymous) return;

            // authorization
            var order = (Application?)context.HttpContext.Items["Order"];
            if (order == null)
            {
                // not logged in
                context.Result = new JsonResult(new
                {
                    message = "Unauthorized"
                })
                {
                    StatusCode = StatusCodes.Status401Unauthorized
                };
            }
        }
    }
}
