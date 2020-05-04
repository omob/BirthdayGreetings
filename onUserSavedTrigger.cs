using System;
using System.Collections.Generic;
using Microsoft.Azure.Documents;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace DeevCorp.Function
{
    public static class onUserSavedTrigger
    {
        [FunctionName("onUserSavedTrigger")]
        public static void Run(
            [CosmosDBTrigger(
                databaseName: "BirthdayWishes",
                collectionName: "Users",
                ConnectionStringSetting = "CosmosDBConnection",
                LeaseCollectionName = "leases",
            CreateLeaseCollectionIfNotExists = true)]
            IReadOnlyList<Document> users,
            ILogger log)
        {
            if (users != null && users.Count > 0)
            {
                log.LogInformation("A new user document has been created" + users[0]);
            }
        }
    }
}
