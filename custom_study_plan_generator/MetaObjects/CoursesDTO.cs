using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace custom_study_plan_generator.MetaObjects
{
    public class CourseDTO
    {
          
        public string course_code { get; set; }
        public string name { get; set; }
        public int num_units { get; set; }
        public int duration { get; set; }

    }
}