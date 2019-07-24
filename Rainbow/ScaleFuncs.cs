using System;

namespace Rainbow
{
	public static class ScaleFuncs
	{
		public static double Lineal(double value) => value;

		public static double _20Log10(double value) =>
			value > 0d ? +20d * Math.Log(+value * ushort.MaxValue, 10d) :
			value < 0d ? -20d * Math.Log(-value * ushort.MaxValue, 10d) :
			0d;

		public static double Log2(double value) =>
			value > 0d ? +Math.Log(+value, 2d) :
			value < 0d ? -Math.Log(-value, 2d) :
			0d;

		public static double Log(double value) =>
			value > 0d ? +Math.Log(+value) :
			value < 0d ? -Math.Log(-value) :
			0d;

		public static double Exp(double value) =>
			value > 0d ? +Math.Exp(+value) :
			value < 0d ? -Math.Exp(-value) :
			0d;

		public static double Sqrt(double value) =>
			value > 0d ? +Math.Sqrt(+value) :
			value < 0d ? -Math.Sqrt(-value) :
			0d;
	}
}
