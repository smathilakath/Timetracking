using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TimeTracking.Controllers
{
    public class BaseController : Controller
    {
        /// <summary>
        /// Need to implement
        /// </summary>
        /// <returns></returns>
        public virtual ActionResult Time()
        {
            return PartialView("_Layout");
        }
    }
}