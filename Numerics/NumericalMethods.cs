using System;
using System.Diagnostics;
using static System.Math;

namespace JA.Numerics
{
    public enum SolutionSet
    {
        First,
        Second,
    }

    public static class NumericalMethods
    {
        public static int MaxIterations { get; } = 512;
        public static float LooseTolerance { get; } = 1e-6f;

        #region Single
        public static bool GaussPointIteration(this Func<float, float> f, float x_init, float tol, out float x)
        {
            x = x_init;
            Debug.WriteLine(f.ToString());
            float x_old;
            int iter = 0;
            do
            {
                iter++;
                x_old = x;
                x = f(x);
                Debug.WriteLine($"iter={iter}, x={x}");
            } while (iter < MaxIterations &&  Abs(x-x_old)>tol);

            return iter < MaxIterations;
        }
        public static bool BisectionRoot(this Func<float, float> f, float x_low, float x_high, float tol, out float x)
        {
            float f_low = f(x_low), f_high = f(x_high);

            if (Abs(f_low)<=tol)
            {
                x = x_low;
                return true;
            }

            if (Abs(f_high)<=tol)
            {
                x = x_high;
                return true;
            }

            int iter = 0;

            while (iter <= MaxIterations && f_low*f_high > 0)
            {
                iter++;
                (x_low, x_high) =((3*x_low-x_high)/2, (3*x_high-x_low)/2);
                (f_low, f_high) =(f(x_low), f(x_high));
            }

            if (iter>MaxIterations)
                throw new ArgumentException("Invalid Initial Conditions for Bisection.");

            iter = 0;
            float f_mid;
            do
            {
                iter++;
                x = (x_low + x_high)/2;
                f_mid = f(x);
                if (Abs(f_mid) <= tol)
                {
                    return true;
                }

                if (f_low*f_mid<0)
                {
                    x_high = x;
                    f_high = f_mid;
                }
                else if (f_high*f_mid<0)
                {
                    x_low = x;
                    f_low = f_mid;
                }
                else
                    throw new InvalidOperationException();

            } while (iter < MaxIterations && Abs(x_high-x_low)>2*tol);

            return iter < MaxIterations;
        }
        #endregion

        #region Double
        public static bool GaussPointIteration(this Func<double, double> f, double x_init, double tol, out double x)
        {
            x = x_init;
            double x_old;
            int iter = 0;
            do
            {
                iter++;
                x_old = x;
                x = f(x);
            } while (iter < MaxIterations &&  Abs(x-x_old)>tol);

            return iter < MaxIterations;
        }
        public static bool BisectionRoot(this Func<double, double> f, double x_low, double x_high, double tol, out double x)
        {
            double f_low = f(x_low), f_high = f(x_high);

            if (Abs(f_low)<=tol)
            {
                x = x_low;
                return true;
            }

            if (Abs(f_high)<=tol)
            {
                x = x_high;
                return true;
            }

            int iter = 0;

            while (iter <= MaxIterations && f_low*f_high > 0)
            {
                iter++;
                (x_low, x_high) =((3*x_low-x_high)/2, (3*x_high-x_low)/2);
                (f_low, f_high) =(f(x_low), f(x_high));
            }

            if (iter>MaxIterations)
                throw new ArgumentException("Invalid Initial Conditions for Bisection.");

            iter = 0;
            double f_mid;
            do
            {
                iter++;
                x = (x_low + x_high)/2;
                f_mid = f(x);
                if (Abs(f_mid) <= tol)
                {
                    return true;
                }

                if (f_low*f_mid<0)
                {
                    x_high = x;
                    f_high = f_mid;
                }
                else if (f_high*f_mid<0)
                {
                    x_low = x;
                    f_low = f_mid;
                }
                else
                    throw new InvalidOperationException();

            } while (iter < MaxIterations && Abs(x_high-x_low)>2*tol);

            return iter < MaxIterations;
        }
        #endregion
    }
}
