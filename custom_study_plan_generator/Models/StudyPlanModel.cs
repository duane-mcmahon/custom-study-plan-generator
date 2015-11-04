using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using Google.Apis.Drive.v2;
using Google.Apis.Drive.v2.Data;
using File = Google.Apis.Drive.v2.Data.File;
using System.Diagnostics;
using custom_study_plan_generator.MetaObjects;
using Google.Apis.Services;
using Google.GData.Client;
using Google.GData.Spreadsheets;

namespace custom_study_plan_generator.Models
{
    public class StudyPlanModel
    {
        public const string StudyPlanDirectory = "RMITStudentStudyPlans";

        public List<CoursePlan> StudentPlan {

            get; set;
        }

        public string StudentId
        {
            get; set;
        }

        public string CourseCode
        {
           
            get; set;

        }

        public List<ExemptionModel> Exemptions
        {

            get; set;
        
        }

        public int? BeginningSemester { get; set; }




        public static File createDirectory(DriveService service, string _title, string _description, string _parent)
        {

            File NewDirectory = null;

            // Create metaData for a new Directory
            File body = new File();
            body.Title = _title;
            body.Description = _description;
            body.MimeType = "application/vnd.google-apps.folder";
            body.Parents = new List<ParentReference>() { new ParentReference() { Id = _parent } };
            //this may or may not require the try/catch idiom. 

            try
            {
                FilesResource.InsertRequest request = service.Files.Insert(body);
                NewDirectory = request.Execute();
            }

            catch (Exception e)
            {

                Debug.WriteLine(e.Message);

                return null;
            }

            return NewDirectory;
        }



        //generate a google spread sheet from model data in sql database
        //returns the uploaded File result
        public static File generateGoogleSpreadSheet(DriveService service, string studentID, string fileID,
            FileList list, StudyPlanModel u)
        {

            var file = new File();
            file.Title = studentID;
            file.Description = string.Format("Created via {0} at {1}", service.ApplicationName, DateTime.Now.ToString());
            file.MimeType = "application/vnd.google-apps.spreadsheet";
            
            File result = null;
           
            // Set the parent folder.

            file.Parents = new List<ParentReference>() { new ParentReference() { Id = fileID } };



            //check if file with same title exists - it does update, otherwise insert.
            //sample code (untested):

            for (var i = 0; i < list.Items.Count; i++)
            {
                // Doesn't work for a file titled 'Untitled'
                if (list.Items[i].Title == studentID)
                {

                    // File exists in the drive already!
                    // Yes... overwrite the file

                 

                   // var request = service.Files.Update(file, list.Items[i].Id);

                    var delete_requested = service.Files.Delete(list.Items[i].Id); 

                    delete_requested.Execute();

                }

            }

               

                var request = service.Files.Insert(file);

                result = request.Execute();

                populateGoogleSpreadSheet(file, u);


            return result;

        }

        //generate a google spread sheet from model data in sql database
        //returns the uploaded File result
        public static File generateGoogleSpreadSheet(DriveService service, string studentID, string fileID, StudyPlanModel u)
        {

            var file = new File();
            file.Title = studentID;
            file.Description = string.Format("Created via {0} at {1}", service.ApplicationName, DateTime.Now.ToString());
            file.MimeType = "application/vnd.google-apps.spreadsheet";

            // Set the parent folder.
            if (!String.IsNullOrEmpty(fileID))
            {
                file.Parents = new List<ParentReference>() { new ParentReference() { Id = fileID } };
            }


            var request = service.Files.Insert(file);

            var result = request.Execute();

            populateGoogleSpreadSheet(file, u);


            return result;

        }

        //http://www.dreamincode.net/forums/topic/300526-add-row-to-google-spreadsheet/
        //Filling the Spreadsheet
        public static void populateGoogleSpreadSheet(File file, StudyPlanModel uploadable)
        {

            string CLIENT_ID = "623863401464-a141i4uk8boeu1bt0v2bjhmbkat6914l.apps.googleusercontent.com";

            // This is the OAuth 2.0 Client Secret retrieved
            // above.  Be sure to store this value securely.  Leaking this
            // value would enable others to act on behalf of your application!
            string CLIENT_SECRET = "M9VrJ6WN9s_9FXLFfpgttwuO";

            // Space separated list of scopes for which to request access.
            string SCOPE = "https://spreadsheets.google.com/feeds https://docs.google.com/feeds";

            // STEP 2: Set up the OAuth 2.0 object
            ////////////////////////////////////////////////////////////////////////////

            // OAuth2Parameters holds all the parameters related to OAuth 2.0.
            OAuth2Parameters parameters = new OAuth2Parameters();

            // Set your OAuth 2.0 Client Id (which you can register at
            // https://code.google.com/apis/console).
            parameters.ClientId = CLIENT_ID;

            // Set your OAuth 2.0 Client Secret, which can be obtained at
            // https://code.google.com/apis/console.
            parameters.ClientSecret = CLIENT_SECRET;

            // STEP 3: Get the Authorization URL
            ////////////////////////////////////////////////////////////////////////////

            // Set the scope for this particular service.
            parameters.Scope = SCOPE;

            // Get the authorization url.  The user of your application must visit
            // this url in order to authorize with Google.  If you are building a
            // browser-based application, you can redirect the user to the authorization
            // url.
            string authorizationUrl = OAuthUtil.CreateOAuth2AuthorizationUrl(parameters);

            // parameters.AccessCode = 
            // Once the user authorizes with Google, the request token can be exchanged
            // for a long-lived access token.  If you are building a browser-based
            // application, you should parse the incoming request token from the url and
            // set it in OAuthParameters before calling GetAccessToken().
            OAuthUtil.GetAccessToken(parameters);

            string accessToken = parameters.AccessToken;

            // Initialize the variables needed to make the request
            GOAuth2RequestFactory requestFactory =
                new GOAuth2RequestFactory(null, "custom-study-plan-generator", parameters);
            var spreadsheetService = new SpreadsheetsService("custom-study-plan-generator");

            spreadsheetService.RequestFactory = requestFactory;




            // TODO: Authorize the service object for a specific user (see other sections)

            // Instantiate a SpreadsheetQuery object to retrieve spreadsheets.
            SpreadsheetQuery query = new SpreadsheetQuery();

            query.Title = file.Title;

            SpreadsheetFeed feed = spreadsheetService.Query(query);
            SpreadsheetEntry spreadsheet = (SpreadsheetEntry)feed.Entries[0];
            // Create a local representation of the new worksheet.
            WorksheetEntry worksheet = new WorksheetEntry();
            worksheet.Title.Text = "New Worksheet";
            worksheet.Cols = 10;
            worksheet.Rows = 20;

            // Send the local representation of the worksheet to the API for
            // creation.  The URL to use here is the worksheet feed URL of our
            // spreadsheet.
            WorksheetFeed wsFeed = spreadsheet.Worksheets;
            spreadsheetService.Insert(wsFeed, worksheet);

        }

        // Adds a permission to a file. i.e. Allows sharing
        public static void addPermission(DriveService service, string fileID, string value, string type, string role, StudyPlanModel uploadable)
        {
            Permission permission = new Permission { Value = value, Type = type, Role = role };
            service.Permissions.Insert(permission, fileID).Execute();
        }





    }


}