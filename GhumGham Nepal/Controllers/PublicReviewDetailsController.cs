using GhumGham_Nepal.Services;
using GhumGhamNepal.Core.ApplicationDbContext;
using GhumGhamNepal.Core.Models.DbEntity;
using GhumGhamNepal.Core.Services;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace GhumGham_Nepal.Controllers
{
    public class PublicReviewDetailsController : Controller
    {
        #region ctor

        private readonly ProjectContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IHttpContextService _httpContext;

        public PublicReviewDetailsController(ProjectContext context, IWebHostEnvironment webHostEnvironment, IHttpContextService httpContext)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
            _httpContext = httpContext;
        }
        #endregion

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddReview(int PlaceId, string ReviewerName, string Role, string ReviewText, int Rating)
        {
            var userId = _httpContext.User.UserId;

            // Create a new review object and populate it
            var newReview = new PublicReviewDetails
            {
                PlaceId = PlaceId,
                ReviewerName = ReviewerName,
                Rating = Rating,
                Comment = ReviewText,
                UserRefId = userId
            };

            var resp = await AddReviewAsync(newReview).ConfigureAwait(false);
            TempData["Message"] = JsonConvert.SerializeObject(resp);

            // Redirect back to the details page
            return RedirectToAction("Details", "Places", new { id = PlaceId });
        }

        private async Task<ServiceResult> AddReviewAsync(PublicReviewDetails entity)
        {
            try
            {
                await _context.PublicReviewDetails.AddAsync(entity);
                await _context.SaveChangesAsync();
                return ServiceResult.Success("Review added successfully!");
            }
            catch (Exception)
            {
                return ServiceResult.Fail("Error while adding review!");
            }
        }

    }
}
