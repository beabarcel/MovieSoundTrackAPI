using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

class MovieSTDb : DbContext
{
    public MovieSTDb(DbContextOptions<MovieSTDb> options)
    : base(options) { }

    public DbSet<MovieSoundTrack> Movies => Set<MovieSoundTrack>();
}