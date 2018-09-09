using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PartionnyAccount.Models.Sklad.Dir
{
    [Table("DirCharTextures")]
    public class DirCharTexture
    {
        [Key]
        public int? DirCharTextureID { get; set; }
        public bool Del { get; set; }
        [Required]
        public string DirCharTextureName { get; set; }
    }
}