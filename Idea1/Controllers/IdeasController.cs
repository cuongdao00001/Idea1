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
using OfficeOpenXml;
using Ionic.Zip;


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
        public ActionResult ExportZip()
        {
            // Create a new zip file
            using (var zip = new ZipFile())
            {
                // Get all ideas from the database
                var ideas = db.Ideas.ToList();

                // Add each idea to the zip file
                foreach (var idea in ideas)
                {
                    // Create a file name for the idea based on its ID and title
                    var fileName = string.Format("{0}_{1}.txt", idea.IdeaId, idea.Title);

                    // Create a text file with the idea's information
                    var fileContent = string.Format("Title: {0}\r\nBrief: {1}\r\nContent: {2}\r\nCategory: {3}\r\nTopic: {4}\r\nLikes: {5}\r\nDislikes: {6}\r\nViews: {7}",
                        idea.Title, idea.Brief, idea.Content, idea.Category.Name, idea.Topic.Title, idea.Like, idea.Dislike, idea.Views);

                    // Add the file to the zip file
                    zip.AddEntry(fileName, fileContent);
                }

                // Set the name of the zip file
                string ZipfileName = "ideas.zip";

                // Set the content type of the response
                Response.ContentType = "application/zip";
                Response.AddHeader("Content-Disposition", "attachment; Zipfilename=" + ZipfileName);

                // Write the zip file to the response
                zip.Save(Response.OutputStream);
            }

            return new EmptyResult();
        }
        public ActionResult ExportExcel()
        {
            var ideas = db.Ideas.ToList();
            // Create a new Excel package
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using (var package = new ExcelPackage())
            {
                // Create a new worksheet
                var worksheet = package.Workbook.Worksheets.Add("ideas");


                // Add data to the worksheet
                worksheet.Cells["A1"].Value = "ID";
                worksheet.Cells["B1"].Value = "Title";
                worksheet.Cells["C1"].Value = "Brief";
                worksheet.Cells["D1"].Value = "Content";
                worksheet.Cells["E1"].Value = "Category";
                worksheet.Cells["F1"].Value = "Topic";
                worksheet.Cells["G1"].Value = "IsAnonymous";
                worksheet.Cells["H1"].Value = "Like";
                worksheet.Cells["I1"].Value = "Dislike";
                worksheet.Cells["J1"].Value = "Views";

                // Thêm dữ liệu vào bảng
                for (int i = 0; i < ideas.Count; i++)
                {
                    var idea = ideas[i];
                    worksheet.Cells[i + 2, 1].Value = idea.IdeaId;
                    worksheet.Cells[i + 2, 2].Value = idea.Title;
                    worksheet.Cells[i + 2, 3].Value = idea.Brief;
                    worksheet.Cells[i + 2, 4].Value = idea.Content;
                    worksheet.Cells[i + 2, 5].Value = idea.Category.Name;
                    worksheet.Cells[i + 2, 6].Value = idea.Topic.Title;
                    worksheet.Cells[i + 2, 7].Value = idea.IsAnonymous;
                    worksheet.Cells[i + 2, 8].Value = idea.Like;
                    worksheet.Cells[i + 2, 9].Value = idea.Dislike;
                    worksheet.Cells[i + 2, 10].Value = idea.Views;
                }

                // Set the name of the Excel file
                string fileName = "data.xlsx";

                // Set the content type of the response
                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                Response.AddHeader("Content-Disposition", "attachment; filename=" + fileName);

                // Write the Excel file to the response
                Response.BinaryWrite(package.GetAsByteArray());
            }

            return new EmptyResult();
        }

    }
}
    
