using System.Media;
using System.Windows;

namespace ClientManager
{
    /// <summary>
    /// Логика взаимодействия для InfoBox.xaml
    /// </summary>
    public partial class InfoBox : Window
    {
        public InfoBox(string message)
        {
            SystemSounds.Exclamation.Play();

            InitializeComponent();

            infoBlock.Text = message;
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;

            Close();
        }
    }
}
