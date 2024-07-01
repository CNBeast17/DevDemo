using SkillsAssessment.DataAccessLayer.RepositoryInterfaces;
using SkillsAssessment.DataAccessLayer.UnitOfWork;
using SkillsAssessment.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace SkillsAssessment.DataAccessLayer.Repositories
{
    public class TransactionRepository:ITransactionRepository
    {
        private TraqSoftwareContext context;
        private bool disposed;

        public TransactionRepository(IUnitOfWork<TraqSoftwareContext> unitOfWork)
          : this(unitOfWork.Context)
        {
        }
        public TransactionRepository(TraqSoftwareContext context)
        {
            this.context = context;
            this.disposed = false;
        }
        public IEnumerable<Transaction> GetTransactions()
        {
            return context.Transactions.ToList();
        }

        public Transaction GetTransactionByID(int code)
        {
            return context.Transactions.Find(code);
        }

        public void InsertTransaction(Transaction transaction)
        {
            if (context == null || disposed)
            {
                context = new TraqSoftwareContext();
            }
            context.Transactions.Add(transaction);
        }

        public void DeleteTransaction(int code)
        {
            if (context == null || disposed)
            {
                context = new TraqSoftwareContext();
            }
            Transaction transaction = GetTransactionByID(code);
            context.Transactions.Remove(transaction);
        }

        public void UpdateTransaction(Transaction transaction)
        {
            if (context == null || disposed)
            {
                context = new TraqSoftwareContext();
            }
            transaction.Account = null;
            context.Entry(transaction).State = EntityState.Modified;
        }
        public IEnumerable<Transaction> GetAccountTransactions(int accountCode)
        {
            return context.Transactions.Where(x => x.AccountCode == accountCode);
        }
        public IEnumerable<Transaction> GetAccountCreditTransactions(int accountCode)
        {
            return context.Transactions.Where(x => x.AccountCode == accountCode && x.Description == "Credit Amount");
        }
        public IEnumerable<Transaction> GetAccountDebitTransactions(int accountCode)
        {
            return context.Transactions.Where(x => x.AccountCode == accountCode && x.Description == "Charge Off Amount");
        }
        public IEnumerable<decimal> GetAccountCreditTransactionsAmounts(int accountCode)
        {
            return GetAccountCreditTransactions(accountCode).Select(x => x.Amount);
        }
        public IEnumerable<decimal> GetAccountDebitTransactionsAmounts(int accountCode)
        {
            return GetAccountDebitTransactions(accountCode).Select(x => x.Amount);
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