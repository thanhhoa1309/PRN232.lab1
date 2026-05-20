namespace Prn232.Lab1.Repositories.Domain
{
    public class Semester
    {
        public int SemesterId { get; set; }

        public string SemesterName { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public ICollection<Course> Courses { get; set; } = new List<Course>();
    }
}
