// Type: System.Numerics.Complex
// Assembly: System.Numerics, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 2BCD559E-1E00-4581-80D1-080BCD16D4B6
// Assembly location: C:\Windows\Microsoft.NET\Framework\v4.0.30319\System.Numerics.dll

using System;
using System.Globalization;

// ReSharper disable once CheckNamespace
// ReSharper disable CompareOfFloatsByEqualityOperator

namespace Rainbow
{
	public struct Complex : IEquatable<Complex>, IFormattable
	{
		public static readonly Complex Zero = new Complex();
		public static readonly Complex RealOne = new Complex(1.0);
		public static readonly Complex RealTwo = new Complex(2.0);
		public static readonly Complex ImaginaryOne = new Complex(0.0, 1.0);
		public static readonly Complex One = RealOne;

        public Complex(double real = 0.0, double imaginary = 0.0)
		{
			Real = real;
			Imaginary = imaginary;
		}

        public double Real { get; }
        public double Imaginary { get; }
        public double Magnitude => Abs(this);
		public double Phase => Math.Atan2(Imaginary, Real);
		
		public bool Is(Complex value) => Real == value.Real && Imaginary == value.Imaginary;
		public bool IsNot(Complex value) => Real != value.Real && Imaginary != value.Imaginary;
		public bool Is(object obj) => obj is Complex value && Is(value);
		public bool IsNot(object obj) => !Is(obj);
		
		public bool Equals(Complex value) => Is(value);
		public override bool Equals(object obj) => Is(obj);
		
		public static bool operator ==(Complex left, Complex right) => left.Is(right);
		public static bool operator !=(Complex left, Complex right) => left.IsNot(right);

		public static implicit operator Complex(short value) => new Complex(value);
		public static implicit operator Complex(int value) => new Complex(value);
		public static implicit operator Complex(long value) => new Complex(value);
		public static implicit operator Complex(ushort value) => new Complex(value);
		public static implicit operator Complex(uint value) => new Complex(value);
		public static implicit operator Complex(ulong value) => new Complex(value);
		public static implicit operator Complex(sbyte value) => new Complex(value);
		public static implicit operator Complex(byte value) => new Complex(value);
		public static implicit operator Complex(float value) => new Complex(value);
		public static implicit operator Complex(double value) => new Complex(value);

		public static explicit operator Complex(decimal value) => new Complex((double) value);

		public static Complex operator +(Complex value) => new Complex(+value.Real, +value.Imaginary);
		public static Complex operator -(Complex value) => new Complex(-value.Real, -value.Imaginary);

		public static Complex operator +(Complex left, Complex right) => new Complex(
			left.Real + right.Real, left.Imaginary + right.Imaginary);

		public static Complex operator -(Complex left, Complex right) => new Complex(
			left.Real - right.Real, left.Imaginary - right.Imaginary);

		public static Complex operator *(Complex left, Complex right) => new Complex(
			left.Real * right.Real - left.Imaginary * right.Imaginary,
			left.Imaginary * right.Real + left.Real * right.Imaginary);

		public static Complex operator /(Complex left, Complex right)
		{
			var d2 = right.Real;
			var d3 = right.Imaginary;
			var flag = Math.Abs(d3) < Math.Abs(d2);
			var d0 = flag ? left.Real : left.Imaginary;
			var d1 = flag ? left.Imaginary : left.Real;
			
			var d4 = d3 / d2;
			var d5 = d2 + d3 * d4;
			var d6 = flag ? +d5 : -d5;
			return new Complex((d0 + d1 * d4) / d6, (d1 - d0 * d4) / d6);
		}

		public static Complex Negate(Complex value) => -value;
		public static Complex Add(Complex left, Complex right) => left + right;
		public static Complex Subtract(Complex left, Complex right) => left - right;
		public static Complex Multiply(Complex left, Complex right) => left * right;
		public static Complex Divide(Complex dividend, Complex divisor) => dividend / divisor;

		public static double Abs(Complex value) => Abs(value.Real, value.Imaginary);
		
		public static double Abs(double real, double imaginary)
		{
			if (double.IsInfinity(real) || double.IsInfinity(imaginary))
				return double.PositiveInfinity;
			var d0 = Math.Abs(real);
			var d1 = Math.Abs(imaginary);
			if (d0 > d1)
			{
				var d2 = d1/d0;
				return d0*Math.Sqrt(1.0 + d2*d2);
			}
			else
			{
				if (d1 == 0.0)
					return d0;
				var d2 = d0/d1;
				return d1*Math.Sqrt(1.0 + d2*d2);
			}
		}

