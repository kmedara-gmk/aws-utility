using Amazon;
using Amazon.Lambda;
using Amazon.Lambda.Model;
using Amazon.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AWSUtility.Lambda
{
    public class AWSLambdaClient
    {
        private readonly AmazonLambdaClient _client;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="credentials"></param>
        /// <param name="region"></param>
        public AWSLambdaClient(AWSCredentials credentials, Amazon.RegionEndpoint region)
        {
            _client = new AmazonLambdaClient(credentials, region);
        }


        /// <summary>
        /// Deletes all functions versions not assigned to an alias, does not include $LATEST
        /// </summary>
        public List<DeleteFunctionResponse> DeleteUnassignedFunctionVersions(string functionName)
        {
            var aliases =  ListLambdaAliases(functionName).Aliases;
            var usedversions = new List<string>();
            IList<DeleteFunctionResponse> response = new List<DeleteFunctionResponse>();

            foreach (AliasConfiguration alias in aliases)
            {
                usedversions.Add(alias.FunctionVersion);
            }

            List<string> allVersions = new List<string>() ;
            ListVersionsByFunction(functionName).Versions.ForEach(config => allVersions.Add(config.Version));

            IEnumerable<string> unusedVersions = allVersions.Except(usedversions);

            unusedVersions
                .ToList()
                .Where(version => version != "$LATEST")
                .ToList()
                .ForEach(version => response.Add(DeleteFunction(functionName, version)));

            return response.ToList();
        }

        /// <summary>
        /// List all aliases of a given function
        /// </summary>
        /// <param name="functionName"></param>
        /// <returns></returns>
        public ListAliasesResponse ListLambdaAliases(string functionName)
        {
            var response = _client.ListAliasesAsync(new ListAliasesRequest
            {
                FunctionName = functionName
            });

            return response.Result;
        }

        /// <summary>
        /// List layers used by function
        /// </summary>
        /// <param name="functionName"></param>
        /// <returns></returns>
        public ListLayersResponse ListLambdaLayers(string functionName)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Deletes function from name/arn and qualifier
        /// use qualifier to specify version
        /// cannot specify $LATEST
        /// Qualifier in required to prevent accidental deletion of entire function
        /// </summary>
        /// <param name="functionName"></param>
        /// <param name="qualifier"></param>
        /// <returns></returns>
        public  DeleteFunctionResponse DeleteFunction(string functionName, string qualifier)
        {
            var response = _client.DeleteFunctionAsync(new DeleteFunctionRequest
            {
                FunctionName = functionName,
                Qualifier = qualifier
            });

            return response.Result;
        }

        public ListVersionsByFunctionResponse ListVersionsByFunction(string functionName, string marker = null, int max = 0)
        {
            var request = new ListVersionsByFunctionRequest();
            request.FunctionName = functionName;
            if (marker != null)
                request.Marker = marker;
            if (max > 0)
                request.MaxItems = max;
            var response =  _client.ListVersionsByFunctionAsync(request);

            return response.Result;
        }
    }

    


    
}
