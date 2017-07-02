using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace UsnJournalProject
{
   /// <summary>Interaction logic for VolumeSelectDialog.xaml</summary>
   public partial class VolumeSelectDialog : Window
   {
      public DriveInfo Volume { get; private set; }

      public VolumeSelectDialog(Window owner)
      {
         InitializeComponent();
         Owner = owner;
         WindowStartupLocation = WindowStartupLocation.CenterOwner;

         foreach (var di in DriveInfo.GetDrives())
            if (di.IsReady && 0 == string.Compare(di.DriveFormat, "NTFS", StringComparison.OrdinalIgnoreCase))
               drivesLb.Items.Add(new ListBoxItem
               {
                  Content = di.Name,
                  Tag = di
               });
      }


      private void cancel_Click(object sender, RoutedEventArgs e)
      {
         e.Handled = true;
         DialogResult = false;
      }


      private void ok_Click(object sender, RoutedEventArgs e)
      {
         e.Handled = true;
         HandleSelection();
      }


      private void drivesLb_SelectionChanged(object sender, SelectionChangedEventArgs e)
      {
         e.Handled = true;
         selectionErrorTb.Text = string.Empty;
      }


      private void drivesLb_MouseDoubleClick(object sender, MouseButtonEventArgs e)
      {
         e.Handled = true;
         HandleSelection();
      }


      private void HandleSelection()
      {
         if (null != drivesLb.SelectedItem)
         {
            var lbItem = drivesLb.SelectedItem as ListBoxItem;

            if (null != lbItem)
               Volume = lbItem.Tag as DriveInfo;

            DialogResult = true;
         }

         else
            selectionErrorTb.Text = "No volume selected";
      }
   }
}
