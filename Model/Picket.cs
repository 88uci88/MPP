using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MPP.Model
{
    public class Picket : PropChange
    {
        int id;
        double x, y;
        Profile profile;
        ObservableCollection<PicketValue>? picketValues;
        Operator? _operator;

        public bool IsOnProfile()
        {
            foreach (var point in profile.Points)
            {
                if (X == point.X && Y == point.Y) return true;
            }
            foreach (var lineSegment in GetLineSegments())
            {
                if (ArePointsOnLineSegment(X, Y, lineSegment.Item1.X, lineSegment.Item1.Y, lineSegment.Item2.X, lineSegment.Item2.Y)) return true;
            }
            return false;
        }

        private ObservableCollection<Tuple<Point, Point>> GetLineSegments()
        {
            var lineSegments = new ObservableCollection<Tuple<Point, Point>>();

            for (int i = 0; i < profile.Points.Count - 1; i++)
            {
                lineSegments.Add(new Tuple<Point, Point>(profile.Points[i].AsPoint, profile.Points[i + 1].AsPoint));
            }

            return lineSegments;
        }

        private bool ArePointsOnLineSegment(double px, double py, double x1, double y1, double x2, double y2)
        {
            double d1 = Distance(px, py, x1, y1);
            double d2 = Distance(px, py, x2, y2);
            double lineLength = Distance(x1, y1, x2, y2);

            return Math.Abs(d1 + d2 - lineLength) < 0.001;
        }

        private double Distance(double x1, double y1, double x2, double y2)
        {
            double dx = x2 - x1;
            double dy = y2 - y1;
            return Math.Sqrt(dx * dx + dy * dy);
        }

        public int Id
        {
            get { return id; }
            set
            {
                id = value;
                OnPropertyChanged(nameof(Id));
            }
        }

        public Profile Profile
        {
            get { return profile; }
            set
            {
                profile = value;
                OnPropertyChanged(nameof(Profile));
            }
        }

        public double X
        {
            get { return x; }
            set
            {
                x = value;
                OnPropertyChanged(nameof(X));
            }
        }

        public double Y
        {
            get { return y; }
            set
            {
                y = value;
                OnPropertyChanged(nameof(Y));
            }
        }

        public ObservableCollection<PicketValue>? PicketValues
        {
            get { return picketValues; }
            set
            {
                picketValues = value;
                OnPropertyChanged(nameof(PicketValues));
            }
        }

        public Operator? Operator
        {
            get { return _operator; }
            set
            {
                _operator = value;
                OnPropertyChanged(nameof(Operator));
            }
        }

        public Point AsPoint => new Point(x, y);
    }
}