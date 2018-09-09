using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PartionnyAccount.Models.Sklad.Dir
{
    [Table("DirDomesticExpenses")]
    public class DirDomesticExpense
    {
        [Key]
        public int? DirDomesticExpenseID { get; set; }
        public bool Del { get; set; }
        [Required]
        public string DirDomesticExpenseName { get; set; }
        [Required]
        public int DirDomesticExpenseType { get; set; }
        [Required]
        public int Sign { get; set; }
    }
}