using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace UsnJournalProject
{
   /// <summary>Interaction logic for EnterFileFilterString.xaml</summary>
   public partial class EnterFileFilterString : Window
   {
      private string _filter = string.Empty;
      public string FileFilter
      {
         get { return _filter; }
      }


      public EnterFileFilterString(Window owner)
      {
         InitializeComponent();
         Owner = owner;
         WindowState = WindowState.Normal;
         WindowStartupLocation = WindowStartupLocation.CenterOwner;
      }


      private void _cancel_Click(object sender, RoutedEventArgs e)
      {
         e.Handled = true;
         DialogResult = false;
      }


      private void _ok_Click(object sender, RoutedEventArgs e)
      {
         e.Handled = true;
         DialogResult = true;
         _filter = _fileFilterTb.Text;
      }


      private void _fileFilter_PreviewMouseDown(object sender, MouseButtonEventArgs e)
      {
         // When true, disables input.
         //e.Handled = true;

         var tb = sender as TextBox;

         if (null != tb && 0 == string.Compare(tb.Text, "*", StringComparison.Ordinal))
            tb.Text = string.Empty;
      }
   }
}
