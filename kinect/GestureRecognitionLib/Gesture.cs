//#define USE_3D_GESTURES

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using log4net;
using Microsoft.Kinect;

namespace GestureRecognition
{
    [Serializable()]
    public class Gesture
    {
        [field: NonSerialized()]
        private static readonly ILog Logger = LogManager.GetLogger(typeof(Gesture));

        private const int DefMinLength = 10;
        private const int DefMaxSlope = 2;

        #region Member Variables
        /// <summary>
        /// The example time sequence for this gesture. This is filled with Dictionary<JointType, Point3D>> instances.
        /// </summary>
        private List<Dictionary<JointType, Point3D>> _time_sequence;

        /// <summary>
        /// This gestures unique ID
        /// </summary>
        private Guid _id;

        /// <summary>
        /// This gestures label - possibly not unique?
        /// </summary>
        private string _label;

        /// <summary>
        /// The minimum length for which this gesture can be recognized
        /// </summary>
        private readonly int _minLength;

        /// <summary>
        /// The maximum number of vertical or horizontal steps in a row
        /// </summary>
        private readonly int _maxSlope;
        #endregion

        #region properties
        public string Label { get { return _label; } }

        public Guid ID { get { return _id; } }

        public List<Dictionary<JointType, Point3D>> Sequence { get { return _time_sequence; } }

        #endregion

        #region Constructors
        
        public Gesture(List<Dictionary<JointType, Point3D>> sequence, string label) : this(sequence, DefMinLength, DefMaxSlope, Guid.NewGuid(), label) { }

        public Gesture(List<Dictionary<JointType, Point3D>> sequence, int minLength, string label)
            : this(sequence, minLength, DefMaxSlope, Guid.NewGuid(), label) { }

        public Gesture(List<Dictionary<JointType, Point3D>> sequence, int minLength, int maxSlope, Guid id, string label)
        {
            this._time_sequence = sequence;
            this._minLength = minLength;
            this._id = id;
            this._label = label;
            this._maxSlope = maxSlope;
        }
        #endregion

        /// <summary>
        /// Computes the distance between the end of the comparision sequence 
        /// and the end of the example sequence for this gesture.
        /// </summary>
        /// <param name="toCompare">The sequence to compare against.</param>
        /// <returns>The distance between the two sequences</returns>
        public double EuclideanDistanceForEndingSamples(List<Dictionary<JointType, Point3D>> toCompare)
        {
            return _EuclideanDistance(toCompare[toCompare.Count - 1], _time_sequence[_time_sequence.Count - 1]);
        }

        public double DynamicTimeWarpDistance(List<Dictionary<JointType, Point3D>> toCompare)
        {
            return _Dtw(toCompare, _time_sequence) / _time_sequence.Count;
        }

        #region Helper Functions
        /// <summary>
        /// Poorly written function which might easily break if things are 
        /// changed - however this is \less\ important because it isn't public API.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        private static double _EuclideanDistance(Dictionary<JointType, Point3D> a, Dictionary<JointType, Point3D> b)
        {
            double result = 0;
            foreach (JointType jointType in b.Keys)
            {
                Point3D item;
                if (a.TryGetValue(jointType, out item))
                {
                    result += item.DistanceSquared(b[jointType]);
                }
                else
                {
                    Logger.Warn("Observation dictionary a does not have the same joint types as b");
                }
            }
            return Math.Sqrt(result);
        }

