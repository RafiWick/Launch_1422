using CaddyShackMVC.DataAccess;
using CaddyShackMVC.Models;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestPlatform.TestHost;

namespace CaddyShackTests
{
    public class GolfBagCrudTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;

        public GolfBagCrudTests(WebApplicationFactory<Program> factory)
        {
            _factory = factory;
        }

        private CaddyShackContext GetDbContext()
        {
            var optionsBuilder = new DbContextOptionsBuilder<CaddyShackContext>();
            optionsBuilder.UseInMemoryDatabase("TestDatabase");

            var context = new CaddyShackContext(optionsBuilder.Options);
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();

            return context;
        }
        [Fact]
        public async Task Index_ShowsIdPlayerCapacityForAllBags()
        {
            var client = _factory.CreateClient();
            var context = GetDbContext();
            var bag1 = new GolfBag { Player = "John", Capacity = 4 };
            var bag2 = new GolfBag { Player = "Mark", Capacity = 7 };
            var bag3 = new GolfBag { Player = "Lucy", Capacity = 3 };
            context.GolfBags.Add(bag1);
            context.GolfBags.Add(bag2);
            context.GolfBags.Add(bag3);
            context.SaveChanges();

            var response = await client.GetAsync("/golfbags");
            var html = await response.Content.ReadAsStringAsync();

            response.EnsureSuccessStatusCode();
            Assert.Contains(bag1.Player, html);
            Assert.Contains(bag2.Player, html);
            Assert.Contains(bag3.Player, html);
            Assert.Contains(bag1.Capacity.ToString(), html);
            Assert.Contains(bag2.Capacity.ToString(), html);
            Assert.Contains(bag3.Capacity.ToString(), html);
            Assert.Contains(bag1.Id.ToString(), html);
            Assert.Contains(bag2.Id.ToString(), html);
            Assert.Contains(bag3.Id.ToString(), html);
        }
        [Fact]
        public async Task Show_ShowsIdPlayerCapacityandClubsInOneBag()
        {
            var client = _factory.CreateClient();
            var context = GetDbContext();
            var bag1 = new GolfBag { Player = "John", Capacity = 4 };
            var bag2 = new GolfBag { Player = "Mark", Capacity = 7 };
            var bag3 = new GolfBag { Player = "Lucy", Capacity = 3 };
            context.GolfBags.Add(bag1);
            context.GolfBags.Add(bag2);
            context.GolfBags.Add(bag3);
            context.SaveChanges();

            var response = await client.GetAsync($"/golfbags/{bag1.Id}");
            var html = await response.Content.ReadAsStringAsync();

            response.EnsureSuccessStatusCode();
            Assert.Contains(bag1.Player, html);
            Assert.Contains(bag1.Capacity.ToString(), html);
            Assert.Contains(bag1.Id.ToString(), html);
            Assert.DoesNotContain(bag2.Player, html);
            Assert.DoesNotContain(bag3.Player, html);
            Assert.DoesNotContain(bag2.Capacity.ToString(), html);
            Assert.DoesNotContain(bag3.Capacity.ToString(), html);
            Assert.DoesNotContain(bag2.Id.ToString(), html);
            Assert.DoesNotContain(bag3.Id.ToString(), html);
        }
        [Fact]
        public async Task Index_ContainsDeleteButtonForEachBag()
        {

            var client = _factory.CreateClient();
            var context = GetDbContext();
            var bag1 = new GolfBag { Player = "John", Capacity = 4 };
            var bag2 = new GolfBag { Player = "Mark", Capacity = 7 };
            var bag3 = new GolfBag { Player = "Lucy", Capacity = 3 };
            context.GolfBags.Add(bag1);
            context.GolfBags.Add(bag2);
            context.GolfBags.Add(bag3);
            context.SaveChanges();

            var response = await client.GetAsync("/golfbags");
            var html = await response.Content.ReadAsStringAsync();

            response.EnsureSuccessStatusCode();
            Assert.Contains($"<form method=\"post\" action=\"/golfbags/delete/{bag1.Id}\">", html);
            Assert.Contains($"<form method=\"post\" action=\"/golfbags/delete/{bag2.Id}\">", html);
            Assert.Contains($"<form method=\"post\" action=\"/golfbags/delete/{bag3.Id}\">", html);

        }
        [Fact]
        public async Task Delete_RedirectsToIndexWithoutDeletedBag()
        {
            var client = _factory.CreateClient();
            var context = GetDbContext();
            var bag1 = new GolfBag { Player = "John", Capacity = 4 };
            var bag2 = new GolfBag { Player = "Mark", Capacity = 7 };
            var bag3 = new GolfBag { Player = "Lucy", Capacity = 3 };
            context.GolfBags.Add(bag1);
            context.GolfBags.Add(bag2);
            context.GolfBags.Add(bag3);
            context.SaveChanges();

            var formdata = new Dictionary<string, string>
            {

            };

            var response = await client.PostAsync($"/golfbags/delete/{bag1.Id}", new FormUrlEncodedContent(formdata));
            var html = await response.Content.ReadAsStringAsync();

            response.EnsureSuccessStatusCode();
            Assert.DoesNotContain(bag1.Player, html);
            Assert.DoesNotContain(bag1.Capacity.ToString(), html);
            Assert.DoesNotContain(bag1.Id.ToString(), html);

            Assert.Contains(bag2.Player, html);
            Assert.Contains(bag3.Player, html);
            Assert.Contains(bag2.Capacity.ToString(), html);
            Assert.Contains(bag3.Capacity.ToString(), html);
            Assert.Contains(bag2.Id.ToString(), html);
            Assert.Contains(bag3.Id.ToString(), html);
        }
        [Fact]
        public async Task New_ReturnsViewWithFormForNewBag()
        {
            var client = _factory.CreateClient();
            var response = await client.GetAsync("/golfbags/new");
            var html = await response.Content.ReadAsStringAsync();
            Assert.Contains($"<form method=\"post\" action=\"/golfbags\">", html);
            Assert.Contains("input", html);

        }
        [Fact]
        public async Task Create_RedirectsToNewBagsShowPage()
        {
            var client = _factory.CreateClient();
            var context = GetDbContext();
            var bag1 = new GolfBag { Player = "John", Capacity = 9 };
            var bag2 = new GolfBag { Player = "Mark", Capacity = 7 };
            var bag3 = new GolfBag { Player = "Lucy", Capacity = 3 };
            context.GolfBags.Add(bag1);
            context.GolfBags.Add(bag2);
            context.GolfBags.Add(bag3);
            context.SaveChanges();

            var formdata = new Dictionary<string, string>
                {
                    {"Player", "Greg" },
                    {"Capacity", "8" }
                };

            var response = await client.PostAsync($"/golfbags", new FormUrlEncodedContent(formdata));
            var html = await response.Content.ReadAsStringAsync();

            response.EnsureSuccessStatusCode();
            Assert.DoesNotContain(bag1.Player, html);
            Assert.DoesNotContain(bag1.Capacity.ToString(), html);
            Assert.DoesNotContain(bag1.Id.ToString(), html);
            Assert.DoesNotContain(bag2.Player, html);
            Assert.DoesNotContain(bag3.Player, html);
            Assert.DoesNotContain(bag2.Capacity.ToString(), html);
            Assert.DoesNotContain(bag3.Capacity.ToString(), html);
            Assert.DoesNotContain(bag2.Id.ToString(), html);
            Assert.DoesNotContain(bag3.Id.ToString(), html);
            Assert.Contains("Greg", html);
            Assert.Contains("8", html);
            Assert.Contains("4", html);
        }
        [Fact]
        public async Task Edit_ShowsFormToAddNewClubToBag()
        {
            var client = _factory.CreateClient();
            var context = GetDbContext();
            var bag1 = new GolfBag { Player = "John", Capacity = 9 };
            var bag2 = new GolfBag { Player = "Mark", Capacity = 7 };
            var bag3 = new GolfBag { Player = "Lucy", Capacity = 3 };
            context.GolfBags.Add(bag1);
            context.GolfBags.Add(bag2);
            context.GolfBags.Add(bag3);
            context.SaveChanges();
            var response = await client.GetAsync($"/golfbags/edit/{bag1.Id}");
            var html = await response.Content.ReadAsStringAsync();
            Assert.Contains($"<form method=\"post\" action=\"/golfbags/{bag1.Id}\">", html);
            Assert.Contains("input", html);
        }
        [Fact]
        public async Task Update_AddsNewClubToBagAndShowsItOnBagShowPage()
        {
            var client = _factory.CreateClient();
            var context = GetDbContext();
            var bag1 = new GolfBag { Player = "John", Capacity = 9 };
            var bag2 = new GolfBag { Player = "Mark", Capacity = 7 };
            var bag3 = new GolfBag { Player = "Lucy", Capacity = 3 };
            context.GolfBags.Add(bag1);
            context.GolfBags.Add(bag2);
            context.GolfBags.Add(bag3);
            context.SaveChanges();

            var formdata = new Dictionary<string, string>
                {
                    {"Name", "five iron" },
                };

            var response = await client.PostAsync($"/golfbags/{bag1.Id}", new FormUrlEncodedContent(formdata));
            var html = await response.Content.ReadAsStringAsync();

            response.EnsureSuccessStatusCode();
            Assert.Contains(bag1.Player, html);
            Assert.Contains(bag1.Capacity.ToString(), html);
            Assert.Contains(bag1.Id.ToString(), html);
            Assert.DoesNotContain(bag2.Player, html);
            Assert.DoesNotContain(bag3.Player, html);
            Assert.DoesNotContain(bag2.Capacity.ToString(), html);
            Assert.DoesNotContain(bag3.Capacity.ToString(), html);
            Assert.DoesNotContain(bag2.Id.ToString(), html);
            Assert.DoesNotContain(bag3.Id.ToString(), html);

            Assert.Contains("five iron", html);
        }
    }
}