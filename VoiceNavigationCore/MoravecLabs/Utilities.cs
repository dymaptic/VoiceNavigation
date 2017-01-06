using System;
namespace MoravecLabs
{
	public static class Utilities
	{
		public static int StringNumberToInteger(string number)
		{
			try
			{
				return Convert.ToInt32(number);
			}
			catch
			{
			}

			switch (number)
			{
				case "first":
					return 1;
				case "second":
					return 2;
				case "third":
					return 3;
				case "one":
					return 1;
				case "two":
					return 2;
				case "three":
					return 3;
				default:
					throw new InvalidCastException("Unable to convert string to number");
			}
		}
	}
}
