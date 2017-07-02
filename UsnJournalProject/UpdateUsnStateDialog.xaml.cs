using System.Windows;

namespace UsnJournalProject
{
   /// <summary>Interaction logic for UpdateUsnStateDialog.xaml</summary>
   public partial class UpdateUsnStateDialog : Window
   {
      public UpdateUsnStateDialog(Window owner)
      {
         InitializeComponent();
         Owner = owner;
         WindowStartupLocation = WindowStartupLocation.CenterOwner;
      }


      private void _ok_Click(object sender, RoutedEventArgs e)
      {
         e.Handled = true;
         DialogResult = true;
      }


      private void _cancel_Click(object sender, RoutedEventArgs e)
      {
         e.Handled = true;
         DialogResult = false;
      }
   }
}
