using System.Globalization;
using System.Windows;
using System.Windows.Media;

namespace MPP.Model
{
    public class DrawModel
    {
        private readonly DrawingGroup drawingGroup = new DrawingGroup();

        public void DrawLine(double x1, double y1, double x2, double y2, string hexColor, double width)
        {
            SolidColorBrush brush = (SolidColorBrush)new BrushConverter().ConvertFromString(hexColor)!;
            DrawLine(x1, y1, x2, y2, brush, width);
        }

        public void DrawLine(double x1, double y1, double x2, double y2, Brush brush, double width)
        {
            var lineGeometry = new LineGeometry(new Point(x1, y1), new Point(x2, y2));
            var geometryDrawing = new GeometryDrawing(null, new Pen(brush, width) { DashCap = PenLineCap.Round }, lineGeometry);
            drawingGroup.Children.Add(geometryDrawing);
        }

        public void DrawText(string text, double x, double y, Brush brush, double size = 10)
        {
            var formattedText = new FormattedText(text, CultureInfo.CurrentCulture, FlowDirection.LeftToRight, new Typeface("Arial"), size, brush, 1);
            var textGeometry = formattedText.BuildGeometry(new Point(x, y));
            var geometryDrawing = new GeometryDrawing(brush, null, textGeometry);
            drawingGroup.Children.Add(geometryDrawing);
        }

        public void DrawCircle(double x, double y, double radius, Brush brush)
        {
            var ellipseGeometry = new EllipseGeometry(new Point(x, y), radius, radius);
            var geometryDrawing = new GeometryDrawing(brush, null, ellipseGeometry);
            drawingGroup.Children.Add(geometryDrawing);
        }

        public void DrawCircle(double x, double y, double radius, string hexColor)
        {
            SolidColorBrush brush = (SolidColorBrush)new BrushConverter().ConvertFromString(hexColor)!;
            DrawCircle(x, y, radius, brush);
        }

        public void DrawPoly(ICollection<Point> points, Brush brush, double width, bool isClosed)
        {
            if (points == null || !points.Any()) return;
            var figure = new PathFigure(points.First(), points.Skip(1).Select(p => new LineSegment(p, true)), isClosed);
            var pathGeometry = new PathGeometry(new PathFigure[] { figure });
            var geometryDrawing = new GeometryDrawing(null, new Pen(brush, width) { LineJoin = PenLineJoin.Round, DashCap = PenLineCap.Round }, pathGeometry);
            drawingGroup.Children.Add(geometryDrawing);
        }

        public void DrawFlag(double x, double y, double radius, Brush brush)
        {
            var flagDrawingGroup = new DrawingGroup();

            var stickStartPoint = new Point(x, y); 
            var stickEndPoint = new Point(x, y - radius * 2); 
            var stickGeometry = new LineGeometry(stickStartPoint, stickEndPoint);
            var stickDrawing = new GeometryDrawing(Brushes.Brown, new Pen(brush, 0.05), stickGeometry);
            flagDrawingGroup.Children.Add(stickDrawing);

            var circleGeometry = new EllipseGeometry(new Point(x, y), radius / 2, radius / 2);
            var circleDrawing = new GeometryDrawing(brush, null, circleGeometry);
            flagDrawingGroup.Children.Add(circleDrawing);

            var triangleGeometry = new PathGeometry();
            var figure = new PathFigure();
            figure.StartPoint = new Point(x - radius / 5, y - radius * 2 - radius / 5);
            figure.Segments.Add(new LineSegment(new Point(x + radius / 5, y - radius * 2), true)); 
            figure.Segments.Add(new LineSegment(new Point(x - radius / 5, y - radius * 2 + radius / 5), true)); 
            figure.IsClosed = true;
            triangleGeometry.Figures.Add(figure);
            var triangleDrawing = new GeometryDrawing(brush, new Pen(brush, 0.1), triangleGeometry);
            flagDrawingGroup.Children.Add(triangleDrawing);

            drawingGroup.Children.Add(flagDrawingGroup);
        }

        public DrawingImage Render(int offset = 3, int grid = 10, bool drawAxes = false)
        {
            double minX = (int)drawingGroup.Bounds.Left - offset;
            double minY = (int)drawingGroup.Bounds.Top - offset;
            double maxX = (int)drawingGroup.Bounds.Right - offset;
            double maxY = (int)drawingGroup.Bounds.Bottom + offset;

            var newChildren = drawingGroup.Children.ToArray();
            drawingGroup.Children.Clear();


            if ((minY < 0 && maxY > 0) || drawAxes) DrawLine(minX, 0, maxX, 0, "#ECECF0", 0.1);
            if ((minX < 0 && maxX > 0) || drawAxes) DrawLine(0, minY, 0, maxY, "#ECECF0", 0.1);

            for (double i = minX > 0 ? minX + grid - minX % grid : minX - minX % grid; i < maxX; i += grid)
            {
                DrawLine(i, minY, i, maxY, "#ECECF0", 0.08);
                for (double j = minY > 0 ? minY + grid - minY % grid : minY - minY % grid; j < maxY; j += grid)
                {
                    DrawLine(minX, j, maxX, j, "#ECECF0", 0.08);
                    DrawText($"{i}; {j}", i-0.7, j, Brushes.Gray, 0.5);
                    DrawCircle(i, j, 0.14, "#D6D7DA");
                }
            }            
            foreach (var child in newChildren) drawingGroup.Children.Add(child);
            return new DrawingImage(drawingGroup);
        }
    }
}
