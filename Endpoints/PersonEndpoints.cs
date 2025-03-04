using Microsoft.EntityFrameworkCore;
using RestApiLabb.Data;
using RestApiLabb.DTOs.EducationDTOs;
using RestApiLabb.DTOs.ExperienceDTOs;
using RestApiLabb.DTOs.PersonDTOs;
using RestApiLabb.Models;
using System;
using System.ComponentModel.DataAnnotations;

namespace RestApiLabb.Endpoints
{
    public class PersonEndpoints
    {
        public static void RegisterEndpoints(WebApplication app)
        {
            // Get all info about all persons, and their respective experience and education.
            app.MapGet("/person/persons", async (RestApiLabbDbContext context) =>
            {
                var persons = await context.Persons
                    .Include(p => p.Educations)
                    .Include(p => p.Experiences)
                    .Select(p => new PersonsDto
                    {
                        Name = p.FullName,
                        EmailAddress = p.Email,
                        Phone = p.MobileNumber,
                        Educations = p.Educations.Select(e => new EducationDto
                        {
                            School = e.School,
                            Degree = e.Degree,
                            StartDate = e.StartDate,
                            EndDate = e.EndDate
                        }).ToList(),
                        Experiences = p.Experiences.Select(e => new ExperienceDto
                        {
                            Company = e.Company,
                            JobTitle = e.JobTitle,
                            Description = e.Description,
                            StartYear = e.StartDate.Year,
                            EndYear = e.EndDate.HasValue ? e.EndDate.Value.Year : (int?)null
                        }).ToList()
                    })
                    .ToListAsync();

                return Results.Ok(persons);
            });

            // Get specific info on id
            app.MapGet("/person/{id}", async (RestApiLabbDbContext context, int id) =>
            {
                if (id <= 0)
                {
                    return Results.BadRequest(new { message = "Invalid person ID." });
                }

                try
                {
                    var person = await context.Persons
                        .Where(p => p.PersonId == id)
                        .Select(p => new PersonDto
                        {
                            Name = p.FullName,
                            EmailAddress = p.Email,
                            Phone = p.MobileNumber,
                            Description = p.Description
                        })
                        .SingleOrDefaultAsync();

                    if (person is null)
                    {
                        return Results.NotFound();
                    }

                    return Results.Ok(person);
                }
                catch (Exception)
                {
                    // For production level, use the ex message to log it.
                    
                    return Results.Json(new { message = "Something went unexpectedly wrong, please try again later." }, statusCode: 500);
                }
            }).WithName("GetPersonById");

            // Post/add a new Person
            app.MapPost("/person", async (PersonCreateDto newPerson, RestApiLabbDbContext context) =>
            {
                try
                {
                    var validationContext = new ValidationContext(newPerson);
                    var validationResult = new List<ValidationResult>();

                    bool isValid = Validator.TryValidateObject(newPerson, validationContext, validationResult, true);

                    if (!isValid)
                        return Results.BadRequest(new { errors = validationResult.Select(v => v.ErrorMessage).ToList() });

                    var existingPerson = await context.Persons
                        .Where(p => (p.Email == newPerson.Email || p.MobileNumber == newPerson.MobileNumber))
                        .FirstOrDefaultAsync();

                    if (existingPerson != null)
                    {
                        return Results.BadRequest(new { message = "A person with this email or phone number already exists." });
                    }

                    var person = new Person
                    {
                        FullName = newPerson.FullName,
                        Email = newPerson.Email,
                        MobileNumber = newPerson.MobileNumber,
                        Description = newPerson.Description
                    };

                    context.Persons.Add(person);
                    await context.SaveChangesAsync();

                    var personDto = new PersonDto
                    {
                        Name = person.FullName,
                        EmailAddress = person.Email,
                        Phone = person.MobileNumber,
                        Description = person.Description
                    };

                    return Results.CreatedAtRoute("GetPersonById", new { id = person.PersonId }, personDto);
                }
                catch (Exception)
                {
                    // For production level, use the ex message to log it.

                    return Results.Json(new { message = "Something went unexpectedly wrong, please try again later." }, statusCode: 500);
                }
            });

            // Put/Change existing info
            app.MapPut("/person/{id}", async (PersonUpdateDto personDto, RestApiLabbDbContext context, int id) =>
            {
                try
                {
                    var existingPerson = await context.Persons.FirstOrDefaultAsync(s => s.PersonId == id);
                    if (existingPerson == null)
                        return Results.NotFound($"Student with ID {id} not found.");

                    var validationContext = new ValidationContext(personDto);
                    var validationResult = new List<ValidationResult>();

                    bool isValid = Validator.TryValidateObject(personDto, validationContext, validationResult, true);

                    if (!isValid)
                        return Results.BadRequest(validationResult.Select(v => v.ErrorMessage));

                    var duplicateData = await context.Persons
                        .Where(p => (p.Email == personDto.EmailAddress || p.MobileNumber == personDto.Phone) && p.PersonId != id)
                        .FirstOrDefaultAsync();

                    if (duplicateData != null)
                    {
                        string duplicateField = duplicateData.Email == duplicateData.MobileNumber ? "email" : "phone number";
                        return Results.BadRequest(new { message = $"A person with this {duplicateField} already exists." });
                    }


                    existingPerson.FullName = personDto.FullName ?? existingPerson.FullName;
                    existingPerson.Email = personDto.EmailAddress ?? existingPerson.Email;
                    existingPerson.MobileNumber = personDto.Phone ?? existingPerson.MobileNumber;
                    existingPerson.Description = personDto.Description ?? existingPerson.Description;

                    await context.SaveChangesAsync();

                    var updatedPersonDto = new PersonDto
                    {
                        Name = existingPerson.FullName,
                        EmailAddress = existingPerson.Email,
                        Phone = existingPerson.MobileNumber,
                        Description = existingPerson.Description
                    };

                    return Results.AcceptedAtRoute("GetPersonById", new { id = existingPerson.PersonId }, updatedPersonDto);
                }
                catch (Exception)
                {
                    // For production level, use the ex message to log it.

                    return Results.Json(new { message = "Something went unexpectedly wrong, please try again later." }, statusCode: 500);
                }
            });

            // Delete a record
            app.MapDelete("/person/{id}", async (RestApiLabbDbContext context, int id) =>
            {
                var person = await context.Persons
                    .FirstOrDefaultAsync(p => p.PersonId == id);
                // Since in my program, The experiences and educations are of course tied to that person

                if (person is null)
                    return Results.NotFound(new { message = "Person not found" });

                context.Persons.Remove(person);
                await context.SaveChangesAsync();

                return Results.Ok(new { message = "Person deleted successfully" });
            });
        }
    }
}
