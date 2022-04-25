namespace Rainbow
{
	public static partial class Butterfly
	{
		private static void DecimationInTime(ref Complex[] sample, System.Func<int, Complex[]> getRotor)
		{
			if (sample.Length < 2) return;
			var length = sample.Length / 2;

			var sampleA = new Complex[length];
			var sampleB = new Complex[length];
			var rotor = getRotor(length);

			for (int i = 0, j = 0; i < length; i++) // j+=2
			{
				sampleA[i] = sample[j++];
				sampleB[i] = sample[j++];
			}

			DecimationInTime(ref sampleA, getRotor);
			DecimationInTime(ref sampleB, getRotor);

			for (int i = 0, j = length; i < length; i++, j++)
			{
				var a = sampleA[i];
				var b = sampleB[i] * rotor[i];

				sample[i] = a + b;
				sample[j] = a - b;
			}
		}
	}
}