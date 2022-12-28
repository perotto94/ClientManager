using System;
using System.Collections.Generic;
using System.Globalization;

using static ClientManager.Toolkit;

namespace ClientManager
{
    /// <summary>
    /// Client info
    /// </summary>
    public partial class Client: IDated
    {
        /// <summary>
        /// Reference to parent database
        /// </summary>
        public Database Database { get; set; } = null;

        /// <summary>
        /// First name
        /// </summary>
        public string FirstName { get; set; } = "";

        /// <summary>
        /// Middle name
        /// </summary>
        public string MiddleName { get; set; } = "";

        /// <summary>
        /// Last name
        /// </summary>
        public string LastName { get; set; } = "";

        /// <summary>
        /// Cell phone number
        /// </summary>
        public string Phone { get; set; } = "";

        /// <summary>
        /// Messenger
        /// </summary>
        public Messengers Messenger { set; get; } = Messengers.Other;

        /// <summary>
        /// Source
        /// </summary>
        public Sources Source { set; get; } = Sources.Other;

        /// <summary>
        /// Date when the client was added
        /// </summary>
        public DateTime DateStart { get; set; } = DateTime.MinValue;

        /// <summary>
        /// Path to working directory
        /// </summary>
        public string FolderPath { get; set; } = "";

        /// <summary>
        /// Comments on client
        /// </summary>
        public string Comments { get; set; } = "";

        /// <summary>
        /// The most relevant order date
        /// </summary>
        public DateTime DateRelevant { get { return Orders.Count > 0 ? Orders[0].DateRelevant : DateTime.MaxValue; } }

        /// <summary>
        /// Orders
        /// </summary>
        public List<Order> Orders { get; } = new();

        /// <summary>
        /// Get i-th order
        /// </summary>
        public Order this[int i]
        {
            get => Orders[i];
        }

        /// <summary>
        /// Add order with reference
        /// </summary>
        public void Add(Order order)
        {
            order.Client = this;

            Orders.Add(order);

            SortOrders();
        }

        /// <summary>
        /// Sort orders by date
        /// </summary>
        public void SortOrders()
        {
            SortByDate(Orders);

            if (Database != null)
            {
                Database.SortClients();
            }
        }

        /// <summary>
        /// Convert all properties to CSV
        /// </summary>
        public override string ToString()
        {
            int ordersSingle = 0;
            int ordersRegular = 0;

            foreach (Order order in Orders)
            {
                if (order is OrderSingle)
                {
                    ordersSingle++;
                }
                else if (order is OrderRegular)
                {
                    ordersRegular++;
                }
            }

            return $"{FirstName},{MiddleName},{LastName},{Phone},{Messenger},{Source},{DateStart:yyyy-MM-dd-HH-mm},{FolderPath},{Comments},{ordersSingle},{ordersRegular}";
        }

        /// <summary>
        /// Read all properties from string array
        /// </summary>
        public bool FromString(string[] str, out int ordersSingle, out int ordersRegular)
        {
            ordersSingle = 0;
            ordersRegular = 0;

            if (str.Length != 11)
            {
                return false;
            }

            CultureInfo enUS = new("en-US");

            bool result = FindEnumInString(str[4], out Messengers sMessenger);
            result &= FindEnumInString(str[5], out Sources sSource);
            result &= DateTime.TryParseExact(str[6], "yyyy-MM-dd-HH-mm", enUS, DateTimeStyles.None, out DateTime sDateStart);
            result &= int.TryParse(str[9], out ordersSingle);
            result &= int.TryParse(str[10], out ordersRegular);

            if (!result)
            {
                return false;
            }

            FirstName = str[0];
            MiddleName = str[1];
            LastName = str[2];
            Phone = str[3];
            Messenger = sMessenger;
            Source = sSource;
            DateStart = sDateStart;
            FolderPath = str[7];
            Comments = str[8];

            return true;
        }
    }
}
