using System;
using OldWebServicesGetResourceCount.ProjectorWebServicesV2;

namespace OldWebServicesGetResourceCount
{
    class Program
    {
        public static void Main(string[] args)
        {
            //Initialize necessary user details. Need added progrommatically as readline inconsistent
            PwsProjectorServices pwsProjectorServices = new PwsProjectorServices();
            //Console.WriteLine("Username:");
            string username = "CHANGEME";
            //Console.WriteLine("Password:");
            string password = "CHANGEME";
            //Console.WriteLine("Account:");
            string account = "CHANGEME";
            //Authenticate user and print session key
            string sessionKey = Authenticate(ref pwsProjectorServices, account, username, password);
            Console.WriteLine(sessionKey);
            //Using session key create and expense report request, and print out request status
            PwsGetExpenseReportsRq pwsGetExpenseReportsRq = new PwsGetExpenseReportsRq();
            pwsGetExpenseReportsRq.SessionTicket = sessionKey;
            PwsGetExpenseReportsRs pwsGetExpenseReportsRs = pwsProjectorServices.PwsGetExpenseReports(pwsGetExpenseReportsRq);
            Console.WriteLine(pwsGetExpenseReportsRs.Status);

            //Get some user details from user name and print to console. Needs added progrommatically as readline inconsistent
            PwsGetUserRq getUserRq = new PwsGetUserRq();
            getUserRq.SessionTicket = sessionKey;
            PwsUserRef userRef = new PwsUserRef();
            userRef.UserDisplayName = "CHANGEME";
            PwsUserRef[] userArray = new PwsUserRef[] { userRef };
            getUserRq.UserIdentities = userArray;
            PwsGetUserRs getUserRs = pwsProjectorServices.PwsGetUser(getUserRq);
            Console.WriteLine(getUserRs.Status);
            PwsUserElement[] pwsUsers = getUserRs.Users;
            Console.WriteLine("User:");
            Console.WriteLine(pwsUsers[0].ResourceIdentity.ResourceDisplayName);
            Console.WriteLine("Email Address:");
            Console.WriteLine(pwsUsers[0].UserDetail.EmailAddress);
            Console.WriteLine("Mobile:");
            Console.WriteLine(pwsUsers[0].UserDetail.MobilePhone);
            Console.WriteLine("Start Date:");
            Console.WriteLine(pwsUsers[0].UserDetail.StartDate.ToString());
            Console.WriteLine("Time Zone:");
            Console.WriteLine(pwsUsers[0].UserDetail.TimeZoneIdentity.TimeZoneIdentifier);
        }

        private static string Authenticate(ref PwsProjectorServices psc, string accountCode, string userName, string password)
        {
            PwsAuthenticateRs rs = psc.PwsAuthenticate(new PwsAuthenticateRq()
            {
                AccountCode = accountCode,
                UserName = userName,
                Password = password
            });

            //If request fails bounce out
            if (rs.Status != RequestStatus.Ok)
            {
                return null;
            }

            //To prevent infinite recursion, only try to reconnect if RedirectUrl is different from current url
            if (rs.RedirectUrl != null && psc.Url.StartsWith(rs.RedirectUrl))
            {
                return null;
            }

            //If a RedirectUrl was returned then your account data is on a different server. Retry with new url.
            if (rs.RedirectUrl != null)
            {
                Uri uri = new Uri(psc.Url);
                psc.Url = rs.RedirectUrl + uri.LocalPath;
                return Authenticate(ref psc, accountCode, userName, password);
            }

            return rs.SessionTicket;
        }
    }
}
