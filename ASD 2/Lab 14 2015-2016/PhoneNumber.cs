namespace Linq
{
    public class PhoneNumber
    {
        public string PESEL { get; set; }
        public string Phone { get; set; }
        public PhoneNumber(string pesel, string phone)
        {
            PESEL = pesel;
            Phone = phone;
        }
    }
}
