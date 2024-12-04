using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProyectoFinal_JorgeSayegh.Contexts;
using ProyectoFinal_JorgeSayegh.Models;

namespace ProyectoFinal_JorgeSayegh.Controllers
{
    [Authorize(Roles = "Admin")]
    public class SettingsController : Microsoft.AspNetCore.Mvc.Controller
    {
        private readonly ApplicationDbContext _context;

        public SettingsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var widgets = _context.Widgets.ToList();
            return View(widgets);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Widget widget)
        {
            if (ModelState.IsValid)
            {
                _context.Widgets.Add(widget);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(widget);
        }

        [HttpGet]
        public IActionResult Edit(Guid id)
        {
            var widget = _context.Widgets.FirstOrDefault(w => w.WidgetId == id);
            if (widget == null) return NotFound();
            return View(widget);
        }

        [HttpPost]
        public IActionResult Edit(Widget widget)
        {
            if (ModelState.IsValid)
            {
                _context.Widgets.Update(widget);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(widget);
        }

        [HttpGet]
        public IActionResult Delete(Guid id)
        {
            var widget = _context.Widgets.FirstOrDefault(w => w.WidgetId == id);
            if (widget == null) return NotFound();
            return View(widget);
        }

        [HttpPost, ActionName("Delete")]
        public IActionResult DeleteConfirmed(Guid id)
        {
            var widget = _context.Widgets.FirstOrDefault(w => w.WidgetId == id);
            if (widget != null)
            {
                _context.Widgets.Remove(widget);
                _context.SaveChanges();
            }
            return RedirectToAction("Index");
        }
        [HttpGet]
        public IActionResult GlobalSettings()
        {
            var settings = _context.GlobalSettings.FirstOrDefault();
            if (settings == null)
            {
                settings = new GlobalSettings
                {
                    DefaultRefreshInterval = 60,
                    DefaultApiKey = string.Empty,
                    DefaultWidgetSize = "Medium"
                };
                _context.GlobalSettings.Add(settings);
                _context.SaveChanges();
            }
            return View(settings);
        }

        [HttpPost]
        public IActionResult GlobalSettings(GlobalSettings model)
        {
            if (ModelState.IsValid)
            {
                var settings = _context.GlobalSettings.FirstOrDefault();
                if (settings != null)
                {
                    settings.DefaultRefreshInterval = model.DefaultRefreshInterval;
                    settings.DefaultApiKey = model.DefaultApiKey;
                    settings.DefaultWidgetSize = model.DefaultWidgetSize;
                    _context.SaveChanges();
                }
                return RedirectToAction("Index");
            }
            return View(model);
        }
    }
}
