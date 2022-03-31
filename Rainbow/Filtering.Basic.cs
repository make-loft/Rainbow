using System.Collections.Generic;

namespace Rainbow
{
	public static partial class Filtering
	{
		public static IEnumerable<Bin> GetSpectrum(IList<Complex> spectrum, double sampleRate)
		{
			var frameSize = spectrum.Count;
			var binToFrequency = sampleRate / frameSize;

			for (var i = 0; i < frameSize; i++)
			{
				yield return new()
				{
					Phase = spectrum[i].Phase,
					Magnitude = spectrum[i].Magnitude,
					Frequency = i * binToFrequency
				};
			}
		}

		public static IEnumerable<Bin> EnumeratePeaks(this IList<Bin> spectrum, double silenceThreshold = 0.01)
		{
			var count = spectrum.Count - 3;
			for (var i = 0; i < count; i++)
			{
				spectrum[i + 0].Deconstruct(out var aF, out var aM, out var aP);
				spectrum[i + 1].Deconstruct(out var bF, out var bM, out var bP);
				spectrum[i + 2].Deconstruct(out var cF, out var cM, out var cP);

				if ((aM + cM) * 0.25d < bM && aM < bM && bM > cM && bM > silenceThreshold)
					yield return spectrum[i + 1];
			}
		}
	}
}