using SkillsAssessment.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillsAssessment.DataAccessLayer.RepositoryInterfaces
{
    interface IStatusRepository
    {
        IEnumerable<Status> GetStatuses();
        Status GetStatusByID(int code);
    }
}
