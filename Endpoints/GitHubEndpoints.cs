using RestApiLabb.DTOs.GitHubDTOs;
using RestApiLabb.Models;
using System.Text.Json;

namespace RestApiLabb.Endpoints
{
    public class GitHubEndpoints
    {
        private static readonly HttpClient httpClient = new();

        public static void RegisterEndpoints(WebApplication app)
        {
            app.MapGet("/person/github/{username}", async (string username) =>
            {
                if (string.IsNullOrEmpty(username))
                    return Results.BadRequest(new { message = "GitHub username is required" });

                try
                {
                    var requestMessage = new HttpRequestMessage(HttpMethod.Get, $"https://api.github.com/users/{username}/repos");

                    requestMessage.Headers.UserAgent.ParseAdd("RestApiLabb/1.0"); 

                    var response = await httpClient.SendAsync(requestMessage);

                    response.EnsureSuccessStatusCode();

                    var responseContent = await response.Content.ReadAsStringAsync();

                    var repositories = JsonSerializer.Deserialize<List<GitHubRepo>>(responseContent);

                    if (repositories == null || repositories.Count == 0)
                    {
                        return Results.NotFound(new { message = "No repositories found for this username, please try another one." });
                    }

                    var repoDtos = repositories.Select(repo => new GitHubRepoDto
                    {
                        RepositoryName = repo.Name,
                        Language = string.IsNullOrEmpty(repo.Language) ? "unknown" : repo.Language,
                        Description = string.IsNullOrEmpty(repo.Description) ? "missing" : repo.Description,
                        RepositoryLink = repo.HtmlUrl
                    }).ToList();

                    return Results.Ok(repoDtos);
                }
                catch (JsonException)
                {
                    return Results.BadRequest(new { message = "Error processing GitHub API response." });
                }
                catch (HttpRequestException)
                {
                    return Results.Json(new { message = "Error connecting to GitHub API." }, statusCode: 500);
                }
                catch (Exception)
                {
                    return Results.Json(new { message = "Error fetching data from GitHub. Please try again later." }, statusCode: 500);
                }
            });
        }
    }
}
