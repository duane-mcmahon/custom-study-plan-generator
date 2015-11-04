using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using Google.Apis.Drive.v2;
using Google.Apis.Drive.v2.Data;
using File = Google.Apis.Drive.v2.Data.File;
using System.Diagnostics;
using Google.GData.Spreadsheets;
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

        public static File generateGoogleSpreadSheet(DriveService service, SpreadsheetsService sheetsService, string studentID, string fileID,
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

            populateGoogleSpreadSheet(file, u, sheetsService);

            return result;

        }

        //generate a google spread sheet from model data in sql database
        //returns the uploaded File result

        public static File generateGoogleSpreadSheet(DriveService service, SpreadsheetsService sheetsService, string studentID,
            string fileID, StudyPlanModel u)
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

            populateGoogleSpreadSheet(file, u, sheetsService);


            return result;

        }

        
        //Filling the Spreadsheet
        public static void populateGoogleSpreadSheet(File file, StudyPlanModel uploadable, SpreadsheetsService sheetsService)
        {

            /* Sheets api testing: verified as working (duane)
           
            //test must be called after plan has been saved/uploaded via ui at index page 
             
            // Instantiate a SpreadsheetQuery object to retrieve spreadsheets.
            
            SpreadsheetQuery query = new SpreadsheetQuery();

            query.Title = file.Title;

            SpreadsheetFeed feed = sheetsService.Query(query);

            SpreadsheetEntry spreadsheet = (SpreadsheetEntry)feed.Entries[0];
            
            // Create a local representation of the new worksheet.
            WorksheetEntry worksheet = new WorksheetEntry();
            worksheet.Title.Text = "Testing Connection Worksheet";
            worksheet.Cols = 10;
            worksheet.Rows = 20;

            // Send the local representation of the worksheet to the API for
            // creation.  The URL to use here is the worksheet feed URL of our
            // spreadsheet.
            WorksheetFeed wsFeed = spreadsheet.Worksheets;
            sheetsService.Insert(wsFeed, worksheet); */

            //TODO

        }

        // Adds a permission to a file. i.e. Allows sharing
        public static void addPermission(DriveService service, string fileID, string value, string type, string role, StudyPlanModel uploadable)
        {
            Permission permission = new Permission { Value = value, Type = type, Role = role };
            service.Permissions.Insert(permission, fileID).Execute();
        }





    }


}