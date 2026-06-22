using Microsoft.EntityFrameworkCore;
using Prn232.Lab1.Repositories.Domain;
using Prn232.Lab1.Repositories.Interfaces;
using Prn232.Lab1.Service.Interfaces;
using Prn232.Lab1.Service.Utils;

namespace Prn232.Lab1.Service.Service;

public class SeedDataService : ISeedDataService
{
    private const int MinSemesters = 5;
    private const int MinSubjects = 10;
    private const int MinStudents = 50;
    private const int MinCourses = 20;
    private const int MinEnrollments = 500;

    private static readonly string[] SubjectPrefixes = ["SE", "CE", "AI", "SS", "DB"];
    private static readonly string[] EnrollmentStatuses = ["Active", "Completed", "Dropped", "Pending"];

    private readonly IUnitOfWork _unitOfWork;
    private readonly PasswordHasher _passwordHasher;

    public SeedDataService(IUnitOfWork unitOfWork, PasswordHasher passwordHasher)
    {
        _unitOfWork = unitOfWork;
        _passwordHasher = passwordHasher;
    }

    public async Task EnsureSeedAsync()
    {
        await SeedUsersIfEmptyAsync();

        if (await _unitOfWork.StudentRepository.GetAllAsQueryable().AnyAsync())
            return;

        await SeedLmsDataInternalAsync();
    }

    public async Task<object> SeedAsync()
    {
        await ClearAsync();
        await SeedUsersIfEmptyAsync();
        return await SeedLmsDataInternalAsync();
    }

    public Task ClearAsync()
    {
        return _unitOfWork.ClearLmsDataAsync();
    }

    private async Task SeedUsersIfEmptyAsync()
    {
        if (await _unitOfWork.UserRepository.GetAllAsQueryable().AnyAsync())
            return;

        var users = new List<User>
        {
            new()
            {
                Username = "admin",
                PasswordHash = _passwordHasher.HashPassword("123456"),
                Role = "Admin"
            },
            new()
            {
                Username = "teacher",
                PasswordHash = _passwordHasher.HashPassword("123456"),
                Role = "Lecturer"
            }
        };

        await _unitOfWork.UserRepository.AddRangeAsync(users);
        await _unitOfWork.SaveChangesAsync();
    }

    private async Task<object> SeedLmsDataInternalAsync()
    {
        var random = new Random(42);

        var semesters = new List<Semester>
        {
            new() { SemesterName = "Spring 2024", StartDate = new DateTime(2024, 1, 15), EndDate = new DateTime(2024, 5, 15) },
            new() { SemesterName = "Summer 2024", StartDate = new DateTime(2024, 6, 1), EndDate = new DateTime(2024, 8, 15) },
            new() { SemesterName = "Fall 2024", StartDate = new DateTime(2024, 9, 1), EndDate = new DateTime(2024, 12, 20) },
            new() { SemesterName = "Spring 2025", StartDate = new DateTime(2025, 1, 15), EndDate = new DateTime(2025, 5, 15) },
            new() { SemesterName = "Fall 2025", StartDate = new DateTime(2025, 9, 1), EndDate = new DateTime(2025, 12, 20) }
        };

        var subjects = Enumerable.Range(1, MinSubjects)
            .Select(i => new Subject
            {
                SubjectCode = $"{SubjectPrefixes[i % SubjectPrefixes.Length]}{10000 + i:D5}",
                SubjectName = $"Subject {i}",
                Credit = 2 + (i % 3)
            })
            .ToList();

        var students = Enumerable.Range(1, MinStudents)
            .Select(i => new Student
            {
                FullName = $"Student Nguyen {i}",
                Email = $"SE{18000 + i:D5}@fpt.edu.vn",
                DateOfBirth = new DateTime(2000 + (i % 5), (i % 12) + 1, (i % 28) + 1)
            })
            .ToList();

        await _unitOfWork.SemesterRepository.AddRangeAsync(semesters);
        await _unitOfWork.SubjectRepository.AddRangeAsync(subjects);
        await _unitOfWork.StudentRepository.AddRangeAsync(students);
        await _unitOfWork.SaveChangesAsync();

        var courses = Enumerable.Range(1, MinCourses)
            .Select(i => new Course
            {
                CourseName = $"Course {i}",
                SemesterId = semesters[random.Next(semesters.Count)].SemesterId
            })
            .ToList();

        await _unitOfWork.CourseRepository.AddRangeAsync(courses);
        await _unitOfWork.SaveChangesAsync();

        var enrollments = Enumerable.Range(1, MinEnrollments)
            .Select(_ => new Enrollment
            {
                StudentId = students[random.Next(students.Count)].StudentId,
                CourseId = courses[random.Next(courses.Count)].CourseId,
                EnrollDate = DateTime.UtcNow.AddDays(-random.Next(1, 365)),
                Status = EnrollmentStatuses[random.Next(EnrollmentStatuses.Length)]
            })
            .ToList();

        await _unitOfWork.EnrollmentRepository.AddRangeAsync(enrollments);
        await _unitOfWork.SaveChangesAsync();

        return new
        {
            message = "Seed data created.",
            semesters = semesters.Count,
            subjects = subjects.Count,
            students = students.Count,
            courses = courses.Count,
            enrollments = enrollments.Count
        };
    }
}
