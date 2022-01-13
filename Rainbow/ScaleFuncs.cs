using System;

namespace Rainbow
{
	public static class ScaleFuncs
	{
		public static double Lineal(double value) => value;

		public static double _20Log10(double value) =>
			value > 0d ? +Math.Log(+value * ushort.MaxValue, 10d) * 20d :
			value < 0d ? -Math.Log(-value * ushort.MaxValue, 10d) * 20d :
			value;

		public static double _10Pow20(double value) =>
			value > 0d ? +Math.Pow(+value * ushort.MaxValue, 20d) / 10d :
			value < 0d ? -Math.Pow(-value * ushort.MaxValue, 20d) / 10d :
			value;

		public static double Log2(double value) =>
			value > 0d ? +Math.Log(+value, 2d) :
			value < 0d ? -Math.Log(-value, 2d) :
			value;

		public static double _2Pow(double value) =>
			value > 0d ? +Math.Pow(2, +value) :
			value < 0d ? -Math.Pow(2, -value) :
			value;

		public static double Log(double value) =>
			value > 0d ? +Math.Log(+value) :
			value < 0d ? -Math.Log(-value) :
			value;

		public static double Exp(double value) =>
			value > 0d ? +Math.Exp(+value) :
			value < 0d ? -Math.Exp(-value) :
			value;

		public static double Sqrt(double value) =>
			value > 0d ? +Math.Sqrt(+value) :
			value < 0d ? -Math.Sqrt(-value) :
			value;

		public static double Pow2(double value) =>
			value > 0d ? +value * value :
			value < 0d ? -value * value :
			value;
	}
}
