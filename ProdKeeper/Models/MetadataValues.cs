using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace ProdKeeper.Models
{
    public partial class MetadataValues
    {
        public MetadataValues()
        {
            ItemMetadata = new HashSet<ItemMetadata>();
        }

        public int Id { get; set; }
        public string Libelle { get; set; }
        public int Idkey { get; set; }

        public virtual MetadataKey IdkeyNavigation { get; set; }
        public virtual ICollection<ItemMetadata> ItemMetadata { get; set; }
    }
}
