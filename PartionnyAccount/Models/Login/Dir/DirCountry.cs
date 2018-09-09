using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PartionnyAccount.Models.Login.Dir
{
    [Table("DirCountry")]
    public class DirCountry
    {
        [Key]
        public int DirCountryID { get; set; }
        [Required]
        public string DirCountryName { get; set; }
    }
}