using System;
using System.ComponentModel;

namespace ClientManager
{
    /// <summary>
    /// Reminder times
    /// </summary>
    public enum ReminderTimes
    {
        [Description("reminder_event_1h")]
        Event1H,

        [Description("reminder_event_2h")]
        Event2H,

        [Description("reminder_exercise_15m")]
        Exercise15M,

        [Description("reminder_exercise_30m")]
        Exercise30M,

        [Description("reminder_exercise_1h")]
        Exercise1H,

        [Description("")]
        None
    }

    /// <summary>
    /// Reminder for the event
    /// </summary>
    public class Reminder
    {
        /// <summary>
        /// Time for reminder
        /// </summary>
        public ReminderTimes ReminderTime { set; get; } = ReminderTimes.None;

        /// <summary>
        /// Happened or not
        /// </summary>
        public bool Happened { set; get; } = false;

        /// <summary>
        /// Sound file name by enum
        /// </summary>
        public string SoundFileName
        {
            get => EnumHelper.GetDescription(ReminderTime);
        }

        public Reminder(ReminderTimes reminderTime)
        {
            ReminderTime = reminderTime;
        }

        public TimeSpan TimeToEvent
        {
            get
            {
                switch (ReminderTime)
                {
                    case ReminderTimes.Exercise15M: return TimeSpan.FromMinutes(15);
                    case ReminderTimes.Exercise30M: return TimeSpan.FromMinutes(30);
                    case ReminderTimes.Exercise1H: return TimeSpan.FromHours(1);
                    case ReminderTimes.Event1H: return TimeSpan.FromHours(1);
                    case ReminderTimes.Event2H: return TimeSpan.FromHours(2);
                    default: return TimeSpan.Zero;
                }
            }
        }
    }
}
