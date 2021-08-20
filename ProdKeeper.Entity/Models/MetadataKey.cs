using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace ProdKeeper.Entity.Models
{
    public partial class MetadataKey
    {
        public MetadataKey()
        {
            MetadataValues = new HashSet<MetadataValues>();
        }

        public int Id { get; set; }
        public string Libelle { get; set; }

        public virtual ICollection<MetadataValues> MetadataValues { get; set; }
    }
}
