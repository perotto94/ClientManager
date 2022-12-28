using System;
using System.Collections.Generic;
using System.Globalization;

namespace ClientManager
{
    /// <summary>
    /// Event for the date
    /// </summary>
    public class PlannedEvent : IDated
    {
        /// <summary>
        /// Reference to parent
        /// </summary>
        public Database Database { get; set; } = null;

        /// <summary>
        /// Planned date - start
        /// </summary>
        public DateTime DateStart { set; get; } = DateTime.MinValue;

        /// <summary>
        /// Start date for sorting
        /// </summary>
        public DateTime DateRelevant { get => DateStart; }

        /// <summary>
        /// List of reminders
        /// </summary>
        public List<Reminder> Reminders { get; } = new();

        /// <summary>
        /// Duration
        /// </summary>
        public TimeSpan Duration { set; get; } = TimeSpan.FromHours(1);

        /// <summary>
        /// Planned date - end
        /// </summary>
        public DateTime DateEnd { get => DateStart + Duration; }

        /// <summary>
        /// Description of the event
        /// </summary>
        public string Description { set; get; } = "";

        /// <summary>
        /// Convert all properties to CSV
        /// </summary>
        public override string ToString()
        {
            return $"{DateStart:yyyy-MM-dd-HH-mm},{Duration:hh\\-mm},{Description}";
        }

        /// <summary>
        /// Read all properties from string array
        /// </summary>
        public bool FromString(string[] str)
        {
            if (str.Length != 3)
            {
                return false;
            }

            CultureInfo enUS = new("en-US");

            bool result = DateTime.TryParseExact(str[0], "yyyy-MM-dd-HH-mm", enUS, DateTimeStyles.None, out DateTime sDateStart);
            result &= TimeSpan.TryParseExact(str[1], "hh\\-mm", enUS, TimeSpanStyles.None, out TimeSpan sDuration);

            if (!result)
            {
                return false;
            }

            DateStart = sDateStart;
            Duration = sDuration;
            Description = str[2];

            return true;
        }

        public PlannedEvent(bool addReminders = true)
        {
            if (addReminders)
            {
                Reminders.Add(new Reminder(ReminderTimes.Event1H));
                Reminders.Add(new Reminder(ReminderTimes.Event2H));
            }
        }
    }
}
