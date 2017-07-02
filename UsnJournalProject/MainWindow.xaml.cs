using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using PInvoke;
using UsnJournal;

namespace UsnJournalProject
{
   /// <summary>Interaction logic for MainWindow.xaml</summary>
   public partial class MainWindow : Window
   {
      public NtfsUsnJournal Journal { get; private set; }


      private Win32Api.USN_JOURNAL_DATA_V0 _usnCurrentJournalState;

      private UsnEntryDetail _usnEntryDetail;
      private double _lbItemX;
      private double _lbItemY;

      private UsnEntryDetail.EntryDetail _entryDetail;


      private delegate void FillListBoxDelegate(NtfsUsnJournal.UsnJournalReturnCode rtnCode, List<Win32Api.UsnEntry> usnEntries, Win32Api.USN_JOURNAL_DATA_V0 newUsnState);
      private delegate void FillListBoxWithFilesDelagate(NtfsUsnJournal.UsnJournalReturnCode rtnCode, List<Win32Api.UsnEntry> fileList);
      private delegate void FillListBoxWithFoldersDelegate(NtfsUsnJournal.UsnJournalReturnCode rtnCode, List<Win32Api.UsnEntry> folders);


      public MainWindow()
      {
         InitializeComponent();

         _usnCurrentJournalState = new Win32Api.USN_JOURNAL_DATA_V0();
      }


      private void SelectVolume_Click(object sender, RoutedEventArgs e)
      {
         e.Handled = true;

         _usnEntryDetail.Visibility = Visibility.Hidden;
         resultsLb.ItemsSource = null;
         resultsLb.Items.Clear();

         var selectVolumeDlg = new VolumeSelectDialog(this);

         var rtn = selectVolumeDlg.ShowDialog();
         if (null != rtn && rtn.Value)
         {
            var driveInfo = selectVolumeDlg.Volume;
            try
            {
               Journal = new NtfsUsnJournal(driveInfo);
               FunctionElapsedTime.Content = string.Format(CultureInfo.InvariantCulture, "{0} duration: {1} (ms) - Volume: {2}", "NtfsUsnJournal constructor", NtfsUsnJournal.ElapsedTime.TotalMilliseconds, driveInfo.Name);
               QueryUsnJournal.IsEnabled = true;
               CreateUsnJournal.IsEnabled = true;
               DeleteUsnJournal.IsEnabled = true;
               SaveUsnState.IsEnabled = true;
               ViewUsnChanges.IsEnabled = true;
               ListFiles.IsEnabled = true;
               ListFolders.IsEnabled = true;
            }
            catch (Exception ex)
            {
               if (ex.Message.Contains("Access is denied"))
               {
                  var lbItem = new ListBoxItem
                  {
                     Content = "\'Access Denied\' exception caught attempting to select volume.  \nYou need \'Admin\' rights to run this application.",
                     Foreground = Brushes.Red
                  };

                  resultsLb.Items.Add(lbItem);
               }

               else
               {
                  var lbItem = new ListBoxItem
                  {
                     Content = string.Format(CultureInfo.InvariantCulture, "{0} exception caught attempting to select volume. \n{1}", ex.GetType(), ex.Message),
                     Foreground = Brushes.Red
                  };

                  resultsLb.Items.Add(lbItem);
               }
            }
         }

         else
         {
            var lbItem = new ListBoxItem
            {
               Content = "Select Volume -- No Volume Selected",
               Foreground = Brushes.Red
            };

            resultsLb.Items.Add(lbItem);
         }
      }


      private void QueryUsnJournal_Click(object sender, RoutedEventArgs e)
      {
         e.Handled = true;

         _usnEntryDetail.Visibility = Visibility.Hidden;
         resultsLb.ItemsSource = null;
         resultsLb.Items.Clear();

         var journalState = new Win32Api.USN_JOURNAL_DATA_V0();
         var rtn = Journal.GetUsnJournalState(ref journalState);

         FunctionElapsedTime.Content = string.Format(CultureInfo.InvariantCulture, "Query->{0} duration: {1} (ms)", "GetUsnJournalState", NtfsUsnJournal.ElapsedTime.TotalMilliseconds);

         if (rtn == (int) NtfsUsnJournal.UsnJournalReturnCode.USN_JOURNAL_SUCCESS)
         {
            var lbItem = new ListBoxItem
            {
               Foreground = Brushes.Black,
               Content = FormatUsnJournalState(journalState)
            };

            resultsLb.Items.Add(lbItem);
         }

         else
         {
            var lbItem = new ListBoxItem
            {
               Content = string.Format(CultureInfo.InvariantCulture, "Query->{0} returned error code: {1}", "GetUsnJournalState", rtn),
               Foreground = Brushes.Red
            };

            resultsLb.Items.Add(lbItem);
         }
      }


