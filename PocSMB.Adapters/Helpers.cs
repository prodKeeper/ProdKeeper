using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;
using System.Linq;
using SMBLibrary;

namespace PocSMB.Adapters
{
    public static class Helpers
    {
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static string GetCurrentMethod()
        {
            var st = new StackTrace();
            var sf = st.GetFrame(1);

            return sf.GetMethod().Name;
        }

        public static IEnumerable<T> Flatten<T>(this T source, Func<T, IEnumerable<T>> selector)
        {
            return selector(source).SelectMany(c => Flatten(c, selector))
                                   .Concat(new[] { source });
        }


        public static Shared getSharedElement(Shared shared, string path)
        {
            foreach (Shared s in shared.children)
            {
                if (s.path == path)
                    return s;
                else
                    return getSharedElement(s, path);
            }
            return null;
        }


        public static QueryDirectoryFileInformation fromShareToQDFI(Shared s, FileInformationClass informationClass)
        {
            switch (informationClass)
            {
                case FileInformationClass.FileBothDirectoryInformation:
                    {
                        FileBothDirectoryInformation result = new FileBothDirectoryInformation();
                        result.CreationTime = s.creationTime;
                        result.LastAccessTime = s.lastAccess;
                        result.LastWriteTime = s.lastWrite;
                        result.ChangeTime = s.lastWrite;
                        result.EndOfFile = (long)10;
                        result.AllocationSize = (long)10;
                        result.FileAttributes = s.isFolder?FileAttributes.Directory:FileAttributes.Normal;
                        result.EaSize = 0;
                        result.FileName = s.name;
                        return result;
                    }
                case FileInformationClass.FileDirectoryInformation:
                    {
                        FileDirectoryInformation result = new FileDirectoryInformation();
                        result.CreationTime = s.creationTime;
                        result.LastAccessTime = s.lastAccess;
                        result.LastWriteTime = s.lastWrite;
                        result.ChangeTime = s.lastWrite;
                        result.EndOfFile = (long)10;
                        result.AllocationSize = (long)10;
                        result.FileAttributes = s.isFolder ? FileAttributes.Directory : FileAttributes.Normal;
                        result.FileName = s.name;
                        return result;
                    }
                case FileInformationClass.FileFullDirectoryInformation:
                    {
                        FileFullDirectoryInformation result = new FileFullDirectoryInformation();
                        result.CreationTime = s.creationTime;
                        result.LastAccessTime = s.lastAccess;
                        result.LastWriteTime = s.lastWrite;
                        result.ChangeTime = s.lastWrite;
                        result.EndOfFile = (long)10;
                        result.AllocationSize = (long)10;
                        result.FileAttributes = s.isFolder ? FileAttributes.Directory : FileAttributes.Normal;
                        result.EaSize = 0;
                        result.FileName = s.name;
                        return result;
                    }
                case FileInformationClass.FileIdBothDirectoryInformation:
                    {
                        FileIdBothDirectoryInformation result = new FileIdBothDirectoryInformation();
                        result.CreationTime = s.creationTime;
                        result.LastAccessTime = s.lastAccess;
                        result.LastWriteTime = s.lastWrite;
                        result.ChangeTime = s.lastWrite;
                        result.EndOfFile = (long)10;
                        result.AllocationSize = (long)10;
                        result.FileAttributes = s.isFolder ? FileAttributes.Directory : FileAttributes.Normal;
                        result.EaSize = 0;
                        result.FileId = 0;
                        result.FileName = s.name;
                        return result;
                    }
                case FileInformationClass.FileIdFullDirectoryInformation:
                    {
                        FileIdFullDirectoryInformation result = new FileIdFullDirectoryInformation();
                        result.CreationTime = s.creationTime;
                        result.LastAccessTime = s.lastAccess;
                        result.LastWriteTime = s.lastWrite;
                        result.ChangeTime = s.lastWrite;
                        result.EndOfFile = (long)10;
                        result.AllocationSize = (long)10;
                        result.FileAttributes = s.isFolder ? FileAttributes.Directory : FileAttributes.Normal;
                        result.EaSize = 0;
                        result.FileId = 0;
                        result.FileName = s.name;
                        return result;
                    }
                case FileInformationClass.FileNamesInformation:
                    {
                        FileNamesInformation result = new FileNamesInformation();
                        result.FileName = s.name;
                        return result;
                    }
                default:
                    {
                        throw new UnsupportedInformationLevelException();
                    }
            }
        }
    }
}
