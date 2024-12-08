using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ProyectoFinal_JorgeSayegh.Contexts;
using ProyectoFinal_JorgeSayegh.Models;
using ProyectoFinal_JorgeSayegh.Services;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace ProyectoFinal_JorgeSayegh.Controllers
{
    [Authorize(Roles = "Admin,User")] // Both Admin and User can access widgets
    public class WidgetController : Microsoft.AspNetCore.Mvc.Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ApiService _apiService;
        private readonly ILogger<WidgetController> _logger;

        public WidgetController(ApplicationDbContext context, ApiService apiService, ILogger<WidgetController> logger)
        {
            _context = context;
            _apiService = apiService;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Index()
        {
            _logger.LogInformation("Accessing the Index action.");
            var widgets = _context.Widgets.ToList();
            _logger.LogInformation("Fetched {Count} widgets from the database.", widgets.Count);
            return View(widgets);
        }

        [HttpGet]
public async Task<IActionResult> RefreshData(Guid id)
{
    _logger.LogInformation("Accessing RefreshData for WidgetId {Id}.", id);
    var widget = _context.Widgets.FirstOrDefault(w => w.WidgetId == id);
    if (widget == null)
    {
        _logger.LogWarning("Widget with ID {Id} not found.", id);
        return NotFound("Widget not found.");
    }

    if (string.IsNullOrEmpty(widget.DataUrl))
    {
        _logger.LogWarning("Widget with ID {Id} does not have a DataUrl.", id);
        return BadRequest("Widget does not have a valid DataUrl.");
    }

    try
    {
        var client = new HttpClient();
        var response = await client.GetStringAsync(widget.DataUrl);
        var parsedData = string.Empty;

        if (widget.Type.Equals("Image", StringComparison.OrdinalIgnoreCase))
        {
            // Parse JSON response to get the image URL
            var json = System.Text.Json.JsonDocument.Parse(response);
            if (json.RootElement.TryGetProperty("message", out var message))
            {
                parsedData = message.GetString();
            }
        }
        else if (widget.Type.Equals("Text", StringComparison.OrdinalIgnoreCase))
        {
            // Parse jokes or other text-based data
            var json = System.Text.Json.JsonDocument.Parse(response);
            if (json.RootElement.TryGetProperty("setup", out var setup) &&
                json.RootElement.TryGetProperty("punchline", out var punchline))
            {
                parsedData = $"{setup.GetString()} {punchline.GetString()}";
            }
            else if (json.RootElement.TryGetProperty("fact", out var fact))
            {
                parsedData = fact.GetString();
            }
        }
        else
        {
            parsedData = response; // Default to raw response for unknown types
        }

        _logger.LogInformation("Data fetched and parsed successfully for WidgetId {Id}.", id);
        return Json(new { data = parsedData });
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Error fetching data for WidgetId {Id}.", id);
        return StatusCode(500, "Error fetching data from the API.");
    }
}



        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddToFavorites(Guid id)
        {
            var widget = _context.Widgets.FirstOrDefault(w => w.WidgetId == id);
            if (widget != null)
            {
                widget.IsFavorite = true;
                _context.SaveChanges();
                _logger.LogInformation("Widget {Name} added to favorites.", widget.Name);
            }
            return RedirectToAction("Index", "Dashboard");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult RemoveFromFavorites(Guid id)
        {
            var widget = _context.Widgets.FirstOrDefault(w => w.WidgetId == id);
            if (widget != null)
            {
                widget.IsFavorite = false;
                _context.SaveChanges();
                _logger.LogInformation("Widget {Name} removed from favorites.", widget.Name);
            }
            return RedirectToAction("Index", "Dashboard");
        }
        
        [HttpGet]
        public IActionResult Create()
        {
            _logger.LogInformation("Accessing the Create GET action.");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Widget widget)
        {
            _logger.LogInformation("Accessing the Create POST action.");
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("ModelState is invalid.");
                foreach (var state in ModelState)
                {
                    if (state.Value.Errors.Any())
                    {
                        foreach (var error in state.Value.Errors)
                        {
                            _logger.LogWarning("Key: {Key}, Error: {ErrorMessage}", state.Key, error.ErrorMessage);
                        }
                    }
                }
                return View(widget);
            }

            // Asignar el usuario que crea el widget
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                _logger.LogWarning("User is not authenticated.");
                return Forbid(); // Denegar si no hay autenticación
            }

            widget.CreatedBy = Guid.Parse(userId);
            widget.WidgetId = Guid.NewGuid();

            _context.Widgets.Add(widget);
            _context.SaveChanges();
            _logger.LogInformation("Widget {Name} created successfully.", widget.Name);

            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Edit(Guid id)
        {
            _logger.LogInformation("Accessing the Edit GET action with WidgetId {Id}.", id);
            var widget = _context.Widgets.FirstOrDefault(w => w.WidgetId == id);
            if (widget == null)
            {
                _logger.LogWarning("Widget with ID {Id} not found.", id);
                return NotFound();
            }

            // Verificar si el usuario tiene permiso para editar (opcional)
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (widget.CreatedBy != Guid.Parse(userId) && !User.IsInRole("Admin"))
            {
                _logger.LogWarning("User {UserId} does not have permission to edit widget {WidgetId}.", userId, id);
                return Forbid();
            }

            return View(widget);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Widget widget)
        {
            _logger.LogInformation("Accessing the Edit POST action for WidgetId {Id}.", widget.WidgetId);

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("ModelState is invalid for WidgetId {Id}.", widget.WidgetId);
                foreach (var state in ModelState)
                {
                    if (state.Value.Errors.Any())
                    {
                        foreach (var error in state.Value.Errors)
                        {
                            _logger.LogWarning("Key: {Key}, Error: {ErrorMessage}", state.Key, error.ErrorMessage);
                        }
                    }
                }
                return View(widget);
            }

            try
            {
                // Verificar permisos
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var existingWidget = _context.Widgets.FirstOrDefault(w => w.WidgetId == widget.WidgetId);

                if (existingWidget == null)
                {
                    _logger.LogWarning("Widget with ID {Id} not found.", widget.WidgetId);
                    return NotFound();
                }

                if (existingWidget.CreatedBy != Guid.Parse(userId) && !User.IsInRole("Admin"))
                {
                    _logger.LogWarning("User {UserId} does not have permission to edit widget {WidgetId}.", userId, widget.WidgetId);
                    return Forbid();
                }

                // Actualizar solo las propiedades necesarias
                existingWidget.Name = widget.Name;
                existingWidget.Description = widget.Description;
                existingWidget.Type = widget.Type;
                existingWidget.DataUrl = widget.DataUrl;
                existingWidget.RefreshInterval = widget.RefreshInterval;

                _context.Widgets.Update(existingWidget);
                _context.SaveChanges();

                _logger.LogInformation("Widget {Name} updated successfully by User {UserId}.", widget.Name, userId);
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while editing WidgetId {Id}.", widget.WidgetId);
                return View(widget);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(Guid id)
        {
            _logger.LogInformation("Accessing the Delete POST action with WidgetId {Id}.", id);
            try
            {
                var widget = _context.Widgets.FirstOrDefault(w => w.WidgetId == id);
                if (widget == null)
                {
                    _logger.LogWarning("Widget with ID {Id} not found.", id);
                    return NotFound();
                }

                _context.Widgets.Remove(widget);
                _context.SaveChanges();
                _logger.LogInformation("Widget {Name} deleted successfully.", widget.Name);
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while deleting a widget.");
                return RedirectToAction("Index");
            }
        }
    }
}
