using System;
using System.IO;
using System.Windows;
using System.Text;

namespace ClientManager
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        GlobalVars globalVars;

        /// <summary>
        /// On startup configure global variables and databases
        /// </summary>
        void ApplicationOnStartup(object sender, StartupEventArgs args)
        {
            ShutdownMode = ShutdownMode.OnExplicitShutdown;

            globalVars = new();

            InfoBox infoBox;

            try
            {
                // If config.txt file exists, check that sync folder exists and read databases
                if (File.Exists("config.txt"))
                {
                    using (FileStream fs = new("config.txt", FileMode.Open, FileAccess.Read))
                    using (StreamReader f = new(fs, Encoding.UTF8))
                    {
                        globalVars.SyncDir = f.ReadLine();

                        if (Directory.Exists(globalVars.SyncDir))
                        {
                            ReadDatabasesFromDir();

                            MainWindow mainWindow = new(globalVars);
                            mainWindow.ShowDialog();

                            Shutdown(); return;
                        }
                        else
                        {
                            File.Delete("config.txt");

                            infoBox = new("Папка, указанная в config файле, не существует");
                            infoBox.ShowDialog();
                        }
                    }
                }

                // Offer to choose a sync directory

                infoBox = new("Выберите папку для синхронизации БД");
                infoBox.ShowDialog();

                var folderBrowser = new Ookii.Dialogs.Wpf.VistaFolderBrowserDialog();

                if (folderBrowser.ShowDialog() == true)
                {
                    string? folderPath = folderBrowser.SelectedPath;

                    using (FileStream fs = new("config.txt", FileMode.Create))
                    using (StreamWriter f = new(fs, Encoding.UTF8, 128))
                    {
                        f.Write(folderPath);
                    }

                    globalVars.SyncDir = folderPath;
                    ReadDatabasesFromDir();

                    MainWindow mainWindow = new(globalVars);
                    mainWindow.ShowDialog();

                    Shutdown(); return;
                }
                else
                {
                    Shutdown(); return;
                }
            }
            catch (Exception e)
            {
                infoBox = new(e.Message);
                infoBox.ShowDialog();

                Shutdown(); return;
            }
        }

        /// <summary>
        /// Read databases from directory
        /// </summary>
        private void ReadDatabasesFromDir()
        {
            string[] dirFiles = Directory.GetFiles(globalVars.SyncDir);

            foreach (string filePath in dirFiles)
            {
                string? fileName = Path.GetFileName(filePath);
                string? fileExt = Path.GetExtension(filePath);

                if (fileExt != ".txt")
                {
                    continue;
                }

                Database db = new();

                bool statusDBRead = db.ReadFromFile(filePath, out string resultDBRead);

                if (statusDBRead)
                {
                    globalVars.DB.Add(db);
                }
                else
                {
                    InfoBox infoBox = new($"Database {db.Name} could not be read:\n" + resultDBRead);
                    infoBox.ShowDialog();
                }
            }
        }
    }
}
