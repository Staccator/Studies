using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace WpfApp2
{
    /// <summary>
    /// Logika interakcji dla klasy AddEmployeeHere.xaml
    /// </summary>
    public partial class AddEmployee : Window, IComponentConnector
    {
        private readonly ObservableCollection<Employee> ListOfEmployees;

        public static readonly DependencyProperty IsMaleProperty = DependencyProperty.Register("IsMale", typeof(bool), typeof(AddEmployee), new PropertyMetadata(true));
        public static readonly DependencyProperty EmplProp = DependencyProperty.Register("AddedEmployee", typeof(Employee), typeof(AddEmployee), new PropertyMetadata((object)null));
        
        public bool IsMale{ get {return (bool)GetValue(IsMaleProperty);}
            set { SetValue(IsMaleProperty, value);}
        }

        public Employee AddedEmployee{get{return (Employee)GetValue(EmplProp);}
            set { SetValue(EmplProp, value);}
        }
        
        public AddEmployee(ObservableCollection<Employee> employees)
        {
            ListOfEmployees = employees;
            AddedEmployee = new Employee();
            DateTime dt = System.DateTime.Now;
            dt = dt.AddYears(-30);
            AddedEmployee.BirthDate = dt;
            AddedEmployee.Salary = 5000;
            InitializeComponent();
        }

        private void Add_OnClick(object sender, RoutedEventArgs e)
        {
            AddedEmployee.Sex = (IsMale ? "Male" : "Female");
            ListOfEmployees.Add(AddedEmployee);
            DateTime dt = System.DateTime.Now;
            dt = dt.AddYears(-30);
            AddedEmployee.BirthDate = dt;
            AddedEmployee = new Employee("","","",dt,"",5000,Currency.PLN,Role.Worker);
        }
        
    }
}
