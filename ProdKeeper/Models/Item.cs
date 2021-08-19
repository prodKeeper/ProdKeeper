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
            ItemVersion = new HashSet<ItemVersion>();
        }

        public int Id { get; set; }
        public string Libelle { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateLastAccess { get; set; }
        public bool Hidden { get; set; }
        public bool ReadOnly { get; set; }
        public bool Archive { get; set; }
        public bool IsDeleted { get; set; }

        public virtual ICollection<ItemVersion> ItemVersion { get; set; }
    }
}
