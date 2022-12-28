using System;
using System.Collections.Generic;
using System.Windows.Controls;

namespace ClientManager
{
    public static partial class Toolkit
    {
        /// <summary>
        /// Check if text is timespan and adjust textbox to 23:59 format
        /// </summary>
        public static void TextBoxTimeSpan(TextBox textBox, ref string oldText, ref bool skipCheck)
        {
            if (skipCheck) return;

            skipCheck = true;

            string text = textBox.Text;
            int caretPosition = textBox.CaretIndex;

            if (!(text.Length == 6 && TextIsTimeSpan(text)))
            {
                textBox.Text = oldText; skipCheck = false; textBox.CaretIndex = Math.Max(0, caretPosition - 1); return;
            }

            int newNumber;
            int oldHour;

            if (caretPosition == 1)
            {
                newNumber = text[0] - '0';
                oldHour = text[2] - '0';

                if (newNumber > 2)
                {
                    textBox.Text = oldText; skipCheck = false; textBox.CaretIndex = caretPosition - 1; return;
                }
                else if (newNumber == 2 && oldHour > 3)
                {
                    text = text.Remove(1, 2);
                    text = text.Insert(1, "0");

                    textBox.Text = text; textBox.CaretIndex = 1;
                }
                else
                {
                    textBox.Text = text.Remove(1, 1); textBox.CaretIndex = 1;
                }
            }
            else if (caretPosition == 2)
            {
                newNumber = text[1] - '0';
                oldHour = text[0] - '0';

                if (oldHour == 2 && newNumber > 3)
                {
                    textBox.Text = oldText; skipCheck = false; textBox.CaretIndex = caretPosition - 1; return;
                }
                else
                {
                    textBox.Text = text.Remove(2, 1); textBox.CaretIndex = 3;
                }
            }
            else if (caretPosition == 3)
            {
                newNumber = text[2] - '0';

                if (newNumber > 5)
                {
                    textBox.Text = oldText; skipCheck = false; textBox.CaretIndex = caretPosition - 1; return;
                }
                else
                {
                    text = text.Remove(2, 3);
                    text = text.Insert(2, $":{newNumber:G}");

                    textBox.Text = text; textBox.CaretIndex = 4;
                }
            }
            else if (caretPosition == 4)
            {
                newNumber = text[3] - '0';

                if (newNumber > 5)
                {
                    textBox.Text = oldText; skipCheck = false; textBox.CaretIndex = caretPosition - 1; return;
                }
                else
                {
                    textBox.Text = text.Remove(4, 1); textBox.CaretIndex = 4;
                }
            }
            else if (caretPosition == 5 || caretPosition == 6)
            {
                textBox.Text = text.Remove(5, 1); textBox.CaretIndex = 5;
            }
            else
            {
                textBox.Text = oldText; skipCheck = false; textBox.CaretIndex = caretPosition - 1; return;
            }

            oldText = textBox.Text;

            skipCheck = false;
        }

        /// <summary>
        /// Return if the text only contains digits and one ':' sign in between
        /// </summary>
        private static bool TextIsTimeSpan(string text)
        {
            List<int> columns = new();

            for (int i = 0; i < text.Length; i++)
            {
                bool check = false;

                for (char c = '0'; c <= '9'; c++)
                {
                    if (text[i] == c)
                    {
                        check = true; break;
                    }
                }

                if (check) continue;

                if (text[i] == ':')
                {
                    columns.Add(i);
                }
                else
                {
                    return false;
                }
            }

            return columns.Count == 1 && (columns[0] == 2 || columns[0] == 3);
        }
    }
}
