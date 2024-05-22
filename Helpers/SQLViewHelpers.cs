using SkillsAssessment.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace SkillsAssessment.Helpers
{
    public class SQLViewHelpers
    {
        public List<Person> ExtractPeople(SqlDataReader reader)
        {
            List<Person> people = new List<Person>();
            while (reader.Read())
            {
                Person person = new Person
                {
                    Code = reader.GetInt32(0),
                    Name = reader.GetString(1),
                    Surname = reader.GetString(2),
                    IDNumber = reader.GetString(3),
                    IsActive = reader.GetBoolean(4)
                };
                people.Add(person);
            }
            return people;
        }
        public List<Account> ExtractAccounts(SqlDataReader reader)
        {
            List<Account> accounts = new List<Account>();
            while (reader.Read())
            {
                Account account = new Account
                {
                    Code = reader.GetInt32(0),
                    PersonCode = reader.GetInt32(1),
                    AccountNumber = reader.GetString(2),
                    OutstandingBalance = reader.GetDecimal(3),
                    AccountStatusCode = reader.GetInt32(4),
                    Status = new Status
                    {
                        Name = reader.GetString(5),
                        Key = reader.GetString(8)
                    },
                    Person = new Person
                    {
                        Name = reader.GetString(6),
                        Surname = reader.GetString(7)
                    },
                };
                accounts.Add(account);
            }
            return accounts;
        }
    }
}