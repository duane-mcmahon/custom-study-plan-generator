using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace custom_study_plan_generator.MetaObjects
{
    public class StudentMeta
    {

        [NotMapped]
        [Required(ErrorMessage = "Student ID is required")]
        [RegularExpression(@"^[0-9]{7}$", ErrorMessage = "Student ID must match the format '1234567'")]
        public int student_id { get; set; }

        [NotMapped]
        [Required(ErrorMessage = "First name is required")]
        public string firstname { get; set; }
        
        [NotMapped]
        [Required(ErrorMessage = "Surname is required")]
        public string lastname { get; set; }

    }
}