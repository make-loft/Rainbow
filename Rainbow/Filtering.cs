using System;
using System.Collections.Generic;
using System.Linq;

namespace Rainbow
{
	//  Δ∂ωπ
	public static class Filtering
	{
		public static List<Complex> GetJoinedSpectrum(
			IList<Complex> spectrum0, IList<Complex> spectrum1,
			double shiftsPerFrame, double sampleRate)
		{
			var frameSize = spectrum0.Count;
			var frameTime = frameSize / sampleRate;
			var shiftTime = frameTime / shiftsPerFrame;
			var binToFrequancy = sampleRate / frameSize;
			var items = new List<Complex>();
			var binsCount = frameSize / 2;

			for (var bin = 0; bin < binsCount; bin++)
			{
				var omegaExpected = Pi.Double * (bin * binToFrequancy); // ω=2πf
				var omegaActual = (spectrum1[bin].Phase - spectrum0[bin].Phase) / shiftTime; // ω=∂φ/∂t
				var omegaDelta = Align(omegaActual - omegaExpected, Pi.Double); // Δω=(∂ω + π)%2π - π
				var binDelta = omegaDelta / (Pi.Double * binToFrequancy);
				var frequancyActual = (bin + binDelta) * binToFrequancy;
				var magnitude = (spectrum1[bin].Magnitude + spectrum0[bin].Magnitude) / 2d;
				var item = new Complex(frequancyActual, magnitude * (0.5 + Math.Abs(binDelta)));
				items.Add(item);
			}

			return items;
		}


		public static List<Complex> GetSpectrum(IList<Complex> spectrum, double sampleRate)
		{
			var frameSize = spectrum.Count;
			var binToFrequancy = sampleRate / frameSize;
			var items = new List<Complex>();
			var binsCount = frameSize;

			for (var bin = 0; bin < binsCount; bin++)
			{
				var frequancyActual = bin * binToFrequancy;
				var magnitude = spectrum[bin].Magnitude;
				var item = new Complex(frequancyActual, magnitude);
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

		public static List<Complex> Correct(this List<Complex> data)
		{
			var correctedValues = new List<Complex>();
			var halfStep = (data[1].Real - data[0].Real) / 2;
			var count = data.Count / 2 - 4;
			for (var i = 0; i < count; i++)
			{
				//var x = i < 0 ? count / 2 : 0;
				data[i + 0].Deconstruct(out var ax, out var ay);
				data[i + 1].Deconstruct(out var bx, out var by);
				data[i + 2].Deconstruct(out var cx, out var cy);
				data[i + 3].Deconstruct(out var dx, out var dy);
				//ax = i < 0 ? bx - cx : ax;

				var magicFactor = cx / dx; /* for better accuracy, but why? */
				var applyCorrection =
					ay < by && ay < cy
					&&
					dy < cy && dy * magicFactor < by;

				//var max0 = ay > by ? ay : by;
				//var max1 = cy > dy ? cy : dy;
				//var max = max0 > max1 ? max0 : max1;
				//var my = (by + cy) - (ay + dy) / Pi.Half;
				//var applyCorrection = my > max * magicFactor;

				if (applyCorrection)
				{
					var my = (by + cy) - (ay + dy) / Pi.Half;
					var middle = (bx + cx) / 2;
					var delta = (cy - by) / (cy + by);
					var mx = middle + delta * halfStep;

					var lx = ax + (mx - bx);
					var rx = dx + (mx - cx);

					var ly = ay; //* (cx - mx) / halfStep;
					var ry = dy; //* (mx - bx) / halfStep;

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