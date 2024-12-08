using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ProyectoFinal_JorgeSayegh.Contexts;
using ProyectoFinal_JorgeSayegh.Models;
using System;
using System.IO;
using System.Linq;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;


namespace ProyectoFinal_JorgeSayegh.Controllers
{
    [Authorize]
    public class FeedController : Microsoft.AspNetCore.Mvc.Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<FeedController> _logger;

        public FeedController(ApplicationDbContext context, ILogger<FeedController> logger)
        {
            _context = context;
            _logger = logger;
        }

        public IActionResult Index()
        {
            var posts = _context.Posts
                .Include(p => p.User) // Explicitly load the User navigation property
                .Include(p => p.Comments) // Explicitly load the Comments navigation property
                .ThenInclude(c => c.User) // Load the User for each comment
                .OrderByDescending(p => p.CreatedAt)
                .ToList();
            _logger.LogInformation("Fetched {Count} posts from the database.", posts.Count);
            return View(posts);
        }

       /* public IActionResult Index()
        {
            var posts = _context.Posts
                .Include(p => p.User) // Explicitly load the User navigation property
                .OrderByDescending(p => p.CreatedAt)
                .ToList();
            _logger.LogInformation("Fetched {Count} posts from the database.", posts.Count);
            return View(posts);
        }*/

        [HttpGet]
        public IActionResult Create()
        {
            _logger.LogInformation("Accessing Feed Create GET.");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Post post, IFormFile Image)
        {
            _logger.LogInformation("Accessing Feed Create POST.");

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                _logger.LogWarning("User is not authenticated.");
                return Forbid();
            }

            // Assign CreatedBy programmatically
            post.CreatedBy = Guid.Parse(userId);

            // Handle file upload
            if (Image != null && Image.Length > 0)
            {
                _logger.LogInformation("Processing image upload.");
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                    _logger.LogInformation("Created uploads folder at {Path}.", uploadsFolder);
                }

                var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(Image.FileName);
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    Image.CopyTo(fileStream);
                }

                post.ImageUrl = "/uploads/" + uniqueFileName;
                _logger.LogInformation("Image uploaded successfully to {Path}.", post.ImageUrl);
            }

            // Validate the model state manually since User is now assigned
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("ModelState is invalid.");
                foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                {
                    _logger.LogWarning("Error: {ErrorMessage}", error.ErrorMessage);
                }
                return View(post);
            }

            post.CreatedAt = DateTime.UtcNow;

            _context.Posts.Add(post);
            _context.SaveChanges();
            _logger.LogInformation("Post created successfully with ID {PostId}.", post.PostId);

            return RedirectToAction("Index");
        }


        [HttpGet]
        public IActionResult Edit(Guid id)
        {
            var post = _context.Posts.FirstOrDefault(p => p.PostId == id);
            if (post == null) return NotFound();

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (post.CreatedBy != Guid.Parse(userId) && !User.IsInRole("Admin")) return Forbid();

            return View(post);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Post post, IFormFile Image)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var existingPost = _context.Posts.FirstOrDefault(p => p.PostId == post.PostId);

            if (existingPost == null || (existingPost.CreatedBy != Guid.Parse(userId) && !User.IsInRole("Admin")))
            {
                return Forbid();
            }

            existingPost.Text = post.Text;

            if (Image != null && Image.Length > 0)
            {
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads");
                var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(Image.FileName);
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    Image.CopyTo(fileStream);
                }

                existingPost.ImageUrl = "/uploads/" + uniqueFileName;
            }

            _context.Posts.Update(existingPost);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult Delete(Guid id)
        {
            var post = _context.Posts.FirstOrDefault(p => p.PostId == id);
            if (post == null) return NotFound();

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (post.CreatedBy != Guid.Parse(userId) && !User.IsInRole("Admin")) return Forbid();

            _context.Posts.Remove(post);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }
        [HttpPost]
        public IActionResult Like(Guid postId)
        {
            var post = _context.Posts.FirstOrDefault(p => p.PostId == postId);
            if (post == null) return NotFound();

            post.Likes++;
            _context.SaveChanges();

            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddComment(Guid postId, string commentText)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Forbid();

            var post = _context.Posts.Include(p => p.Comments).FirstOrDefault(p => p.PostId == postId);
            if (post == null) return NotFound();

            var comment = new Comment
            {
                Text = commentText,
                PostId = postId,
                CreatedBy = Guid.Parse(userId),
            };

            _context.Comments.Add(comment);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }

    }
}

