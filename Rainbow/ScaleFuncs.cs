using System;

namespace Rainbow
{
	public delegate double ScaleFunc(double value);

	public static class ScaleFuncs
	{
		public static double Lineal(double value) => value;
		public static double _20Log10(double value) => value > 0d ? 20d * Math.Log(value, 10d) : value;
		public static double Log2(double value) => value > 0d ? Math.Log(value, 2d) : value;
		public static double Log(double value) => value > 0d ? Math.Log(value) : value;
		public static double Exp(double value) => Math.Exp(value);
	}
}
