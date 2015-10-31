using System;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;

using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Auth.OAuth2.Mvc;
using Google.Apis.Drive.v2;
using Google.Apis.Util.Store;

namespace custom_study_plan_generator.App_Start
{
    public class AppAuthFlowMetadata : FlowMetadata
    {
        private static readonly IAuthorizationCodeFlow flow =
            new GoogleAuthorizationCodeFlow(new GoogleAuthorizationCodeFlow.Initializer
            {
                ClientSecrets = new ClientSecrets
                {
                    ClientId = "623863401464-a141i4uk8boeu1bt0v2bjhmbkat6914l.apps.googleusercontent.com",
                    ClientSecret = "M9VrJ6WN9s_9FXLFfpgttwuO"
                },
                Scopes = new[] { DriveService.Scope.Drive },
                DataStore = new FileDataStore("Google.Apis.MVC")
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