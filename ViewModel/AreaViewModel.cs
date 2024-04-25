using MPP.Model;
using MPP.View;
using Microsoft.EntityFrameworkCore;
using System.Windows;
using System.Windows.Media;

namespace MPP.ViewModel
{
    public class AreaViewModel : PropChange
    {
        ApplicationContext db = ApplicationContext.getInstance();
        DrawingImage image;
        Random rnd = new Random();

        Profile selectedProfile;
        AreaPoint selectedPoint;
        public Area Area { get; set; }
        public AreaViewModel() : this(new Area()) { }
        public AreaViewModel(Area area)
        {
            Area = area;
            AddPointCommand = new(AddPoint);
            DeletePointCommand = new(DeletePoint, (obj) => SelectedPoint != null);
            AddProfileCommand = new(AddProfile);
            DeleteProfileCommand = new(DeleteProfile, (obj) => SelectedProfile != null);
            OpenProfileCommand = new(OpenProfile);
            SavePointCommand = new(SavePoint);
            AddRandomPointCommand = new(AddRandomPoint);
            Redraw();
        }

        public RelayCommand AddPointCommand { get; set; }
        public RelayCommand DeletePointCommand { get; set; }
        public RelayCommand AddProfileCommand { get; set; }
        public RelayCommand DeleteProfileCommand { get; set; }
        public RelayCommand OpenProfileCommand { get; set; }
        public RelayCommand SavePointCommand { get; set; }
        public RelayCommand AddRandomPointCommand { get; set; }

        public string? AreaName
        {
            get => Area.Name;
            set
            {
                Area.Name = value;
                db.Entry(Area).State = EntityState.Modified;
                db.SaveChanges();
            }
        }

        void AddRandomPoint(object obj)
        {
            AreaPoint ap, p = new();
            if (Area.Points != null && Area.Points.Count > 0) ap = Area.Points.Last();
            else
            {
                ap = new AreaPoint();
                ap.X = rnd.Next(-70,70);
                ap.Y = rnd.Next(-70, 70);
            }
            int off = 20;
            for (int i = 0; i < 1000; i++)
            {
                p = new AreaPoint();
                p.X = ap.X + rnd.Next(-off, off);
                p.Y = ap.Y + rnd.Next(-off, off);
                p.Area = Area;
                db.AreaPoints.Add(p);
                if (Area.IsCorrect()) break;
                else if (i == 999 && !Area.IsCorrect())
                {
                    MessageBox.Show("Некуда ставить точку, исправьте сами либо попробуйте еще раз.");
                    db.AreaPoints.Remove(p);
                    p = ap;
                    break;
                }
                else db.AreaPoints.Remove(p);
            }
            db.SaveChanges();
            SelectedPoint = p;
            OnPropertyChanged(nameof(Area));
            OnPropertyChanged(nameof(Area.Points));
            Redraw();
        }

        /*void AddRandomPoint(object obj)
        {
            if (Area?.Points == null)
            {
                while (true)
                {
                    int deltaX, deltaY, r = rnd.Next(0, 100);
                    do
                    {
                        deltaX = rnd.Next(-r, r);
                        deltaY = rnd.Next(-r, r);
                    } while (deltaX == 0 && deltaY == 0);
                    Area.Points = new() { new() { X = deltaX, Y = deltaY, Area = Area } };
                    if (Area.IsCorrect())
                    {
                        db.SaveChanges();
                        SelectedPoint = Area.Points[0];
                        OnPropertyChanged(nameof(Area));
                        return;
                    }
                    else Area.Points.Remove(new() { X = deltaX, Y = deltaY, Area = Area });
                }
            }
            while (true)
            {
                var lastPoint = Area.Points.OrderBy(p => p.Id).Last();
                int deltaX, deltaY, r = rnd.Next(1, 15);
                do
                {
                    deltaX = rnd.Next(-r, r);
                    deltaY = rnd.Next(-r, r);
                } while (deltaX == 0 && deltaY == 0);
                var newpoint = new AreaPoint() { X = Area.Points.Last().X + deltaX, Y = Area.Points.Last().Y + deltaY, Area = Area };
                Area.Points.Add(newpoint);
                if (Area.IsCorrect())
                {
                    db.SaveChanges();
                    SelectedPoint = newpoint;
                    OnPropertyChanged(nameof(Area));
                    break;
                }
                else Area.Points.Remove(newpoint);
            }
        }*/

