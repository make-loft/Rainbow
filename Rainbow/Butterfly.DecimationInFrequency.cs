namespace Rainbow
{
	public static partial class Butterfly
	{
		private static void DecimationInFrequency(ref Complex[] sample, System.Func<int, Complex[]> getRotor)
		{
			if (sample.Length < 2) return;
			var length = sample.Length / 2;

			var sampleA = new Complex[length];
			var sampleB = new Complex[length];
			var rotor = getRotor(length);

			for (int i = 0, j = length; i < length; i++, j++)
			{
				var a = sample[i];
				var b = sample[j];

				sampleA[i] = a + b;
				sampleB[i] = (a - b) * rotor[i];
			}

			DecimationInFrequency(ref sampleA, getRotor);
			DecimationInFrequency(ref sampleB, getRotor);

			for (int i = 0, j = 0; i < length; i++) // j += 2
			{
				sample[j++] = sampleA[i];
				sample[j++] = sampleB[i];
			}
		}
	}
}