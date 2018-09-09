using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PartionnyAccount.Models.Sklad.Dir
{
    [Table("DirSmsTemplates")]
    public class DirSmsTemplate
    {
        [Key]
        public int? DirSmsTemplateID { get; set; }
        [Required]
        public string DirSmsTemplateName { get; set; }
        [Required]
        public string DirSmsTemplateMsg { get; set; }
        [Display(Name = "Тип: 1-Другое, 2-Отремонтированный, 3-Отказной")]
        [Required]
        public int DirSmsTemplateType { get; set; }

        [Display(Name = "Тип меню: 1-Сервисный центр, 2-Логистика, 3-Заказы")]
        public int MenuID { get; set; }
    }
}