using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace ProdKeeper.Entity.Models
{
    public partial class ItemVersion
    {
        public ItemVersion()
        {
            ItemMetadata = new HashSet<ItemMetadata>();
        }

        public int Id { get; set; }
        public int Iditem { get; set; }
        public double Version { get; set; }
        public Guid FilePath { get; set; }
        public DateTime DateCreated { get; set; }

        public virtual Item IditemNavigation { get; set; }
        public virtual ICollection<ItemMetadata> ItemMetadata { get; set; }
    }
}
