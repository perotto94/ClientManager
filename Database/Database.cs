using System.Collections.Generic;

using static ClientManager.Toolkit;

namespace ClientManager
{
    /// <summary>
    /// Contains clients
    /// </summary>
    public partial class Database
    {
        /// <summary>
        /// Name of database
        /// </summary>
        public string Name { get; set; } = "";

        /// <summary>
        /// List of clients
        /// </summary>
        public List<Client> Clients { get; } = new();

        /// <summary>
        /// List of events
        /// </summary>
        public List<PlannedEvent> Events { get; } = new();

        /// <summary>
        /// Add client to list with reference
        /// </summary>
        public void Add(Client client)
        {
            client.Database = this;

            Clients.Add(client);

            SortByDate(Clients);
        }

        /// <summary>
        /// Add event to list with reference
        /// </summary>
        public void Add(PlannedEvent plannedEvent)
        {
            plannedEvent.Database = this;

            Events.Add(plannedEvent);

            SortEvents();
        }

        /// <summary>
        /// Sort clients by date
        /// </summary>
        public void SortClients()
        {
            SortByDate(Clients);
        }

        /// <summary>
        /// Sort events by date
        /// </summary>
        public void SortEvents()
        {
            SortByDate(Events);
        }

        /// <summary>
        /// Folder for synchronization. Sync file = folder + name.txt
        /// </summary>
        public string SyncFolder { get; set; } = "";

        /// <summary>
        /// Convert all properties to CSV
        /// </summary>
        public override string ToString()
        {
            return $"{Name},{SyncFolder},{Clients.Count},{Events.Count}";
        }

        /// <summary>
        /// Read all properties from string array
        /// </summary>
        public bool FromString(string[] str, out int clientsNum, out int eventsNum)
        {
            clientsNum = 0;
            eventsNum = 0;

            if (str.Length != 4)
            {
                return false;
            }

            bool result = int.TryParse(str[2], out clientsNum);
            result &= int.TryParse(str[3], out eventsNum);

            if (!result)
            {
                return false;
            }

            Name = str[0];
            SyncFolder = str[1];

            return true;
        }
    }
}
