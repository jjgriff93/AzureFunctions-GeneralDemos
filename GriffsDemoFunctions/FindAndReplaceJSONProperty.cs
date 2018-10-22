
using System.IO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs.Host;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace GriffsDemoFunctions
{
    public static class FindAndReplaceJSONProperty
    {
        [FunctionName("FindAndReplaceJSONProperty")] //How to include the syntax?
        public static IActionResult Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = "FindAndReplaceJSONProperty/{PropertyName}/{NewValue}")]HttpRequest req, string PropertyName, string NewValue, TraceWriter log)
        {
            log.Info("JSON received. Parsing begins...");

            string editedJSON = null;

            string requestBody = new StreamReader(req.Body).ReadToEnd();

            bool replaceValue = false;
            bool successfullyFoundAndReplaced = false;

            using (var reader = new JsonTextReader(new StringReader(requestBody)))
            {
                while (reader.Read())
                {
                    if (replaceValue)
                    {
                        editedJSON += '"' + NewValue + '"';
                        replaceValue = false;
                        successfullyFoundAndReplaced = true;
                    }
                    else if (reader.TokenType.ToString() == "PropertyName")
                    {
                        if (reader.Value.ToString() == PropertyName)
                        {
                            replaceValue = true;
                            editedJSON += reader.Value;
                        }
                        else
                        {
                            editedJSON += reader.Value;
                        }
                    }
                    else
                    {
                        editedJSON += reader.Value;
                    }
                }
            }

            log.Info("Parsing and replacing ends. Returning edited JSON.");

            return successfullyFoundAndReplaced
                ? (ActionResult)new JsonResult(editedJSON)
                : new BadRequestObjectResult("No matching PropertyName found. Please pass a valid PropertyName parameter in the URL (api/FindAndReadJSONProperty/{PropertyName}) and a valid JSON in the request body");
        }
    }
}
