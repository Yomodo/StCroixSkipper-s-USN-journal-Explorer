using System;
using System.IO;
using System.Runtime.InteropServices;

namespace PInvoke
{
   public static class Win32Api
   {
      public enum GetLastErrorEnum
      {
         INVALID_HANDLE_VALUE = -1,
         ERROR_SUCCESS = 0,
         ERROR_INVALID_FUNCTION = 1,
         ERROR_FILE_NOT_FOUND = 2,
         ERROR_PATH_NOT_FOUND = 3,
         ERROR_TOO_MANY_OPEN_FILES = 4,
         ERROR_ACCESS_DENIED = 5,
         ERROR_INVALID_HANDLE = 6,
         ERROR_INVALID_DATA = 13,
         ERROR_HANDLE_EOF = 38,
         ERROR_NOT_SUPPORTED = 50,
         ERROR_INVALID_PARAMETER = 87,
         ERROR_JOURNAL_DELETE_IN_PROGRESS = 1178,
         ERROR_JOURNAL_NOT_ACTIVE = 1179,
         ERROR_JOURNAL_ENTRY_DELETED = 1181,
         ERROR_INVALID_USER_BUFFER = 1784
      }

      public enum UsnJournalDeleteFlags
      {
         USN_DELETE_FLAG_DELETE = 1,
         USN_DELETE_FLAG_NOTIFY = 2
      }

      public enum FILE_INFORMATION_CLASS
      {
         //FileDirectoryInformation = 1, // 1
         //FileFullDirectoryInformation = 2, // 2
         //FileBothDirectoryInformation = 3, // 3
         //FileBasicInformation = 4, // 4
         //FileStandardInformation = 5, // 5
         //FileInternalInformation = 6, // 6
         //FileEaInformation = 7, // 7
         //FileAccessInformation = 8, // 8
         FileNameInformation = 9, // 9
         //FileRenameInformation = 10, // 10
         //FileLinkInformation = 11, // 11
         //FileNamesInformation = 12, // 12
         //FileDispositionInformation = 13, // 13
         //FilePositionInformation = 14, // 14
         //FileFullEaInformation = 15, // 15
         //FileModeInformation = 16, // 16
         //FileAlignmentInformation = 17, // 17
         //FileAllInformation = 18, // 18
         //FileAllocationInformation = 19, // 19
         //FileEndOfFileInformation = 20, // 20
         //FileAlternateNameInformation = 21, // 21
         //FileStreamInformation = 22, // 22
         //FilePipeInformation = 23, // 23
         //FilePipeLocalInformation = 24, // 24
         //FilePipeRemoteInformation = 25, // 25
         //FileMailslotQueryInformation = 26, // 26
         //FileMailslotSetInformation = 27, // 27
         //FileCompressionInformation = 28, // 28
         //FileObjectIdInformation = 29, // 29
         //FileCompletionInformation = 30, // 30
         //FileMoveClusterInformation = 31, // 31
         //FileQuotaInformation = 32, // 32
         //FileReparsePointInformation = 33, // 33
         //FileNetworkOpenInformation = 34, // 34
         //FileAttributeTagInformation = 35, // 35
         //FileTrackingInformation = 36, // 36
         //FileIdBothDirectoryInformation = 37, // 37
         //FileIdFullDirectoryInformation = 38, // 38
         //FileValidDataLengthInformation = 39, // 39
         //FileShortNameInformation = 40, // 40
         //FileHardLinkInformation = 46 // 46    
      }


      #region constants

      public const int INVALID_HANDLE_VALUE = -1;

      public const uint GENERIC_READ = 0x80000000;
      public const uint GENERIC_WRITE = 0x40000000;
      public const uint FILE_SHARE_READ = 0x00000001;
      public const uint FILE_SHARE_WRITE = 0x00000002;
      public const uint FILE_ATTRIBUTE_DIRECTORY = 0x00000010;

      //public const uint CREATE_NEW = 1;
      //public const uint CREATE_ALWAYS = 2;
      public const uint OPEN_EXISTING = 3;
      //public const uint OPEN_ALWAYS = 4;
      //public const uint TRUNCATE_EXISTING = 5;

      //public const uint FileNameInformationClass = 9;
      //public const uint FILE_ATTRIBUTE_NORMAL = 0x80;
      public const uint FILE_FLAG_BACKUP_SEMANTICS = 33554432;
      public const uint FILE_OPEN_FOR_BACKUP_INTENT = 16384;
      public const uint FILE_OPEN_BY_FILE_ID = 8192;
      public const uint FILE_OPEN = 1;

