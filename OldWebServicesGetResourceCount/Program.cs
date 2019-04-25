using System;
using System.IO;
using System.Text;
using ProjectorAutomation.ProjectorWebServicesV2;

namespace ProjectorAutomation
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
            Session session = new Session(ref pwsProjectorServices, account, username, password);
            string sessionKey = session.getSessionTicket();
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
            Console.WriteLine("\n============ User Details ============\n");
            Console.WriteLine("User:");
            Console.WriteLine(pwsUsers[0].ResourceIdentity.ResourceDisplayName);
            Console.WriteLine("\nEmail Address:");
            Console.WriteLine(pwsUsers[0].UserDetail.EmailAddress);
            Console.WriteLine("\nMobile:");
            Console.WriteLine(pwsUsers[0].UserDetail.MobilePhone);
            Console.WriteLine("\nStart Date:");
            Console.WriteLine(pwsUsers[0].UserDetail.StartDate.ToString());
            Console.WriteLine("\nTime Zone:");
            Console.WriteLine(pwsUsers[0].UserDetail.TimeZoneIdentity.TimeZoneIdentifier);
            Console.WriteLine("\n======================================\n");
            //DumpXML(getUserRs);

            //Get existing timecard for user and total year to date time off
            //Create timecard request
            PwsGetTimeCardsRq timeCardsRq = new PwsGetTimeCardsRq();
            timeCardsRq.SessionTicket = sessionKey;
            timeCardsRq.StartDate = DateTime.Parse("2019-04-24T00:00:00Z").ToUniversalTime();
            timeCardsRq.EndDate = DateTime.Parse("2019-04-25T00:00:00Z").ToUniversalTime();
            timeCardsRq.ResourceIdentity = pwsUsers[0].ResourceIdentity;

            //Send request and check response status
            PwsGetTimeCardsRs timeCardsRs = pwsProjectorServices.PwsGetTimeCards(timeCardsRq);
            Console.WriteLine(timeCardsRs.Status);

            //Iterate through time off projects and total YTD timeoff
            PwsTimeEntryTimeOff[] pwsTimeEntryProject = timeCardsRs.TimeEntryTimeOff;
            int minYtd = 0;
            foreach (PwsTimeEntryTimeOff timeOff in pwsTimeEntryProject)
            {
                minYtd += timeOff.MinutesYearToDate;
            }
            Console.WriteLine(minYtd);

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
    }
}
