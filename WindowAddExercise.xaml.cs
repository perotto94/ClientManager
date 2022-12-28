using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;

using static ClientManager.Toolkit;

namespace ClientManager
{
    /// <summary>
    /// Interaction logic for WindowAddExercise.xaml
    /// </summary>
    public partial class WindowAddExercise : Window
    {
        // Reference to order
        readonly OrderRegular order;

        // Reference to exercise
        readonly Exercise exercise;

        // If order is being edited
        readonly bool ifEdit;

        // If textbox check is allowed
        bool skipTextBoxCheck = true;

        // Old text in timespan text boxes
        string tbStartTextOld, tbDurationTextOld;

        public WindowAddExercise(OrderRegular order, bool ifEdit = false, Exercise exercise = null)
        {
            this.ifEdit = ifEdit;
            this.order = order;

            InitializeComponent();

            // If adding order
            if (!ifEdit)
            {
                this.exercise = new Exercise();

                cbStatus.SelectedIndex = 0;
                tbValue.Text = $"{order.Value:G}";

                DateTime now = DateTime.Now;

                datePickerStartDate.SelectedDate = now;
                tbStartTime.Text = $"{now:HH\\:mm}";
            }

            // If editing order
            else
            {
                buttonDeleteExercise.IsEnabled = true;

                this.exercise = exercise;

                Title = "Редактировать разовое занятие";

                datePickerStartDate.SelectedDate = exercise.DateStart;
                tbStartTime.Text = $"{exercise.DateStart:HH\\:mm}";
                tbDuration.Text = $"{exercise.Duration:hh\\:mm}";
                tbValue.Text = $"{exercise.Value:G}";
                cbStatus.SelectedIndex = (int)exercise.Status;
                tbComments.Text = exercise.Comments;
            }

            tbDurationTextOld = tbDuration.Text;
            tbStartTextOld = tbStartTime.Text;

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
        /// Add exercise
        /// </summary>
        private void buttonAddExercise_Click(object sender, RoutedEventArgs e)
        {
            CultureInfo enUS = new("en-US");

            exercise.DateStart = DateTime.ParseExact(datePickerStartDate.Text, "dd.MM.yyyy", enUS) +
                TimeSpan.ParseExact(tbStartTime.Text, @"hh\:mm", enUS);

            exercise.Duration = TimeSpan.ParseExact(tbDuration.Text, @"hh\:mm", enUS);
            exercise.Value = int.Parse(tbValue.Text);
            exercise.Status = (ExerciseStatus)cbStatus.SelectedIndex;
            exercise.Comments = tbComments.Text;

            if (!ifEdit)
            {
                order.Add(exercise);
            }

            DialogResult = true;

            Close();
        }

        /// <summary>
        /// Delete exercise
        /// </summary>
        private void buttonDeleteExercise_Click(object sender, RoutedEventArgs e)
        {
            order.Exercises.Remove(exercise);

            DialogResult = true;

            Close();
        }

        /// <summary>
        /// Limit box to timespan format
        /// </summary>
        private void tbDuration_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBoxTimeSpan(tbDuration, ref tbDurationTextOld, ref skipTextBoxCheck);
        }

        /// <summary>
        /// Limit box to timespan format
        /// </summary>
        private void tbStartTime_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBoxTimeSpan(tbStartTime, ref tbStartTextOld, ref skipTextBoxCheck);
        }
    }
}
