using SMBLibrary;
using System;
using System.Collections.Generic;
using System.Text;
using Utilities;
using System.Linq;

namespace PocSMB.Adapters
{
    public class JSONFileSystemAdapter : INTFileStore
    {
        private vfsMock mockValue;
        public JSONFileSystemAdapter()
        {
            Console.WriteLine(Helpers.GetCurrentMethod());
            mockValue = vfsMock.FillMock();
        }

        public NTStatus Cancel(object ioRequest)
        {
            Console.WriteLine(Helpers.GetCurrentMethod());
            return NTStatus.STATUS_NOT_SUPPORTED;
        }

        public NTStatus CloseFile(object handle)
        {
            Console.WriteLine(Helpers.GetCurrentMethod());
            return NTStatus.STATUS_SUCCESS;
        }

        public NTStatus CreateFile(out object handle, out FileStatus fileStatus, string path, AccessMask desiredAccess, FileAttributes fileAttributes, ShareAccess shareAccess, CreateDisposition createDisposition, CreateOptions createOptions, SecurityContext securityContext)
        {
            Console.WriteLine(Helpers.GetCurrentMethod());
            handle = new FileHandle(path, false, null, true);
            fileStatus = FileStatus.FILE_CREATED;
            return NTStatus.STATUS_SUCCESS;
        }

        public NTStatus DeviceIOControl(object handle, uint ctlCode, byte[] input, out byte[] output, int maxOutputLength)
        {
            Console.WriteLine(Helpers.GetCurrentMethod());
            output = null;
            return NTStatus.STATUS_NOT_SUPPORTED;
        }

        public NTStatus FlushFileBuffers(object handle)
        {
            Console.WriteLine(Helpers.GetCurrentMethod());
            return NTStatus.STATUS_SUCCESS;
        }

        public NTStatus GetFileInformation(out FileInformation result, object handle, FileInformationClass informationClass)
        {
            Console.WriteLine(Helpers.GetCurrentMethod());
            FileHandle fileHandle = (FileHandle)handle;
            switch (informationClass)
            {
                case FileInformationClass.FileBasicInformation:
                    {
                        FileBasicInformation information = new FileBasicInformation();
                        information.CreationTime = DateTime.Now.AddDays(-1);
                        information.LastAccessTime = DateTime.Now.AddDays(-1);
                        information.LastWriteTime = DateTime.Now.AddDays(-1);
                        information.ChangeTime = DateTime.Now.AddDays(-1);
                        information.FileAttributes = FileAttributes.Directory;
                        result = information;
                        return NTStatus.STATUS_SUCCESS;
                    }
                case FileInformationClass.FileStandardInformation:
                    {
                        FileStandardInformation information = new FileStandardInformation();
                        information.AllocationSize = (long)10;
                        information.EndOfFile = (long)10;
                        information.Directory = true;
                        information.DeletePending = fileHandle.DeleteOnClose;
                        result = information;
                        return NTStatus.STATUS_SUCCESS;
                    }
                case FileInformationClass.FileInternalInformation:
                    {
                        FileInternalInformation information = new FileInternalInformation();
                        result = information;
                        return NTStatus.STATUS_SUCCESS;
                    }
                case FileInformationClass.FileEaInformation:
                    {
                        FileEaInformation information = new FileEaInformation();
                        information.EaSize = 0;
                        result = information;
                        return NTStatus.STATUS_SUCCESS;
                    }
                case FileInformationClass.FileAccessInformation:
                    {
                        result = null;
                        return NTStatus.STATUS_NOT_IMPLEMENTED;
                    }
                case FileInformationClass.FileNameInformation:
                    {
                        FileNameInformation information = new FileNameInformation();
                        information.FileName = "Test";
                        result = information;
                        return NTStatus.STATUS_SUCCESS;
                    }
                case FileInformationClass.FilePositionInformation:
                    {
                        result = null;
                        return NTStatus.STATUS_NOT_IMPLEMENTED;
                    }
                case FileInformationClass.FileFullEaInformation:
                    {
                        result = null;
                        return NTStatus.STATUS_NOT_IMPLEMENTED;
                    }
                case FileInformationClass.FileModeInformation:
                    {
                        result = null;
                        return NTStatus.STATUS_NOT_IMPLEMENTED;
                    }
                case FileInformationClass.FileAlignmentInformation:
                    {
                        result = null;
                        return NTStatus.STATUS_NOT_IMPLEMENTED;
                    }
                case FileInformationClass.FileAllInformation:
                    {
                        FileAllInformation information = new FileAllInformation();
                        information.BasicInformation.CreationTime = DateTime.Now.AddDays(-1);
                        information.BasicInformation.LastAccessTime = DateTime.Now.AddDays(-1);
                        information.BasicInformation.LastWriteTime = DateTime.Now.AddDays(-1);
                        information.BasicInformation.ChangeTime = DateTime.Now.AddDays(-1);
                        information.BasicInformation.FileAttributes = FileAttributes.Directory;
                        information.StandardInformation.AllocationSize = (long)10;
                        information.StandardInformation.EndOfFile = (long)10;
                        information.StandardInformation.Directory = true;
                        information.StandardInformation.DeletePending = fileHandle.DeleteOnClose;
                        information.NameInformation.FileName = "Test";
                        result = information;
                        return NTStatus.STATUS_SUCCESS;
                    }
                case FileInformationClass.FileAlternateNameInformation:
                    {
                        // If there is no alternate name Windows will return STATUS_OBJECT_NAME_NOT_FOUND
                        result = null;
                        return NTStatus.STATUS_OBJECT_NAME_NOT_FOUND;
                    }
                case FileInformationClass.FileStreamInformation:
                    {
                        // This information class is used to enumerate the data streams of a file or a directory.
                        // A buffer of FileStreamInformation data elements is returned by the server.
                        FileStreamInformation information = new FileStreamInformation();
                        List<KeyValuePair<string, ulong>> dataStreams = new List<KeyValuePair<string, ulong>>();
                        foreach (KeyValuePair<string, ulong> dataStream in dataStreams)
                        {
                            FileStreamEntry streamEntry = new FileStreamEntry();
                            ulong streamSize = dataStream.Value;
                            streamEntry.StreamSize = (long)streamSize;
                            streamEntry.StreamAllocationSize = (long)10;
                            streamEntry.StreamName = dataStream.Key;
                            information.Entries.Add(streamEntry);
                        }
                        result = information;
                        return NTStatus.STATUS_SUCCESS;
                    }
                case FileInformationClass.FilePipeInformation:
                    {
                        result = null;
                        return NTStatus.STATUS_NOT_IMPLEMENTED;
                    }
                case FileInformationClass.FilePipeLocalInformation:
                    {
                        result = null;
                        return NTStatus.STATUS_NOT_IMPLEMENTED;
                    }
                case FileInformationClass.FilePipeRemoteInformation:
                    {
                        result = null;
                        return NTStatus.STATUS_NOT_IMPLEMENTED;
                    }
                case FileInformationClass.FileCompressionInformation:
                    {
                        result = null;
                        return NTStatus.STATUS_NOT_IMPLEMENTED;
                    }
                case FileInformationClass.FileNetworkOpenInformation:
                    {
                        FileNetworkOpenInformation information = new FileNetworkOpenInformation();
                        information.CreationTime = DateTime.Now.AddDays(-1);
                        information.LastAccessTime = DateTime.Now.AddDays(-1);
                        information.LastWriteTime = DateTime.Now.AddDays(-1);
                        information.ChangeTime = DateTime.Now.AddDays(-1);
                        information.AllocationSize = (long)10;
                        information.EndOfFile = (long)10;
                        information.FileAttributes = FileAttributes.Directory;
                        result = information;
                        return NTStatus.STATUS_SUCCESS;
                    }
                case FileInformationClass.FileAttributeTagInformation:
                    {
                        result = null;
                        return NTStatus.STATUS_NOT_IMPLEMENTED;
                    }
                default:
                    result = null;
                    return NTStatus.STATUS_INVALID_INFO_CLASS;
            }
        }

