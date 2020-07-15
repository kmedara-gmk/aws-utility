using Amazon;
using Amazon.SimpleSystemsManagement;
using Amazon.SimpleSystemsManagement.Model;
using System;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using Amazon.Runtime;

namespace AWSUtility.SSM
{
    public class AWSParameterStoreClient
    {   
        private readonly RegionEndpoint _region;
        private readonly AmazonSimpleSystemsManagementClient _ssmClient;
        private const string CORS_ORIGIN_CONFIG = "CORS_ORIGIN_CONFIG";

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
            string corsConfigOriginStoreParam = GetValueAsync(CORS_ORIGIN_CONFIG).Result;
            var originHeader = "";
            if(Headers.TryGetValue("origin", out originHeader))
            {
                JObject corsConfig = JObject.Parse(corsConfigOriginStoreParam);
                if (!String.IsNullOrEmpty(originHeader) && originHeader.Contains("https://localhost:8080"))
                {
                    return corsConfig["local-secure"].ToString();
                }
                else if (!String.IsNullOrEmpty(originHeader) && originHeader.Contains("http://localhost:8080"))
                {
                    return corsConfig["local-dev"].ToString();
                }
                else if (!String.IsNullOrEmpty(originHeader) && originHeader.Contains("https://fsfleet.gmkdev.com"))
                {
                    return corsConfig["fs-fleet-dev"].ToString();
                }
                return "no-origin";
            } else
            {
                return "no-origin";
            } 
        }
    }
}
