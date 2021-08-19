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
        string docRepoStore;
        public FileSystemService(FileSystemOption option)
        {
            _context = option.DbContext;
            docRepoStore = option.StorePath;
        }



        private (string[], Dictionary<string, string>) parseString(string path)
        {
            Dictionary<string, string> dicReturn = new Dictionary<string, string>();
            if (path.Contains("\\"))
                path = path.Replace("\\", "/");
            if (!path.EndsWith("/"))
                path = path + "/";

            var viewID = path.Split("/")[0];
            path = path.Replace(viewID, "");
            if (!path.EndsWith("/"))
                path = path + "/";
            var patternRepo = _context.PatternsRepository.FirstOrDefault(x => x.Id == Guid.Parse(viewID));
            if (patternRepo == null)
                throw new Exception("View does not exist");
            var pattern = patternRepo.Patterns;
            var keyToFind = pattern.Substring(1).Split("/").Where(x => !string.IsNullOrEmpty(x)).ToArray();
            var patternValue = "/" + String.Join("/", keyToFind);
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


        public Guid CreateView(string libelle, string Pattern)
        {
            var view = (from v in _context.PatternsRepository where v.Patterns == Pattern select v).FirstOrDefault();
            if (view != null)
                return view.Id;
            var pattern = new Models.PatternsRepository();
            pattern.Libelle = libelle;
            pattern.Patterns = Pattern;
            _context.PatternsRepository.Add(pattern);
            _context.SaveChanges();
            return pattern.Id;
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
            (var keyToFind, var dic) = parseString(path);

            var meta = (from m in _context.MetadataValues.Include(m => m.IdkeyNavigation) where dic.Keys.Any(d => d == m.IdkeyNavigation.Libelle) && dic.Values.Any(d => d == m.Libelle) select m).AsEnumerable();
            var item = (from i in _context.ItemVersion.Include(i => i.ItemMetadata).Include(i => i.IditemNavigation) where i.IditemNavigation.IsDeleted == false && i.ItemMetadata.Any(im => meta.Any(m => m.Id == im.IdmetadataValue)) select i).AsEnumerable();
            var itemFilter = item.Where(i => i.ItemMetadata.Select(im => im.IdmetadataValue).ToArray<int>().SequenceEqual(meta.Select(m => m.Id).ToArray<int>()));
            return itemFilter.Select(i => i.IditemNavigation.Libelle).ToArray();
        }

        public Models.Item GetFile(string path)
        {
            var dirName = System.IO.Path.GetDirectoryName(path);
            var fileName = System.IO.Path.GetFileName(path);
            (var keyToFind, var dic) = parseString(dirName);

            var meta = (from m in _context.MetadataValues.Include(m => m.IdkeyNavigation) where dic.Keys.Any(d => d == m.IdkeyNavigation.Libelle) && dic.Values.Any(d => d == m.Libelle) select m).AsEnumerable();
            var item = (from i in _context.ItemVersion.Include(i => i.ItemMetadata).Include(i => i.IditemNavigation) where i.IditemNavigation.IsDeleted == false && i.ItemMetadata.Any(im => meta.Any(m => m.Id == im.IdmetadataValue)) && i.IditemNavigation.Libelle == fileName select i).AsEnumerable();
            var itemFilter = item.Where(i => i.ItemMetadata.Select(im => im.IdmetadataValue).ToArray<int>().SequenceEqual(meta.Select(m => m.Id).ToArray<int>())).Select(i => i.IditemNavigation);
            return itemFilter.FirstOrDefault();
        }

        public void SaveFile(string path, byte[] content)
        {
            var item = GetFile(path);
            Models.ItemVersion itemVersion = new Models.ItemVersion(); ;
            System.IO.FileInfo fi = SaveFileContent(content);
            (var keyToFind, var dic) = parseString(System.IO.Path.GetDirectoryName(path));
            CreateFolder(System.IO.Path.GetDirectoryName(path));
            var filename = System.IO.Path.GetFileName(path);
            if (item == null)
            {
                item = new Models.Item();
                _context.Item.Add(item);
                item.DateCreated = fi.CreationTime;
            }
            item.ItemVersion.Add(itemVersion);
            itemVersion.FilePath = fi.Name;
            itemVersion.DateCreated = fi.CreationTime;
            item.DateLastAccess = fi.LastWriteTime;
            item.Archive = ((fi.Attributes & System.IO.FileAttributes.Archive) == System.IO.FileAttributes.Archive);
            item.Hidden = ((fi.Attributes & System.IO.FileAttributes.Hidden) == System.IO.FileAttributes.Hidden);
            item.ReadOnly = ((fi.Attributes & System.IO.FileAttributes.ReadOnly) == System.IO.FileAttributes.ReadOnly);
            item.Libelle = filename;
            foreach (var d in dic)
            {
                var im = new Models.ItemMetadata();
                int mvID = _context.MetadataValues.Where(mv => mv.Libelle == d.Value).Select(mv => mv.Id).First();
                im.IdmetadataValue = mvID;
                itemVersion.ItemMetadata.Add(im);
            }
            _context.SaveChanges();
        }

        private System.IO.FileInfo SaveFileContent(byte[] content)
        {
            Guid guidFile = Guid.NewGuid();
            string path = System.IO.Path.Combine(this.docRepoStore, guidFile.ToString());
            System.IO.File.WriteAllBytes(path, content);
            System.IO.FileInfo fi = new System.IO.FileInfo(path);
            return fi;
        }


        public void CreateFolder(string path)
        {
            (var keyToFind, var dic) = parseString(path);
            foreach (var d in dic)
            {
                var mk = _context.MetadataKey.Include(m => m.MetadataValues).Where(mk => mk.Libelle == d.Key).FirstOrDefault();
                if (mk == null)
                    mk = CreateMetaData(d.Key);
                if (mk.MetadataValues.Where(mv => mv.Libelle == d.Value).Count() == 0)
                {
                    var mv = new Models.MetadataValues();
                    mv.Libelle = d.Value;
                    mk.MetadataValues.Add(mv);
                }

            }
            _context.SaveChanges();
        }

        public Models.MetadataKey CreateMetaData(string Libelle)
        {

            var mk = new Models.MetadataKey();
            mk.Libelle = Libelle;
            _context.MetadataKey.Add(mk);
            _context.SaveChanges();
            return mk;
        }
        public void DeleteFolder(string path)
        {
            var files = GetFiles(path);
            if (files.Length > 0)
                throw new Exception("Can't delete non empty file");

            var folderToDelete = System.IO.Directory.GetParent(path).Name;
            var metaval = (from val in _context.MetadataValues where val.Libelle == folderToDelete select val).FirstOrDefault();
            if (metaval == null)
                throw new Exception("Folder does not exist");
            _context.MetadataValues.Remove(metaval);
            _context.SaveChanges();
        }

        public void DeleteFile(string path)
        {
            var item = GetFile(path);
            if (item == null)
                throw new Exception("File does not exist");
            item.IsDeleted = true;

            _context.SaveChanges();
        }


        public void ProcessRecycleBin()
        {
            var iToDelete = from i in _context.Item.Include(iv => iv.ItemVersion).ThenInclude(mv=>mv.ItemMetadata) where i.IsDeleted select i;
            foreach (var item in iToDelete)
            {
                foreach (var version in item.ItemVersion.AsEnumerable())
                {
                    DeleteFileContent(version.FilePath);
                    foreach (var metavalue in version.ItemMetadata.AsEnumerable())
                    {
                        _context.ItemMetadata.Remove(metavalue);
                    }
                    _context.ItemVersion.Remove(version);

                }
                _context.Item.Remove(item);
            }
            _context.SaveChanges();

        }

        private void DeleteFileContent(string filePath)
        {
            string path = System.IO.Path.Combine(this.docRepoStore, filePath);
            System.IO.File.Delete(path);
        }
    }
}
