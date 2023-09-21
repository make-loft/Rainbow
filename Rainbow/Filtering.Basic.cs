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
				var bin = spectrum[i];
				yield return new(i * binToFrequency, bin.Magnitude, bin.Phase);
			}
		}
	}
}