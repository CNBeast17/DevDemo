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
using SkillsAssessment.DataAccessLayer.UnitOfWork;
using SkillsAssessment.Helpers;
using SkillsAssessment.Keys;
using SkillsAssessment.Models;
using SkillsAssessment.ViewModels;

namespace SkillsAssessment.Controllers
{
    [Authorize]
    public class BankAccountsController : Controller
    {
        private UnitOfWork<TraqSoftwareContext> unitOfWork;
        private IAccountRepository accountRepository;
        private IPersonRepository personRepository;
        private IStatusRepository statusRepository;
        private ITransactionRepository transactionRepository;
        private AccountHelpers _accountHelpers;
        private PersonHelpers _personHelpers;

        public BankAccountsController()
        {
            unitOfWork = new UnitOfWork<TraqSoftwareContext>();
            this.accountRepository = new AccountRepository(unitOfWork);
            this.personRepository = new PersonRepository(unitOfWork);
            this.statusRepository = new StatusRepository(unitOfWork);
            this.transactionRepository = new TransactionRepository(unitOfWork);
            _accountHelpers = new AccountHelpers(unitOfWork);
            _personHelpers = new PersonHelpers(unitOfWork);
        }
        public JsonResult CheckDuplicateAccountNumber(string accountNumber,int? accountCode)
        {
            object result = _accountHelpers.CheckGetDuplicateAccount(accountNumber, accountCode);                  
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        // GET: BankAccounts
        public ActionResult Index()
        {
            return View(_accountHelpers.GetAccounts());
        }

        // GET: BankAccounts/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AccountVM accountVM = _accountHelpers.GetAccountVM(id.Value);
            if (accountVM.Account == null)
            {
                return HttpNotFound();
            }
            return View(accountVM);
        }

        // GET: BankAccounts/Create
        public ActionResult Create(int id)
        {
            Person person = _personHelpers.GetPersonByID(id);
            if (!person.IsActive)
            {
                return View("Details", "People", _personHelpers.GetPersonVMUserDeleted(id));
            }
            else {
                return View(_accountHelpers.GetAccountCreateVM(id));
            }            
        }

        // POST: BankAccounts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(AccountCreateVM accountCreateVM)
        {
            object accountExits = CheckDuplicateAccountNumber(accountCreateVM.Account.AccountNumber, null).Data;
            if (accountExits != null)
            {
                TempData["Success"] = false;
                TempData["CompletedAction"] = "Account Number already Exists!";
                TempData.Keep("Success");
                TempData.Keep("CompletedAction");
                TempData.Keep();
                return View(accountCreateVM);
            }
            if (ModelState.IsValid)
            {
                unitOfWork.CreateTransaction();

                accountCreateVM.Account.AccountStatusCode = statusRepository.GetStatuses().FirstOrDefault(x => x.Key == StatusKeys.AccountOpen).Code;
                accountRepository.InsertAccount(accountCreateVM.Account);
                unitOfWork.Save();
                unitOfWork.Commit();
                TempData["Success"] = true;
                TempData["CompletedAction"] = "Account successfuly created!";
                TempData.Keep("Success");
                TempData.Keep("CompletedAction");
                TempData.Keep();
                return RedirectToAction("Details", "People", new { id = accountCreateVM.Account.PersonCode });
            }

            ViewBag.PersonCode = new SelectList(new List<Person> { personRepository.GetPersonByID(accountCreateVM.Account.PersonCode) }, "Code", "Name", accountCreateVM.Account.PersonCode);
            return View(accountCreateVM);
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
            ViewBag.PersonCode = new SelectList(unitOfWork.Context.People, "Code", "Name", account.PersonCode);
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
                unitOfWork.CreateTransaction();

                accountRepository.UpdateAccount(account);
                unitOfWork.Save();
                unitOfWork.Commit();
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
            unitOfWork.CreateTransaction();

            accountRepository.DeleteAccount(id);
            unitOfWork.Save();
            unitOfWork.Commit();
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

            unitOfWork.CreateTransaction();

            accountRepository.OpenAccount(id);
            unitOfWork.Save();
            unitOfWork.Commit();
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
