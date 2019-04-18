using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;
using OldWebServicesGetResourceCount.ProjectorWebServicesV1;

namespace OldWebServicesGetResourceCount
{
    class Program
    {
        static void Main(string[] args)
        {
            Program program = new Program();
            Console.WriteLine("Projector console app running....\n");
            //Running

            //create an instance of the Projector Service
            var svc = new OpsProjectorSvcSoapClient();
            var header = new OpsAuthenticationHeader
            {
                AccountName = "yourAccountName",
                EmailAddress = "yourEmailAddress",
                Password = "yourPassword"
            };
            //Authenticate with server and ensure we have the correct Service URL
            var getWebServiceUrlRq = new GetWebServiceUrlRq();
            GetWebServiceUrlRs getWebServiceUrlRs = svc.GetWebServiceUrl(header, getWebServiceUrlRq);
            if (getWebServiceUrlRs.WebServiceUrl != null)
            {
                var newEndpoint = string.Format("{0}/OpsProjectorWebSvc/OpsProjectorSvc.asmx", getWebServiceUrlRs.WebServiceUrl);
                Console.WriteLine("Redirecting your endpoint to: \n" + newEndpoint + "\n");
                svc = new OpsProjectorSvcSoapClient(new BasicHttpsBinding(), new EndpointAddress(newEndpoint));
            }
            //Retrieve some data from the installation using Rq/Rs
            var exportResourcesRq = new ExportResourcesRq
            {
                Parameters = new ExportResourcesRequest
                {
                    OnlyCountRows = true
                }
            };
            ExportResourcesRs exportResourcesRs = svc.ExportResources(header, exportResourcesRq);
            int iResources = exportResourcesRs.Data.RowCount;

            Console.WriteLine("Your installation has " + iResources.ToString() + " active resources as of today.\n");
            Console.WriteLine("Press enter to quit");

            Console.ReadLine();
        }
    }
}
