using DiskAccessLibrary.FileSystems.Abstractions;
using ProdKeeper.Data;
using ProdKeeper.VirtualFileSystem;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace PocSMB.Adapters
{
    public class ProdKeeperSMBAdapter : FileSystem
    {

        private FileSystemService _fss;
        public ProdKeeperSMBAdapter(FileSystemService fss)
        {
            _fss = fss;

        }
        public override string Name => "ProdKeeper";

        public override long Size => 10000000;

        public override long FreeSpace => Size / 2;

        public override bool SupportsNamedStreams => false;

        public override FileSystemEntry CreateDirectory(string path)
        {
            var fsi = _fss.CreateFolder(path);
            FileSystemEntry fse = new FileSystemEntry(fsi.FullName, fsi.Name, fsi.IsDirectory, fsi.Size, fsi.DateCreated, fsi.DateModified, fsi.AccessTime, fsi.IsHidden, false, fsi.IsArchive);
            return fse;
        }

        public override FileSystemEntry CreateFile(string path)
        {
            var fsi = _fss.CreateFile(path);
            FileSystemEntry fse = new FileSystemEntry(fsi.FullName, fsi.Name, fsi.IsDirectory, fsi.Size, fsi.DateCreated, fsi.DateModified, fsi.AccessTime, fsi.IsHidden, false, fsi.IsArchive);
            return fse;

        }

        public override void Delete(string path)
        {
            _fss.DeleteFile(path);
        }

        public override FileSystemEntry GetEntry(string path)
        {
            FileSystemItem fsi = null;
            try
            {
                fsi = _fss.GetItem(path);
            }
            catch (Exception ex)
            {
                return null;
            }
            if (fsi == null)
                return null;
            FileSystemEntry fse = new FileSystemEntry(fsi.FullName, fsi.Name, fsi.IsDirectory, fsi.Size, fsi.DateCreated, fsi.DateModified, fsi.AccessTime, fsi.IsHidden, false, fsi.IsArchive);
            return fse;
        }

        public override List<FileSystemEntry> ListEntriesInDirectory(string path)
        {
            List<FileSystemEntry> lstFSE = new List<FileSystemEntry>();
            FileSystemItem[] folder = new FileSystemItem[0];
            if (path == "\\")
            {
                folder = _fss.GetViews();
            }
            else
            {
                FileSystemItem[] files = new FileSystemItem[0];
                try
                {
                    files = _fss.GetFiles(path);
                    folder = _fss.GetFolders(path);
                }
                catch { }
                
                foreach (var fi in files)
                {
                    FileSystemEntry fse = new FileSystemEntry(fi.FullName, fi.Name, fi.IsDirectory, fi.Size, fi.DateCreated, fi.DateModified, fi.AccessTime, fi.IsHidden, false, fi.IsArchive);
                    lstFSE.Add(fse);
                }
            }
            foreach (var fo in folder)
            {
                FileSystemEntry fse = new FileSystemEntry(fo.FullName, fo.Name, fo.IsDirectory, fo.Size, fo.DateCreated, fo.DateModified, fo.AccessTime, fo.IsHidden, false, fo.IsArchive);
                lstFSE.Add(fse);
            }
            return lstFSE;
        }

        public override void Move(string source, string destination)
        {
            throw new NotImplementedException();
        }

        public override Stream OpenFile(string path, FileMode mode, FileAccess access, FileShare share, FileOptions options)
        {
            return _fss.OpenFile(path, mode, access, share, options);
        }

        public override void SetAttributes(string path, bool? isHidden, bool? isReadonly, bool? isArchived)
        {
            throw new NotImplementedException();
        }

        public override void SetDates(string path, DateTime? creationDT, DateTime? lastWriteDT, DateTime? lastAccessDT)
        {
            throw new NotImplementedException();
        }
    }
}
