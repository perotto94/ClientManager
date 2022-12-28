using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;

using static ClientManager.Toolkit;

namespace ClientManager
{
    /// <summary>
    /// Interaction logic for WindowAddEvent.xaml
    /// </summary>
    public partial class WindowAddEvent : Window
    {
        // Reference to global vars
        readonly List<Database> dbList;

        // Reference to event
        readonly PlannedEvent plannedEvent;

        // If event is being edited
        readonly bool ifEdit;

        // If edit event, find its database index
        readonly int editDBIndex;

        // If edit event, find its index in database
        readonly int editEventIndex;

        // Time boxes variables
        bool skipTimeCheck = true;

        // Old text in timespan boxes
        string tbTimeStartText, tbDurationText;

        /// <summary>
        /// Add or edit an event
        /// </summary>
        public WindowAddEvent(GlobalVars globalVars, bool ifEdit = false, PlannedEvent plannedEvent = null)
        {
            dbList = globalVars.DB;
            this.ifEdit = ifEdit;

            InitializeComponent();

            // Initialize databases combobox
            foreach (Database db in globalVars.DB)
            {
                cbDatabase.Items.Add(db.Name);
            }

            // If adding event
            if (!ifEdit)
            {
                this.plannedEvent = new PlannedEvent();

                cbDatabase.SelectedIndex = 0;

                DateTime now = DateTime.Now;

                datePickerStart.SelectedDate = now;
                tbTimeStart.Text = $"{now:HH:mm}";
            }

            // If editing event
            else
            {
                this.plannedEvent = plannedEvent;

                Title = "Редактировать событие";

                for (editDBIndex = 0;
                    editDBIndex < dbList.Count &&
                    dbList[editDBIndex] != plannedEvent.Database;
                    editDBIndex++) ;

                for (editEventIndex = 0;
                    editEventIndex < dbList[editDBIndex].Events.Count &&
                    dbList[editDBIndex].Events[editEventIndex] != plannedEvent;
                    editEventIndex++) ;

                cbDatabase.SelectedIndex = editDBIndex;

                datePickerStart.SelectedDate = plannedEvent.DateStart;
                tbTimeStart.Text = $"{plannedEvent.DateStart:HH:mm}";

                tbDuration.Text = $"{plannedEvent.Duration:hh\\:mm}";

                tbDescription.Text = plannedEvent.Description;

                buttonDelete.IsEnabled = true;
            }

            tbTimeStartText = tbTimeStart.Text;
            tbDurationText = tbDuration.Text;

            skipTimeCheck = false;
        }

        /// <summary>
        /// Edit time in TimeStart textbox
        /// </summary>
        private void tbTimeStart_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBoxTimeSpan(tbTimeStart, ref tbTimeStartText, ref skipTimeCheck);
        }

        /// <summary>
        /// Edit time in Duration textbox
        /// </summary>
        private void tbDuration_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBoxTimeSpan(tbDuration, ref tbDurationText, ref skipTimeCheck);
        }

        /// <summary>
        /// Save event
        /// </summary>
        private void buttonSave_Click(object sender, RoutedEventArgs e)
        {
            CultureInfo enUS = new("en-US");

            plannedEvent.DateStart = DateTime.ParseExact(datePickerStart.Text, "dd.MM.yyyy", enUS) +
                TimeSpan.ParseExact(tbTimeStart.Text, @"hh\:mm", enUS);

            plannedEvent.Duration = TimeSpan.ParseExact(tbDuration.Text, @"hh\:mm", enUS);
            plannedEvent.Description = tbDescription.Text;

            // Just add event to selected DB
            if (!ifEdit)
            {
                plannedEvent.Database = dbList[cbDatabase.SelectedIndex];

                dbList[cbDatabase.SelectedIndex].Events.Add(plannedEvent);
            }

            // If DB was changed, delete event and add to selected DB
            else if (cbDatabase.SelectedIndex != editDBIndex)
            {
                dbList[cbDatabase.SelectedIndex].Events.Add(plannedEvent);

                dbList[editDBIndex].Events.RemoveAt(editEventIndex);
            }

            DialogResult = true;

            Close();
        }

        /// <summary>
        /// Delete event
        /// </summary>
        private void buttonDelete_Click(object sender, RoutedEventArgs e)
        {
            Database db = plannedEvent.Database;
            db.Events.Remove(plannedEvent);

            DialogResult = true;

            Close();
        }
    }
}
