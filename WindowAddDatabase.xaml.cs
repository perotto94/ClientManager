using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ClientManager
{
    /// <summary>
    /// Логика взаимодействия для WindowAddDatabase.xaml
    /// </summary>
    public partial class WindowAddDatabase : Window
    {
        GlobalVars globalVars;

        public WindowAddDatabase(GlobalVars globalVars)
        {
            this.globalVars = globalVars;

            InitializeComponent();

            tbDBName.Select(0, 0);
        }

        // Check for existing names and add db
        private void buttonAddDB_Click(object sender, RoutedEventArgs e)
        {
            string text = tbDBName.Text;

            // Check for empty string
            if (text == "")
            {
                textBlockNameStatus.Text = "Имя не может быть пустым";

                return;
            }

            // Check for existing db
            foreach (Database db in globalVars.DB)
            {
                if (db.Name.Equals(text, StringComparison.OrdinalIgnoreCase))
                {
                    textBlockNameStatus.Text = "Имя уже есть в базе";

                    return;
                }
            }

            // Check for invalid characters
            for (int i = 0; i < text.Length; i++)
            {
                bool check = false;

                for (char letter = 'a'; letter <= 'z'; letter++)
                {
                    if (text[i].Equals(letter))
                    {
                        check = true;
                    }
                }

                for (char letter = 'а'; letter <= 'я'; letter++)
                {
                    if (text[i].Equals(letter))
                    {
                        check = true;
                    }
                }

                for (char letter = 'А'; letter <= 'Я'; letter++)
                {
                    if (text[i].Equals(letter))
                    {
                        check = true;
                    }
                }

                for (char letter = 'A'; letter <= 'Z'; letter++)
                {
                    if (text[i].Equals(letter))
                    {
                        check = true;
                    }
                }

                for (char letter = '0'; letter <= '9'; letter++)
                {
                    if (text[i].Equals(letter))
                    {
                        check = true;
                    }
                }

                if (!check)
                {
                    textBlockNameStatus.Text = "Имя может содержать только буквы и цифры";

                    return;
                }
            }

            // Add db
            globalVars.DB.Add(new() { Name = text, SyncFolder = globalVars.SyncDir });

            DialogResult = true;

            Close();
        }

        // Clear status box on text changed
        private void tbDBName_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (textBlockNameStatus != null)
            {
                textBlockNameStatus.Text = "";
            }
        }
    }
}
