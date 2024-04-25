using System.Collections.ObjectModel;

namespace MPP.Model
{
    public class Customer : PropChange
    {
        private int id;
        private string name;
        private string email;
        ObservableCollection<Project> projects;

        public override string ToString()
        {
            return $"{name}, {email}";
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

        public string? Name
        {
            get { return name; }
            set
            {
                name = value;
                OnPropertyChanged(nameof(Name));
            }
        }

        public string Email
        {
            get { return email; }
            set
            {
                email = value;
                OnPropertyChanged(nameof(Email));
            }
        }

        public ObservableCollection<Project> Projects
        {
            get { return projects; }
            set
            {
                projects = value;
                OnPropertyChanged(nameof(Projects));
            }
        }
    }
}