      public const uint OBJ_CASE_INSENSITIVE = 0x40;
      //public const OBJ_KERNEL_HANDLE = 0x200;

      // CTL_CODE( DeviceType, Function, Method, Access ) (((DeviceType) << 16) | ((Access) << 14) | ((Function) << 2) | (Method))
      private const uint FILE_DEVICE_FILE_SYSTEM = 0x00000009;

      private const uint METHOD_NEITHER = 3;
      private const uint METHOD_BUFFERED = 0;
      private const uint FILE_ANY_ACCESS = 0;
      //private const uint FILE_SPECIAL_ACCESS = 0;
      //private const uint FILE_READ_ACCESS = 1;
      //private const uint FILE_WRITE_ACCESS = 2;

      public const uint USN_REASON_DATA_OVERWRITE = 0x00000001;
      public const uint USN_REASON_DATA_EXTEND = 0x00000002;
      public const uint USN_REASON_DATA_TRUNCATION = 0x00000004;
      public const uint USN_REASON_NAMED_DATA_OVERWRITE = 0x00000010;
      public const uint USN_REASON_NAMED_DATA_EXTEND = 0x00000020;
      public const uint USN_REASON_NAMED_DATA_TRUNCATION = 0x00000040;
      public const uint USN_REASON_FILE_CREATE = 0x00000100;
      public const uint USN_REASON_FILE_DELETE = 0x00000200;
      public const uint USN_REASON_EA_CHANGE = 0x00000400;
      public const uint USN_REASON_SECURITY_CHANGE = 0x00000800;
      public const uint USN_REASON_RENAME_OLD_NAME = 0x00001000;
      public const uint USN_REASON_RENAME_NEW_NAME = 0x00002000;
      public const uint USN_REASON_INDEXABLE_CHANGE = 0x00004000;
      public const uint USN_REASON_BASIC_INFO_CHANGE = 0x00008000;
      public const uint USN_REASON_HARD_LINK_CHANGE = 0x00010000;
      public const uint USN_REASON_COMPRESSION_CHANGE = 0x00020000;
      public const uint USN_REASON_ENCRYPTION_CHANGE = 0x00040000;
      public const uint USN_REASON_OBJECT_ID_CHANGE = 0x00080000;
      public const uint USN_REASON_REPARSE_POINT_CHANGE = 0x00100000;
      public const uint USN_REASON_STREAM_CHANGE = 0x00200000;
      public const uint USN_REASON_CLOSE = 0x80000000;

      //public static int GWL_EXSTYLE = -20;
      //public static int WS_EX_LAYERED = 0x00080000;
      //public static int WS_EX_TRANSPARENT = 0x00000020;

      //public const uint FSCTL_GET_OBJECT_ID = 0x9009c;

      // FSCTL_ENUM_USN_DATA = CTL_CODE(FILE_DEVICE_FILE_SYSTEM, 44,  METHOD_NEITHER, FILE_ANY_ACCESS)
      public const uint FSCTL_ENUM_USN_DATA = (FILE_DEVICE_FILE_SYSTEM << 16) | (FILE_ANY_ACCESS << 14) | (44 << 2) | METHOD_NEITHER;

      // FSCTL_READ_USN_JOURNAL = CTL_CODE(FILE_DEVICE_FILE_SYSTEM, 46,  METHOD_NEITHER, FILE_ANY_ACCESS)
      public const uint FSCTL_READ_USN_JOURNAL = (FILE_DEVICE_FILE_SYSTEM << 16) | (FILE_ANY_ACCESS << 14) | (46 << 2) | METHOD_NEITHER;

      //  FSCTL_CREATE_USN_JOURNAL        CTL_CODE(FILE_DEVICE_FILE_SYSTEM, 57,  METHOD_NEITHER, FILE_ANY_ACCESS)
      public const uint FSCTL_CREATE_USN_JOURNAL = (FILE_DEVICE_FILE_SYSTEM << 16) | (FILE_ANY_ACCESS << 14) | (57 << 2) | METHOD_NEITHER;

      //  FSCTL_QUERY_USN_JOURNAL         CTL_CODE(FILE_DEVICE_FILE_SYSTEM, 61, METHOD_BUFFERED, FILE_ANY_ACCESS)
      public const uint FSCTL_QUERY_USN_JOURNAL = (FILE_DEVICE_FILE_SYSTEM << 16) | (FILE_ANY_ACCESS << 14) | (61 << 2) | METHOD_BUFFERED;

