using System;
using System.Collections.Generic;
using System.Text;

namespace ProdKeeper.VirtualFileSystem
{
    public class FileSystemItem
    {
        public string FullName { get; set; }
        public string Name { get; set; }

        public bool IsDirectory { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateModified { get; set; }
        public DateTime AccessTime { get; set; }
        public bool IsHidden { get; set; }
        public bool IsArchive { get; set; }

        public ulong Size { get; set; }
    }
}
