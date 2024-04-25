using System.Collections.ObjectModel;
using System.Windows.Media;

namespace MPP.Model
{
    public class Project : PropChange
    {
        private int id;
        private string name;
        string? address;
        Customer customer;
        ObservableCollection<Area> areas;

        public override string ToString()
        {
            return $"{name}, {address ?? ""}";
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

        public string? Address
        {
            get { return address; }
            set
            {
                address = value;
                OnPropertyChanged(nameof(Address));
            }
        }

        public Customer Customer
        {
            get { return customer; }
            set
            {
                customer = value;
                OnPropertyChanged(nameof(Customer));
            }
        }

        public ObservableCollection<Area> Areas
        {
            get { return areas; }
            set
            {
                areas = value;
                OnPropertyChanged(nameof(Areas));
            }
        }
    }
}