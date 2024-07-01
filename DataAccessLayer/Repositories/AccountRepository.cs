using SkillsAssessment.DataAccessLayer.RepositoryInterfaces;
using SkillsAssessment.DataAccessLayer.UnitOfWork;
using SkillsAssessment.Helpers;
using SkillsAssessment.Keys;
using SkillsAssessment.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace SkillsAssessment.DataAccessLayer.Repositories
{
    public class AccountRepository :IAccountRepository
    {
        private TraqSoftwareContext context;
        private SQLViewHelpers sQLViewHelpers;
        private string connectionString;
        private bool disposed;
        public AccountRepository(IUnitOfWork<TraqSoftwareContext> unitOfWork)
          : this(unitOfWork.Context)
        {          
        }
        public AccountRepository(TraqSoftwareContext context)
        {
            this.sQLViewHelpers = new SQLViewHelpers();
            this.context = context;
            this.connectionString = ConfigurationManager.ConnectionStrings["TraqSoftwareContext"].ConnectionString;
            this.disposed = false;
        }
        public IEnumerable<Account> GetAccounts()
        {
            List<Account> accounts = new List<Account>();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = connection.CreateCommand())
                {
                    command.CommandText = "SELECT * from AccountsView";
                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        accounts.AddRange(sQLViewHelpers.ExtractAccounts(reader));
                    }
                }
            }
            return accounts;
        }

        public Account GetAccountByID(int code)
        {
            List<Account> accounts = new List<Account>();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = connection.CreateCommand())
                {
                    command.CommandText = "SELECT * from AccountsView where [code] =" + code;
                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        accounts.AddRange(sQLViewHelpers.ExtractAccounts(reader));
                    }
                }
            }
            return accounts.FirstOrDefault();
        }

        public void InsertAccount(Account account)
        {
            if (context == null || disposed)
            {
                context = new TraqSoftwareContext();
            }
            context.Accounts.Add(account);
        }

        public void DeleteAccount(int code)
        {
            if (context == null || disposed)
            {
                context = new TraqSoftwareContext();
            }
            Account account = GetAccountByID(code);
            account.AccountStatusCode = context.Statuses.FirstOrDefault(x => x.Key == StatusKeys.AccountClosed).Code;
            UpdateAccount(account);
        }
        public void OpenAccount(int code)
        {
            if (context == null || disposed)
            {
                context = new TraqSoftwareContext();
            }
            Account account = GetAccountByID(code);
            account.AccountStatusCode = context.Statuses.FirstOrDefault(x => x.Key == StatusKeys.AccountOpen).Code;
            UpdateAccount(account);
        }

        public void UpdateAccount(Account account)
        {
            if (context == null || disposed)
            {
                context = new TraqSoftwareContext();
            }
            account.Person = null;
            account.Status = null;
            context.Set<Account>().AddOrUpdate(account);
            //context.Entry(account).State = EntityState.Modified;
        }
        public IEnumerable<Account> GetPersonAccounts(int personCode)
        {
            List<Account> accounts = new List<Account>();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = connection.CreateCommand())
                {
                    command.CommandText = "SELECT * from AccountsView where person_code = "+personCode;
                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        accounts.AddRange(sQLViewHelpers.ExtractAccounts(reader));
                    }
                }
            }
            return accounts;
        }
        public IEnumerable<Account> SearchAccounts(string accountNumber)
        {
            //Dynamic search that searchs by parameter if value is provided
            //Ignores parameter if value is null or empty
            return context.Accounts.Where(x => (string.IsNullOrEmpty(accountNumber) || x.AccountNumber == accountNumber)           
            );
        }
        //public void Save()
        //{
        //    context.SaveChanges();
        //}
      
       

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    context.Dispose();
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