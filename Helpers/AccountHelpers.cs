using SkillsAssessment.DataAccessLayer.Repositories;
using SkillsAssessment.DataAccessLayer.RepositoryInterfaces;
using SkillsAssessment.DataAccessLayer.UnitOfWork;
using SkillsAssessment.Models;
using SkillsAssessment.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http.Results;
using System.Web.Mvc;

namespace SkillsAssessment.Helpers
{
    public class AccountHelpers
    {
        private UnitOfWork<TraqSoftwareContext> _unitOfWork;
        private IAccountRepository _accountRepository;
        private ITransactionRepository _transactionRepository;
        private IPersonRepository _personRepository;
        private IStatusRepository _statusRepository;
        public AccountHelpers(UnitOfWork<TraqSoftwareContext> unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _accountRepository = new AccountRepository(_unitOfWork);
            _transactionRepository = new TransactionRepository(_unitOfWork);
            _statusRepository = new StatusRepository(_unitOfWork);
            _personRepository = new PersonRepository(_unitOfWork);
        }
        public AccountHelpers()
        {
            _unitOfWork = new UnitOfWork<TraqSoftwareContext>();
            _accountRepository = new AccountRepository(_unitOfWork);
            _transactionRepository = new TransactionRepository(_unitOfWork);
            _statusRepository = new StatusRepository(_unitOfWork);
            _personRepository = new PersonRepository(_unitOfWork);
        }

        public AccountVM GetAccountVM(int accountId)
        {
            AccountVM accountVM = new AccountVM{
                Account = _accountRepository.GetAccountByID(accountId),
                Transactions = _transactionRepository.GetAccountTransactions(accountId).ToList()
            };
            return accountVM;
        }
        private Account GetDuplicateAccount(string accountNumber, int? accountCode)
        {
            return _accountRepository.SearchAccounts(accountNumber).FirstOrDefault(x => x.Code != accountCode);
        }
        public Account CheckGetDuplicateAccount(string accountNumber, int? accountCode)
        {
            if(string.IsNullOrEmpty(accountNumber) && accountNumber != null)
            {
                return GetDuplicateAccount(accountNumber, accountCode); 
            }else
            {
                return null;
            }
        }
        public IEnumerable<Account> GetAccounts() {
            return _accountRepository.GetAccounts();
        }
        public AccountCreateVM GetAccountCreateVM(int personId)
        {
            return new AccountCreateVM()
            {
                PersonCodeList = new SelectList(new List<Person> { _personRepository.GetPersonByID(personId) }, "Code", "Name", personId),
                StatusCodeList = new SelectList(_statusRepository.GetStatuses(), "Code", "Name"),
                Account = new Account { PersonCode = personId}
            };

        }
    }
}