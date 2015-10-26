using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace custom_study_plan_generator.MetaObjects
{
    public class StudentID
    {

        [NotMapped]
        [Required(ErrorMessage = "Student ID is required")]
        [RegularExpression(@"[s]{1}\d{7}", ErrorMessage = "Student ID must match the format s1234567")]
        public string studentID { get; set; }

    }
}