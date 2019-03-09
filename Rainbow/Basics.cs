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
	}
}
