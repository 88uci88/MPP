using Microsoft.EntityFrameworkCore;
using MPP.Model;
using MPP.View;
using OxyPlot;
using OxyPlot.Annotations;
using OxyPlot.Series;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

namespace MPP.ViewModel
{
    class PicketViewModel : PropChange
    {
        private readonly ApplicationContext db = ApplicationContext.getInstance();
        private PicketValue selectedPicketValue;
        private Operator selectedOperator;
        private int currentTime = 0;
        private readonly Random rnd = new Random(); // Инициализируем генератор случайных чисел

        public Profile Profile { get; set; }
        public Picket Picket { get; set; }

        public ObservableCollection<Operator> Operators => db.Operators.Local.ToObservableCollection();

        public PicketViewModel() : this(new Picket()) { }

        public PicketViewModel(Picket picket)
        {
            Picket = picket;
            if (Picket.Operator is not null) SelectedOperator = Picket.Operator;
            AddPicketValueCommand = new RelayCommand(AddPicketValue);
            DeletePicketValueCommand = new RelayCommand(DeletePicketValue, (obj) => SelectedPicketValue != null);
            SavePicketValueCommand = new RelayCommand(SavePicketValue);
            AddOperatorCommand = new RelayCommand(AddOperator);
            DeleteOperatorCommand = new RelayCommand(DeleteOperator, (obj) => SelectedOperator != null);
            RefreshPlotCommand = new RelayCommand(RefreshPlot);
            AddRandomPicketValueCommand = new RelayCommand(AddRandomPicketValue);
            SetPlotModel();
        }

        public RelayCommand AddPicketValueCommand { get; set; }
        public RelayCommand RefreshPlotCommand { get; set; }
        public RelayCommand DeletePicketValueCommand { get; set; }
        public RelayCommand SavePicketValueCommand { get; set; }
        public RelayCommand AddOperatorCommand { get; set; }
        public RelayCommand DeleteOperatorCommand { get; set; }
        public RelayCommand AddRandomPicketValueCommand { get; set; }

        private void AddRandomPicketValue(object obj)
        {
            PicketValue newval = new PicketValue();
            if (Picket.PicketValues != null && Picket.PicketValues.Any())
            {
                newval.Amplitude = Picket.PicketValues.Last().Amplitude + rnd.Next(-15, 15);
                while (newval.Amplitude < 0)
                    newval.Amplitude = Picket.PicketValues.Last().Amplitude + rnd.Next(-15, 15);
                newval.H_value = Picket.PicketValues.Last().H_value + rnd.Next(0, 15);
                newval.Picket = Picket;
                Picket.PicketValues.Add(newval);
            }
            else
            {
                var val = new PicketValue() { H_value = 0, Amplitude = 0, Picket = Picket };
                Picket.PicketValues = new ObservableCollection<PicketValue>() { val };
            }
            db.SaveChanges();
            OnPropertyChanged(nameof(Picket));
            SetPlotModel();
        }

        private void SetPlotModel()
        {
            var plotModel = new PlotModel() { Title = "График МПП" };

            var xAxis = new OxyPlot.Axes.LinearAxis { Position = OxyPlot.Axes.AxisPosition.Bottom, Title = "Время (минуты)" };
            var yAxis = new OxyPlot.Axes.LinearAxis { Position = OxyPlot.Axes.AxisPosition.Left, Title = "Амплитуда", StartPosition = 1, EndPosition = 0, IsZoomEnabled = false };

            plotModel.Axes.Add(xAxis);
            plotModel.Axes.Add(yAxis);

            if (Picket.PicketValues != null && Picket.PicketValues.Any())
            {
                var lineSeries = new LineSeries
                {
                    Title = "График",
                    Color = OxyColors.Black,
                    StrokeThickness = 3,
                    MarkerType = MarkerType.Diamond,
                    MarkerSize = 4,
                    MarkerStroke = OxyColors.Red,
                    MarkerFill = OxyColors.Red
                };

                foreach (var val in Picket.PicketValues)
                {
                    lineSeries.Points.Add(new DataPoint(currentTime++, val.Amplitude));
                }
                plotModel.Series.Add(lineSeries);
            }

            PlotModel = plotModel;
        }

        private void AddOperator(object obj)
        {
            var oper = new Operator() { Name = "", Surname = "" };
            if (new AddOperatorDialogWindow(oper).ShowDialog() == false) return;
            else if (oper.Name == "" || oper.Surname == "")
            {
                MessageBox.Show("Заполните все поля!", "Предупреждение");
            }
            else
            {
                db.Operators.Add(oper);
                db.SaveChanges();
                SelectedOperator = oper;
            }
        }

        private void RefreshPlot(object obj)
        {
            SetPlotModel();
        }

        private void DeleteOperator(object obj)
        {
            if (MessageBox.Show("Удалить этого оператора?", "Удаление", MessageBoxButton.YesNo) == MessageBoxResult.No) return;
            else
            {
                db.Operators.Remove(SelectedOperator);
                SelectedOperator = null;
                db.SaveChanges();
            }
        }

        private void AddPicketValue(object obj)
        {
            var val = new PicketValue() { H_value = 0, Amplitude = 0, Picket = Picket };
            db.PicketValues.Add(val);
            db.SaveChanges();
            SelectedPicketValue = val;
            OnPropertyChanged(nameof(Picket));
            SetPlotModel();
        }

        private void DeletePicketValue(object obj)
        {
            db.PicketValues.Remove(SelectedPicketValue);
            db.SaveChanges();
            OnPropertyChanged(nameof(Picket));
            SetPlotModel();
        }

        private void SavePicketValue(object obj)
        {
            if (obj is PicketValue)
            {
                db.Entry((PicketValue)obj).State = EntityState.Modified;
                db.SaveChanges();
            }
        }

        public PicketValue SelectedPicketValue
        {
            get => selectedPicketValue;
            set
            {
                selectedPicketValue = value;
                OnPropertyChanged(nameof(SelectedPicketValue));
                SetPlotModel();
            }
        }

        public Operator SelectedOperator
        {
            get => selectedOperator;
            set
            {
                selectedOperator = value;
                Picket.Operator = value;
                OnPropertyChanged(nameof(SelectedOperator));
                db.Entry(Picket).State = EntityState.Modified;
                db.SaveChanges();
                SetPlotModel();
            }
        }

        private PlotModel plotModel;
        public PlotModel PlotModel
        {
            get => plotModel;
            set
            {
                plotModel = value;
                OnPropertyChanged(nameof(PlotModel));
            }
        }
    }
}
