using MPP.Model;
using Microsoft.EntityFrameworkCore;
using Brushes = System.Windows.Media.Brushes;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows;
using System.Windows.Media;
using MPP.View;
using System.Drawing;

namespace MPP.ViewModel
{
    public class ProfileViewModel : PropChange
    {
        ApplicationContext db = ApplicationContext.getInstance();
        DrawingImage image;
        Random rnd = new Random();

        Picket selectedPicket;
        ProfilePoint selectedPoint;

        public Profile Profile { get; set; }
        public Picket Picket { get; set; }

        public ProfileViewModel() : this(new Profile()) { }
        public ProfileViewModel(Profile profile)
        {
            Profile = profile;
            AddPointCommand = new(AddPoint);
            DeletePointCommand = new(DeletePoint, (obj) => SelectedPoint != null);
            AddPicketCommand = new(AddPicket);
            DeletePicketCommand = new(DeletePicket, (obj) => SelectedPicket != null);
            SavePicketCommand = new(SavePicket);
            SavePointCommand = new(SavePoint);
            OpenPicketCommand = new(OpenPicket);
            AddRandomPointCommand = new(AddRandomPoint);
            AddRandomPicketCommand = new(AddRandomPicket);
            Redraw();
        }
        public RelayCommand AddPointCommand { get; set; }
        public RelayCommand DeletePointCommand { get; set; }
        public RelayCommand AddPicketCommand { get; set; }
        public RelayCommand OpenPicketCommand { get; set; }
        public RelayCommand DeletePicketCommand { get; set; }
        public RelayCommand SavePicketCommand { get; set; }
        public RelayCommand SavePointCommand { get; set; }
        public RelayCommand AddRandomPointCommand { get; set; }
        public RelayCommand AddRandomPicketCommand { get; set; }

        void AddRandomPicket(object obj)
        {
            Picket newpicket = new Picket();
            if (Profile.Points != null)
            {
                while (true)
                {
                    double minX = Profile.Points.Min(point => point.X);
                    double maxX = Profile.Points.Max(point => point.X);
                    double minY = Profile.Points.Min(point => point.Y);
                    double maxY = Profile.Points.Max(point => point.Y);

                    double randomX = minX + (maxX - minX) * rnd.NextDouble();
                    double randomY = minY + (maxY - minY) * rnd.NextDouble();

                    newpicket.X = Math.Round(randomX, 1);
                    newpicket.Y = Math.Round(randomY, 1);

                    newpicket.Profile = Profile;

                    db.Pickets.Add(newpicket);

                    if (newpicket.IsOnProfile() && !db.Pickets.Any(p => p.X == newpicket.X && p.Y == newpicket.Y)) break;
                    else db.Pickets.Remove(newpicket);
                }
            }
            else
            {
                MessageBox.Show("Отсутсвует профиль!");
                return;
            }
            db.SaveChanges();
            SelectedPicket = newpicket;
            OnPropertyChanged(nameof(Profile));
            OnPropertyChanged(nameof(Picket));
            Redraw();
        }


        void AddRandomPoint(object obj)
        {
            ProfilePoint pp, p = new();
            if (Profile.Points != null && Profile.Points.Count > 0) pp = Profile.Points.Last();
            else
            {
                pp = new ProfilePoint();
                pp.X = Profile.Area.Points[0].X;
                pp.Y = Profile.Area.Points[0].Y;
            }
            int off = 15;
            for (int i = 0; i < 1000; i++)
            {
                p = new ProfilePoint();
                p.X = pp.X + rnd.Next(-off, off);
                p.Y = pp.Y + rnd.Next(-off, off);
                p.Profile = Profile;
                db.ProfilePoints.Add(p);
                if (Profile.IsCorrect()) break;
                else if (i == 999 && !Profile.IsCorrect())
                {
                    MessageBox.Show("Некуда ставить точку, исправьте сами либо попробуйте еще раз.");
                    db.ProfilePoints.Remove(p);
                    break;
                }    
                else db.ProfilePoints.Remove(p);
            }
            db.SaveChanges();
            SelectedPoint = p;
            OnPropertyChanged(nameof(Profile));
            Redraw();
        }


        void OpenPicket(object obj)
        {
            new PicketWindow()
            {
                DataContext = new PicketViewModel((Picket)obj)
            }.ShowDialog();
            OnPropertyChanged(nameof(obj));
            Redraw();
        }

        void AddPoint(object obj)
        {
            var point = new ProfilePoint() { X = 0, Y = 0, Profile = Profile };
            db.ProfilePoints.Add(point);
            db.SaveChanges();
            SelectedPoint = point;
            OnPropertyChanged(nameof(Profile));
        }
        void DeletePoint(object obj)
        {
            db.ProfilePoints.Remove(SelectedPoint);
            db.SaveChanges();
        }
        void AddPicket(object obj)
        {
            var picket = new Picket() { Profile = Profile };
            db.Pickets.Add(picket);
            db.SaveChanges();
            SelectedPicket = picket;
            OnPropertyChanged(nameof(Profile));
        }
        void DeletePicket(object obj)
        {
            db.Pickets.Remove(SelectedPicket);
            db.SaveChanges();
            OnPropertyChanged(nameof(Profile));
        }
        void SavePicket(object obj)
        {
            if (obj is Picket)
            {
                db.Entry((Picket)obj).State = EntityState.Modified;
                db.SaveChanges();
            }
        }
        void SavePoint(object obj)
        {
            if (obj is ProfilePoint)
            {
                db.Entry((ProfilePoint)obj).State = EntityState.Modified;
                db.SaveChanges();
            }
        }
        public Picket SelectedPicket
        {
            get => selectedPicket;
            set
            {
                selectedPicket = value;
                OnPropertyChanged(nameof(SelectedPicket));
                Redraw();
            }
        }
        public ProfilePoint SelectedPoint
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
            if (Profile.IsCorrect()) Profile.Draw(newimage, Brushes.Gray);
            else Profile.Draw(newimage, Brushes.Red);
            foreach (var p in Profile?.Points ?? new()) 
            {
                if (p == SelectedPoint) newimage.DrawCircle(p.X, p.Y, 0.15, Brushes.Green);
                else newimage.DrawCircle(p.X, p.Y, 0.15, Brushes.DarkGray);
            }
            foreach (var p in Profile?.Pickets ?? new())
            {
                if (p == SelectedPicket && p.IsOnProfile()) newimage.DrawFlag(p.X, p.Y, 0.17, Brushes.Red);
                else if (p == SelectedPicket && !p.IsOnProfile()) newimage.DrawFlag(p.X, p.Y, 0.17, Brushes.Black);
                else if (p != SelectedPicket && p.IsOnProfile()) newimage.DrawFlag(p.X, p.Y, 0.17, Brushes.DarkRed);
                else if (p != SelectedPicket && !p.IsOnProfile()) newimage.DrawFlag(p.X, p.Y, 0.17, Brushes.DarkOliveGreen);
            }
            Image = newimage.Render();
        }
    }
}
