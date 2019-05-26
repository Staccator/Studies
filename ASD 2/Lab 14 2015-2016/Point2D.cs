namespace Linq
{
    public class Point2D
    {
        public double X { get; set; }
        public double Y { get; set; }

        public Point2D(double x, double y)
        {
            X = x;
            Y = y;
        }

        public double Length()
        {
            return System.Math.Sqrt(X*X+Y*Y);
        }

        public override string ToString()
        {
            return System.String.Format("({0}, {1})", X, Y);
        }
    }
}