      // FSCTL_DELETE_USN_JOURNAL        CTL_CODE(FILE_DEVICE_FILE_SYSTEM, 62, METHOD_BUFFERED, FILE_ANY_ACCESS)
      public const uint FSCTL_DELETE_USN_JOURNAL = (FILE_DEVICE_FILE_SYSTEM << 16) | (FILE_ANY_ACCESS << 14) | (62 << 2) | METHOD_BUFFERED;

      #endregion

      #region dll imports

      /// <summary>
      /// Creates the file specified by 'lpFileName' with desired access, share mode, security attributes,
      /// creation disposition, flags and attributes.
      /// </summary>
      /// <param name="lpFileName">Fully qualified path to a file</param>
      /// <param name="dwDesiredAccess">Requested access (write, read, read/write, none)</param>
      /// <param name="dwShareMode">Share mode (read, write, read/write, delete, all, none)</param>
      /// <param name="lpSecurityAttributes">IntPtr to a 'SECURITY_ATTRIBUTES' structure</param>
      /// <param name="dwCreationDisposition">Action to take on file or device specified by 'lpFileName' (CREATE_NEW,
      /// CREATE_ALWAYS, OPEN_ALWAYS, OPEN_EXISTING, TRUNCATE_EXISTING)</param>
      /// <param name="dwFlagsAndAttributes">File or device attributes and flags (typically FILE_ATTRIBUTE_NORMAL)</param>
      /// <param name="hTemplateFile">IntPtr to a valid handle to a template file with 'GENERIC_READ' access right</param>
      /// <returns>IntPtr handle to the 'lpFileName' file or device or 'INVALID_HANDLE_VALUE'</returns>
      [DllImport("kernel32.dll", SetLastError = true)]
      public static extern IntPtr
         CreateFile(string lpFileName,
            uint dwDesiredAccess,
            uint dwShareMode,
            IntPtr lpSecurityAttributes,
            uint dwCreationDisposition,
            uint dwFlagsAndAttributes,
            IntPtr hTemplateFile);

      /// <summary>
      /// Closes the file specified by the IntPtr 'hObject'.
      /// </summary>
      /// <param name="hObject">IntPtr handle to a file</param>
      /// <returns>'true' if successful, otherwise 'false'</returns>
      [DllImport("kernel32.dll", SetLastError = true)]
      [return: MarshalAs(UnmanagedType.Bool)]
      public static extern bool
         CloseHandle(
            IntPtr hObject);

      /// <summary>
      /// Fills the 'BY_HANDLE_FILE_INFORMATION' structure for the file specified by 'hFile'.
      /// </summary>
      /// <param name="hFile">Fully qualified name of a file</param>
      /// <param name="lpFileInformation">Out BY_HANDLE_FILE_INFORMATION argument</param>
      /// <returns>'true' if successful, otherwise 'false'</returns>
      [DllImport("kernel32.dll", SetLastError = true)]
      [return: MarshalAs(UnmanagedType.Bool)]
      public static extern bool
         GetFileInformationByHandle(
            IntPtr hFile,
            out BY_HANDLE_FILE_INFORMATION lpFileInformation);

      /// <summary>
      /// Deletes the file specified by 'fileName'.
      /// </summary>
      /// <param name="fileName">Fully qualified path to the file to delete</param>
      /// <returns>'true' if successful, otherwise 'false'</returns>
      [DllImport("kernel32.dll", SetLastError = true)]
      [return: MarshalAs(UnmanagedType.Bool)]
      public static extern bool DeleteFile(
         string fileName);

      /// <summary>
      /// Read data from the file specified by 'hFile'.
      /// </summary>
      /// <param name="hFile">IntPtr handle to the file to read</param>
      /// <param name="lpBuffer">IntPtr to a buffer of bytes to receive the bytes read from 'hFile'</param>
      /// <param name="nNumberOfBytesToRead">Number of bytes to read from 'hFile'</param>
      /// <param name="lpNumberOfBytesRead">Number of bytes read from 'hFile'</param>
      /// <param name="lpOverlapped">IntPtr to an 'OVERLAPPED' structure</param>
      /// <returns>'true' if successful, otherwise 'false'</returns>
      [DllImport("kernel32.dll")]
      [return: MarshalAs(UnmanagedType.Bool)]
      public static extern bool ReadFile(
         IntPtr hFile,
         IntPtr lpBuffer,
         uint nNumberOfBytesToRead,
         out uint lpNumberOfBytesRead,
         IntPtr lpOverlapped);

