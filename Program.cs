using Microsoft.EntityFrameworkCore;
using System.Net.Http.Headers;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(5001, listenOptions =>
    {
        listenOptions.UseHttps();
    });
});

builder.Services.AddDbContext<MovieSTDb>(opt => opt.UseInMemoryDatabase("MovieDatabase"));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();
builder.Configuration.AddJsonFile("appsettings.json");

var app = builder.Build();


app.MapGet("/movies", async (MovieSTDb db) =>
{
    var movies = await db.Movies.ToListAsync();
    return Results.Ok(movies);
});

app.MapGet("/movies/genre/{genre}", async (string genre, MovieSTDb db) =>
{
    var movies = await db.Movies
        .Where(m => m.Genre.Equals(genre, StringComparison.OrdinalIgnoreCase))
        .ToListAsync();

    if (movies.Any())
    {
        return Results.Ok(movies);
    }
    else
    {
        return Results.NotFound($"Movies with genre '{genre}' not found.");
    }
});

app.MapGet("/movies/{id}", async (int id, MovieSTDb db) =>
{
    var movie = await db.Movies.FindAsync(id);
    if (movie != null)
    {
        return Results.Ok(movie);
    }
    else
    {
        return Results.NotFound($"Movie with ID '{id}' not found.");
    }
});

app.MapPost("/movies", async (MovieSoundTrack movie, MovieSTDb db) =>
{
    db.Movies.Add(movie);
    await db.SaveChangesAsync();

    return Results.Created($"/movies/{movie.Id}", movie);
});

app.MapPut("/movies/{id}", async (int id, MovieSoundTrack inputMovie, MovieSTDb db) =>
{
    var movie = await db.Movies.FindAsync(id);
    if (movie == null)
    {
        return Results.NotFound();
    }

    movie.Title = inputMovie.Title;
    movie.Genre = inputMovie.Genre;
    movie.SoundTrack = inputMovie.SoundTrack;

    await db.SaveChangesAsync();

    return Results.NoContent();
});

app.MapDelete("/movies/{id}", async (int id, MovieSTDb db) =>
{
    var movie = await db.Movies.FindAsync(id);
    if (movie == null)
    {
        return Results.NotFound();
    }

    db.Movies.Remove(movie);
    await db.SaveChangesAsync();

    return Results.NoContent();
});

app.Run();
