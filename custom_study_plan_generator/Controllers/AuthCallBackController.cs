using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using custom_study_plan_generator.App_Start;

namespace custom_study_plan_generator.Controllers
{
    public class AuthCallbackController :
             Google.Apis.Auth.OAuth2.Mvc.Controllers.AuthCallbackController
    {
        protected override Google.Apis.Auth.OAuth2.Mvc.FlowMetadata FlowData
        {
            get { return new AppAuthFlowMetadata(); }
        }
    }
}