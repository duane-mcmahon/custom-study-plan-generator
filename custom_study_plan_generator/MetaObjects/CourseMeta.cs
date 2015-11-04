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
        [Required]
        public string course_code { get; set; }

        [NotMapped]
        [Required]
        public string name { get; set; }
        
        [NotMapped]
        [Required]
        [Range(0,64)]
        public int num_units { get; set; }

        /* Check if wee need this property */
        public int duration { get; set; }

        /* Check if we need this property */
        public int max_credit { get; set; }

    }
}