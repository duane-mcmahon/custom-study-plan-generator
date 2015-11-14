using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using Google.Apis.Drive.v2;
using Google.Apis.Drive.v2.Data;
using File = Google.Apis.Drive.v2.Data.File;
using System.Diagnostics;
using System.Web.Script.Services;
using Google.GData.Spreadsheets;
using custom_study_plan_generator.MetaObjects;
using Google.Apis.Script.v1;
using Google.Apis.Script.v1.Data;
using Google.Apis.Services;
using Google.GData.Client;

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
        
             
            // Instantiate a SpreadsheetQuery object to retrieve spreadsheets.
            
            SpreadsheetQuery query = new SpreadsheetQuery();

            query.Title = file.Title;

            SpreadsheetFeed feed = sheetsService.Query(query);

            SpreadsheetEntry spreadsheet = (SpreadsheetEntry)feed.Entries[0];
            // Send the local representation of the worksheet to the API for
            // creation.  The URL to use here is the worksheet feed URL of our
            // spreadsheet.
            WorksheetFeed wsFeed = spreadsheet.Worksheets;
            // Create a local representation of the new worksheet.
            WorksheetEntry worksheet = (WorksheetEntry)wsFeed.Entries[0];
            
            worksheet.Title.Text = "Study Plan";

            
            //real model: 4 units per semester (= 4 columns), 2 semesters per year (list of courses is sorted in that way)
            
            //the following works, just need to pass the correct model data to it and tweak other spects of the spreadsheet
            
            //hardcoded: 
            worksheet.Cols = 5;


            worksheet.Rows = 6;
                 

            //updating the worksheet to contain the feedlinks, etc.
            var updated = sheetsService.Insert(wsFeed, worksheet);
            
            CellQuery cellquery = new CellQuery(updated.CellFeedLink);

            CellFeed cellFeed = sheetsService.Query(cellquery);


            // Build list of cell addresses to be filled in
            List<CellAddress> cellAddrs = new List<CellAddress>();
            for (uint row = 1; row <= worksheet.Rows; ++row)
            {
                for (uint col = 1; col <= worksheet.Cols; ++col)
                {
                    cellAddrs.Add(new CellAddress(row, col));
                }
            }


            CellFeed batchRequest = new CellFeed(new Uri(cellFeed.Self), sheetsService);
            foreach (CellAddress cellId in cellAddrs)
            {
                CellEntry batchEntry = new CellEntry(cellId.Row, cellId.Col, cellId.IdString);
                batchEntry.Id = new AtomId(string.Format("{0}/{1}", cellFeed.Self, cellId.IdString));
                batchEntry.BatchData = new GDataBatchEntryData(cellId.IdString, GDataBatchOperationType.query);
                batchRequest.Entries.Add(batchEntry);
            }

            CellFeed queryBatchResponse = (CellFeed)sheetsService.Batch(batchRequest, new Uri(cellFeed.Batch));

            Dictionary<String, CellEntry> cellEntries = new Dictionary<String, CellEntry>();
            foreach (CellEntry entry in queryBatchResponse.Entries)
            {
                cellEntries.Add(entry.BatchData.Id, entry);
               
            }


            int index = 0;

            int semester_number = 1;

            char col_num = '1';

            foreach (CellAddress cellAddr in cellAddrs)
            {
                if (cellAddr.IdString[cellAddr.IdString.Length - 1] == col_num)
                {

                    CellEntry batchEntry = cellEntries[cellAddr.IdString];
                    batchEntry.InputValue = "Semester " + semester_number.ToString();
                    batchEntry.BatchData = new GDataBatchEntryData(cellAddr.IdString, GDataBatchOperationType.update);
                    batchRequest.Entries.Add(batchEntry);
                    semester_number++;

                }
                else
                {
                    if (uploadable.StudentPlan[index] != null)
                    {
                    
                        CellEntry batchEntry = cellEntries[cellAddr.IdString];
                        batchEntry.InputValue = uploadable.StudentPlan[index].unit_code;
                        batchEntry.BatchData = new GDataBatchEntryData(cellAddr.IdString, GDataBatchOperationType.update);
                        batchRequest.Entries.Add(batchEntry);
                        index++;

                }
                    else
                {

                    CellEntry batchEntry = cellEntries[cellAddr.IdString];
                    batchEntry.InputValue = "Void";
                    batchEntry.BatchData = new GDataBatchEntryData(cellAddr.IdString, GDataBatchOperationType.update);
                    batchRequest.Entries.Add(batchEntry);
                    index++;


                }
            }
        }

            // Submit the update
            sheetsService.Batch(batchRequest, new Uri(cellFeed.Batch));

        }

        //format the spreadsheet using Google Apps Script
        public static bool? curateGoogleSpreadSheet(string fileID, string scriptID, ScriptService service)
        {

            List<object> arg = new List<object>();
            arg.Add(fileID);

            bool? success = null;



            //todo
            // Create an execution request object.
            ExecutionRequest request = new ExecutionRequest();
            request.Function = "setDefaultTab";
            request.Parameters = arg;
            ScriptsResource.RunRequest runReq =
                    service.Scripts.Run(request, scriptID);

            try
            {
                // Make the API request.
                Operation op = runReq.Execute();

                if (op.Error != null)
                {
                    // The API executed, but the script returned an error.

                    // Extract the first (and only) set of error details
                    // as a IDictionary. The values of this dictionary are
                    // the script's 'errorMessage' and 'errorType', and an
                    // array of stack trace elements. Casting the array as
                    // a JSON JArray allows the trace elements to be accessed
                    // directly.
                    IDictionary<string, object> error = op.Error.Details[0];
                    Console.WriteLine(
                        "Script error message: {0}", error["errorMessage"]);
                    if (error["scriptStackTraceElements"] != null)
                    {
                        // There may not be a stacktrace if the script didn't
                        // start executing.
                        Console.WriteLine("Script error stacktrace:");
                        Newtonsoft.Json.Linq.JArray st =
                            (Newtonsoft.Json.Linq.JArray) error["scriptStackTraceElements"];
                        foreach (var trace in st)
                        {
                            Console.WriteLine(
                                "\t{0}: {1}",
                                trace["function"],
                                trace["lineNumber"]);
                        }
                    }

                    success = false;
                }
                else
                {

                    success = true;

                }
                
                    
                
            }
            catch (Google.GoogleApiException e)
            {
                // The API encountered a problem before the script
                // started executing.
                Console.WriteLine("Error calling API:\n{0}", e);
            }


            return success;
        }


        // Adds a permission to a file. i.e. Allows sharing
        public static void addPermission(DriveService service, string fileID, string type, string role, StudyPlanModel uploadable)
        {
            string email = uploadable.StudentId + "@student.rmit.edu.au";
            Permission permission = new Permission { Value = email, Type = type, Role = role };
            service.Permissions.Insert(permission, fileID).Execute();
        }

       

    }

    class CellAddress
    {
        public uint Row;
        public uint Col;
        public string IdString;

        /**
         * Constructs a CellAddress representing the specified {@code row} and
         * {@code col}. The IdString will be set in 'RnCn' notation.
         */
        public CellAddress(uint row, uint col)
        {
            this.Row = row;
            this.Col = col;
            this.IdString = string.Format("R{0}C{1}", row, col);
        }
    }
}