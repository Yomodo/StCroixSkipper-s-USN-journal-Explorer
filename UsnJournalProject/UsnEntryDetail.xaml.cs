using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Input;
using PInvoke;
using UsnJournal;

namespace UsnJournalProject
{
   /// <summary>Interaction logic for UsnEntryDetail.xaml</summary>
   public partial class UsnEntryDetail : Window
   {
      public enum EntryDetail
      {
         Folder = 0,
         File = 1,
         UsnEntry = 2
      }


      public UsnEntryDetail(Window owner)
      {
         InitializeComponent();
         Owner = owner;
         WindowStyle = WindowStyle.None;
         Visibility = Visibility.Hidden;
      }


      public void ChangeDisplay(NtfsUsnJournal usnJournal, double top, double left, Win32Api.UsnEntry usnEntry, EntryDetail entryDetail)
      {
         Top = top;
         Left = left;

         _nameLbl.Text = string.Format(CultureInfo.CurrentCulture, usnEntry.IsFolder ? "Directory: {0}" : "File: {0}", usnEntry.Name);

         string path;

         var lastError = usnJournal.GetPathFromFileReference(usnEntry.ParentFileReferenceNumber, out path);
         if (lastError == (int) NtfsUsnJournal.UsnJournalReturnCode.USN_JOURNAL_SUCCESS && !path.Equals("Unavailable", StringComparison.OrdinalIgnoreCase))
            path = string.Format(CultureInfo.CurrentCulture, "{0}{1}\\", usnJournal.VolumeName.TrimEnd('\\'), path);

         _pathLbl.Text = path;


         var sb = new StringBuilder();
         sb.AppendFormat("  File Ref No: {0}", usnEntry.FileReferenceNumber);
         sb.AppendFormat("\n  Parent FRN   {0}", usnEntry.ParentFileReferenceNumber);

         if (entryDetail == EntryDetail.UsnEntry)
         {
            sb.AppendFormat("\n  Length:  {0}", usnEntry.RecordLength);
            sb.AppendFormat("\n  USN:     {0}", usnEntry.USN);
            AddReasonData(sb, usnEntry);
         }

         if (!usnEntry.IsFolder)
         {
            var fullPath = Path.Combine(_pathLbl.Text, usnEntry.Name);
            if (File.Exists(fullPath))
            {
               var fi = new FileInfo(fullPath);
               sb.AppendFormat("\n  File Length:   {0} (bytes)", fi.Length);
               sb.AppendFormat("\n  Creation Time: {0} - {1}", fi.CreationTime.ToShortDateString(), fi.CreationTime.ToShortTimeString());
               sb.AppendFormat("\n  Last Modify:   {0} - {1}", fi.LastWriteTime.ToShortDateString(), fi.LastWriteTime.ToShortTimeString());
               sb.AppendFormat("\n  Last Access:   {0} - {1}", fi.LastAccessTime.ToShortDateString(), fi.LastAccessTime.ToShortTimeString());
            }
         }

         _entryDetailLbl.Content = sb.ToString();
         Visibility = Visibility.Visible;
      }


