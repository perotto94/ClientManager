using System;
using System.Globalization;
using System.Windows;
using System.Windows.Documents;
using static ClientManager.Toolkit;

namespace ClientManager
{
    /// <summary>
    /// Single order
    /// </summary>
    public class OrderSingle : Order
    {
        /// <summary>
        /// Value in rubles
        /// </summary>
        public int Value { get; set; } = 0;

        OrderStatus status = OrderStatus.None;

        /// <summary>
        /// Exercise status
        /// </summary>
        public OrderStatus Status
        {
            get => status;

            set
            {
                status = value;

                if (Client != null)
                {
                    Client.SortOrders();
                }
            }
        }

        DateTime dueDate = DateTime.MinValue;

        /// <summary>
        /// Due date
        /// </summary>
        public DateTime DueDate
        {
            get => dueDate;

            set
            {
                dueDate = value;

                if (Client != null)
                {
                    Client.SortOrders();
                }
            }
        }

        /// <summary>
        /// Get most relevant exercise datetime
        /// </summary>
        public override DateTime DateRelevant
        {
            get
            {
                if (Status == OrderStatus.None ||
                    Status == OrderStatus.Freeze ||
                    Status == OrderStatus.Canceled ||
                    Status == OrderStatus.Completed)
                {
                    return DateTime.MaxValue;
                }

                return dueDate;
            }
        }

        public override string ShortDescription()
        {
            string result = Subject;

            if (Topic != "") result += " / " + Topic;

            return $"{result} / {EnumHelper.GetDescription(Status)}";
        }

        /// <summary>
        /// Convert all properties to CSV
        /// </summary>
        public override string ToString()
        {
            return $"{DueDate:yyyy-MM-dd-HH-mm},{Value:G},{Subject},{Topic},{Comments},{Status}";
        }

        /// <summary>
        /// Read all properties from string array
        /// </summary>
        public bool FromString(string[] str)
        {
            if (str.Length != 6)
            {
                return false;
            }

            CultureInfo enUS = new("en-US");

            bool result = DateTime.TryParseExact(str[0], "yyyy-MM-dd-HH-mm", enUS, DateTimeStyles.None, out DateTime sDueDate);
            result &= int.TryParse(str[1], out int sValue);
            result &= FindEnumInString(str[5], out OrderStatus sStatus);

            if (!result)
            {
                return false;
            }

            DueDate = sDueDate;
            Value = sValue;
            Subject = str[2];
            Topic = str[3];
            Comments = str[4];
            Status = sStatus;

            return true;
        }

        public override FlowDocument ToFlowDocument()
        {
            Paragraph pDueDate = new() { FontSize = 12, LineHeight = 3, TextAlignment = TextAlignment.Left };
            pDueDate.Inlines.Add(new Bold(new Run($"Срок: {DueDate:dd.MM.yyyy HH:mm}")));

            Paragraph pValue = new() { FontSize = 12, LineHeight = 3, TextAlignment = TextAlignment.Left };
            pValue.Inlines.Add(new Run($"Стоимость: {Value:G} руб."));

            Paragraph pComments = new() { FontSize = 12, LineHeight = 3, TextAlignment = TextAlignment.Left };
            pComments.Inlines.Add(new Run($"Комментарии: {Comments}"));

            FlowDocument flowDocument = new();

            flowDocument.Blocks.Add(pDueDate);
            flowDocument.Blocks.Add(pValue);
            if (Comments != "") flowDocument.Blocks.Add(pComments);

            return flowDocument;
        }
    }
}
