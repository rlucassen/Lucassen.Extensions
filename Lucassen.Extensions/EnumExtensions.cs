﻿namespace Lucassen.Extensions
{
    using System;
    using System.ComponentModel;

    public static class EnumExtensions
    {
        public static string GetDescription(this object value)
        {
            var type = value.GetType();
            var name = Enum.GetName(type, value);
            if (name != null)
            {
                var field = type.GetField(name);
                if (field != null)
                {
                    if (Attribute.GetCustomAttribute(field,
                        typeof(DescriptionAttribute)) is DescriptionAttribute attr) return attr.Description;
                }
            }

            return null;
        }
    }
}