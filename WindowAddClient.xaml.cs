using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace ClientManager
{
    /// <summary>
    /// Логика взаимодействия для WindowAddDatabase.xaml
    /// </summary>
    public partial class WindowAddClient : Window
    {
        // Reference to global vars
        readonly List<Database> dbList;

        // Reference to client
        readonly Client client;

        // If client is being edited
        readonly bool ifEdit;

        // If edit client, find its database index
        readonly int editDBIndex;

        // If edit client, find its index in database
        readonly int editClientIndex;

        // Skip phone check
        bool skipPhoneCheck = false;

        /// <summary>
        /// Add or edit client
        /// </summary>
        public WindowAddClient(GlobalVars globalVars, bool ifEdit = false, Client client = null)
        {
            dbList = globalVars.DB;
            this.ifEdit = ifEdit;

            InitializeComponent();

            // Initialize databases combobox
            foreach (Database db in globalVars.DB)
            {
                cbDatabase.Items.Add(db.Name);
            }

            // If adding client
            if (!ifEdit)
            {
                this.client = new Client();

                cbDatabase.SelectedIndex = 0;
                cbMessenger.SelectedIndex = 0;
                cbSource.SelectedIndex = 0;

                datePickerStart.SelectedDate = DateTime.Now;
            }

            // If editing client
            else
            {
                this.client = client;

                buttonDeleteClient.IsEnabled = true;

                Title = "Редактировать клиента";

                for (editDBIndex = 0;
                    editDBIndex < dbList.Count &&
                    dbList[editDBIndex] != client.Database;
                    editDBIndex++) ;

                for (editClientIndex = 0;
                    editClientIndex < dbList[editDBIndex].Clients.Count &&
                    dbList[editDBIndex].Clients[editClientIndex] != client;
                    editClientIndex++) ;

                cbDatabase.SelectedIndex = editDBIndex;
                cbMessenger.SelectedIndex = (int)client.Messenger;
                cbSource.SelectedIndex = (int)client.Source;

                datePickerStart.SelectedDate = client.DateStart;

                tbFirstName.Text = client.FirstName;
                tbMiddleName.Text = client.MiddleName;
                tbLastName.Text = client.LastName;

                tbPhone.Text = client.Phone;

                tbFolderPath.Text = client.FolderPath;
                tbFolderPath.CaretIndex = tbFolderPath.Text.Length;

                tbComments.Text = client.Comments;
            }
        }

        /// <summary>
        /// Add client
        /// </summary>
        private void buttonAddClient_Click(object sender, RoutedEventArgs e)
        {
            client.FirstName = tbFirstName.Text;
            client.MiddleName = tbMiddleName.Text;
            client.LastName = tbLastName.Text;
            client.Phone = tbPhone.Text;
            client.Messenger = (Messengers)cbMessenger.SelectedIndex;
            client.Source = (Sources)cbSource.SelectedIndex;
            client.DateStart = datePickerStart.DisplayDate;
            client.Comments = tbComments.Text;
            
            // Just add client to selected DB
            if (!ifEdit)
            {
                client.Database = dbList[cbDatabase.SelectedIndex];

                dbList[cbDatabase.SelectedIndex].Clients.Add(client);
            }

            // If DB was changed, delete client and add to selected DB
            else if (cbDatabase.SelectedIndex != editDBIndex)
            {
                dbList[cbDatabase.SelectedIndex].Clients.Add(client);

                dbList[editDBIndex].Clients.RemoveAt(editClientIndex);
            }
            
            DialogResult = true;

            Close();
        }

        /// <summary>
        /// Limit phone to numbers and '+' sign
        /// </summary>
        private void tbPhone_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (skipPhoneCheck) return;

            skipPhoneCheck = true;

            int caretIndex = tbPhone.CaretIndex;

            string text = tbPhone.Text;
            int textLength = text.Length;

            string textEdit = "";

            for (int i = 0; i < textLength; i++)
            {
                for (char c = '0'; c <= '9'; c++)
                {
                    if (text[i] == c)
                    {
                        textEdit += c;

                        continue;
                    }
                }

                if (text[i] == '+')
                {
                    textEdit += '+';

                    continue;
                }
            }

            tbPhone.Text = textEdit;

            int textEditLength = textEdit.Length;

            if (textEditLength < textLength && caretIndex - 1 >= 0)
            {
                tbPhone.CaretIndex = caretIndex - 1;
            }

            skipPhoneCheck = false;
        }

        /// <summary>
        /// Choose path to client folder
        /// </summary>
        private void buttonChooseFolder_Click(object sender, RoutedEventArgs e)
        {
            var folderBrowser = new Ookii.Dialogs.Wpf.VistaFolderBrowserDialog();

            if (folderBrowser.ShowDialog() == true)
            {
                client.FolderPath = folderBrowser.SelectedPath;

                tbFolderPath.Text = client.FolderPath;
                tbFolderPath.CaretIndex = tbFolderPath.Text.Length;
            }
        }

        /// <summary>
        /// Delete client
        /// </summary>
        private void buttonDeleteClient_Click(object sender, RoutedEventArgs e)
        {
            Database db = client.Database;
            db.Clients.Remove(client);

            DialogResult = true;

            Close();
        }
    }
}
