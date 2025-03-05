using Microsoft.EntityFrameworkCore;
using RestApiLabb.Data;
using RestApiLabb.DTOs.ExperienceDTOs;
using RestApiLabb.DTOs.PersonDTOs;
using RestApiLabb.Models;
using System.ComponentModel.DataAnnotations;

namespace RestApiLabb.Endpoints
{
    public class ExperienceEndpoints
    {
        public static void RegisterEndpoints(WebApplication app)
        {
            // Get specific experience on Id
            app.MapGet("/experience/{id}", async (RestApiLabbDbContext context, int id) =>
            {
                if (id <= 0)
                {
                    return Results.BadRequest(new { message = "Invalid experience ID." });
                }

                try
                {
                    var experience = await context.Experiences
                        .Where(e => e.ExperienceId == id)
                        .Select(e => new ExperienceDto
                        {
                            Company = e.Company,
                            JobTitle = e.JobTitle,
                            Description = e.Description,
                            StartYear = e.StartDate.Year,
                            EndYear = e.EndDate.HasValue ? e.EndDate.Value.Year : (int?)null
                        })
                        .SingleOrDefaultAsync();

                    if (experience == null)
                    {
                        return Results.NotFound();
                    }

                    return Results.Ok(experience);
                }
                catch (Exception)
                {
                    // For production level, use the ex message to log it.

                    return Results.Json(new { message = "Something went unexpectedly wrong, please try again later." }, statusCode: 500);
                }
            }).WithName("GetExperienceById"); 

            // Post/add a new experience
            app.MapPost("/experience", async (RestApiLabbDbContext context, ExperienceCreateDto newExperience) =>
            {
                if (newExperience == null)
                {
                    return Results.BadRequest(new { message = "Invalid experience data." });
                }

                try
                {
                    // Validate incoming data
                    var validationContext = new ValidationContext(newExperience);
                    var validationResult = new List<ValidationResult>();
                    bool isValid = Validator.TryValidateObject(newExperience, validationContext, validationResult, true);

                    if (!isValid)
                    {
                        return Results.BadRequest(validationResult.Select(v => v.ErrorMessage));
                    }

                    var person = await context.Experiences.FirstOrDefaultAsync(e => e.PersonId_FK == newExperience.PersonId);
                    if (person == null)
                    {
                        return Results.BadRequest(new { message = "Person not found." });
                    }

                    var experience = new Experience
                    {
                        Company = newExperience.Company,
                        JobTitle = newExperience.JobTitle,
                        Description = newExperience.Description,
                        StartDate = newExperience.StartDate,
                        EndDate = newExperience.EndDate,
                        PersonId_FK = newExperience.PersonId
                    };

                    context.Experiences.Add(experience);
                    await context.SaveChangesAsync();

                    return Results.Created($"/experience/{experience.ExperienceId}", new { message = "Experience created successfully." });
                }
                catch (Exception)
                {
                    return Results.Json(new { message = "Something went unexpectedly wrong, please try again later." }, statusCode: 500);
                }
            });

            // Put/Change existing info
            app.MapPut("/experience/{id}", async (RestApiLabbDbContext context, ExperiencePutDto experienceDto, int id) =>
            {
                if (id <= 0)
                {
                    return Results.BadRequest(new { message = "Invalid experience ID." });
                }

                try
                {
                    var experience = await context.Experiences
                    .FirstOrDefaultAsync(e => e.ExperienceId == id);

                    if (experience == null)
                    {
                        return Results.NotFound(new { message = "Experience not found." });
                    }

                    // Validate incoming data
                    var validationContext = new ValidationContext(experienceDto);
                    var validationResult = new List<ValidationResult>();
                    bool isValid = Validator.TryValidateObject(experienceDto, validationContext, validationResult, true);

                    if (!isValid)
                    {
                        return Results.BadRequest(validationResult.Select(v => v.ErrorMessage));
                    }

                    experience.Company = experienceDto.Company;
                    experience.JobTitle = experienceDto.JobTitle;
                    experience.Description = experienceDto.Description;
                    experience.StartDate = experienceDto.StartDate;
                    experience.EndDate = experienceDto.EndDate;

                    await context.SaveChangesAsync();

                    return Results.Ok(new { message = "Experience updated successfully." });
                }
                catch (Exception)
                {
                    return Results.Json(new { message = "Something went unexpectedly wrong, please try again later." }, statusCode: 500);
                }
            });

            // Patch/Change some info
            app.MapPatch("/experience/{id}", async (RestApiLabbDbContext context, ExperiencePatchDto experienceDto, int id) =>
            {
                if (id <= 0)
                {
                    return Results.BadRequest(new { message = "Invalid experience ID." });
                }

                try
                {
                    var experience = await context.Experiences
                    .FirstOrDefaultAsync(e => e.ExperienceId == id);

                    if (experience == null)
                    {
                        return Results.NotFound(new { message = "Experience not found." });
                    }

                    // Apply changes only for non-null fields
                    experience.PersonId_FK = experienceDto.PersonId;

                    if (experienceDto.Company != null)
                        experience.Company = experienceDto.Company;

                    if (experienceDto.JobTitle != null)
                        experience.JobTitle = experienceDto.JobTitle;

                    if (experienceDto.Description != null)
                        experience.Description = experienceDto.Description;

                    if (experienceDto.StartDate.HasValue)
                        experience.StartDate = experienceDto.StartDate.Value;

                    if (experienceDto.EndDate.HasValue)
                        experience.EndDate = experienceDto.EndDate.Value;

                    await context.SaveChangesAsync();

                    return Results.AcceptedAtRoute("GetExperienceById", new { id = experience.PersonId_FK }, experience);
                }
                catch (Exception)
                {
                    return Results.Json(new { message = "Something went unexpectedly wrong, please try again later." }, statusCode: 500);
                }
            });

            // Delete a record
            app.MapDelete("/experience/{id}", async (RestApiLabbDbContext context, int id) =>
            {
                if (id <= 0)
                {
                    return Results.BadRequest(new { message = "Invalid experience ID." });
                }

                try
                {
                    var experience = await context.Experiences
                    .FirstOrDefaultAsync(e => e.ExperienceId == id);

                    if (experience == null)
                    {
                        return Results.NotFound(new { message = "Experience not found." });
                    }

                    context.Experiences.Remove(experience);
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
