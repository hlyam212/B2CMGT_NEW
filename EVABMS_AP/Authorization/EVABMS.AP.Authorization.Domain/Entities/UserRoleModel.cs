using System.Text.Json.Serialization;

namespace EVABMS.AP.Authorization.Domain.Entities
{
    public class UserRoleModel
    {
        public UserRole Role { get; private set; }
        public List<User> RoleMemberList { get; private set; }
        public List<AuthorizationModel> RoleFunctionList { get; private set; }

        public UserRoleModel() { }

        [JsonConstructor]
        public UserRoleModel(UserRole Role,
                             List<User> RoleMemberList,
                             List<AuthorizationModel> RoleFunctionList)
        {
            this.Role = Role;
            this.RoleMemberList = RoleMemberList;
            this.RoleFunctionList = RoleFunctionList;
        }

        /// <summary>
        /// Create
        /// </summary>
        public static UserRoleModel Create(UserRole Role,
                                           List<User> RoleMemberList,
                                           List<AuthorizationModel> RoleFunctionList)
        {
            UserRoleModel model = new UserRoleModel(Role, RoleMemberList, RoleFunctionList);
            return model;
        }
    }
}
