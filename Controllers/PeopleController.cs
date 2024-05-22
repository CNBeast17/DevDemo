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
using SkillsAssessment.ViewModels;

namespace SkillsAssessment.Controllers
{
    [Authorize]
    public class PeopleController : Controller
    {
        private TraqSoftwareContext db;
        private IPersonRepository personRepository;
        private IAccountRepository accountRepository;
        private IAccountDetailsViewRepository accountDetailsViewRepository;

        public PeopleController()
        {
            db = new TraqSoftwareContext();
            this.personRepository = new PersonRepository(db);
            this.accountRepository = new AccountRepository(db);
            this.accountDetailsViewRepository = new AccountDetailsViewRepository();
        }

        public JsonResult CheckDuplicateIdNumber(string idNum, int? personCode)
        {
            object result = null;
            try
            {                
                if (idNum != "" && idNum != null)
                {
                    //reuse of functionality 
                    //instead of creating another similar method that specifically brings back one record
                    result = personRepository.SearchPeople(idNum, "", "").Where(x => x.Code != personCode).FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                string e = ex.Message;
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        // GET: People
        public ActionResult Index(string IdNumber,string Name, string Surname)
        {
            //accountDetailsViewRepository.GetAccounts();
            return View(personRepository.SearchPeople(IdNumber,Name,Surname));
            //return View(personRepository.GetPeople());
        }

        // GET: People/Details/5
        public ActionResult Details(int? id)
        {

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            IAccountRepository accountRepository = new AccountRepository(new TraqSoftwareContext());
            PersonAccountsVM personAccountsVM = new PersonAccountsVM();
            personAccountsVM.Person = personRepository.GetPersonByID(id.Value);
            //Person person = personRepository.GetPersonByID(id.Value);
            personAccountsVM.Accounts = accountRepository.GetPersonAccounts(id.Value).ToList();
            //ViewData["AccountInfo"] = accountRepository.GetPersonAccounts(id.Value);
            if (personAccountsVM.Person == null)
            {
                return HttpNotFound();
            }
            return View(personAccountsVM);
        }

        // GET: People/Create
        public ActionResult Create()
        {
            
            return View();
        }

        // POST: People/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Code,Name,Surname,IDNumber")] Person person)
        {        
            object idExists = CheckDuplicateIdNumber(person.IDNumber, null).Data;
            if (idExists != null)
            {
                TempData["Success"] = false;
                TempData["CompletedAction"] = "ID Number already Exists!";
                TempData.Keep("Success");
                TempData.Keep("CompletedAction");
                TempData.Keep();
                return View(person);
            }
            if (ModelState.IsValid)
            {
                person.IsActive = true;
                personRepository.InsertPerson(person);
                personRepository.Save();
                TempData["Success"] = true;
                TempData["CompletedAction"] = "Person successfuly created!";
                TempData.Keep("Success");
                TempData.Keep("CompletedAction");
                TempData.Keep();
                return RedirectToAction("Index");
            }

            return View(person);
        }

        // GET: People/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Person person = personRepository.GetPersonByID(id.Value);
            if (person == null)
            {
                return HttpNotFound();
            }
            return View(person);
        }

        // POST: People/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Code,Name,Surname,IDNumber,IsActive")] Person person)
        {
            object idExists = CheckDuplicateIdNumber(person.IDNumber, person.Code).Data;
            if (idExists != null)
            {
                TempData["Success"] = false;
                TempData["CompletedAction"] = "ID Number already Exists!";
                TempData.Keep("Success");
                TempData.Keep("CompletedAction");
                TempData.Keep();
                return View(person);
            }
            if (ModelState.IsValid)
            {
                personRepository.UpdatePerson(person);
                personRepository.Save();
                TempData["Success"] = true;
                TempData["CompletedAction"] = "Person successfuly updated!";
                TempData.Keep("Success");
                TempData.Keep("CompletedAction");
                TempData.Keep();
                return RedirectToAction("Index");
            }
            return View(person);
        }

        // GET: People/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Person person = personRepository.GetPersonByID(id.Value);
           
            
            if (person == null)
            {
                return HttpNotFound();
            }
            List<Account> accounts = accountRepository.GetPersonAccounts(id.Value).ToList();
            //checks if person has at least one open account
            if (accounts.Select(x => x.Status.Key).Contains(StatusKeys.AccountOpen))
            {
                TempData["Success"] = false;
                TempData["CompletedAction"] = "People with open accounts cannot be deleted!";
                TempData.Keep("Success");
                TempData.Keep("CompletedAction");
                TempData.Keep();
                return RedirectToAction("Index");
            }
            return View(person);
        }

        // POST: People/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            personRepository.DeletePerson(id);
            personRepository.Save();
            TempData["Success"] = true;
            TempData["CompletedAction"] = "Person has been deleted successfuly!";
            TempData.Keep("Success");
            TempData.Keep("CompletedAction");
            TempData.Keep();
            return RedirectToAction("Index");
  
        }


        public ActionResult Restore(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Person person = personRepository.GetPersonByID(id.Value);
            if (person == null)
            {
                return HttpNotFound();
            }
            if (person.IsActive)
            {
                TempData["Success"] = false;
                TempData["CompletedAction"] = "Only deleted people can be restored!";
                TempData.Keep("Success");
                TempData.Keep("CompletedAction");
                TempData.Keep();
                return RedirectToAction("Index");
            }
            return View(person);
        }

        // POST: People/Restore/5
        [HttpPost, ActionName("Restore")]
        [ValidateAntiForgeryToken]
        public ActionResult RestoreConfirmed(int id)
        {
            personRepository.RestorePerson(id);
            personRepository.Save();
            TempData["Success"] = true;
            TempData["CompletedAction"] = "Person has been restored successfuly!";
            TempData.Keep("Success");
            TempData.Keep("CompletedAction");
            TempData.Keep();
            return RedirectToAction("Index");
        }
        protected override void Dispose(bool disposing)
        {
            personRepository.Dispose();
        }
    }
}
