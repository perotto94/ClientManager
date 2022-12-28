using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;

using static ClientManager.Toolkit;

namespace ClientManager
{
    /// <summary>
    /// Interaction logic for WindowAddOrderSingle.xaml
    /// </summary>
    public partial class WindowAddOrderSingle : Window
    {
        // Reference to client
        readonly Client client;

        // Reference to order
        readonly OrderSingle order;

        // If order is being edited
        readonly bool ifEdit;

        // If textbox check is allowed
        bool skipTextBoxCheck = true;

        // Old text in timespan text boxes
        string tbDueTextOld;

        public WindowAddOrderSingle(Client client, bool ifEdit = false, OrderSingle order = null)
        {
            this.ifEdit = ifEdit;
            this.client = client;

            InitializeComponent();

            // If adding order
            if (!ifEdit)
            {
                this.order = new OrderSingle();

                cbStatus.SelectedIndex = 0;

                DateTime now = DateTime.Now;

                datePickerDueDate.SelectedDate = now;
                tbDueTime.Text = $"{now:HH\\:mm}";
            }

            // If editing order
            else
            {
                buttonDeleteOrder.IsEnabled = true;

                this.order = order;

                Title = "Редактировать разовое занятие";

                datePickerDueDate.SelectedDate = order.DueDate;
                tbDueTime.Text = $"{order.DueDate:HH\\:mm}";
                tbSubject.Text = order.Subject;
                tbTopic.Text = order.Topic;
                tbValue.Text = $"{order.Value:G}";
                cbStatus.SelectedIndex = (int)order.Status;
                tbComments.Text = order.Comments;
            }

            tbDueTextOld = tbDueTime.Text;

            skipTextBoxCheck = false;
        }

        /// <summary>
        /// Limit value field to digits
        /// </summary>
        private void tbValue_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBoxDigits(tbValue, ref skipTextBoxCheck);
        }

        /// <summary>
        /// Save order
        /// </summary>
        private void buttonAddOrder_Click(object sender, RoutedEventArgs e)
        {
            CultureInfo enUS = new("en-US");

            order.DueDate = DateTime.ParseExact(datePickerDueDate.Text, "dd.MM.yyyy", enUS) +
                TimeSpan.ParseExact(tbDueTime.Text, @"hh\:mm", enUS);

            order.Subject = tbSubject.Text;
            order.Topic = tbTopic.Text;
            order.Value = int.Parse(tbValue.Text);
            order.Status = (OrderStatus)cbStatus.SelectedIndex;
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

        /// <summary>
        /// Limit to timespan
        /// </summary>
        private void tbDueTime_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBoxTimeSpan(tbDueTime, ref tbDueTextOld, ref skipTextBoxCheck);
        }
    }
}
