using Amazon;
using Amazon.SimpleSystemsManagement;
using Amazon.SimpleSystemsManagement.Model;
using System;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using Amazon.Runtime;
using Amazon.Lambda.Core;

namespace AWSUtility.SSM
{
    public class AWSParameterStoreClient
    {   
        private readonly AmazonSimpleSystemsManagementClient _ssmClient;
        private const string CORS_ORIGIN_CONFIG = "CORS_ORIGIN_CONFIG";

        public AWSParameterStoreClient(RegionEndpoint region)
        {
            _ssmClient = new AmazonSimpleSystemsManagementClient(region);
        }

        public AWSParameterStoreClient(AWSCredentials credentials, RegionEndpoint region)
        {
            _ssmClient = new AmazonSimpleSystemsManagementClient(credentials, region);
        }

        /// <summary>
        /// Get Value of SSM parameter by name
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public async Task<string> GetValueAsync(string parameter)
        {
            

            var response = await _ssmClient.GetParameterAsync(new GetParameterRequest
            {
                Name = parameter,
                WithDecryption = true
            });

            return response.Parameter.Value;
        }

        /// <summary>
        /// Specifically for the CORS_ORIGIN_CONFIG
        /// </summary>
        /// <param name="Headers"></param>
        /// <returns></returns>
        public string RetrieveOrigin(IDictionary<string,string> Headers)
        {
            LambdaLogger.Log("ssmClient Retrieve Origin function");
            string corsConfigOriginStoreParam = GetValueAsync(CORS_ORIGIN_CONFIG).Result;
            LambdaLogger.Log("Cors Origin Store SSM Store Parameter values: " + corsConfigOriginStoreParam);

            JObject corsConfig = JObject.Parse(corsConfigOriginStoreParam);
            var originHeader = "";
            var allowedOrigin = "";

            bool foundOrigin = Headers.TryGetValue("Origin", out originHeader);

            if (!foundOrigin)
            {
                foundOrigin = Headers.TryGetValue("origin", out originHeader);
            }

            if (!String.IsNullOrEmpty(originHeader) && originHeader.Contains("https://localhost:8080"))
            {
                allowedOrigin = corsConfig["local-secure"].ToString();
            }
            else if (!String.IsNullOrEmpty(originHeader) && originHeader.Contains("http://localhost:8080"))
            {
                allowedOrigin = corsConfig["local-dev"].ToString();
            }
            else if (!String.IsNullOrEmpty(originHeader) && originHeader.Contains("https://fsfleet.gmkdev.com"))
            {
                allowedOrigin = corsConfig["fs-fleet-dev"].ToString();
            }
            else if (!String.IsNullOrEmpty(originHeader) && originHeader.Contains("https://fsfleet.growmark.com"))
            {
                allowedOrigin = corsConfig["fs-fleet-prod"].ToString();
            }
            else if (!String.IsNullOrEmpty(originHeader) && originHeader.Contains("https://webadmin.gmkdev.com"))
            {
                allowedOrigin = corsConfig["web-admin-dev"].ToString();
            } else 
            {
                allowedOrigin = "no-origin";
            }
            LambdaLogger.Log("ssmClient Allowed Origin: " + allowedOrigin);
            return allowedOrigin;
        }
    }
}
