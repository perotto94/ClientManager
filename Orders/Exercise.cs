using System;
using System.Globalization;
using System.Windows;
using System.Windows.Documents;
using System.Collections.Generic;

using static ClientManager.Toolkit;

namespace ClientManager
{
    /// <summary>
    /// Single exercise 
    /// </summary>
    public class Exercise : IDated
    {
        /// <summary>
        /// Reference to parent
        /// </summary>
        public OrderRegular OrderRegular { get; set; } = null;

        DateTime dateStart = DateTime.MinValue;

        /// <summary>
        /// Planned date - start
        /// </summary>
        public DateTime DateStart
        {
            get => dateStart;

            set
            {
                dateStart = value;

                if (OrderRegular != null)
                {
                    OrderRegular.SortExercises();
                }
            }
        }

        /// <summary>
        /// Duration
        /// </summary>
        public TimeSpan Duration { set; get; } = TimeSpan.FromHours(1);

        /// <summary>
        /// Planned date - end
        /// </summary>
        public DateTime DateEnd { get => DateStart + Duration; }

        /// <summary>
        /// Start date for sorting
        /// </summary>
        public DateTime DateRelevant
        {
            get
            {
                if (Status == ExerciseStatus.None ||
                    Status == ExerciseStatus.Canceled ||
                    Status == ExerciseStatus.Completed)
                {
                    return DateTime.MaxValue;
                }

                return DateStart;
            }
        }

        /// <summary>
        /// List of reminders
        /// </summary>
        public List<Reminder> Reminders { get; } = new();

        /// <summary>
        /// Value in rubles
        /// </summary>
        public int Value { get; set; } = 0;

        /// <summary>
        /// Comments on exercise
        /// </summary>
        public string Comments { get; set; } = "";

        ExerciseStatus status = ExerciseStatus.None;

        /// <summary>
        /// Exercise status
        /// </summary>
        public ExerciseStatus Status
        {
            get => status;

            set
            {
                status = value;

                if (OrderRegular != null)
                {
                    OrderRegular.SortExercises();
                }
            }
        }

        /// <summary>
        /// Convert all properties to CSV
        /// </summary>
        public override string ToString()
        {
            return $"{DateStart:yyyy-MM-dd-HH-mm},{Duration:hh\\-mm},{Value:G},{Comments},{Status}";
        }

        /// <summary>
        /// Read all properties from string array
        /// </summary>
        public bool FromString(string[] str)
        {
            if (str.Length != 5)
            {
                return false;
            }

            CultureInfo enUS = new("en-US");

            bool result = DateTime.TryParseExact(str[0], "yyyy-MM-dd-HH-mm", enUS, DateTimeStyles.None, out DateTime sDateStart);
            result &= TimeSpan.TryParseExact(str[1], "hh\\-mm", enUS, TimeSpanStyles.None, out TimeSpan sDuration);
            result &= int.TryParse(str[2], out int sValue);
            result &= FindEnumInString(str[4], out ExerciseStatus sStatus);

            if (!result)
            {
                return false;
            }

            DateStart = sDateStart;
            Duration = sDuration;
            Value = sValue;
            Comments = str[3];
            Status = sStatus;

            return true;
        }

        /// <summary>
        /// Order description for the listbox
        /// </summary>
        public string ShortDescription()
        {
            return $"{DateStart:dd.MM.yyyy HH:mm} / {EnumHelper.GetDescription(Status)}";
        }

        /// <summary>
        /// Flow document for rich text box
        /// </summary>
        public FlowDocument ToFlowDocument()
        {
            Paragraph pDuration = new() { FontSize = 12, LineHeight = 3, TextAlignment = TextAlignment.Left };
            pDuration.Inlines.Add(new Run($"Продолжительность: {Duration:hh\\:mm}"));

            Paragraph pValue = new() { FontSize = 12, LineHeight = 3, TextAlignment = TextAlignment.Left };
            pValue.Inlines.Add(new Run($"Стоимость: {Value:G} руб."));

            Paragraph pComments = new() { FontSize = 12, LineHeight = 3, TextAlignment = TextAlignment.Left };
            pComments.Inlines.Add(new Run($"Комментарии: {Comments}"));

            FlowDocument flowDocument = new();

            flowDocument.Blocks.Add(pDuration);
            flowDocument.Blocks.Add(pValue);
            if (Comments != "") flowDocument.Blocks.Add(pComments);

            return flowDocument;
        }

        public Exercise(bool addReminders = true)
        {
            if (addReminders)
            {
                Reminders.Add(new Reminder(ReminderTimes.Exercise15M));
                Reminders.Add(new Reminder(ReminderTimes.Exercise30M));
                Reminders.Add(new Reminder(ReminderTimes.Exercise1H));
            }
        }
    }
}
