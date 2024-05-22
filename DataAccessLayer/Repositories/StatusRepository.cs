using SkillsAssessment.DataAccessLayer.RepositoryInterfaces;
using SkillsAssessment.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SkillsAssessment.DataAccessLayer.Repositories
{
    public class StatusRepository: IStatusRepository
    {
        private TraqSoftwareContext context;
        public StatusRepository(TraqSoftwareContext context)
        {
            this.context = context;
        }
        public IEnumerable<Status> GetStatuses()
        {
            return context.Statuses.ToList();
        }
        public Status GetStatusByID(int code)
        {
            return context.Statuses.FirstOrDefault(x => x.Code == code);
        }
    }
}