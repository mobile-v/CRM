using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PartionnyAccount.Models.Sklad.List
{
    [Table("ListObjectPFs")]
    public class ListObjectPF
    {
        [Key]
        public int? ListObjectPFID { get; set; }
        public int ListObjectPFIDSys { get; set; }

        public bool Del { get; set; }
        public bool SysRecord { get; set; }

        [Display(Name = "Объект")]
        [Required]
        public int ListObjectID { get; set; }
        [ForeignKey("ListObjectID")]
        public virtual List.ListObject listObject { get; set; }

        [Display(Name = "Язык")]
        [Required]
        public int ListLanguageID { get; set; }
        [ForeignKey("ListLanguageID")]
        public virtual List.ListLanguage listLanguage { get; set; }

        public bool? ListObjectPFHtmlCSSUse { get; set; }
        public bool? ListObjectPFSys { get; set; }
        public string ListObjectPFName { get; set; }
        public bool? ListObjectPFHtmlHeaderUse { get; set; }
        public bool? ListObjectPFHtmlDouble { get; set; }
        public string ListObjectPFHtmlHeader { get; set; }
        public bool? ListObjectPFHtmlTabUseCap { get; set; }
        public string ListObjectPFHtmlTabCap { get; set; }
        public bool? ListObjectPFHtmlTabUseTab { get; set; }
        public bool? ListObjectPFHtmlTabEnumerate { get; set; }
        public bool? ListObjectPFHtmlTabFont { get; set; }
        public int ListObjectPFHtmlTabFontSize { get; set; }
        public bool? ListObjectPFHtmlTabUseFooter { get; set; }
        public string ListObjectPFHtmlTabFooter { get; set; }
        public bool? ListObjectPFHtmlTabUseText { get; set; }
        public string ListObjectPFHtmlTabText { get; set; }
        public bool? ListObjectPFHtmlFooterUse { get; set; }
        public string ListObjectPFHtmlFooter { get; set; }
        public string ListObjectPFDesc { get; set; }

        [Display(Name = "Отступы")]
        public int MarginTop { get; set; }
        public int MarginBottom { get; set; }
        public int MarginLeft { get; set; }
        public int MarginRight { get; set; }



        //Табличные данные *** *** *** *** *** *** *** *** *** ***
        [NotMapped]
        public string recordsListObjectPFTab { get; set; }



        //Заполняем пустные поля (которые не должны быть пустыми)
        public void Substitute()
        {
            //Заполняем пустные поля
            if (ListObjectPFHtmlCSSUse == null) ListObjectPFHtmlCSSUse = false;
            if (ListObjectPFSys == null) ListObjectPFSys = false;
            if (ListObjectPFHtmlHeaderUse == null) ListObjectPFHtmlHeaderUse = false;
            if (ListObjectPFHtmlDouble == null) ListObjectPFHtmlDouble = false;
            if (ListObjectPFHtmlTabUseCap == null) ListObjectPFHtmlTabUseCap = false;
            if (ListObjectPFHtmlTabUseTab == null) ListObjectPFHtmlTabUseTab = false;
            if (ListObjectPFHtmlTabEnumerate == null) ListObjectPFHtmlTabEnumerate = false;
            if (ListObjectPFHtmlTabFont == null) ListObjectPFHtmlTabFont = false;
            if (ListObjectPFHtmlTabUseFooter == null) ListObjectPFHtmlTabUseFooter = false;
            if (ListObjectPFHtmlTabUseText == null) ListObjectPFHtmlTabUseText = false;
            if (ListObjectPFHtmlFooterUse == null) ListObjectPFHtmlFooterUse = false;
        }
    }

    [Table("ListObjectPFTabs")]
    public class ListObjectPFTab
    {
        [Key]
        public int? ListObjectPFTabID { get; set; }

        [Display(Name = "Печатная форма (Шапка)")]
        [Required]
        public int ListObjectPFID { get; set; }
        [ForeignKey("ListObjectPFID")]
        public virtual List.ListObjectPF listObjectPF { get; set; }

        public string ListObjectPFTabName { get; set; }

        [Display(Name = "Связь между")]
        [Required]
        public int ListObjectFieldNameID { get; set; }
        [ForeignKey("ListObjectFieldNameID")]
        public virtual List.ListObjectFieldName listObjectFieldName { get; set; }

        public int PositionID { get; set; }
        public int? Percent { get; set; }

        [Display(Name = "№ табличной части, если в ПФ несколько табличных частей")]
        public int? TabNum { get; set; }
        [Display(Name = "Фикс.размер ячейки")]
        public int? Width { get; set; }
    }
}