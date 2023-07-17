using CommonHelper;
using System.DirectoryServices;
using System.Runtime.Versioning;
using System.Security.Principal;
using System.Web;
using WebCommonHelper.Entities;
using WebCommonHelper.Entities.Enum;
using WebCommonHelper.Models;

namespace WebCommonHelper.Services.Authenticaiton
{
    [SupportedOSPlatform("windows")]
    public class LdapIdentity : IIdentity
    {
        private const string ObjectSidAttribute = "objectSid";
        private const string UserIdAttribute = "samaccountname";
        private const string FullNameAttribute = "displayname";
        private const string FirstNameAttribute = "givenname";
        private const string LastNameAttribute = "sn";
        private const string MailAttribute = "mail";
        private const string CompanyAttribute = "company";
        private const string OfficeAttribute = "physicaldeliveryofficename";
        private const string JobTitleAttribute = "title";

        private UnauthorizedAccessException unauthorizedException { get; set; }

        public User Authenticate(AuthenticateRequest model)
        {
            User user = new User(
                model.UserCompany,
                model.UserAccount);

            DirectoryEntry entry = new DirectoryEntry();

            try
            {
                if (model.UserCompany == Company.EVA)
                {
                    entry = new DirectoryEntry(
                        path: "LDAP://evaair.com",
                        username: $@"evaair.com\{model.UserAccount}",
                        password: model.Password);
                }
                else if (model.UserCompany == Company.UNI)
                {
                    entry = new DirectoryEntry(
                         path: $@"LDAP://uniair.com.tw/DC=uniair,DC=com,DC=tw",
                         username: $@"UNIAIR.com.tw\{model.UserAccount}",
                         password: model.Password);
                }

                object? objectSid = entry.Properties[ObjectSidAttribute].Value;
                if (objectSid == null)
                {
                    throw new UnauthorizedAccessException("Account or password incorrect.");  //帳號密碼錯誤
                }

                string validUsers = new SecurityIdentifier((byte[])objectSid, 0).Value;
                if (validUsers != null)
                {
                    string encodeAccount = FortifySecurityUtils.PathManipulationUrl(model.UserAccount);

                    DirectorySearcher searcher = new DirectorySearcher(entry);
                    searcher.Filter = $@"{UserIdAttribute}={encodeAccount}";
                    searcher.PropertiesToLoad.Add(FullNameAttribute);
                    searcher.PropertiesToLoad.Add(FirstNameAttribute);
                    searcher.PropertiesToLoad.Add(LastNameAttribute);
                    searcher.PropertiesToLoad.Add(MailAttribute);
                    searcher.PropertiesToLoad.Add(CompanyAttribute);
                    searcher.PropertiesToLoad.Add(OfficeAttribute);
                    searcher.PropertiesToLoad.Add(JobTitleAttribute);

                    SearchResult? result = searcher.FindOne();
                    if (result != null)
                    {
                        foreach (string key in result.Properties.PropertyNames)
                        {
                            foreach (var propertyValue in result.Properties[key])
                            {
                                switch (key.ToLower())
                                {
                                    case FullNameAttribute:
                                        user.FullName += propertyValue;
                                        break;
                                    case FirstNameAttribute:
                                        user.FirstName += propertyValue;
                                        break;
                                    case LastNameAttribute:
                                        user.LastName += propertyValue;
                                        break;
                                    case CompanyAttribute:
                                        user.CompanyName += propertyValue;
                                        break;
                                    case OfficeAttribute:
                                        user.OfficeCode += propertyValue;
                                        break;
                                    case MailAttribute:
                                        user.Mail += propertyValue;
                                        break;
                                    case JobTitleAttribute:
                                        user.JobTitle += propertyValue;
                                        break;
                                }
                            }
                        }
                    }
                    searcher.PropertiesToLoad.Clear();
                    searcher.Dispose();
                }

            }
            catch
            {
                unauthorizedException = new UnauthorizedAccessException("Account or password incorrect.");  //帳號密碼錯誤
            }
            finally
            {
                entry.Dispose();

                if (unauthorizedException != null)
                {
                    throw unauthorizedException;
                }
            }


            return user;
        }
    }
}
