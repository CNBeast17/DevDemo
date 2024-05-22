using SkillsAssessment.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SkillsAssessment.DataAccessLayer.RepositoryInterfaces
{
    public interface ITransactionRepository:IDisposable
    {
        IEnumerable<Transaction> GetTransactions();
        Transaction GetTransactionByID(int code);
        void InsertTransaction(Transaction transaction);
        void DeleteTransaction(int code);
        void UpdateTransaction(Transaction transaction);
        IEnumerable<Transaction> GetAccountTransactions(int accountCode);
        IEnumerable<Transaction> GetAccountCreditTransactions(int accountCode);
        IEnumerable<Transaction> GetAccountDebitTransactions(int accountCode);
        IEnumerable<decimal> GetAccountCreditTransactionsAmounts(int accountCode);
        IEnumerable<decimal> GetAccountDebitTransactionsAmounts(int accountCode);
        void Save();
    }
}