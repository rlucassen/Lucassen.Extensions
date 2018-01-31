namespace PwCPPP.Model.Helpers
{
    using System;
    using System.Collections.Generic;
    using Lucassen.Extensions;

    public class EnumHelper
    {
        public static IList<EnumItem> EnumToList(Type enumType)
        {
            if (enumType.BaseType != typeof(Enum))
                throw new ArgumentException("T must be of type System.Enum");

            var enumValArray = Enum.GetValues(enumType);

            var enumValList = new List<EnumItem>();

            foreach (Enum val in enumValArray)
            {
                enumValList.Add(new EnumItem() { Enum = Enum.Parse(enumType, val.ToString()), Value = val.GetHashCode(), Name = val.ToString(), Readable = val.GetDescription() });
            }

            return enumValList;
        }

        public class EnumItem
        {
            public object Enum { get; set; }
            public int Value { get; set; }
            public string Name { get; set; }
            public string Readable { get; set; }
        }
    }
}