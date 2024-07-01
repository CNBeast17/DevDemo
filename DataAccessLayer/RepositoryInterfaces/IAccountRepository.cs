using SkillsAssessment.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SkillsAssessment.DataAccessLayer.RepositoryInterfaces
{
    public interface IAccountRepository : IDisposable

    {
        IEnumerable<Account> GetAccounts();
        IEnumerable<Account> GetPersonAccounts(int personCode);
        Account GetAccountByID(int code);
        void InsertAccount(Account account);
        void DeleteAccount(int code);
        void OpenAccount(int code);
        void UpdateAccount(Account account);
        IEnumerable<Account> SearchAccounts(string accountNumber);
      //  void Save();


    }
}