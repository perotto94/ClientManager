using System.Collections.Generic;

using static ClientManager.Toolkit;

namespace ClientManager
{
    /// <summary>
    /// Class with global variables
    /// </summary>
    public class GlobalVars
    {
        /// <summary>
        /// Directory for sync
        /// </summary>
        public string? SyncDir { get; set; } = null;

        /// <summary>
        /// List of databases of clients
        /// </summary>
        public List<Database> DB { get; } = new();

        /// <summary>
        /// Get all clients from all databases
        /// </summary>
        public List<Client> GetClients(bool sortByDueDate)
        {
            List<Client> clients = new();

            foreach (Database db in DB)
            {
                foreach (Client client in db.Clients)
                {
                    clients.Add(client);
                }
            }

            if (sortByDueDate)
            {
                SortByDate(clients);
            }

            return clients;
        }

        /// <summary>
        /// Get number of clients in all databases
        /// </summary>
        /// <returns></returns>
        public int GetNumberOfClients()
        {
            return GetClients(false).Count;
        }
    }
}
