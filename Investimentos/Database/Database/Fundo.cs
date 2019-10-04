//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Database
{
    using System;
    using System.Collections.Generic;
    
    public partial class Fundo
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Fundo()
        {
            this.Historico = new HashSet<Historico>();
        }
    
        public int FundoID { get; set; }
        public string Categoria { get; set; }
        public string Nome { get; set; }
        public string Tipo { get; set; }
        public Nullable<decimal> RentabilidadeBruta1Ano { get; set; }
        public Nullable<decimal> RentabilidadeLiquida1Ano { get; set; }
        public Nullable<decimal> InvestimentoMinimo { get; set; }
        public string Liquidez { get; set; }
        public Nullable<decimal> IR { get; set; }
        public Nullable<decimal> TaxaAdmAno { get; set; }
        public Nullable<decimal> TaxaCustodiaAno { get; set; }
        public string Emissor { get; set; }
        public string Corretora { get; set; }
        public string Distribuidor { get; set; }
        public string Gestor { get; set; }
        public string Administrador { get; set; }
        public string URL { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Historico> Historico { get; set; }
    }
}
