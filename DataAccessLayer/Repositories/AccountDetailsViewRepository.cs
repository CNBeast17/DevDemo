using SkillsAssessment.DataAccessLayer.RepositoryInterfaces;
using SkillsAssessment.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace SkillsAssessment.DataAccessLayer.Repositories
{
    public class AccountDetailsViewRepository: IAccountDetailsViewRepository
    {
        public IEnumerable<Account> GetAccounts()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["TraqSoftwareContext"].ConnectionString;       
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = connection.CreateCommand())
                {
                    command.CommandText = "SELECT * from AccountDetailsView";
                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            // process result
                            reader.GetInt32(0); // get first column from view, assume it's a 32-bit int
                            reader.GetString(1); // get second column from view, assume it's a string
                                                 // etc.
                        }
                    }
                }
            }
            return new List<Account>();
        }
       
    }
}