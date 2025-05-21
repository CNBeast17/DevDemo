using SkillsAssessment.Models;
using SkillsAssessment.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SkillsAssessment.ViewModels
{
    public class PersonVM :IUI
    {
        public Person Person { get; set; }
        public string Message { get; set; }
        public string MessageType { get; set; }
    }
}