      /// <summary>
      /// Writes the 
      /// </summary>
      /// <param name="hFile">IntPtr handle to the file to write</param>
      /// <param name="bytes">IntPtr to a buffer of bytes to write to 'hFile'</param>
      /// <param name="nNumberOfBytesToWrite">Number of bytes in 'lpBuffer' to write to 'hFile'</param>
      /// <param name="lpNumberOfBytesWritten">Number of bytes written to 'hFile'</param>
      /// <param name="overlapped">IntPtr to an 'OVERLAPPED' structure</param>
      /// <returns>'true' if successful, otherwise 'false'</returns>
      [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
      [return: MarshalAs(UnmanagedType.Bool)]
      public static extern bool WriteFile(
         IntPtr hFile,
         IntPtr bytes,
         uint nNumberOfBytesToWrite,
         out uint lpNumberOfBytesWritten,
         int overlapped);

      /// <summary>
      /// Writes the data in 'lpBuffer' to the file specified by 'hFile'.
      /// </summary>
      /// <param name="hFile">IntPtr handle to file to write</param>
      /// <param name="lpBuffer">Buffer of bytes to write to file 'hFile'</param>
      /// <param name="nNumberOfBytesToWrite">Number of bytes in 'lpBuffer' to write to 'hFile'</param>
      /// <param name="lpNumberOfBytesWritten">Number of bytes written to 'hFile'</param>
      /// <param name="overlapped">IntPtr to an 'OVERLAPPED' structure</param>
      /// <returns>'true' if successful, otherwise 'false'</returns>
      [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
      [return: MarshalAs(UnmanagedType.Bool)]
      public static extern bool WriteFile(
         IntPtr hFile,
         byte[] lpBuffer,
         uint nNumberOfBytesToWrite,
         out uint lpNumberOfBytesWritten,
         int overlapped);

      /// <summary>
      /// Sends the 'dwIoControlCode' to the device specified by 'hDevice'.
      /// </summary>
      /// <param name="hDevice">IntPtr handle to the device to receive 'dwIoControlCode'</param>
      /// <param name="dwIoControlCode">Device IO Control Code to send</param>
      /// <param name="lpInBuffer">Input buffer if required</param>
      /// <param name="nInBufferSize">Size of input buffer</param>
      /// <param name="lpOutBuffer">Output buffer if required</param>
      /// <param name="nOutBufferSize">Size of output buffer</param>
      /// <param name="lpBytesReturned">Number of bytes returned in output buffer</param>
      /// <param name="lpOverlapped">IntPtr to an 'OVERLAPPED' structure</param>
      /// <returns>'true' if successful, otherwise 'false'</returns>
      [DllImport("kernel32.dll", ExactSpelling = true, SetLastError = true, CharSet = CharSet.Auto)]
      [return: MarshalAs(UnmanagedType.Bool)]
      public static extern bool DeviceIoControl(
         IntPtr hDevice,
         uint dwIoControlCode,
         IntPtr lpInBuffer,
         int nInBufferSize,
         out USN_JOURNAL_DATA_V0 lpOutBuffer,
         int nOutBufferSize,
         out uint lpBytesReturned,
         IntPtr lpOverlapped);

      /// <summary>
      /// Sends the control code 'dwIoControlCode' to the device driver specified by 'hDevice'.
      /// </summary>
      /// <param name="hDevice">IntPtr handle to the device to receive 'dwIoControlCode</param>
      /// <param name="dwIoControlCode">Device IO Control Code to send</param>
      /// <param name="lpInBuffer">Input buffer if required</param>
      /// <param name="nInBufferSize">Size of input buffer </param>
      /// <param name="lpOutBuffer">Output buffer if required</param>
      /// <param name="nOutBufferSize">Size of output buffer</param>
      /// <param name="lpBytesReturned">Number of bytes returned</param>
      /// <param name="lpOverlapped">Pointer to an 'OVERLAPPED' struture</param>
      /// <returns></returns>
      [DllImport("kernel32.dll", ExactSpelling = true, SetLastError = true, CharSet = CharSet.Auto)]
      [return: MarshalAs(UnmanagedType.Bool)]
      public static extern bool DeviceIoControl(
         IntPtr hDevice,
         uint dwIoControlCode,
         IntPtr lpInBuffer,
         int nInBufferSize,
         IntPtr lpOutBuffer,
         int nOutBufferSize,
         out uint lpBytesReturned,
         IntPtr lpOverlapped);


      /// <summary>Sets the number of bytes specified by 'size' of the memory associated with the argument 'ptr' to zero.</summary>
      [DllImport("kernel32.dll", SetLastError = false, CharSet = CharSet.Unicode)]
      public static extern void ZeroMemory(IntPtr ptr, int size);


      /// <summary>
      /// Creates a new file or directory, or opens an existing file, device, directory, or volume
      /// </summary>
      /// <param name="handle">A pointer to a variable that receives the file handle if the call is successful (out)</param>
      /// <param name="access">ACCESS_MASK value that expresses the type of access that the caller requires to the file or directory (in)</param>
      /// <param name="objectAttributes">A pointer to a structure already initialized with InitializeObjectAttributes (in)</param>
      /// <param name="ioStatus">A pointer to a variable that receives the final completion status and information about the requested operation (out)</param>
      /// <param name="allocSize">The initial allocation size in bytes for the file (in)(optional)</param>
      /// <param name="fileAttributes">file attributes (in)</param>
      /// <param name="share">type of share access that the caller would like to use in the file (in)</param>
      /// <param name="createDisposition">what to do, depending on whether the file already exists (in)</param>
      /// <param name="createOptions">options to be applied when creating or opening the file (in)</param>
      /// <param name="eaBuffer">Pointer to an EA buffer used to pass extended attributes (in)</param>
      /// <param name="eaLength">Length of the EA buffer</param>
      /// <returns>either STATUS_SUCCESS or an appropriate error status. If it returns an error status, the caller can find more information about the cause of the failure by checking the IoStatusBlock</returns>
      [DllImport("ntdll.dll", ExactSpelling = true, SetLastError = true)]
      public static extern int NtCreateFile(ref IntPtr handle, FileAccess access,
         ref OBJECT_ATTRIBUTES objectAttributes, ref IO_STATUS_BLOCK ioStatus, ref long allocSize, uint fileAttributes,
         FileShare share, uint createDisposition, uint createOptions, IntPtr eaBuffer, uint eaLength);


      /// <summary>
      /// 
      /// </summary>
      /// <param name="fileHandle"></param>
      /// <param name="IoStatusBlock"></param>
      /// <param name="pInfoBlock"></param>
      /// <param name="length"></param>
      /// <param name="fileInformation"></param>
      /// <returns></returns>
      [DllImport("ntdll.dll", ExactSpelling = true, SetLastError = true)]
      public static extern int NtQueryInformationFile(
         IntPtr fileHandle,
         ref IO_STATUS_BLOCK IoStatusBlock,
         IntPtr pInfoBlock,
         uint length,
         FILE_INFORMATION_CLASS fileInformation);

      #endregion

      #region structures

      /// <summary>
      /// By Handle File Information structure, contains File Attributes (32bits), Creation Time(FILETIME),
      /// Last Access Time(FILETIME), Last Write Time(FILETIME), Volume Serial Number (32bits),
      /// File Size High (32bits), File Size Low (32bits), Number of Links (32bits), File Index High (32bits),
      /// File Index Low (32bits).
      /// </summary>
      [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
      public struct BY_HANDLE_FILE_INFORMATION
      {
         public uint FileAttributes;
         public FILETIME CreationTime;
         public FILETIME LastAccessTime;
         public FILETIME LastWriteTime;
         public uint VolumeSerialNumber;
         public uint FileSizeHigh;
         public uint FileSizeLow;
         public uint NumberOfLinks;
         public uint FileIndexHigh;
         public uint FileIndexLow;
      }


      /// <summary>Represents an update sequence number (USN) change journal, its records, and its capacity. This structure is the output buffer for the FSCTL_QUERY_USN_JOURNAL control code.</summary>
      /// <remarks>Prior to Windows 8 and Windows Server 2012 this structure was named USN_JOURNAL_DATA.</remarks>
      [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
      public struct USN_JOURNAL_DATA_V0
      {
         /// <summary>The current journal identifier. A journal is assigned a new identifier on creation and can be stamped with a new identifier in the course of its existence.
         /// The NTFS file system uses this identifier for an integrity check.</summary>
         [MarshalAs(UnmanagedType.U8)] public ulong UsnJournalID;


         /// <summary>The number of first record that can be read from the journal.</summary>
         [MarshalAs(UnmanagedType.I8)] public long FirstUsn;


         /// <summary>The number of next record to be written to the journal.</summary>
         [MarshalAs(UnmanagedType.I8)] public long NextUsn;


         /// <summary>The first record that was written into the journal for this journal instance.</summary>
         /// <remarks>Enumerating the files or directories on a volume can return a USN lower than this value (in other words, a FirstUsn member value less than the LowestValidUsn member value).
         /// If it does, the journal has been stamped with a new identifier since the last USN was written. In this case, LowestValidUsn may indicate a discontinuity in the journal,
         /// in which changes to some or all files or directories on the volume may have occurred that are not recorded in the change journal.</remarks>
         [MarshalAs(UnmanagedType.I8)] public long LowestValidUsn;


         /// <summary>The largest USN that the change journal supports. An administrator must delete the change journal as the value of NextUsn approaches this value.</summary>
         [MarshalAs(UnmanagedType.I8)] public long MaxUsn;


         /// <summary>The target maximum size for the change journal, in bytes.
         /// The change journal can grow larger than this value, but it is then truncated at the next NTFS file system checkpoint to less than this value.</summary>
         [MarshalAs(UnmanagedType.U8)] public ulong MaximumSize;


         /// <summary>The number of bytes of disk memory added to the end and removed from the beginning of the change journal each time memory is allocated or deallocated.
         /// In other words, allocation and deallocation take place in units of this size. An integer multiple of a cluster size is a reasonable value for this member.</summary>
         [MarshalAs(UnmanagedType.U8)] public ulong AllocationDelta;
      }


      /// <summary>Contains information defining the boundaries for and starting place of an enumeration of update sequence number (USN) change journal records.
      /// It is used as the input buffer for the FSCTL_ENUM_USN_DATA control code.</summary>
      /// <remarks>Prior to Windows Server 2012 this structure was named MFT_ENUM_DATA.</remarks>
      [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
      public struct MFT_ENUM_DATA_V0
      {
         /// <summary>The ordinal position within the files on the current volume at which the enumeration is to begin.</summary>
         /// <remarks>The first call to FSCTL_ENUM_USN_DATA during an enumeration must have the StartFileReferenceNumber member set to (DWORDLONG)0.
         /// Each call to FSCTL_ENUM_USN_DATA retrieves the starting point for the subsequent call as the first entry in the output buffer.
         /// Subsequent calls must be made with StartFileReferenceNumber set to this value. For more information, see FSCTL_ENUM_USN_DATA.</remarks>
         [MarshalAs(UnmanagedType.U8)] public ulong StartFileReferenceNumber;


         /// <summary>The lower boundary of the range of USN values used to filter which records are returned.
         /// Only records whose last change journal USN is between or equal to the LowUsn and HighUsn member values are returned.</summary>
         [MarshalAs(UnmanagedType.I8)] public long LowUsn;


         /// <summary>The upper boundary of the range of USN values used to filter which files are returned.</summary>
         [MarshalAs(UnmanagedType.I8)] public long HighUsn;
      }


      /// <summary>Contains information that describes an update sequence number (USN) change journal.</summary>
      [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
      public struct CREATE_USN_JOURNAL_DATA
      {
         /// <summary>The target maximum size that the NTFS file system allocates for the change journal, in bytes.</summary>
         /// <remarks>The change journal can grow larger than this value, but it is then truncated at the next NTFS file system checkpoint to less than this value.</remarks>
         [MarshalAs(UnmanagedType.U8)] public ulong MaximumSize;


         /// <summary>The size of memory allocation that is added to the end and removed from the beginning of the change journal, in bytes.</summary>
         /// <remarks>The change journal can grow to more than the sum of the values of <see cref="MaximumSize"/> and <see cref="AllocationDelta"/> before being trimmed.</remarks>
         [MarshalAs(UnmanagedType.U8)] public ulong AllocationDelta;
      }


      /// <summary>Contains information on the deletion of an update sequence number (USN) change journal using the FSCTL_DELETE_USN_JOURNAL control code.</summary>
      [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
      public struct DELETE_USN_JOURNAL_DATA
      {
         /// <summary>The identifier of the change journal to be deleted.</summary>
         /// <remarks> If the journal is active and deletion is requested by setting the USN_DELETE_FLAG_DELETE flag in the DeleteFlags member, then this identifier must specify the change journal for the current volume.
         /// Use FSCTL_QUERY_USN_JOURNAL to retrieve the identifier of this change journal. If in this case the identifier is not for the current volume's change journal, FSCTL_DELETE_USN_JOURNAL fails.
         /// If notification instead of deletion is requested by setting only the USN_DELETE_FLAG_NOTIFY flag in DeleteFlags, UsnJournalID is ignored.</remarks>
         [MarshalAs(UnmanagedType.U8)] public ulong UsnJournalID;


         /// <summary>Indicates whether deletion or notification regarding deletion is performed, or both.</summary>
         [MarshalAs(UnmanagedType.U4)] public uint DeleteFlags;
      }


      /// <summary>Contains information defining a set of update sequence number (USN) change journal records to return to the calling process.
      /// It is used by the FSCTL_QUERY_USN_JOURNAL and FSCTL_READ_USN_JOURNAL control codes.</summary>
      /// <remarks>Prior to Windows 8 and Windows Server 2012 this structure was named READ_USN_JOURNAL_DATA.</remarks>
      [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
      public struct READ_USN_JOURNAL_DATA_V0
      {
         /// <summary>The USN at which to begin reading the change journal.</summary>
         [MarshalAs(UnmanagedType.U8)] public ulong StartUsn;


         /// <summary>A mask of flags, each flag noting a change for which the file or directory has a record in the change journal.</summary>
         [MarshalAs(UnmanagedType.U4)] public uint ReasonMask;


         /// <summary>A value that specifies when to return change journal records.</summary>
         [MarshalAs(UnmanagedType.U4)] public uint ReturnOnlyOnClose;


         /// <summary>The time-out value, in seconds, used with the <see cref="BytesToWaitFor"/> member to tell the operating system what to do
         /// if the FSCTL_READ_USN_JOURNAL operation requests more data than exists in the change journal.</summary>
         [MarshalAs(UnmanagedType.U8)] public ulong Timeout;


         /// <summary>The number of bytes of unfiltered data added to the change journal. Use this value with <see cref="Timeout"/> to tell the operating system what to do
         /// if the FSCTL_READ_USN_JOURNAL operation requests more data than exists in the change journal.</summary>
         [MarshalAs(UnmanagedType.U8)] public ulong BytesToWaitFor;


         /// <summary>The identifier for the instance of the journal that is current for the volume.</summary>
         [MarshalAs(UnmanagedType.U8)] public ulong UsnJournalId;
      }


      /// <summary>A driver sets an IRP's I/O status block to indicate the final status of an I/O request, before calling IoCompleteRequest for the IRP.</summary>
      [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
      public struct IO_STATUS_BLOCK
      {
         /// <summary>This is the completion status, either STATUS_SUCCESS if the requested operation was completed successfully or an informational, warning, or error STATUS_XXX value.</summary>
         [MarshalAs(UnmanagedType.U4)] public uint Status;


         /// <summary>This is set to a request-dependent value. For example, on successful completion of a transfer request, this is set to the number of bytes transferred.
         /// If a transfer request is completed with another STATUS_XXX, this member is set to zero.</summary>
         [MarshalAs(UnmanagedType.U8)] public ulong Information;
      }


      /// <summary>The OBJECT_ATTRIBUTES structure specifies attributes that can be applied to objects or object handles by routines that create objects and/or return handles to objects.</summary>
      [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
      public struct OBJECT_ATTRIBUTES
      {
         /// <summary>The number of bytes of data contained in this structure. The InitializeObjectAttributes macro sets this member to sizeof(OBJECT_ATTRIBUTES).</summary>
         [MarshalAs(UnmanagedType.U8)] public ulong Length;


         /// <summary>Optional handle to the root object directory for the path name specified by the ObjectName member.</summary>
         public IntPtr RootDirectory;


         /// <summary>Pointer to a Unicode string that contains the name of the object for which a handle is to be opened.</summary>
         public IntPtr ObjectName;


         /// <summary>Bitmask of flags that specify object handle attributes.</summary>
         [MarshalAs(UnmanagedType.U8)] public ulong Attributes;


         /// <summary>Specifies a security descriptor (SECURITY_DESCRIPTOR) for the object when the object is created. If this member is NULL, the object will receive default security settings.</summary>
         public IntPtr SecurityDescriptor;


         /// <summary>Optional quality of service to be applied to the object when it is created. Used to indicate the security impersonation level and context tracking mode (dynamic or static).</summary>
         public IntPtr SecurityQualityOfService;
      }


      /// <summary>The UNICODE_STRING structure is used to define Unicode strings.</summary>
      [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
      public struct UNICODE_STRING
      {
         /// <summary>The length, in bytes, of the string stored in <see cref="Buffer"/>.</summary>
         public ushort Length;


         /// <summary>The length, in bytes, of <see cref="Buffer"/>.</summary>
         public ushort MaximumLength;


         /// <summary>Pointer to a buffer used to contain a string of wide characters.</summary>
         public IntPtr Buffer;
      }



      /// <summary>The 32bit File Name Length, the 32bit File Name Offset and the File Name.</summary>
      public class UsnEntry : IComparable<UsnEntry>
      {
         private const int FR_OFFSET = 8;
         private const int PFR_OFFSET = 16;
         private const int USN_OFFSET = 24;
         private const int REASON_OFFSET = 40;
         public const int FA_OFFSET = 52;
         private const int FNL_OFFSET = 56;
         private const int FN_OFFSET = 58;

         
         private readonly uint _recordLength;
         /// <summary>The 32bit USN Record Length.</summary>
         public uint RecordLength
         {
            get { return _recordLength; }
         }


         private readonly long _usn;
         /// <summary>The 64bit USN.</summary>
         public long USN
         {
            get { return _usn; }
         }


         private readonly ulong _frn;
         /// <summary>The 64bit File Reference Number.</summary>
         public ulong FileReferenceNumber
         {
            get { return _frn; }
         }


         private readonly ulong _pfrn;
         /// <summary>The 64bit Parent File Reference Number.</summary>
         public ulong ParentFileReferenceNumber
         {
            get { return _pfrn; }
         }


         private readonly uint _reason;
         /// <summary>The 32bit Reason Code.</summary>
         public uint Reason
         {
            get { return _reason; }
         }


         private readonly string _name;
         /// <summary>The 32bit Reason Code.</summary>
         public string Name
         {
            get { return _name; }
         }


         private string _oldName;
         public string OldName
         {
            get { return 0 != (_fileAttributes & USN_REASON_RENAME_OLD_NAME) ? _oldName : null; }
            set { _oldName = value; }
         }


         /// <summary>The 32bit File Attributes.</summary>
         private readonly uint _fileAttributes;


         public bool IsFolder
         {
            get { return (_fileAttributes & FILE_ATTRIBUTE_DIRECTORY) != 0; }
         }


         /// <summary>USN Record Constructor.</summary>
         /// <param name="ptrToUsnRecord">Buffer pointer to first byte of the USN Record</param>
         public UsnEntry(IntPtr ptrToUsnRecord)
         {
            _recordLength = (uint) Marshal.ReadInt32(ptrToUsnRecord);

            _frn = (ulong) Marshal.ReadInt64(ptrToUsnRecord, FR_OFFSET);
            _pfrn = (ulong) Marshal.ReadInt64(ptrToUsnRecord, PFR_OFFSET);
            _usn = Marshal.ReadInt64(ptrToUsnRecord, USN_OFFSET);

            _reason = (uint) Marshal.ReadInt32(ptrToUsnRecord, REASON_OFFSET);

            _fileAttributes = (uint) Marshal.ReadInt32(ptrToUsnRecord, FA_OFFSET);

            var fileNameLength = Marshal.ReadInt16(ptrToUsnRecord, FNL_OFFSET);
            var fileNameOffset = Marshal.ReadInt16(ptrToUsnRecord, FN_OFFSET);

            _name = Marshal.PtrToStringUni(new IntPtr(ptrToUsnRecord.ToInt64() + fileNameOffset), fileNameLength / sizeof(char));
         }


         #region IComparable<UsnEntry> Members

         public int CompareTo(UsnEntry other)
         {
            return string.Compare(Name, other.Name, StringComparison.OrdinalIgnoreCase);
         }

         #endregion
      }


      ///// <summary>
      ///// Contains the Start USN (64bits), Reason Mask (32bits), Return Only on Close flag (32bits),
      ///// Time Out (64bits), Bytes To Wait For (64bits), and USN journal ID (64bits).
      ///// </summary>
      ///// <remarks> possible reason bits are from Win32Api
      ///// USN_REASON_DATA_OVERWRITE
      ///// USN_REASON_DATA_EXTEND
      ///// USN_REASON_DATA_TRUNCATION
      ///// USN_REASON_NAMED_DATA_OVERWRITE
      ///// USN_REASON_NAMED_DATA_EXTEND
      ///// USN_REASON_NAMED_DATA_TRUNCATION
      ///// USN_REASON_FILE_CREATE
      ///// USN_REASON_FILE_DELETE
      ///// USN_REASON_EA_CHANGE
      ///// USN_REASON_SECURITY_CHANGE
      ///// USN_REASON_RENAME_OLD_NAME
      ///// USN_REASON_RENAME_NEW_NAME
      ///// USN_REASON_INDEXABLE_CHANGE
      ///// USN_REASON_BASIC_INFO_CHANGE
      ///// USN_REASON_HARD_LINK_CHANGE
      ///// USN_REASON_COMPRESSION_CHANGE
      ///// USN_REASON_ENCRYPTION_CHANGE
      ///// USN_REASON_OBJECT_ID_CHANGE
      ///// USN_REASON_REPARSE_POINT_CHANGE
      ///// USN_REASON_STREAM_CHANGE
      ///// USN_REASON_CLOSE
      ///// </remarks>
      
      #endregion
   }
}
