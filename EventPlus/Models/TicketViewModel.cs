using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EventPlus.Models
{
    public class TicketViewModel
    {
        public int ID { get; set; }
        public int EventID { get; set; }
        public string TicketName { get; set; }
        public int TicketPrice { get; set; }
    }
}