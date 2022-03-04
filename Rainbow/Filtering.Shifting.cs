using System;
using System.Collections.Generic;

namespace Rainbow
{
	//  Δ∂ωπ
	public static partial class Filtering
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
				yield return new(actualFrequancy, magnitude * (1 + binDeviation), actualDeltaPhase);
			}
		}
	}
}