using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProyectoFinal_JorgeSayegh.Contexts;
using ProyectoFinal_JorgeSayegh.Models;
using System;
using System.Linq;
using System.Security.Claims;

namespace ProyectoFinal_JorgeSayegh.Controllers
{
    [Authorize]
    public class MessagesController : Microsoft.AspNetCore.Mvc.Controller
    {
        private readonly ApplicationDbContext _context;

        public MessagesController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Inbox()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId)) return Forbid();

            var messages = _context.Messages
                .Where(m => m.RecipientId == Guid.Parse(userId))
                .Include(m => m.Sender)
                .OrderByDescending(m => m.SentAt)
                .ToList();

            // Pass the list of users to the view
            ViewBag.Users = _context.Users
                .Where(u => u.UserId != Guid.Parse(userId)) // Exclude the current user
                .ToList();
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return PartialView("_InboxMessages", messages);
            }

            return View("Inbox",messages);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Send(Guid RecipientId, string Content)
        {
            var senderId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(senderId)) return Forbid();

            var recipient = _context.Users.FirstOrDefault(u => u.UserId == RecipientId);
            if (recipient == null) return NotFound("Recipient not found.");

            var message = new Message
            {
                MessageId = Guid.NewGuid(),
                Content = Content,
                SenderId = Guid.Parse(senderId),
                RecipientId = recipient.UserId,
                SentAt = DateTime.UtcNow,
                IsRead = false
            };

            _context.Messages.Add(message);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Sent()
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var sentMessages = _context.Messages
                .Where(m => m.SenderId == userId)
                .Include(m => m.Recipient)
                .OrderByDescending(m => m.SentAt)
                .ToList();
            
            return PartialView("_SentMessages", sentMessages); // Ensure correct partial view name
        }

        [HttpGet]
        public IActionResult Compose()
        {
            var users = _context.Users.Select(u => new { u.UserId, u.Username }).ToList();
            ViewBag.Users = users;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Compose(string content, Guid recipientId)
        {
            var senderId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var message = new Message
            {
                Content = content,
                SenderId = senderId,
                RecipientId = recipientId
            };

            _context.Messages.Add(message);
            _context.SaveChanges();

            return RedirectToAction("Sent");
        }

        [HttpPost]
        public IActionResult MarkAsRead(Guid messageId)
        {
            var message = _context.Messages.FirstOrDefault(m => m.MessageId == messageId);
            if (message == null) return NotFound();

            message.IsRead = true;
            _context.SaveChanges();

            return RedirectToAction("Inbox");
        }
    }
}
