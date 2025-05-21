using SkillsAssessment.Models;
using SkillsAssessment.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SkillsAssessment.ViewModels
{
    public class AccountCreateVM:IUI
    {
        public Account Account { get; set; }
        public string Message { get; set; }
        public string MessageType { get; set; }
        public SelectList PersonCodeList { get; set; }
        public SelectList StatusCodeList { get; set; }
    }
}