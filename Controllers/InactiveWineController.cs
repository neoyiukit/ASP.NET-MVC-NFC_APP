using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FYPNFCWineSystem.Models;
using PagedList;

namespace FYPNFCWineSystem.Controllers
{
    [Authorize]
    public class InactiveWineController : Controller
    {
        private FYPNFCWineSystemEntities db = new FYPNFCWineSystemEntities();

        //
        // GET: /InactiveWine/

        public ViewResult Index(string sortOrder, string currentFilter, string searchString, int? page)
        {
            ViewBag.CurrentSort = sortOrder;
            ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "Name desc" : "";
            ViewBag.vintageSortParm = sortOrder == "Vintage" ? "Vintage desc" : "Vintage";
            ViewBag.CateSortParm = sortOrder == "Cate" ? "Cate desc" : "Cate";
            ViewBag.producerSortParm = sortOrder == "producer" ? "producer desc" : "producer";
            ViewBag.countrySortParm = sortOrder == "country" ? "country desc" : "country";
            ViewBag.DateSortParm = sortOrder == "Date" ? "Date desc" : "Date";

            if (Request.HttpMethod == "GET")
            {
                searchString = currentFilter;    //remain current page if no new search
            }

            else
            {
                page = 1;    //reset to page 1 if user submits a search
            }

            ViewBag.CurrentFilter = searchString;    //store 'searchString' in temp data container

            //var inactivewines = from a in db.InactiveWines.Include("ActiveWine").Include("Rejection").Include("WineCategory").Include("WineStatu") select a;
            var inactivewines = db.ActiveWines
                .Where(x => x.WineStatusID == 2);


            if (!String.IsNullOrEmpty(searchString))
            {
                inactivewines = inactivewines.Where(a => a.WineTitle.ToUpper().Contains(searchString));
            }


            switch (sortOrder)
            {
                case "Name desc":
                    inactivewines = inactivewines.OrderByDescending(a => a.WineTitle);
                    break;
                case "Vintage":
                    inactivewines = inactivewines.OrderBy(a => a.Vintage);
                    break;
                case "Vintage desc":
                    inactivewines = inactivewines.OrderByDescending(a => a.Vintage);
                    break;

                case "Cate desc":
                    inactivewines = inactivewines.OrderByDescending(a => a.WineCategory.WineCategoryName);
                    break;
                case "Cate":
                    inactivewines = inactivewines.OrderBy(a => a.WineCategory.WineCategoryName);
                    break;
                case "Producer desc":
                    inactivewines = inactivewines.OrderByDescending(a => a.Producer);
                    break;
                case "Producer":
                    inactivewines = inactivewines.OrderBy(a => a.Producer);
                    break;
                case "country desc":
                    inactivewines = inactivewines.OrderByDescending(a => a.Country);
                    break;
                case "country":
                    inactivewines = inactivewines.OrderBy(a => a.Country);
                    break;
                case "Date desc":
                    inactivewines = inactivewines.OrderByDescending(a => a.RejectionDate);
                    break;
                case "Date":
                    inactivewines = inactivewines.OrderBy(a => a.RejectionDate);
                    break;
                default:
                    inactivewines = inactivewines.OrderBy(a => a.WineTitle);
                    break;

            }

            int pageSize = 10;
            int pageNumber = (page ?? 1);

            return View(inactivewines.ToPagedList(pageNumber, pageSize));
        }

        //
        // GET: /InactiveWine/Details/5

        public ViewResult Details(int id)
        {
            ActiveWine activeWine = db.ActiveWines.Find(id);
            return View(activeWine);
        }

        //
        // GET: /InactiveWine/Create

        public ActionResult Create()
        {
            ViewBag.WID = new SelectList(db.ActiveWines, "WID", "WineTitle");
            ViewBag.ProjectID = new SelectList(db.Projects, "ProjectID", "ProjectName");
            ViewBag.RejectionID = new SelectList(db.Rejections, "RejectionID", "RejectionReason");
            ViewBag.WineCategoryID = new SelectList(db.WineCategories, "WineCategoryID", "WineCategoryName");
            ViewBag.WineStatusID = new SelectList(db.WineStatus, "WineStatusID", "WineStatusName");
            return View();
        } 

        //
        // POST: /InactiveWine/Create

        [HttpPost]
        public ActionResult Create(ActiveWine activewine)
        {
            if (ModelState.IsValid)
            {
                db.ActiveWines.Add(activewine);
                db.SaveChanges();
                return RedirectToAction("Index");  
            }

            ViewBag.WID = new SelectList(db.ActiveWines, "WID", "WineTitle", activewine.WID);
            ViewBag.ProjectID = new SelectList(db.Projects, "ProjectID", "ProjectName");
            ViewBag.RejectionID = new SelectList(db.Rejections, "RejectionID", "RejectionReason", activewine.RejectionID);
            ViewBag.WineCategoryID = new SelectList(db.WineCategories, "WineCategoryID", "WineCategoryName", activewine.WineCategoryID);
            ViewBag.WineStatusID = new SelectList(db.WineStatus, "WineStatusID", "WineStatusName", activewine.WineStatusID);
            return View(activewine);
        }
        
        //
        // GET: /InactiveWine/Edit/5
 
        public ActionResult Edit(int id)
        {
            ActiveWine activeWine = db.ActiveWines.Find(id);
            ViewBag.WID = new SelectList(db.ActiveWines, "WID", "WineTitle", activeWine.WID);
            ViewBag.ProjectID = new SelectList(db.Projects, "ProjectID", "ProjectName");
            ViewBag.RejectionID = new SelectList(db.Rejections, "RejectionID", "RejectionReason", activeWine.RejectionID);
            ViewBag.WineCategoryID = new SelectList(db.WineCategories, "WineCategoryID", "WineCategoryName", activeWine.WineCategoryID);
            ViewBag.WineStatusID = new SelectList(db.WineStatus, "WineStatusID", "WineStatusName", activeWine.WineStatusID);
            return View(activeWine);
        }

        //
        // POST: /InactiveWine/Edit/5

        [HttpPost]
        public ActionResult Edit(ActiveWine activeWine)
        {
            if (ModelState.IsValid)
            {
                db.Entry(activeWine).State = EntityState.Modified;
                db.Entry(activeWine).Property(x => x.readCount).IsModified = false;
                db.Entry(activeWine).Property(x => x.NFCCurrentTag).IsModified = false;

                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.WID = new SelectList(db.ActiveWines, "WID", "WineTitle", activeWine.WID);
            ViewBag.ProjectID = new SelectList(db.Projects, "ProjectID", "ProjectName");
            ViewBag.RejectionID = new SelectList(db.Rejections, "RejectionID", "RejectionReason", activeWine.RejectionID);
            ViewBag.WineCategoryID = new SelectList(db.WineCategories, "WineCategoryID", "WineCategoryName", activeWine.WineCategoryID);
            ViewBag.WineStatusID = new SelectList(db.WineStatus, "WineStatusID", "WineStatusName", activeWine.WineStatusID);
            return View(activeWine);
        }

        //
        // GET: /InactiveWine/Delete/5
 
        public ActionResult Delete(int id)
        {
            ActiveWine activeWine = db.ActiveWines.Find(id);
            return View(activeWine);
        }

        //
        // POST: /InactiveWine/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {            
            ActiveWine activeWine = db.ActiveWines.Find(id);
            db.ActiveWines.Remove(activeWine);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}