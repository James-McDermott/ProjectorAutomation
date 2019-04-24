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
