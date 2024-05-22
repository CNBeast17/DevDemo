using SkillsAssessment.DataAccessLayer.RepositoryInterfaces;
using SkillsAssessment.Helpers;
using SkillsAssessment.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace SkillsAssessment.DataAccessLayer.Repositories
{
    public class PersonRepository : IPersonRepository
    {
        private TraqSoftwareContext context;
        private string connectionString;
        private SQLViewHelpers sQLViewHelpers;
        public PersonRepository(TraqSoftwareContext context)
        {
            this.sQLViewHelpers = new SQLViewHelpers();
            this.context = context;
            this.connectionString = ConfigurationManager.ConnectionStrings["TraqSoftwareContext"].ConnectionString;
        }
        public IEnumerable<Person> GetPeople()
        {
            List<Person> people = new List<Person>();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = connection.CreateCommand())
                {
                    command.CommandText = "SELECT * from PersonView";
                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        people.AddRange(sQLViewHelpers.ExtractPeople(reader));
                    }
                }
            }
            return people;
        }

        public Person GetPersonByID(int code)
        {
            List<Person> people = new List<Person>();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = connection.CreateCommand())
                {
                    command.CommandText = "SELECT * from PersonView where [code] =" + code;
                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        people.AddRange(sQLViewHelpers.ExtractPeople(reader));
                    }
                }
            }
            return people.FirstOrDefault();
        }

        public void InsertPerson(Person person)
        {
            context.People.Add(person);
        }

        public void DeletePerson(int code)
        {
            Person person = GetPersonByID(code);
            person.IsActive = false;
            UpdatePerson(person);
        }
        public void RestorePerson(int code)
        {
            Person person = GetPersonByID(code);
            person.IsActive = true;
            UpdatePerson(person);
        }
        public void UpdatePerson(Person person)
        {
            context.Entry(person).State = EntityState.Modified;
        }
        public void Save()
        {
            context.SaveChanges();
        }
        public IEnumerable<Person> SearchPeople(string idNum, string name, string surname)
        {
            //Dynamic search that searchs by parameter if value is provided
            //Ignores parameter if value is null or empty
            List<Person> people = new List<Person>();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = connection.CreateCommand())
                {
                    command.CommandText = "SELECT * from PersonView ";
                    
                    if(!string.IsNullOrEmpty(idNum))
                    {
                        command.CommandText += "where [id_number] ='" + idNum + "' ";
                    }
                    else
                    {
                        command.CommandText += "where [code] > 0 ";
                    }
                    if (!string.IsNullOrEmpty(name))
                    {
                        command.CommandText += "and [name] ='" + name + "' ";
                    }
                    else
                    {
                        command.CommandText += "and [code] > 0 ";
                    }
                    if (!string.IsNullOrEmpty(surname))
                    {
                        command.CommandText += "and [surname] ='" + surname + "' ";
                    }
                    else
                    {
                        command.CommandText += "and [code] > 0 ";
                    }
                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        people.AddRange(sQLViewHelpers.ExtractPeople(reader));
                    }
                }
            }
            return people;
        }
        private bool disposed = false;

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