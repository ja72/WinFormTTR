using System;
using System.Numerics;

namespace JA
{
    public readonly struct Line2 :
        ICloneable,
        IEquatable<Line2>
    {
        //line is a*x+b*y+c=0
        readonly float a, b, c;

        #region Factory
        public Line2(float a, float b, float c)
        {
            this.a=a;
            this.b=b;
            this.c=c;
        }
        public Line2(Vector2 vector, float scalar)
        {
            this.a=vector.X;
            this.b=vector.Y;
            this.c=scalar;
        }
        public Line2(Line2 other) { this=other; }
        public static Line2 FromPosition(Vector2 r)
        {
            return new Line2(
                r.X,
                r.Y,
                -Vector2.Dot(r, r));
        }
        public static Line2 FromPosition(Point2 P)
        {
            return new Line2(
                P.U*P.W,
                P.V*P.W,
                -P.U*P.U-P.V*P.V);
        }
        public static Line2 FromTwoPoints(Vector2 r1, Vector2 r2)
        {
            return new Line2(
                a: r1.Y-r2.Y,
                b: r2.X-r1.X,
                c: r1.X*r2.Y-r1.Y*r2.X);
        }
        public static Line2 FromPointAndDirection(Vector2 r, Vector2 e)
        {
            return new Line2(
                -e.Y,
                e.X,
                r.X*e.Y-r.Y*e.X);
        }
        public static Line2 FromTwoPoints(Point2 P, Point2 Q)
        {
            return new Line2(
                P.V*Q.W-P.W*Q.V,
                P.W*Q.U-P.U*Q.W,
                P.U*Q.V-P.V*Q.U);
        }
        public static Line2 FromPointAndDirection(Point2 P, Vector2 e)
        {
            return new Line2(
                -e.Y*P.W,
                e.X*P.W,
                P.U*e.Y-P.V*e.X);
        }
        public static Line2 LinearCombination(Line2 A, Line2 B, float wA, float wB)
        {
            return new Line2(wA*A.Vector+wB*B.Vector, wA*A.Scalar+wB*B.Scalar);
        }

        #endregion

        #region Properties
        public int Count { get { return 3; } }
        public float A { get { return a; } }
        public float B { get { return b; } }
        public float C { get { return c; } }
        public float Magnitude { get { return (float)Math.Sqrt(a*a+b*b); } }
        public Vector2 Vector { get { return new Vector2(a, b); } }
        public float Scalar { get { return c; } }
        public Point2 Position
        {
            get
            {
                float ab2 = a*a+b*b;
                return new Point2(-a*c, -b*c, ab2);
            }
        }
        public float Distance
        {
            get
            {
                float ab = (float)Math.Sqrt(a*a+b*b);
                return c/ab;
            }
        }
        public Vector2 Direction
        {
            get
            {
                float ab = (float)Math.Sqrt(a*a+b*b);
                return new Vector2(b/ab, -a/ab);
            }
        }
        public Vector2 Normal
        {
            get
            {
                float ab = (float)Math.Sqrt(a*a+b*b);
                return new Vector2(a/ab, b/ab);
            }
        }

        public string Equation
        {
            get { return string.Format("{0}x{1}y{2}=0", a.ToString("+#;-#;+0"), b.ToString("+#;-#;+0"), c.ToString("+#;-#;+0")); }
        }

        #endregion

        #region Methods

        public float SumSquares() { return a*a+b*b; }

        public float Dot(Point2 P)
        {
            return a*P.U+b*P.V+c*P.W;
        }

        public Point2 GetPointAlong(float distance)
        {
            float ab2 = a*a+b*b;
            float ab = (float)Math.Sqrt(ab2);
            return new Point2(
                b*distance*ab-a*c,
                -a*distance*ab-b*c,
                ab2);
        }

        public Point2 GetPointFrom(Point2 point, float distance)
        {
            float t = ParallelDistanceTo(point);
            return GetPointAlong(t+distance);
        }

        public float ParallelDistanceTo(Point2 point)
        {
            float ab2 = a*a+b*b;
            float u = point.U;
            float v = point.V;
            float w = point.W;
            return (b*u-a*v)/(ab2*w);
        }

        public float PerpendicularDistanceTo(Point2 point)
        {
            float ab = (float)Math.Sqrt(a*a+b*b);
            float u = point.U;
            float v = point.V;
            float w = point.W;
            return (a*u+b*v+c*w)/(ab*w);
        }

        public Line2 LineParallelThrough(Point2 point)
        {
            float u = point.U;
            float v = point.V;
            float w = point.W;

            return new Line2(a*w, b*w, -a*u-b*v);
        }
        public Line2 LinePerpendicularThrough(Point2 point)
        {
            float u = point.U;
            float v = point.V;
            float w = point.W;

            return new Line2(-b*w, a*w, b*u-a*v);
        }


        public Point2 ProjectedPoint(Point2 point)
        {
            float ab = (float)Math.Sqrt(a*a+b*b);
            float u = point.U;
            float v = point.V;
            float w = point.W;

            return new Point2(
                b*b*u-a*(ab*c*w+b*v),
                a*a*v-b*(a*u+ab*c*w),
                w*ab*ab*ab);
        }

        public Point2 IntersectWithLine(Line2 other)
        {
            return new Point2(
                b*other.c-other.b*c,
                other.a*c-a*other.c,
                a*other.b-other.a*b);

        }
        public Line2 Offset(float distance)
        {
            float ab = (float)Math.Sqrt(a*a+b*b);
            return new Line2(
                a, b, c-distance*ab);
        }

        public Line2 RotateAbout(Point2 fulcrum, float angle)
        {
            float u = fulcrum.U;
            float v = fulcrum.V;
            float w = fulcrum.W;
            float cθ = (float)Math.Cos(angle);
            float sθ = (float)Math.Sin(angle);

            return new Line2(
                w*(a*cθ-b*sθ),
                w*(b*cθ+a*sθ),
                (b*u-a*v)*sθ-(a*u+b*v)*cθ+a*u+b*v+c*w);
        }

        public Line2 MirrorAbout(Line2 line)
        {
            //float a1=a;
            //float b1=b;
            //float c1=c;
            float oa = line.a;
            float ob = line.b;
            float oc = line.c;

            return new Line2(
                a*(oa*oa-ob*ob)+2*oa*ob*b,
                2*oa*ob*a+b*(ob*ob-oa*oa),
                2*oa*oc*a+2*ob*oc*b-c*(oa*oa+ob*ob));
        }

        #endregion

        #region Operators
        public Line2 Scale(float factor)
        {
            return new Line2(factor*a, factor*b, factor*c);
        }
        public Line2 Flip()
        {
            return new Line2(-a, -b, c);
        }

        public Line2 Add(Line2 other)
        {
            return new Line2(a+other.a, b+other.b, c+other.c);
        }
        public Line2 Offset(Line2 other)
        {
            return new Line2(a*other.c+other.a*c, b*other.c+other.b*c, c*other.c);
        }
        public Line2 Relative(Line2 other)
        {
            return new Line2(a*other.c-other.a*c, b*other.c-other.b*c, c*other.c);
        }

        public static float operator *(Line2 line, Point2 point)
        {
            return line.Dot(point);
        }
        public static Line2 operator *(float factor, Line2 line)
        {
            return line.Scale(factor);
        }
        public static Line2 operator +(Line2 l, Line2 m)
        {
            return l.Add(m);
        }
        public static Line2 operator -(Line2 l, Line2 m)
        {
            return l.Add(m.Scale(-1));
        }
        #endregion

        #region IArray Members
        public Line2 Normalize()
        {
            return FromPointAndDirection(Position, Direction);
        }
        public float this[int index]
        {
            get
            {
                switch (index)
                {
                    case 0: return a;
                    case 1: return b;
                    case 2: return c;
                }
                throw new IndexOutOfRangeException();
            }
        }
        public float this[int row, int column]
        {
            get
            {
                if (column==0)
                {
                    return this[row];
                }
                throw new IndexOutOfRangeException();
            }
        }
        public float[] ToArray() { return new float[] { a, b, c }; }
        public float[][] ToJaggedArray()
        {
            return new float[][] {
                new float[] { a },
                new float[] { b },
                new float[] { c } };
        }


        #endregion

        #region IFormattable Members

        public override string ToString()
        {
            return string.Format("Line2({0},{1},{2})",
                a, b, c);
        }

        #endregion

        #region ICloneable Members

        public Line2 Clone() { return new Line2(this); }

        object ICloneable.Clone()
        {
            return Clone();
        }

        #endregion

        #region IEquatable Members

        /// <summary>
        /// Equality overrides from <see cref="System.Object"/>
        /// </summary>
        /// <param name="obj">The object to compare this with</param>
        /// <returns>False if object is a different type, otherwise it calls <code>Equals(Line2)</code></returns>
        public override bool Equals(object obj)
        {
            if (obj is Line2)
            {
                return Equals((Line2)obj);
            }
            return false;
        }

        /// <summary>
        /// Checks for equality among <see cref="Line2"/> classes
        /// </summary>
        /// <param name="other">The other <see cref="Line2"/> to compare it to</param>
        /// <returns>True if equal</returns>
        public bool Equals(Line2 other)
        {
            return a == other.a
                && b == other.b
                && c == other.c;
        }

        /// <summary>
        /// Calculates the hash code for the <see cref="Line2"/>
        /// </summary>
        /// <returns>The int hash value</returns>
        public override int GetHashCode()
        {
            return ((17*23+a.GetHashCode())*23+b.GetHashCode())*23+c.GetHashCode();
        }
        public static bool operator ==(Line2 target, Line2 other) { return target.Equals(other); }
        public static bool operator !=(Line2 target, Line2 other) { return !target.Equals(other); }

        #endregion

    }

    public struct LineSegment2
    {
        readonly Line2 line;
        float t1, t2;

        public LineSegment2(Vector2 r1, Vector2 r2)
        {
            this.line=Line2.FromTwoPoints(r1, r2);
            this.t1=line.ParallelDistanceTo(Point2.FromLocation(r1));
            this.t2=line.ParallelDistanceTo(Point2.FromLocation(r2));
        }

        public Line2 Line { get { return line; } }
        public Point2 Start { get { return line.GetPointAlong(t1); } }
        public Point2 End { get { return line.GetPointAlong(t2); } }
    }

}
