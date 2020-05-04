using System.Globalization;
using System.Linq;
using System;
using System.Collections.Generic;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace DeevCorp.Function
{
    public static class TimerTrigger
    {
        [FunctionName("TimerTrigger")]
        public static void Run(
            [TimerTrigger("0 */1 * * * *")]
            TimerInfo myTimer,
            [CosmosDB(
                Constants.COSMOS_DB_DATABASE_NAME,
                Constants.COSMOS_DB_CONTAINER_NAME,
                ConnectionStringSetting = "CosmosDBConnection")
            ]
            IEnumerable<dynamic> users,
            ILogger log)
        {

            // timer executes every day at 12:00am
            // loop through db and check if any date matches current day
            var usersWithBirthdateToday = new List<dynamic>();

            foreach (var user in users)
            {
                var userBirthdate = (DateTime)user.birthdate;

                if (userBirthdate.Month == DateTime.Today.Month &&
                  userBirthdate.Day == DateTime.Today.Day)
                {
                    usersWithBirthdateToday.Add(user);
                }
            }

            if (usersWithBirthdateToday.Count == 0) return;

            usersWithBirthdateToday.ForEach(user =>
            {
                log.LogInformation("Happy Birthday: {0}", (string)user.name.firstName);
            });
        }
    }
}
