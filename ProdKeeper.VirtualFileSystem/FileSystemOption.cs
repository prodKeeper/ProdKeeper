using ProdKeeper.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProdKeeper.Services
{
    public class FileSystemOption
    {
        public FileSystemOption(ApplicationDbContext context)
        {
            DbContext = context;
            StorePath = System.IO.Directory.GetCurrentDirectory();
        }

        public ApplicationDbContext DbContext { get; set; }

        public string StorePath { get; set; }

    }
}
