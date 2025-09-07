using System;
using System.Collections.Generic;
using System.Linq;

namespace BackpackSurvivors.System;

public static class FlagExtensions
{
	public static TEnum AllFlags<TEnum>(this TEnum @enum) where TEnum : struct
	{
		Type typeFromHandle = typeof(TEnum);
		long num = 0L;
		foreach (object value in Enum.GetValues(typeFromHandle))
		{
			long num2 = (long)Convert.ChangeType(value, TypeCode.Int64);
			if (num2 == 1 || num2 % 2 == 0L)
			{
				num |= num2;
			}
		}
		return (TEnum)Enum.ToObject(typeFromHandle, num);
	}

	public static IEnumerable<Enum> GetUniqueFlags(this Enum flags)
	{
		ulong flag = 1uL;
		foreach (Enum item in Enum.GetValues(flags.GetType()).Cast<Enum>())
		{
			if (Convert.ToInt64(item) != -1)
			{
				ulong num = Convert.ToUInt64(item);
				while (flag < num)
				{
					flag <<= 1;
				}
				if (flag == num && flags.HasFlag(item))
				{
					yield return item;
				}
			}
		}
	}

	public static IEnumerable<T> ListFlags<T>(this T value) where T : struct, Enum
	{
		if (!Attribute.IsDefined(typeof(T), typeof(FlagsAttribute)))
		{
			yield return value;
			yield break;
		}
		T[] values = (T[])Enum.GetValues(typeof(T));
		for (int index = 0; index < values.Length; index++)
		{
			T val = values[index];
			if (!val.Equals(default(T)) && value.HasFlag(val))
			{
				yield return val;
			}
		}
	}
}
