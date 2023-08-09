using System;
using System.Collections.Generic;
using System.Linq;

namespace Rainbow
{
	public static partial class Filtering
	{
		public static List<Bin> Interpolate(this IList<Bin> spectrum, out List<Bin> peaks) =>
			spectrum.Interpolate(peaks = new()).ToList();

		public static bool TryReconstructPeak(this IList<Bin> spectrum, int i, out Bin peak)
		{
			/* Frequency (F); Magnitude (M); Phase (P); */
			spectrum[i + 0].Deconstruct(out var aF, out var aM, out var aP);
			spectrum[i + 1].Deconstruct(out var bF, out var bM, out var bP);
			spectrum[i + 2].Deconstruct(out var cF, out var cM, out var cP);
			spectrum[i + 3].Deconstruct(out var dF, out var dM, out var dP);

			var distance = (cP - bP) / Pi.Single;
			var turn = Math.Abs(distance + 0.5) > 1.0;
			if (turn && distance < -1.0)
				distance += 2.0;

			var isPeakByPhase = 0.9 < Math.Abs(distance) && Math.Abs(distance) <= 1.1;

			if (isPeakByPhase is false)
			{
				peak = default;
				return false;
			}

			//var halfStepF = (cF - bF) / 2;
			//var bcMiddleF = (bF + cF) / 2;
			//var bcOffsetScale = (cM - bM) / (cM + bM);
			//var bcF = bcMiddleF + bcOffsetScale * halfStepF;

			var bcF = (bF * bM + cF * cM) / (bM + cM);
			var bcM = (bM + cM) - (aM + dM) / Pi.Half;
			var bcP = bP + (bcF - bF) * (cP - bP) / (cF - bF);
			/* y(x) = y0 + ( x  - x0) * (y1 - y0) / (x1 - x0) */

			if (turn)
				bcP += Pi.Double * (cF - bcF) / (cF - bF);
			if (bcP > Pi.Single)
				bcP -= Pi.Double;
			
			//if (bcM < (bM + cM)) // resonanse estimation
			//	bcM = Math.Sqrt(bM * bM + cM * cM);

			//var _bcM = bM + (bcF - bF) * (cM - bM) / (cF - bF);
			//var isPeakByMagnitude = _bcM < bcM;
			var isPeakByMagnitude = bcM > 0d && bcM * bcM > bM * bM + cM * cM;
			if (isPeakByMagnitude is false && bcM < (bM + cM)) // resonanse estimation
				bcM = Math.Sqrt(bM * bM + cM * cM);

			peak = new(bcF, bcM, bcP);

			return isPeakByPhase;
		}

		public static IEnumerable<Bin> Interpolate(this IList<Bin> spectrum)
		{
			var count = spectrum.Count / 2;

			for (var i = 0; i < count; i++)
			{
				var hasPeak = spectrum.TryReconstructPeak(i + 0, out var peak);

				yield return spectrum[i + 0];

				if (hasPeak)
					yield return peak;
			}
		}

		public static IEnumerable<Bin> Interpolate(this IList<Bin> spectrum, List<Bin> peaks)
		{
			var interpolatedSpectrum = Interpolate(spectrum).OrderBy(p => p.Frequency).ToList();

			for (var i = 1; i < interpolatedSpectrum.Count - 1; i++)
			{
				var a = interpolatedSpectrum[i - 1].Magnitude;
				var b = interpolatedSpectrum[i].Magnitude;
				var c = interpolatedSpectrum[i + 1].Magnitude;

				if (b > Math.Max(a, c))
				{
					peaks.Add(interpolatedSpectrum[i]);
				}
			}

			return interpolatedSpectrum;
		}
	}
}