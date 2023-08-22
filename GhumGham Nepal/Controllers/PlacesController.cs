using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GhumGham_Nepal.DTO;
using GhumGham_Nepal.Factory;
using Microsoft.AspNetCore.Authorization;
using GhumGham_Nepal.Repository;
using GhumGhamNepal.Core.Models.DbEntity;
using GhumGhamNepal.Core.Services.AttachmentService;

namespace GhumGham_Nepal.Controllers
{
    //[Route("districts")]
    public class PlacesController : Controller
    {
        #region ctor & prop

        private readonly IRepository<Place> _placesRepository;
        private readonly ICommonAttachmentService _commonAttachmentService;

        public PlacesController(IRepository<Place> placeRepository, ICommonAttachmentService attachmentService)
        {
            _placesRepository = placeRepository;
            _commonAttachmentService = attachmentService;
        }
        #endregion

        #region Read

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var data = await _placesRepository.GetAllAsync().ConfigureAwait(false);
            return View(data.ToList().ToDTO());
        }

        [HttpGet]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _placesRepository.Table == null)
            {
                return NotFound();
            }

            var place = await _placesRepository.Table
                .FirstOrDefaultAsync(m => m.PlaceId == id)
                .ConfigureAwait(false);

            if (place == null)
            {
                return NotFound();
            }

            return View(place.ToDTO());
        }
        #endregion

        #region Create

        [HttpGet]
        [Authorize]
        public IActionResult Create()
        {
            return View();
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PlaceDTO dto)
        {
            if (ModelState.IsValid)
            {
                List<IFormFile> picture = new List<IFormFile>();
                if (dto.File != null)
                {
                    string folder = "/Images/Places/Thumbnail/";
                    dto.ThumbnailUrl = folder;

                    picture.Add(dto.File);
                }

                var attachmentUploadResp = _commonAttachmentService.UploadCommonAttachment(picture
                   .Select(x => new FileUploadRequest(x.ReadBytes(), x.ContentType, x.FileName)).ToList());

                await _placesRepository.AddAsync(dto.ToEntity(attachmentUploadResp.Data)).ConfigureAwait(false);
                await _placesRepository.CommitAsync().ConfigureAwait(false);
                return RedirectToAction(nameof(Index));
            }
            return View(dto);
        }
        #endregion

        #region Edit

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _placesRepository.Table == null)
            {
                return NotFound();
            }

            var place = await _placesRepository.GetByIdAsync(id.Value).ConfigureAwait(false);
            if (place == null)
            {
                return NotFound();
            }
            var res = place.ToDTO();
            return View(res);
        }

        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, PlaceDTO dto)
        {
            if (id != dto.Id)
                return NotFound();

            if (dto.ThumbnailUrl != null)
            {
                string[] url = dto.ThumbnailUrl.Split('/');

                string empty = url[0];  
                string image = url[1];  
                string place = url[2]; 
                string thumbnail = url[3]; 
                string serverFileName = url[4]; 

                List<string> files = new() { serverFileName };
                _commonAttachmentService.DeleteFile(files);
            }

            List<IFormFile> picture = new();
            if (dto.File != null)
            {
                string folder = "/Images/Places/Thumbnail/";
                dto.ThumbnailUrl = folder;

                picture.Add(dto.File);
            }

            var attachmentUploadResp = _commonAttachmentService.UploadCommonAttachment(picture
               .Select(x => new FileUploadRequest(x.ReadBytes(), x.ContentType, x.FileName)).ToList());

            if (ModelState.IsValid)
            {
                try
                {
                    _placesRepository.Update(dto.ToEntity(attachmentUploadResp.Data));
                    await _placesRepository.CommitAsync().ConfigureAwait(false);
                }
                catch (Exception)
                {
                    throw;
                }

                return RedirectToAction(nameof(Index));
            }

            return View(dto);
        }

        #endregion

        #region Delete

        //[Authorize]
        //[HttpGet]
        //public async Task<IActionResult> Delete(int? id)
        //{
        //    if (id == null || _placesRepository.Table == null)
        //    {
        //        return NotFound();
        //    }

        //    var place = await _placesRepository.Table
        //        .FirstOrDefaultAsync(m => m.PlaceId == id);
        //    if (place == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(place);
        //}

        [Authorize]
        [HttpPost, ActionName("Delete")]
        [HttpDelete]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_placesRepository.Table == null)
            {
                return Problem("Entity set 'ProjectContext.Places'  is null.");
            }
            var place = await _placesRepository.GetByIdAsync(id).ConfigureAwait(false);
            if (place != null)
            {

                if (place.ServerFileName != null)
                {
                    List<string> files = new() { place.ServerFileName };
                    _commonAttachmentService.DeleteFile(files);
                }

                _placesRepository.Delete(place);
                await _placesRepository.CommitAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        #endregion

    }
}
