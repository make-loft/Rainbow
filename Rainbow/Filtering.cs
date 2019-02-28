using System;
using System.Collections.Generic;
using System.Linq;

namespace Rainbow
{
	//  Δ∂ωπ
	public static class Filtering
	{
		public const double SinglePi = Math.PI;
		public const double DoublePi = 2 * Math.PI;

		public static Dictionary<double, double> GetJoinedSpectrum(
			IList<Complex> spectrum0, IList<Complex> spectrum1,
			double shiftsPerFrame, double sampleRate)
		{
			var frameSize = spectrum0.Count;
			var frameTime = frameSize / sampleRate;
			var shiftTime = frameTime / shiftsPerFrame;
			var binToFrequancy = sampleRate / frameSize;
			var dictionary = new Dictionary<double, double>();

			for (var bin = 0; bin < frameSize; bin++)
			{
				var omegaExpected = DoublePi * (bin * binToFrequancy); // ω=2πf
				var omegaActual = (spectrum1[bin].Phase - spectrum0[bin].Phase) / shiftTime; // ω=∂φ/∂t
				var omegaDelta = Align(omegaActual - omegaExpected, DoublePi); // Δω=(∂ω + π)%2π - π
				var binDelta = omegaDelta / (DoublePi * binToFrequancy);
				var frequancyActual = (bin + binDelta) * binToFrequancy;
				var magnitude = spectrum1[bin].Magnitude + spectrum0[bin].Magnitude;
				dictionary.Add(frequancyActual, magnitude * (0.5 + Math.Abs(binDelta)));
			}

			return dictionary;
		}

		public static int InvertSign(this int d, bool negate) => negate ? -d : +d;

		public static double Align(double angle, double period)
		{
			var qpd = (int)(angle / period);
			qpd += (qpd & 1).InvertSign(qpd < 0);
			angle -= period * qpd;
			return angle;
		}

		private static void Calculate(IList<KeyValuePair<double, double>> data, int i, int j,
			out double x0, out double y0,
			out double x1, out double y1,
			out double a, out double b)
		{
			x0 = data[i].Key;
			y0 = data[i].Value;
			x1 = data[j].Key;
			y1 = data[j].Value;

			a = (y1 - y0) / (x1 - x0);
			b = y0 - a * x0;
		}

		public static Dictionary<double, double> Antialiasing(Dictionary<double, double> spectrum)
		{
			var data = spectrum.ToArray();
			var result = new Dictionary<double, double>();
			for (var i = 0; i < data.Length - 4; i++)
			{
				Calculate(data, i + 0, i + 1, out var xa, out var ya, out var xb, out var yb, out var a, out var b);
				Calculate(data, i + 2, i + 3, out var xc, out var yc, out var xd, out var yd, out var c, out var d);

				var x = (d - b) / (a - c);
				var y = (a * d - b * c) / (a - c);

				result.Add(xb, yb);

				if (y > ya && y > yb && y > yc && y > yd &&
					x > xa && x > xb && x < xc && x < xd)
				{
					result.Add(x, y);
				}
			}

			return result;
		}
	}
}