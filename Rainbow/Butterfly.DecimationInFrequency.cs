using static System.Math;

namespace Rainbow
{
	public static partial class Butterfly
	{
		private static void DecimationInFrequency(ref Complex[] sample, bool direct)
		{
			if (sample.Length < 2) return;
			var length = sample.Length / 2; // sample.Length >> 1

			var sampleA = new Complex[length];
			var sampleB = new Complex[length];

			var abs = (Pi.Single / length).InvertSign(direct);
			var rotorBase = new Complex(Cos(abs), Sin(abs));
			var rotor = Complex.One; // rotor = rotorBase.Pow(0)

			for (int i = 0, j = length; i < length; i++, j++)
			{
				var a = sample[i];
				var b = sample[j];
				sampleA[i] = a + b;
				sampleB[i] = (a - b) * rotor;
				rotor *= rotorBase; // rotor = rotorBase.Pow(i + 1)
			}

			DecimationInFrequency(ref sampleA, direct);
			DecimationInFrequency(ref sampleB, direct);

			for (int i = 0, j = 0; i < length; i++) // j += 2
			{
				sample[j++] = sampleA[i];
				sample[j++] = sampleB[i];
			}
		}
	}
}