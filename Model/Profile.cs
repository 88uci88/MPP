using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Media;

namespace MPP.Model
{
    public class Profile : PropChange
    {
        int id;
        Area area;
        ObservableCollection<Picket> pickets;
        ObservableCollection<ProfilePoint> points;

        public void Draw(DrawModel dm, Brush br)
        {
            if (points is null) return;
            dm.DrawPoly(points.Select(p => p.AsPoint).ToArray(), br, 0.08, false);
            foreach (var p in points)
            {
                dm.DrawText($"{p.X};{p.Y}", p.X, p.Y, br, 0.5);
                dm.DrawCircle(p.X, p.Y, 0.1, br);
            }
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

        public Area Area
        {
            get { return area; }
            set
            {
                area = value;
                OnPropertyChanged(nameof(Area));
            }
        }

        public ObservableCollection<Picket> Pickets
        {
            get { return pickets; }
            set
            {
                pickets = value;
                OnPropertyChanged(nameof(Pickets));
            }
        }

        public ObservableCollection<ProfilePoint> Points
        {
            get { return points; }
            set
            {
                points = value;
                OnPropertyChanged(nameof(Points));
            }
        }

        public bool IsCorrect()
        {
            bool IsPointInsideArea(ProfilePoint point)
            {
                bool isInside = false;
                for (int i = 0, j = Area.Points.Count - 1; i < Area.Points.Count; j = i++)
                {
                    if ((Area.Points[i].Y > point.Y) != (Area.Points[j].Y > point.Y) &&
                        (point.X < (Area.Points[j].X - Area.Points[i].X) * (point.Y - Area.Points[i].Y) / (Area.Points[j].Y - Area.Points[i].Y) + Area.Points[i].X))
                    {
                        isInside = !isInside;
                    }
                }
                return isInside;
            }

            for (int i = 0; i < points?.Count - 1; i++)
                for (int j = 0; j < Area.Points.Count; j++)
                    if (AreCrossing(points[i].AsPoint, points[i + 1].AsPoint, Area.Points[j].AsPoint, Area.Points[(j + 1) % Area.Points.Count].AsPoint))
                        return false;
            foreach (var pr in Area?.Profiles ?? new())
                for (int i = 0; i < pr.Points?.Count - 1; i++)
                    for (int j = 0; j < points?.Count - 1; j++)
                        if (AreCrossing(pr.Points[i].AsPoint, pr.Points[i + 1].AsPoint, points[j].AsPoint, points[j + 1].AsPoint, colideSegments: pr == this ? Math.Abs(i - j) > 1 : true))
                            return false;

            if (points != null)
            {
                foreach (var point in points)
                {
                    if (!IsPointInsideArea(point))
                        return false;
                }
                if (points.Any(p => points.Count(p2 => p2.X == p.X && p2.Y == p.Y) > 1))
                    return false;
            }

            return true;
        }

        public bool AreCrossing(Point a1, Point a2, Point b1, Point b2, bool colideSegments = true)
        {
            double CrossProduct(Point p1, Point p2, Point p3)
            {
                return (p2.X - p1.X) * (p3.Y - p1.Y) - (p2.Y - p1.Y) * (p3.X - p1.X);
            }

            bool IsPointOnSegment(Point p, Point q, Point r)
            {
                if (!colideSegments && (p == q || p == r || q == r)) return false;
                return q.X <= Math.Max(p.X, r.X) && q.X >= Math.Min(p.X, r.X) &&
                       q.Y <= Math.Max(p.Y, r.Y) && q.Y >= Math.Min(p.Y, r.Y);
            }

            double cp1 = CrossProduct(a1, a2, b1);
            double cp2 = CrossProduct(a1, a2, b2);
            double cp3 = CrossProduct(b1, b2, a1);
            double cp4 = CrossProduct(b1, b2, a2);

            if ((cp1 > 0 && cp2 < 0 || cp1 < 0 && cp2 > 0) && (cp3 > 0 && cp4 < 0 || cp3 < 0 && cp4 > 0))
                return true;

            if (cp1 == 0 && IsPointOnSegment(a1, b1, a2))
                return true;
            if (cp2 == 0 && IsPointOnSegment(a1, b2, a2))
                return true;
            if (cp3 == 0 && IsPointOnSegment(b1, a1, b2))
                return true;
            if (cp4 == 0 && IsPointOnSegment(b1, a2, b2))
                return true;
            return false;
        }
    }
}
