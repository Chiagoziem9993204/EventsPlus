using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EventPlus.Models
{
    public class AttendeeViewModel
    {
        public int ID { get; set; }
        public int NoOfEventsAttended { get; set; }
        public int UserID { get; set; }
    }
}