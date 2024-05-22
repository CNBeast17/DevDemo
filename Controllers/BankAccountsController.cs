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
    public class BankAccountsController : Controller
    {
        private TraqSoftwareContext db = new TraqSoftwareContext();
        private IAccountRepository accountRepository;
        private IPersonRepository personRepository;
        private IStatusRepository statusRepository;
        private ITransactionRepository transactionRepository;

        public BankAccountsController()
        {
            this.db = new TraqSoftwareContext();
            this.accountRepository = new AccountRepository(db);
            this.personRepository = new PersonRepository(db);
            this.statusRepository = new StatusRepository(db);
            this.transactionRepository = new TransactionRepository(db);
        }
        public JsonResult CheckDuplicateAccountNumber(string accountNumber,int? accountCode)
        {
            object result = null;
             if (accountNumber != "" && accountNumber != null)
                {
                    //reuse of functionality 
                    //instead of creating another similar method that specifically brings back one record
                    result = accountRepository.SearchAccounts(accountNumber).Where(x=>x.Code != accountCode).FirstOrDefault();
                }         
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        // GET: BankAccounts
        public ActionResult Index()
        {
            return View(accountRepository.GetAccounts());
        }

        // GET: BankAccounts/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }           
            Account account = accountRepository.GetAccountByID(id.Value);
            ViewData["TransactionsInfo"] = transactionRepository.GetAccountTransactions(id.Value);
            if (account == null)
            {
                return HttpNotFound();
            }
            return View(account);
        }

        // GET: BankAccounts/Create
        public ActionResult Create(int id)
        {
            Person person = personRepository.GetPersonByID(id);
            if (!person.IsActive)
            {
                TempData["Success"] = false;
                TempData["CompletedAction"] = "Accounts cannot be created for deleted people";
                TempData.Keep("Success");
                TempData.Keep("CompletedAction");
                TempData.Keep();
                return RedirectToAction("Details", "People", new { id = id });
            }
            ViewBag.PersonCode = new SelectList(new List<Person> { personRepository.GetPersonByID(id) }, "Code", "Name", id);
            ViewBag.StatusCode = new SelectList(statusRepository.GetStatuses(), "Code", "Name");
            return View(new Account { PersonCode = id});
        }

        // POST: BankAccounts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Code,PersonCode,AccountNumber,OutstandingBalance,AccountStatusCode")] Account account)
        {
            object accountExits = CheckDuplicateAccountNumber(account.AccountNumber, null).Data;
            if (accountExits != null)
            {
                TempData["Success"] = false;
                TempData["CompletedAction"] = "Account Number already Exists!";
                TempData.Keep("Success");
                TempData.Keep("CompletedAction");
                TempData.Keep();
                return View(account);
            }
            if (ModelState.IsValid)
            {
                account.AccountStatusCode = statusRepository.GetStatuses().FirstOrDefault(x => x.Key == StatusKeys.AccountOpen).Code;
                accountRepository.InsertAccount(account);
                accountRepository.Save();
                TempData["Success"] = true;
                TempData["CompletedAction"] = "Account successfuly created!";
                TempData.Keep("Success");
                TempData.Keep("CompletedAction");
                TempData.Keep();
                return RedirectToAction("Details", "People", new { id = account.PersonCode });
            }

            ViewBag.PersonCode = new SelectList(new List<Person> { personRepository.GetPersonByID(account.PersonCode) }, "Code", "Name", account.PersonCode);
            return View(account);
        }

        // GET: BankAccounts/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Account account = accountRepository.GetAccountByID(id.Value);
            if (account == null)
            {
                return HttpNotFound();
            }
            ViewBag.PersonCode = new SelectList(db.People, "Code", "Name", account.PersonCode);
            return View(account);
        }

        // POST: BankAccounts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Code,PersonCode,AccountNumber,OutstandingBalance,AccountStatusCode")] Account account)
        {
           
            object accountExits = CheckDuplicateAccountNumber(account.AccountNumber, account.Code).Data;
            if (accountExits != null)
            {
                TempData["Success"] = false;
                TempData["CompletedAction"] = "Account Number already Exists!";
                TempData.Keep("Success");
                TempData.Keep("CompletedAction");
                TempData.Keep();
                return View(account);
            }
            if (ModelState.IsValid)
            {
                accountRepository.UpdateAccount(account);
                accountRepository.Save();
                TempData["Success"] = true;
                TempData["CompletedAction"] = "Account successfuly updated!";
                TempData.Keep("Success");
                TempData.Keep("CompletedAction");
                TempData.Keep();
                return RedirectToAction("Details", "People", new { id = account.PersonCode });
            }
            ViewBag.PersonCode = new SelectList(new List<Person> { personRepository.GetPersonByID(account.PersonCode) }, "Code", "Name", account.PersonCode);
            return View(account);
        }

        // GET: BankAccounts/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Account account = accountRepository.GetAccountByID(id.Value);
            account.SetAccountBalance(transactionRepository.GetAccountDebitTransactionsAmounts(id.Value).ToList()
                , transactionRepository.GetAccountCreditTransactionsAmounts(id.Value).ToList());
            if (account.OutstandingBalance != 0m)
            {
                TempData["Success"] = false;
                TempData["CompletedAction"] = "Account with non zero balances cannot be closed!";
                TempData.Keep("Success");
                TempData.Keep("CompletedAction");
                TempData.Keep();
                return RedirectToAction("Details", new { id = id });
            }
            else if (account.Status.Key == StatusKeys.AccountClosed)
            {
                TempData["Success"] = false;
                TempData["CompletedAction"] = "Account already closed!";
                TempData.Keep("Success");
                TempData.Keep("CompletedAction");
                TempData.Keep();
                return RedirectToAction("Details", new { id = id });
            }
            if (account == null)
            {
                return HttpNotFound();
            }
            return View(account);
        }

        // POST: BankAccounts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {

            accountRepository.DeleteAccount(id);
            accountRepository.Save();
            TempData["Success"] = true;
            TempData["CompletedAction"] = "Account has been closed!";
            TempData.Keep("Success");
            TempData.Keep("CompletedAction");
            TempData.Keep();
            return RedirectToAction("Details", new { id = id });
        }
        public ActionResult Open(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Account account = accountRepository.GetAccountByID(id.Value);
            Person person = personRepository.GetPersonByID(account.PersonCode);
            if (!person.IsActive)
            {
                TempData["Success"] = false;
                TempData["CompletedAction"] = "Accounts for deleted users cannot be reopened!";
                TempData.Keep("Success");
                TempData.Keep("CompletedAction");
                TempData.Keep();
                return RedirectToAction("Details", new { id = id });
            }
            else if (account.Status.Key == StatusKeys.AccountOpen)
            {
                TempData["Success"] = false;
                TempData["CompletedAction"] = "Account already opened!";
                TempData.Keep("Success");
                TempData.Keep("CompletedAction");
                TempData.Keep();
                return RedirectToAction("Details", new { id = id });
            }
            if (account == null)
            {
                return HttpNotFound();
            }
            return View(account);
        }

        // POST: BankAccounts/Delete/5
        [HttpPost, ActionName("Open")]
        [ValidateAntiForgeryToken]
        public ActionResult OpenConfirmed(int id)
        {

            accountRepository.OpenAccount(id);
            accountRepository.Save();
            TempData["Success"] = true;
            TempData["CompletedAction"] = "Account has been reopened!";
            TempData.Keep("Success");
            TempData.Keep("CompletedAction");
            TempData.Keep();
            return RedirectToAction("Details", new { id = id });
        }
        protected override void Dispose(bool disposing)
        {
            accountRepository.Dispose();
        }
    }
}