      private void CreateUsnJournal_Click(object sender, RoutedEventArgs e)
      {
         e.Handled = true;

         _usnEntryDetail.Visibility = Visibility.Hidden;
         resultsLb.ItemsSource = null;
         resultsLb.Items.Clear();

         var rtn = Journal.CreateUsnJournal(1000 * 1024, 16 * 1024);

         FunctionElapsedTime.Content = string.Format(CultureInfo.InvariantCulture, "Create->{0} duration: {1} (ms)", "CreateUsnJournal", NtfsUsnJournal.ElapsedTime.TotalMilliseconds);

         if (rtn == (int) NtfsUsnJournal.UsnJournalReturnCode.USN_JOURNAL_SUCCESS)
         {
            var lbItem = new ListBoxItem
            {
               Content = string.Format(CultureInfo.InvariantCulture, "USN journal Successfully created, CreateUsnJournal returned: {0}", rtn),
               Foreground = Brushes.Black
            };
            resultsLb.Items.Add(lbItem);
         }

         else
         {
            var lbItem = new ListBoxItem
            {
               Content = string.Format(CultureInfo.InvariantCulture, "Create->{0} returned error code: {1}", "GetUsnJournalState", rtn),
               Foreground = Brushes.Red
            };

            resultsLb.Items.Add(lbItem);
         }
      }


      private void DeleteUsnJournal_Click(object sender, RoutedEventArgs e)
      {
         e.Handled = true;

         _usnEntryDetail.Visibility = Visibility.Hidden;
         resultsLb.ItemsSource = null;
         resultsLb.Items.Clear();

         int rtn;
         if (_usnCurrentJournalState.UsnJournalID == 0)
         {
            var journalState = new Win32Api.USN_JOURNAL_DATA_V0();
            rtn = Journal.GetUsnJournalState(ref journalState);

            if (rtn != (int) NtfsUsnJournal.UsnJournalReturnCode.USN_JOURNAL_SUCCESS)
            {
               var lbItem = new ListBoxItem
               {
                  Content = string.Format(CultureInfo.InvariantCulture, "Delete->{0} returned error code: {1}", "GetUsnJournalState", rtn),
                  Foreground = Brushes.Red
               };

               resultsLb.Items.Add(lbItem);
               return;
            }

            _usnCurrentJournalState = journalState;
         }


         rtn = Journal.DeleteUsnJournal(_usnCurrentJournalState);

         FunctionElapsedTime.Content = string.Format(CultureInfo.InvariantCulture, "Delete->{0} duration: {1} (ms)", "DeleteUsnJournal", NtfsUsnJournal.ElapsedTime.TotalMilliseconds);

         if (rtn == (int) NtfsUsnJournal.UsnJournalReturnCode.USN_JOURNAL_SUCCESS)
         {
            var lbItem = new ListBoxItem
            {
               Foreground = Brushes.Black,
               Content = string.Format(CultureInfo.InvariantCulture, "USN journal successfully deleted, DeleteUsnJournal returned: {0}", rtn)
            };
            ;
            resultsLb.Items.Add(lbItem);
         }

         else
         {
            var lbItem = new ListBoxItem
            {
               Content = string.Format(CultureInfo.InvariantCulture, "Delete->{0} returned error code: {1}", "DeleteUsnJournal", rtn),
               Foreground = Brushes.Red
            };

            resultsLb.Items.Add(lbItem);
         }
      }


