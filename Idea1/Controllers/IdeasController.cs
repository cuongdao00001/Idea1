using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Idea1.Models;
using Microsoft.AspNet.Identity;

namespace Idea1.Controllers
{
    public class IdeasController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Ideas
        public ActionResult Index(int? topicId)
        {

            if (topicId == null)
            {
                return RedirectToAction("Index", "Topics");
            }

            var topic = db.Topics.Find(topicId);
            if (topic == null)
            {
                return HttpNotFound();
            }

            var ideas = db.Ideas.Where(i => i.TopicId == topicId).ToList();
            ViewBag.TopicId = topicId;
            ViewBag.TopicTitle = topic.Title;
            ViewBag.TopicFirstDate = topic.FirstDate;
            ViewBag.TopicLastDate = topic.LastDate;

            return View(ideas);
        }


        // GET: Ideas/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Idea idea = db.Ideas.Find(id);
            if (idea == null)
            {
                return HttpNotFound();
            }
            string fileName = Path.GetFileName(idea.FileData);
            idea.FileName = fileName;

            return View(idea);
        }
        // GET: Ideas/Create
        public ActionResult Create(string id)
        {
            var categories = db.Categories.ToList();
           
            ViewBag.CategoryId = new SelectList(categories, "CategoryId", "Name");
            ViewBag.TopicId = id;
            var topicId = Convert.ToInt32(id);
            var topics = db.Topics.Where(t => t.TopicId == topicId).ToList();
            var selectList = new SelectList(topics, "TopicId", "Title");
            ViewBag.TopicId = selectList;
            return View();
        }

        // POST: Ideas/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "IdeaId,Title,Brief,FileName,FileData,Content,CategoryId,TopicId,IsAnonymous")] Idea idea, HttpPostedFileBase file)
        {
            if (ModelState.IsValid)
            {
                // Gán thông tin ApplicationUser vào Idea
                idea.UserId = User.Identity.GetUserId();

                
                // Kiểm tra xem có đang ở chế độ ẩn danh hay không
                if (idea.IsAnonymous)
                {
                    idea.UserId = null;
                }


                // Lấy topic từ database theo TopicId được chọn
                var topic = db.Topics.Find(idea.TopicId);
                if (topic == null)
                {
                    return HttpNotFound();
                }

                if (file != null && file.ContentLength > 0)
                {
                    var newFileName = Guid.NewGuid();
                    var _extension = Path.GetExtension(file.FileName);
                    string NewName = newFileName + _extension;
                    string fileName = Path.GetFileName(NewName);
                    string filePath = Path.Combine(Server.MapPath("~/Uploads/"), fileName);
                    file.SaveAs(filePath);
                    idea.FileData = filePath;
                    idea.FileName = fileName;
                }



                // Lưu idea vào database
                db.Ideas.Add(idea);
                db.SaveChanges();

                return RedirectToAction("Index");
            }

            ViewBag.CategoryId = new SelectList(db.Categories, "CategoryId", "Name", idea.CategoryId);
            ViewBag.TopicId = new SelectList(db.Topics, "TopicId", "Title", idea.TopicId);
            return View(idea);
        }


        // POST: Ideas/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "IdeaId,Title,Brief,FileName,FileData,Content")] Idea idea)
        {
            if (ModelState.IsValid)
            {
                db.Entry(idea).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(idea);
        }

        // GET: Ideas/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Idea idea = db.Ideas.Find(id);
            if (idea == null)
            {
                return HttpNotFound();
            }
            return View(idea);
        }

        // POST: Ideas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Idea idea = db.Ideas.Find(id);
            db.Ideas.Remove(idea);
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
        public FileResult Download(string fileName)
        {
            var fileVirtualPath = "~/Uploads/" + fileName;
            return File(fileVirtualPath, "application/force-download", fileName);
        }

    }
}
