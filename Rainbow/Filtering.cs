using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace Rainbow
{
	//  Δ∂ωπ
	public static class Filtering
	{
		public const double SinglePi = Math.PI;
		public const double DoublePi = 2*Math.PI;

		public static Dictionary<double, double> GetJoinedSpectrum(
			IList<Complex> spectrum0, IList<Complex> spectrum1,
			double shiftsPerFrame, double sampleRate)
		{
			var frameSize = spectrum0.Count;
			var frameTime = frameSize/sampleRate;
			var shiftTime = frameTime/shiftsPerFrame;
			var binToFrequancy = sampleRate/frameSize;
			var dictionary = new Dictionary<double, double>();

			for (var bin = 0; bin < frameSize; bin++)
			{
				var omegaExpected = DoublePi*(bin*binToFrequancy); // ω=2πf
				var omegaActual = (spectrum1[bin].Phase - spectrum0[bin].Phase)/shiftTime; // ω=∂φ/∂t
				var omegaDelta = Align(omegaActual - omegaExpected, DoublePi); // Δω=(∂ω + π)%2π - π
				var binDelta = omegaDelta/(DoublePi*binToFrequancy);
				var frequancyActual = (bin + binDelta)*binToFrequancy;
				var magnitude = spectrum1[bin].Magnitude + spectrum0[bin].Magnitude;
				dictionary.Add(frequancyActual, magnitude*(0.5 + Math.Abs(binDelta)));
			}

			return dictionary;
		}

		public static double Align(double angle, double period)
		{
			var qpd = (int) (angle/period);
			if (qpd >= 0) qpd += qpd & 1;
			else qpd -= qpd & 1;
			angle -= period*qpd;
			return angle;
		}

		public static Dictionary<double, double> Antialiasing(Dictionary<double, double> spectrum)
		{
			var result = new Dictionary<double, double>();
			var data = spectrum.ToList();
			for (var j = 0; j < spectrum.Count - 4; j++)
			{
				var i = j;
				var x0 = data[i].Key;
				var x1 = data[i + 1].Key;
				var y0 = data[i].Value;
				var y1 = data[i + 1].Value;

				var a = (y1 - y0)/(x1 - x0);
				var b = y0 - a*x0;

				i += 2;
				var u0 = data[i].Key;
				var u1 = data[i + 1].Key;
				var v0 = data[i].Value;
				var v1 = data[i + 1].Value;

				var c = (v1 - v0)/(u1 - u0);
				var d = v0 - c*u0;

				var x = (d - b)/(a - c);
				var y = (a*d - b*c)/(a - c);

				if (y > y0 && y > y1 && y > v0 && y > v1 &&
					x > x0 && x > x1 && x < u0 && x < u1)
				{
					result.Add(x1, y1);
					result.Add(x, y);
				}
				else
				{
					result.Add(x1, y1);
				}
			}

			return result;
		}
	}
}