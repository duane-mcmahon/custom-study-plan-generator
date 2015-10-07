using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using Google.Apis.Drive.v2;
using Google.Apis.Drive.v2.Data;
using File = Google.Apis.Drive.v2.Data.File;

namespace custom_study_plan_generator.Models
{
    public class StudyPlanModel
    {


        // Create a new Directory. This has been tested and works (by Duane) (will be called in Submit Study Plan process)
        // Documentation: https://developers.google.com/drive/v2/reference/files/insert
        // <param name="_service">a Valid authenticated DriveService</param>
        // <param name="_title">The title of the file. Used to identify file or folder name.</param>
        // <param name="_description">A short description of the file.</param>
        // <param name="_parent">Collection of parent folders which contain this file. 
        //                       Setting this field will put the file in all of the provided folders. root folder.</param>


        public static File createDirectory(DriveService service, string _title, string _description, string _parent)
        {

            File NewDirectory = null;

            // Create metaData for a new Directory
            File body = new File();
            body.Title = _title;
            body.Description = _description;
            body.MimeType = "application/vnd.google-apps.folder";
            body.Parents = new List<ParentReference>() { new ParentReference() { Id = _parent } };
            try
            {
                FilesResource.InsertRequest request = service.Files.Insert(body);
                NewDirectory = request.Execute();
            }
            catch (Exception e)
            {
                return null;
            }

            return NewDirectory;
        }



        //generate a google spread sheet from model data in sql database

        public static void generateGoogleSpreadSheet(DriveService service, string StudentID)
        {
            String ApplicationName = "RMIT Custom Study Plan Generator";
            var file = new File();
            file.Title = StudentID;
            file.Description = string.Format("Created via {0} at {1}", ApplicationName, DateTime.Now.ToString());
            file.MimeType = "application/vnd.google-apps.spreadsheet";

            var request = service.Files.Insert(file);
            var result = request.Execute();  //.Fetch() in example
            var spreadsheetLink = "https://spreadsheets.google.com/feeds/spreadsheets/" + result.Id;
        }

        public static File uploadFile(DriveService service, String title, String description, String parentId, String mimeType, String filename)
        {
            // File's metadata.
            File body = new File();
            body.Title = title;
            body.Description = description;
            body.MimeType = mimeType;

            // Set the parent folder.
            if (!String.IsNullOrEmpty(parentId))
            {
                body.Parents = new List<ParentReference>() { new ParentReference() { Id = parentId } };
            }

            // File's content.
            byte[] byteArray = System.IO.File.ReadAllBytes(filename);
            MemoryStream stream = new MemoryStream(byteArray);

            try
            {
                FilesResource.InsertMediaUpload request = service.Files.Insert(body, stream, mimeType);
                request.Upload();

                File file = request.ResponseBody;


                return file;
            }
            catch (Exception e)
            {
                
                return null;
            }
        }

  
 
    }




}