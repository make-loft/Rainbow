using System;
using System.Collections.Generic;

namespace Rainbow
{
	public struct Bin
	{
		public double Frequency { get; set; }
		public double Magnitude { get; set; }
		public double Phase { get; set; }

		public override string ToString() => $"{Magnitude:F4} {Frequency:F4} {Phase:F4}";

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
				var expectedDeltaPhase = binBase * binToPhase;
				var actualDeltaPhase = spectrum1[binBase].Phase - spectrum0[binBase].Phase;

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

		public static IEnumerable<Bin> EnumeratePeaks(this IList<Bin> spectrum, double silenceThreshold = 0.01)
		{
			var count = spectrum.Count / 2 - 3;
			for (var i = 0; i < count; i++)
			{
				spectrum[i + 0].Deconstruct(out var ax, out var ay, out var ap);
				spectrum[i + 1].Deconstruct(out var bx, out var by, out var bp);
				spectrum[i + 2].Deconstruct(out var cx, out var cy, out var cp);
				if ((ay + cy) * 0.5d < by && by > silenceThreshold)
					yield return spectrum[i + 1];
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
				var applyMagnitudeCorrection =
					ay < by && ay < cy
					&&
					dy < cy && dy * magicFactor < by;

				var deltaPhase = Math.Abs(bp - cp);
				var applyPhaseCorrection = Pi.Single * 0.9 < deltaPhase && deltaPhase < Pi.Single * 1.1;

				var applyCorrection = applyMagnitudeCorrection && applyPhaseCorrection;
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

					var x0 = bx;
					var x1 = cx;
					var y0 = bp; //bp < -(3 * Pi.Quarter) ? bp + Pi.Double : bp;
					var y1 = cp; //cp > +(1 * Pi.Quarter) ? cp - Pi.Double : cp;

					var mp = y0 + (mx - x0) * (y0 - y1) / (x0 - x1);

					yield return new(in lx, in ly, in ap);
					yield return new(in mx, in my, in mp);
					yield return new(in rx, in ry, in dp);

					i += 3;
				}
				else
				{
					yield return new(in ax, in ay, in ap);
				}
			}
		}
	}
}