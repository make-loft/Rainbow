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
			var length = frame.Length / 2; // frame.Length >> 1

			var frameA = new Complex[length];
			var frameB = new Complex[length];

			for (int i = 0, j = 0; i < length; i++) // j+=2
			{
				frameA[i] = frame[j++];
				frameB[i] = frame[j++];
			}

			var spectrumA = DecimationInTime(frameA, direct);
			var spectrumB = DecimationInTime(frameB, direct);

			var spectrum = frame; // new Complex[frame.Length];
			var arg = (Pi.Single / length).InvertSign(direct);
			var omegaPowBase = new Complex(Cos(arg), Sin(arg));
			var omega = Complex.One;

			for (int i = 0, j = length; i < length; i++, j++)
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
			var length = frame.Length / 2; // frame.Length >> 1

			var arg = (Pi.Single / length).InvertSign(direct);
			var omegaPowBase = new Complex(Cos(arg), Sin(arg));
			var omega = Complex.One;
			var spectrum = frame; // new Complex[frame.Length];

			for (var j = 0; j < length; j++)
			{
				var a = frame[j];
				var b = frame[j + length];
				spectrum[j] = a + b;
				spectrum[j + length] = omega * (a - b);
				omega *= omegaPowBase;
			}

			var yTop = new Complex[length];
			var yBottom = new Complex[length];
			for (var i = 0; i < length; i++)
			{
				yTop[i] = spectrum[i];
				yBottom[i] = spectrum[i + length];
			}

			var spectrumTop = DecimationInFrequency(yTop, direct);
			var spectrumBottom = DecimationInFrequency(yBottom, direct);
			for (var i = 0; i < length; i++)
			{
				var j = i << 1; // i = 2*j;
				spectrum[j] = spectrumTop[i];
				spectrum[j + 1] = spectrumBottom[i];
			}

			return spectrum;
		}
	}
}