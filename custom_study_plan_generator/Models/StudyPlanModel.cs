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

            // Sheets api testing: verified as working (duane)
           
            //test must be called after plan has been saved/uploaded via ui at index page 
             
            // Instantiate a SpreadsheetQuery object to retrieve spreadsheets.
            
            SpreadsheetQuery query = new SpreadsheetQuery();

            query.Title = file.Title;

            SpreadsheetFeed feed = sheetsService.Query(query);

            SpreadsheetEntry spreadsheet = (SpreadsheetEntry)feed.Entries[0];
            
            // Create a local representation of the new worksheet.
            WorksheetEntry worksheet = new WorksheetEntry();
            worksheet.Title.Text = "Testing Study Plan";

            //task obviously is to determine size or rows cols from list data etc.
            //real model: 4 units per semester, 2 semesters per year (list of courses is sorted in that way)
            //dummy data:
            worksheet.Cols = 10;
            worksheet.Rows = 10;
           
            // Send the local representation of the worksheet to the API for
            // creation.  The URL to use here is the worksheet feed URL of our
            // spreadsheet.
            WorksheetFeed wsFeed = spreadsheet.Worksheets;

            //updating the worksheet to contain the feedlinks, etc.
            var updated = sheetsService.Insert(wsFeed, worksheet);

             //todo 
            foreach (var course in uploadable.StudentPlan)
            {
                uint row = 1;
                uint col = 1;
                UpdateCell(sheetsService, row, col, updated, course.name);
                row++;
                col++;
            }

      
        }

        // Adds a permission to a file. i.e. Allows sharing
        public static void addPermission(DriveService service, string fileID, string value, string type, string role, StudyPlanModel uploadable)
        {
            Permission permission = new Permission { Value = value, Type = type, Role = role };
            service.Permissions.Insert(permission, fileID).Execute();
        }


        /// <summary>
        /// Updates a single cell in the specified worksheet.
        /// </summary>
        /// <param name="service">an authenticated SpreadsheetsService object</param>
        /// <param name="entry">the worksheet to update</param>
        private static void UpdateCell(SpreadsheetsService service, uint row, uint col, WorksheetEntry entry, string newValue)
        {
            
            CellQuery query = new CellQuery(entry.CellFeedLink);
           
            query.ReturnEmpty = ReturnEmptyCells.yes;

            query.MinimumRow = query.MaximumRow = row;
            query.MinimumColumn = query.MaximumColumn = col;

            CellFeed feed = service.Query(query);
            CellEntry cell = feed.Entries[0] as CellEntry;

            cell.Cell.InputValue = newValue;
            
            cell.Update();

           
        }

        /// <summary>
        /// Inserts a new row in the specified worksheet.
        /// </summary>
        /// <param name="service">an authenticated SpreadsheetsService object</param>
        /// <param name="entry">the worksheet into which the row will be inserted</param>
        /// <returns>the inserted ListEntry object, representing the new row</returns>
        private static ListEntry InsertRow(SpreadsheetsService service, WorksheetEntry entry)
        {
            AtomLink listFeedLink = entry.Links.FindService(GDataSpreadsheetsNameTable.ListRel, null);

            ListQuery query = new ListQuery(listFeedLink.HRef.ToString());

            ListFeed feed = service.Query(query);

            ListEntry firstRow = feed.Entries[0] as ListEntry;
            
            ListEntry newRow = new ListEntry();

         
            foreach (ListEntry.Custom element in firstRow.Elements)
            {
                
                String elementValue = Console.ReadLine();//inputValue

                ListEntry.Custom curElement = new ListEntry.Custom();
                curElement.LocalName = element.LocalName;
                curElement.Value = elementValue;

                newRow.Elements.Add(curElement);
            }

            ListEntry insertedRow = feed.Insert(newRow);

            return insertedRow;
        }

        /// <summary>
        /// Updates the value of a cell in a single worksheet row.
        /// </summary>
        /// <param name="service">an authenticated SpreadsheetsService object</param>
        /// <param name="entry">the ListEntry representing the row to update</param>
        /// <returns>the updated ListEntry object</returns>
        private static ListEntry UpdateRow(SpreadsheetsService service, ListEntry entry)
        {
            ListEntry.Custom firstColumn = entry.Elements[0];
      
            String newValue = Console.ReadLine();//inputValue

            firstColumn.Value = newValue;

            ListEntry updatedRow = entry.Update() as ListEntry;

            return updatedRow;
        }





    }


}