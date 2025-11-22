using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Numerics;

using static System.Math;

namespace JA.Numerics.Geometry
{
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public readonly struct Ellipse2 :
        IEquatable<Ellipse2>,
        IFormattable
    {
        public Ellipse2(float majorAxis, float minorAxis)
            : this(Point2.Origin, majorAxis, minorAxis)
        { }
        public Ellipse2(Point2 center, float majorAxis, float minorAxis)
        {
            Center = center;
            MajorAxis=majorAxis;
            MinorAxis=minorAxis;
        }

        public float MajorAxis { get; }
        public float MinorAxis { get; }
        public Point2 Center { get; }

        public float[,] GetCoefficients()
        {
            float a2 = MajorAxis*MajorAxis, b2 = MinorAxis*MinorAxis;
            float cx = Center.U, cy = Center.V;
            return new[,]
            {
                { 1/a2, 0, -cx/a2 },
                { 0, 1/b2, -cy/b2 },
                { -cx/a2, -cy/b2, cx*cx/a2+cy*cy/b2 }
            };
        }
        public void Deconstruct(out float A, out float B, out float C, out float D, out float E, out float F)
        {
            float a2 = MajorAxis*MajorAxis, b2 = MinorAxis*MinorAxis;
            float cx = Center.U, cy = Center.V;
            A = 1/a2;
            B = 0;
            C = 1/b2;
            D = -cx/a2;
            E = -cy/b2;
            F = cx*cx/a2+cy*cy/b2;
        }

        public Point2 GetPoint(float t)
        {
            var delta = Geometry.Elliptical(MajorAxis, MinorAxis, t);
            return Center + delta;
        }
        public Point2 GetClosestPoint(Circle2 circle, float tol)
            => GetClosestPoint(circle.Center, tol);

        public Point2 GetClosestPoint(Point2 point, float tol)
        {
            var delta = Center.VectorTo(point);
            var sign = Math.Sign(delta.X);
            var Q = MajorAxis*MajorAxis-MinorAxis*MinorAxis;
            var A = 2*delta.X*MajorAxis/Q;
            var B = 2*delta.Y*MinorAxis/Q;
            float t;
            if (A!=0)
            {

                float IterFun(float z)
                {
                    return 1/A*(B + (sign)* 2*z/(float)Sqrt(1+z*z));
                }

                var converge = NumericalMethods.GaussPointIteration(IterFun, 0, tol, out var z_sol);
                if (!float.IsInfinity(z_sol) && !float.IsNaN(z_sol))
                {
                    t = sign == 1 ? (float)Atan(z_sol) : (float)Atan(z_sol) + (float)PI;
                }
                else
                {
                    // eh:
                    t = 0;
                }
            }
            else
            {
                t = Sign(B)*(float)(PI/2);
            }
            return GetPoint(t);
        }
        public Point2 GetClosestPoint(Line2 line, SolutionSet solution)
        {
            (var A, var B, var C, var D, var E, var F) = this;
            (var G, var H, var I) = line;
            var sign = (int)solution == 0 ? -1 : 1;
            float u = A*H-B*G, v = B*H-C*G, w = D*H-E*G;
            var P = A*v*v - u*(2*B*v-C*u);
            float vv = v*v, uu = u*u, uuvv = uu+vv;
            var Q = v*w*(2*B*v+u*(A-C))/uuvv + (E*u-D*v-B*w);
            var R = w*w*(A*uu+2*B*u*v+C*vv)/(uuvv*uuvv)+2*w*(E*v+D*u)/uuvv+F;
            var d = Q*Q - P * R;
            if (d>=0)
            {
                var t = (sign*(float)Sqrt(d)-Q)/P;
                return new Point2(
                    -t*v*uuvv-u*w,
                     t*v*uuvv-v*w,
                     uuvv);
            }

            throw new NotImplementedException();
        }
        public Point2 GetClosestPoint(Line2 line, float tol)
        {
            var (a, b, c)= line.Coords;
            var cen = Center.AsVector();
            var (cx, cy) = (cen.X, cen.Y);
            // transform line coords to being relative to ellipse center
            c -= -a*cx - b*cy;
            float rx = MajorAxis, ry = MinorAxis;
            float t;
            if (a != 0)
            {

                float IterFun(float z)
                {
                    return (b*ry-a*rx*z)*(a*rx+b*ry*z)/(a*c*rx*(float)Sqrt(1+z*z))+(b*ry)/(a*rx);
                }
                var converge = NumericalMethods.GaussPointIteration(IterFun, 0, tol, out var z_sol);
                if (!float.IsInfinity(z_sol) && !float.IsNaN(z_sol))
                {
                    t = (float)Atan(z_sol);
                }
                else
                {
                    // eh!
                    t = 0;
                }
            }
            else
            {
                t = (float)PI/2;
            }

            return GetPoint(t);

        }

        public float DistanceTo(Point2 point, float tol)
            => GetClosestPoint(point, tol).DistanceTo(point);
        public float DistanceTo(Circle2 circle, float tol)
            => GetClosestPoint(circle.Center, tol).DistanceTo(circle);
        public float DistanceTo(Line2 line, float tol)
            => GetClosestPoint(line, tol).DistanceTo(line);
        public float DistanceTo(Line2 line, SolutionSet solution)
            => GetClosestPoint(line, solution).DistanceTo(line);

        #region Equality
        public override bool Equals(object obj) => obj is Ellipse2 ellipse && Equals(ellipse);

        public bool Equals(Ellipse2 other)
        {
            return MajorAxis==other.MajorAxis&&
                   MinorAxis==other.MinorAxis&&
                   Center.Equals(other.Center);
        }

        public override int GetHashCode()
        {
            var hashCode = -1545607722;
            hashCode=hashCode*-1521134295+MajorAxis.GetHashCode();
            hashCode=hashCode*-1521134295+MinorAxis.GetHashCode();
            hashCode=hashCode*-1521134295+EqualityComparer<Point2>.Default.GetHashCode(Center);
            return hashCode;
        }

        public static bool operator ==(Ellipse2 ellipse1, Ellipse2 ellipse2)
        {
            return ellipse1.Equals(ellipse2);
        }

        public static bool operator !=(Ellipse2 ellipse1, Ellipse2 ellipse2)
        {
            return !(ellipse1==ellipse2);
        }
        #endregion

        #region Formatting
        public string ToString(string formatting, IFormatProvider provider)
        {
            var x_str = Center.AsVector().X.ToString(formatting, provider);
            var y_str = Center.AsVector().Y.ToString(formatting, provider);
            var a_str = MajorAxis.ToString(formatting, provider);
            var b_str = MinorAxis.ToString(formatting, provider);
            return $"Ellipse(x={x_str}, y={y_str}, rx={a_str}, ry={b_str})";
        }
        public string ToString(string formatting)
            => ToString(formatting, null);
        public override string ToString()
            => ToString("g4");
        #endregion

    }
}
