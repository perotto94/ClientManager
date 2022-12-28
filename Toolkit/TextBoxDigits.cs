using System.Windows.Controls;

namespace ClientManager
{
    public static partial class Toolkit
    {
        /// <summary>
        /// Check if text contains only digits and adjust
        /// </summary>
        public static void TextBoxDigits(TextBox textBox, ref bool skipCheck)
        {
            if (skipCheck) return;

            skipCheck = true;

            int caretIndex = textBox.CaretIndex;

            string text = textBox.Text;

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

            if (textBox.Text == "")
            {
                textBox.Text = "0";
            }
            else
            {
                textBox.Text = text;
            }

            if (caretIndex > -1)
            {
                textBox.CaretIndex = caretIndex;
            }

            skipCheck = false;
        }
    }
}
