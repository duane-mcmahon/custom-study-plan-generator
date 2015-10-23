using custom_study_plan_generator.MetaObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace custom_study_plan_generator.StudyPlanAlgorithm
{
    // Class to convert from database data structure to data structure appropriate to algorithm
    // and back again.
    public class Course
    {
        // Linked list for algorithm
        public LinkedList<Unit> courseStructure = new LinkedList<Unit>();
        // SessionList for view
        private List<CoursePlan> oldSessionList;
        public Course(List<CoursePlan> _sessionList)
        {
            // Save sessionList for other methods.
            oldSessionList = _sessionList;

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
                string preferredYear = "1"; 
                if (unit.preferred_year != null)
                    preferredYear = unit.preferred_year.ToString();
                // Setting preReqs to false for now. XYZ TO DO!!!!!! need to query DB to find array of pre req codes.
                string preReq = null;
                // isPreReq XYZ TO DO!!!!! Appears algorithm doesn't use this.
                bool isPreReq = false;

                // Create Unit object suitable for the algorithm.
                Unit tempUnit = new Unit(unit.name, unit.unit_code, unit.type_code, semester, preferredYear, preReq, isPreReq, unit.exempt);

                // Add unit object to end of linked list.
                courseStructure.AddLast(tempUnit);
            }

            /*
             * Left to check against old way of creating units.
             * Unit unit1 = new Unit("Intro to Programming (Python)", "CO101", "c", "Any", "1", null, true, false);
             * courseStructure.AddLast(unit1);
             */
        }
        // Method to return list of coursePlans.
        internal List<CoursePlan> ToList(LinkedList<Unit> linkedList)
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
            foreach (Unit unit in linkedList)
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
    }
}