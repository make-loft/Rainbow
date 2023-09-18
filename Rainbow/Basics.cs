using System;
using System.Collections.Generic;
using System.Linq;

namespace Rainbow
{
	public delegate double Projection(double value);

	public static class Basics
	{
		public static double Identity(this in double value) => +value;
		public static double Negation(this in double value) => -value;

		public static double Shift(this in double value, in double offset) => value + offset;
		public static double Scale(this in double value, in double factor) => value * factor;

		public static double Increment(this in double value, in double offset) => value + offset;
		public static double Decrement(this in double value, in double offset) => value - offset;

		public static double Stretch(this in double value, in double factor) => value * factor;
		public static double Squeeze(this in double value, in double factor) => value / factor;

		public static IEnumerable<double> Identity(this IEnumerable<double> values) => values.Select(v => v.Identity());
		public static IEnumerable<double> Negation(this IEnumerable<double> values) => values.Select(v => v.Negation());

		public static IEnumerable<double> Shift(this IEnumerable<double> values, double offset) => values.Select(v => v.Shift(offset));
		public static IEnumerable<double> Scale(this IEnumerable<double> values, double factor) => values.Select(v => v.Scale(factor));

		public static IEnumerable<double> Increment(this IEnumerable<double> values, double offset) => values.Select(v => v.Increment(offset));
		public static IEnumerable<double> Decrement(this IEnumerable<double> values, double offset) => values.Select(v => v.Decrement(offset));

		public static IEnumerable<double> Stretch(this IEnumerable<double> values, double factor) => values.Select(v => v.Stretch(factor));
		public static IEnumerable<double> Squeeze(this IEnumerable<double> values, double factor) => values.Select(v => v.Squeeze(factor));

		public static double Project(this double value, Projection projection) => projection is null ? value : projection(value);
		public static double Abs(this in double value) => value > -0 ? +value : -value;
		public static double Mod(this in double value, in double module = +1d) => value % module;
		public static double Truncate(this in double value, in double module = +1d) => value - value % module;
		public static double InvertSign(this in double d, bool negate) => negate ? -d : +d;

		public static bool HitInterval(this double value,
			double from = double.NegativeInfinity,
			double till = double.PositiveInfinity,
			HitMode mode = HitMode.OpenOpen) => mode switch
			{
				HitMode.OpenOpen => value.HitIntervalOpenOpen(from, till),
				HitMode.OpenClose => value.HitIntervalOpenClose(from, till),
				HitMode.CloseOpen => value.HitIntervalCloseOpen(from, till),
				HitMode.CloseClose => value.HitIntervalCloseClose(from, till),
				_ => throw new NotImplementedException(),
			};

		public static bool HitIntervalOpenOpen(this double value, double from, double till) => from < till
			? from < value && value < till
			: from < value || value < till
			;

		public static bool HitIntervalCloseOpen(this double value, double from, double till) => from < till
			? from <= value && value < till
			: from <= value || value < till
			;

		public static bool HitIntervalOpenClose(this double value, double from, double till) => from < till
			? from < value && value <= till
			: from < value || value <= till
			;

		public static bool HitIntervalCloseClose(this double value, double from, double till) => from < till
			? from <= value && value <= till
			: from <= value || value <= till
			;
	}

	public enum HitMode { OpenOpen, OpenClose, CloseOpen, CloseClose };
}
