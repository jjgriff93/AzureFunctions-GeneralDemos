using System.IO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs.Host;
using Newtonsoft.Json;
using Microsoft.Azure.Documents.Client;
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Azure.Documents;

namespace GriffsDemoFunctions
{
    public static class CosmosDBCRUD
    {
        [FunctionName("CosmosDBCRUD_CreateDoc")]
        public static async Task<ActionResult> CreateDoc([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)]HttpRequest req, ILogger log)
        {
            log.LogInformation("JSON recieved for upload to Cosmos DB. Processing...");

            DocumentClient documentClient = new DocumentClient(new Uri(Environment.GetEnvironmentVariable("CosmosDBURI")), Environment.GetEnvironmentVariable("CosmosDBKey"));

            // Get body of HTTP call - JSON document to upload to Cosmos
            string requestBody = new StreamReader(req.Body).ReadToEnd();
            dynamic document = JsonConvert.DeserializeObject(requestBody);

            ResourceResponse<Document> uploadResult;

            if (document.id != null)
            {
                try
                {
                    uploadResult = await documentClient.CreateDocumentAsync(UriFactory.CreateDocumentCollectionUri(Environment.GetEnvironmentVariable("CosmosDBDatabaseName"), Environment.GetEnvironmentVariable("CosmosDBCollectionName")), document);
                    return new OkResult();
                }
                catch (Exception ex)
                {
                    return new BadRequestObjectResult(ex.Message);
                }
            }
            else
            {
                return new BadRequestObjectResult("No valid 'id' field found in the document provided.");
            }
        }
    }
}