        private double _Dtw(List<Dictionary<JointType, Point3D>> seq1, List<Dictionary<JointType, Point3D>> seq2)
        {
            // Init
            var seq1R = new List<Dictionary<JointType, Point3D>>(seq1);
            seq1R.Reverse();
            var seq2R = new List<Dictionary<JointType, Point3D>>(seq2);
            seq2R.Reverse();
            var tab = new double[seq1R.Count + 1, seq2R.Count + 1];
            var slopeI = new int[seq1R.Count + 1, seq2R.Count + 1];
            var slopeJ = new int[seq1R.Count + 1, seq2R.Count + 1];

            for (int i = 0; i < seq1R.Count + 1; i++)
            {
                for (int j = 0; j < seq2R.Count + 1; j++)
                {
                    tab[i, j] = double.PositiveInfinity;
                    slopeI[i, j] = 0;
                    slopeJ[i, j] = 0;
                }
            }

            tab[0, 0] = 0;

            // Dynamic computation of the DTW matrix.
            for (int i = 1; i < seq1R.Count + 1; i++)
            {
                for (int j = 1; j < seq2R.Count + 1; j++)
                {
                    if (tab[i, j - 1] < tab[i - 1, j - 1] && tab[i, j - 1] < tab[i - 1, j] &&
                        slopeI[i, j - 1] < _maxSlope)
                    {
                        tab[i, j] = _EuclideanDistance(seq1R[i - 1], seq2R[j - 1]) + tab[i, j - 1];
                        slopeI[i, j] = slopeJ[i, j - 1] + 1;
                        slopeJ[i, j] = 0;
                    }
                    else if (tab[i - 1, j] < tab[i - 1, j - 1] && tab[i - 1, j] < tab[i, j - 1] &&
                             slopeJ[i - 1, j] < _maxSlope)
                    {
                        tab[i, j] = _EuclideanDistance(seq1R[i - 1], seq2R[j - 1]) + tab[i - 1, j];
                        slopeI[i, j] = 0;
                        slopeJ[i, j] = slopeJ[i - 1, j] + 1;
                    }
                    else
                    {
                        tab[i, j] = _EuclideanDistance(seq1R[i - 1], seq2R[j - 1]) + tab[i - 1, j - 1];
                        slopeI[i, j] = 0;
                        slopeJ[i, j] = 0;
                    }
                }
            }

            // Find best between seq2 and an ending (postfix) of seq1.
            double bestMatch = double.PositiveInfinity;
            for (int i = 1; i < (seq1R.Count + 1) - _minLength; i++)
            {
                if (tab[i, seq2R.Count] < bestMatch)
                {
                    bestMatch = tab[i, seq2R.Count];
                }
            }

            return bestMatch;
        }

        #endregion

    }

    [Serializable()]
    public class Point3D
    {
        private double _x;
        private double _y;
        private double _z;

        public double X { get { return _x; } set { _x = value; } }
        public double Y { get { return _y; } set { _y = value; } }
        public double Z { get { return _z; } set { _z = value; } }

        public Point3D() : this(0, 0, 0) { }
        public Point3D(double x, double y, double z)
        {
            _x = x;
            _y = y;
#if USE_3D_GESTURES
            _z = z;
#else
            _z = 0;
#endif
        }

        public static Point3D operator -(Point3D point1, Point3D point2)
        {
            return Subtract(point1, point2);
        }
        public static Point3D operator +(Point3D point1, Point3D point2)
        {
            return Add(point1, point2);
        }
        public static Point3D operator /(Point3D point1, double scalar)
        {
            return Divide(point1, scalar);
        }
        public static implicit operator Point3D(SkeletonPoint point)
        {
            return new Point3D(point.X, point.Y, point.Z);
        }

        public static Point3D Subtract(Point3D a, Point3D b)
        {
            return new Point3D(a._x - b._x, a._y - b._y, a._z - b._z);
        }
        public static Point3D Add(Point3D a, Point3D b)
        {
            return new Point3D(a._x + b._x, a._y + b._y, a._z + b._z);
        }
        public static Point3D Divide(Point3D a, double b)
        {
            return new Point3D(a._x / b, a._y / b, a._z / b);
        }

        public override string ToString()
        {
            return "(" + _x + ", " + _y + ", " + _z + ")";
        }

        public double DistanceSquared(Point3D other)
        {
            double x_dist = this.X - other.X;
            double y_dist = this.Y - other.Y;
            double z_dist = this.Z - other.Z;
            return x_dist * x_dist + y_dist * y_dist + z_dist * z_dist;
        }
    }
}
