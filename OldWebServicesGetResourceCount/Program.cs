using System;
using OldWebServicesGetResourceCount.ProjectorWebServicesV2;

namespace OldWebServicesGetResourceCount
{
    class Program
    {
        public static void Main(string[] args)
        {
            PwsProjectorServices pwsProjectorServices = new PwsProjectorServices();
            Console.WriteLine("Username:");
            string username = Console.ReadLine();
            Console.WriteLine("Password:");
            string password = Console.ReadLine();
            Console.WriteLine("Account:");
            string account = Console.ReadLine();
            Console.WriteLine(Authenticate(ref pwsProjectorServices, account, username, password));
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
