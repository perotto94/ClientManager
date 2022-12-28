using System.Windows;
using System.Windows.Documents;

namespace ClientManager
{
    public partial class Client : IDated
    {
        /// <summary>
        /// Flow document for rich text box
        /// </summary>
        public FlowDocument ToFlowDocument()
        {
            Paragraph pName = new() { FontSize = 16, LineHeight = 8, TextAlignment = TextAlignment.Center };
            pName.Inlines.Add(new Bold(new Run(Name)));

            Paragraph pDB = new() { FontSize = 12, LineHeight = 3, TextAlignment = TextAlignment.Left };
            pDB.Inlines.Add(new Bold(new Run($"БД: {Database.Name}")));

            Paragraph pMessenger = new() { FontSize = 12, LineHeight = 3, TextAlignment = TextAlignment.Left };
            pMessenger.Inlines.Add(new Run($"Мессенджер: {EnumHelper.GetDescription(Messenger)}"));

            Paragraph pPhone = new() { FontSize = 12, LineHeight = 3, TextAlignment = TextAlignment.Left };
            pPhone.Inlines.Add(new Run($"Телефон: {Phone}"));

            Paragraph pSource = new() { FontSize = 12, LineHeight = 3, TextAlignment = TextAlignment.Left };
            pSource.Inlines.Add(new Run($"Источник: {EnumHelper.GetDescription(Source)}"));

            Paragraph pComments = new() { FontSize = 12, LineHeight = 3, TextAlignment = TextAlignment.Left };
            pComments.Inlines.Add(new Run($"Комментарии: {Comments}"));

            FlowDocument flowDocument = new();

            flowDocument.Blocks.Add(pName);
            flowDocument.Blocks.Add(pDB);
            flowDocument.Blocks.Add(pMessenger);
            if (Phone != "") flowDocument.Blocks.Add(pPhone);
            flowDocument.Blocks.Add(pSource);
            if (Comments != "") flowDocument.Blocks.Add(pComments);

            return flowDocument;
        }
    }
}
