using System;
using System.Numerics;

namespace JA
{
    public readonly struct Point2 :
        ICloneable,
        IEquatable<Point2>
    {
        #region Factory
        public Point2(float u, float v, float w = 1)
        {
            this.Vector=new Vector2(u, v);
            this.W=w;
        }
        public Point2(Vector2 vector, float scalar = 1)
        {
            this.Vector=vector;
            this.W=scalar;
        }
        public Point2(Point2 other) { this=other; }
        public static Point2 FromLocation(float x, float y)
        {
            return new Point2(x, y, 1);
        }
        public static Point2 FromLocation(Vector2 position)
        {
            return new Point2(position.X, position.Y, 1);
        }
        public static Point2 Meet(Line2 l, Line2 m)
        {
            return new Point2(
                u: l.B*m.C-m.B*l.C,
                v: m.A*l.C-l.A*m.C,
                w: l.A*m.B-m.A*l.B);
        }
        public static Point2 Polar(float radius, float angle)
        {
            return new Point2(radius*(float)Math.Cos(angle), radius*(float)Math.Sin(angle), 1);
        }
        public Vector2 ToVector() => new Vector2(Vector.X/W, Vector.Y/W);
        public static readonly Point2 O = FromLocation(0, 0);
        #endregion

        #region Properties
        public int Count { get { return 3; } }
        public float U { get { return Vector.X; } }
        public float V { get { return Vector.Y; } }
        public float W { get; }
        public float Magnitude { get { return Math.Abs(W); } }
        public Vector2 Vector { get; }
        public float Scalar { get { return W; } }
        public float LX { get { return Vector.X/W; } }
        public float LY { get { return Vector.Y/W; } }
        public Vector2 LocationVector { get { return Vector/W; } }
        public Vector2 DirectionVector { get { return Vector2.Normalize(Vector); } }
        public float DistanceFromOrigin { get { return Vector.Length()/W; } }
        public bool IsReadOnly { get { return true; } }
        #endregion

        #region Operators

        public Point2 Scale(float factor)
        {
            return new Point2(factor*Vector, factor*W);
        }
        public Point2 Flip()
        {
            return new Point2(-Vector, W);
        }
        public Point2 Add(Point2 other)
        {
            return new Point2(Vector+other.Vector, W+other.W);
        }
        public Point2 StepBy(Vector2 step)
        {
            return new Point2(Vector+W*step, W);
        }
        public Vector2 StepFrom(Point2 other)
        {
            return LocationVector-other.LocationVector;
        }
        public Point2 Offset(Point2 other)
        {
            return new Point2(Vector*other.W+other.Vector*W, W*other.W);
        }
        public Point2 Relative(Point2 other)
        {
            return new Point2(Vector*other.W-other.Vector*W, W*other.W);
        }

        public static float operator *(Point2 point, Line2 line)
        {
            return point.Dot(line);
        }

        public static Point2 operator *(float factor, Point2 point)
        {
            return point.Scale(factor);
        }
        public static Point2 operator +(Point2 p, Point2 q)
        {
            return p.Add(q);
        }
        public static Point2 operator -(Point2 p, Point2 q)
        {
            return p.Add(q.Scale(-1));
        }
        public static Point2 operator +(Point2 p, Vector2 step)
        {
            return new Point2(p.Vector+p.Scalar*step, p.Scalar);
        }
        #endregion

        #region Methods

        public float SumSquares() { return W*W; }

        public float Dot(Line2 L)
        {
            return Vector.X*L.A+Vector.Y*L.B+W*L.C;
        }

        public Point2 RotateAbout(Point2 fulcrum, float angle)
        {
            float fu = fulcrum.Vector.Y;
            float fv = fulcrum.Vector.X;
            float fw = fulcrum.W;
            float cθ = (float)Math.Cos(angle);
            float sθ = (float)Math.Sin(angle);

            float dx = (Vector.X*fw-fu*W);
            float dy = (Vector.Y*fw-fv*W);

            return new Point2(
                fu*W+dx*cθ-dy*sθ,
                fv*W+dx*sθ+dy*cθ,
                W*fw);
        }

        public float DistanceTo(Point2 other)
        {
            float u = this.Vector.X;
            float v = this.Vector.Y;
            float ou = other.Vector.X;
            float ov = other.Vector.Y;
            float ow = other.W;

            return new Vector2(ow*u-ou*W, ow*v-ov*W).Length()/(ow*W);
                //Math.Sqrt((ow*u-ou*δ).Sqr()+(ow*v-ov*δ).Sqr())/(ow*δ);
        }

        public float DistanceTo(Line2 line)
        {
            float u = this.Vector.X;
            float v = this.Vector.Y;
            float a = line.A;
            float b = line.B;
            float c = line.C;
            float ab2 = a*a+b*b;
            return (a*u+b*v+c*W)/(W*ab2);
        }

        public Vector2 DirectionTo(Point2 P)
        {
            float u = this.Vector.X;
            float v = this.Vector.Y;            
            return Vector2.Normalize(new Vector2(P.U*W-P.W*u, P.V*W-P.W*v));
        }
        public Vector2 DirectionTo(Line2 L)
        {
            float s = -Math.Sign(Dot(L));
            return Vector2.Normalize(new Vector2(s*L.A, s*L.B));
        }

        public Point2 MirrorAbout(Line2 line)
        {
            float u = this.Vector.X;
            float v = this.Vector.Y;
            float a = line.A;
            float b = line.B;
            float c = line.C;

            return new Point2(
                u*(b*b-a*a)-2*a*(b*v+c*W),
                v*(a*a-b*b)-2*b*(a*u+c*W),
                W*(a*a+b*b));
        }
        #endregion

        #region IArray Methods
        public float this[int index]
        {
            get
            {
                switch(index)
                {
                    case 0: return Vector.X;
                    case 1: return Vector.Y;
                    case 2: return W;
                }
                throw new IndexOutOfRangeException();
            }
        }
        public float this[int row, int column]
        {
            get
            {
                if(column==0)
                {
                    return this[row];
                }
                throw new IndexOutOfRangeException();
            }
        }
        public float[] ToArray() { return new float[] { Vector.X, Vector.Y, W }; }


        #endregion

        #region IFormattable Members
        public override string ToString()
        {
            return string.Format("Point2({0},{1},{2})",
                Vector.X,
                Vector.Y,
                W);
        }

        #endregion

        #region ICloneable Members

        public Point2 Clone() { return new Point2(this); }

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
        /// <returns>False if object is a different type, otherwise it calls <code>Equals(Point2)</code></returns>
        public override bool Equals(object obj)
        {
#pragma warning disable S3247 // Duplicate casts should not be made
            if(obj is Point2)
#pragma warning restore S3247 // Duplicate casts should not be made
            {
                return Equals((Point2)obj);
            }
            return false;
        }

        /// <summary>
        /// Checks for equality among <see cref="Point2"/> classes
        /// </summary>
        /// <param name="other">The other <see cref="Point2"/> to compare it to</param>
        /// <returns>True if equal</returns>
        public bool Equals(Point2 other)
        {
            return Vector.Equals(other.Vector)
                && W == other.W;
        }

        /// <summary>
        /// Calculates the hash code for the <see cref="Point2"/>
        /// </summary>
        /// <returns>The int hash value</returns>
        public override int GetHashCode()
        {
            return (17*23+Vector.GetHashCode())*23+W.GetHashCode();
        }
        public static bool operator ==(Point2 target, Point2 other) { return target.Equals(other); }
        public static bool operator !=(Point2 target, Point2 other) { return !target.Equals(other); }

        #endregion

        #region IAlgebra<Point2> Members


        public bool IsZero
        {
            get { return Vector == Vector2.Zero && W==0; }
        }


        public float Norm2()
        {
            return W;
        }

        #endregion
    }
}
