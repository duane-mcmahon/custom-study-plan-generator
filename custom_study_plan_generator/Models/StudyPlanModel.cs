using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using Google.Apis.Drive.v2;
using Google.Apis.Drive.v2.Data;
using File = Google.Apis.Drive.v2.Data.File;
using System.Diagnostics;

namespace custom_study_plan_generator.Models
{
    public class StudyPlanModel
    {
        public const string StudyPlanDirectory = "RMITStudentStudyPlans";

        // Create a new Directory. This has been tested and works (by Duane) (will be called in Submit Study Plan process)
        // Documentation: https://developers.google.com/drive/v2/reference/files/insert



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
            FileList list)
        {

            var file = new File();
            file.Title = studentID;
            file.Description = string.Format("Created via {0} at {1}", service.ApplicationName, DateTime.Now.ToString());
            file.MimeType = "application/vnd.google-apps.spreadsheet";
            
            File result = null;
            Boolean file_exists = false;
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

                    file_exists = true;

                    var request = service.Files.Update(file, list.Items[i].Id);

                    result = request.Execute();

                }

            }

            if (file_exists == false)
            {

                var request = service.Files.Insert(file);

                result = request.Execute();

            }


            return result;

        }

        //generate a google spread sheet from model data in sql database
        //returns the uploaded File result
        public static File generateGoogleSpreadSheet(DriveService service, string studentID, string fileID)
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




            return result;

        }

        // Adds a permission to a file. i.e. Allows sharing
        public static void addPermission(DriveService service, string fileID, string value, string type, string role)
        {
            Permission permission = new Permission { Value = value, Type = type, Role = role };
            service.Permissions.Insert(permission, fileID).Execute();
        }



        public static void populateGoogleSpreadSheet()
        {

        }



    }


}