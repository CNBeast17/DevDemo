using SkillsAssessment.DataAccessLayer.Repositories;
using SkillsAssessment.DataAccessLayer.RepositoryInterfaces;
using SkillsAssessment.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;

namespace SkillsAssessment.APIServices
{
    [ApiExplorerSettings(IgnoreApi = false)]
    [Authorize]
    public class PeopleController : ApiController
    {
        private TraqSoftwareContext db;
        private IPersonRepository personRepository;
        private IAccountRepository accountRepository;


        public PeopleController()
        {
            db = new TraqSoftwareContext();
            this.personRepository = new PersonRepository(db);
            this.accountRepository = new AccountRepository(db);
            
        }
        [HttpGet]
        public IHttpActionResult GetPeople()
        {
            return Ok(personRepository.GetPeople());
        }
        [HttpGet]
        public IHttpActionResult SearchPeople(string IdNumber, string Name, string Surname)
        {
            return Ok(personRepository.SearchPeople(IdNumber, Name, Surname));
        }
        [HttpGet]
        public IHttpActionResult ViewPerson(int id)
        {          
            Person person = personRepository.GetPersonByID(id);
           return Ok(person);
        }
    }
}
