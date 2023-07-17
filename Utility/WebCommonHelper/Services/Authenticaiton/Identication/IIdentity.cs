using WebCommonHelper.Entities;
using WebCommonHelper.Models;

namespace WebCommonHelper.Services.Authenticaiton
{
    public interface IIdentity
    {
        User Authenticate(AuthenticateRequest model);
    }
}
