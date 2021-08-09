using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;

namespace PocSMB.Adapters
{
    
    public class Shared
    {
        public string name { get; set; }
        public bool isFolder { get; set; }
        public DateTime creationTime { get; set; }
        public DateTime lastAccess { get; set; }
        public DateTime lastWrite { get; set; }
        public DateTime changeTime { get; set; }
        public List<Shared> children { get; set; }
        public string path { get; set; }
    }

    public class vfsMock
    {
        public List<Shared> shared { get; set; }

        public static vfsMock FillMock()
        {
            string jsonContent = File.ReadAllText(Path.Combine(System.AppContext.BaseDirectory, "vfs.json"));
            vfsMock mock = JsonSerializer.Deserialize<vfsMock>(jsonContent);
            return mock;
        }
    }
}
