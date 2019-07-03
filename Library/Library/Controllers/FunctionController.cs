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
    }
}