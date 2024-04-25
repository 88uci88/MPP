using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Windows;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPP.Model
{
    public class AreaPoint : PropChange
    {
        int id;
        double x;
        double y;
        Area area;

        public int Id
        {
            get { return id; }
            set
            {
                id = value;
                OnPropertyChanged(nameof(Id));
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
        public Area Area
        {
            get { return area; }
            set
            {
                area = value;
                OnPropertyChanged(nameof(Area));
            }
        }

        public Point AsPoint => new Point(x, y);
    }
}