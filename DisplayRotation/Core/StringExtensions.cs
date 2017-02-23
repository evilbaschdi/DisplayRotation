using System;
using System.ComponentModel;

namespace DisplayRotation.Core
{
    public static class StringExtensions
    {
        /// <summary>
        ///     Bestimmt ob die angegebene Zeichenfolge ein Teil der gegebenen Zeichenfolge ist. Die Überprüfung findet aufgrund
        ///     des gegebenen <see cref="StringComparison" /> statt.
        /// </summary>
        /// <param name="source">Die Zeichenfolge in der gesucht werden soll.</param>
        /// <param name="value">Die zu suchende Zeichenfogle.</param>
        /// <param name="comparisonType">Der Modus der beim Vergleichen angewendet werden soll.</param>
        /// <returns>
        ///     <c>true</c> wenn <paramref name="value" /> in <paramref name="source" /> gefunden wurde; andernfalls
        ///     <c>false</c>
        /// </returns>
        public static bool Contains(this string source, string value, StringComparison comparisonType)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }
            if (!Enum.IsDefined(typeof(StringComparison), comparisonType))
            {
                throw new InvalidEnumArgumentException(nameof(comparisonType), (int) comparisonType, typeof(StringComparison));
            }
            return source.IndexOf(value, comparisonType) >= 0;
        }
    }
}