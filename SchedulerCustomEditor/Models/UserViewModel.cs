using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SchedulerCustomEditor.Models
{
    public class UserViewModel 
    {

        public int UserID { get; set; }
        public string FacebookID { get; set; }
       [Required]      
        public string EmailId { get; set; }
         [Required]      
        public string Password { get; set; }

         public string UserName { get; set; }
         

        public virtual ICollection<Meeting> Meetings { get; set; }
        public User ToEntity()
        {
            var user = new User
            {
                UserID = UserID,
                FacebookID = FacebookID,
                EmailId = EmailId,
                Password = Password ,
                Meetings=Meetings
            };

            return user;
        } 
    }
}