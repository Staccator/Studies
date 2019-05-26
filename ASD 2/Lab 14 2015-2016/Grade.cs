namespace Linq
{
    public class GradeInfo
    {
        public string IndexNo { get; set; }
        public double Grade { get; set; }
        public string Subject { get; set; }

        public GradeInfo(string indexno, double grade, string subject)
        {
            IndexNo = indexno;
            Grade = grade;
            Subject = subject;
        }
    }
}
