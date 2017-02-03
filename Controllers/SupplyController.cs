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
    public class SupplyController : Controller
    {
        private FYPNFCWineSystemEntities db = new FYPNFCWineSystemEntities();

        //
        // GET: /Supply/

        public ViewResult Index(string sortOrder, string currentFilter, string searchString, int? page)
        {
            ViewBag.CurrentSort = sortOrder;
            ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "Name desc" : "";
            ViewBag.MarketValueSortParm = sortOrder == "MarketValue" ? "MarketValue desc" : "MarketValue";
            ViewBag.GroupSortParm = sortOrder == "Group" ? "Group desc" : "Group";
            ViewBag.TrustStatusSortParm = sortOrder == "TrustStatus" ? "TrustStatus desc" : "TrustStatus";

            if (Request.HttpMethod == "GET")
            {
                searchString = currentFilter;    //remain current page if no new search
            }

            else
            {
                page = 1;    //reset to page 1 if user submits a search
            }

            ViewBag.CurrentFilter = searchString;    //store 'searchString' in temp data container

            var supplychains = from a in db.SupplyChains.Include("ActiveWine").Include("Group").Include("TrustStatu") select a;

            if (!String.IsNullOrEmpty(searchString))
            {
                supplychains = supplychains.Where(a => a.PartnerName.Contains(searchString));
            }

            switch (sortOrder)
            {
                case "Name desc":
                    supplychains = supplychains.OrderByDescending(a => a.PartnerName);
                    break;
                case "MarketValue":
                    supplychains = supplychains.OrderBy(a => a.MarketValue);
                    break;
                case "MarketValue desc":
                    supplychains = supplychains.OrderByDescending(a => a.MarketValue);
                    break;
                case "Group":
                    supplychains = supplychains.OrderBy(a => a.Group.GroupName);
                    break;
                case "Group desc":
                    supplychains = supplychains.OrderByDescending(a => a.Group.GroupName);
                    break;
                case "TrustStatus":
                    supplychains = supplychains.OrderBy(a => a.TrustStatu.TrustStatusName);
                    break;
                case "TrustStatus desc":
                    supplychains = supplychains.OrderByDescending(a => a.TrustStatu.TrustStatusName);
                    break;
                default:
                    supplychains = supplychains.OrderBy(a => a.PartnerName);
                    break;
            }

            int pageSize = 10;
            int pageNumber = (page ?? 1);

            return View(supplychains.ToPagedList(pageNumber, pageSize));
        }

        public ActionResult AddGroup()
        {
            return View();
        }

        [HttpPost]
        public ActionResult AddGroup(Group group)
        {
            Group newGroup = db.Groups.Create();
            newGroup.GroupName = group.GroupName;
            db.Groups.Add(newGroup);
            db.SaveChanges();
            return View();
        }

        public ActionResult ViewGroup(int? page)
        {
            var allGroup = db.Groups.ToList();

            int pageSize = 10;
            int pageNumber = (page ?? 1);


            return View(allGroup.ToPagedList(pageNumber,pageSize));
        }

        //
        // GET: /Supply/Details/5

        public ViewResult Details(int id)
        {
            SupplyChain supplychain = db.SupplyChains.Find(id);

            
            return View(supplychain);
        }

        //
        // GET: /Supply/Create

        public ActionResult Create()
        {
            ViewBag.WID = new SelectList(db.ActiveWines, "WID", "WineTitle");
            ViewBag.GroupID = new SelectList(db.Groups, "GroupID", "GroupName");
            ViewBag.TrustStatusID = new SelectList(db.TrustStatus, "TrustStatusID", "TrustStatusName");
            return View();
        } 

        //
        // POST: /Supply/Create

        [HttpPost]
        public ActionResult Create(SupplyChain supplychain)
        {
            supplychain.contactPic = "~/Content/5.jpg";
            if (ModelState.IsValid)
            {
                db.SupplyChains.Add(supplychain);
                db.SaveChanges();
                return RedirectToAction("Index");  
            }

            ViewBag.WID = new SelectList(db.ActiveWines, "WID", "WineTitle", supplychain.WID);
            ViewBag.GroupID = new SelectList(db.Groups, "GroupID", "GroupName", supplychain.GroupID);
            ViewBag.TrustStatusID = new SelectList(db.TrustStatus, "TrustStatusID", "TrustStatusName", supplychain.TrustStatusID);
            return View(supplychain);
        }
        
        //
        // GET: /Supply/Edit/5
 
        public ActionResult Edit(int id)
        {
            SupplyChain supplychain = db.SupplyChains.Find(id);
            ViewBag.WID = new SelectList(db.ActiveWines, "WID", "WineTitle", supplychain.WID);
            ViewBag.GroupID = new SelectList(db.Groups, "GroupID", "GroupName", supplychain.GroupID);
            ViewBag.TrustStatusID = new SelectList(db.TrustStatus, "TrustStatusID", "TrustStatusName", supplychain.TrustStatusID);
            return View(supplychain);
        }

        //
        // POST: /Supply/Edit/5

        [HttpPost]
        public ActionResult Edit(SupplyChain supplychain)
        {
            if (ModelState.IsValid)
            {
                db.Entry(supplychain).State = EntityState.Modified;
                db.Entry(supplychain).Property(x => x.contactPic).IsModified = false;
                try
                {
                    db.SaveChanges();
                }
                catch (Exception e)
                {
                    var ee = e;
                }
                
                return RedirectToAction("Index");
            }
            ViewBag.WID = new SelectList(db.ActiveWines, "WID", "WineTitle", supplychain.WID);
            ViewBag.GroupID = new SelectList(db.Groups, "GroupID", "GroupName", supplychain.GroupID);
            ViewBag.TrustStatusID = new SelectList(db.TrustStatus, "TrustStatusID", "TrustStatusName", supplychain.TrustStatusID);
            return View(supplychain);
        }

        //
        // GET: /Supply/Delete/5
 
        public ActionResult Delete(int id)
        {
            SupplyChain supplychain = db.SupplyChains.Find(id);
            return View(supplychain);
        }

        //
        // POST: /Supply/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {            
            SupplyChain supplychain = db.SupplyChains.Find(id);
            db.SupplyChains.Remove(supplychain);
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