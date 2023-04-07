using Idea1.Models;
using Microsoft.Ajax.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Idea1.Controllers
{
    public class StaffController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        // GET: Staff
        public ActionResult Index()
        {
            var topics = db.Topics.ToList();
            foreach (var topic in topics)
            {
                var lastDate = topic.LastDate.LocalDateTime;
                if (lastDate > DateTime.Now)
                {
                    topic.CanAddIdea = true;
                }
                else if (lastDate == DateTime.Now)
                {
                    topic.CanAddIdea = true;
                }
                else
                {
                    topic.CanAddIdea = false;
                }
            }
            return View(topics);
        }
    }
}