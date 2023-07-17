namespace EVABMS_AP.Interface
{
    public class UserRoleDataModel
    {
        public MGT_USER_ROLESDataModel? Userrole { get; set; }
        public List<MGT_USERSDataModel>? Rolememberlist { get; set; }
        public List<AuthorizationDataModel>? Rolefunctionlist { get; set; }
    }
}
