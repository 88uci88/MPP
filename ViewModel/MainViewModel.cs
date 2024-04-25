using MPP.Model;
using MPP.View;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Media;

namespace MPP.ViewModel
{
    public class MainViewModel : PropChange
    {
        ApplicationContext db = ApplicationContext.getInstance();
        DrawingImage image;

        Customer selectedCustomer;
        Project selectedProject;
        Area selectedArea;

        public ObservableCollection<Customer> Customers { get => db.Customers.Local.ToObservableCollection(); }

        public MainViewModel()
        {
            AddCustomerCommand = new(AddCustomer);
            AddProjectCommand = new(AddProject, (obj) => SelectedCustomer != null);
            DeleteProjectCommand = new(DeleteProject, (obj)=> SelectedProject != null);
            DeleteCustomerCommand = new(DeleteCustomer, (obj) => SelectedCustomer != null);
            AddAreaCommand = new(AddArea, (o) => SelectedProject != null);
            DeleteAreaCommand = new(DeleteArea, (o) => SelectedArea != null);
            OpenAreaCommand = new(OpenArea);
        }
        public RelayCommand DeleteCustomerCommand { get; set; }
        public RelayCommand AddCustomerCommand { get; set; }
        public RelayCommand AddProjectCommand { get; set; }
        public RelayCommand DeleteProjectCommand { get; set; }
        public RelayCommand AddAreaCommand { get; set; }
        public RelayCommand DeleteAreaCommand { get; set; }
        public RelayCommand OpenAreaCommand { get; set; }

        void DeleteCustomer(object obj)
        {
            if (MessageBox.Show("Удалить этого заказчика?", "Удаление", MessageBoxButton.YesNo) == MessageBoxResult.No) return;
            else
            {
                db.Customers.Remove(SelectedCustomer);
                db.SaveChanges();
            }
        }

        void AddCustomer(object obj)
        {
            var customer = new Customer() { Name = "", Email = "" };
            if (new AddCustomerDialogWindow(customer).ShowDialog() == false) return;
            else if (customer.Name == "" || customer.Email == "")
            {
                MessageBox.Show("Заполните все поля!", "Предупреждение");
            }
            else
            {
                db.Customers.Add(customer);
                db.SaveChanges();
                SelectedCustomer = customer;
            }            
        }

        void DeleteProject(object obj)
        {
            if (MessageBox.Show("Удалить этот проект?", "Удаление", MessageBoxButton.YesNo) == MessageBoxResult.No) return;
            else
            {
                db.Projects.Remove(SelectedProject);
                db.SaveChanges();
            }
        }

        void AddProject(object obj)
        {
            var project = new Project() { Name = "", Address = "", Customer = SelectedCustomer };
            if (new AddProjectDialogWindow(project).ShowDialog() == false) return;
            else if (project.Name == "" || project.Address == "")
            {
                MessageBox.Show("Заполните все поля!", "Предупреждение");
            }
            else
            {
                db.Projects.Add(project);
                db.SaveChanges();
                SelectedProject = project;
                OnPropertyChanged(nameof(SelectedProject));
                OnPropertyChanged(nameof(SelectedCustomer));
            }
        }

        void AddArea(object obj)
        {
            var a = new Area() { Project = SelectedProject };
            db.Areas.Add(a);
            db.SaveChanges();
            a.Name = $"Площадь {a.Id}";
            db.SaveChanges();
            OnPropertyChanged(nameof(SelectedProject));
            SelectedArea = a;
        }
        void DeleteArea(object obj)
        {
            if (MessageBox.Show("Удалить выбранную площадь?", "Удаление", MessageBoxButton.YesNo) == MessageBoxResult.No) return;
            db.Areas.Remove(SelectedArea);
            db.SaveChanges();
            OnPropertyChanged(nameof(SelectedProject));
            db.Areas.Load();
        }
        void OpenArea(object obj)
        {
            new AreaWindow()
            {
                DataContext = new AreaViewModel((Area)obj)
            }.ShowDialog();
            OnPropertyChanged(nameof(SelectedProject.Areas));
            OnPropertyChanged(nameof(obj));
            Redraw();
        }

        public Customer SelectedCustomer
        {
            get => selectedCustomer;
            set
            {
                selectedCustomer = value;
                OnPropertyChanged();
                Redraw();
            }
        }
        public Project SelectedProject
        {
            get => selectedProject;
            set
            {
                selectedProject = value;
                OnPropertyChanged(nameof(SelectedProject));
                Redraw();
            }
        }

        public Area SelectedArea
        {
            get => selectedArea;
            set
            {
                selectedArea = value;
                OnPropertyChanged();
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
            foreach (var area in SelectedProject?.Areas ?? new())
            {
                //if (area.Project.Areas.Count >= 2 && area.AreAreasIntersecting()) MessageBox.Show("Конфликт пересечения площадей! Необходимо исправить.", "Внимание!");
                if (area == SelectedArea && area.IsCorrect()) area.Draw(newimage, Brushes.Blue);
                else if (area == SelectedArea && !area.IsCorrect()) area.Draw(newimage, Brushes.Orange);
                else if (area != SelectedArea && area.IsCorrect()) area.Draw(newimage, Brushes.LightBlue);
                else if (area != SelectedArea && !area.IsCorrect()) area.Draw(newimage, Brushes.Red);
                foreach (var profile in area.Profiles ?? new())
                {
                    if (area == SelectedArea && profile.IsCorrect()) profile.Draw(newimage, Brushes.Gray);
                    else if (area == SelectedArea && !profile.IsCorrect()) profile.Draw(newimage, Brushes.Red);
                    else if (area != SelectedArea && profile.IsCorrect()) profile.Draw(newimage, Brushes.LightGray);
                    else if (area != SelectedArea && !profile.IsCorrect()) profile.Draw(newimage, Brushes.IndianRed);
                }
            }
            Image = newimage.Render();
        }
    }
}
