﻿using SkillsAssessment.Models;
using SkillsAssessment.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SkillsAssessment.ViewModels
{
    public class AccountVM :IUI
    {
        public Account Account { get; set; }
        public List<Transaction> Transactions { get; set; }
        public string Message { get; set; }
        public string MessageType { get; set; }
    }
}