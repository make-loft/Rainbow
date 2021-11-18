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

			for (var i = 0; i < frameSize; i++)
			{
				yield return new()
				{
					Phase = spectrum[i].Phase,
					Magnitude = spectrum[i].Magnitude,
					Frequency = i * binToFrequencyFactor
				};
			}
		}

		public static IEnumerable<Bin> EnumeratePeaks(this IList<Bin> spectrum, double silenceThreshold = 0.01)
		{
			var count = spectrum.Count / 2 - 3;
			for (var i = 0; i < count; i++)
			{
				spectrum[i + 0].Deconstruct(out var aF, out var aM, out var aP);
				spectrum[i + 1].Deconstruct(out var bF, out var bM, out var bP);
				spectrum[i + 2].Deconstruct(out var cF, out var cM, out var cP);

				if ((aM + cM) * 0.5d < bM && bM > silenceThreshold)
					yield return spectrum[i + 1];
			}
		}

		public static bool CanCorrect(double aM, double bM, double cM, double dM, double magicFactor) =>
			aM < bM && aM < cM
			&&
			dM < cM && dM * magicFactor < bM;

		public static bool CanCorrect(double deltaPhase) =>
			Pi.Single * 0.9 < deltaPhase && deltaPhase < Pi.Single * 1.1;

		public static IEnumerable<Bin> Interpolate(this IList<Bin> spectrum, bool skipResonances)
		{
			var count = spectrum.Count / 2 - 4;
			for (var i = 0; i < count; i++)
			{
				/* Frequency (F); Magnitude (M); Phase (P); */
				spectrum[i + 0].Deconstruct(out var aF, out var aM, out var aP);
				spectrum[i + 1].Deconstruct(out var bF, out var bM, out var bP);
				spectrum[i + 2].Deconstruct(out var cF, out var cM, out var cP);
				spectrum[i + 3].Deconstruct(out var dF, out var dM, out var dP);

				var magicFactor = cF / dF; /* for better accuracy, but why? */
				var applyCorrection = CanCorrect(aM, bM, cM, dM, magicFactor)
					&& (skipResonances is false || CanCorrect(Math.Abs(cP - bP)));

				if (applyCorrection)
				{
					/*
					var halfStep = (cF - bF) / 2;
					var bcMiddleF = (bF + cF) / 2;
					var bcOffsetScale = (cM - bM) / (cM + bM);
					var bcF = bcMiddleF + bcOffsetScale * halfStep;
					*/

					var bcF = (bF * bM + cF * cM) / (bM + cM);
					var bcM = (bM + cM) - (aM + dM) / Pi.Half;
					var bcP = bP + (bcF - bF) * (cP - bP) / (cF - bF);
					/* y(x) = y0 + ( x  - x0) * (y1 - y0) / (x1 - x0) */

					var abF = aF + (bcF - bF);
					var abM = aM;
					var abP = aP;

					var dcF = dF + (bcF - cF);
					var dcM = dM;
					var dcP = dP;

					yield return new(in abF, in abM, in abP);
					yield return new(in bcF, in bcM, in bcP);
					yield return new(in dcF, in dcM, in dcP);

					i += 3;
				}
				else
				{
					yield return new(in aF, in aM, in aP);
				}
			}
		}
	}
}