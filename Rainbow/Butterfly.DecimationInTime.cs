using static System.Math;

namespace Rainbow
{
	public static partial class Butterfly
	{
		private static void DecimationInTime(ref Complex[] frame, bool direct)
		{
			if (frame.Length < 2) return;
			var length = frame.Length / 2; // frame.Length >> 1

			var frameA = new Complex[length];
			var frameB = new Complex[length];

			var abs = (Pi.Single / length).InvertSign(direct);
			var rotorBase = new Complex(Cos(abs), Sin(abs));
			var rotor = Complex.One; // rotor = rotorBase.Pow(0)

			for (int i = 0, j = 0; i < length; i++) // j+=2
			{
				frameA[i] = frame[j++];
				frameB[i] = frame[j++];
			}

			DecimationInTime(ref frameA, direct);
			DecimationInTime(ref frameB, direct);

			for (int i = 0, j = length; i < length; i++, j++)
			{
				var a = frameA[i];
				var b = frameB[i] * rotor;
				frame[i] = a + b;
				frame[j] = a - b;
				rotor *= rotorBase; // rotor = rotorBase.Pow(i + 1)
			}
		}
	}
}