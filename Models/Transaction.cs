using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SkillsAssessment.Models
{
    [Table("Transactions")]
    public class Transaction
    {
        [Key]
        [Column("code")]
        public int Code { get; set; }
        [Column("account_code")]
        public int AccountCode { get; set; }
        [ForeignKey("AccountCode")]
        public Account Account { get; set; }
        [Display(Name = "Transaction Date")]
        [Column("transaction_date")]
        public DateTime TransactionDate { get; set; }
        [Column("capture_date")]
        public DateTime CaptureDate { get; set; }
        [Column("amount")]
        [DataType(DataType.Currency)]
        public decimal Amount { get; set; }
        [MaxLength(100)]
        [Column("description")]
        public string Description { get; set; }

        public void SetCaptureDate()
        {
            CaptureDate = DateTime.Now;
        }
    }
}