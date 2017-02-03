using FYPNFCWineSystem.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;

using System.Data.Entity;

namespace FYPNFCWineSystem.Controllers
{
    public class AppController : Controller
    {
        private FYPNFCWineSystemEntities db = new FYPNFCWineSystemEntities();

        //
        // GET: /App/

        public ActionResult getAllWine()
        {
            var allActiveWine = db.ActiveWines
                .Where(x => x.WineStatusID != 2)
                .Where(x => x.TagStatusID == 0)
                .Select(x => new
                {
                    x.WID,
                    x.WineTitle,
                    x.WineCategory.WineCategoryName,
                    x.Producer,
                    x.Country,
                    x.Vintage,
                    x.Price,
                    x.WineTransactionHistory,
                    x.WineDescription
                })
                .ToList();
            return Json(allActiveWine);
        }

        public ActionResult getWine(int id)
        {
            var wine = db.ActiveWines
                .Where(x => x.WID == id)
                .Select(x => new
                {
                    WID= x.WID,
                    WineTitle = x.WineTitle,
                    WineCategoryName = x.WineCategory.WineCategoryName,
                    Producer = x.Producer,
                    Country = x.Country,
                    Vintage = x.Vintage,
                    Price = x.Price,
                    WineTransactionHistory = x.WineTransactionHistory,
                    WineDescription = x.WineDescription,
                    WinePic = x.WinePic,
                    TagID = x.TagID
                })
                .SingleOrDefault();

            if (wine == null)
            {
                return Json(new { wine = "" });
            }
            return Json(new { wine = wine });
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

        /// <summary>
        /// used for the producer app to get the tag value to write to the nfc tag
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult ProducerWritingTagForWineID(int id)
        {
            var wine = db.ActiveWines.Find(id);
            wine.TagStatusID = 1; 
            wine.WineTransactionHistory += string.Format("{0} WroteTag\r\n", DateTime.Now.ToString("dd-MM-yyyy"));
            wine.readCount = 0;

            string tagValue = genTagHash(wine.WID, wine.readCount?? 0);
            wine.NFCCurrentTag = tagValue;

            db.SaveChanges();
            return Json(new { NFCTag = tagValue });
        }

        /// <summary>
        /// invalidate the wine
        /// </summary>
        /// <param name="wid"></param>
        /// <returns> invalidated wine</returns>
        private ActiveWine invalidateActiveWine(int wid){
            var wine = db.ActiveWines.Find(wid);
            wine.WineStatusID = 2;
            db.SaveChanges();
            return wine;
        }

        public object formatAsOutputJSON(ActiveWine wine)
        {
            bool isStillValid = wine.WineStatusID == 1 ? true : false;

            return new
            {
                WID = wine.WID,
                WineTitle = wine.WineTitle,
                WineCategoryName = wine.WineCategory.WineCategoryName,
                Producer = wine.Producer,
                Country = wine.Country,
                Vintage = wine.Vintage,
                Price = wine.Price,
                WineTransactionHistory = wine.WineTransactionHistory,
                WineDescription = wine.WineDescription,
                WinePic = wine.WinePic,
                isValid = isStillValid,
                TagID = wine.TagID 
            };
        }

        // Used for the app to find the scanned wine data
        public ActionResult ConsumerFindForWine(string NFCTag)
        {
            //check if the the scanned tag exist in achieve table
            var isNFCTagExistInAchieve = db.TagValueAchieve.SingleOrDefault(x => x.tag_value == NFCTag);
            if (isNFCTagExistInAchieve != null)
            {
                //if some one read the old tag value, then we invalidate the wine
                var invalidatedWine = invalidateActiveWine(isNFCTagExistInAchieve.wine_id);

                //return back the invalidated wine
                var retJSON = formatAsOutputJSON(invalidatedWine);
                return Json(new { wine = retJSON, nextNFCTag = "", isInCommit = false });
            }
            
            var existingUpdateTransaction = db.TagUpdateTransaction.SingleOrDefault(x => x.new_tag_value == NFCTag);
            if (existingUpdateTransaction != null)
            {
                //this is the case that the NFC tag is updated, but it cannot commit it back to the server
                //therefore when the user scan again, it will scanned the new tag value
                //so we need to tell the app that it need to commit the update

                var wine1 = existingUpdateTransaction.ActiveWine;
                string newTag = existingUpdateTransaction.new_tag_value;
                var returnWine1 = formatAsOutputJSON(wine1);
                return Json(new { wine = returnWine1, nextNFCTag = newTag, isInCommit = true });
            }

            //try to find the wine
            var wine = db.ActiveWines
                .Include(x=>x.WineCategory)
                .Where(x => x.WineStatusID != 2)
                .SingleOrDefault(x => x.NFCCurrentTag == NFCTag);
            if (wine == null)
                return Json(new { wine = "", nextNFCTag = "", isInCommit = false });

            //get the new hash
            int? newReadCount = wine.readCount + 1;
            string newTagValue = genTagHash(wine.WID, newReadCount ?? 0);

            var isOldNFCTagInUpdatetransaction = db.TagUpdateTransaction.Any(x => x.old_tag_value == NFCTag);
            if (isOldNFCTagInUpdatetransaction)
            {
                //this is that case that the app request for the wine data before
                //but it seems that the nfc tag is not updated at all
                //so when the uesr scan it again, same old tag value is sent
                //therefore we let the app to continue the NFC tag update process as if it is the first time it scan the tag
                var returnWine = formatAsOutputJSON(wine);
                return Json(new { wine = returnWine, nextNFCTag = newTagValue, isInCommit = false });
            }
            else
            {
                //create new tag update transaction in database
                var TagUpdateTransaction = new TagUpdateTransaction();
                TagUpdateTransaction.old_tag_value = wine.NFCCurrentTag;
                TagUpdateTransaction.new_tag_value = newTagValue;
                TagUpdateTransaction.wine_id = wine.WID;
                db.TagUpdateTransaction.Add(TagUpdateTransaction);
                db.SaveChanges();

                //return the required wine data back to the app
                var returnWine = formatAsOutputJSON(wine);
                return Json(new { wine = returnWine, nextNFCTag = newTagValue, isInCommit = false });
            }
        }

        // used for the app to commit the update to the database
        public ActionResult CommitTagUpdate(string currentNFCTagValue)
        {
            var updateTransaction = db.TagUpdateTransaction
                .Include(x=>x.ActiveWine)
                .SingleOrDefault(x => x.new_tag_value == currentNFCTagValue);
            if (updateTransaction == null)
            {
                return Json(new { commitResult = "false"});
            }

            //update active wine record
            var wine = updateTransaction.ActiveWine;
            wine.readCount += 1;
            wine.NFCCurrentTag = updateTransaction.new_tag_value;
            wine.WineTransactionHistory += string.Format("{0} Scanned\r\n", DateTime.Now.ToString("dd-MM-yyyy"));
            db.Entry<ActiveWine>(wine).State = System.Data.EntityState.Modified;
            

            //store old record in the achieve
            var achieveRecord = new TagValueAchieve();
            achieveRecord.tag_value = updateTransaction.old_tag_value;
            achieveRecord.wine_id = wine.WID;
            db.TagValueAchieve.Add(achieveRecord);

            //remove update transaction record
            db.TagUpdateTransaction.Remove(updateTransaction);

            //save all update
            db.SaveChanges();

            return Json(new { commitResult = "true" });
        }

        // used for the app to buy for the wine
        public ActionResult ConsumerBuyForWine(string NFCTag)
        {
            var wine = db.ActiveWines
                .Where(x => x.WineStatusID != 2)
                .SingleOrDefault(x => x.NFCCurrentTag == NFCTag);
            if (wine == null)
                return Json(new { wine="", isBuySuccess = false, reason = "Cannot buy invalid wine" });

            wine.WineStatusID = 2; // invalidate wine
            wine.WineTransactionHistory += string.Format("{0} Sold\r\n", DateTime.Now.ToString("dd-MM-yyyy"));
            db.SaveChanges();

            var retWine = formatAsOutputJSON(wine);

            return Json(new { wine = retWine, isBuySuccess = true, reason = "" });
        }

        // deprecated
        /*
        public ActionResult writingTagForWineID(int id)
        {
            var wine = db.ActiveWines.Find(id);
            wine.TagStatusID = 1;  
            db.SaveChanges();
            return Json(new { });
        }
        */
        public ActionResult imageUpload(HttpPostedFileBase file)
        {
            if (Request.HttpMethod == "POST")
            {
                int WID;
                try
                {
                    WID = Convert.ToInt32(Request.Form["WID"]);
                    /*try to find if WID exist */
                    bool isExist = db.ActiveWines.Any(x => x.WID == WID);
                    if (!isExist)
                    {
                        throw new Exception();
                    }
                }
                catch (Exception e)
                {
                    ViewData["error"] = "WID does not exist";
                    return View();
                }

                if (file.ContentLength > 0)
                {
                    ActiveWine aw = db.ActiveWines.Single(x => x.WID == WID);

                    string ext = System.IO.Path.GetExtension(file.FileName); //get extension name e.g. .jpg
                    //var fileName = Path.GetFileName(file.FileName);
                    var fileName = string.Format("{0}{1}", WID, ext); //create the filename e.g. 1.jpg 
                    var path = Path.Combine(Server.MapPath("~/img/"), fileName); //get the physical path like C:/img/1.jpg
                    file.SaveAs(path); //save to that path

                    //get the virtual path for the app to show
                    string urlPath = Url.Content("~/img/" + fileName);

                    //update the database record
                    aw.WinePic = urlPath;
                    db.SaveChanges();

                    //redirect to index on success
                    return RedirectToAction("Index", "Home", null);
                }
            }

            return View();

        }
    }
}
