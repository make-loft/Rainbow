using System;
using System.Collections.Generic;
using System.Linq;

namespace Rainbow
{
	public static partial class Filtering
	{
		public static List<Bin> Interpolate(this IList<Bin> spectrum, out List<Bin> peaks) =>
			spectrum.Interpolate(peaks = new()).ToList();

		public static bool TryReconstructPeak(this IList<Bin> spectrum, int i, out Bin peak, out double deltaPhase)
		{
			/* Frequency (F); Magnitude (M); Phase (P); */
			spectrum[i + 0].Deconstruct(out var aF, out var aM, out var aP);
			spectrum[i + 1].Deconstruct(out var bF, out var bM, out var bP);
			spectrum[i + 2].Deconstruct(out var cF, out var cM, out var cP);
			spectrum[i + 3].Deconstruct(out var dF, out var dM, out var dP);

			//var halfStepF = (cF - bF) / 2;
			//var bcMiddleF = (bF + cF) / 2;
			var bcOffsetScale = (cM - bM) / (cM + bM);
			//var bcF = bcMiddleF + bcOffsetScale * halfStepF;

			var bcF = (bF * bM + cF * cM) / (bM + cM);
			var bcM = (bM + cM) - (aM + dM) / Pi.Half;
			var bcP = bP + (bcF - bF) * (cP - bP) / (cF - bF);
			/* y(x) = y0 + ( x  - x0) * (y1 - y0) / (x1 - x0) */

			if (cP > bP)
				bcP += Pi.Double * (cF - bcF) / (cF - bF);
			if (bcP > Pi.Single)
				bcP -= Pi.Double;

			peak = new(bcF, bcM, bcP);
			
			deltaPhase = cP - bP;

			var isPeakByFrequency = bcF.BelongOpen(bF, cF);
			var isPeakByMagnitude = bcM > Math.Max(bM, cM) / 2d;
			var isPeakByPhase = Math.Abs(deltaPhase).BelongClose((1d - Math.Abs(bcOffsetScale)) * Pi.Half, 1.5 * Pi.Single);
			
			return isPeakByPhase && isPeakByMagnitude && isPeakByFrequency;
		}


		public static IEnumerable<Bin> Interpolate(this IList<Bin> spectrum, List<Bin> peaks)
		{
			var count = spectrum.Count / 2;

			for (var i = 0; i < count; i++)
			{
				var hasL = spectrum.TryReconstructPeak(i + 0, out var peakCandidateL, out var deltaPhaseL);
				var hasR = spectrum.TryReconstructPeak(i + 1, out var peakCandidateR, out var deltaPhaseR);
				
				if (hasL is false || (hasR is true && Math.Abs(deltaPhaseR) > Math.Abs(deltaPhaseL)))
				{
					yield return spectrum[i];
				}
				else
				{
					peaks.Add(peakCandidateL);

					var bcF = peakCandidateL.Frequency;
					var bF = spectrum[i + 1].Frequency;
					var cF = spectrum[i + 2].Frequency;

					spectrum[i + 0].Deconstruct(out var aF, out var aM, out var aP);
					spectrum[i + 3].Deconstruct(out var dF, out var dM, out var dP);

					var abF = aF + (bcF - bF);
					var abM = aM;
					var abP = aP;

					var dcF = dF + (bcF - cF);
					var dcM = dM;
					var dcP = dP;

					yield return new(in abF, in abM, in abP);
					yield return peakCandidateL;
					yield return new(in dcF, in dcM, in dcP);

					i += 2;
				}
			}
		}
	}
}