        public NTStatus GetFileSystemInformation(out FileSystemInformation result, FileSystemInformationClass informationClass)
        {
            Console.WriteLine(Helpers.GetCurrentMethod());
            result = null;
            return NTStatus.STATUS_SUCCESS;
        }

        public NTStatus GetSecurityInformation(out SecurityDescriptor result, object handle, SecurityInformation securityInformation)
        {
            Console.WriteLine(Helpers.GetCurrentMethod());
            result = null;
            return NTStatus.STATUS_NOT_SUPPORTED;
        }

        public NTStatus LockFile(object handle, long byteOffset, long length, bool exclusiveLock)
        {
            Console.WriteLine(Helpers.GetCurrentMethod());
            return NTStatus.STATUS_NOT_SUPPORTED;
        }

        public NTStatus NotifyChange(out object ioRequest, object handle, NotifyChangeFilter completionFilter, bool watchTree, int outputBufferSize, OnNotifyChangeCompleted onNotifyChangeCompleted, object context)
        {
            Console.WriteLine(Helpers.GetCurrentMethod());
            ioRequest = null;
            return NTStatus.STATUS_NOT_SUPPORTED;
        }

        public NTStatus QueryDirectory(out List<QueryDirectoryFileInformation> result, object handle, string fileName, FileInformationClass informationClass)
        {
            Console.WriteLine(Helpers.GetCurrentMethod());
            FileHandle fileHandle = (FileHandle)handle;
            Shared s=Helpers.getSharedElement(mockValue.shared[0], fileHandle.Path);
            result = new List<QueryDirectoryFileInformation>();
            if (mockValue.shared[0].path==fileHandle.Path)
                result.Add(Helpers.fromShareToQDFI(mockValue.shared[0], informationClass));
            else
                foreach (Shared c in s.children)
                {
                    result.Add(Helpers.fromShareToQDFI(c, informationClass));
                }
            return NTStatus.STATUS_SUCCESS;
        }

        public NTStatus ReadFile(out byte[] data, object handle, long offset, int maxCount)
        {
            Console.WriteLine(Helpers.GetCurrentMethod());
            data = null;
            return NTStatus.STATUS_SUCCESS;
        }

        public NTStatus SetFileInformation(object handle, FileInformation information)
        {
            Console.WriteLine(Helpers.GetCurrentMethod());
            return NTStatus.STATUS_SUCCESS;
        }

        public NTStatus SetFileSystemInformation(FileSystemInformation information)
        {
            Console.WriteLine(Helpers.GetCurrentMethod());
            return NTStatus.STATUS_SUCCESS;
        }

        public NTStatus SetSecurityInformation(object handle, SecurityInformation securityInformation, SecurityDescriptor securityDescriptor)
        {
            Console.WriteLine(Helpers.GetCurrentMethod());
            return NTStatus.STATUS_NOT_SUPPORTED;
        }

        public NTStatus UnlockFile(object handle, long byteOffset, long length)
        {
            Console.WriteLine(Helpers.GetCurrentMethod());
            return NTStatus.STATUS_NOT_SUPPORTED;
        }

        public NTStatus WriteFile(out int numberOfBytesWritten, object handle, long offset, byte[] data)
        {
            Console.WriteLine(Helpers.GetCurrentMethod());
            numberOfBytesWritten = 0;
            return NTStatus.STATUS_SUCCESS;
        }
    }
}
