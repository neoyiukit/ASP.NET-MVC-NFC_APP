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
    public class ProjectController : Controller
    {
        private FYPNFCWineSystemEntities db = new FYPNFCWineSystemEntities();

        //
        // GET: /Project/
        public ViewResult Index(string sortOrder, string currentFilter, string searchString, int? page)
        {
            ViewBag.CurrentSort = sortOrder;
            ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "Name desc" : "";
            ViewBag.GroupSortParm = sortOrder == "Group" ? "Group desc" : "Group";
            ViewBag.PartnerSortParm = sortOrder == "Partner" ? "Partner desc" : "Partner";
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

            var projects = from a in db.Projects.Include("Group").Include("ProjectStatu").Include("SupplyChain") select a;

            if (!String.IsNullOrEmpty(searchString))
            {
                projects = projects.Where(a => a.ProjectName.Contains(searchString));
            }


            switch (sortOrder)
            {
                case "Name desc":
                    projects = projects.OrderByDescending(a => a.ProjectName);
                    break;
                case "Group":
                    projects = projects.OrderBy(a => a.Group.GroupName);
                    break;
                case "Group desc":
                    projects = projects.OrderByDescending(a => a.Group.GroupName);
                    break;
                case "Partner":
                    projects = projects.OrderBy(a => a.SupplyChain.PartnerName);
                    break;
                case "Partner desc":
                    projects = projects.OrderByDescending(a => a.SupplyChain.PartnerName);
                    break;
                case "Date":
                    projects = projects.OrderBy(a => a.ProjectStartDate);
                    break;
                case "Date desc":
                    projects = projects.OrderByDescending(a => a.ProjectStartDate);
                    break;
                default:
                    projects = projects.OrderBy(a => a.ProjectName);
                    break;
            }



            int pageSize = 10;
            int pageNumber = (page ?? 1);

            return View(projects.ToPagedList(pageNumber, pageSize));
        }
        //
        // GET: /Project/Details/5

        public ViewResult Details(int id)
        {
            Project project = db.Projects.Find(id);
            return View(project);
        }

        //
        // GET: /Project/Create

        public ActionResult Create()
        {
            ViewBag.GroupID = new SelectList(db.Groups, "GroupID", "GroupName");
            ViewBag.ProjectStatusID = new SelectList(db.ProjectStatus, "ProjectStatusID", "ProjectStatusName");
            ViewBag.SupplyID = new SelectList(db.SupplyChains, "SupplyID", "PartnerName");
            ViewBag.WID = new SelectList(db.ActiveWines, "WID", "WineTitle");
            return View();
        } 

        //
        // POST: /Project/Create

        [HttpPost]
        public ActionResult Create(Project project)
        {
            if (ModelState.IsValid)
            {
                db.Projects.Add(project);
                db.SaveChanges();
                return RedirectToAction("Index");  
            }

            ViewBag.GroupID = new SelectList(db.Groups, "GroupID", "GroupName", project.GroupID);
            ViewBag.ProjectStatusID = new SelectList(db.ProjectStatus, "ProjectStatusID", "ProjectStatusName", project.ProjectStatusID);
            ViewBag.SupplyID = new SelectList(db.SupplyChains, "SupplyID", "Address", project.SupplyID);
            return View(project);
        }
        
        //
        // GET: /Project/Edit/5
 
        public ActionResult Edit(int id)
        {
            Project project = db.Projects.Find(id);
            ViewBag.WID = new SelectList(db.ActiveWines, "WID", "WineTitle", project.WID);
            ViewBag.GroupID = new SelectList(db.Groups, "GroupID", "GroupName", project.GroupID);
            ViewBag.ProjectStatusID = new SelectList(db.ProjectStatus, "ProjectStatusID", "ProjectStatusName", project.ProjectStatusID);
            ViewBag.SupplyID = new SelectList(db.SupplyChains, "SupplyID", "PartnerName", project.SupplyID);
            return View(project);
        }

        //
        // POST: /Project/Edit/5

        [HttpPost]
        public ActionResult Edit(Project project)
        {
            if (ModelState.IsValid)
            {
                db.Entry(project).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.GroupID = new SelectList(db.Groups, "GroupID", "GroupName", project.GroupID);
            ViewBag.ProjectStatusID = new SelectList(db.ProjectStatus, "ProjectStatusID", "ProjectStatusName", project.ProjectStatusID);
            ViewBag.SupplyID = new SelectList(db.SupplyChains, "SupplyID", "Address", project.SupplyID);
            return View(project);
        }

        //
        // GET: /Project/Delete/5
 
        public ActionResult Delete(int id)
        {
            Project project = db.Projects.Find(id);
            return View(project);
        }

        //
        // POST: /Project/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {            
            Project project = db.Projects.Find(id);
            db.Projects.Remove(project);
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