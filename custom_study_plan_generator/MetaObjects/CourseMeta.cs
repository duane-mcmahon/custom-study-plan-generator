using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace custom_study_plan_generator.MetaObjects
{
    public class CourseMeta
    {
        [NotMapped]
        [Required(ErrorMessage = "Course Code is required")]
        public string course_code { get; set; }

        [NotMapped]
        [Required(ErrorMessage = "Course Name is required")]
        public string name { get; set; }
        
        [NotMapped]
        [Required(ErrorMessage = "Number of Units is required")]
        [Range(0, 64, ErrorMessage = "The number of units must be between 0 and 64")]
        public int num_units { get; set; }

        [NotMapped]
        [Required(ErrorMessage = "Maximum Credit is required")]
        public int max_credit { get; set; }

    }
}