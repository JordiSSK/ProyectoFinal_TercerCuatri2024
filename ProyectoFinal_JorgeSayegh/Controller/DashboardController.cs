using Microsoft.AspNetCore.Mvc;
using ProyectoFinal_JorgeSayegh.Contexts;
using System.Linq;

namespace ProyectoFinal_JorgeSayegh.Controllers
{
    public class DashboardController : Microsoft.AspNetCore.Mvc.Controller 
    {
        private readonly ApplicationDbContext _context;

        public DashboardController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            // Obtener widgets favoritos primero
            var widgets = _context.Widgets
                .OrderByDescending(w => w.IsFavorite) // Prioriza favoritos
                .ToList();

            return View(widgets);
        }
        [HttpPost]
        public IActionResult ToggleFavorite(Guid widgetId)
        {
            var widget = _context.Widgets.FirstOrDefault(w => w.WidgetId == widgetId);
            if (widget != null)
            {
                widget.IsFavorite = !widget.IsFavorite;
                _context.SaveChanges();
            }
            return RedirectToAction("Index");
        }
    }
}