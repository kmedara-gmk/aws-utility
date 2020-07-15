using Amazon;
using Amazon.Lambda;
using Amazon.Lambda.Model;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LambdaUtility
{
    public class AWSLambdaClient
    {
        private readonly AmazonLambdaClient _client;

        public AWSLambdaClient(Amazon.RegionEndpoint region)
        {
            _client = new AmazonLambdaClient(region);
        }


        /// <summary>
        /// Deletes all functions versions not assigned to an alias, does not include $LATEST
        /// </summary>
        //public async Task<DeleteFunctionResponse> DeleteUnassignedFunctionVersions(string functionName)
        //{
        //    var aliases = ListLambdaAliases(functionName).Result.Aliases;
        //    var usedVersions = new List<string>();
               
        //    foreach(AliasConfiguration alias in aliases)
        //    {
        //       usedVersions.Add(alias.FunctionVersion);
        //    }


        //    return ;
        //}

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
        public  DeleteFunctionResponse deleteFunction(string functionName, string qualifier)
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
