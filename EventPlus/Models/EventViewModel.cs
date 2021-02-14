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
        public int TicketQuantity { get; set; }
        public System.DateTime ScheduledDateTime { get; set; }
        public IsRecurring IsRecurring { get; set; }
        public string Location { get; set; }
        public string Link { get; set; }
        public string Description { get; set; }

        public int IsDeleted { get; set; }
        public int OrganizationID { get; set; }

        public int TicketPrice { get; set; }

        public int GetEventRecurringValue(IsRecurring isRecurring)
        {
            if (isRecurring == IsRecurring.Yes)
            {
                return 1;
            }
            return 0;
        }

        public IsRecurring SetEventRecurringValue(int value)
        {
            if (value == 1)
            {
                return IsRecurring.Yes;
            }
            return IsRecurring.No;
        }
    }

    public enum IsRecurring
    {
        Yes,
        No
    }
}