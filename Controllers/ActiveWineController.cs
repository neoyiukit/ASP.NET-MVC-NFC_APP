using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FYPNFCWineSystem.Models;
using PagedList;
using System.Security.Cryptography;
using System.Text;

namespace FYPNFCWineSystem.Controllers
{
    [Authorize]
    public class ActiveWineController : Controller
    {
        private FYPNFCWineSystemEntities db = new FYPNFCWineSystemEntities();

        //
        // GET: /ActiveWine/

        public ViewResult Index(string sortOrder, string currentFilter, string searchString, int? page)
        {
            ViewBag.CurrentSort = sortOrder;
            ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "Name desc" : "";
            ViewBag.vintageSortParm = sortOrder == "Vintage" ? "Vintage desc" : "Vintage";
            ViewBag.CateSortParm = sortOrder == "Cate" ? "Cate desc" : "Cate";
            ViewBag.producerSortParm = sortOrder == "producer" ? "producer desc" : "producer";
            ViewBag.countrySortParm = sortOrder == "country" ? "country desc" : "country";
            ViewBag.PriSortParm = sortOrder == "Pri" ? "Pri desc" : "Pri";

            if (Request.HttpMethod == "GET")
            {
                searchString = currentFilter;    //remain current page if no new search
            }

            else
            {
                page = 1;    //reset to page 1 if user submits a search
            }

            ViewBag.CurrentFilter = searchString;    //store 'searchString' in temp data container

            var activewines = from a in db.ActiveWines.Include("Project").Include("Rejection").Include("WineCategory").Include("WineStatu").Include("InactiveWine") select a;

            activewines = activewines.Where(x => x.WineStatusID == 1);

            if (!String.IsNullOrEmpty(searchString))
            {
                activewines = activewines.Where(a => a.WineTitle.ToUpper().Contains(searchString));
            }


            switch (sortOrder)
            {
                case "Name desc":
                    activewines = activewines.OrderByDescending(a => a.WineTitle);
                    break;
                case "Vintage":
                    activewines = activewines.OrderBy(a => a.Vintage);
                    break;
                case "Vintage desc":
                    activewines = activewines.OrderByDescending(a => a.Vintage);
                    break;

                case "Cate desc":
                    activewines = activewines.OrderByDescending(a => a.WineCategory.WineCategoryName);
                    break;
                case "Cate":
                    activewines = activewines.OrderBy(a => a.WineCategory.WineCategoryName);
                    break;
                case "Producer desc":
                    activewines = activewines.OrderByDescending(a => a.Producer);
                    break;
                case "Producer":
                    activewines = activewines.OrderBy(a => a.Producer);
                    break;
                case "country desc":
                    activewines = activewines.OrderByDescending(a => a.Country);
                    break;
                case "country":
                    activewines = activewines.OrderBy(a => a.Country);
                    break;
                case "Pri desc":
                    activewines = activewines.OrderByDescending(a => a.Price);
                    break;
                case "Pri":
                    activewines = activewines.OrderBy(a => a.Price);
                    break;
                default:
                    activewines = activewines.OrderBy(a => a.WineTitle);
                    break;

            }

            int pageSize = 10;
            int pageNumber = (page ?? 1);

            return View(activewines.ToPagedList(pageNumber, pageSize));
        }

        //
        // GET: /ActiveWine/Details/5

        public ViewResult Details(int id)
        {
            ActiveWine activewine = db.ActiveWines.Find(id);
            return View(activewine);
        }

        public ViewResult WineTagAchieve(int id)
        {
            List<TagValueAchieve> tagAchieveList = db.TagValueAchieve
                .Include(x=>x.ActiveWine)
                .Where(x => x.wine_id == id)
                .ToList();

            return View(tagAchieveList);
        }

        public ViewResult purgeTagAchieve()
        {
            var tagAchieveList = db.TagValueAchieve.ToList();
            foreach (var tagAchieve in tagAchieveList)
            {
                db.TagValueAchieve.Remove(tagAchieve);
            }
            db.SaveChanges();
            return View();
        }

        //
        // GET: /ActiveWine/Create

        public ActionResult Create()
        {
            ViewBag.ProjectID = new SelectList(db.Projects, "ProjectID", "ProjectName");
            ViewBag.RejectionID = new SelectList(db.Rejections, "RejectionID", "RejectionReason");
            ViewBag.WineCategoryID = new SelectList(db.WineCategories, "WineCategoryID", "WineCategoryName");
            ViewBag.WineStatusID = new SelectList(db.WineStatus, "WineStatusID", "WineStatusName");
            ViewBag.WID = new SelectList(db.InactiveWines, "WID", "WineTitle");
            return View();
        } 

