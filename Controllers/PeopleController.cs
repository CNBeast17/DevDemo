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
    public class PeopleController : Controller
    {
        private UnitOfWork<TraqSoftwareContext> _unitOfWork;
        private PersonHelpers _personHelpers;

        public PeopleController()
        {
            _unitOfWork = new UnitOfWork<TraqSoftwareContext>();           
            _personHelpers = new PersonHelpers(_unitOfWork);
        }

        public JsonResult CheckDuplicateIdNumber(string idNum, int? personCode)
        {
            object result = _personHelpers.CheckDuplicatePerson(idNum, personCode);            
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        // GET: People
        public ActionResult Index(string IdNumber,string Name, string Surname)
        {
            return View(_personHelpers.GetPeopleVM(IdNumber,Name,Surname));
        }

        // GET: People/Details/5
        public ActionResult Details(int? id)
        {
            return View(_personHelpers.GetPersonAccountVM(id));
        }

        // GET: People/Create
        public ActionResult Create()
        {           
            return View(new PersonVM());
        }

        // POST: People/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(PersonVM personVM)
        {        
            object idExists = CheckDuplicateIdNumber(personVM.Person.IDNumber, null).Data;
            if (idExists != null)
            {
                return View(_personHelpers.GetPersonVMUserExists(personVM.Person));
            }
            else
            {
                return View("Index", _personHelpers.SavePerson(personVM.Person));
            }
        }

        // GET: People/Edit/5
        public ActionResult Edit(int? id)
        {
            return View(_personHelpers.GetPersonVM(id.Value));
        }

        // POST: People/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Person person)
        {
            object idExists = CheckDuplicateIdNumber(person.IDNumber, person.Code).Data;
            if (idExists != null)
            {
                return View(_personHelpers.GetPersonVMUserExists(person));
            }
            else 
            {
                return View("Index", _personHelpers.UpdatePerson(person));
            }
        }

        // GET: People/Delete/5
        public ActionResult Delete(int? id)
        {            
            if (_personHelpers.PersonHasActiveAccount(id))
            {
                return View("Index",_personHelpers.GetPeopleVMUserHasActiveAccount());
            }
            else
            {
                return View(_personHelpers.GetPersonVM(id.Value));
            }
        }

        // POST: People/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            return View("Index", _personHelpers.DeletePerson(id));
        }


        public ActionResult Restore(int? id)
        {
            Person person = _personHelpers.GetPersonByID(id.Value);
            if (person.IsActive)
            {
                return View("Edit", _personHelpers.GetPersonVMCannotRestoreActiveUser(id.Value));
            }
            else
            {
                return View(_personHelpers.GetPersonVM(id.Value));
            }
        }

        // POST: People/Restore/5
        [HttpPost, ActionName("Restore")]
        [ValidateAntiForgeryToken]
        public ActionResult RestoreConfirmed(int id)
        {
            return View("Index", _personHelpers.RestorePerson(id));
        }
        protected override void Dispose(bool disposing)
        {
            _personHelpers.Dispose();
        }
    }
}
