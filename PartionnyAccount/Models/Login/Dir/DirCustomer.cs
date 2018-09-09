using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PartionnyAccount.Models.Login.Dir
{
    [Table("DirCustomers")]
    public class DirCustomer
    {
        [Key]
        public int DirCustomersID { get; set; }

        /*[System.ComponentModel.DefaultValue(1)]
        [Required]
        public int DirRightsID { get; set; }
        [ForeignKey("DirRightsID")]
        public virtual Dir.DirRights dirRights { get; set; }*/

        //[DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{yyyy-MM-dd}")]
        public DateTime DirCustomersDate { get; set; }

        [Display(Name = "EMail-адрес")]
        [Required]
        [DataType(DataType.EmailAddress)]
        [EmailAddress]
        public string Email { get; set; }

        //http://habrahabr.ru/post/123845/ - Валидации
        [Required(ErrorMessage = "Поле Login не должно быть пустым")]
        [RegularExpression(@"^[a-zA-Z0-9-.]+$", ErrorMessage = @"Извините, но допустимые символы Логина 'Набор из букв и цифр (латиница)'. Подберите Логин только с этими символами!")]
        [StringLength(25, MinimumLength = 3, ErrorMessage = "Длина Логина должна быть от 3 до 25 символов")]
        public string Login { get; set; }

        public string FIO { get; set; }

        [Display(Name = "Страна")]
        [Required]
        public int DirCountryID { get; set; }
        [ForeignKey("DirCountryID")]
        public virtual Models.Login.Dir.DirCountry dirCountry { get; set; }

        [Display(Name = "Язык")]
        [Required]
        public int DirLanguageID { get; set; }
        [ForeignKey("DirLanguageID")]
        public virtual Models.Login.Dir.DirLanguage dirLanguage { get; set; }

        public string Telefon { get; set; }
        public string Pswd { get; set; }
        //[Required]
        public bool? Active { get; set; }
        //[Required]
        public bool? Pay { get; set; }
        public Int32 Refer { get; set; }
        public bool? SendMail { get; set; }
        public bool? Confirmed { get; set; }

        /*
        public string Disc { get; set; }
        [Required]
        public bool SendMsgEndPeriod { get; set; }
        [Required]
        public bool TranslationRu { get; set; }
        [Required]
        public bool TranslationUa { get; set; }
        [Required]
        public bool TranslationUs { get; set; }
        [Required]
        public bool TranslationAz { get; set; }
        */

        public string DomainName { get; set; }



        //Заполняем пустные поля (которые не должны быть пустыми)
        public void Substitute()
        {
            //Заполняем пустные поля
            if (Pay == null) Pay = true;
            if (Active == null) Active = true;
        }


        public string Error { get { return null; } }
        public string this[string propName]
        {
            get
            {
                if ((propName == "Email") && string.IsNullOrEmpty(Email)) return "Некорректный e-mail";
                else if ((propName == "Login") && string.IsNullOrEmpty(Login)) return "Некорректный Login";
                else if ((propName == "DirCountryID") && (DirCountryID < 1)) return "Выбирите страну";
                else if ((propName == "DirLanguageID") && (DirLanguageID < 1)) return "Выбирите язык";
                return null;
            }
        }
    }


    [Table("DirCustomers")]
    public class DirCustomerLogin
    {
        [Key]
        public int DirEmployeesID { get; set; }
        public bool Del { get; set; }

        [Display(Name = "EMail-адрес")]
        [DataType(DataType.EmailAddress)]
        [EmailAddress]
        public string Email { get; set; }

        [Display(Name = "Логин")]
        [Required]
        public string Login { get; set; }

        [Display(Name = "Пароль")]
        [Required]
        public string Pswd { get; set; }

        [Display(Name = "Пароль")]
        [NotMapped]
        public string Pswd2 { get; set; }

        [Required]
        public int DirCountryID { get; set; }
        public string Telefon { get; set; }
        public string FIO { get; set; }

        [Required]
        public int DirInterfaceID { get; set; }
        [Required]
        public int DirThemeID { get; set; }
        [Required]
        public int DirLanguageID { get; set; }

        [Required]
        public int Refer { get; set; }
    }


    [Table("DirCustomers")]
    public class DirCustomerReg
    {
        [Key]
        public int? DirCustomersID { get; set; }
        //public int? DirEmployeesID { get; set; }
        public bool Del { get; set; }

        [Display(Name = "EMail-адрес")]
        [DataType(DataType.EmailAddress)]
        [EmailAddress]
        [Required]
        public string Email { get; set; }

        [Required]
        [Display(Name = "Логин")]
        public string Login { get; set; }
        [Display(Name = "Пароль")]
        public string Pswd { get; set; }
        public string Telefon { get; set; }
        public string FIO { get; set; }

        //[Required]
        public int DirInterfaceID { get; set; }
        //[Required]
        public int DirThemeID { get; set; }
        [Required]
        public int DirLanguageID { get; set; }
        [Display(Name = "Страна")]
        [Required]
        public int DirCountryID { get; set; }

        public bool? SendMail { get; set; }

        //[Required]
        public int Refer { get; set; }
    }
}