using MPP.Model;
using MPP.View;
using System.Threading.Channels;
using System.Windows;
using System.Windows.Input;

namespace MPP.ViewModel
{
    public class DBConnectViewModel : PropChange
    {
        private string dbName;
        public string DbName
        {
            get { return dbName; }
            set
            {
                dbName = value;
                OnPropertyChanged(nameof(DbName));
            }
        }

        public ICommand ConnectCommand { get; set; }
        public ICommand EnterDefaultCommand { get; set; }

        public DBConnectViewModel()
        {
            ConnectCommand = new RelayCommand(ConnectToDatabase);
            EnterDefaultCommand = new RelayCommand(EnterDefault);
        }

        private void EnterDefault(object obj)
        {
            DbName = "Demo";
            OnPropertyChanged(nameof(DbName));
        }

        private void ConnectToDatabase(object obj)
        {
            if (!string.IsNullOrWhiteSpace(DbName))
            {
                using (var db = new ApplicationContext(DbName.ToString()))
                {
                    bool exists = db.Database.CanConnect();
                    if (exists)
                    {
                        MessageBox.Show($"Подключение к {DbName}", "Подключение");                        
                    }
                    else
                    {
                        db.Database.EnsureCreated();
                        MessageBox.Show($"База данных {DbName} создана, подключение.", "Подключение");
                    }
                    var win = new MainWindow()
                    {
                        DataContext = new MainViewModel()
                    };
                    win.Show();
                    Application.Current.MainWindow.Close();
                    Application.Current.MainWindow = win;
                    OnPropertyChanged(nameof(obj));
                }
            }
        }
    }
}
