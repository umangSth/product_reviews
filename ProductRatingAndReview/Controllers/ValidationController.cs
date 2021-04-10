using ProductRatingAndReview.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ProductRatingAndReview.Controllers
{
    public class ValidationController : Controller
    {
        /* GET: Validation
        public ActionResult Index()
        {
            return View();
        }*/
        ProductReviewDBEntities _db = new ProductReviewDBEntities();

        public JsonResult IsUserNameExist(string UserName)
        {
            bool isExist = _db.UserTBs.Where(u => u.UserName.Equals(UserName)).FirstOrDefault() == null;
            return Json(isExist, JsonRequestBehavior.AllowGet);
        }

        public JsonResult IsProductNameExist(string ProductName)
        {
            bool isExist = _db.ProductTBs.Where(u => u.ProductName.Equals(ProductName)).FirstOrDefault() == null;
            return Json(isExist, JsonRequestBehavior.AllowGet);
        }
    }
}