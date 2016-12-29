using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace SchedulerCustomEditor.Models
{
    public class SchedulerLoginService
    {
        private SampleEntities db;

        public SchedulerLoginService(SampleEntities context)
        {
            db = context;
        }

        public SchedulerLoginService()
            : this(new SampleEntities())
        {
        }

        public UserViewModel GetUser_ByEmailId_Password(string EmailId, string Password, string facebookId)
        {
            UserViewModel model = null;

            try
            {
                if (!string.IsNullOrEmpty(EmailId) && !string.IsNullOrEmpty(Password))
                {
                    SchedulerCustomEditor.Models.User user = db.Users.Where(s => s.EmailId == EmailId && s.Password == Password).FirstOrDefault();
                    if (user != null)
                    {
                        model = new UserViewModel
                        {
                            UserID = user.UserID,
                            FacebookID = user.FacebookID,
                            EmailId = user.EmailId,
                            Password = user.Password,
                            UserName = user.FirstName + " " + user.LastName
                        };
                    }

                }
                else if (!string.IsNullOrEmpty(facebookId) && db.Users.Any(u => u.FacebookID.Equals(facebookId)))
                {
                    model = db.Users.Where(s => s.FacebookID == facebookId).ToList().Select(u => new UserViewModel
                    {
                        UserID = u.UserID,
                        UserName = u.FirstName + " " + u.LastName
                    }).FirstOrDefault();
                }

            }
            catch { }
            
            return model;
        }



    }
}