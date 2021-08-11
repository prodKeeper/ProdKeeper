using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace ProdKeeper.Models
{
    public partial class Item
    {
        public Item()
        {
            ItemMetadata = new HashSet<ItemMetadata>();
        }

        public int Id { get; set; }
        public string Libelle { get; set; }
        public byte[] FileContent { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateModified { get; set; }
        public DateTime DateLastAccess { get; set; }
        public double Version { get; set; }
        public bool Hidden { get; set; }
        public bool ReadOnly { get; set; }
        public bool Archive { get; set; }

        public virtual ICollection<ItemMetadata> ItemMetadata { get; set; }
    }
}
