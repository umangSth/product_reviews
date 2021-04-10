using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ProductRatingAndReview.Models.ViewModel
{
    public class ReviewViewModel
    {        
        public int ProductId { get; set; }
        public int ReviewId { get; set; }
        public int? StarNumber { get; set; }
        public string Message { get; set; }
        public int? UserId { get; set; }
        public string UserName { get; set; }
        public DateTime? Date { get; set; }
    }
    public class ProductReviewViewModel
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string ProductDetail { get; set; }
        public decimal? Price { get; set; }
        public string ProductImageUrl { get; set; }
        public List<ReviewViewModel> RVM { get; set; }
    }

    public class ProductViewModel
    {
        public int ProductId { get; set; }
        [Required(ErrorMessage = "Please Enter Product")]
        [Remote("IsProductNameExist", "Validation", ErrorMessage = "Product already exist, Enter another")]
        public string ProductName { get; set; }
        [Required(ErrorMessage = "Please Enter Details")]
        public string ProductDetail { get; set; }
        [Required(ErrorMessage = "Please Enter Price")]
        public decimal? Price { get; set; }
        [Required(ErrorMessage = "Please Enter Product Image Url")]
        [RegularExpression("((http|https)://)(www.)?[a-zA-Z0-9@:%._\\+~#?&//=]{2,256}\\.[a-z]{2,6}\\b([-a-zA-Z0-9@:%._\\+~#?&//=]*)", ErrorMessage = "Enter correct image url!!!!!!")]
        public string ProductImageUrl { get; set; }
    }

    public class LoginViewModel
    {
        public int UserId { get; set; }
        [Required(ErrorMessage = "Please Enter Username")]
        public string UserName { get; set; }
        [Required(ErrorMessage = "Please Enter Password")]
        public  string Password { get; set; }
        public bool RememberMe { get; set; }
    }

    public class Signup
    {
        public int UserId { get; set; }
        [Required(ErrorMessage = "Please Enter Username")]
        [Remote("IsUserNameExist", "Validation", ErrorMessage = "UserName already exist, Enter another")]
        public string UserName { get; set; }
        [Required(ErrorMessage = "Please Enter password")]
        public string Password { get; set; }
        [Required(ErrorMessage = "Re-Confirm your Password")]
        [System.ComponentModel.DataAnnotations.Compare("Password")]
        public string RePassword { get; set; }
        [Required(ErrorMessage = "Please Enter Email")]
        public string Email { get; set; }
    }
    public class CSVExportViewModel
    {
        public int? ProductId { get; set; }
        public  string ProductName { get; set; }
        public int? NumberOfStar { get; set; }
        public string Comment { get; set; }
        public DateTime? Date { get; set; }
        public string UserName { get; set; }
    }

}