using Microsoft.EntityFrameworkCore;
using RestApiLabb.Data;
using RestApiLabb.DTOs.EducationDTOs;
using RestApiLabb.Models;
using System.ComponentModel.DataAnnotations;

namespace RestApiLabb.Endpoints
{
    public class EducationEndpoints
    {
        public static void RegisterEndpoints(WebApplication app)
        {
            // Get specific education on Id
            app.MapGet("/educations/{id}", async (RestApiLabbDbContext context, int id) =>
            {
                if (id <= 0)
                {
                    return Results.BadRequest(new { message = "Invalid education ID." });
                }

                try
                {
                    var education = await context.Educations
                        .Where(e => e.EducationId == id)
                        .Select(e => new EducationDto
                        {
                            School = e.School,
                            Degree = e.Degree,
                            StartDate = e.StartDate,
                            EndDate = e.EndDate
                        })
                        .SingleOrDefaultAsync();

                    if (education == null)
                    {
                        return Results.NotFound(new { message = "Education not found." });
                    }

                    return Results.Ok(education);
                }
                catch (Exception)
                {
                    return Results.Json(new { message = "Something went unexpectedly wrong, please try again later." }, statusCode: 500);
                }
            }).WithName("GetEducationById");

            // Post/Add a new education
            app.MapPost("/educations", async (RestApiLabbDbContext context, EducationCreateDto newEducation) =>
            {
                if (newEducation == null)
                {
                    return Results.BadRequest(new { message = "Invalid education data." });
                }

                try
                {
                    var validationContext = new ValidationContext(newEducation);
                    var validationResult = new List<ValidationResult>();
                    bool isValid = Validator.TryValidateObject(newEducation, validationContext, validationResult, true);

                    if (!isValid)
                    {
                        return Results.BadRequest(validationResult.Select(v => v.ErrorMessage));
                    }

                    // Ensure PersonId is valid
                    var person = await context.Persons.FirstOrDefaultAsync(p => p.PersonId == newEducation.PersonId);
                    if (person == null)
                    {
                        return Results.BadRequest(new { message = "Person not found." });
                    }

                    var education = new Education
                    {
                        School = newEducation.School,
                        Degree = newEducation.Degree,
                        StartDate = newEducation.StartDate,
                        EndDate = newEducation.EndDate,
                        PersonId_FK = newEducation.PersonId
                    };

                    context.Educations.Add(education);
                    await context.SaveChangesAsync();

                    return Results.Created($"/educations/{education.EducationId}", new { message = "Education created successfully." });
                }
                catch (Exception)
                {
                    return Results.Json(new { message = "Something went unexpectedly wrong, please try again later." }, statusCode: 500);
                }
            });

            // Put/Change existing info
            app.MapPut("/educations/{id}", async (RestApiLabbDbContext context, EducationPutDto educationDto, int id) =>
            {
                if (id <= 0)
                {
                    return Results.BadRequest(new { message = "Invalid education ID." });
                }

                try
                {
                    var education = await context.Educations
                        .FirstOrDefaultAsync(e => e.EducationId == id);

                    if (education == null)
                    {
                        return Results.NotFound(new { message = "Education not found." });
                    }

                    // Validate incoming data
                    var validationContext = new ValidationContext(educationDto);
                    var validationResult = new List<ValidationResult>();
                    bool isValid = Validator.TryValidateObject(educationDto, validationContext, validationResult, true);

                    if (!isValid)
                    {
                        return Results.BadRequest(validationResult.Select(v => v.ErrorMessage));
                    }

                    education.School = educationDto.School;
                    education.Degree = educationDto.Degree;
                    education.StartDate = educationDto.StartDate;
                    education.EndDate = educationDto.EndDate;

                    await context.SaveChangesAsync();

                    return Results.Ok(new { message = "Education updated successfully." });
                }
                catch (Exception)
                {
                    return Results.Json(new { message = "Something went unexpectedly wrong, please try again later." }, statusCode: 500);
                }
            });

            // Patch/Change some of the existing info
            app.MapPatch("/educations/{id}", async (RestApiLabbDbContext context, EducationPatchDto patchedEducation, int id) =>
            {
                if (id <= 0)
                {
                    return Results.BadRequest(new { message = "Invalid education ID." });
                }

                try
                {
                    var education = await context.Educations
                    .FirstOrDefaultAsync(e => e.EducationId == id);

                    if (education == null)
                    {
                        return Results.NotFound(new { message = "Education not found." });
                    }

                    // Apply changes only for non-null fields
                    if (patchedEducation.School != null)
                        education.School = patchedEducation.School;

                    if (patchedEducation.Degree != null)
                        education.Degree = patchedEducation.Degree;

                    if (patchedEducation.StartDate.HasValue)
                        education.StartDate = patchedEducation.StartDate.Value;

                    if (patchedEducation.EndDate.HasValue)
                        education.EndDate = patchedEducation.EndDate;

                    await context.SaveChangesAsync();

                    return Results.Ok(new { message = "Education partially updated successfully." });
                }
                catch (Exception)
                {
                    return Results.Json(new { message = "Something went unexpectedly wrong, please try again later." }, statusCode: 500);
                }
            });

            // Delete a record
            app.MapDelete("/educations/{id}", async (RestApiLabbDbContext context, int id) =>
            {
                if (id <= 0)
                {
                    return Results.BadRequest(new { message = "Invalid education ID." });
                }

                try
                {
                    var education = await context.Educations
                        .FirstOrDefaultAsync(e => e.EducationId == id);

                    if (education == null)
                    {
                        return Results.NotFound(new { message = "Education not found." });
                    }

                    context.Educations.Remove(education);
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
