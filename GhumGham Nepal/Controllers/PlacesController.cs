using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using GhumGham_Nepal.Models;
using GhumGham_Nepal.DTO;
using GhumGham_Nepal.Factory;
using Microsoft.AspNetCore.Authorization;

namespace GhumGham_Nepal.Controllers
{
    public class PlacesController : Controller
    {
        #region ctor

        private readonly ProjectContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public PlacesController(ProjectContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;

        }
        #endregion

        #region Read
        // GET: Places
        public async Task<IActionResult> Index()
        {
            var data = await _context.Places.ToListAsync();
            return View(data.ToDTO());
        }

        // GET: Places/Details/5
        public async Task<IActionResult> Details(int? id)
        { 
            if (id == null || _context.Places == null)
            {
                return NotFound();
            }

            var place = await _context.Places
                .FirstOrDefaultAsync(m => m.PlaceId == id);
            if (place == null)
            {
                return NotFound();
            }

            return View(place.ToDTO());
        }
        #endregion

        #region Create

        // GET: Places/Create
        [Authorize]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Places/Create
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PlaceDTO place)
        { 
            if (ModelState.IsValid)
            {
                if (place.ThumbnailFile != null)
                {
                    string folder = "Images/Places/Thumbnail/";
                    folder += Guid.NewGuid().ToString() + "_" + place.ThumbnailFile.FileName;

                    place.ThumbnailUrl = "/" + folder;

                    string serverFolder = Path.Combine(_webHostEnvironment.WebRootPath, folder);

                    await place.ThumbnailFile.CopyToAsync(new FileStream(serverFolder, FileMode.Create));
                }

                _context.Add(place.ToEntity());
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(place);
        }
        #endregion

        #region Edit

        [Authorize]
        // GET: Places/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Places == null)
            {
                return NotFound();
            }

            var place = await _context.Places.FindAsync(id);
            if (place == null)
            {
                return NotFound();
            }
            var res = place.ToDTO();
            return View(res);
        }

        // POST: Places/Edit/5
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, PlaceDTO place)
        {
            if (id != place.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(place.ToEntity());
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PlaceExists(place.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(place);
        }

        private bool PlaceExists(int id)
        {
            return (_context.Places?.Any(e => e.PlaceId == id)).GetValueOrDefault();
        }
        #endregion

        #region Delete


        // GET: Places/Delete/5
        [Authorize]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Places == null)
            {
                return NotFound();
            }

            var place = await _context.Places
                .FirstOrDefaultAsync(m => m.PlaceId == id);
            if (place == null)
            {
                return NotFound();
            }

            return View(place);
        }

        // POST: Places/Delete/5
        [Authorize]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Places == null)
            {
                return Problem("Entity set 'ProjectContext.Places'  is null.");
            }
            var place = await _context.Places.FindAsync(id);
            if (place != null)
            {
                _context.Places.Remove(place);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        #endregion
        
    }
}
