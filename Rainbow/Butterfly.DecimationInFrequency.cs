using static System.Math;

namespace Rainbow
{
	public static partial class Butterfly
	{
		private static void DecimationInFrequency(ref Complex[] frame, bool direct)
		{
			if (frame.Length < 2) return;
			var length = frame.Length / 2; // frame.Length >> 1

			var frameA = new Complex[length];
			var frameB = new Complex[length];

			var abs = (Pi.Single / length).InvertSign(direct);
			var rotorBase = new Complex(Cos(abs), Sin(abs));
			var rotor = Complex.One; // rotor = rotorBase.Pow(0)

			for (int i = 0, j = length; i < length; i++, j++)
			{
				var a = frame[i];
				var b = frame[j];
				frameA[i] = a + b;
				frameB[i] = (a - b) * rotor;
				rotor *= rotorBase; // rotor = rotorBase.Pow(i + 1)
			}

			DecimationInFrequency(ref frameA, direct);
			DecimationInFrequency(ref frameB, direct);

			for (int i = 0, j = 0; i < length; i++) // j += 2
			{
				frame[j++] = frameA[i];
				frame[j++] = frameB[i];
			}
		}
	}
}