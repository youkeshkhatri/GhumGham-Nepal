using GhumGhamNepal.Core.ApplicationDbContext;
using GhumGhamNepal.Core.Models.DbEntity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GhumGham_Nepal.Controllers
{
    public class LoginController : Controller
    {
        private readonly ProjectContext _context;

        public LoginController(ProjectContext context)
        {
            _context = context;
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(Login registration)
        {
            var userExist = _context.Registrations.Any
            (x => x.Username == registration.Username && x.Password == registration.Password);

            if (userExist)
            {
                registration.LoginDate = DateTime.Now;
                registration.LoginStatus = true;
                _context.Add(registration);
                _context.SaveChanges();
                return RedirectToAction("Index", "Registrations");
            }
            return View(nameof(Login));
        }


      

        [HttpPost]
        public IActionResult Logout(int id)
        {
            var user = GetByIdAsync(id).Result;
            user.LogoutDate = DateTime.Now;
            user.LoginStatus = false;
            _context.Update(user);
            _context.SaveChanges();
            return RedirectToAction("Index", "Home");
        }



        public async Task<Login> GetByIdAsync(int id)
        {
            return await _context.Set<Login>().FindAsync(id).ConfigureAwait(false);
        }
    }
}
