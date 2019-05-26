using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using Microsoft.Win32;
using System.Windows.Markup;
using System.Globalization;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;

namespace WpfApp2
{
    public partial class MainWindow : Window, IComponentConnector
    {
        private bool state_saved = true;
        private string loadedFilePath;
        private string first_row;

        public ObservableCollection<Employee> employees
        {
            get { return (ObservableCollection<Employee>)GetValue(employees_prop); }
            set { SetValue(employees_prop, value); }
        }

        public int CurrentIndex
        {
            get { return (int)GetValue(index_prop); }
            set { SetValue(index_prop, value); }
        }

        public static readonly DependencyProperty index_prop = DependencyProperty.Register("CurrentIndex", typeof(int), typeof(MainWindow), new PropertyMetadata(0));
        public static readonly DependencyProperty employees_prop = DependencyProperty.Register("employees", typeof(ObservableCollection<Employee>), typeof(MainWindow), new PropertyMetadata((object)null));
        
        private AddEmployee AddWindow;
        
        private void Changed_Delegate(object sender, PropertyChangedEventArgs e)
        {
            state_saved = false;
        }

        private void Add_Employee_Click(object sender, RoutedEventArgs e)
        {
            if (employees != null)
            {
                if (AddWindow != null)
                {AddWindow.WindowState = WindowState.Normal;
                    return;
                }
                AddWindow = new AddEmployee(employees); AddWindow.Closed += delegate
                {
                    AddWindow = null;
                };
                AddWindow.Show();
            }
        }
        private void Delete_Employee_Click(object sender, RoutedEventArgs e)
        {
            if (employees != null &&  CurrentIndex >=  0  &&  CurrentIndex  <=  employees.Count  -  1)
            {
                employees.RemoveAt(CurrentIndex);
            }
        }

        private void Button_Click_Up(object sender, RoutedEventArgs e)
        {
            if (employees   !=   null   &&   CurrentIndex > 0)
            {
                int temp = CurrentIndex;
                Employee value = employees[CurrentIndex - 1]; employees[CurrentIndex - 1] = employees[CurrentIndex]; employees[CurrentIndex] = value;
                CurrentIndex = temp - 1;
            }
        }

        private void Button_Click_Down(object sender, RoutedEventArgs e)
        {
            if (employees   !=   null   &&   CurrentIndex != -1   &&   CurrentIndex  <  employees.Count - 1)
            {
                int temp = CurrentIndex;
                Employee value = employees[CurrentIndex + 1];employees[CurrentIndex + 1] = employees[CurrentIndex]; employees[CurrentIndex] = value;
                CurrentIndex = temp + 1;
            }
        }

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Open_Button_Click(object sender, RoutedEventArgs e)
        {
            if (!state_saved)
            {
                switch (MessageBox.Show( "Do you want to save changes before opening new file? " , "Save changes " ,  MessageBoxButton.YesNoCancel ))
                {
                    case  MessageBoxResult.Yes:  SaveState(false);
                        break;
                    case  MessageBoxResult.Cancel:  return;
                }
            }
            
            OpenFileDialog openFileDialog = new OpenFileDialog();
            List<string> Text = new List<string>();
            openFileDialog.Filter = "Csv files (*.csv)|*.csv";
            if (openFileDialog.ShowDialog() == true)
                Text = File.ReadAllLines(openFileDialog.FileName).ToList();
            employees = new ObservableCollection<Employee>();

            loadedFilePath = openFileDialog.FileName;
            first_row = Text.First();

            foreach (string line in Text.Skip(1))
            {
                string[] tab;
                tab = line.Split(';');
                string birth = tab[3];
                string firstName = tab[0];
                string lastName = tab[1];
                string Sex = tab[2];
                DateTime BirthDate = new DateTime(Int32.Parse(birth.Substring(6, 4)), Int32.Parse(birth.Substring(3, 2)), Int32.Parse(birth.Substring(0, 2)));
                string BirthCountry = tab[4];
                int Salary = Int32.Parse(tab[5]);
                Currency SalaryCurrency = (Currency)Int32.Parse(tab[6]);
                Role CompanyRole = (Role)Int32.Parse(tab[7]);
                Console.WriteLine();
                Employee empl = new Employee(firstName, lastName, Sex, BirthDate, BirthCountry, Salary, SalaryCurrency, CompanyRole);
                employees.Add(empl);
                for (int i = 0; i < 8; i++) Console.Write(tab[i] + "|");
            }

            employees.CollectionChanged += Collection_Changed_Delegate;
            foreach (Employee employee in employees)
            {
                employee.PropertyChanged += Changed_Delegate;
            }
            state_saved = true;
            //CurrentIndex = 0;
            lista.Visibility = Visibility.Visible;
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            SaveState(false);
        }

        private void Save_file_Click(object sender, RoutedEventArgs e)
        {
            SaveState();
        }
        
        private void Close_Click(object sender, RoutedEventArgs e)
        {

            if (!state_saved)
            {
                switch (MessageBox.Show("Do you want to save changes before closing?", "Save changes", MessageBoxButton.YesNoCancel))
                {
                    case MessageBoxResult.Yes:
                        SaveState(false);
                        break;
                    case MessageBoxResult.Cancel:
                        return;
                }
            }

            AddWindow?.Close(); // REMEMBER
            Close();
        }

        private void SaveState(bool askWhereToSave = true)
        {
            if (employees == null)
            {
                return;
            }
            if (askWhereToSave)
            {
                SaveFileDialog sfd = new SaveFileDialog();
                sfd.Filter = "CSV Files|*.csv";
                if (sfd.ShowDialog() != true) return;
                loadedFilePath = sfd.FileName;
            }
            List<string> employees_to_save = new List<string>();

            employees_to_save.Add(first_row);
            employees_to_save.AddRange(employees.Select(e => e.ToString()));
            File.WriteAllLines(loadedFilePath, employees_to_save.ToArray());
            state_saved = true;
        }

        private void Collection_Changed_Delegate(object sender, NotifyCollectionChangedEventArgs args)
        {
            if (! (args.OldItems is null))
            {
                foreach (Employee item in args.OldItems.OfType<Employee>())  item.PropertyChanged -= Changed_Delegate;
                
            }
            if (!(args.NewItems is null))
            {
                foreach (Employee item in args.NewItems.OfType<Employee>())   item.PropertyChanged += Changed_Delegate;
            }
            state_saved = false;
        }


    }


    

    
    
}
