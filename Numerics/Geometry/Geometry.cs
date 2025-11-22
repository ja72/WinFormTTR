using System;
using System.Numerics;

namespace JA.Numerics.Geometry
{
    public enum CornerSolution
    {
        Inside,
        Opposing,
        Outside1,
        Outside2,
    }


    public static class Geometry
    {
        public static (float r, float θ) ToPolar(this Vector2 vector)
        {
            var r = vector.Length();
            var θ = (float)Math.Atan2(vector.Y, vector.X);
            return (r, θ);
        }
        public static Vector2 Polar(float radius, float angle)
            => new Vector2((float)Math.Cos(angle)*radius, (float)Math.Sin(angle)*radius);
        public static Vector2 Elliptical(float majorRadius, float minorRadius, float angle)
            => new Vector2((float)Math.Cos(angle)*majorRadius, (float)Math.Sin(angle)*minorRadius);

        public static Point2 TanTanRadius(Point2 apex, Point2 side1, Point2 side2, float radius, CornerSolution corner = CornerSolution.Inside)
        {
            var edge1 = Line2.Join(apex, side1);
            var edge2 = Line2.Join(apex, side2);

            int s1 = 0, s2 = 0;

            switch (corner)
            {
                case CornerSolution.Inside:
                    s1 = 1;
                    s2 = -1;
                    break;
                case CornerSolution.Opposing:
                    s1 = -1;
                    s2 = 1;
                    break;
                case CornerSolution.Outside1:
                    s1 = 1;
                    s2 = 1;
                    break;
                case CornerSolution.Outside2:
                    s1 = -1;
                    s2 = -1;
                    break;
            }

            var offset1 = edge1.Offset(s1*radius);
            var offset2 = edge2.Offset(s2*radius);

            return Point2.Meet(offset1, offset2);
        }
    }

}
