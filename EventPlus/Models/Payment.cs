//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace EventPlus.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Payment
    {
        public int ID { get; set; }
        public int TicketID { get; set; }
        public int AttendeeID { get; set; }
        public System.DateTime PaymentDateTime { get; set; }
    
        public virtual Attendee Attendee { get; set; }
        public virtual Ticket Ticket { get; set; }
    }
}
