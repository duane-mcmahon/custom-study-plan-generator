using custom_study_plan_generator.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using System.Web;

namespace custom_study_plan_generator.MetaObjects
{
    public class CheckStudentIDAttribute : ValidationAttribute
    {

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            
            
           

            return null;
        }
    }
}