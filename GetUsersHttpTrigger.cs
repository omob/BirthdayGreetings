using System.Collections.Generic;
using System.Reflection.Metadata;
using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Collections;

namespace DeevCorp.Function
{
    public static class GetUsers
    {
        [FunctionName("GetUsers")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "users")]
            HttpRequest req,
            [CosmosDB(
                Constants.COSMOS_DB_DATABASE_NAME,
                Constants.COSMOS_DB_CONTAINER_NAME,
                ConnectionStringSetting = "CosmosDBConnection")
            ]
            IEnumerable<dynamic> users,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            return new OkObjectResult(users);
        }
    }
}
