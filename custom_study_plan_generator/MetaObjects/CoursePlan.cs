using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace custom_study_plan_generator.MetaObjects
{
    public class CoursePlan
    {
        public CoursePlan()
        {

        }

        public int position { get; set; }
        public int semester { get; set; }
        public string unit_code { get; set; }
        public string name { get; set; }
        public string type_code { get; set; }
        public bool semester1 { get; set; }
        public bool semester2 { get; set; }
        public bool exempt { get; set; }
        public Nullable<int> preferred_year { get; set; }

    }
}