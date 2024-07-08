using Microsoft.EntityFrameworkCore;
using BoostLingo.Core;

namespace BoostLingo.Repository
{
    public class PersonContext : DbContext
    {
        public PersonContext(DbContextOptions<PersonContext> options) : base(options) { }

        public DbSet<Person> Persons { get; set; }

        public DbSet<PersonBio> PersonBios { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Person>()
                .HasKey(p => p.PersonId);

            modelBuilder.Entity<PersonBio>()
                .HasKey(b => b.PersonId);


            modelBuilder.Entity<Person>()
                .HasIndex(p => p.UniqueId)
                .IsUnique();

            modelBuilder.Entity<PersonBio>()
                .HasOne(b => b.Person)
                .WithOne(p => p.PersonBio)
                .HasForeignKey<PersonBio>(b => b.PersonId);
        }


    }
}
