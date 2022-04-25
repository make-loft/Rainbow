using System.Collections.Generic;
using System.Linq;

using static System.Math;

namespace Rainbow
{
	public static partial class Butterfly
	{
		public static Complex[] Transform(this IEnumerable<Complex> sample, bool direct, bool inTime = false)
		{
			var workSample = sample.ToArray();
			var rotors = direct ? DirectRotors : RevertRotors;
			Complex[] getRotor(int length) => rotors.TryGetValue(length, out var rotor)
				? rotor
				: rotors[length] = GenerateRotor(length, direct);

			if (inTime) DecimationInTime(ref workSample, getRotor);
			else DecimationInFrequency(ref workSample, getRotor);

			double normalizationFactor = direct ? workSample.Length / 2 : 2;
			for (var i = 0; i < workSample.Length; i++)
				workSample[i] /= normalizationFactor;

			return workSample;
		}

		static readonly Dictionary<int, Complex[]> DirectRotors = new();
		static readonly Dictionary<int, Complex[]> RevertRotors = new();

		static Complex[] GenerateRotor(int length, bool direct)
		{
			var abs = (Pi.Single / length).InvertSign(direct);
			var rotorBase = new Complex(Cos(abs), Sin(abs));
			var rotor = new Complex[length];
			for (var i = 0; i < length; i++)
				rotor[i] = Complex.Pow(rotorBase, i);
			return rotor;
		}
	}
}