using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace v2_frontend.Models
{
    public class FormItemModel
    {
        [Required]
        [StringLength(10, ErrorMessage = "Name is too long.")]
        public string Name { get; set; }

        [Required]
        public DateTime Date { get; set; }
    }

    public class FormItemResults
    {
        public string key { get; set; }

        public FormItemModel data { get; set; }
    }
    public class FormItemResultsSDK
    {
        public string Key { get; set; }

        public FormItemModel Value { get; set; }
    }
}
