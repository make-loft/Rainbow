using System;
using System.Collections.Generic;
using System.Linq;

namespace Rainbow
{
	public static class Butterfly
	{
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

		public static double InvertSign(this double d, bool negate) => negate ? -d : +d;

		public static Complex[] DecimationInTime(this IEnumerable<Complex> frame, bool direct) =>
			frame.ToArray()._DecimationInTime(direct);

		public static Complex[] _DecimationInFrequency(this IEnumerable<Complex> frame, bool direct) =>
			frame.ToArray()._DecimationInFrequency(direct);

		private static Complex[] _DecimationInTime(this Complex[] frame, bool direct)
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

			var spectrumOdd = _DecimationInTime(frameOdd, direct);
			var spectrumEven = _DecimationInTime(frameEven, direct);

			var arg = (Pi.Double / frameFullSize).InvertSign(direct);
			var omegaPowBase = new Complex(Math.Cos(arg), Math.Sin(arg));
			var omega = Complex.One;
			var spectrum = frame; // new Complex[frameFullSize];

			for (var j = 0; j < frameHalfSize; j++)
			{
				var a = spectrumEven[j];
				var b = spectrumOdd[j];
				spectrum[j] = a + omega * b;
				spectrum[j + frameHalfSize] = a - omega * b;
				omega *= omegaPowBase;
			}

			return spectrum;
		}

		private static Complex[] _DecimationInFrequency(this Complex[] frame, bool direct)
		{
			if (frame.Length == 1) return frame;
			var frameHalfSize = frame.Length >> 1; // frame.Length/2
			var frameFullSize = frame.Length;

			var arg = (Pi.Double / frameFullSize).InvertSign(direct);
			var omegaPowBase = new Complex(Math.Cos(arg), Math.Sin(arg));
			var omega = Complex.One;
			var spectrum = frame; // new Complex[frameFullSize];

			for (var j = 0; j < frameHalfSize; j++)
			{
				var a = frame[j];
				var b = frame[j + frameHalfSize];
				spectrum[j] = a + b;
				spectrum[j + frameHalfSize] = omega * (a - b);
				omega *= omegaPowBase;
			}

			var yTop = new Complex[frameHalfSize];
			var yBottom = new Complex[frameHalfSize];
			for (var i = 0; i < frameHalfSize; i++)
			{
				yTop[i] = spectrum[i];
				yBottom[i] = spectrum[i + frameHalfSize];
			}

			var spectrumTop = _DecimationInFrequency(yTop, direct);
			var spectrumBottom = _DecimationInFrequency(yBottom, direct);
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