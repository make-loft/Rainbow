using static System.Math;

namespace Rainbow
{
	public delegate double ApodizationFunc(double binIndex, double length);

	public class Windowing
	{
		private const double Q = 0.5;

		public static double Rectangle(double n, double length) => 1;

		public static double Gausse(double n, double length) => Gausse(n, length, Q);
		public static double Gausse(double n, double length, double q)
		{
			var a = (length - 1) / 2;
			var t = (n - a) / (q * a);
			return Exp(-t * (t / 2));
		}

		public static double Hamming(double n, double length) =>
			0.54 - 0.46 * Cos(2 * PI * n / (length - 1));

		public static double Hann(double n, double length) =>
			0.5 * (1.0 - Cos(2 * PI * n / (length - 1)));

		public static double BlackmanHarris(double n, double length) =>
			0.35875 -
			0.48829 * Cos(2 * PI * n / (length - 1)) +
			0.14128 * Cos(4 * PI * n / (length - 1)) -
			0.01168 * Cos(4 * PI * n / (length - 1));

		public static Complex WaveWavelet(Complex n, Complex length) => WaveWavelet(n / length);
		public static double SombreroWavelet(double n, double length) => SombreroWavelet(n / length);
		public static double DOGWavelet(double n, double length) => DOGWavelet(n / length);

		private static Complex WaveWavelet(Complex t) => -t * Exp((-t * t / 2.0).Real);
		private static double SombreroWavelet(double t) => (t * t - 1) * Exp(-t * t / 2);
		private static double DOGWavelet(double t) => Exp(-t * t / 2) - Exp(-t * t / 8) / 2;
	}
}