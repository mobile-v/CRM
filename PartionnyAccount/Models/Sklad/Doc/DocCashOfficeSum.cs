using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PartionnyAccount.Models.Sklad.Doc
{
    [Table("DocCashOfficeSums")]
    public class DocCashOfficeSum
    {
        [Key]
        public int? DocCashOfficeSumID { get; set; }

        [Display(Name = "Касса")]
        [Required]
        public int DirCashOfficeID { get; set; }
        [ForeignKey("DirCashOfficeID")]
        public virtual Dir.DirCashOffice dirCashOffice { get; set; }

        [Display(Name = "Сотрудник, который создал запись")]
        //[Required]
        public int? DirEmployeeID { get; set; }
        [ForeignKey("DirEmployeeID")]
        public virtual Dir.DirEmployee dirEmployee { get; set; }

        [Display(Name = "Тип кассовой операции")]
        //[Required]
        public int DirCashOfficeSumTypeID { get; set; }
        [ForeignKey("DirCashOfficeSumTypeID")]
        public virtual Dir.DirCashOfficeSumType dirCashOfficeSumType { get; set; }

        [Display(Name = "Дата и время операции")]
        //[Required]
        public DateTime? DocCashOfficeSumDate { get; set; }

        [Display(Name = "Дата операции")]
        //[Required]
        public DateTime? DateOnly { get; set; }

        [Display(Name = "Документ - на основании кого документа создана запись")]
        public int? DocID { get; set; }
        [ForeignKey("DocID")]
        public virtual Doc doc { get; set; }

        [Display(Name = "Документ - ID-шник документа: DocPurches, DocSales, ...")]
        public int? DocXID { get; set; }

        [Display(Name = "Сумма документа")]
        public double DocCashOfficeSumSum { get; set; }

        [Display(Name = "Валюта")]
        //[Required]
        public int? DirCurrencyID { get; set; }
        [ForeignKey("DirCurrencyID")]
        public virtual Dir.DirCurrency dirCurrency { get; set; }

        public double? DirCurrencyRate { get; set; }
        public int? DirCurrencyMultiplicity { get; set; }

        public string Base { get; set; }
        public int? DirEmployeeIDMoney { get; set; }
        public string Description { get; set; }

        public double? Discount { get; set; }


        #region KKMS

        [Display(Name = "Номер чека в ККМ")]
        public int? KKMSCheckNumber { get; set; }
        [Display(Name = "Уникалдьный номер комманды ККМS")]
        public string KKMSIdCommand { get; set; }

        //параметры клиента
        [Display(Name = "EMail")]
        public string KKMSEMail { get; set; }
        [Display(Name = "Phone")]
        public string KKMSPhone { get; set; }

        #endregion

    }



    [Table("DocCashOfficeSumMovements")]
    public class DocCashOfficeSumMovement
    {
        [Key]
        public int? DocCashOfficeSumMovementID { get; set; }

        [Display(Name = "Документ")]
        //[Required]
        public int? DocID { get; set; }
        [ForeignKey("DocID")]
        public virtual Models.Sklad.Doc.Doc doc { get; set; }


        [Display(Name = "Склад с")]
        [Required]
        public int DirWarehouseIDFrom { get; set; }
        [ForeignKey("DirWarehouseIDFrom")]
        public virtual Dir.DirWarehouse dirWarehouseFrom { get; set; }

        [Display(Name = "Касса с")]
        [Required]
        public int DirCashOfficeIDFrom { get; set; }
        [ForeignKey("DirCashOfficeIDFrom")]
        public virtual Dir.DirCashOffice dirCashOfficeFrom { get; set; }

        [Display(Name = "Сумма с")]
        [Required]
        public double DirCashOfficeSumFrom { get; set; }


        [Display(Name = "Склад на")]
        [Required]
        public int DirWarehouseIDTo { get; set; }
        [ForeignKey("DirWarehouseIDTo")]
        public virtual Dir.DirWarehouse dirWarehouseTo { get; set; }

        [Display(Name = "Касса на")]
        [Required]
        public int DirCashOfficeIDTo { get; set; }
        [ForeignKey("DirCashOfficeIDTo")]
        public virtual Dir.DirCashOffice dirCashOfficeTo { get; set; }

        [Display(Name = "Сумма на")]
        [Required]
        public double DirCashOfficeSumTo { get; set; }


        public bool Reserve { get; set; }

        [Display(Name = "Сумма")]
        [Required]
        public double Sums { get; set; }
        //[Required]
        public double? SumsCurrency { get; set; }

        [Display(Name = "Валюта")]
        [Required]
        public int DirCurrencyID { get; set; }
        [ForeignKey("DirCurrencyID")]
        public virtual Dir.DirCurrency dirCurrency { get; set; }

        public double DirCurrencyRate { get; set; }
        public int DirCurrencyMultiplicity { get; set; }


        [NotMapped]
        public string DescriptionMovement { get; set; }

        [Display(Name = "Курьер")]
        public int? DirEmployeeIDCourier { get; set; }
        [ForeignKey("DirEmployeeIDCourier")]
        public virtual Dir.DirEmployee dirEmployee_Courier { get; set; }


        //1 Перемещение
        //2	Логистика: в ожидании курьера
        //3	Логистика: курьер принял
        //4	Логистика: курьер отдал
        [Display(Name = "Статус Логистики")]
        //[Required]
        public int? DirMovementStatusID { get; set; }
        [ForeignKey("DirMovementStatusID")]
        public virtual Dir.DirMovementStatus dirMovementStatus { get; set; }



        #region  Doc

        [NotMapped]
        public bool? Del { get; set; }
        [NotMapped]
        public int ListObjectID { get; set; }
        [NotMapped]
        public string NumberInt { get; set; }
        [NotMapped]
        public DateTime? DocDate { get; set; }
        [NotMapped]
        public DateTime? DocDateCreate { get; set; }
        [NotMapped]
        public bool? Held { get; set; }
        [NotMapped]
        public double Discount { get; set; }
        [NotMapped]
        [Display(Name = "На основании какого документа создан данный")]
        public int? DocIDBase { get; set; }
        [NotMapped]
        public string Base { get; set; }
        [Display(Name = "Организация")]
        [NotMapped]
        public int DirContractorIDOrg { get; set; }
        [Display(Name = "Контрагент")]
        [NotMapped]
        public int DirContractorID { get; set; }
        [Display(Name = "Кто создал документ. Используется для начислении премии сотруднику")]
        [NotMapped]
        public int DirEmployeeID { get; set; }
        [NotMapped]
        public bool? IsImport { get; set; }
        [NotMapped]
        public string Description { get; set; }
        [Display(Name = "Тип оплаты: Касса или Банк.")]
        [NotMapped]
        public int DirPaymentTypeID { get; set; }
        [Display(Name = "Сумма оплаты")]
        [NotMapped]
        public double Payment { get; set; }
        [Display(Name = "НДС")]
        [NotMapped]
        public double DirVatValue { get; set; }

        #region KKMS

        [Display(Name = "Номер чека в ККМ")]
        [NotMapped]
        public int? KKMSCheckNumber { get; set; }
        [Display(Name = "Уникалдьный номер комманды ККМS")]
        [NotMapped]
        public string KKMSIdCommand { get; set; }

        //параметры клиента
        [Display(Name = "EMail")]
        [NotMapped]
        public string KKMSEMail { get; set; }
        [Display(Name = "Phone")]
        [NotMapped]
        public string KKMSPhone { get; set; }

        #endregion

        [NotMapped]
        public DateTime? DocDateHeld { get; set; }
        [NotMapped]
        public DateTime? DocDatePayment { get; set; }

        #endregion



        //Табличные данные *** *** *** *** *** *** *** *** *** ***
        [NotMapped]
        public string recordsDocCashOfficeSumMovementTab { get; set; }



        //Заполняем пустные поля (которые не должны быть пустыми)
        public void Substitute()
        {
            //Заполняем пустные поля
            //if (String.IsNullOrEmpty(DirContractorLegalName)) DirContractorLegalName = DirContractorName;
            if (DirPaymentTypeID == null) DirPaymentTypeID = 1;
            if (SumsCurrency == null) SumsCurrency = Sums * DirCurrencyRate / DirCurrencyMultiplicity;
            if (Payment == null) Payment = 0;
            ListObjectID = 79;
        }
    }


}