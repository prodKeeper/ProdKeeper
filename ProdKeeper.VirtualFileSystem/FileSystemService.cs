using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Routing.Template;
using ProdKeeper.Entity.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.IO;

namespace ProdKeeper.VirtualFileSystem
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
            if (path.StartsWith("/"))
                path = path.Substring(1);
            if (!path.EndsWith("/"))
                path = path + "/";

            var viewID = path.Split("/")[0];
            if (string.IsNullOrWhiteSpace(viewID))
                throw new Exception("No view");
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
            var keyToReturn = (from rt in routeTemplate.Parameters select rt.Name).ToArray<String>();
            return (keyToReturn, dicReturn);
        }

        public void AccessFile(string path, bool isRead, bool isWrite)
        {
            var file=GetFile(path);
        }


        public Guid CreateView(string libelle, string Pattern)
        {
            var view = (from v in _context.PatternsRepository where v.Patterns == Pattern select v).FirstOrDefault();
            if (view != null)
                return view.Id;
            var pattern = new ProdKeeper.Entity.Models.PatternsRepository();
            pattern.Libelle = libelle;
            pattern.Patterns = Pattern;
            _context.PatternsRepository.Add(pattern);
            _context.SaveChanges();
            return pattern.Id;
        }

        public FileSystemItem[] GetViews()
        {
            List<FileSystemItem> lstFSI = new List<FileSystemItem>();
            var viewIDS = from v in _context.PatternsRepository select v.Id;
            foreach (var view in viewIDS)
            {
                FileSystemItem fsi = new FileSystemItem();
                fsi.FullName = System.IO.Path.Combine("\\", view.ToString());
                fsi.IsDirectory = true;
                fsi.Name = view.ToString();
                fsi.DateCreated = DateTime.Now.AddDays(-1);
                fsi.AccessTime = DateTime.Now;
                fsi.DateModified = DateTime.Now.AddDays(-1);
                lstFSI.Add(fsi);
            }
            return lstFSI.ToArray();
        }

        public FileSystemItem GetItem(string path)
        {
            if (path == "\\")
                return GetRootFolder();
            Entity.Models.Item file;
            try
            {
                file = GetFile(path);
            }
            catch
            {
                file = null;
            }
            if (file == null)
                return GetFolder(path);
            FileSystemItem fsi = new FileSystemItem();
            fsi.FullName = path;
            fsi.IsDirectory = false;
            fsi.Name = file.Libelle;
            var fileVersion = file.ItemVersion.OrderByDescending(i => i.MajorVersion).ThenBy(i=>i.MinorVersion).FirstOrDefault();
            var pathFile = System.IO.Path.Combine(docRepoStore, fileVersion.FilePath.ToString());
            var fi = new System.IO.FileInfo(pathFile);
            fsi.Size = (ulong)fi.Length;
            fsi.AccessTime = fi.LastAccessTime;
            fsi.DateCreated = fi.CreationTime;
            fsi.DateModified = fi.LastWriteTime;
            return fsi;
        }

        private FileSystemItem GetRootFolder()
        {
            FileSystemItem fsi = new FileSystemItem();
            fsi.FullName = "\\";
            fsi.IsDirectory = true;
            fsi.Name = "\\";
            fsi.DateCreated = DateTime.Now.AddDays(-1);
            fsi.AccessTime = DateTime.Now;
            fsi.DateModified = DateTime.Now.AddDays(-1);
            return fsi;

        }

        public FileSystemItem GetFolder(string path)
        {
            if (path.EndsWith("\\"))
                path = path.Substring(0, path.Length - 1);
            (var keyToFind, var dic) = parseString(path);
            if (dic.Count == 0)
                return (from v in GetViews() where v.Name == path.Replace("\\","") select v).FirstOrDefault();

            var folder = path.Split("\\").LastOrDefault();
            var meta = (from mv in _context.MetadataValues where mv.Libelle == folder select mv).FirstOrDefault();
            var files = from fv in _context.ItemVersion
                        join im in _context.ItemMetadata 
                        on fv.Id equals im.IditemVersion
                        where im.IdmetadataValue == meta.Id
                        select fv;

            ulong size = 0;
            foreach (var f in files)
            {
                var pathFile = System.IO.Path.Combine(docRepoStore, f.FilePath.ToString());
                var fi = new System.IO.FileInfo(pathFile);
                size += (ulong)fi.Length;
            }

            FileSystemItem fsi = new FileSystemItem();
            fsi.FullName = path;
            fsi.IsDirectory = true;
            fsi.AccessTime = meta.DateCreated;
            fsi.DateCreated = meta.DateCreated;
            fsi.DateModified = meta.DateCreated;
            fsi.Size = size;
            fsi.Name = folder;
            return fsi;
        }
        public FileSystemItem[] GetFolders(string path)
        {
            List<string> lstReturn = new List<string>();
            List<FileSystemItem> lstFSI = new List<FileSystemItem>();
            (var keyToFind, var dic) = parseString(path);
            DirectoryInfo di = new DirectoryInfo(path);
            var parentFolder =di.Name;

            foreach (var k in keyToFind)
            {
                if (!dic.Keys.Contains(k))
                {
                    var listFolders = (from f in _context.MetadataValues.Include(i => i.InverseIdparentNavigation).Include(i => i.IdkeyNavigation) where (f.IdparentNavigation.Libelle == parentFolder || f.Idparent == null) && f.IdkeyNavigation.Libelle == k select f.Libelle).ToList();
                    lstReturn.AddRange(listFolders.ToArray());
                    break;
                }
            }
            foreach (var folder in lstReturn)
            {
                FileSystemItem fsi = GetFolder(System.IO.Path.Combine(path, folder));
                lstFSI.Add(fsi);
            }
            return lstFSI.ToArray();
        }

        public FileSystemItem[] GetFiles(string path)
        {
            (var keyToFind, var dic) = parseString(path);
            List<FileSystemItem> lstFSI = new List<FileSystemItem>();
            var meta = (from m in _context.MetadataValues.Include(m => m.IdkeyNavigation) where dic.Keys.Any(d => d == m.IdkeyNavigation.Libelle) && dic.Values.Any(d => d == m.Libelle) select m).AsEnumerable();
            var item = (from i in _context.ItemVersion.Include(i => i.ItemMetadata).Include(i => i.IditemNavigation) where i.IditemNavigation.IsDeleted == false && i.ItemMetadata.Any(im => meta.Any(m => m.Id == im.IdmetadataValue)) select i).AsEnumerable();
            var itemFilter = item.Where(i => i.ItemMetadata.Select(im => im.IdmetadataValue).ToArray<int>().SequenceEqual(meta.Select(m => m.Id).ToArray<int>()));


            foreach (var file in itemFilter)
            {
                FileSystemItem fsi = new FileSystemItem();
                fsi.FullName = System.IO.Path.Combine(path, file.IditemNavigation.Libelle);
                fsi.Name = file.IditemNavigation.Libelle;
                fsi.IsDirectory = false;
                fsi.IsArchive = false;
                fsi.IsHidden = false;
                fsi.DateCreated = file.IditemNavigation.DateCreated;
                fsi.DateModified = file.DateCreated;
                fsi.AccessTime = DateTime.Now;
                lstFSI.Add(fsi);
            }
            return lstFSI.ToArray();
        }

        private ProdKeeper.Entity.Models.Item GetFile(string path)
        {
            var dirName = System.IO.Path.GetDirectoryName(path);
            var fileName = System.IO.Path.GetFileName(path);

            (var keyToFind, var dic) = parseString(dirName);

            var meta = (from m in _context.MetadataValues.Include(m => m.IdkeyNavigation) where dic.Keys.Any(d => d == m.IdkeyNavigation.Libelle) && dic.Values.Any(d => d == m.Libelle) select m).AsEnumerable();
            var item = (from i in _context.ItemVersion.Include(i => i.ItemMetadata).Include(i => i.IditemNavigation) where i.IditemNavigation.IsDeleted == false && i.ItemMetadata.Any(im => meta.Any(m => m.Id == im.IdmetadataValue)) && i.IditemNavigation.Libelle == fileName select i).AsEnumerable();
            var itemFilter = item.Where(i => i.ItemMetadata.Select(im => im.IdmetadataValue).ToArray<int>().SequenceEqual(meta.Select(m => m.Id).ToArray<int>())).Select(i => i.IditemNavigation);
            return itemFilter.FirstOrDefault();
        }

        public FileSystemItem CreateFile(string path)
        {
            var item = GetFile(path);
            ProdKeeper.Entity.Models.ItemVersion itemVersion = new ProdKeeper.Entity.Models.ItemVersion(); ;
            (var keyToFind, var dic) = parseString(System.IO.Path.GetDirectoryName(path));
            CreateFolder(System.IO.Path.GetDirectoryName(path));
            var filename = System.IO.Path.GetFileName(path);
            if (item == null)
            {
                item = new ProdKeeper.Entity.Models.Item();
                _context.Item.Add(item);
            }
            item.ItemVersion.Add(itemVersion);
            item.Libelle = filename;
            itemVersion.FilePath = Guid.NewGuid();
            foreach (var d in dic)
            {
                var im = new ProdKeeper.Entity.Models.ItemMetadata();
                int mvID = _context.MetadataValues.Where(mv => mv.Libelle == d.Value).Select(mv => mv.Id).First();
                im.IdmetadataValue = mvID;

                itemVersion.ItemMetadata.Add(im);
            }
            _context.SaveChanges();
            var filePath = System.IO.Path.Combine(this.docRepoStore, itemVersion.FilePath.ToString());
            File.Create(filePath).Dispose();
            FileSystemItem fsi = new FileSystemItem();
            fsi.FullName = path;
            fsi.Name = filename;
            fsi.IsDirectory = false;
            fsi.IsArchive = false;
            fsi.IsHidden = false;
            fsi.DateCreated = item.DateCreated;
            fsi.DateModified = itemVersion.DateCreated;
            fsi.AccessTime = DateTime.Now;
            return fsi;
        }

        public Stream OpenFile(string path, FileMode mode, FileAccess access, FileShare share, FileOptions options = FileOptions.None)
        {
            var file = GetFile(path);
            var fileVersion = file.ItemVersion.OrderByDescending(iv => iv.MajorVersion).ThenBy(iv=>iv.MinorVersion).FirstOrDefault();
            var filePath = System.IO.Path.Combine(this.docRepoStore, fileVersion.FilePath.ToString());
            FileStream fs = new FileStream(filePath, mode, access, share, 4096, options);
            return fs;
        }


        public FileSystemItem CreateFolder(string path)
        {
            (var keyToFind, var dic) = parseString(path);
            string folderCreated = "";
            foreach (var d in dic)
            {
                var mk = _context.MetadataKey.Include(m => m.MetadataValues).Where(mk => mk.Libelle == d.Key).FirstOrDefault();
                if (mk == null)
                    mk = CreateMetaData(d.Key);
                if (mk.MetadataValues.Where(mv => mv.Libelle == d.Value).Count() == 0)
                {
                    var mv = new ProdKeeper.Entity.Models.MetadataValues();
                    mv.Libelle = d.Value;
                    mk.MetadataValues.Add(mv);
                    folderCreated = d.Value;
                }

            }
            _context.SaveChanges();
            FileSystemItem fsi = new FileSystemItem();
            fsi.FullName = path;
            fsi.IsDirectory = true;
            fsi.Name = folderCreated;
            fsi.AccessTime = DateTime.Now;
            fsi.DateCreated = DateTime.Now;
            fsi.DateModified = DateTime.Now;
            return fsi;
        }

        public ProdKeeper.Entity.Models.MetadataKey CreateMetaData(string Libelle)
        {

            var mk = new ProdKeeper.Entity.Models.MetadataKey();
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


        public void MoveFile(string source, string dest)
        {
            var file= GetFile(source);
            file.Libelle = System.IO.Path.GetFileName(dest);
            _context.SaveChanges();
        }

        public void CreateVersion(string path, bool majorVersion=false)
        {
            var file=GetFile(path);
            var oldVersion = file.ItemVersion.OrderByDescending(v => v.MajorVersion).ThenBy(v=>v.MinorVersion).FirstOrDefault();
            Entity.Models.ItemVersion version = new Entity.Models.ItemVersion();
            version.DateCreated = DateTime.Now;
            version.FilePath = Guid.NewGuid();
            if (majorVersion)
            {
                version.MajorVersion = oldVersion.MajorVersion+1;
                version.MinorVersion = 0;
            }
            else
            {
                version.MajorVersion = oldVersion.MajorVersion;
                version.MinorVersion = oldVersion.MinorVersion+1;
            }
            foreach (var md in oldVersion.ItemMetadata)
            {
                Entity.Models.ItemMetadata im = new Entity.Models.ItemMetadata();
                im.IdmetadataValue = md.IdmetadataValue;
                version.ItemMetadata.Add(im);
            }
            string pathSource = System.IO.Path.Combine(docRepoStore, oldVersion.FilePath.ToString());
            string pathDest = System.IO.Path.Combine(docRepoStore, version.FilePath.ToString());
            File.Copy(pathSource, pathDest);
            file.ItemVersion.Add(version);
            _context.SaveChanges();
        }

        public void SetAttributes(string path, bool? isHidden, bool? isReadonly, bool? isArchived)
        {
            var file = GetFile(path);
            if (file == null)
                return;
            if (isHidden.HasValue)
                file.Hidden = isHidden.Value;
            if (isReadonly.HasValue)
                file.ReadOnly = isReadonly.Value;
            if (isArchived.HasValue)
                file.Archive = isArchived.Value;
        }

        public void SetDate(string path, DateTime? creationDT, DateTime? lastWriteDT, DateTime? lastAccessDT)
        {
            var file = GetFile(path);
            if (file == null)
                return;
            if (creationDT.HasValue)
                file.DateCreated = creationDT.Value;
            if (creationDT.HasValue)
                file.DateCreated = creationDT.Value;
            if (lastAccessDT.HasValue)
                file.DateLastAccess = lastAccessDT.Value;

        }

        public void ProcessRecycleBin()
        {
            var iToDelete = from i in _context.Item.Include(iv => iv.ItemVersion).ThenInclude(mv => mv.ItemMetadata) where i.IsDeleted select i;
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

        private void DeleteFileContent(Guid filePath)
        {
            string path = System.IO.Path.Combine(this.docRepoStore, filePath.ToString());
            System.IO.File.Delete(path);
        }
    }
}
