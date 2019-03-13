using System.Collections.Generic;
using System.Linq;

namespace Rainbow
{
	public static class Basics
	{
		public static double Identity(this double value) => +value;
		public static double Negation(this double value) => -value;

		public static double Increment(this double value, double offset) => value + offset;
		public static double Decrement(this double value, double offset) => value - offset;

		public static double Stretch(this double value, double factor) => value * factor;
		public static double Squeeze(this double value, double factor) => value / factor;

		public static IEnumerable<double> Identity(this IEnumerable<double> values) => values.Select(Identity);
		public static IEnumerable<double> Negation(this IEnumerable<double> values) => values.Select(Identity);

		public static IEnumerable<double> Increment(this IEnumerable<double> values, double offset) => values.Select(v => v.Increment(offset));
		public static IEnumerable<double> Decrement(this IEnumerable<double> values, double offset) => values.Select(v => v.Decrement(offset));

		public static IEnumerable<double> Stretch(this IEnumerable<double> values, double factor) => values.Select(v => v.Stretch(factor));
		public static IEnumerable<double> Squeeze(this IEnumerable<double> values, double factor) => values.Select(v => v.Squeeze(factor));
	}
}
