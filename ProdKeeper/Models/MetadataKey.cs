using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace ProdKeeper.Models
{
    public partial class MetadataKey
    {
        public MetadataKey()
        {
            InverseIdparentNavigation = new HashSet<MetadataKey>();
            MetadataValues = new HashSet<MetadataValues>();
        }

        public int Id { get; set; }
        public string Libelle { get; set; }
        public int? Idparent { get; set; }

        public virtual MetadataKey IdparentNavigation { get; set; }
        public virtual ICollection<MetadataKey> InverseIdparentNavigation { get; set; }
        public virtual ICollection<MetadataValues> MetadataValues { get; set; }
    }
}