      private void SaveUsnState_Click(object sender, RoutedEventArgs e)
      {
         e.Handled = true;

         _usnEntryDetail.Visibility = Visibility.Hidden;
         resultsLb.ItemsSource = null;
         resultsLb.Items.Clear();

         var journalState = new Win32Api.USN_JOURNAL_DATA_V0();
         var rtn = Journal.GetUsnJournalState(ref journalState);

         FunctionElapsedTime.Content = string.Format(CultureInfo.InvariantCulture, "Save State->{0} duration: {1} (ms)", "GetUsnJournalState", NtfsUsnJournal.ElapsedTime.TotalMilliseconds);

         if (rtn == (int) NtfsUsnJournal.UsnJournalReturnCode.USN_JOURNAL_SUCCESS)
         {
            _usnCurrentJournalState = journalState;

            var lbItem = new ListBoxItem
            {
               Foreground = Brushes.Black,
               Content = FormatUsnJournalState(journalState)
            };

            resultsLb.Items.Add(lbItem);
         }

         else
         {
            var lbItem = new ListBoxItem
            {
               Content = string.Format(CultureInfo.InvariantCulture, "Save State->{0} returned error code: {1}", "GetUsnJournalState", rtn),
               Foreground = Brushes.Red
            };

            resultsLb.Items.Add(lbItem);
         }
      }


      private void ViewUsnChanges_Click(object sender, RoutedEventArgs e)
      {
         e.Handled = true;

         if (_usnCurrentJournalState.UsnJournalID != 0)
         {
            _usnEntryDetail.Visibility = Visibility.Hidden;
            resultsLb.ItemsSource = null;
            resultsLb.Items.Clear();

            Cursor = Cursors.Wait;
            new Thread(ViewChangesThreadStart).Start();
         }

         else
         {
            var lbItem = new ListBoxItem
            {
               Content = "\'View Changes\'-> Must \'Save State\' before calling \'View Changes\'",
               Foreground = Brushes.Red
            };

            resultsLb.Items.Add(lbItem);
         }
      }


      private void ViewChangesThreadStart()
      {
         Win32Api.USN_JOURNAL_DATA_V0 newUsnState;
         List<Win32Api.UsnEntry> usnEntries;

         const uint reasonMask = Win32Api.USN_REASON_DATA_OVERWRITE | Win32Api.USN_REASON_DATA_EXTEND | Win32Api.USN_REASON_NAMED_DATA_OVERWRITE | Win32Api.USN_REASON_NAMED_DATA_TRUNCATION |
                                 Win32Api.USN_REASON_FILE_CREATE | Win32Api.USN_REASON_FILE_DELETE | Win32Api.USN_REASON_EA_CHANGE | Win32Api.USN_REASON_SECURITY_CHANGE |
                                 Win32Api.USN_REASON_RENAME_OLD_NAME | Win32Api.USN_REASON_RENAME_NEW_NAME | Win32Api.USN_REASON_INDEXABLE_CHANGE | Win32Api.USN_REASON_BASIC_INFO_CHANGE |
                                 Win32Api.USN_REASON_HARD_LINK_CHANGE | Win32Api.USN_REASON_COMPRESSION_CHANGE | Win32Api.USN_REASON_ENCRYPTION_CHANGE | Win32Api.USN_REASON_OBJECT_ID_CHANGE |
                                 Win32Api.USN_REASON_REPARSE_POINT_CHANGE | Win32Api.USN_REASON_STREAM_CHANGE | Win32Api.USN_REASON_CLOSE;


         var rtnCode = Journal.GetUsnJournalEntries(_usnCurrentJournalState, reasonMask, out usnEntries, out newUsnState);

         Dispatcher.Invoke(new FillListBoxDelegate(FillListBoxWithUsnEntries), rtnCode, usnEntries, newUsnState);
      }


      private void FillListBoxWithUsnEntries(NtfsUsnJournal.UsnJournalReturnCode rtnCode, List<Win32Api.UsnEntry> usnEntries, Win32Api.USN_JOURNAL_DATA_V0 newUsnState)
      {
         FunctionElapsedTime.Content = string.Format(CultureInfo.InvariantCulture, "'View Changes'->{0} duration: {1} (ms)", "GetUsnJournalEntries", NtfsUsnJournal.ElapsedTime.TotalMilliseconds);

         if (rtnCode == NtfsUsnJournal.UsnJournalReturnCode.USN_JOURNAL_SUCCESS)
         {
            if (usnEntries.Count > 0)
            {
               _entryDetail = UsnEntryDetail.EntryDetail.UsnEntry;
               resultsLb.ItemsSource = usnEntries;

               var updateUsnStateDlg = new UpdateUsnStateDialog(this) {Owner = this};

               var bRtn = updateUsnStateDlg.ShowDialog();
               if (bRtn != null && bRtn.Value)
                  _usnCurrentJournalState = newUsnState;
            }

            else
            {
               var lbItem = new ListBoxItem
               {
                  Content = "\'View Changes\'-> No Journal entries found",
                  Foreground = Brushes.Red
               };

               resultsLb.Items.Add(lbItem);
            }
         }

         else
         {
            var lbItem = new ListBoxItem
            {
               Content = string.Format(CultureInfo.InvariantCulture, "'View Changes'->{0} returned error code: {1}", "GetUsnJournalEntries", rtnCode),
               Foreground = Brushes.Red
            };

            resultsLb.Items.Add(lbItem);
         }

         Cursor = Cursors.Arrow;
      }


