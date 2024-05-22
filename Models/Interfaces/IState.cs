using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillsAssessment.Models.Intefaces
{
    interface IState
    {
         bool IsActive { get; set; }
    }
}
