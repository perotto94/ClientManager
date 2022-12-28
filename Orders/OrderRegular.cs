using System;
using System.Collections.Generic;
using System.Windows.Documents;
using System.Windows;

using static ClientManager.Toolkit;

namespace ClientManager
{
    /// <summary>
    /// Regular order
    /// </summary>
    public class OrderRegular : Order
    {
        /// <summary>
        /// Standard value in rubles
        /// </summary>
        public int Value { get; set; } = 0;

        /// <summary>
        /// List of exercises
        /// </summary>
        public List<Exercise> Exercises { get; } = new();

        /// <summary>
        /// Get i-th exercise
        /// </summary>
        public Exercise this[int i]
        {
            get => Exercises[i];
        }

        /// <summary>
        /// Get most relevant exercise datetime
        /// </summary>
        public override DateTime DateRelevant { get => Exercises.Count > 0 ? Exercises[0].DateRelevant : DateTime.MaxValue; }

        /// <summary>
        /// Add exercise with reference
        /// </summary>
        public void Add(Exercise exercise)
        {
            exercise.OrderRegular = this;

            Exercises.Add(exercise);

            SortExercises();
        }

        /// <summary>
        /// Sort exercises by date
        /// </summary>
        public void SortExercises()
        {
            SortByDate(Exercises);

            if (Client != null)
            {
                Client.SortOrders();
            }
        }

        /// <summary>
        /// Total value of all exercises
        /// </summary>
        public int ValueTotal(bool completedOnly = true)
        {
            int result = 0;

            foreach (Exercise exercise in Exercises)
            {
                if (completedOnly && exercise.Status != ExerciseStatus.Completed)
                {
                    continue;
                }

                result += exercise.Value;
            }

            return result;
        }

        public override string ShortDescription()
        {
            string result = Subject;

            if (Topic != "") result += " / " + Topic;

            return result;
        }

        /// <summary>
        /// Convert all properties to CSV
        /// </summary>
        public override string ToString()
        {
            return $"{Value:G},{Subject},{Topic},{Comments},{Exercises.Count}";
        }

        /// <summary>
        /// Read all properties from string array
        /// </summary>
        public bool FromString(string[] str, out int exerciseCount)
        {
            exerciseCount = 0;

            if (str.Length != 5)
            {
                return false;
            }

            bool result = int.TryParse(str[0], out int sValue);
            result &= int.TryParse(str[4], out exerciseCount);

            if (!result)
            {
                return false;
            }

            Value = sValue;
            Subject = str[1];
            Topic = str[2];
            Comments = str[3];

            return true;
        }

        public override FlowDocument ToFlowDocument()
        {
            string nextExercise = DateRelevant == DateTime.MaxValue ? "нет" : $"{DateRelevant:dd.MM.yyyy HH:mm}";

            Paragraph pDueDate = new() { FontSize = 12, LineHeight = 3, TextAlignment = TextAlignment.Left };
            pDueDate.Inlines.Add(new Bold(new Run($"След. занятие: {nextExercise}")));

            Paragraph pValue = new() { FontSize = 12, LineHeight = 3, TextAlignment = TextAlignment.Left };
            pValue.Inlines.Add(new Run($"За занятие: {Value:G} руб."));

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
