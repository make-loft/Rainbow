namespace Rainbow
{
	public struct Bin
	{
		public double Frequency { get; set; }
		public double Magnitude { get; set; }
		public double Phase { get; set; }

		public override string ToString() => $"{Magnitude:F4} {Frequency:F4} {Phase:F4}";

		public Bin(in double frequency, in double magnitude, in double phase)
		{
			Frequency = frequency;
			Magnitude = magnitude;
			Phase = phase;
		}

		public void Construct(in double frequency, in double magnitude, in double phase)
		{
			Frequency = frequency;
			Magnitude = magnitude;
			Phase = phase;
		}

		public void Deconstruct(out double frequency, out double magnitude, out double phase)
		{
			frequency = Frequency;
			magnitude = Magnitude;
			phase = Phase;
		}
	}
}
