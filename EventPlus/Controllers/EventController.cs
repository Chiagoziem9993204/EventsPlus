using EventPlus.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EventPlus.Controllers
{
    public class EventController : Controller
    {

        // GET: Event
        public ActionResult Index()
        {
            return View();
        }


        public ActionResult CreateEvent()
        {

            EventPlusEntities db = new EventPlusEntities();
            List<Organization> list = db.Organizations.ToList();
            ViewBag.OrganizationList = new SelectList(list, "ID", "Name");
            return View();
        }

        [HttpPost]
        public ActionResult CreateEvent(EventViewModel eventViewModel)
        {

            try
            {
                EventPlusEntities db = new EventPlusEntities();
                Event eventEntityModel = new Event();

                List<Organization> list = db.Organizations.ToList();
                ViewBag.OrganizationList = new SelectList(list, "ID", "Name");

                eventEntityModel.Name = eventViewModel.Name;
                eventEntityModel.Type = eventViewModel.Type;
                eventEntityModel.NoOfTickets = eventViewModel.NoOfTickets;
                eventEntityModel.DateTime = eventViewModel.DateTime;
                eventEntityModel.Recurring =
                    eventViewModel.GetEventRecurringValue(eventViewModel.Recurring);
                eventEntityModel.Venue = eventViewModel.Venue;
                eventEntityModel.Link = eventViewModel.Link;
                eventEntityModel.Description = eventViewModel.Description;
                eventEntityModel.Deleted = 0;
                if (Session["LoggedInUserType"] == "Admin")
                {
                    eventEntityModel.OrganizationID = eventViewModel.OrganizationID;

                } else
                {
                    eventEntityModel.OrganizationID = Int32.Parse(Session["OrganizationID"].ToString());
                }

                db.Events.Add(eventEntityModel);
                db.SaveChanges();
                int lastestEvtId = eventEntityModel.ID;

                Ticket ticket = new Ticket();
                ticket.TicketPrice = eventViewModel.TicketPrice;
                ticket.EventID = lastestEvtId;
                ticket.TicketName = eventViewModel.Name + " Ticket";
                db.Tickets.Add(ticket);
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                throw ex;

            }


            return RedirectToAction("AllEvents");
        }

        public ActionResult AllEvents()
        {
            EventPlusEntities db = new EventPlusEntities();
            List<Event> eventsList = db.Events.ToList();
            EventViewModel eventViewModel = new EventViewModel();
            List<EventViewModel> eventViewModelsList = eventsList.Where(x=>x.Deleted==0).Select(x => new EventViewModel { ID = x.ID, Name = x.Name, Type = x.Type, NoOfTickets = x.NoOfTickets, DateTime = x.DateTime, Recurring = eventViewModel.SetEventRecurringValue(x.Recurring), Venue = x.Venue, Link = x.Link, Description = x.Description, OrganizationID = x.OrganizationID }).ToList();
            return View(eventViewModelsList);
        }


        public ActionResult EventDetail(int eventID)
        {
            EventPlusEntities db = new EventPlusEntities();
            Event singleEvent = db.Events.SingleOrDefault(x => x.ID == eventID);
            EventViewModel eventViewModel = new EventViewModel();


            List<Organization> list = db.Organizations.ToList();
            ViewBag.OrganizationList = new SelectList(list, "ID", "Name");

            eventViewModel.ID = singleEvent.ID;
            eventViewModel.Name = singleEvent.Name;
            eventViewModel.Type = singleEvent.Type;
            eventViewModel.NoOfTickets = singleEvent.NoOfTickets;
            eventViewModel.DateTime = singleEvent.DateTime;
            eventViewModel.Recurring = eventViewModel.SetEventRecurringValue(singleEvent.Recurring);
            eventViewModel.Venue = singleEvent.Venue;
            eventViewModel.Link = singleEvent.Link;
            eventViewModel.Description = singleEvent.Description;
            eventViewModel.OrganizationID = singleEvent.OrganizationID;
            eventViewModel.TicketPrice = db.Tickets.SingleOrDefault(x => x.EventID == eventID).TicketPrice;

            return View(eventViewModel);
        }

        [HttpPost]
        public ActionResult EventDetail(EventViewModel eventViewModel)
        {
            EventPlusEntities db = new EventPlusEntities();

            Event eventEntityModel = db.Events.SingleOrDefault(x => x.ID == eventViewModel.ID);

            List<Organization> list = db.Organizations.ToList();
            ViewBag.OrganizationList = new SelectList(list, "ID", "Name");

            eventEntityModel.Name = eventViewModel.Name;
            eventEntityModel.Type = eventViewModel.Type;
            eventEntityModel.NoOfTickets = eventViewModel.NoOfTickets;
            if (eventViewModel.DateTime.Year != 1)
            {
                eventEntityModel.DateTime = eventViewModel.DateTime;
            }

            eventEntityModel.Recurring = eventViewModel.GetEventRecurringValue(eventViewModel.Recurring);
            eventEntityModel.Venue = eventViewModel.Venue;
            eventEntityModel.Link = eventViewModel.Link;
            eventEntityModel.Description = eventViewModel.Description;
            if (Session["LoggedInUserType"] == "Admin")
            {
                eventEntityModel.OrganizationID = eventViewModel.OrganizationID;

            }
            else
            {
                eventEntityModel.OrganizationID = Int32.Parse(Session["OrganizationID"].ToString());
            }
            db.SaveChanges();

            Ticket ticket = db.Tickets.SingleOrDefault(x => x.EventID == eventViewModel.ID);
            ticket.TicketPrice = eventViewModel.TicketPrice;

            db.SaveChanges();

            return RedirectToAction("AllEvents");
        }

        public ActionResult DeleteEvent(int eventID)
        {
            EventPlusEntities db = new EventPlusEntities();

            Event eventEntityModel = db.Events.SingleOrDefault(x => x.ID == eventID);

            eventEntityModel.Deleted = 1;
            db.SaveChanges();

            return RedirectToAction("AllEvents");

        }


        public ActionResult PayForEvent(int eventID)
        {
            EventPlusEntities db = new EventPlusEntities();
            int attendeeID = Int32.Parse(Session["AttendeeID"].ToString());

            Payment existingPayment = db.Payments.SingleOrDefault(x => x.AttendeeID == attendeeID);

            if (existingPayment == null)
            {
                Ticket ticket = db.Tickets.SingleOrDefault(x => x.EventID == eventID);

                Payment payment = new Payment();

                payment.TicketID = ticket.ID;
                payment.AttendeeID = attendeeID;
                payment.PaymentDateTime = System.DateTime.Now;

                db.Payments.Add(payment);
                db.SaveChanges();
            }

         

            return RedirectToAction("AllEvents");

        }
    }
}