namespace Linq
{
    public class Student
    {
        public string Name{ get; set; }
        public string Surname { get; set; }
        public string PESEL { get; set; }
        public string IndexNo { get; set; }
        public bool Gender {get; set;}

        public Student(string imie, string nazwisko, string pesel, string index, bool gender)
        {
            Name = imie;
            Surname = nazwisko;
            PESEL = pesel;
            IndexNo = index;
            Gender = gender;
        }
    

        public override string ToString()
        {
            var sb = new System.Text.StringBuilder();
            sb.AppendLine(System.String.Format("({0}, {1}, {2}, {3}, {4})", Name, Surname, PESEL, IndexNo, Gender ? "Male" : "Female"));
            return sb.ToString();
        }
    }
}
