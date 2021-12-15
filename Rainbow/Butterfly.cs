
using System;
using System.Collections.Generic;
using System.Linq;

using static System.Math;

namespace Rainbow
{
	public static class Butterfly
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

		public static Complex[] Decimation(this IEnumerable<Complex> frame, bool direct, bool inTime = true)
		{
			var inputFrame = frame.ToArray();
			var inputFactor = direct ? 1d : inputFrame.Length / 2;
			inputFrame.Squeeze(inputFactor);

			var outputFrame = inTime
				? inputFrame.DecimationInTime(direct)
				: inputFrame.DecimationInFrequency(direct);

			var outputFactor = direct ? outputFrame.Length / 2 : 1d;

			outputFrame.Squeeze(outputFactor);
			return outputFrame;
		}

		private static Complex[] DecimationInTime(this Complex[] frame, bool direct)
		{
			if (frame.Length == 1) return frame;
			var frameHalfSize = frame.Length / 2; // frame.Length >> 1
			var frameFullSize = frame.Length;

			var frameA = new Complex[frameHalfSize];
			var frameB = new Complex[frameHalfSize];

			for (int i = 0, j = 0; i < frameHalfSize; i++) // j+=2
			{
				frameA[i] = frame[j++];
				frameB[i] = frame[j++];
			}

			var spectrumA = DecimationInTime(frameA, direct);
			var spectrumB = DecimationInTime(frameB, direct);

			var spectrum = frame; // new Complex[frameFullSize];
			var arg = (Pi.Double / frameFullSize).InvertSign(direct);
			var omegaPowBase = new Complex(Cos(arg), Sin(arg));
			var omega = Complex.One;

			for (int i = 0, j = frameHalfSize; i < frameHalfSize; i++, j++)
			{
				var a = spectrumA[i];
				var b = spectrumB[i] * omega;
				spectrum[i] = a + b;
				spectrum[j] = a - b;
				omega *= omegaPowBase;
			}

			return spectrum;
		}

		private static Complex[] DecimationInFrequency(this Complex[] frame, bool direct)
		{
			if (frame.Length == 1) return frame;
			var frameHalfSize = frame.Length >> 1; // frame.Length/2
			var frameFullSize = frame.Length;

			var arg = (Pi.Double / frameFullSize).InvertSign(direct);
			var omegaPowBase = new Complex(Cos(arg), Sin(arg));
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