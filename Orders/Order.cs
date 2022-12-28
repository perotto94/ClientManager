using System;
using System.Windows.Documents;

namespace ClientManager
{
    /// <summary>
    /// Order info
    /// </summary>
    public abstract class Order : IDated
    {
        /// <summary>
        /// Reference to parent
        /// </summary>
        public Client Client { get; set; } = null;

        /// <summary>
        /// Subject
        /// </summary>
        public string Subject { get; set; } = "";

        /// <summary>
        /// Topic
        /// </summary>
        public string Topic { get; set; } = "";

        /// <summary>
        /// Comments on order
        /// </summary>
        public string Comments { get; set; } = "";

        /// <summary>
        /// The most relevant order date
        /// </summary>
        public abstract DateTime DateRelevant { get; }

        /// <summary>
        /// Order description for the listbox
        /// </summary>
        public abstract string ShortDescription();

        /// <summary>
        /// Flow document for rich text box
        /// </summary>
        public abstract FlowDocument ToFlowDocument();
    }
}
