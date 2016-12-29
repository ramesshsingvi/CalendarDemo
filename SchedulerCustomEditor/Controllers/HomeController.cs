namespace SchedulerCustomEditor.Controllers
{
    using Kendo.Mvc.Extensions;
    using Kendo.Mvc.UI;
    using SchedulerCustomEditor.Models;
    using System;
    using System.Web;
    using System.Web.Mvc;
    using System.IO;
    using System.Collections.Generic;
    public class HomeController : Controller
    {
        private SchedulerMeetingService meetingService; 

        public HomeController()
        {
            this.meetingService = new SchedulerMeetingService();
        }

        public ActionResult Index()
        {
            ViewBag.Message = "Welcome!";
            return View(new MeetingViewModel());
        }

        public ActionResult About()
        {
            return View();
        }

        public virtual JsonResult Meetings_Read([DataSourceRequest] DataSourceRequest request)
        {
            return Json(meetingService.GetAll().ToDataSourceResult(request));
        }

        public virtual JsonResult Meetings_Destroy([DataSourceRequest] DataSourceRequest request, MeetingViewModel meeting)
        {
            if (ModelState.IsValid)
            {
                meetingService.Delete(meeting, ModelState);
            }

            return Json(new[] { meeting }.ToDataSourceResult(request, ModelState));
        }

        public virtual JsonResult Meetings_Create([DataSourceRequest] DataSourceRequest request, MeetingViewModel meeting)
        {
            if (ModelState.IsValid)
            { 
                meetingService.Insert(meeting, ModelState);
            }

            return Json(new[] { meeting }.ToDataSourceResult(request, ModelState));
        }

        public virtual JsonResult Meetings_Update([DataSourceRequest] DataSourceRequest request, MeetingViewModel meeting)
        {
            if (ModelState.IsValid)
            {
                meetingService.Update(meeting, ModelState);
            }

            return Json(new[] { meeting }.ToDataSourceResult(request, ModelState));
        }

        public ActionResult Save(IEnumerable<HttpPostedFileBase> files)
        {
            // The Name of the Upload component is "files"
            if (files != null)
            {
                foreach (var file in files)
                {
                    // Some browsers send file names with full path.
                    // We are only interested in the file name.
                    var fileName = Path.GetFileName(file.FileName);
                    var physicalPath = Path.Combine(Server.MapPath("~/App_Data/Uploads"), fileName);

                     file.SaveAs(physicalPath);
                }
            }

            // Return an empty string to signify success
            return Content("");
        }

        public ActionResult Remove(string[] fileNames)
        {
            // The parameter of the Remove action must be called "fileNames"

            if (fileNames != null)
            {
                foreach (var fullName in fileNames)
                {
                    var fileName = Path.GetFileName(fullName);
                    var physicalPath = Path.Combine(Server.MapPath("~/App_Data/Uploads"), fileName);

                    // TODO: Verify user permissions

                    if (System.IO.File.Exists(physicalPath))
                    {
                         System.IO.File.Delete(physicalPath);
                    }
                }
            }

            // Return an empty string to signify success
            return Content("");
        }



    }
}
