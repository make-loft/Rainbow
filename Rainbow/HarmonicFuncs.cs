namespace Rainbow
{
	public static class HarmonicFuncs
	{		public static double Align(this in double value, in double period) =>
			value.Decrement(value.Truncate(period)).Decrement(period.InvertSign(value < +0d) / +2d);

		public static double Rectangle(double value) => value.Align(Pi.Double) > -0d ? +1d : -1d;
		public static double Sawtooth(double value) => value.Align(Pi.Double).Squeeze(Pi.Single);
		public static double Triangle(double value) => Sawtooth(value).Abs().Stretch(2d).Decrement(1d);
	}
}
