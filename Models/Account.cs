using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SkillsAssessment.Models
{
    [Table("Accounts")]
    public class Account
    {
        [Key]
        [Column("code")]
        public int Code { get; set; }

        [Column("person_code")]
        public int PersonCode { get; set; }
        [ForeignKey("PersonCode")]
        public Person Person { get; set; }

        [Index(IsUnique = true)]
        [Display(Name = "Account Number")]
        [MaxLength(50)]        
        [Column("account_number")]
        public string AccountNumber { get; set; }

        [Column("outstanding_balance")]
        [Display(Name ="Outstanding Balance")]
        [DataType(DataType.Currency)]
        public decimal OutstandingBalance { get; set; }

        [Column("account_status_code")]
        public int? AccountStatusCode { get; set; }
        [ForeignKey("AccountStatusCode")]
        public Status Status { get; set; }

        public void SetAccountBalance(List<decimal> debitAmounts, List<decimal> creditAmounts)
        {
            decimal accountBalance = 0m;
            for(int i = 0; i<debitAmounts.Count;i++)
            {
                accountBalance += debitAmounts[i];
            }
            for (int i = 0; i < creditAmounts.Count; i++)
            {
                accountBalance -= creditAmounts[i];
            }
            OutstandingBalance = accountBalance;
        }
    }
}