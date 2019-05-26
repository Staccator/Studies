namespace Linq
{
    public class Obywatel
    {
        public string Name { get; set; }
        public string Surname { get; set; }
        public string PESEL { get; set; }
        public bool IsZUSPaid { get; set; }
        public bool AreTaxesPaid { get; set; }
        public bool IsMilitaryServiceServed { get; set; }
        public double Salary { get; set; }

        public Obywatel(string name, string surname, string pesel, bool zus, bool taxes, bool military, double salary)
        {
            Name = name;
            Surname = surname;
            PESEL = pesel;
            IsZUSPaid = zus;
            AreTaxesPaid = taxes;
            IsMilitaryServiceServed = military;
            Salary = salary;
        }
    }
}
