//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace QLVTFinal.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Material
    {
        public int idMaterial { get; set; }
        public string nameMaterial { get; set; }
        public Nullable<double> price { get; set; }
        public Nullable<int> count { get; set; }
        public Nullable<int> idSubCategory { get; set; }
        public string qrcode { get; set; }
        public Nullable<int> idAdmin { get; set; }
    
        public virtual SubCategory SubCategory { get; set; }
        public virtual tblAdmin tblAdmin { get; set; }

        public string nameSubCategory { get; set; }
        public Nullable<int> idCategory { get; set; }
        public string nameCategory { get; set; }
    }
}
