using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EventPlus.Models
{
    public class EventViewModel
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public int NoOfTickets { get; set; }
        public System.DateTime DateTime { get; set; }
        public Recurring Recurring { get; set; }
        public string Venue { get; set; }
        public string Link { get; set; }
        public string Description { get; set; }

        public int Deleted { get; set; }
        public int OrganizationID { get; set; }

        public int TicketPrice { get; set; }

        public int GetEventRecurringValue(Recurring isRecurring)
        {
            if (isRecurring == Recurring.Yes)
            {
                return 1;
            }
            return 0;
        }

        public Recurring SetEventRecurringValue(int value)
        {
            if (value == 1)
            {
                return Recurring.Yes;
            }
            return Recurring.No;
        }
    }

    public enum Recurring
    {
        Yes,
        No
    }
}