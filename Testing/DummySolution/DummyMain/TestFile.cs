using Dummy;
namespace Parties
{
    class MathsParty
    {
        /// <summary>
        /// Recreate signal given Fourier components
        /// <image url="$(SolutionDir)\CommonImages\fourier.png" />
        /// </summary>
        public Signal ReconstituteSignal(double a0, double[,] a, double[,] b, double[] x, double L)
        {
            Signal s = null;
            // ...
            return s;
        }

        /// <summary>
        /// Gets point of intersection of line and plane. 3 cases to consider.
        /// <image url="http://bit.ly/108beTg" scale="0.7" />
        /// </summary>
        public Point LinePlaneIntersecter(Plane plane, Line line)
        {
            Point p = null;
            // ...
            return p;
        }
    }
}
namespace Dummy
{
    class Plane
    {
    }

    class Line
    {
    }

    class Point
    {
    }

    class Signal
    {
    }
}
