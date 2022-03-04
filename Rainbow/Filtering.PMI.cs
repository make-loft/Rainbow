using System;
using System.Collections.Generic;

namespace Rainbow
{
	public static partial class Filtering
	{
		public static IEnumerable<Bin> Interpolate(this IList<Bin> spectrum, List<int> resonances = default)
		{
			resonances?.Clear();

			var count = spectrum.Count / 2 - 4;
			for (var i = 0; i < count; i++)
			{
				/* Frequency (F); Magnitude (M); Phase (P); */
				spectrum[i + 0].Deconstruct(out var aF, out var aM, out var aP);
				spectrum[i + 1].Deconstruct(out var bF, out var bM, out var bP);
				spectrum[i + 2].Deconstruct(out var cF, out var cM, out var cP);
				spectrum[i + 3].Deconstruct(out var dF, out var dM, out var dP);

				double GetPeakProbabilityByPhase() => Math.Abs(cP - bP) / Pi.Single;
				double GetPeakProbabilityByMagnitude()
				{
					var bcM = (bM + cM) - (aM + dM) / Pi.Half;
					return 
						(aM < bcM && bcM > dM)
						&&
						(bM * 0.95 < bcM && bcM > cM * 0.95)
							? 0.95
							: 0.05;
				}

				var peakProbabilityByPhase = GetPeakProbabilityByPhase();
				var peakProbabilityByMagnitude = GetPeakProbabilityByMagnitude();

				var peakProbability = peakProbabilityByPhase * peakProbabilityByMagnitude;
				if (peakProbabilityByMagnitude > 0.5 && peakProbabilityByPhase < 0.5)
					resonances?.Add(i);

				if (peakProbability > 0.5)
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