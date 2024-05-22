using SkillsAssessment.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SkillsAssessment.ViewModels
{
    public class PersonAccountsVM
    {
        public Person Person { get; set; }
        public List<Account> Accounts { get; set; }
    }
}