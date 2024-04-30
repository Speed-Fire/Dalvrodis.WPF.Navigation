using System;

#nullable disable

namespace Synergy.WPF.Navigation.Extensions
{
	internal static class TypeExtensions
	{
		internal static bool IsDerivedFromGenericType(this Type toCheck, Type generic)
		{
			while (toCheck != null && toCheck.IsClass && toCheck != typeof(object))
			{
				var cur = toCheck.IsGenericType ? toCheck.GetGenericTypeDefinition() : toCheck;

				if (generic == cur)
				{
					return true;
				}

				toCheck = toCheck.BaseType;
			}
			return false;
		}
	}
}
