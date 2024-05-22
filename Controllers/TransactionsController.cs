using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using SkillsAssessment.DataAccessLayer.Repositories;
using SkillsAssessment.DataAccessLayer.RepositoryInterfaces;
using SkillsAssessment.Keys;
using SkillsAssessment.Models;

namespace SkillsAssessment.Controllers
{
    [Authorize]
    public class TransactionsController : Controller
    {
        private TraqSoftwareContext db;
        private ITransactionRepository transactionRepository;
        private IAccountRepository accountRepository;
        private IStatusRepository statusRepository;

        public TransactionsController()
        {
            db = new TraqSoftwareContext();
            this.transactionRepository = new TransactionRepository(db);
            this.accountRepository = new AccountRepository(db);
            this.statusRepository = new StatusRepository(db);
        }
        // GET: Transactions
        public ActionResult Index()
        {         
            return View(transactionRepository.GetTransactions());
        }

        // GET: Transactions/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Transaction transaction = transactionRepository.GetTransactionByID(id.Value);
            if (transaction == null)
            {
                return HttpNotFound();
            }
            return View(transaction);
        }

        // GET: Transactions/Create
        public ActionResult Create(int? Code)
        {
           
            Account account = accountRepository.GetAccountByID(Code.Value);
            if(statusRepository.GetStatusByID(account.AccountStatusCode.Value).Key == StatusKeys.AccountClosed)
            {
                TempData["Success"] = false;
                TempData["CompletedAction"] = "Transactions cannot be captured for closed accounts!";
                TempData.Keep("Success");
                TempData.Keep("CompletedAction");
                TempData.Keep();
                return RedirectToAction("Details", "BankAccounts", new { id = Code });
            }


            //if account does not exist return error message
            ViewBag.Description = new SelectList(new List<string> { "Charge Off Amount", "Credit Amount" });
            ViewBag.AccountCode = new SelectList(new List<Account> { accountRepository.GetAccountByID(Code.Value) }, "Code", "AccountNumber", Code);
            return View(new Transaction { 
            AccountCode=Code.Value
            });;
        }

        // POST: Transactions/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Code,AccountCode,TransactionDate,Amount,Description")] Transaction transaction)
        {
            if (ModelState.IsValid)
            {               
                transaction.SetCaptureDate();
                transactionRepository.InsertTransaction(transaction);
                transactionRepository.Save();
                //Update and save account balance with new transaction
                this.accountRepository = new AccountRepository(new TraqSoftwareContext());
                Account account = accountRepository.GetAccountByID(transaction.AccountCode);
                account.SetAccountBalance(transactionRepository.GetAccountDebitTransactionsAmounts(transaction.AccountCode).ToList()
                    ,transactionRepository.GetAccountCreditTransactionsAmounts(transaction.AccountCode).ToList());        
                accountRepository.UpdateAccount(account);
                accountRepository.Save();

                TempData["Success"] = true;
                TempData["CompletedAction"] = "Transaction captured succesfully!";
                TempData.Keep("Success");
                TempData.Keep("CompletedAction");
                TempData.Keep();
                return RedirectToAction("Details","BankAccounts",new { id=transaction.AccountCode});
            }
            ViewBag.Description = new SelectList(new List<string> { "Charge Off Amount", "Credit Amount" },transaction.Description);
            ViewBag.AccountCode = new SelectList(new List<Account> { accountRepository.GetAccountByID(transaction.AccountCode) }, "Code", "AccountNumber", transaction.AccountCode);
            return View(transaction);
        }

        // GET: Transactions/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Transaction transaction = transactionRepository.GetTransactionByID(id.Value);
            if (transaction == null)
            {
                return HttpNotFound();
            }
            ViewBag.Description = new SelectList(new List<string> { "Charge Off Amount", "Credit Amount" },transaction.Description);
            ViewBag.AccountCode = new SelectList(new List<Account> { accountRepository.GetAccountByID(transaction.AccountCode) }, "Code", "AccountNumber", transaction.AccountCode);
            return View(transaction);
        }

        // POST: Transactions/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Code,AccountCode,TransactionDate,Amount,Description")] Transaction transaction)
        {
            if (ModelState.IsValid)
            {
                transaction.SetCaptureDate();
                transactionRepository.UpdateTransaction(transaction);
                transactionRepository.Save();

                //Update and save account balance with new transaction
                this.accountRepository = new AccountRepository(new TraqSoftwareContext());
                Account account = accountRepository.GetAccountByID(transaction.AccountCode);
                account.SetAccountBalance(transactionRepository.GetAccountDebitTransactionsAmounts(transaction.AccountCode).ToList()
                    , transactionRepository.GetAccountCreditTransactionsAmounts(transaction.AccountCode).ToList());
                accountRepository.UpdateAccount(account);
                accountRepository.Save();
                return RedirectToAction("Details", "BankAccounts", new { id = transaction.AccountCode });
            }
            ViewBag.Description = new SelectList(new List<string> { "Charge Off Amount", "Credit Amount" }, transaction.Description);
            ViewBag.AccountCode = new SelectList(new List<Account> { accountRepository.GetAccountByID(transaction.AccountCode) }, "Code", "AccountNumber", transaction.AccountCode);
            return View(transaction);
        }

        // GET: Transactions/Delete/5
        //public ActionResult Delete(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    Transaction transaction = transactionRepository.GetTransactionByID(id.Value);
        //    if (transaction == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(transaction);
        //}

        //// POST: Transactions/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public ActionResult DeleteConfirmed(int id)
        //{
        //    transactionRepository.DeleteTransaction(id);
        //    transactionRepository.Save();
        //    return RedirectToAction("Index");
        //}

        protected override void Dispose(bool disposing)
        {
            transactionRepository.Dispose();
        }
    }
}
