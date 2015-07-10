using System;
using System.Threading;

namespace Free.Controls
{
	internal static class ClientUtils
	{
		public static bool IsEnumValid(Enum enumValue, int value, int minValue, int maxValue)
		{
			return ((value>=minValue)&&(value<=maxValue));
		}

		public static bool IsCriticalException(Exception ex)
		{
			return (ex is NullReferenceException)||(ex is StackOverflowException)||(ex is OutOfMemoryException)||(ex is ThreadAbortException)||(ex is IndexOutOfRangeException)||(ex is AccessViolationException);
		}
	}
}
