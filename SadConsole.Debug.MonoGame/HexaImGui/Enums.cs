using System;

namespace Hexa.NET.ImGui;

//public static class ConvertHorizontalAlignment
//{
//    public static string[] Names;

//    static ConvertHorizontalAlignment()
//    {
//        List<string> names = new List<string>();

//        foreach(var item in Enum.GetValues<HorizontalAlignment>())
//            names.Add(Enum.GetName(item));

//        Names = names.ToArray();
//    }
//}

public static class Enums<TEnum> where TEnum : struct, Enum
{
    public static string[] Names = Enum.GetNames<TEnum>();
    public static TEnum[] Values = Enum.GetValues<TEnum>();
    public static int Count => Names.Length;

    public static TEnum GetValueFromIndex(int index) =>
        Values[index];

    public static int GetIndexFromValue(TEnum value) =>
        Array.IndexOf(Values, value);
}
