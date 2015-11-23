using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace custom_study_plan_generator.MetaObjects
{
    public class UnitMeta
    {

        [NotMapped]
        [Required]
        public string unit_code { get; set; }

        [NotMapped]
        [Required]
        public string name { get; set; }

        /* Will need client side validation to make sure on or the other is set */
        [NotMapped]
        public bool semester1 { get; set; }

        [NotMapped]
        public bool semester2 { get; set; }

    }
}