        //
        // POST: /ActiveWine/Create

        [HttpPost]
        public ActionResult Create(ActiveWine activewine)
        {
            if (ModelState.IsValid)
            {
                db.ActiveWines.Add(activewine);
                db.SaveChanges();
                return RedirectToAction("Index");  
            }

            ViewBag.ProjectID = new SelectList(db.Projects, "ProjectID", "ProjectName", activewine.ProjectID);
            ViewBag.RejectionID = new SelectList(db.Rejections, "RejectionID", "RejectionReason", activewine.RejectionID);
            ViewBag.WineCategoryID = new SelectList(db.WineCategories, "WineCategoryID", "WineCategoryName", activewine.WineCategoryID);
            ViewBag.WineStatusID = new SelectList(db.WineStatus, "WineStatusID", "WineStatusName", activewine.WineStatusID);
            ViewBag.WID = new SelectList(db.InactiveWines, "WID", "WineTitle", activewine.WID);
            return View(activewine);
        }
        
        //
        // GET: /ActiveWine/Edit/5
 
        public ActionResult Edit(int id)
        {
            ActiveWine activewine = db.ActiveWines.Find(id);
            ViewBag.ProjectID = new SelectList(db.Projects, "ProjectID", "ProjectName", activewine.ProjectID);
            ViewBag.RejectionID = new SelectList(db.Rejections, "RejectionID", "RejectionReason", activewine.RejectionID);
            ViewBag.WineCategoryID = new SelectList(db.WineCategories, "WineCategoryID", "WineCategoryName", activewine.WineCategoryID);
            ViewBag.WineStatusID = new SelectList(db.WineStatus, "WineStatusID", "WineStatusName", activewine.WineStatusID);
            ViewBag.WID = new SelectList(db.InactiveWines, "WID", "WineTitle", activewine.WID);
            return View(activewine);
        }

        //
        // POST: /ActiveWine/Edit/5

        [HttpPost]
        public ActionResult Edit(ActiveWine activewine)
        {
            if (ModelState.IsValid)
            {
                db.Entry(activewine).State = EntityState.Modified;
                db.Entry(activewine).Property(x => x.readCount).IsModified = false;
                db.Entry(activewine).Property(x => x.NFCCurrentTag).IsModified = false;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ProjectID = new SelectList(db.Projects, "ProjectID", "ProjectName", activewine.ProjectID);
            ViewBag.RejectionID = new SelectList(db.Rejections, "RejectionID", "RejectionReason", activewine.RejectionID);
            ViewBag.WineCategoryID = new SelectList(db.WineCategories, "WineCategoryID", "WineCategoryName", activewine.WineCategoryID);
            ViewBag.WineStatusID = new SelectList(db.WineStatus, "WineStatusID", "WineStatusName", activewine.WineStatusID);
            ViewBag.WID = new SelectList(db.InactiveWines, "WID", "WineTitle", activewine.WID);
            return View(activewine);
        }

        //
        // GET: /ActiveWine/Delete/5
 
        public ActionResult Delete(int id)
        {
            ActiveWine activewine = db.ActiveWines.Find(id);
            return View(activewine);
        }

        private string genTagHash(int win, int readCount)
        {
            string win_str = win.ToString();
            string readCount_str = readCount.ToString();
            string toHash = win_str + readCount_str;
            return createHash(toHash);
        }

        private string createHash(string stringToHash)
        {
            string salt = "124j29098098amfwaf109dsf80s9dg782q934t34tkdjsgdg98s0a7f9a7w9fe80w9ver";
            string strToHash = stringToHash + salt;
            return string.Join("", MD5.Create().ComputeHash(Encoding.ASCII.GetBytes(strToHash)).Select(s => s.ToString("x2")));
        }

        public ActionResult ResetDatabase()
        {
            var activeWineList = db.ActiveWines.ToList();

            foreach (var wine in activeWineList)
            {
                wine.WinePic = string.Format("img/{0}.jpg", wine.WID.ToString());
                wine.readCount = 0;
                wine.WineStatusID = 1;
                wine.TagStatusID = 0;
                wine.NFCCurrentTag = genTagHash(wine.WID,0);
            }

            db.SaveChanges();

            return View();
        }

        //
        // POST: /ActiveWine/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {            
            ActiveWine activewine = db.ActiveWines.Find(id);
            db.ActiveWines.Remove(activewine);
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