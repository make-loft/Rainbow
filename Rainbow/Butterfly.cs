using System;

namespace Rainbow
{
	public static class Butterfly
	{
		public const double SinglePi = Math.PI;
		public const double DoublePi = 2 * Math.PI;

		public static Complex[] Normalize(this Complex[] frame)
		{
			var size = frame.Length;
			for (var i = 0; i < size; i++)
			{
				frame[i] /= size;
			}

			return frame;
		}

		//private static Complex Omega(int i, int sampleRate)
		//{
		//	if (i%sampleRate == 0) return Complex.One;
		//	var arg = -DoublePi*i/sampleRate;
		//	return new Complex(Math.Cos(arg), Math.Sin(arg));
		//}

		public static double InvertSign(this double d, bool negate) => negate ? -d : d;

		public static Complex[] DecimationInTime(this Complex[] frame, bool direct)
		{
			if (frame.Length == 1) return frame;
			var frameHalfSize = frame.Length >> 1; // frame.Length/2
			var frameFullSize = frame.Length;

			var frameOdd = new Complex[frameHalfSize];
			var frameEven = new Complex[frameHalfSize];
			for (var i = 0; i < frameHalfSize; i++)
			{
				var j = i << 1; // i = 2*j;
				frameOdd[i] = frame[j + 1];
				frameEven[i] = frame[j];
			}

			var spectrumOdd = DecimationInTime(frameOdd, direct);
			var spectrumEven = DecimationInTime(frameEven, direct);

			var arg = (DoublePi / frameFullSize).InvertSign(direct);
			var omegaPowBase = new Complex(Math.Cos(arg), Math.Sin(arg));
			var omega = Complex.One;
			var spectrum = new Complex[frameFullSize];

			for (var j = 0; j < frameHalfSize; j++)
			{
				spectrum[j] = spectrumEven[j] + omega * spectrumOdd[j];
				spectrum[j + frameHalfSize] = spectrumEven[j] - omega * spectrumOdd[j];
				omega *= omegaPowBase;
			}

			return spectrum;
		}

		public static Complex[] DecimationInFrequency(this Complex[] frame, bool direct)
		{
			if (frame.Length == 1) return frame;
			var frameHalfSize = frame.Length >> 1; // frame.Length/2
			var frameFullSize = frame.Length;

			var arg = (DoublePi / frameFullSize).InvertSign(direct);
			var omegaPowBase = new Complex(Math.Cos(arg), Math.Sin(arg));
			var omega = Complex.One;
			var spectrum = new Complex[frameFullSize];

			for (var j = 0; j < frameHalfSize; j++)
			{
				spectrum[j] = frame[j] + frame[j + frameHalfSize];
				spectrum[j + frameHalfSize] = omega * (frame[j] - frame[j + frameHalfSize]);
				omega *= omegaPowBase;
			}

			var yTop = new Complex[frameHalfSize];
			var yBottom = new Complex[frameHalfSize];
			for (var i = 0; i < frameHalfSize; i++)
			{
				yTop[i] = spectrum[i];
				yBottom[i] = spectrum[i + frameHalfSize];
			}

			var spectrumTop = DecimationInFrequency(yTop, direct);
			var spectrumBottom = DecimationInFrequency(yBottom, direct);
			for (var i = 0; i < frameHalfSize; i++)
			{
				var j = i << 1; // i = 2*j;
				spectrum[j] = spectrumTop[i];
				spectrum[j + 1] = spectrumBottom[i];
			}

			return spectrum;
		}
	}
}