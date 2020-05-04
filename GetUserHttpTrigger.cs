using System.Linq;
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
    public static class GetUser
    {
        [FunctionName("GetUser")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "users/{id}")]
            HttpRequest req,
            string id,
            [CosmosDB(
                Constants.COSMOS_DB_DATABASE_NAME,
                Constants.COSMOS_DB_CONTAINER_NAME,
                ConnectionStringSetting = "CosmosDBConnection")
            ]
            IEnumerable<dynamic> users,
            ILogger log)
        {

            log.LogInformation("Get User HTTP trigger function processed a request.");

            var userInDb = users.SingleOrDefault(user => user.id == id);
            if (userInDb == null)
                return new NotFoundResult();

            return new OkObjectResult(userInDb);
        }
    }
}