        void AddPoint(object obj)
        {
            var point = new AreaPoint() { X = 0, Y = 0, Area = Area };
            db.AreaPoints.Add(point);
            db.SaveChanges();
            SelectedPoint = point;
            OnPropertyChanged(nameof(Area));
        }
        void DeletePoint(object obj)
        {
            db.AreaPoints.Remove(SelectedPoint);
            db.SaveChanges();
            OnPropertyChanged(nameof(Area));
            OnPropertyChanged(nameof(Area.Points));
        }
        void AddProfile(object obj)
        {
            var profile = new Profile() { Area = Area };
            db.Profiles.Add(profile);
            db.SaveChanges();
            SelectedProfile = profile;
            OnPropertyChanged(nameof(Area));
        }
        void DeleteProfile(object obj)
        {
            db.Profiles.Remove(SelectedProfile);
            db.SaveChanges();
            OnPropertyChanged(nameof(Area));
        }
        void OpenProfile(object obj)
        {
            new ProfileWindow()
            {
                DataContext = new ProfileViewModel((Profile)obj)
            }.ShowDialog();
            OnPropertyChanged(nameof(obj));
            Redraw();
        }
        void SavePoint(object obj)
        {
            if (obj is AreaPoint)
            {
                db.Entry((AreaPoint)obj).State = EntityState.Modified;
                db.SaveChanges();
            }
        }
        public Profile SelectedProfile
        {
            get => selectedProfile;
            set
            {
                selectedProfile = value;
                OnPropertyChanged(nameof(SelectedProfile));
                Redraw();
            }
        }
        public AreaPoint SelectedPoint
        {
            get => selectedPoint;
            set
            {
                selectedPoint = value;
                OnPropertyChanged(nameof(SelectedPoint));
                Redraw();
            }
        }
        public DrawingImage Image
        {
            get { return image; }
            set
            {
                image = value;
                OnPropertyChanged(nameof(Image));
            }
        }
        void Redraw()
        {
            var newimage = new DrawModel();
            if (Area.IsCorrect()) Area.Draw(newimage, Brushes.Blue);
            else Area.Draw(newimage, Brushes.Red);
            foreach (var point in Area?.Points ?? new())
            {
                if (SelectedPoint == point && Area.IsCorrect()) newimage.DrawCircle(point.X, point.Y, 0.4, Brushes.Yellow);
                else if (SelectedPoint == point && !Area.IsCorrect()) newimage.DrawCircle(point.X, point.Y, 0.4, Brushes.Red);
                else if (SelectedPoint != point && Area.IsCorrect()) newimage.DrawCircle(point.X, point.Y, 0.4, Brushes.Blue);
                else if (SelectedPoint != point && !Area.IsCorrect()) newimage.DrawCircle(point.X, point.Y, 0.4, Brushes.IndianRed);
            }
            foreach (var profile in Area?.Profiles ?? new())
            {
                if (SelectedProfile == profile && profile.IsCorrect()) profile.Draw(newimage, Brushes.Green);
                else if (SelectedProfile == profile && !profile.IsCorrect()) profile.Draw(newimage, Brushes.Red);
                else if (SelectedProfile != profile && profile.IsCorrect()) profile.Draw(newimage, Brushes.Gray);
                else if (SelectedProfile != profile && !profile.IsCorrect()) profile.Draw(newimage, Brushes.IndianRed);
            }
            Image = newimage.Render();
        }
    }
}
