using CaddyShackMVC.DataAccess;
using CaddyShackMVC.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CaddyShackMVC.Controllers
{
    public class GolfBagsController : Controller
    {
        private readonly CaddyShackContext _context;

        public GolfBagsController(CaddyShackContext context)
        {
            _context = context;
        }

        [Route("/golfbags")]
        public IActionResult Index()
        {
            var bags = _context.GolfBags.ToList();
            return View(bags);
        }

        [Route("/golfbags/{id:int}")]
        public IActionResult Show(int id)
        {
            var bag = _context.GolfBags.Include(b => b.Clubs).Where(b => b.Id == id).Single();
            return View(bag);
        }

        [HttpPost]
        [Route("/golfbags/delete/{id:int}")]
        public IActionResult Delete(int id)
        {
            var bag = _context.GolfBags.Find(id);
            _context.GolfBags.Remove(bag);
            _context.SaveChanges();
            return Redirect("/golfbags");
        }

        [Route("/golfbags/new")]
        public IActionResult New()
        {
            return View();
        }

        [HttpPost]
        [Route("/golfbags")]
        public IActionResult Create(GolfBag bag)
        {
            _context.GolfBags.Add(bag);
            _context.SaveChanges();
            return Redirect($"/golfbags/{bag.Id}");
        }

        [Route("/golfbags/{id:int}/edit")]
        public IActionResult Edit(int id)
        {
            var bag = _context.GolfBags.Include(b => b.Clubs).Where(b => b.Id == id).Single();
            return View(bag);
        }

        [HttpPost]
        [Route("/golfbags/{id:int}")]
        public IActionResult Update(int id, Club club)
        {
            var bag = _context.GolfBags.Include(b => b.Clubs).Where(b => b.Id == id).Single();
            bag.Clubs.Add(club);
            _context.GolfBags.Update(bag);
            _context.SaveChanges();
            return Redirect($"/golfbags/{id}");
        }
    }
}
