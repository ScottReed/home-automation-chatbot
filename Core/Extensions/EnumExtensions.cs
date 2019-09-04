using System;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace Core.Extensions
{
    /// <summary>
    /// Extensions for an enm.
    /// </summary>
    public static class EnumExtensions
    {
        /// <summary>
        /// Gets the enum description.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>System.String.</returns>
        public static string GetEnumDescription(this Enum value)
        {
            FieldInfo fi = value.GetType().GetField(value.ToString());

            DescriptionAttribute[] attributes = fi.GetCustomAttributes(typeof(DescriptionAttribute), false) as DescriptionAttribute[];

            if (attributes != null && attributes.Any())
            {
                return attributes.First().Description;
            }

            return value.ToString();
        }
    }
}