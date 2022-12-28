using System;
using System.IO;
using System.Text;

namespace ClientManager
{
    public partial class Database
    {
        /// <summary>
        /// Read database from file
        /// </summary>
        public bool ReadFromFile(string filePath, out string result)
        {
            result = "";

            int lineNum = -1;

            try
            {
                using (FileStream fs = new(filePath, FileMode.Open, FileAccess.Read))
                using (StreamReader f = new(fs, Encoding.UTF8))
                {
                    string? line = f.ReadLine();
                    lineNum = 0;
                    string[] str = line.Split(',');

                    bool conv = FromString(str, out int clientsNum, out int eventsNum);

                    if (!conv)
                    {
                        throw new Exception($"Cannot read db info");
                    }

                    for (int i = 0; i < clientsNum; i++)
                    {
                        line = f.ReadLine();
                        lineNum++;
                        str = line.Split(',');

                        Client client = new();

                        conv = client.FromString(str, out int ordersSingle, out int ordersRegular);

                        if (!conv)
                        {
                            throw new Exception($"Cannot read client info");
                        }

                        for (int j = 0; j < ordersSingle; j++)
                        {
                            line = f.ReadLine();
                            lineNum++;
                            str = line.Split(',');

                            OrderSingle orderSingle = new();

                            conv = orderSingle.FromString(str);

                            if (!conv)
                            {
                                throw new Exception($"Cannot read single order info");
                            }

                            orderSingle.Client = client;

                            client.Add(orderSingle);
                        }

                        for (int j = 0; j < ordersRegular; j++)
                        {
                            line = f.ReadLine();
                            lineNum++;
                            str = line.Split(',');

                            OrderRegular orderRegular = new();

                            conv = orderRegular.FromString(str, out int exerciseNum);

                            if (!conv)
                            {
                                throw new Exception($"Cannot read regular order info");
                            }

                            orderRegular.Client = client;

                            for (int k = 0; k < exerciseNum; k++)
                            {
                                line = f.ReadLine();
                                lineNum++;
                                str = line.Split(',');

                                Exercise exercise = new();

                                conv = exercise.FromString(str);

                                if (!conv)
                                {
                                    throw new Exception($"Cannot read exercise info");
                                }

                                exercise.OrderRegular = orderRegular;

                                orderRegular.Add(exercise);
                            }

                            client.Add(orderRegular);
                        }

                        Add(client);
                    }

                    for (int i = 0; i < eventsNum; i++)
                    {
                        line = f.ReadLine();
                        lineNum++;
                        str = line.Split(',');

                        PlannedEvent plannedEvent = new();

                        conv = plannedEvent.FromString(str);

                        if (!conv)
                        {
                            throw new Exception($"Cannot read event info");
                        }

                        Add(plannedEvent);
                    }

                    return true;
                }
            }
            catch (Exception e)
            {
                result = e.Message + $"\nTriggered in line {lineNum}";

                return false;
            }
        }
    }
}
