# TaskImel
# ImelTask
#ApiUser Projekt

Ovo je ASP.NET Core projekt koji koristi **Duende IdentityServer** sa **Entity Framework Core** za upravljanje korisnicima, konfiguracijom klijenata i tokenima. Projekt uključuje migracije za baze podataka, seed-ovanje podataka i frontend aplikaciju (React ili sličnu) koju je moguće pokrenuti lokalno.
Napomena:potrebno je startati sve tri projekta odjednom

## 📦 Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/en-us/download)
- [Node.js i npm](https://nodejs.org/)
- EF Core CLI (ako nije instaliran):
  ```bash
  dotnet tool install --global dotnet-ef


🚀 Pokretanje projekta
1. Kreiranje migracija za IdentityServer baze
    Developer Power Shell
 dotnet ef migrations add Grants -c PersistedGrantDbContext -o Migrations/PersistedGrantDb
 dotnet ef migrations add Configuration -c ConfigurationDbContext -o Migrations/ConfigurationDb 
dotnet ef migrations add InitialMigration -c ApplicationDbContext

3. Ažuriranje baza podataka
Developer Power Shell
dotnet ef database update -c PersistedGrantDbContext
dotnet ef database update -c ConfigurationDbContext
dotnet ef database update -c ApplicationDbContext
4. Kreiranje i ažuriranje aplikacione baze
Developer Power Shell
dotnet ef migrations add InitialMigration -c ApplicationDbContext
dotnet ef database update -c ApplicationDbContext

🖥️ Pokretanje frontend aplikacije
Za pokretanje React frontend aplikacije s HTTPS podrškom:

Developer Power Shell

$env:HTTPS="true"; npm start

Pass Login
alice Pass123$
