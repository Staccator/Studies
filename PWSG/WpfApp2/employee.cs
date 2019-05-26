using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp2
{
    public enum Currency { PLN, USD, EUR, GBP, NOK }
    public enum Role { Worker, SeniorWorker, IT, Manager, Director, CEO }
    public class Employee : INotifyPropertyChanged
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Sex { get; set; }
        public DateTime BirthDate { get; set; }
        public string BirthCountry { get; set; }
        public int Salary { get; set; }
        public int SalaryW { get { return Salary; }
            set {
                Console.WriteLine("Test Salary Change");
                if (value != this.Salary)
                {
                    this.Salary = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("salary"));
                }
            }
        }
        public Currency SalaryCurrency { get; set; }
        public int Curr { get { return (int)SalaryCurrency; }
            set
            {
                Console.WriteLine("Salary Currency Change");
                if (value != (int)this.SalaryCurrency)
                {
                    this.SalaryCurrency = (Currency)value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("currency"));
                }
            }
        }
        public int role { get { return (int)CompanyRole; }
            set
            {
                Console.WriteLine("Role Change");
                if (value != (int)this.CompanyRole)
                {
                    this.CompanyRole = (Role)value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("role"));
                }
            }
        }
        public Role CompanyRole { get; set; }

        public string imienazwisko { get { return FirstName + " " + LastName; } }

       // public Action<object, PropertyChangedEventArgs> PropertyChanged { get; internal set; }

        public Employee(string firstName, string lastName, string sex, DateTime birthDate, string birthCountry, int salary, Currency salaryCurrency, Role companyRole)
        {
            FirstName = firstName;
            LastName = lastName;
            Sex = sex;
            BirthDate = birthDate;
            BirthCountry = birthCountry;
            SalaryW = salary;
            SalaryCurrency = salaryCurrency;
            CompanyRole = companyRole;
        }

        public Employee()
        {
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public override string ToString()
        {
            string addday = BirthDate.Day < 10 ? "0" : "";
            string addmonth = BirthDate.Month < 10 ? "0" : "";

            string str = FirstName + ";" + LastName + ";" + Sex + ";" + addday + $"{BirthDate.Day}.{addmonth}{BirthDate.Month}.{BirthDate.Year}" + ";" +
                BirthCountry + ";" + SalaryW + ";" + Curr + ";" + role;
            return str;
        }
    }
}
