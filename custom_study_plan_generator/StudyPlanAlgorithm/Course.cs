using custom_study_plan_generator.MetaObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.Entity;
using custom_study_plan_generator.Models;

namespace custom_study_plan_generator.StudyPlanAlgorithm
{
    // Class to convert from database data structure to data structure appropriate to algorithm
    // and back again.
    public class Course
    {
        // Linked list for algorithm
        public LinkedList<Unit> courseStructure = new LinkedList<Unit>();
        public int courseDuration;
        public bool midYearIntake;
        // SessionList for view
        private List<CoursePlan> oldSessionList;
        public Course(List<CoursePlan> _sessionList, int _courseDuration, bool _midYearIntake)
        {
            courseDuration = _courseDuration;
            midYearIntake = _midYearIntake;

            // Save sessionList for other methods.
            oldSessionList = _sessionList;

            // Creating EF object.
            custom_study_plan_generatorEntities db = new custom_study_plan_generatorEntities();


            // Covert CoursePaln info from sessionList to units for algorithm list.
            foreach (var unit in _sessionList)
            {
                // Converting properties as necessary to match algorithms needs.
                // Convert semester1 & semester2 booleans into string representation.
                string semester = "Any";
                if (!unit.semester1)
                    semester = "July";
                if (!unit.semester2)
                    semester = "Feb";
                // Checking that preferredYear is not null and converting to string for algorithm.
                // XYZ TODO check preferred year is in appropriate range? Is it even needed??????
                string preferred_year = "1"; 
                /*if (unit.preferred_year != null)
                    preferredYear = unit.preferred_year.ToString();*/

                /*
                 * XYZ TO DO OLD WAY. remove when happy
                 * // Just grabbing pre req directly from database for now. XYZ TO DO!!!!!!
                var _preReqs =
                    from unitPreReqs in db.UnitPrerequisites
                    where unitPreReqs.unit_code == unit.unit_code
                    select unitPreReqs.prereq_code;

                // list of preReqs
                List<string> preReqs = null;

                if (_preReqs.Any())
                    preReqs = _preReqs.ToList();*/
                
                // Set preReqs if any.
                List<string> preReqs = new List<string>();
                preReqs = null;
                if (unit.prerequisites.Any())
                    preReqs = unit.prerequisites;

                // Test pre reqs
                System.Diagnostics.Debug.WriteLine(unit.name);
                if (preReqs != null)
                {
                    foreach(var preReq in unit.prerequisites)
                    {
                        System.Diagnostics.Debug.WriteLine(preReq);
                    }
                }


                // isPreReq XYZ TO DO!!!!! Appears algorithm doesn't use this.
                bool isPreReq = false;

                // Create Unit object suitable for the algorithm.
                Unit tempUnit = new Unit(unit.name, unit.unit_code, unit.type_code, semester, preferred_year, preReqs, isPreReq, unit.exempt);

                // Add unit object to end of linked list.
                courseStructure.AddLast(tempUnit);
            }

            // Close DB.

            /*
             * Left to check against old way of creating units.
             * Unit unit1 = new Unit("Intro to Programming (Python)", "CO101", "c", "Any", "1", null, true, false);
             * courseStructure.AddLast(unit1);
             */
        }
        // Method to return list of coursePlans.
        internal List<CoursePlan> ToList(LinkedList<Unit> algorithmList)
        {
            System.Diagnostics.Debug.WriteLine("In the ToList method of course object!!!");
            // List for update units.
            List<CoursePlan> newSessionlist = new List<CoursePlan>();
            // int to keep track of new position of units.
            int newPosition = 1;

            /*
                public int position { get; set; }
                public int semester { get; set; }
                public string unit_code { get; set; }
                public string name { get; set; }
                public string type_code { get; set; }
                public bool semester1 { get; set; }
                public bool semester2 { get; set; }
                public bool exempt { get; set; }
                public Nullable<int> preferred_year { get; set; }
             */
            // loop through algorithms linked list to create new session list.
            foreach (Unit unit in algorithmList)
            {
                // Find matching CoursePlan object in old session list.
                CoursePlan tempCoursePlan = oldSessionList.Find(x => x.unit_code.Equals(unit.UnitCode));
                // First update new semester.
                tempCoursePlan.semester = (newPosition - 1) / 4 + 1;
                // Now update position and increment it.
                tempCoursePlan.position = newPosition++;
                // Add to new list.
                newSessionlist.Add(tempCoursePlan);
            }
            // Return new list.
            return newSessionlist;
        }
        public int CourseDuration
        {
            get
            {
                return courseDuration;
            }
            set
            {
                courseDuration = value;
            }
        }

        public bool MidYearIntake
        {
            get
            {
                return midYearIntake;
            }
            set
            {
                midYearIntake = value;
            }
        }
        public void check()
        {
            foreach( var unit in courseStructure)
            {
                System.Diagnostics.Debug.WriteLine(unit.UnitName);
                if (unit.PreReq != null)
                {
                    foreach (var preReq in unit.PreReq)
                        System.Diagnostics.Debug.WriteLine(preReq);
                }
            }
        }
    }
}