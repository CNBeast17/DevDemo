using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillsAssessment.Models.Interfaces
{
    public interface IUI
    {
        string Message { get; set; }
        string MessageType { get; set; }
    }
}
