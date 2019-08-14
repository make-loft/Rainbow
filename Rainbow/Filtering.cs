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

		public Bin(in double frequency, in double magnitude, in double phase)
		{
			Frequency = frequency;
			Magnitude = magnitude;
			Phase = phase;
		}

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
		public static IEnumerable<Bin> GetJoinedSpectrum(
			IList<Complex> spectrum0, IList<Complex> spectrum1,
			double shiftsPerFrame, double sampleRate)
		{
			var frameSize = spectrum0.Count;
			var binToFrequency = sampleRate / frameSize;
			//var frameTime = frameSize / sampleRate;
			//var shiftTime = frameTime / shiftsPerFrame;
			//var binToPhase = Pi.Double * binToFrequency * shiftTime;
			// = Pi.Double * (sampleRate / frameSize) * (frameTime / shiftsPerFrame);
			// = Pi.Double * (sampleRate / frameSize) * ((frameSize / sampleRate) / shiftsPerFrame);
			var binToPhase = Pi.Double / shiftsPerFrame;
			var binsCount = frameSize / 2;

			for (var binBase = 0; binBase < binsCount; binBase++)
			{
				var actualDeltaPhase = spectrum1[binBase].Phase - spectrum0[binBase].Phase;
				var expectedDeltaPhase = binBase * binToPhase;

				var phaseDeviation = actualDeltaPhase - expectedDeltaPhase;
				var binDeviation = (phaseDeviation / binToPhase) % 1;

				var actualFrequancy = (binBase + binDeviation) * binToFrequency;
				var magnitude = (spectrum1[binBase].Magnitude + spectrum0[binBase].Magnitude) / 2d;
				yield return new Bin(actualFrequancy, magnitude * (1 + binDeviation), actualDeltaPhase);
			}
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

		public static IEnumerable<Bin> EnumeratePeaks(this IList<Bin> spectrum)
		{
			var count = spectrum.Count / 2 - 3;
			for (var i = 0; i < count; i++)
			{
				spectrum[i + 0].Deconstruct(out var ax, out var ay, out var ap);
				spectrum[i + 1].Deconstruct(out var bx, out var by, out var bp);
				spectrum[i + 2].Deconstruct(out var cx, out var cy, out var cp);
				if ((ay + cy) * 2d < by) yield return spectrum[i + 1];
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
					var middle = (bx + cx) / 2;
					var delta = (cy - by) / (cy + by);
					var mx = middle + delta * halfStep;
					var my = (by + cy) - (ay + dy) / Pi.Half;

					var lx = ax + (mx - bx);
					var rx = dx + (mx - cx);

					var ly = ay; //* (cx - mx) / halfStep;
					var ry = dy; //* (mx - bx) / halfStep;

					yield return new Bin(in lx, in ly, in ap);
					yield return new Bin(in mx, in my, in bp);
					yield return new Bin(in rx, in ry, in cp);

					i += 3;
				}
				else
				{
					yield return new Bin(in ax, in ay, in ap);
				}
			}
		}
	}
}