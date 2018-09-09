using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PartionnyAccount.Models.Sklad.Logistic
{
    public class Logistic
    {
        [Key]
        public int? LogisticID { get; set; }

        public int? TypeXID { get; set; }

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

        [Display(Name = "Склад на")]
        [Required]
        public int DirWarehouseIDTo { get; set; }
        [ForeignKey("DirWarehouseIDTo")]
        public virtual Dir.DirWarehouse dirWarehouseTo { get; set; }

        public bool Reserve { get; set; }

        [Display(Name = "Комментарий: причина ")]
        public int? DirMovementDescriptionID { get; set; }
        [ForeignKey("DirMovementDescriptionID")]
        public virtual Dir.DirMovementDescription dirMovementDescription { get; set; }

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



        //Таблица Doc *** *** *** *** *** *** *** *** *** ***
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



        //Табличные данные *** *** *** *** *** *** *** *** *** ***
        [NotMapped]
        public string recordsDocMovementTab { get; set; }



        //Заполняем пустные поля (которые не должны быть пустыми)
        public void Substitute(Models.Sklad.Doc.DocMovement docMovement, Models.Sklad.Doc.DocSecondHandMovement docSecondHandMovement)
        {
            if (docMovement != null)
            {

            }
            else if (docSecondHandMovement != null)
            {

            }
        }
    }
}