      private static void AddReasonData(StringBuilder sb, Win32Api.UsnEntry usnEntry)
      {
         sb.AppendFormat("\n  Reason Codes:");
         var value = usnEntry.Reason & Win32Api.USN_REASON_DATA_OVERWRITE;
         if (0 != value)
            sb.AppendFormat("\n     -DATA OVERWRITE");
         
         value = usnEntry.Reason & Win32Api.USN_REASON_DATA_EXTEND;
         if (0 != value)
            sb.AppendFormat("\n     -DATA EXTEND");
         
         value = usnEntry.Reason & Win32Api.USN_REASON_DATA_TRUNCATION;
         if (0 != value)
            sb.AppendFormat("\n     -DATA TRUNCATION");
         
         value = usnEntry.Reason & Win32Api.USN_REASON_NAMED_DATA_OVERWRITE;
         if (0 != value)
            sb.AppendFormat("\n     -NAMED DATA OVERWRITE");
         
         value = usnEntry.Reason & Win32Api.USN_REASON_NAMED_DATA_EXTEND;
         if (0 != value)
            sb.AppendFormat("\n     -NAMED DATA EXTEND");
         
         value = usnEntry.Reason & Win32Api.USN_REASON_NAMED_DATA_TRUNCATION;
         if (0 != value)
            sb.AppendFormat("\n     -NAMED DATA TRUNCATION");
         
         value = usnEntry.Reason & Win32Api.USN_REASON_FILE_CREATE;
         if (0 != value)
            sb.AppendFormat("\n     -FILE CREATE");
         
         value = usnEntry.Reason & Win32Api.USN_REASON_FILE_DELETE;
         if (0 != value)
            sb.AppendFormat("\n     -FILE DELETE");
         
         value = usnEntry.Reason & Win32Api.USN_REASON_EA_CHANGE;
         if (0 != value)
            sb.AppendFormat("\n     -EA CHANGE");
         
         value = usnEntry.Reason & Win32Api.USN_REASON_SECURITY_CHANGE;
         if (0 != value)
            sb.AppendFormat("\n     -SECURITY CHANGE");
         
         value = usnEntry.Reason & Win32Api.USN_REASON_RENAME_OLD_NAME;
         if (0 != value)
            sb.AppendFormat("\n     -RENAME OLD NAME");
         
         value = usnEntry.Reason & Win32Api.USN_REASON_RENAME_NEW_NAME;
         if (0 != value)
            sb.AppendFormat("\n     -RENAME NEW NAME");
         
         value = usnEntry.Reason & Win32Api.USN_REASON_INDEXABLE_CHANGE;
         if (0 != value)
            sb.AppendFormat("\n     -INDEXABLE CHANGE");
         
         value = usnEntry.Reason & Win32Api.USN_REASON_BASIC_INFO_CHANGE;
         if (0 != value)
            sb.AppendFormat("\n     -BASIC INFO CHANGE");
         
         value = usnEntry.Reason & Win32Api.USN_REASON_HARD_LINK_CHANGE;
         if (0 != value)
            sb.AppendFormat("\n     -HARD LINK CHANGE");
         
         value = usnEntry.Reason & Win32Api.USN_REASON_COMPRESSION_CHANGE;
         if (0 != value)
            sb.AppendFormat("\n     -COMPRESSION CHANGE");
         
         value = usnEntry.Reason & Win32Api.USN_REASON_ENCRYPTION_CHANGE;
         if (0 != value)
            sb.AppendFormat("\n     -ENCRYPTION CHANGE");
         
         value = usnEntry.Reason & Win32Api.USN_REASON_OBJECT_ID_CHANGE;
         if (0 != value)
            sb.AppendFormat("\n     -OBJECT ID CHANGE");
         
         value = usnEntry.Reason & Win32Api.USN_REASON_REPARSE_POINT_CHANGE;
         if (0 != value)
            sb.AppendFormat("\n     -REPARSE POINT CHANGE");
         
         value = usnEntry.Reason & Win32Api.USN_REASON_STREAM_CHANGE;
         if (0 != value)
            sb.AppendFormat("\n     -STREAM CHANGE");
         
         value = usnEntry.Reason & Win32Api.USN_REASON_CLOSE;
         if (0 != value)
            sb.AppendFormat("\n     -CLOSE");
      }


      private void _nameLbl_MouseDoubleClick(object sender, MouseButtonEventArgs e)
      {
         e.Handled = true;

         var name = _nameLbl.Text;
         var path = _pathLbl.Text;

         if (name.StartsWith("Directory: ", StringComparison.OrdinalIgnoreCase))
         {
            name = name.Replace("Directory: ", string.Empty).Trim();
            MessageBox.Show(string.Format(CultureInfo.InvariantCulture, "Entry is a directory: {0}", name));
         }

         else if (name.StartsWith("File: ", StringComparison.OrdinalIgnoreCase))
         {
            name = name.Replace("File: ", string.Empty).Trim();
            path = path.Replace("  Path: ", string.Empty).Trim();

            if (!path.Contains("Unavailable"))
            {
               var fullPath = Path.Combine(path, name);
               if (File.Exists(fullPath))
               {
                  try
                  {
                     using (Process.Start(fullPath)) {}
                  }
                  catch (Exception ex)
                  {
                     MessageBox.Show(ex.Message);
                  }
               }

               else
                  MessageBox.Show(string.Format(CultureInfo.InvariantCulture, "File: '{0}' not found", fullPath));
            }

            else
               MessageBox.Show(string.Format(CultureInfo.InvariantCulture, "File '{0}' path unavailable", name));
         }
      }
   }
}