      private void ListFiles_Click(object sender, RoutedEventArgs e)
      {
         e.Handled = true;
         var filterStringDlg = new EnterFileFilterString(this);

         var bRtn = filterStringDlg.ShowDialog();

         if (null != bRtn && bRtn.Value)
         {
            var fileFilter = filterStringDlg.FileFilter;

            if (!string.IsNullOrEmpty(fileFilter))
            {
               _usnEntryDetail.Visibility = Visibility.Hidden;
               resultsLb.ItemsSource = null;
               resultsLb.Items.Clear();

               Cursor = Cursors.Wait;
               new Thread(ListFilesThreadStart).Start(fileFilter);
            }

            else
            {
               var lbItem = new ListBoxItem
               {
                  Content = "\'List Files\'-> File Filter is Null or Empty",
                  Foreground = Brushes.Red
               };

               resultsLb.Items.Add(lbItem);
            }
         }
      }


      private void ListFilesThreadStart(object fileFilterObj)
      {
         var fileFilter = (string) fileFilterObj;
         List<Win32Api.UsnEntry> fileList;
         var rtnCode = Journal.GetUsnFiles(fileFilter, out fileList);
         Dispatcher.Invoke(new FillListBoxWithFilesDelagate(FillListBoxWithFiles), rtnCode, fileList);
      }


      private void FillListBoxWithFiles(NtfsUsnJournal.UsnJournalReturnCode rtnCode, List<Win32Api.UsnEntry> fileList)
      {
         FunctionElapsedTime.Content = string.Format(CultureInfo.InvariantCulture, "Duration: {0} (ms)  Files: {1}", NtfsUsnJournal.ElapsedTime.TotalMilliseconds, fileList.Count);

         if (rtnCode == NtfsUsnJournal.UsnJournalReturnCode.USN_JOURNAL_SUCCESS)
         {
            if (fileList.Count > 0)
            {
               _entryDetail = UsnEntryDetail.EntryDetail.File;
               resultsLb.ItemsSource = fileList;
            }
         }

         else
         {
            var lbItem = new ListBoxItem
            {
               Content = string.Format(CultureInfo.InvariantCulture, "'List Files'->{0} returned error code: {1}", "GetFilesMatchingFilter", rtnCode),
               Foreground = Brushes.Red
            };

            resultsLb.Items.Add(lbItem);
         }

         Cursor = Cursors.Arrow;
      }


      private void ListFolders_Click(object sender, RoutedEventArgs e)
      {
         e.Handled = true;
         _usnEntryDetail.Visibility = Visibility.Hidden;
         resultsLb.ItemsSource = null;
         resultsLb.Items.Clear();

         Cursor = Cursors.Wait;
         new Thread(ListFoldersThreadStart).Start();
      }


      private void ListFoldersThreadStart(object fileFilterObj)
      {
         List<Win32Api.UsnEntry> folders;
         var rtnCode = Journal.GetUsnDirectories(out folders);
         Dispatcher.Invoke(new FillListBoxWithFoldersDelegate(FillListBoxWithFolders), rtnCode, folders);
      }


      private void FillListBoxWithFolders(NtfsUsnJournal.UsnJournalReturnCode rtnCode, List<Win32Api.UsnEntry> folders)
      {
         FunctionElapsedTime.Content = string.Format(CultureInfo.InvariantCulture, "Duration: {0} (ms)  Folders: {1}", NtfsUsnJournal.ElapsedTime.TotalMilliseconds, folders.Count);

         if (rtnCode == NtfsUsnJournal.UsnJournalReturnCode.USN_JOURNAL_SUCCESS)
         {
            if (folders.Count > 0)
            {
               _entryDetail = UsnEntryDetail.EntryDetail.File;
               resultsLb.ItemsSource = folders;
            }
         }

         else
         {
            var lbItem = new ListBoxItem
            {
               Content = string.Format(CultureInfo.InvariantCulture, "'List Folders'->{0} returned error code: {1}", "GetNtfsVolumeFolders", rtnCode),
               Foreground = Brushes.Red
            };

            resultsLb.Items.Add(lbItem);
         }

         Cursor = Cursors.Arrow;
      }


