using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;
using static System.Math;

namespace JA.Numerics.Geometry
{
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public readonly struct Circle2 :
        IEquatable<Circle2>,
        IFormattable
    {
        public Circle2(float radius)
            : this(Point2.Origin, radius)
        {
        }
        public Circle2(Point2 center, float radius)
        {
            Center = center;
            Radius=radius;
        }
        public static Circle2 FromDiagonals(Point2 point, Point2 other)
        {
            Point2 center = Point2.Add(point, other);

            return Circle2.FromCenterAndPoint(center, point);
        }

        public static Circle2 FromCenterAndPoint(Point2 center, Point2 point)
        {
            float radius = center.DistanceTo(point);
            return new Circle2(center, radius);
        }

        public float Radius
        {
            get; 
        }
        public Point2 Center
        {
            get ; 
        }

        public Point2 GetPoint(float t)
        {
            Vector2 delta = Geometry.Polar(Radius, t);
            return Center + delta;
        }

        public Point2 GetClosestPoint(Point2 point)
        {
            Vector2 delta = Center.VectorTo(point);
            var (_, θ)= delta.ToPolar();
            delta = Geometry.Polar(Radius, θ);
            return Center + delta;
        }

        public Point2 GetClosestPoint(Line2 line)
            => GetClosestPoint(line.GetClosestPoint(Center));

        public float DistanceTo(Point2 point)
            => Center.DistanceTo(point) - Radius;
        public float DistanceTo(Circle2 circle)
            => Center.DistanceTo(circle.Center) - Radius - circle.Radius;
        public float DistanceTo(Line2 line)
            => Center.DistanceTo(line) - Radius;

        public bool Intersect(Circle2 other, out IReadOnlyList<Point2> solutions)
        {
            float d = Center.DistanceTo(other.Center);
            if (d > Radius + other.Radius || d < Abs(Radius - other.Radius))
            {
                solutions = Array.Empty<Point2>();
                return false;
            }
            float a = (Radius * Radius - other.Radius * other.Radius + d * d) / (2 * d);
            float h = (float)Sqrt(Radius * Radius - a * a);
            Vector2 P0 = Center.AsVector();
            Vector2 P1 = other.Center.AsVector();
            Vector2 P2 = P0 + a * (P1 - P0) / d;
            float rx = -(P1.Y - P0.Y) * (h / d);
            float ry = (P1.X - P0.X) * (h / d);
            Point2 intersection1 = Point2.FromVector(new Vector2(P2.X + rx, P2.Y + ry));
            Point2 intersection2 = Point2.FromVector(new Vector2(P2.X - rx, P2.Y - ry));
            var points = new List<Point2>();
            points.Add(intersection1);
            if (intersection1 != intersection2)
            {
                points.Add(intersection2);
            }
            solutions=points.AsReadOnly();
            return true;
        }

        #region Equality
        public override bool Equals(object obj) => obj is Circle2 circle && Equals(circle);

        public bool Equals(Circle2 other)
        {
            return Radius==other.Radius&&
                   Center.Equals(other.Center);
        }

        public override int GetHashCode()
        {
            var hashCode = -1545607722;
            hashCode=hashCode*-1521134295+Radius.GetHashCode();
            hashCode=hashCode*-1521134295+EqualityComparer<Point2>.Default.GetHashCode(Center);
            return hashCode;
        }

        public static bool operator ==(Circle2 circle1, Circle2 circle2)
        {
            return circle1.Equals(circle2);
        }

        public static bool operator !=(Circle2 circle1, Circle2 circle2)
        {
            return !(circle1==circle2);
        }
        #endregion

        #region Formatting
        public string ToString(string formatting, IFormatProvider provider)
        {
            string x_str = Center.AsVector().X.ToString(formatting, provider);
            string y_str = Center.AsVector().Y.ToString(formatting, provider);
            string r_str = Radius.ToString(formatting, provider);
            return $"Circle(x={x_str}, y={y_str}, r={r_str})";
        }
        public string ToString(string formatting)
            => ToString(formatting, null);
        public override string ToString()
            => ToString("g4");
        #endregion
    }
}
