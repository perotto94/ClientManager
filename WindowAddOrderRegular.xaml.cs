using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ClientManager
{
    /// <summary>
    /// Interaction logic for WindowAddOrderRegular.xaml
    /// </summary>
    public partial class WindowAddOrderRegular : Window
    {
        // Reference to client
        readonly Client client;

        // Reference to order
        readonly OrderRegular order;

        // If order is being edited
        readonly bool ifEdit;

        // If value textbox is locked
        bool tbValueLocked = false;

        public WindowAddOrderRegular(Client client, bool ifEdit = false, OrderRegular order = null)
        {
            this.ifEdit = ifEdit;
            this.client = client;

            InitializeComponent();

            // If adding order
            if (!ifEdit)
            {
                this.order = new OrderRegular();
            }

            // If editing order
            else
            {
                buttonDeleteOrder.IsEnabled = true;

                this.order = order;

                Title = "Редактировать регулярные занятия";

                tbSubject.Text = order.Subject;
                tbTopic.Text = order.Topic;
                tbValue.Text = $"{order.Value:G}";
                tbComments.Text = order.Comments;
            }
        }

        /// <summary>
        /// Limit value field to digits
        /// </summary>
        private void tbValue_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (tbValueLocked) return;

            tbValueLocked = true;

            int caretIndex = tbValue.CaretIndex;

            string text = tbValue.Text;

            for (int i = text.Length - 1; i >= 0; i--)
            {
                bool isDigit = false;

                for (char c = '0'; c <= '9'; c++)
                {
                    if (text[i] == c)
                    {
                        isDigit = true;

                        continue;
                    }
                }

                if (!isDigit)
                {
                    text = text.Remove(i, 1);
                    caretIndex--;
                }
            }

            tbValue.Text = text;

            if (caretIndex > -1)
            {
                tbValue.CaretIndex = caretIndex;
            }

            tbValueLocked = false;
        }

        /// <summary>
        /// Save order
        /// </summary>
        private void buttonAddOrder_Click(object sender, RoutedEventArgs e)
        {
            order.Subject = tbSubject.Text;
            order.Topic = tbTopic.Text;
            order.Value = int.Parse(tbValue.Text);
            order.Comments = tbComments.Text;

            if (!ifEdit)
            {
                client.Add(order);
            }

            DialogResult = true;

            Close();
        }

        /// <summary>
        /// Delete order
        /// </summary>
        private void buttonDeleteOrder_Click(object sender, RoutedEventArgs e)
        {
            client.Orders.Remove(order);

            DialogResult = true;

            Close();
        }
    }
}
