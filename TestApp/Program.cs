using System;
using System.Text;
using Amazon;
using Amazon.Lambda.Model;
using Amazon.Runtime;
using Amazon.Runtime.CredentialManagement;
using AWSUtility.Lambda;
using AWSUtility.Credentials;
using System.Collections.Generic;

namespace TestApp
{
    class Program
    {
        static void Main(string[] args)
        {
            var credentials = ProfileClient.GetCredentialsByProfileName("kmedaradev-devrole-login");
            var lambdaClient = new AWSLambdaClient(credentials, RegionEndpoint.USEast1);

            List<string> functions = new List<string>();

            functions.Add("sap-core-delete-salesorder");
            functions.Add("sap-core-delete-salesorder-item");
            functions.Add("sap-core-get-contract-header-items");
            functions.Add("sap-core-get-customerpartners-expands");
            functions.Add("sap-core-get-customers");
            functions.Add("sap-core-get-materials");
            functions.Add("sap-core-get-plants");
            functions.Add("sap-core-post-agvance-order");
            functions.Add("sap-core-post-salesorder");
            functions.Add("sap-core-post-salesorder-item");
            functions.Add("sap-core-put-salesorder-item");
            functions.Add("sap-core-get-salesorder-headeritems");
           


            foreach(string func in functions)
            {
                lambdaClient.DeleteUnassignedFunctionVersions(func);
            }

            
        }
    }
}
