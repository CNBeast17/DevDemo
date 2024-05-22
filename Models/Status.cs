using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SkillsAssessment.Models
{
    public class Status
    {
        [Key]
        [Column("code")]
        public int Code { get; set; }
        public string Key { get; set; }
        public string Name { get; set; }
    }
}