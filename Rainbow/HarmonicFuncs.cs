using System;
using System.Collections.Generic;
using System.Text;

namespace Rainbow
{
	public static class HarmonicFuncs
	{
		public static double Dirac(double value)
		{
			value = Impulse(value);
			return value == 0d ? 1d : 0d;
		}

		public static double Impulse(double value)
		{
			var periods = (long)(value / Pi.Single);
			value = (value - periods * Pi.Single) - 1d;
			return
				value > 0.5d ? +1 :
				value < 0.5d ? -1 :
				0d;
		}

		public static double Triangle(double value) =>
			value.Truncate(Pi.Double).Decrement(value).Negation() > Pi.Single
			? +value.Align(Pi.Single)
			: -value.Align(Pi.Single);

		public static double Identity(double value) => +value.Align(Pi.Double);
		public static double Negation(double value) => -value.Align(Pi.Double);

		public static double Mod(this in double value, in double module = +1d) => value % module;
		public static double Truncate(this in double value, in double module = +1d) => value - value % module;
		public static double Align(this in double value, in double period) =>
			value.Truncate(period).Decrement(value).Negation().Decrement(period / 2d);
	}
}
