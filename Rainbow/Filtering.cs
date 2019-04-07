using System;
using System.Collections.Generic;
using System.Linq;

namespace Rainbow
{
	public struct Bin
	{
		public double Frequancy { get; set; }
		public double Magnitude { get; set; }
		public double Phase { get; set; }

		public void Construct(ref double frequancy, ref double magnitude, ref double phase)
		{
			Frequancy = frequancy;
			Magnitude = magnitude;
			Phase = phase;
		}

		public void Deconstruct(out double frequancy, out double magnitude, out double phase)
		{
			frequancy = Frequancy;
			magnitude = Magnitude;
			phase = Phase;
		}
	}

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

		public static IEnumerable<Bin> GetSpectrum(IList<Complex> spectrum, double sampleRate)
		{
			var frameSize = spectrum.Count;
			var binToFrequancyFactor = sampleRate / frameSize;

			for (var bin = 0; bin < frameSize; bin++)
			{
				yield return new Bin
				{
					Phase = spectrum[bin].Phase,
					Magnitude = spectrum[bin].Magnitude,
					Frequancy = bin * binToFrequancyFactor
				};
			}
		}

		public static int InvertSign(this int d, bool negate) => negate ? -d : +d;

		public static double Align(double angle, double period)
		{
			var qpd = (int)(angle / period);
			qpd += (qpd & 1).InvertSign(qpd < 0);
			angle -= period * qpd;
			return angle;
		}

		public static IEnumerable<Bin> Correct(this IList<Bin> data)
		{
			var halfStep = (data[1].Frequancy - data[0].Frequancy) / 2;
			var count = data.Count / 2 - 4;
			for (var i = 0; i < count; i++)
			{
				//var x = i < 0 ? count / 2 : 0;
				data[i + 0].Deconstruct(out var ax, out var ay, out var ap);
				data[i + 1].Deconstruct(out var bx, out var by, out var bp);
				data[i + 2].Deconstruct(out var cx, out var cy, out var cp);
				data[i + 3].Deconstruct(out var dx, out var dy, out var dp);
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

					Bin a = new Bin();
					Bin b = new Bin();
					Bin c = new Bin();
					a.Construct(ref lx, ref ly, ref ap);
					b.Construct(ref mx, ref my, ref bp);
					c.Construct(ref rx, ref ry, ref cp);

					yield return a;
					yield return b;
					yield return c;

					i += 3;
				}
				else
				{
					Bin a = new Bin();
					a.Construct(ref ax, ref ay, ref ap);
					yield return a;
				}
			}
		}
	}
}