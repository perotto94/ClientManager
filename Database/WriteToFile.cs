using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ClientManager
{
    public partial class Database
    {
        /// <summary>
        /// Write databases to file
        /// </summary>
        public static bool WriteToFile(List<Database> dbList, out string result)
        {
            result = "";

            try
            {
                foreach (Database db in dbList)
                {
                    string filePath = db.SyncFolder + $"\\{db.Name}.txt";

                    bool createBackup = File.Exists(filePath);
                    string fileBackup = db.SyncFolder + $"\\{db.Name}_backup.txt";

                    if (File.Exists(fileBackup))
                    {
                        throw new Exception($"Резервный файл для базы {db.Name} уже находится в папке");
                    }

                    if (createBackup)
                    {
                        File.Copy(filePath, fileBackup, true);
                    }

                    using (FileStream fs = new(filePath, FileMode.Create))
                    using (StreamWriter f = new(fs, Encoding.UTF8, 128))
                    {
                        f.WriteLine(db.ToString());

                        foreach (Client client in db.Clients)
                        {
                            f.WriteLine(client.ToString());

                            foreach (Order order in client.Orders)
                            {
                                if (order is OrderSingle orderSingle)
                                {
                                    f.WriteLine(orderSingle.ToString());
                                }
                            }

                            foreach (Order order in client.Orders)
                            {
                                if (order is OrderRegular orderRegular)
                                {
                                    f.WriteLine(orderRegular.ToString());

                                    foreach (Exercise exercise in orderRegular.Exercises)
                                    {
                                        f.WriteLine(exercise.ToString());
                                    }
                                }
                            }
                        }

                        foreach (PlannedEvent plannedEvent in db.Events)
                        {
                            f.WriteLine(plannedEvent.ToString());
                        }
                    }

                    if (createBackup)
                    {
                        File.Delete(fileBackup);
                    }
                }

                return true;
            }
            catch (Exception e)
            {
                result = e.Message;

                return false;
            }
        }
    }
}
