using Prn232.Lab1.Repositories.Domain;

namespace Prn232.Lab1.Repositories.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IGenericRepository<Student> StudentRepository { get; }
        IGenericRepository<Course> CourseRepository { get; }
        IGenericRepository<Enrollment> EnrollmentRepository { get; }
        IGenericRepository<Semester> SemesterRepository { get; }
        IGenericRepository<Subject> SubjectRepository { get; }
        IGenericRepository<User> UserRepository { get; }

        Task<int> SaveChangesAsync();
        Task ClearLmsDataAsync();
    }
}
