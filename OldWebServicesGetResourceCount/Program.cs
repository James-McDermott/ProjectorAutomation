using System;
using System.IO;
using System.Text;
using ProjectorAutomation.ProjectorWebServicesV2;

namespace ProjectorAutomation
{
    class Program
    {
        private static PwsProjectorServices pwsProjectorServices;

        public static void Main(string[] args)
        {
            pwsProjectorServices = new PwsProjectorServices();
            string sessionKey = GenerateSessionKey(pwsProjectorServices, "replaceThis", "replaceThis", "replaceThis");

            //Using session key create and expense report request, and print out request status
            PwsGetExpenseReportsRs pwsGetExpenseReportsRq = GetExpenseReport(sessionKey);

            //Get some user details from user name 
            var userDetails = GetUserDetails("replaceThis", sessionKey);
            //DumpXML(getUserRs);

            //Get existing timecard for user and total year to date time off
            var timeCard=GetTimeCard(sessionKey, userDetails);

            var timeOffDuringYearToDate = CalculateTimeOffInYearToDate(timeCard);

            ////Create a new time card - WIP
            //PwsTimecardDetail pwsTimecardDetail = new PwsTimecardDetail();
            ////For a new timecard to be valid the below must be set as a minimum
            //pwsTimecardDetail.WorkMinutes = 450;
            //pwsTimecardDetail.WorkDate = DateTime.Parse("2019-04-24T00:00:00Z").ToUniversalTime();
            //pwsTimecardDetail.CardStatus = "D";

            ////Also needs the below, each needing at least one of the ID types
            //PwsProjectRef pwsProjectRef = new PwsProjectRef();
            //pwsProjectRef.ProjectCode = "";
            //PwsProjectRateTypeRef pwsProjectRateTypeRef = new PwsProjectRateTypeRef();
            //pwsProjectRateTypeRef.ExternalSystemIdentifier = "";
            //PwsProjectTaskRef pwsProjectTaskRef = new PwsProjectTaskRef();
            //pwsProjectTaskRef.ExternalSystemIdentifier = "";
            //PwsProjectRoleRef pwsProjectRoleRef = new PwsProjectRoleRef();
            //pwsProjectRoleRef.ExternalSystemIdentifier = "";

            //pwsTimecardDetail.ProjectIdentity = pwsProjectRef;
            //pwsTimecardDetail.ProjectRateTypeIdentity = pwsProjectRateTypeRef;
            //pwsTimecardDetail.ProjectTaskIdentity = pwsProjectTaskRef;
            //pwsTimecardDetail.RoleIdentity = pwsProjectRoleRef;

            //PwsTimecardDetail[] timeCards = { pwsTimecardDetail };

            //PwsSaveTimeCardsRq timeCardsRq = new PwsSaveTimeCardsRq();
            //timeCardsRq.SessionTicket = sessionKey;
            //timeCardsRq.SaveTimeCards = timeCards;
            //timeCardsRq.ResourceIdentity = pwsUsers[0].ResourceIdentity;
            //timeCardsRq.StartDate = DateTime.Parse("2019-04-24T00:00:00Z").ToUniversalTime();
            //timeCardsRq.EndDate = DateTime.Parse("2019-04-25T00:00:00Z").ToUniversalTime();

            //PwsSaveTimeCardsRs saveTimeCardsRs = pwsProjectorServices.PwsSaveTimeCards(timeCardsRq);
            //Console.WriteLine(saveTimeCardsRs.SubmittedFlag.ToString());

            //DumpXML(timeCardsRq);
        }

        private static int CalculateTimeOffInYearToDate(PwsGetTimeCardsRs timeCard)
        {
            //Iterate through time off projects and total YTD timeoff
            PwsTimeEntryTimeOff[] pwsTimeEntryProject = timeCard.TimeEntryTimeOff;
            int minYtd = 0;

            foreach (PwsTimeEntryTimeOff timeOff in pwsTimeEntryProject)
            {
                minYtd += timeOff.MinutesYearToDate;
            }

            return minYtd;
        }

        private static PwsGetTimeCardsRs GetTimeCard(string sessionKey, PwsUserElement userDetails)
        {
            //Create timecard request
            PwsGetTimeCardsRq timeCardsRq = new PwsGetTimeCardsRq();
            timeCardsRq.SessionTicket = sessionKey;
            timeCardsRq.StartDate = DateTime.Parse("2019-04-24T00:00:00Z").ToUniversalTime();
            timeCardsRq.EndDate = DateTime.Parse("2019-04-25T00:00:00Z").ToUniversalTime();
            timeCardsRq.ResourceIdentity = userDetails.ResourceIdentity;

            //Send request and check response status
            return pwsProjectorServices.PwsGetTimeCards(timeCardsRq);
        }

        /* 
        *Dump the xml of the request/response passed to it
        *This is a very large xml, for debug only       
        *        
        * requestOrResponseObject - the response/request to be dumped to XML       
        */
        private static void DumpXML(object requestOrResponseObject)
        {
            var serxml = new System.Xml.Serialization.XmlSerializer(requestOrResponseObject.GetType());
            var ms = new MemoryStream();
            serxml.Serialize(ms, requestOrResponseObject);
            string xml = Encoding.UTF8.GetString(ms.ToArray());
            Console.WriteLine(xml);
            Console.WriteLine("");
        }

        private static string GenerateSessionKey(PwsProjectorServices pwsProjectorServices, string username, string password, string accountCode)
        {
            //Authenticate user and print session key
            Session session = new Session(ref pwsProjectorServices, accountCode, username, password);
            string sessionKey = session.GetSessionTicket();
            return sessionKey;
            //Using session key create and expense report request, and print out request status
        }

        private static PwsGetExpenseReportsRs GetExpenseReport(string sessionKey)
        {
            //Using session key create and expense report request, and print out request status
            PwsGetExpenseReportsRq pwsGetExpenseReportsRq = new PwsGetExpenseReportsRq();
            pwsGetExpenseReportsRq.SessionTicket = sessionKey;
            return pwsProjectorServices.PwsGetExpenseReports(pwsGetExpenseReportsRq);
        }

        private static PwsUserElement GetUserDetails(string userName, string sessionKey)
        {
            PwsGetUserRq getUserRq = new PwsGetUserRq();
            getUserRq.SessionTicket = sessionKey;
            PwsUserRef userRef = new PwsUserRef();
            userRef.UserDisplayName = userName;
            PwsUserRef[] userArray = new PwsUserRef[] { userRef };
            getUserRq.UserIdentities = userArray;
            PwsGetUserRs getUserRs = pwsProjectorServices.PwsGetUser(getUserRq);
            return getUserRs.Users[0];
        }
    }
}
