using System;

namespace MPP.Model
{
    public class PicketValue : PropChange
    {
        private int id;
        private Picket picket;
        private double amplitude;
        private double h_value;
        private DateTime time; 

        public int Id
        {
            get { return id; }
            set
            {
                id = value;
                OnPropertyChanged(nameof(Id));
            }
        }

        public double Amplitude
        {
            get { return amplitude; }
            set
            {
                amplitude = value;
                OnPropertyChanged(nameof(Amplitude));
            }
        }

        public double H_value
        {
            get { return h_value; }
            set
            {
                h_value = value;
                OnPropertyChanged(nameof(H_value));
            }
        }

        public Picket Picket
        {
            get { return picket; }
            set
            {
                picket = value;
                OnPropertyChanged(nameof(Picket));
            }
        }

        public DateTime Time 
        {
            get { return time; }
            set
            {
                time = value;
                OnPropertyChanged(nameof(Time));
            }
        }
    }
}
