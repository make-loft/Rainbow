using System.Collections.Generic;
using System.Linq;

namespace Rainbow
{
	public static partial class Butterfly
	{
		public static Complex[] Transform(this IEnumerable<Complex> sample, bool direct, bool inTime = false)
		{
			var workSample = sample.ToArray();

			if (inTime) DecimationInTime(ref workSample, direct);
			else DecimationInFrequency(ref workSample, direct);

			var factor = direct ? workSample.Length / 2 : 2;
			for (var i = 0; i < workSample.Length; i++)	workSample[i] /= factor;

			return workSample;
		}
	}
}