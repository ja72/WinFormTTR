using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Numerics;
using System.Runtime.InteropServices.ComTypes;

namespace JA.Numerics.Geometry
{
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public readonly struct Point2 :
        IEquatable<Point2>,
        IFormattable
    {
        readonly (float x, float y, float w) data;

        public Point2(Vector2 vector, float w = 1) : this(vector.X, vector.Y, w) { }
        public Point2(float u, float v, float w) : this()
        {
            data = (u, v, w);
        }
        public static Point2 FromVector(Vector2 vector)
            => new Point2(vector.X, vector.Y, 1);
        public static Point2 FromCoordinates(float x, float y)
            => new Point2(x, y, 1);
        public static Point2 Meet(Line2 line1, Line2 line2)
            => new Point2(
                line1.B * line2.C - line1.C * line2.B,
                line1.C * line2.A - line1.A * line2.C,
                line1.A * line2.B - line1.B * line2.A);

        public static Point2 Empty { get; } = new Point2(0, 0, 0);
        public static Point2 Origin { get; } = new Point2(0, 0, 1);
        public static Point2 AlongX { get; } = new Point2(1, 0, 0);
        public static Point2 AlongY { get; } = new Point2(0, 1, 0);

        public float U => data.x;
        public float V => data.y;
        public float W => data.w;

        [Browsable(false)]
        public (float x, float y, float w) Coords => data;

        [Browsable(false)]
        public bool IsFinite => data.w != 0;
        [Browsable(false)]
        public bool IsEmpty => data.x != 0 && data.y != 0 && data.w != 0;
        [Browsable(false)]
        public float WeightSqr => data.w*data.w;
        [Browsable(false)]
        public float Weight => data.w;

        public Vector2 AsVector() => new Vector2(data.x/data.w, data.y/data.w);
        public Vector2 AsVectorFrom(Point2 origin) => origin.VectorTo(this);
        public Point2 Normalized() => Origin + AsVector();

        public bool IsCoincident(Line2 line)
            => Dot(this, line) == 0;

        public bool IsCoincident(Point2 other)
            => AsVector().Equals(other.AsVector());
        public bool IsCoincident(Point2 other, float tolerance)
        {
            var P = AsVector();
            var Q = other.AsVector();
            return Math.Abs(P.X - Q.X) <= tolerance
                && Math.Abs(P.Y - Q.Y) <= tolerance;
        }
        public Vector2 VectorTo(Point2 target)
            => Difference(target, this);

        public float DistanceTo(Point2 b) 
            => this.VectorTo(b).Length();

        public float DistanceTo(Circle2 circle)
            => circle.DistanceTo(this);

        public float DistanceTo(Line2 target) => Dot(this, target)/(Weight*target.Weight);

        #region Equality
        public override bool Equals(object obj)
        {
            return obj is Point2 point && Equals(point);
        }
        public bool Equals(Point2 other)
            => data.Equals(other.data);

        public override int GetHashCode()
        {
            var hashCode = 1861411795;
            hashCode=hashCode*-1521134295+data.GetHashCode();
            return hashCode;
        }

        public static bool operator ==(Point2 point1, Point2 point2)
        {
            return point1.Equals(point2);
        }

        public static bool operator !=(Point2 point1, Point2 point2)
        {
            return !(point1==point2);
        }
        #endregion

        #region Algebra
        public static Point2 Negate(Point2 a)
            => new Point2(
                -a.data.x,
                -a.data.y,
                -a.data.w);
        public static Point2 Scale(float factor, Point2 a)
            => new Point2(
                factor*a.data.x,
                factor*a.data.y,
                factor*a.data.w);
        public static Point2 Add(Point2 a, Point2 b)
            => new Point2(
                a.data.x+b.data.x,
                a.data.y+b.data.y,
                a.data.w+b.data.w);
        public static Point2 Subtract(Point2 a, Point2 b)
            => new Point2(
                a.data.x-b.data.x,
                a.data.y-b.data.y,
                a.data.w-b.data.w);

        public static Point2 Add(Point2 a, Vector2 b)
            => new Point2(
                a.data.x+a.data.w*b.X,
                a.data.y+a.data.w*b.Y,
                a.data.w);

        public static Point2 Subtract(Point2 a, Vector2 b)
            => new Point2(
                a.data.x-a.data.w*b.X,
                a.data.y-a.data.w*b.Y,
                a.data.w);

        public static Vector2 Difference(Point2 target, Point2 reference)
        {
            float wa = target.data.w, wb = reference.data.w;
            var dx = wb*target.data.x - wa*reference.data.x;
            var dy = wb*target.data.y - wa*reference.data.y;
            return new Vector2(dx, dy)/(wa*wb);
        }

        public static float Dot(Point2 point, Line2 line)
            => line.A* point.data.x + line.B * point.data.y + line.C * point.data.w;

        //public static Point2 operator -(Point2 a) => Negate(a);
        //public static Point2 operator +(Point2 a, Point2 b) => Add(a, b);
        //public static Point2 operator *(float f, Point2 a) => Scale(f, a);
        //public static Point2 operator *(Point2 a, float f) => Scale(f, a);
        //public static Point2 operator /(Point2 a, float d) => Scale(1/d, a);
        public static Point2 operator +(Point2 a, Vector2 b) => Add(a, b);
        public static Point2 operator -(Point2 a, Vector2 b) => Subtract(a, b);
        public static Vector2 operator -(Point2 a, Point2 b) => Difference(a, b);
        public static float operator *(Point2 a, Line2 b) => Dot(a, b);
        #endregion

        #region Formatting
        public string ToString(string formatting, IFormatProvider provider)
        {
            var x_str = AsVector().X.ToString(formatting, provider);
            var y_str = AsVector().Y.ToString(formatting, provider);
            return $"Point(x={x_str}, y={y_str})";
        }
        public string ToString(string formatting)
            => ToString(formatting, null);
        public override string ToString()
            => ToString("g4");
        #endregion
    }
}
