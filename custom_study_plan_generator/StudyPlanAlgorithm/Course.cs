using custom_study_plan_generator.MetaObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace custom_study_plan_generator.StudyPlanAlgorithm
{
    public class Course
    {
        public LinkedList<Unit> courseStructure = new LinkedList<Unit>();
        public Course(List<CoursePlan> sessionList)
        {

            foreach (var unit in sessionList)
            {
                // Converting properties as necessary to match algorithms needs.
                // Convert semester1 & semester2 booleans into string representation.
                string semester = "any";
                if (!unit.semester1)
                    semester = "2";
                if (!unit.semester2)
                    semester = "1";
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

        internal List<CoursePlan> ToList()
        {
            throw new NotImplementedException();
        }
    }
}