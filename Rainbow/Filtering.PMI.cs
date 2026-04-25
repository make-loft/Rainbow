using System;
using System.Collections.Generic;
using System.Linq;

namespace Rainbow;

public static partial class Filtering
{
	public static bool TryReconstructPeak(in Bin a, in Bin b, in Bin c, in Bin d, out Bin peak)
	{
		/* Frequency (F); Magnitude (M); Phase (P); */
		a.Deconstruct(out var aF, out var aM, out var aP);
		b.Deconstruct(out var bF, out var bM, out var bP);
		c.Deconstruct(out var cF, out var cM, out var cP);
		d.Deconstruct(out var dF, out var dM, out var dP);

		var distance = (cP - bP) / Pi.Single;
		var turn = Math.Abs(distance + 0.5) > 1.0;
		if (turn && distance < -1.0)
			distance += 2.0;

		var peakByPhaseState = (distance * distance).HitInterval(from: 0.5, till :2.0 + 0.01);
		if (peakByPhaseState is false)
		{
			peak = default;
			return false;
		}

	//	var halfStepF = (cF - bF) / 2;
	//	var bcMiddleF = (bF + cF) / 2;
	//	var bcOffsetScale = (cM - bM) / (cM + bM);
	//	var bcF = bcMiddleF + bcOffsetScale * halfStepF;

		var bcF = (bF * bM + cF * cM) / (bM + cM);
		var bcM = (bM + cM) - (aM + dM) / Pi.Half;
		var bcP = bP + (bcF - bF) * (cP - bP) / (cF - bF);
		/* y(x) = y0 + ( x  - x0) * (y1 - y0) / (x1 - x0) */

		if (turn)
			bcP += Pi.Double * (cF - bcF) / (cF - bF);
		if (bcP > Pi.Single)
			bcP -= Pi.Double;

		var bPower = bM * bM;
		var cPower = cM * cM;
		var bcPower = bcM * bcM;

		var peakByMagnitudeState = bcM > 0d && bcPower * 1.4142 > bPower + cPower;
		if (peakByMagnitudeState is false && bcPower < 2 * bM * cM &&  bcM < (bM + cM))
		{
			/* resonanse estimation */
			bcM = Math.Sqrt(bPower + cPower);
			peakByMagnitudeState = true;
		}

		peak = new(bcF, bcM, bcP);

		return peakByPhaseState && peakByMagnitudeState is true;
	}

	public static List<Bin> Interpolate(this IList<Bin> spectrum, out List<Bin> peaks)
		=> spectrum.Interpolate(peaks = new()).ToList();

	private static IEnumerable<Bin> Interpolate(this IList<Bin> spectrum, List<Bin> peaks)
	{
		var count = spectrum.Count / 2 - 3;
		if (count < 0) throw new Exception("Spectrum size is too short");

		for (var i = 0; i < count; i++)
		{
			var a = spectrum[i + 0];
			var b = spectrum[i + 1];
			var c = spectrum[i + 2];
			var d = spectrum[i + 3];

			yield return a;

			var peakReconstructionState = TryReconstructPeak(a, b, c, d, out var peak);
			if (peakReconstructionState is true)
			{
				peaks.Add(peak);
				yield return b;
				yield return peak;
				i++;
			}
			else if (b.Magnitude > Math.Max(a.Magnitude, c.Magnitude))
			{
				peaks.Add(b);
			}
		}
	}
}
