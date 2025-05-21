using SkillsAssessment.DataAccessLayer.Repositories;
using SkillsAssessment.DataAccessLayer.RepositoryInterfaces;
using SkillsAssessment.DataAccessLayer.UnitOfWork;
using SkillsAssessment.Keys;
using SkillsAssessment.Models;
using SkillsAssessment.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SkillsAssessment.Helpers
{
    public class PersonHelpers
    {
        private UnitOfWork<TraqSoftwareContext> _unitOfWork;
        private IPersonRepository _personRepository;

        public PersonHelpers()
        {
            _unitOfWork = new UnitOfWork<TraqSoftwareContext>();
            _personRepository = new PersonRepository(_unitOfWork);
        }
        public PersonHelpers(UnitOfWork<TraqSoftwareContext> unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _personRepository = new PersonRepository(_unitOfWork);
        }
        public Person GetPersonByID(int personId)
        {
           return _personRepository.GetPersonByID(personId);
        }

        private PersonVM GetPersonVM(int personId,string messageType,string message) {
            return new PersonVM
            {
                Message = message,
                MessageType = messageType,
                Person = GetPersonByID(personId)
            };
        }
        public PersonVM GetPersonVMUserDeleted(int personId)
        {
            return GetPersonVM(personId, UIKeys.Error, "Accounts cannot be created for deleted people");
        }

  
    }
}