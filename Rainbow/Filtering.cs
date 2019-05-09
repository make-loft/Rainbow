using System;
using System.Collections.Generic;
using System.Linq;

namespace Rainbow
{
	public struct Bin
	{
		public double Frequency { get; set; }
		public double Magnitude { get; set; }
		public double Phase { get; set; }

		public void Construct(in double frequency, in double magnitude, in double phase)
		{
			Frequency = frequency;
			Magnitude = magnitude;
			Phase = phase;
		}

		public void Deconstruct(out double frequency, out double magnitude, out double phase)
		{
			frequency = Frequency;
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
			var binToFrequency = sampleRate / frameSize;
			var items = new List<Complex>();
			var binsCount = frameSize / 2;

			for (var bin = 0; bin < binsCount; bin++)
			{
				var omegaExpected = Pi.Double * (bin * binToFrequency); // ω=2πf
				var omegaActual = (spectrum1[bin].Phase - spectrum0[bin].Phase) / shiftTime; // ω=∂φ/∂t
				var omegaDelta = Align(omegaActual - omegaExpected, Pi.Double); // Δω=(∂ω + π)%2π - π
				var binDelta = omegaDelta / (Pi.Double * binToFrequency);
				var frequencyActual = (bin + binDelta) * binToFrequency;
				var magnitude = (spectrum1[bin].Magnitude + spectrum0[bin].Magnitude) / 2d;
				var item = new Complex(frequencyActual, magnitude * (0.5 + Math.Abs(binDelta)));
				items.Add(item);
			}

			return items;
		}

		public static IEnumerable<Bin> GetSpectrum(IList<Complex> spectrum, double sampleRate)
		{
			var frameSize = spectrum.Count;
			var binToFrequencyFactor = sampleRate / frameSize;

			for (var bin = 0; bin < frameSize; bin++)
			{
				yield return new Bin
				{
					Phase = spectrum[bin].Phase,
					Magnitude = spectrum[bin].Magnitude,
					Frequency = bin * binToFrequencyFactor
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

		public static IEnumerable<Bin> EnumeratePeaks(this IList<Bin> spectrum)
		{
			var count = spectrum.Count / 2 - 3;
			for (var i = 0; i < count; i++)
			{
				spectrum[i + 0].Deconstruct(out var ax, out var ay, out var ap);
				spectrum[i + 1].Deconstruct(out var bx, out var by, out var bp);
				spectrum[i + 2].Deconstruct(out var cx, out var cy, out var cp);
				if (ay < by && by > cy) yield return spectrum[i + 1];
			}
		}

		public static IEnumerable<Bin> Interpolate(this IList<Bin> spectrum)
		{
			var halfStep = (spectrum[1].Frequency - spectrum[0].Frequency) / 2;
			var count = spectrum.Count / 2 - 4;
			for (var i = 0; i < count; i++)
			{
				//var x = i < 0 ? count / 2 : 0;
				spectrum[i + 0].Deconstruct(out var ax, out var ay, out var ap);
				spectrum[i + 1].Deconstruct(out var bx, out var by, out var bp);
				spectrum[i + 2].Deconstruct(out var cx, out var cy, out var cp);
				spectrum[i + 3].Deconstruct(out var dx, out var dy, out var dp);
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
					a.Construct(in lx, in ly, in ap);
					b.Construct(in mx, in my, in bp);
					c.Construct(in rx, in ry, in cp);

					yield return a;
					yield return b;
					yield return c;

					i += 3;
				}
				else
				{
					Bin a = new Bin();
					a.Construct(in ax, in ay, in ap);
					yield return a;
				}
			}
		}
	}
}