using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PartionnyAccount.Models.Sklad.Service.API
{
    [Table("API10s")]
    public class API10
    {
        [Key]
        public int? API10ID { get; set; }
        public bool Del { get; set; }
        [Required]
        public string API10Key { get; set; }

        //Export
        public bool? ExportDirNomens { get; set; }
        public bool? ExportDirNomen_DirNomenNameFull { get; set; }
        public bool? ExportDirNomen_Description { get; set; }
        public bool? ExportDirNomen_DescriptionFull { get; set; }
        public bool? ExportDirNomen_ImageLink { get; set; }

        public bool? ExportDirChars { get; set; }
        public bool? ExportDirContractors { get; set; }
        public bool? ExportRemRemnants { get; set; }
        public bool? ExportRemParties { get; set; }

        //СЦ
        public bool? ExportDirServiceNomens { get; set; }

        //БУ
        public bool? ExportRem2Parties { get; set; }


        //Import
        public bool? ImportDirNomens { get; set; }
        public bool? ImportDirContractors { get; set; }
        public bool? ImportDocOrderInts { get; set; }


        public void Substitute()
        {
            if (ExportDirNomens == null) ExportDirNomens = false;
            if (ExportDirNomen_DirNomenNameFull == null) ExportDirNomen_DirNomenNameFull = false;
            if (ExportDirNomen_Description == null) ExportDirNomen_Description = false;
            if (ExportDirNomen_DescriptionFull == null) ExportDirNomen_DescriptionFull = false;
            if (ExportDirNomen_ImageLink == null) ExportDirNomen_ImageLink = false;
            if (ExportDirContractors == null) ExportDirContractors = false;
            if (ExportRemRemnants == null) ExportRemRemnants = false;
            if (ExportRemParties == null) ExportRemParties = false;
            if (ExportDirServiceNomens == null) ExportDirServiceNomens = false;
            if (ExportRem2Parties == null) ExportRem2Parties = false;

            if (ImportDirNomens == null) ImportDirNomens = false;
            if (ImportDirContractors == null) ImportDirContractors = false;
            if (ImportDocOrderInts == null) ImportDocOrderInts = false;
        }

    }
}