﻿namespace Vote.Web.Data
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Entities;
    using Helpers;
    using Microsoft.AspNetCore.Identity;

    public class SeedDb
    {
        private readonly DataContext context;
        private readonly IUserHelper userHelper;

        public SeedDb(DataContext context, IUserHelper userHelper)
        {
            this.context = context;
            this.userHelper = userHelper;
        }

        public async Task SeedAsync()
        {
            await this.context.Database.EnsureCreatedAsync();

            await this.CheckRolesAsync();

            if (!this.context.Countries.Any())
            {
                await this.AddCountriesAndCitiesAsync();
            }

            await this.CheckUser("user01@gmail.com", "user01", "user01", "Voter");
            await this.CheckUser("user02@gmail.com", "user02", "user02", "Voter");
            await this.CheckUser("user03@gmail.com", "user03", "user03", "Voter");
            await this.CheckUser("user04@gmail.com", "user04", "user04", "Voter");
            await this.CheckUser("user05@gmail.com", "user05", "user05", "Voter");
            await this.CheckUser("user06@gmail.com", "user06", "user06", "Voter");
            await this.CheckUser("user07@gmail.com", "user07", "user07", "Voter");
            await this.CheckUser("user08@gmail.com", "user08", "user08", "Voter");
            await this.CheckUser("user09@gmail.com", "user09", "user09", "Voter");
            await this.CheckUser("user10@gmail.com", "user10", "user10", "Voter");
            await this.CheckUser("user11@gmail.com", "user11", "user11", "Voter");
            await this.CheckUser("user12@gmail.com", "user12", "user12", "Voter");
            await this.CheckUser("user13@gmail.com", "user13", "user13", "Voter");
            await this.CheckUser("user14@gmail.com", "user14", "user14", "Voter");
            await this.CheckUser("user15@gmail.com", "user15", "user15", "Voter");
            await this.CheckUser("user16@gmail.com", "user16", "user16", "Voter");
            await this.CheckUser("user17@gmail.com", "user17", "user17", "Voter");
            await this.CheckUser("user18@gmail.com", "user18", "user18", "Voter");
            await this.CheckUser("user19@gmail.com", "user19", "user19", "Voter");
            await this.CheckUser("user20@gmail.com", "user20", "user20", "Voter");
            await this.CheckUser("camilocadavid95@gmail.com", "Camilo", "Hernández", "Admin");

            if (!this.context.VoteEvents.Any())
            {
                this.AddVoteEvent("Legalización de la marihuana", "Se trataran diferentes temas desde el uso medicinal hasta el uso recreativo.");
                this.AddVoteEvent("Eleccion del director TI", "Dado que la proxima semana se retira Santacruz, se requiere un nuevo director.");
                await this.context.SaveChangesAsync();
            }

            if(!this.context.Candidates.Any())
            {
                this.AddCandidate("Juan Carmona", "Los niños con sindrome de down necesitan la marihuana medicinal", 1);
                this.AddCandidate("Sandra Cadavid", "Legalizar la marihuana es lo peor que le puede pasar a nuestros jovenes", 1);
                await this.context.SaveChangesAsync();
            }
        }

        private async Task<User> CheckUser(string userName, string firstName, string lastName, string role)
        {
            // Add user
            var user = await this.userHelper.GetUserByEmailAsync(userName);
            if (user == null)
            {
                user = await this.AddUser(userName, firstName, lastName, role);
            }

            var isInRole = await this.userHelper.IsUserInRoleAsync(user, role);
            if (!isInRole)
            {
                await this.userHelper.AddUserToRoleAsync(user, role);
            }

            return user;
        }

        private async Task<User> AddUser(string userName, string firstName, string lastName, string role)
        {
            var user = new User
            {
                FirstName = firstName,
                LastName = lastName,
                Occupation = "Engineer",
                Stratum = 2,
                Gender = 1,
                Email = userName,
                UserName = userName,
                CityId = this.context.Countries.FirstOrDefault().Cities.FirstOrDefault().Id,
                City = this.context.Countries.FirstOrDefault().Cities.FirstOrDefault()
            };

            var result = await this.userHelper.AddUserAsync(user, "123456");
            if (result != IdentityResult.Success)
            {
                throw new InvalidOperationException("Could not create the user in seeder");
            }

            await this.userHelper.AddUserToRoleAsync(user, role);
            var token = await this.userHelper.GenerateEmailConfirmationTokenAsync(user);
            await this.userHelper.ConfirmEmailAsync(user, token);
            return user;
        }

        private async Task AddCountriesAndCitiesAsync()
        {
            this.AddCountry("Colombia", new string[] { "Medellín", "Bogota", "Calí", "Barranquilla", "Bucaramanga", "Cartagena", "Pereira" });
            this.AddCountry("Argentina", new string[] { "Córdoba", "Buenos Aires", "Rosario", "Tandil", "Salta", "Mendoza" });
            this.AddCountry("Estados Unidos", new string[] { "New York", "Los Ángeles", "Chicago", "Washington", "San Francisco", "Miami", "Boston" });
            this.AddCountry("Ecuador", new string[] { "Quito", "Guayaquil", "Ambato", "Manta", "Loja", "Santo" });
            this.AddCountry("Peru", new string[] { "Lima", "Arequipa", "Cusco", "Trujillo", "Chiclayo", "Iquitos" });
            this.AddCountry("Chile", new string[] { "Santiago", "Valdivia", "Concepcion", "Puerto Montt", "Temucos", "La Sirena" });
            this.AddCountry("Uruguay", new string[] { "Montevideo", "Punta del Este", "Colonia del Sacramento", "Las Piedras" });
            this.AddCountry("Bolivia", new string[] { "La Paz", "Sucre", "Potosi", "Cochabamba" });
            this.AddCountry("Venezuela", new string[] { "Caracas", "Valencia", "Maracaibo", "Ciudad Bolivar", "Maracay", "Barquisimeto" });
            this.AddCountry("Paraguay", new string[] { "Asunción", "Ciudad del Este", "Encarnación", "San  Lorenzo", "Luque", "Areguá" });
            this.AddCountry("Brasil", new string[] { "Rio de Janeiro", "São Paulo", "Salvador", "Porto Alegre", "Curitiba", "Recife", "Belo Horizonte", "Fortaleza" });
            await this.context.SaveChangesAsync();
        }

        private void AddCountry(string country, string[] cities)
        {
            var theCities = cities.Select(c => new City { Name = c }).ToList();
            this.context.Countries.Add(new Country
            {
                Cities = theCities,
                Name = country
            });
        }

        private async Task CheckRolesAsync()
        {
            await this.userHelper.CheckRoleAsync("Admin");
            await this.userHelper.CheckRoleAsync("Voter");
        }

        private void AddVoteEvent(string name, string description)
        {
            this.context.VoteEvents.Add(new VoteEvent
            {
                Name = name,
                Description = description,
                ImageUrl = $"~/images/VoteEvents/{name.Replace(" ", "")}.jpg",
                StartDate = new DateTime(2019, 04, 18),
                EndDate = new DateTime(2019, 05, 18)
            });
        }

        private void AddCandidate(string name, string proposal, int voteEventId)
        {
            this.context.Candidates.Add(new Candidate
            {
                Name = name,
                Proposal = proposal,
                ImageUrl = $"~/images/Candidates/{name.Replace(" ", "")}.jpg",
                VoteEventId = voteEventId
            });
        }
    }
}