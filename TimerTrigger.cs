using System.Globalization;
using System.Linq;
using System;
using System.Collections.Generic;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace DeevCorp.Function
{
    public static class TimerTrigger
    {
        [FunctionName("TimerTrigger")]
        public static void Run(
            [TimerTrigger("0 */5 * * * *")]
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

            InitializeTwilio();

            usersWithBirthdateToday.ForEach(user =>
            {
                // Send SMS to recipient
                if (user.messages != null && user.messages.Count > 0)
                {
                    var message = $"{user.messages[user.messages.Count - 1]}";

                    try
                    {
                        SendSMS(message, (string)user.phoneNumber, false);

                        log.LogInformation($"Sent message to {user.name.firstName}");
                    }
                    catch (Exception e)
                    {
                        log.LogError(e.Message);
                    }
                }
            });
        }

        public static void SendSMS(string text, string recipient, bool isSending = false)
        {
            if (isSending)
            {
                MessageResource.Create(
                   body: text,
                   from: new Twilio.Types.PhoneNumber(Environment.GetEnvironmentVariable("TwilioNumber")),
                   to: new Twilio.Types.PhoneNumber(recipient)
               );
            }
        }

        public static void InitializeTwilio()
        {
            var twilioAccountSid = Environment.GetEnvironmentVariable("TwilioAccountSid");
            var twilioAuthToken = Environment.GetEnvironmentVariable("TwilioAuthToken");

            TwilioClient.Init(twilioAccountSid, twilioAuthToken);
        }

    }
}
