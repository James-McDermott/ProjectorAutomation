using System;
using ProjectorAutomation.ProjectorWebServicesV2;

namespace ProjectorAutomation
{
    public class Session
    {
        PwsProjectorServices psc;
        string accountCode, userName, password;

        public Session(ref PwsProjectorServices psc, string accountCode, string userName, string password)
        {
            this.psc = psc;
            this.accountCode = accountCode;
            this.userName = userName;
            this.password = password;
        }

        public string getSessionTicket()
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

            //If a RedirectUrl was returned then account data is on a different server. Retry with new url.
            if (rs.RedirectUrl != null)
            {
                Uri uri = new Uri(psc.Url);
                psc.Url = rs.RedirectUrl + uri.LocalPath;
                return getSessionTicket();
            }

            return rs.SessionTicket;
        }
    }
}
