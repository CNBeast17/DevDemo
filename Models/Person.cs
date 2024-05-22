using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using SkillsAssessment.Models.Intefaces;

namespace SkillsAssessment.Models
{
    [Table("Persons")]
    public class Person : IState
    {
        [Key]
        [Column("code")]

        public int Code { get; set; }
        [Column("name")]
        [MaxLength(50)]
        public string Name { get; set; }
        [Column("surname")]
        [MaxLength(50)]
        public string Surname { get; set; }
        [Index(IsUnique = true)]
        [Display(Name ="ID Number")]
        [Column("id_number")]
        [MaxLength(50)]
        public string IDNumber { get; set; }

        public bool IsActive { get; set; }
    }
}