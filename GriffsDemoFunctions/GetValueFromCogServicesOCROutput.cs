
using System.IO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs.Host;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace GriffsDemoFunctions
{
    public static class GetValueFromCogServicesOCROutput
    {
        [FunctionName("GetValueFromCogServicesOCROutput")]
        public static IActionResult Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)]HttpRequest req, TraceWriter log)
        {
            log.Info("C# HTTP trigger function processed a request.");

            string requestBody = new StreamReader(req.Body).ReadToEnd();
            dynamic data = JsonConvert.DeserializeObject(requestBody);

            string output = "unknown";
            int startPosition = 397;
            int endPosition = 436;

            dynamic lines = data.recognitionResult.lines;

            foreach (dynamic line in lines)
            {
                if (line.boundingBox[0] >= startPosition && line.boundingBox[0] <= endPosition)
                {
                    output = line["text"];
                }
            }

            return (ActionResult)new OkObjectResult($"PassportID: {output}");
        }
    }
}
