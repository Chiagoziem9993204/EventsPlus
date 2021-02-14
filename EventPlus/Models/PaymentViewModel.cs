using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EventPlus.Models
{
    public class PaymentViewModel
    {
        public int ID { get; set; }
        public int TicketID { get; set; }
        public int AttendeeID { get; set; }
        public System.DateTime PaymentDateTime { get; set; }
    }
}