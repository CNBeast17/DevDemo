using SkillsAssessment.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SkillsAssessment.ViewModels
{
    public class AccountVM
    {
        public Account Account { get; set; }
        public List<Transaction> Transactions { get; set; }
    }
}