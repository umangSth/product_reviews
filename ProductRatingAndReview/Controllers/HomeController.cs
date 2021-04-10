using ProductRatingAndReview.Models;
using ProductRatingAndReview.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace ProductRatingAndReview.Controllers
{
    public class HomeController : Controller
    {
        ProductReviewDBEntities _db = new ProductReviewDBEntities();
        public ActionResult Index()
        {
            return View(_db.ProductTBs.ToList());
        }


        // this is Display Product section, according to the good, bad and worst
        // start-------------------------------------------------
        public PartialViewResult All()
        {
            return PartialView("_Product", _db.ProductTBs.ToList());
        }

        public PartialViewResult GoodProduct()
        {
            List<ProductTB> pt = ExtractProduct(1);
            return PartialView("_Product", pt);            
        }

        public PartialViewResult BadProduct()
        {
            List<ProductTB> pt = ExtractProduct(2);
            return PartialView("_Product", pt);
        }

        public PartialViewResult WorstProduct()
        {
            List<ProductTB> pt = ExtractProduct(3);
            return PartialView("_Product", pt);
        }

        private List<ProductTB> ExtractProduct(int catId)
        {
            var goodProduct = _db.AggregateReviewTBs.Where(x => x.CategoryId == catId).Select(p => p.ProductId).ToArray();
            List<ProductTB> pt = new List<ProductTB>();
            foreach (var item in goodProduct)
            {
                var oneItem = _db.ProductTBs.Where(x => x.ProductId == item).FirstOrDefault();
                pt.Add(oneItem);
            }

            return pt;
        }
        //end ------------------------------------------

        // this is for adding the product
        //start ---------------------------------------
        [Authorize(Roles = "Admin")]
        public ActionResult AddProduct()
        {
            return View();
        }
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public ActionResult AddProduct(ProductViewModel pv)
        {
            ProductTB pt = new ProductTB();
            pt.ProductName = pv.ProductName;
            pt.ProductDetail = pv.ProductDetail;
            pt.Price = pv.Price;
            pt.ProductImageUrl = pv.ProductImageUrl;
            _db.ProductTBs.Add(pt);
            _db.SaveChanges();
            return RedirectToAction("Index", "Home");
        }
        //end ---------------------------------------

        // this is the signup section, login section and logout section
        //start ---------------------------------------
        public ActionResult Signup()
        {
            return View();
        }

       [HttpPost]
       public ActionResult Signup(Signup su)
        {
            UserTB ut = new UserTB();            
            ut.UserName = su.UserName;
            ut.Email = su.Email;
            ut.Password = su.Password;
            _db.UserTBs.Add(ut);
            _db.SaveChanges();

            UserRole ur = new UserRole();
            ur.UserId = ut.UserId;
            ur.RoleId = 2;
            _db.UserRoles.Add(ur);
            _db.SaveChanges();           
            return RedirectToAction("Index", "Home");
        }     


        [ChildActionOnly]
        public ActionResult RenderLogin()
        {
            return PartialView("_Login");
        }
        [HttpPost]
        public ActionResult RenderLogin(LoginViewModel l, string ReturnUrl = "")
        {
            var users = _db.UserTBs.Where(a => a.UserName == l.UserName && a.Password == l.Password).FirstOrDefault();
            if (users != null)
            {
                Session.Add("UserId", users.UserId);
                Session.Add("UserName", users.UserName);
                FormsAuthentication.SetAuthCookie(l.UserName, l.RememberMe);
                if (Url.IsLocalUrl(ReturnUrl))
                {
                    return Redirect(ReturnUrl);
                }
                else
                {                   
                    return PartialView("_Login");
                }
            }
            else
            {
                ModelState.AddModelError("", "Invalid User");
            }
            return PartialView("_Login",l);
        }

        [Authorize]
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Index", "Home");
        }
        //end ---------------------------------------


        


        //this will return the detail page
        //start ------------------------------
        public ActionResult DetailPage(int Id)
        {
            ProductReviewViewModel pvm = new ProductReviewViewModel();
            ProductTB tb = new ProductTB();
            List<ReviewTB> rtb = new List<ReviewTB>();
            List<ReviewViewModel> rv = new List<ReviewViewModel>();
            if (Id != 0)
            {
                tb = _db.ProductTBs.Where(p => p.ProductId == Id).FirstOrDefault();
                rtb = _db.ReviewTBs.Where(p => p.ProductId == Id).ToList();
            }
            else
            {
                tb = _db.ProductTBs.Where(p => p.ProductId == 4).FirstOrDefault();
                rtb = _db.ReviewTBs.Where(p => p.ProductId == 4).ToList();
            }

            pvm.ProductId = tb.ProductId;
            pvm.ProductName = tb.ProductName;
            pvm.ProductDetail = tb.ProductDetail;
            pvm.ProductImageUrl = tb.ProductImageUrl;
            pvm.Price = tb.Price;
            foreach (var item in rtb)
            {
                rv.Add(new ReviewViewModel() { ReviewId = item.ReviewId, StarNumber = item.NumberOfStar, Message = item.Comment, UserName = item.UserTB.UserName, UserId = item.UserId, Date = item.Date });
            }
            pvm.RVM = rv;
            return View(pvm);
        }
        //end-----------------------


        // this section will add and delete the post/review
        //--------------------------------
        [Authorize]
        public JsonResult AddReview(ReviewViewModel rv)
        {
            int result = -1;
            if (rv.StarNumber < 0 || rv.Message == null)
            {
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            else
            {
                ReviewTB RT = new ReviewTB();
                RT.ProductId = rv.ProductId;
                RT.Comment = rv.Message;
                RT.NumberOfStar = rv.StarNumber;
                RT.Date = DateTime.Now;
                RT.UserId = rv.UserId;
                _db.ReviewTBs.Add(RT);
                result = _db.SaveChanges();
                AggregateReview(rv.ProductId);
                return Json(result, JsonRequestBehavior.AllowGet);
            }           
            
        }
        [Authorize]
        public JsonResult DeletePost(int id)
        {
            var a = _db.ReviewTBs.Where(x => x.ReviewId == id).FirstOrDefault();
            int? ProId = a.ProductId;
            _db.ReviewTBs.Remove(a);
            int result = _db.SaveChanges();
            AggregateReview(ProId);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        //end ------------------------------------------------


        //it aggregate the review star from review table and return average star 
        //start --------------------------------------
        public void AggregateReview(int? id)
        {
            var reviewArr = _db.ReviewTBs.Where(x => x.ProductId == id).Select(p=>p.NumberOfStar).ToArray();
            int? total = 0;
            for (int i = 0; i < reviewArr.Length; i++)
            {
                total = total + reviewArr[i];
            }
            float AvagerStar;
            if (total <= 0 || reviewArr.Length == 0)
            {
                AvagerStar = 0;
            }
            else
            {
                AvagerStar = (float)(total / reviewArr.Length);
            }
           
            int ? AggregateStar = Convert.ToInt32(Math.Ceiling(AvagerStar));
            
            int? Category = -1;
            if (AggregateStar == 5 || AggregateStar == 4)
            {
                Category = 1;
            }else if(AggregateStar == 2 || AggregateStar == 3)
            {
                Category = 2;
            }else if( AggregateStar == 1 || AggregateStar == 0)
            {
                Category = 3;
            }
            else
            {
                Category = 4;
            }
            var Agg = _db.AggregateReviewTBs.Where(x => x.ProductId == id).FirstOrDefault();
            if(Agg == null)
            {
                AggregateReviewTB agt = new AggregateReviewTB();
                agt.ProductId = id;
                agt.AggregateStar = AggregateStar;
                agt.CategoryId = Category;
                _db.AggregateReviewTBs.Add(agt);
                _db.SaveChanges();
            }
            else
            {
                Agg.CategoryId = Category;
                Agg.AggregateStar = AggregateStar;
                _db.SaveChanges();
            }
            
        }
        //end --------------------------


        //this code is for exporting the csv file for review table
        //start ------------------------------------------
        [Authorize(Roles = "Admin")]
        public void ExportCSV_Report(int CategoryId)
        {
            var sb = new StringBuilder();
            List<CSVExportViewModel> csv = new List<CSVExportViewModel>();
            var list = ReturnProductWithCate(CategoryId);
            foreach (var item in list)
            {
                csv.Add(new CSVExportViewModel() { ProductId = item.ProductId, ProductName = item.ProductTB.ProductName, Comment = item.Comment, Date = item.Date, NumberOfStar = item.NumberOfStar, UserName = item.UserTB.UserName });
            }
            sb.AppendFormat("{0},{1},{2},{3},{4},{5},{6}", "Product Id", "Product Name", "User Name", "Number of Star", "Comment", "Date", Environment.NewLine);
            foreach (var item in csv)
            {
                sb.AppendFormat("{0},{1},{2},{3},{4},{5},{6}", item.ProductId, item.ProductName, item.UserName, item.NumberOfStar, item.Comment, item.Date, Environment.NewLine);
            }
            var response = System.Web.HttpContext.Current.Response;
            response.BufferOutput = true;
            response.Clear();
            response.ClearHeaders();
            response.ContentEncoding = Encoding.Unicode;
            response.AddHeader("content-disposition", "attachment;filename=Review.CSV ");
            response.ContentType = "text/plain";
            response.Write(sb.ToString());
            response.End();
        }
       public List<ReviewTB> ReturnProductWithCate(int CategoryId)
        {
            int?[] ProArr;
            if(CategoryId == 1 || CategoryId == 2|| CategoryId == 3)
            {
                ProArr = _db.AggregateReviewTBs.Where(x => x.CategoryId == CategoryId).Select(p => p.ProductId).ToArray();
            }
            else
            {
                ProArr = _db.AggregateReviewTBs.Select(p => p.ProductId).ToArray();
            }
            List<ReviewTB> rt = new List<ReviewTB>();
            foreach (var item in ProArr)
            {
                var review = _db.ReviewTBs.Where(x => x.ProductId == item).ToList();
                foreach (var pro in review)
                {
                    rt.Add(pro);
                }
            }
            return rt;
        }
        //end -------------------------------------------------------------

    }
}