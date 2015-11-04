using System;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;

using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Auth.OAuth2.Mvc;
using Google.Apis.Drive.v2;
using Google.Apis.Drive.v2.Data;
using Google.Apis.Util.Store;

namespace custom_study_plan_generator.App_Start
{
    public class AppAuthFlowMetadata : FlowMetadata
    {
        //this has been introduced to fix iis permission error on google appdata folder on server
        static string folder = System.Web.HttpContext.Current.Server.MapPath("/App_Data/cust_study_plans_googlestorage");

        private static readonly IAuthorizationCodeFlow flow =
            new GoogleAuthorizationCodeFlow(new GoogleAuthorizationCodeFlow.Initializer
            {


                ClientSecrets = new ClientSecrets
                {
                    ClientId = "623863401464-a141i4uk8boeu1bt0v2bjhmbkat6914l.apps.googleusercontent.com",
                    ClientSecret = "M9VrJ6WN9s_9FXLFfpgttwuO"
                },

                Scopes = new[] { DriveService.Scope.Drive, "https://spreadsheets.google.com/feeds" },

                DataStore = new FileDataStore(folder)
            });

        public override string GetUserId(Controller controller)
        {
            return controller.User.Identity.GetUserName();
        }

        public override IAuthorizationCodeFlow Flow
        {
            get { return flow; }
        }
    }
}