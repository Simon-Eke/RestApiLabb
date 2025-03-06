using Microsoft.EntityFrameworkCore;
using RestApiLabb.Models;

namespace RestApiLabb.Data
{
    public class RestApiLabbDbContext : DbContext
    {
        public RestApiLabbDbContext(DbContextOptions<RestApiLabbDbContext> options) : base(options)
        {
            
        }

        public DbSet<Person> Persons { get; set; }
        public DbSet<Education> Educations { get; set; }
        public DbSet<Experience> Experiences { get; set; }

        // TODO - Seed some data into database
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Person>().HasData(
                new Person
                {
                    PersonId = 1,
                    FullName = "Simon Eke",
                    Description = "A future fullstack developer within the .NET. family of languages and practises. OOP, Agile, C#, EF Core, API, T-SQL, Database, Azure, React.",
                    Email = "simon3eke@gmail.com",
                    MobileNumber = "+46705516182"
                }
            );

            for (int i = 2; i < 100; i++)
            {
                modelBuilder.Entity<Person>().HasData(
                    new Person
                    {
                        PersonId = i,
                        FullName = $"FName LName{i}",
                        Description = $"Random Description {i}",
                        Email = $"FName.LName{i}@mail.com",
                        MobileNumber = $"+46706666{i}"
                    }
                );
            }

            modelBuilder.Entity<Education>().HasData(
                new Education
                {
                    EducationId = 1,
                    PersonId_FK = 1,
                    School = "Chas Academy",
                    Degree = "Full-stack web-dev, backend focus, APIs & services",
                    StartDate = new DateOnly(2024, 9, 2),
                    EndDate = null
                },
                new Education
                {
                    EducationId = 2,
                    PersonId_FK = 1,
                    School = "Sundsgården",
                    Degree = "1-year acting program, stage skills & performance",
                    StartDate = new DateOnly(2022, 8, 22),
                    EndDate = new DateOnly(2023, 6, 9)
                }
            );

            for (int i = 3; i < 300; i++)
            {
                modelBuilder.Entity<Education>().HasData(
                    new Education
                    {
                        EducationId = i,
                        PersonId_FK = (i % 99) + 1,
                        School = $"School {i}",
                        Degree = $"School Degree {i}",
                        StartDate = new DateOnly(2020, 7, 12),
                        EndDate = new DateOnly(2021, 11, 4)
                    }
                );
            }

            modelBuilder.Entity<Experience>().HasData(
                new Experience
                {
                    ExperienceId = 1,
                    PersonId_FK = 1,
                    JobTitle = "Website developer",
                    Company = "Simon Eke Consulting",
                    Description = "Worked on developing a website for a café",
                    StartDate = new DateOnly(2025, 1, 23),
                    EndDate = null
                },
                new Experience
                {
                    ExperienceId = 2,
                    PersonId_FK = 1,
                    JobTitle = "Learner",
                    Company = "Life, the Universe and Everything",
                    Description = "Self-development. Rode the roller-coaster of Life",
                    StartDate = new DateOnly(1998, 4, 29),
                    EndDate = null
                },
                new Experience
                {
                    ExperienceId = 3,
                    PersonId_FK = 1,
                    JobTitle = "Backend-developer",
                    Company = "Big Tech",
                    Description = "Manifesting that I'll be doing databases, APIs",
                    StartDate = new DateOnly(2024, 12, 31),
                    EndDate = new DateOnly(2025, 1, 1)
                }
            );

            for (int i = 4; i < 400; i++)
            {
                modelBuilder.Entity<Experience>().HasData(
                    new Experience
                    {
                        ExperienceId = i,
                        PersonId_FK = (i % 99) + 1,
                        JobTitle = $"Job Title {i}",
                        Company = $"Company {i}",
                        Description = $"Random Description {i}",
                        StartDate = new DateOnly(2016, 2, 3),
                        EndDate = new DateOnly(2019, 9, 11)
                    }
                );
            }
        }
    }
}
