using Prn232.Lab1.Repositories;
using Prn232.Lab1.Repositories.Domain;
using Prn232.Lab1.Repositories.Interfaces;

namespace Prn232.Lab1.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly Prn232Lab1DbContext _dbContext;

        public UnitOfWork(Prn232Lab1DbContext dbContext,
            IGenericRepository<Student> studentRepository,
            IGenericRepository<Course> courseRepository,
            IGenericRepository<Enrollment> enrollmentRepository,
            IGenericRepository<Semester> semesterRepository,
            IGenericRepository<Subject> subjectRepository,
            IGenericRepository<User> userRepository)
        {
            _dbContext = dbContext;
            StudentRepository = studentRepository;
            CourseRepository = courseRepository;
            EnrollmentRepository = enrollmentRepository;
            SemesterRepository = semesterRepository;
            SubjectRepository = subjectRepository;
            UserRepository = userRepository;
        }

        public IGenericRepository<Student> StudentRepository { get; }
        public IGenericRepository<Course> CourseRepository { get; }
        public IGenericRepository<Enrollment> EnrollmentRepository { get; }
        public IGenericRepository<Semester> SemesterRepository { get; }
        public IGenericRepository<Subject> SubjectRepository { get; }
        public IGenericRepository<User> UserRepository { get; }

        public async Task<int> SaveChangesAsync()
        {
            return await _dbContext.SaveChangesAsync();
        }

        public void Dispose()
        {
            _dbContext?.Dispose();
        }
    }
}
