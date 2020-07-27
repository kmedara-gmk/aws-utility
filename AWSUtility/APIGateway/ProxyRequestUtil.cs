using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using Newtonsoft.Json;

namespace AWSUtility.APIGateway
{
    class ProxyRequestUtil
    {
        public object MapQueryStringToSapParameters(object baseParams, PropertyInfo[] parameterProperties, APIGatewayProxyRequest request)
        {
            LambdaLogger.Log("ProxyRequestUtil");
            LambdaLogger.Log(JsonConvert.SerializeObject(baseParams));

            if (request.MultiValueQueryStringParameters != null)
            {
                foreach (var p in baseParams.GetType().GetProperties())
                {
                    if (request.QueryStringParameters.ContainsKey(p.Name) || request.MultiValueQueryStringParameters.ContainsKey(p.Name))
                    {
                        string type = baseParams.GetType().GetProperty(p.Name).PropertyType.ToString().ToUpper();
                       
                        if (type.Contains("LIST"))
                        {
                            baseParams.GetType().GetProperty(p.Name).SetValue(baseParams, request.MultiValueQueryStringParameters[p.Name].ToList());
                        }
                        if (type.Equals("SYSTEM.STRING"))
                        {
                            baseParams.GetType().GetProperty(p.Name).SetValue(baseParams, request.QueryStringParameters[p.Name]);

                        }
                        if (type.Contains("BOOLEAN"))
                        {
                            baseParams.GetType().GetProperty(p.Name).SetValue(baseParams, Convert.ToBoolean(request.QueryStringParameters[p.Name]));
                        }
                    }
                }
            }
            return baseParams;
        }
    }
}
