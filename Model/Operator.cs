using System.Collections.ObjectModel;

namespace MPP.Model
{
    public class Operator : PropChange
    {
        private int id;
        private string name;
        private string surname;
        ObservableCollection<Picket>? pickets;

        public override string ToString()
        {
            return $"{name} {surname}";
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

        public string Name
        {
            get { return name; }
            set
            {
                name = value;
                OnPropertyChanged(nameof(Name));
            }
        }

        public string Surname
        {
            get { return surname; }
            set
            {
                surname = value;
                OnPropertyChanged(nameof(Surname));
            }
        }

        public ObservableCollection<Picket>? Pickets
        {
            get { return pickets; }
            set
            {
                pickets = value;
                OnPropertyChanged(nameof(Pickets));
            }
        }
    }
}