      private void resultsLb_SelectionChanged(object sender, SelectionChangedEventArgs e)
      {
         e.Handled = true;
         var lb = sender as ListBox;

         if (null != lb && null != lb.SelectedItem)
         {
            if (lb.SelectedItem.GetType() == typeof(Win32Api.UsnEntry))
            {
               var item = (Win32Api.UsnEntry) lb.SelectedItem;
               _usnEntryDetail.ChangeDisplay(Journal, _lbItemY, _lbItemX, item, _entryDetail);
            }
         }
      }


      private void resultsLb_MouseDoubleClick(object sender, MouseButtonEventArgs e)
      {
         e.Handled = true;
         var lb = sender as ListBox;

         if (null != lb && null != lb.SelectedItem)
         {
            if (lb.SelectedItem.GetType() == typeof(Win32Api.UsnEntry))
            {
               var usnEntry = (Win32Api.UsnEntry) lb.SelectedItem;
               string path;
               
               var lastError = Journal.GetPathFromFileReference(usnEntry.ParentFileReferenceNumber, out path);
               
               if (lastError == (int) NtfsUsnJournal.UsnJournalReturnCode.USN_JOURNAL_SUCCESS && null != path)
               {
                  if (!usnEntry.IsFolder)
                  {
                     var fullPath = Path.Combine(path, usnEntry.Name);
                     if (File.Exists(fullPath))
                     {
                        try
                        {
                           using (Process.Start(fullPath)) { }
                        }
                        catch (Exception ex)
                        {
                           MessageBox.Show(ex.Message);
                        }
                     }

                     else
                        MessageBox.Show(string.Format(CultureInfo.InvariantCulture, "File not found: {0}", fullPath));
                  }
               }
            }
         }
      }


      private void resultsLb_PreviewMouseDown(object sender, MouseButtonEventArgs e)
      {
         // When true, disables input.
         //e.Handled = true;

         var mousePosition = e.GetPosition(this);
         var pt = resultsDock.PointToScreen(new Point(resultsDock.ActualWidth, mousePosition.Y));
         _lbItemX = pt.X;
         _lbItemY = pt.Y;
      }


      private void Window_Loaded(object sender, RoutedEventArgs e)
      {
         e.Handled = true;
         _usnEntryDetail = new UsnEntryDetail(this);
      }




      private static string FormatUsnJournalState(Win32Api.USN_JOURNAL_DATA_V0 _usnCurrentJournalState)
      {
         var sb = new StringBuilder();

         sb.AppendLine(string.Format(CultureInfo.InvariantCulture, "Journal ID: {0}", _usnCurrentJournalState.UsnJournalID.ToString("X", CultureInfo.InvariantCulture)));
         sb.AppendLine(string.Format(CultureInfo.InvariantCulture, " First USN: {0}", _usnCurrentJournalState.FirstUsn.ToString("X", CultureInfo.InvariantCulture)));
         sb.AppendLine(string.Format(CultureInfo.InvariantCulture, "  Next USN: {0}", _usnCurrentJournalState.NextUsn.ToString("X", CultureInfo.InvariantCulture)));
         sb.AppendLine();
         sb.AppendLine(string.Format(CultureInfo.InvariantCulture, "Lowest Valid USN: {0}", _usnCurrentJournalState.LowestValidUsn.ToString("X", CultureInfo.InvariantCulture)));
         sb.AppendLine(string.Format(CultureInfo.InvariantCulture, "         Max USN: {0}", _usnCurrentJournalState.MaxUsn.ToString("X", CultureInfo.InvariantCulture)));
         sb.AppendLine(string.Format(CultureInfo.InvariantCulture, "        Max Size: {0}", _usnCurrentJournalState.MaximumSize.ToString("X", CultureInfo.InvariantCulture)));
         sb.AppendLine(string.Format(CultureInfo.InvariantCulture, "Allocation Delta: {0}", _usnCurrentJournalState.AllocationDelta.ToString("X", CultureInfo.InvariantCulture)));
         
         return sb.ToString();
      }
   }
}
