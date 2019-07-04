using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Library.Controllers
{
    public class FunctionController : Controller
    {
        Models.BookService bookservice = new Models.BookService();
        // GET: Function
        public ActionResult Index()
        {
            ViewBag.BookClass = this.bookservice.GetClassTable("");
            ViewBag.Borrowpeople = this.bookservice.GetUserTable("");
            ViewBag.Borrowstatus = this.bookservice.GetStatusTable("");
            return View("");
        }

        [HttpPost()]
        public ActionResult Index(Models.BookSearch arg)
        {
            ViewBag.BookClass = this.bookservice.GetClassTable("");
            ViewBag.Borrowpeople = this.bookservice.GetUserTable("");
            ViewBag.Borrowstatus = this.bookservice.GetStatusTable("");
            ViewBag.SearchList = this.bookservice.GetBookData(arg);
            return View("Index");
        }

        [HttpPost()]
        public JsonResult DeleteBook(int BookId)
        {
            bookservice.DeleteBook(BookId);
            return this.Json(true);
        }

        [HttpGet()]
        public ActionResult InsertBook()
        {
            ViewBag.BookClass = this.bookservice.GetClassTable("");
            ViewBag.InsertResult = false;
            return View("InsertBook");
        }

        [HttpPost()]
        public ActionResult InsertBook(Models.BOOK arg)
        {
            ViewBag.BookClass = this.bookservice.GetClassTable("");
            ViewBag.InsertResult = bookservice.InsertBookInfo(arg);
            if (arg.BookName == null || arg.Author == null || arg.Publisher == null || arg.Introduction == null || arg.BuyDate == null || arg.BookClassId == null)
            {
                return View("InsertBook");
            }
            else
            {
                ModelState.Clear();
                return View("InsertBook");
            }         
        }
    }
}