using DiskAccessLibrary.FileSystems.Abstractions;
using System;
using System.Collections.Generic;
using System.IO;

namespace ProdKeeper.SMBAdapter
{
    public class ProdKeeperFileSystem : FileSystem
    {
        public override string Name => "ProdKeeper FileSystem";

        public override long Size => 10000000000;

        public override long FreeSpace => 5000000;

        public override bool SupportsNamedStreams => true;

        public override FileSystemEntry CreateDirectory(string path)
        {
            throw new NotImplementedException();
        }

        public override FileSystemEntry CreateFile(string path)
        {
            throw new NotImplementedException();
        }

        public override void Delete(string path)
        {
            throw new NotImplementedException();
        }

        public override FileSystemEntry GetEntry(string path)
        {
            throw new NotImplementedException();
        }

        public override List<FileSystemEntry> ListEntriesInDirectory(string path)
        {
            throw new NotImplementedException();
        }

        public override void Move(string source, string destination)
        {
            throw new NotImplementedException();
        }

        public override Stream OpenFile(string path, FileMode mode, FileAccess access, FileShare share, FileOptions options)
        {
            throw new NotImplementedException();
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
