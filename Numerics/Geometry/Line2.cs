using System;
using System.ComponentModel;
using System.Numerics;
using static System.Math;

namespace JA.Numerics.Geometry
{
    public readonly struct Line2 :
        IEquatable<Line2>,
        IFormattable
    {
        readonly (float a, float b, float c) data;

        public Line2(float a, float b, float c)
        {
            data = (a, b, c);
        }

        public static Line2 Ray(Point2 origin, Vector2 direction)
            => Join(origin, origin + direction);
        public static Line2 ThroughPointAwayFromOrigin(Point2 center)
            => new Line2(-center.W*center.U, -center.W*center.V, center.U*center.U + center.V*center.V);

        public static Line2 Join(Point2 point1, Point2 point2)
            => new Line2(
                point1.V*point2.W - point1.W*point2.V,
                point1.W*point2.U - point1.U*point2.W,
                point1.U*point2.V - point1.V*point2.U);

        public static Line2 Empty { get; } = new Line2(0, 0, 0);
        public static Line2 XAxis { get; } = new Line2(0, 1, 0);
        public static Line2 YAxis { get; } = new Line2(-1, 0, 0);
        public static Line2 Horizon { get; } = new Line2(0, 0, 1);

        public float A => data.a;
        public float B => data.b;
        public float C => data.c;

        public void Deconstruct(out float a, out float b, out float c)
        {
            a = data.a;
            b = data.b;
            c = data.c;
        }
        public void Deconstruct(out float[] coefficients)
        {
            coefficients = new float[] {
                data.a,
                data.b,
                data.c,
            };
        }

        [Browsable(false)]
        public (float a, float b, float c) Coords => data;

        [Browsable(false)]
        public bool IsFinite => WeightSqr > 0;
        [Browsable(false)]
        public bool IsEmpty => data.a != 0 && data.b != 0 && data.c != 0;
        [Browsable(false)]
        public float WeightSqr => data.a*data.a + data.b*data.b;
        [Browsable(false)]
        public float Weight => (float)Sqrt(WeightSqr);

        public bool IsCoincident(Line2 line)
        {
            float w1 = Weight, w2 = line.Weight;

            return data.a*w2 == line.data.a*w1
                && data.b*w2 == line.data.b*w1
                && data.c*w2 == line.data.c*w1;
        }
        public bool IsCoincident(Point2 point)
            => Dot(this, point) == 0;

        public Point2 GetCenter() => new Point2(-data.c*data.a, -data.c*data.b, WeightSqr);
        public Vector2 GetDirection() => new Vector2(data.b, -data.a)/Weight;

        public Line2 Normalized()
        {
            var w = Weight;
            return new Line2(data.a/w, data.b/w, data.c/w);
        }
        #region Geometry

        public Line2 ParallelThrough(Point2 point)
            => new Line2(data.a*point.W, data.b*point.W, -data.a*point.U-data.b*point.V);

        public Point2 GetClosestPoint(Point2 point)
            => new Point2(
                data.b*data.b*point.U - data.a* (data.b*point.V + data.c*point.W),
                data.a*data.a*point.V - data.b* (data.a*point.U + data.c*point.W),
                (data.a*data.a + data.b*data.b)*point.W);

        public Point2 GetClosestPoint(Circle2 circle) => GetClosestPoint(circle.Center);

        public float DistanceTo(Point2 target) => GetClosestPoint(target).DistanceTo(target);
        public float DistanceTo(Circle2 target) => DistanceTo(target.Center) - target.Radius;
        public float SignedDistanceTo(Point2 target) => Dot(this, target)/(Weight*target.Weight);
        public float SignedDistanceTo(Circle2 target)
        {
            var d = SignedDistanceTo(target.Center);
            return d - Math.Sign(d)*target.Radius;
        }

        public Point2 GetPoint(float t)
        {
            var w = Weight;
            return new Point2(data.b*w*t - data.a*data.c, -data.a*w*t-data.b*data.c, w*w);
        }

        public Vector2 VectorTo(Point2 target)
            => GetClosestPoint(target).VectorTo(target);
        public Point2 GetPointAlong(float distance)
        {
            var ab2 = data.a*data.a+data.b*data.b;
            var ab = (float)Math.Sqrt(ab2);
            return new Point2(
                 data.b*distance*ab-data.a*data.c,
                -data.a*distance*ab-data.b*data.c,
                ab2);
        }

        public Point2 GetPointFrom(Point2 point, float distance)
        {
            var t = ParallelDistanceTo(point);
            return GetPointAlong(t+distance);
        }

        public float ParallelDistanceTo(Point2 point)
        {
            var ab2 = data.a*data.a+data.b*data.b;
            var u = point.U;
            var v = point.V;
            var w = point.W;
            return (data.b*u-data.a*v)/(ab2*w);
        }

        public float PerpendicularDistanceTo(Point2 point)
        {
            var ab = (float)Math.Sqrt(data.a*data.a+data.b*data.b);
            var u = point.U;
            var v = point.V;
            var w = point.W;
            return (data.a*u+data.b*v+data.c*w)/(ab*w);
        }

        public Line2 LineParallelThrough(Point2 point)
        {
            var u = point.U;
            var v = point.V;
            var w = point.W;

            return new Line2(data.a*w, data.b*w, -data.a*u-data.b*v);
        }
        public Line2 LinePerpendicularThrough(Point2 point)
        {
            var u = point.U;
            var v = point.V;
            var w = point.W;

            return new Line2(-data.b*w, data.a*w, data.b*u-data.a*v);
        }


        public Point2 ProjectedPoint(Point2 point)
        {
            var ab = (float)Math.Sqrt(data.a*data.a+data.b*data.b);
            var u = point.U;
            var v = point.V;
            var w = point.W;

            return new Point2(
                data.b*data.b*u-data.a*(ab*data.c*w+data.b*v),
                data.a*data.a*v-data.b*(data.a*u+ab*data.c*w),
                w*ab*ab*ab);
        }

        public Point2 IntersectWithLine(Line2 other)
        {
            return new Point2(
                data.b*other.data.c-other.data.b*data.c,
                other.data.a*data.c-data.a*other.data.c,
                data.a*other.data.b-other.data.a*data.b);

        }
        public Line2 Offset(float distance)
        {
            var ab = (float)Math.Sqrt(data.a*data.a+data.b*data.b);
            return new Line2(
                data.a, data.b, data.c-distance*ab);
        }

        public Line2 RotateAbout(Point2 fulcrum, float angle)
        {
            var u = fulcrum.U;
            var v = fulcrum.V;
            var w = fulcrum.W;
            var cθ = (float)Math.Cos(angle);
            var sθ = (float)Math.Sin(angle);

            return new Line2(
                w*(data.a*cθ-data.b*sθ),
                w*(data.b*cθ+data.a*sθ),
                (data.b*u-data.a*v)*sθ-(data.a*u+data.b*v)*cθ+data.a*u+data.b*v+data.c*w);
        }

        public Line2 MirrorAbout(Line2 line)
        {
            //float a1=a;
            //float b1=b;
            //float c1=c;
            var oa = line.A;
            var ob = line.B;
            var oc = line.C;

            return new Line2(
                data.a*(oa*oa-ob*ob)+2*oa*ob*data.b,
                2*oa*ob*data.a+data.b*(ob*ob-oa*oa),
                2*oa*oc*data.a+2*ob*oc*data.b-data.c*(oa*oa+ob*ob));
        }
        #endregion

        #region Algebra
        public static Line2 Negate(Line2 line)
            => new Line2(
                -line.A,
                -line.B,
                -line.C);
        public static Line2 Scale(float factor, Line2 line)
            => new Line2(
                factor*line.A,
                factor*line.B,
                factor*line.C);
        public static Line2 Add(Line2 line1, Line2 line2)
            => new Line2(
                line1.A+line2.A,
                line1.B+line2.B,
                line1.C+line2.C);
        public static Line2 Subtract(Line2 line1, Line2 line2)
            => new Line2(
                line1.A-line2.A,
                line1.B-line2.B,
                line1.C-line2.C);

        public static float Dot(Line2 line, Point2 point)
            => line.data.a * point.U + line.data.b * point.V + line.data.c * point.W;

        public static Line2 operator +(Line2 a, Line2 b) => Add(a, b);
        public static Line2 operator -(Line2 a) => Negate(a);
        public static Line2 operator -(Line2 a, Line2 b) => Subtract(a, b);
        public static Line2 operator *(float f, Line2 a) => Scale(f, a);
        public static Line2 operator *(Line2 a, float f) => Scale(f, a);
        public static Line2 operator /(Line2 a, float d) => Scale(1/d, a);
        public static float operator *(Line2 a, Point2 b) => Dot(a, b);

        public static bool operator ==(Line2 left, Line2 right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Line2 left, Line2 right)
        {
            return !(left==right);
        }
        #endregion

        #region Formatting        
        public override string ToString() => ToString("g");
        public string ToString(string formatting) => ToString(formatting, null);
        public string ToString(string formatting, IFormatProvider formatProvider)
        {
            var a_str = data.a.ToString(formatting, formatProvider);
            var b_str = data.b.ToString(formatting, formatProvider);
            var c_str = data.c.ToString(formatting, formatProvider);

            var text = string.Empty;
            if (data.a != 0)
            {
                text += $"{a_str}*x";
            }
            if (data.b != 0)
            {
                text += $" + {b_str}*y";
            }
            if (data.c != 0)
            {
                text += $" + {c_str}";
            }
            return text + " = 0";
        }
        #endregion

        #region Equality

        public override bool Equals(object obj)
        {
            return obj is Line2 line&& Equals(line);
        }

        public bool Equals(Line2 line)
            => data.Equals(line.data);

        public override int GetHashCode()
        {
            var hashCode = 1768953197;
            hashCode=hashCode*-1521134295+data.GetHashCode();
            return hashCode;
        } 
        #endregion


    }
}
