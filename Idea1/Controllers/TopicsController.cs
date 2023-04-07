using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Idea1.Models;

namespace Idea1.Controllers
{
    public class TopicsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Topics
        public ActionResult Index()
        {         
            var topics = db.Topics.ToList();
            return View(topics);
            }
        

   


        // GET: Topics/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var topic = db.Topics.Include(t => t.Ideas).FirstOrDefault(t => t.TopicId == id);

            if (topic == null)
            {
                return HttpNotFound();
            }

            return View(topic.Ideas.ToList());
        }


        // GET: Topics/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Topics/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "TopicId,Title,FirstDate,LastDate,Color")] Topic topic)
        {
            if (ModelState.IsValid)
            {
                var now = DateTime.Now.TimeOfDay;
                var firstDate = new DateTimeOffset(topic.FirstDate.Year, topic.FirstDate.Month, topic.FirstDate.Day, now.Hours, now.Minutes, now.Seconds, now.Milliseconds, topic.FirstDate.Offset);
                var lastDate = new DateTimeOffset(topic.LastDate.Year, topic.LastDate.Month, topic.LastDate.Day, now.Hours, now.Minutes, now.Seconds, now.Milliseconds, topic.LastDate.Offset);

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

                topic.FirstDate = firstDate;
                topic.LastDate = lastDate;
                topic.Ideas = new HashSet<Idea>();
                db.Topics.Add(topic);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(topic);
        }




        // GET: Topics/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Topic topic = db.Topics.Find(id);
            if (topic == null)
            {
                return HttpNotFound();
            }
            return View(topic);
        }

        // POST: Topics/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "TopicId,Title,FirstDate,LastDate")] Topic topic)
        {
            if (ModelState.IsValid)
            {
                var now = DateTime.Now.TimeOfDay;
                var firstDate = new DateTimeOffset(topic.FirstDate.Year, topic.FirstDate.Month, topic.FirstDate.Day, now.Hours, now.Minutes, now.Seconds, now.Milliseconds, topic.FirstDate.Offset);
                var lastDate = new DateTimeOffset(topic.LastDate.Year, topic.LastDate.Month, topic.LastDate.Day, now.Hours, now.Minutes, now.Seconds, now.Milliseconds, topic.LastDate.Offset);

                topic.FirstDate = firstDate;
                topic.LastDate = lastDate;

                db.Entry(topic).State = EntityState.Modified;
                db.SaveChanges();

                return RedirectToAction("Index");
            }
            return View(topic);
        }

        // GET: Topics/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Topic topic = db.Topics.Find(id);
            if (topic == null)
            {
                return HttpNotFound();
            }
            return View(topic);
        }

        // POST: Topics/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Topic topic = db.Topics.Find(id);
            db.Topics.Remove(topic);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

    }
}
