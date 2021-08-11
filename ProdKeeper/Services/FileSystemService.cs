using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Routing.Template;
using ProdKeeper.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace ProdKeeper.Services
{
    public class FileSystemService
    {
        ApplicationDbContext _context;
        public FileSystemService(ApplicationDbContext context)
        {
            _context = context;
        }

        private (string[], Dictionary<string, string>) parseString(string path)
        {
            Dictionary<string, string> dicReturn = new Dictionary<string, string>();
            if (path.Contains("\\"))
                path = path.Replace("\\", "/");

            var viewID = path.Split("/")[0];
            path = path.Replace(viewID, "");
            if (!path.EndsWith("/"))
                path = path + "/";
            var pattern = _context.PatternsRepository.First(x => x.Id == Guid.Parse(viewID)).Patterns;
            var keyToFind = pattern.Substring(1).Split("/").Where(x => !string.IsNullOrEmpty(x)).ToArray();
            var patternValue = "/{" + String.Join("?}/{", keyToFind) + "?}";
            var routeTemplate = TemplateParser.Parse(patternValue);
            var matcher = new TemplateMatcher(routeTemplate, null);
            var values = new RouteValueDictionary();

            if (matcher.TryMatch(path, values))
            {
                foreach (var item in values)
                {
                    dicReturn.Add(item.Key, item.Value.ToString());
                }
            }

            return (keyToFind, dicReturn);
        }

        public string[] GetFolders(string path)
        {
            List<string> lstReturn = new List<string>();
            //https://localhost:5001/FileSystem/1540CA18-4EA1-4285-B3DA-83ED6DADAABE/Europe/France/
            (var keyToFind, var dic) = parseString(path);
            foreach (var k in keyToFind)
            {
                if (!dic.Keys.Contains(k))
                {
                    var folders = from m in _context.MetadataValues.Include(m => m.IdkeyNavigation) where k == m.IdkeyNavigation.Libelle select m.Libelle;
                    lstReturn.AddRange(folders.ToArray());
                    break;
                }
            }
            return lstReturn.ToArray();
        }

        public string[] GetFiles(string path)
        {
            //https://localhost:5001/FileSystem/1540CA18-4EA1-4285-B3DA-83ED6DADAABE/Europe/France/Lorraine/
            (var keyToFind, var dic) = parseString(path);

            var meta = from m in _context.MetadataValues.Include(m => m.IdkeyNavigation) where dic.Keys.Any(d => d == m.IdkeyNavigation.Libelle) && dic.Values.Any(d => d == m.Libelle) select m;
            var item = (from i in _context.ItemMetadata.Include(i => i.IditemNavigation) where meta.Any(m => m.Id == i.IdmetadataValue) select i.IditemNavigation.Libelle);


            return item.ToArray();
        }

        public void SaveFile(string path, byte[] content)
        { }

        public void CreateFolder(string path)
        { }


    }
}
