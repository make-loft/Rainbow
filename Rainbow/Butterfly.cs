using System.Collections.Generic;
using System.Linq;

namespace Rainbow
{
	public static partial class Butterfly
	{
		public static Complex[] Stretch(this Complex[] frame, double factor)
		{
			if (factor == 1d) return frame;
			for (var i = 0; i < frame.Length; i++) frame[i] *= factor;
			return frame;
		}

		public static Complex[] Squeeze(this Complex[] frame, double factor)
		{
			if (factor == 1d) return frame;
			for (var i = 0; i < frame.Length; i++) frame[i] /= factor;
			return frame;
		}

		public static Complex[] Decimation(this IEnumerable<Complex> frame, bool direct, bool inTime = false)
		{
			var workFrame = frame.ToArray();

			if (inTime) DecimationInTime(ref workFrame, direct);
			else DecimationInFrequency(ref workFrame, direct);

			var factor = direct ? workFrame.Length / 2 : 2;

			workFrame.Squeeze(factor);

			return workFrame;
		}
	}
}