using Microsoft.EntityFrameworkCore;
using UserApi.DataLayer.Entity;

namespace UserApi.DataLayer
{
    public class SeedData
    {
        private readonly ModelBuilder modelBuilder;

        public SeedData(ModelBuilder modelBuilder)
        {
            this.modelBuilder = modelBuilder;
        }

        public void Seed()
        {
            modelBuilder.Entity<User>().HasData(
                   new User()
                   {
                       Id = 1,
                       FirstName = "Arko",
                       LastName = "Marić",
                       Email = "arko.maric@example.com",
                       IsActive = false
                   },
                   new User()
                   {
                       Id = 2,
                       FirstName = "Tango",
                       LastName = "Anić",
                       Email = "tango.anic@example.com",
                       IsActive = true

                   },
                   new User
                   {
                       Id = 3,
                       FirstName = "Lala",
                       LastName = "Petrović",
                       Email = "lala.petrovic@example.com",
                       IsActive = true
                   }
            );
        }
    }
}
