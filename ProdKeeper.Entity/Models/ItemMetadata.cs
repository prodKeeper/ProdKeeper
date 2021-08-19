using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace ProdKeeper.Models
{
    public partial class ItemMetadata
    {
        public int Id { get; set; }
        public int IdmetadataValue { get; set; }
        public int IditemVersion { get; set; }

        public virtual ItemVersion IditemVersionNavigation { get; set; }
        public virtual MetadataValues IdmetadataValueNavigation { get; set; }
    }
}