		public static Complex Conjugate(Complex value) => new Complex(value.Real, -value.Imaginary);
		public static Complex Reciprocal(Complex value) => value.Is(Zero) ? Zero : One / value;
		
		public string ToString(string format, IFormatProvider provider) =>
			string.Format(provider, "({0}, {1})",
				Real.ToString(format, provider),
				Imaginary.ToString(format, provider));

		public override string ToString() => string.Format(CultureInfo.CurrentCulture, "({0}, {1})", Real, Imaginary);

		public string ToString(string format) => string.Format(CultureInfo.CurrentCulture, "({0}, {1})",
			Real.ToString(format, CultureInfo.CurrentCulture),
			Imaginary.ToString(format, CultureInfo.CurrentCulture));

		public string ToString(IFormatProvider provider) => string.Format(provider, "({0}, {1})", Real, Imaginary);

		public override int GetHashCode() => Real.GetHashCode() % 99999997 ^ Imaginary.GetHashCode();

		public static Complex Sin(double real, double imaginary) =>
			new Complex(Math.Sin(real) * Math.Cosh(imaginary), Math.Cos(real) * Math.Sinh(imaginary));

		public static Complex Sinh(double real, double imaginary) =>
			new Complex(Math.Sinh(real) * Math.Cos(imaginary), Math.Cosh(real) * Math.Sin(imaginary));

		public static Complex Cos(double real, double imaginary) =>
			new Complex(Math.Cos(real) * Math.Cosh(imaginary), -(Math.Sin(real) * Math.Sinh(imaginary)));
		
		public static Complex Cosh(double real, double imaginary) =>
			new Complex(Math.Cosh(real) * Math.Cos(imaginary), Math.Sinh(real) * Math.Sin(imaginary));

		public static Complex Sin(Complex value) => Sin(value.Real, value.Imaginary);
		public static Complex Sinh(Complex value) => Sinh(value.Real, value.Imaginary);
		public static Complex Asin(Complex value) => -ImaginaryOne * Log(ImaginaryOne * value + Sqrt(One - value * value));

		public static Complex Cos(Complex value) => Cos(value.Real, value.Imaginary);	  
		public static Complex Cosh(Complex value) => Cosh(value.Real, value.Imaginary);
		public static Complex Acos(Complex value) => -ImaginaryOne * Log(value + ImaginaryOne * Sqrt(One - value * value));

		public static Complex Tan(Complex value) => Sin(value) / Cos(value);
		public static Complex Tanh(Complex value) => Sinh(value) / Cosh(value);
		public static Complex Atan(Complex value) => ImaginaryOne / RealTwo * (Log(One - ImaginaryOne * value) - Log(One + ImaginaryOne * value));

		public static Complex Log(Complex value) =>
			new Complex(Math.Log(Abs(value)), Math.Atan2(value.Imaginary, value.Real));

		public static Complex Log(Complex value, double baseValue) => Log(value) / Log(baseValue);
		public static Complex Log10(Complex value) => Scale(Log(value), 0.43429448190325);

		public static Complex FromPolarCoordinates(double magnitude, double phase) =>
			new Complex(magnitude * Math.Cos(phase), magnitude * Math.Sin(phase));
		
		public static Complex Exp(Complex value) => FromPolarCoordinates(Math.Exp(value.Real), value.Imaginary);
		public static Complex Sqrt(Complex value) => FromPolarCoordinates(Math.Sqrt(value.Magnitude), value.Phase / 2.0);

		public static Complex Pow(Complex value, Complex power)
		{
			if (power == Zero)
				return One;
			if (value == Zero)
				return Zero;
			var x = value.Real;
			var y1 = value.Imaginary;
			var y2 = power.Real;
			var num1 = power.Imaginary;
			var num2 = Abs(value);
			var num3 = Math.Atan2(y1, x);
			var num4 = y2 * num3 + num1 * Math.Log(num2);
			var num5 = Math.Pow(num2, y2) * Math.Pow(Math.E, -num1 * num3);
			return new Complex(num5 * Math.Cos(num4), num5 * Math.Sin(num4));
		}

		public static Complex Pow(Complex value, double power) => Pow(value, new Complex(power));

		public static Complex Scale(Complex value, double factor) => Scale(value.Real, value.Imaginary, factor);
		
		public static Complex Scale(double real, double imaginary, double factor) =>
			new Complex(factor * real, factor * imaginary);
	}
}