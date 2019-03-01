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

		public static List<Complex> GetJoinedSpectrum(
			IList<Complex> spectrum0, IList<Complex> spectrum1,
			double shiftsPerFrame, double sampleRate)
		{
			var frameSize = spectrum0.Count;
			var frameTime = frameSize / sampleRate;
			var shiftTime = frameTime / shiftsPerFrame;
			var binToFrequancy = sampleRate / frameSize;
			var items = new List<Complex>();

			for (var bin = 0; bin < frameSize; bin++)
			{
				var omegaExpected = DoublePi * (bin * binToFrequancy); // ω=2πf
				var omegaActual = (spectrum1[bin].Phase - spectrum0[bin].Phase) / shiftTime; // ω=∂φ/∂t
				var omegaDelta = Align(omegaActual - omegaExpected, DoublePi); // Δω=(∂ω + π)%2π - π
				var binDelta = omegaDelta / (DoublePi * binToFrequancy);
				var frequancyActual = (bin + binDelta) * binToFrequancy;
				var magnitude = spectrum1[bin].Magnitude + spectrum0[bin].Magnitude;
				var item = new Complex(frequancyActual, magnitude * (0.5 + Math.Abs(binDelta)));
				items.Add(item);
			}

			return items;
		}

		public static int InvertSign(this int d, bool negate) => negate ? -d : +d;

		public static double Align(double angle, double period)
		{
			var qpd = (int)(angle / period);
			qpd += (qpd & 1).InvertSign(qpd < 0);
			angle -= period * qpd;
			return angle;
		}

		private static void Deconstruct(this Complex value, out double real, out double imaginary)
		{
			real = value.Real;
			imaginary = value.Imaginary;
		}

		public static List<Complex> Correct(this List<Complex> data)
		{
			var correctedValues = new List<Complex>();
			var halfStep = (data[1].Real - data[0].Real) / 2;
			var count = data.Count - 4;
			for (var i = 0; i < count; i++)
			{
				data[i + 0].Deconstruct(out var ax, out var ay);
				data[i + 1].Deconstruct(out var bx, out var by);
				data[i + 2].Deconstruct(out var cx, out var cy);
				data[i + 3].Deconstruct(out var dx, out var dy);

				var applyCorrection =
					ay < by && ay < cy && dy < by && dy < cy;

				if (applyCorrection)
				{
					var middle = (bx + cx) / 2;
					var delta = halfStep * (cy - by) / (by + cy);
					var mx = middle + delta;
					var my = (by + cy);

					var lx = ax + (mx - bx);
					var rx = dx + (mx - cx);

					var ly = ay * (cx - mx) / halfStep;
					var ry = dy * (mx - bx) / halfStep;

					correctedValues.Add(new Complex(lx, ly));
					correctedValues.Add(new Complex(mx, my));
					correctedValues.Add(new Complex(rx, ry));

					i += 3;
				}
				else
				{
					correctedValues.Add(new Complex(ax, ay));
				}
			}

			return correctedValues;
		}
	}
}