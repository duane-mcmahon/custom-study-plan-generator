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

        /* Check if we need this property */
        public string type_code { get; set; }

        /* Will need client side validation to make sure on or the other is set */
        [NotMapped]
        public bool semester1 { get; set; }

        [NotMapped]
        public bool semester2 { get; set; }

        /* Need to check if this is required */
        [Range(1, 8)]
        public Nullable<int> preferred_year { get; set; }

    }
}