using SkillsAssessment.DataAccessLayer.Repositories;
using SkillsAssessment.DataAccessLayer.RepositoryInterfaces;
using SkillsAssessment.DataAccessLayer.UnitOfWork;
using SkillsAssessment.Keys;
using SkillsAssessment.Models;
using SkillsAssessment.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Http.Results;
using System.Web.Mvc;

namespace SkillsAssessment.Helpers
{
    public class PersonHelpers : IDisposable
    {
        private UnitOfWork<TraqSoftwareContext> _unitOfWork;
        private IPersonRepository _personRepository;
        private IAccountRepository _accountRepository;
        private bool disposed;

        public PersonHelpers()
        {
            _unitOfWork = new UnitOfWork<TraqSoftwareContext>();
            _personRepository = new PersonRepository(_unitOfWork);
            _accountRepository = new AccountRepository(_unitOfWork);
            this.disposed = false;
        }
        public PersonHelpers(UnitOfWork<TraqSoftwareContext> unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _personRepository = new PersonRepository(_unitOfWork);
            _accountRepository = new AccountRepository(_unitOfWork);
            this.disposed = false;
        }
        public Person GetPersonByID(int personId)
        {
           return _personRepository.GetPersonByID(personId);
        }
        public PersonVM GetPersonVM(int personId)
        {
            return new PersonVM
            {
                Message = string.Empty,
                MessageType = string.Empty,
                Person = GetPersonByID(personId)
            };
        }
        private PersonVM GetPersonVM(int personId,string messageType,string message) {
            return new PersonVM
            {
                Message = message,
                MessageType = messageType,
                Person = GetPersonByID(personId)
            };
        }
        private PersonVM GetPersonVM(Person person, string messageType, string message)
        {
            return new PersonVM
            {
                Message = message,
                MessageType = messageType,
                Person = person
            };
        }
        public PersonVM GetPersonVMUserDeleted(int personId)
        {
            return GetPersonVM(personId, UIKeys.Error, "Accounts cannot be created for deleted people");
        }
        public PersonVM GetPersonVMCannotRestoreActiveUser(int personId)
        {
            return GetPersonVM(personId, UIKeys.Error, "Only deleted people can be restored!");
        }
        public PeopleListVM GetPeopleVMUserHasActiveAccount()
        {
            return new PeopleListVM
            {
                Message = "People with open accounts cannot be deleted!",
                MessageType = UIKeys.Error,
                People = SearchPeople(string.Empty, string.Empty, string.Empty),
            }; 
        }
        public PersonVM GetPersonVMUserExists(Person person)
        {
            return GetPersonVM(person, UIKeys.Error, "ID Number already Exists!");
        }

        public Person CheckDuplicatePerson(string idNum, int? personCode)
        {
            if (idNum != "" && idNum != null)
            {
                //reuse of functionality 
                //instead of creating another similar method that specifically brings back one record
                return _personRepository.SearchPeople(idNum, "", "").Where(x => x.Code != personCode).FirstOrDefault();
            }
            else
            {
                return null;
            }
        }
        public IEnumerable<Person> SearchPeople(string IdNumber, string Name, string Surname)
        {
            return _personRepository.SearchPeople(IdNumber, Name, Surname);
        }

        public PersonAccountsVM GetPersonAccountVM(int? id)
        {           
            return new PersonAccountsVM {
                Person = _personRepository.GetPersonByID(id.Value),
                Accounts = _accountRepository.GetPersonAccounts(id.Value).ToList()
            };
        }
        public PeopleListVM SavePerson(Person person)
        {
            person.IsActive = true;
            _unitOfWork.CreateTransaction();
            _personRepository.InsertPerson(person);
            _unitOfWork.Save();
            _unitOfWork.Commit();
            return new PeopleListVM
            {
                Message = "Person successfuly created!",
                MessageType = UIKeys.Success,
                People = SearchPeople(string.Empty, string.Empty, string.Empty),
            };
        } 
        public PeopleListVM UpdatePerson(Person person)
        {
            _unitOfWork.CreateTransaction();
            _personRepository.UpdatePerson(person);
            _unitOfWork.Save();
            _unitOfWork.Commit();
            return new PeopleListVM
            {
                Message = "Person successfuly updated!",
                MessageType = UIKeys.Success,
                People = SearchPeople(string.Empty, string.Empty, string.Empty),
            };
        }
        public PeopleListVM DeletePerson(int id)
        {
            _unitOfWork.CreateTransaction();

            _personRepository.DeletePerson(id);
            _unitOfWork.Save();
            _unitOfWork.Commit();
            return new PeopleListVM
            {
                Message = "Person has been deleted successfuly!",
                MessageType = UIKeys.Success,
                People = SearchPeople(string.Empty, string.Empty, string.Empty),
            };
        }
        public PeopleListVM RestorePerson(int id)
        {
            _unitOfWork.CreateTransaction();
            _personRepository.RestorePerson(id);
            _unitOfWork.Save();
            _unitOfWork.Commit();
            return new PeopleListVM
            {
                Message = "Person has been restored successfuly!",
                MessageType = UIKeys.Success,
                People = SearchPeople(string.Empty, string.Empty, string.Empty),
            };
        }

        public bool PersonHasActiveAccount(int? id)
        {
            Person person = GetPersonByID(id.Value);
            List<Account> accounts = _accountRepository.GetPersonAccounts(id.Value).ToList();
            //checks if person has at least one open account
            if (accounts.Select(x => x.Status.Key).Contains(StatusKeys.AccountOpen))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public PeopleListVM GetPeopleVM(string IdNumber, string Name, string Surname)
        {
            return new PeopleListVM
            {
                People = SearchPeople(IdNumber, Name, Surname),
            };
        }
        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    _unitOfWork.Dispose();
                }
            }
            this.disposed = true;
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}