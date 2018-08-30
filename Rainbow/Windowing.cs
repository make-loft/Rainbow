using System;

namespace Rainbow
{
    public class Windowing
    {
        private const double Q = 0.5;

        public static double Rectangle(double n, double frameSize) => 1;

        public static double Gausse(double n, double frameSize, double q = Q)
        {
            var a = (frameSize - 1) / 2;
            var t = (n - a) / (q * a);
            return Math.Exp(-t * (t / 2));
        }

        public static double Hamming(double n, double frameSize) =>
            0.54 - 0.46 * Math.Cos(2 * Math.PI * n / (frameSize - 1));

        public static double Hann(double n, double frameSize) =>
            0.5 * (1.0 - Math.Cos(2 * Math.PI * n / (frameSize - 1)));

        public static double BlackmanHarris(double n, double frameSize) =>
            0.35875 -
            0.48829 * Math.Cos(2 * Math.PI * n / (frameSize - 1)) +
            0.14128 * Math.Cos(4 * Math.PI * n / (frameSize - 1)) -
            0.01168 * Math.Cos(4 * Math.PI * n / (frameSize - 1));


        public static Complex WaveWavelet(Complex n, Complex frameSize) => WaveWavelet(n / frameSize);
        public static double SombreroWavelet(double n, double frameSize) => SombreroWavelet(n / frameSize);
        public static double DOGWavelet(double n, double frameSize) => DOGWavelet(n / frameSize);
		
        private static Complex WaveWavelet(Complex t) => -t * Math.Exp((-t * t / 2.0).Real);	   
        private static double SombreroWavelet(double t) => (t * t - 1) * Math.Exp(-t * t / 2);	
        private static double DOGWavelet(double t) => Math.Exp(-t * t / 2) - Math.Exp(-t * t / 8) / 2;
    }
}