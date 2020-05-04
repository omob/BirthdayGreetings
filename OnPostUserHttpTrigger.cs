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
using Newtonsoft.Json.Serialization;


namespace DeevCorp.Function
{
    public static class PostUser
    {
        [FunctionName("PostUser")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "users")] HttpRequest req,
            [CosmosDB(
                Constants.COSMOS_DB_DATABASE_NAME,
                Constants.COSMOS_DB_CONTAINER_NAME,
                ConnectionStringSetting = "CosmosDBConnection")]
            IAsyncCollector<dynamic> users,
            [CosmosDB(
                Constants.COSMOS_DB_DATABASE_NAME,
                Constants.COSMOS_DB_CONTAINER_NAME,
                ConnectionStringSetting = "CosmosDBConnection")
            ]
            IEnumerable<dynamic> usersInDb,
            ILogger log)
        {
            log.LogInformation("Add user request processed");

            string requestBody = new StreamReader(req.Body).ReadToEnd();
            var userInfo = JsonConvert.DeserializeObject<User>(requestBody);
            log.LogInformation($"Received: {userInfo}");

            // check if user exists in db
            var userExists = usersInDb.SingleOrDefault(user => user.phoneNumber == userInfo.PhoneNumber);

            if (userExists != null)
                return new BadRequestObjectResult("A user with same phone number already exist");

            dynamic userObject = new
            {
                name = new
                {
                    firstName = userInfo.FirstName,
                    lastName = userInfo.LastName
                },
                birthdate = userInfo.BirthDate,
                messages = new List<string> {
                    userInfo.Message
                },
                phoneNumber = userInfo.PhoneNumber
            };

            await users.AddAsync(userObject);
            return (ActionResult)new OkResult();
        }
    }
}
