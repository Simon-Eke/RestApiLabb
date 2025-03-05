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
            // GET All info about All persons, and their respective experience and education.
            app.MapGet("/person", async (RestApiLabbDbContext context) =>
            {
                try
                {
                    var persons = await context.Persons
                    .Include(p => p.Educations)
                    .Include(p => p.Experiences)
                    .Select(p => new PersonListDto
                    {
                        Name = p.FullName,
                        EmailAddress = p.Email,
                        Phone = p.MobileNumber,
                        Description = p.Description,
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

                    if (persons == null)
                    {
                        return Results.NotFound();
                    }

                    return Results.Ok(persons);
                }
                catch (Exception)
                {
                    return Results.Json(new { message = "Something went unexpectedly wrong, please try again later." }, statusCode: 500);
                }
            });

            // GET specific Person info on Id
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
                            MobileNumber = p.MobileNumber,
                            Description = p.Description
                        })
                        .SingleOrDefaultAsync();

                    if (person == null)
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

            // GET specific Person with details on Id
            app.MapGet("/person/{id}/details", async (RestApiLabbDbContext context, int id) =>
            {
                if (id <= 0)
                {
                    return Results.BadRequest(new { message = "Invalid person ID." });
                }

                try
                {
                    var person = await context.Persons
                        .Where(p => p.PersonId == id)
                        .Select(p => new PersonWithDetailsDto
                        {
                            Name = p.FullName,
                            EmailAddress = p.Email,
                            Phone = p.MobileNumber,
                            Description = p.Description,
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
                        .SingleOrDefaultAsync();

                    if (person == null)
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
            }).WithName("GetPersonDetailsById");

            // POST/add a new Person
            app.MapPost("/person", async (PersonCreateDto newPerson, RestApiLabbDbContext context) =>
            {
                if (newPerson == null)
                {
                    return Results.BadRequest(new { message = "Invalid Person data." });
                }

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
                        MobileNumber = person.MobileNumber,
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

            // PUT/Change existing info
            app.MapPut("/person/{id}", async (PersonPutDto personDto, RestApiLabbDbContext context, int id) =>
            {
                try
                {
                    var existingPerson = await context.Persons.FirstOrDefaultAsync(s => s.PersonId == id);
                    if (existingPerson == null)
                        return Results.NotFound($"Person with ID {id} not found.");

                    // Validation for provided Data
                    var validationContext = new ValidationContext(personDto);
                    var validationResult = new List<ValidationResult>();
                    bool isValid = Validator.TryValidateObject(personDto, validationContext, validationResult, true);

                    if (!isValid)
                        return Results.BadRequest(validationResult.Select(v => v.ErrorMessage));

                    // Validation for duplicate records
                    var duplicateData = await context.Persons
                        .Where(p => (p.Email == personDto.EmailAddress || p.MobileNumber == personDto.Phone) && p.PersonId != id)
                        .FirstOrDefaultAsync();

                    if (duplicateData != null)
                    {
                        string duplicateField = duplicateData.Email == duplicateData.MobileNumber ? "email" : "phone number";
                        return Results.BadRequest(new { message = $"A person with this {duplicateField} already exists." });
                    }


                    // Replace old data with new
                    existingPerson.FullName = personDto.FullName;
                    existingPerson.Email = personDto.EmailAddress;
                    existingPerson.MobileNumber = personDto.Phone;
                    existingPerson.Description = personDto.Description;

                    await context.SaveChangesAsync();

                    var updatedPersonDto = new PersonDto
                    {
                        Name = existingPerson.FullName,
                        EmailAddress = existingPerson.Email,
                        MobileNumber = existingPerson.MobileNumber,
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

            // PATCH/Change partial existing info
            app.MapPatch("/person/{id}", async (PersonPatchDto personDto, RestApiLabbDbContext context, int id) =>
            {
                try
                {
                    var existingPerson = await context.Persons.FirstOrDefaultAsync(s => s.PersonId == id);
                    if (existingPerson == null)
                        return Results.NotFound($"Person with ID {id} not found.");

                    // Validation for provided Data
                    var validationContext = new ValidationContext(personDto);
                    var validationResult = new List<ValidationResult>();
                    bool isValid = Validator.TryValidateObject(personDto, validationContext, validationResult, true);

                    if (!isValid)
                        return Results.BadRequest(validationResult.Select(v => v.ErrorMessage));

                    // Validation for duplicate records
                    var duplicateData = await context.Persons
                        .Where(p => (p.Email == personDto.EmailAddress || p.MobileNumber == personDto.Phone) && p.PersonId != id)
                        .FirstOrDefaultAsync();

                    if (duplicateData != null)
                    {
                        string duplicateField = duplicateData.Email == duplicateData.MobileNumber ? "email" : "phone number";
                        return Results.BadRequest(new { message = $"A person with this {duplicateField} already exists." });
                    }


                    // Replaces only the fields that are provided in the request
                    existingPerson.FullName = personDto.FullName ?? existingPerson.FullName;
                    existingPerson.Email = personDto.EmailAddress ?? existingPerson.Email;
                    existingPerson.MobileNumber = personDto.Phone ?? existingPerson.MobileNumber;
                    existingPerson.Description = personDto.Description ?? existingPerson.Description;

                    await context.SaveChangesAsync();

                    var updatedPersonDto = new PersonDto
                    {
                        Name = existingPerson.FullName,
                        EmailAddress = existingPerson.Email,
                        MobileNumber = existingPerson.MobileNumber,
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

            // DELETE a record
            app.MapDelete("/person/{id}", async (RestApiLabbDbContext context, int id) =>
            {
                try
                {
                    var person = await context.Persons
                    .FirstOrDefaultAsync(p => p.PersonId == id);

                    if (person == null)
                        return Results.NotFound(new { message = "Person not found" });

                    context.Persons.Remove(person);
                    await context.SaveChangesAsync();

                    return Results.NoContent();
                }
                catch (Exception)
                {
                    return Results.Json(new { message = "Something went unexpectedly wrong, please try again later." }, statusCode: 500);
                }
            });
        }
    }
}
