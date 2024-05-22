using SkillsAssessment.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SkillsAssessment.DataAccessLayer.RepositoryInterfaces
{
    public interface IPersonRepository : IDisposable
    {
        IEnumerable<Person> GetPeople();
        Person GetPersonByID(int code);
        void InsertPerson(Person person);
        void DeletePerson(int code);
        void RestorePerson(int code);
        void UpdatePerson(Person person);
        IEnumerable<Person> SearchPeople(string idNum,string name, string surname);
        void Save();
    }
}