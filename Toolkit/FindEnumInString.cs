using System;

namespace ClientManager
{
    /// <summary>
    /// Useful methods
    /// </summary>
    public static partial class Toolkit
    {
        /// <summary>
        /// <para> Search if string contains enum </para>
        /// <para> Returns if enum was found </para>
        /// </summary>
        public static bool FindEnumInString<T>(string str, out T resultEnum) where T : Enum
        {
            string[] enumNames = Enum.GetNames(typeof(T));

            for (int i = 0; i < enumNames.Length; i++)
            {
                if (str.Contains(enumNames[i], StringComparison.OrdinalIgnoreCase))
                {
                    resultEnum = (T)Enum.GetValues(typeof(T)).GetValue(i);

                    return true;
                }
            }

            resultEnum = (T)Enum.GetValues(typeof(T)).GetValue(0);

            return false;
        }
    }
}
