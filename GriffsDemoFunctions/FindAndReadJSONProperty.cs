
using System.IO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs.Host;
using Newtonsoft.Json;

namespace GriffsDemoFunctions
{
    public static class FindAndReadJSONProperty
    {
        [FunctionName("FindAndReadJSONProperty")] //Consider adding handling for integers and objects, dictionaries and arrays
        public static IActionResult Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = "FindAndReadJSONProperty/{PropertyName}")]HttpRequest req, string PropertyName, TraceWriter log)
        {
            log.Info("JSON received. Parsing begins...");

            string PropertyValue = null;

            string requestBody = new StreamReader(req.Body).ReadToEnd();

            bool takeValue = false;

            using (var reader = new JsonTextReader(new StringReader(requestBody)))
            {
                while (reader.Read())
                {
                    if (takeValue)
                    {
                        PropertyValue = reader.Value.ToString();
                        takeValue = false;
                    }
                    else if (reader.TokenType.ToString() == "PropertyName")
                    {
                        if (reader.Value.ToString() == PropertyName)
                        {
                            takeValue = true;
                        }
                    }
                }
            }

            log.Info("Parsing ends. Returning matching property's value.");

            return PropertyValue != null
                ? (ActionResult)new OkObjectResult($"{PropertyValue}")
                : new BadRequestObjectResult("No matching PropertyName found. Please pass a valid PropertyName parameter in the URL (api/FindAndReadJSONProperty/{PropertyName}) and a valid JSON in the request body");
        }
    }
}
