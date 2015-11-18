  [Authorize]
        public async Task<ActionResult> submitPlanAsync(CancellationToken cancellationToken)
        {
            ViewBag.Message = "Plan Submission Page.";

            var step1 = Session["Step1"] as FileModel;

            var step2 = Session["StudyPlan"] as StudyPlanModel;

            Session.Remove("Step1");

            Session.Remove("StudyPlan");

            var result = await new AuthorizationCodeMvcApp(this, new AppAuthFlowMetadata()).
                    AuthorizeAsync(cancellationToken);

            if (result.Credential == null)
                return new RedirectResult(result.RedirectUri);

            var driveService = new DriveService(new BaseClientService.Initializer
            {
                HttpClientInitializer = result.Credential,
                ApplicationName = "custom-study-plan-generator"
            });


            var folderListReq = driveService.Files.List();
            folderListReq.Fields = "items/title,items/id";
            // Set query
            folderListReq.Q = "mimeType = 'application/vnd.google-apps.folder' and title ='" + StudyPlanModel.StudyPlanDirectory + "' and trashed = false";

            FileList folderList = await folderListReq.ExecuteAsync();


            File returnedFile = null;

            bool? returnedResult = null;

            // Creating spreadsheets api service
            // Spreadsheet api test
            OAuth2Parameters parameters = new OAuth2Parameters()
            {
                AccessToken = result.Credential.Token.AccessToken
            };

            GOAuth2RequestFactory requestFactory = new GOAuth2RequestFactory(null, driveService.ApplicationName, parameters);

            SpreadsheetsService sheetsService = new SpreadsheetsService(driveService.ApplicationName)
            {
                RequestFactory = requestFactory
            };

            // Create Google Apps Script Execution API service.



            //project key in project properties
            string scriptId = "M9QBeBg3n43dSAbJG6RDbedJ7ZTkmZeIJ";

            var sservice = new ScriptService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = result.Credential,
                ApplicationName = "custom-study-plan-generator"
            });



            if (folderList.Items.Count >= 1)
            {
                // If multiple folders with StudyPlanModel.StudyPlanDirectory title always choose first one
                File studyPlanFolder = folderList.Items.First();

                // TODO figure out if page token is necessary here
                var fileListReq = driveService.Files.List();
                fileListReq.Fields = "items/title,items/id";
                // Get all spreadsheets in the studyPlanFolder
                fileListReq.Q = "'" + studyPlanFolder.Id + "' in parents and mimeType = 'application/vnd.google-apps.spreadsheet' and trashed = false";
                FileList fileList = await fileListReq.ExecuteAsync();

                returnedFile = StudyPlanModel.generateGoogleSpreadSheet(driveService, sheetsService, step1.Title, studyPlanFolder.Id, fileList, step2);

                returnedResult = StudyPlanModel.curateGoogleSpreadSheet(returnedFile, scriptId, sservice);

            }
            else
            {

                var folder = StudyPlanModel.createDirectory(driveService, StudyPlanModel.StudyPlanDirectory, "RMIT", "root");

                returnedFile = StudyPlanModel.generateGoogleSpreadSheet(driveService, sheetsService, step1.Title, folder.Id, step2);

                returnedResult = StudyPlanModel.curateGoogleSpreadSheet(returnedFile, scriptId, sservice);

            }


            // Permission args are currently hardcoded. Uncomment and replace STUDENTNUMBER to enable sharing of the file.


            StudyPlanModel.addPermission(driveService, returnedFile.Id, "user", "reader", step2);
            // For javascript sharing popup
            ViewBag.UserAccessToken = result.Credential.Token.AccessToken;
            ViewBag.FileId = returnedFile.Id;
            ViewBag.AlternateLink = returnedFile.AlternateLink;

            return View();

        }