using GhumGhamNepal.Core.ApplicationDbContext;
using GhumGhamNepal.Core.Models.DbEntity;
using Microsoft.AspNetCore.Mvc;

namespace GhumGham_Nepal.Controllers
{
    public class PublicReviewDetailsController : Controller
    {
        #region ctor

        private readonly ProjectContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public PublicReviewDetailsController(ProjectContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;

        }
        #endregion

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddReview(int PlaceId, string ReviewerName, string Role, string ReviewText, int Rating)
        {
            // Create a new review object and populate it
            var newReview = new PublicReviewDetails
            {
                PlaceId = PlaceId,
                ReviewerName = ReviewerName,
                Rating = Rating,
                Comment = ReviewText
            };

            // Add the new review to the place's Reviews collection
            //place.Reviews.Add(newReview);
            await _context.PublicReviewDetails.AddAsync(newReview);
            await _context.SaveChangesAsync();

            // You may want to save the place data to your data source here...

            // Redirect back to the details page
            return RedirectToAction("Details", "Places", new { id = PlaceId });
        }

    }
}
