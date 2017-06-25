using System;
using System.Web.Mvc;
using DeependAncestry.Models;
using PagedList;

namespace DeependAncestry.Controllers
{
    public class AdvancedController : Controller
    {
        // GET: Advanced
        public ActionResult Index(string searchName, string direction, int? page, bool? isMale, bool? isFemale)
        {
            if (searchName == String.Empty)
            {
                ModelState.AddModelError("searchName", "Name is required.");
            }
            if (!string.IsNullOrEmpty(searchName))
            {

                return View(AncestryData.SearchPeopleByDirection(searchName, direction, isMale, isFemale, page));
            }

            return View(new PagedList<People>(null, 1, 1));
        }
    }
}