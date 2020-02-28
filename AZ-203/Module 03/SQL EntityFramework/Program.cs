using System;
using Microsoft.EntityFrameworkCore.Design;
using SQL_EntityFramework.Data;
using SQL_EntityFramework.Models;
using System.Collections.Generic;


namespace SQL_EntityFramework
{
    class Program
    {
        
        static void Main(string[] args)
        {
            using (var context = new SchoolContext())
            {
                var course = new Course {
                    CourseID = 001,
                    Title = "Shakespeare",
                    Credits = 123
                };
                

                var student = new Student {
                    EnrollmentDate = DateTime.Now,
                    LastName = "Roth",
                    FirstMidName = "Kai",
                    Enrollments = new List<Enrollment>()
                };

                var enrollment = new Enrollment {
                    Course = course,
                    CourseID = 001,
                    StudentID = 1,
                    Grade = null,
                    Student = student,
                };
                student.Enrollments.Add(enrollment);

                context.Add(course);
                context.Add(student);
                context.Add(enrollment);

                context.SaveChanges();
            }

        }
    }
}
