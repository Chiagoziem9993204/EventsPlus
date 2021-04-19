using EventPlus.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;

namespace EventPlus.Controllers
{
    public class EventController : Controller
    {

        // ACTION FOR CREATE EVENT
        // IT GETS ALL USERS THAT HASN'T BEEN DELETED FROM THE DATABASE
        // IT IS PASSED TO THE VIEW
        public ActionResult CreateEvent()
        {

            EventPlusEntities db = new EventPlusEntities();
            List<Organization> list = db.Organizations.ToList();
            ViewBag.OrganizationList = new SelectList(list, "ID", "Name");
            return View();
        }


        // ACTION FOR CREATE EVENT (POST METHOD)
        // IT IS USED BY ORGANIZERS AND ADMINS TO CREATE EVENTS
        // EVENT IS SAVED TO THE DATABASE
        // A CORRESPONDING TICKET ROW IS CREATED IN THE DATABASE FOR THE EVENT
        // HE/SHE IS THEN REDIRECTED TO ALL EVENTS PAGE
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


        // ACTION FOR ALL EVENTS
        // GETS ALL THE EVENTS FROM THE DATABASE
        // CHECKS IF ANY EVENT IS SEARCHED BY NAME, DESCRIPTION, VENUE
        // EVENTS ARE DESERIALIZED USING THE EVENTVIEWMODEL
        // PAGINATE RESULSTS AND SEND TO THE VIEW
        public ActionResult AllEvents(string searchString, int page = 1, int pageSize = 2)
        {
            EventPlusEntities db = new EventPlusEntities();
            List<Event> eventsList = db.Events.ToList();
            if (!String.IsNullOrEmpty(searchString))
            {
                eventsList = eventsList.Where(s => s.Name.Contains(searchString)
                                       || s.Description.Contains(searchString)
                                       || s.Venue.Contains(searchString)).ToList();
            }
            EventViewModel eventViewModel = new EventViewModel();
            List<EventViewModel> eventViewModelsList = eventsList.Where(x=>x.Deleted==0).Select(x => new EventViewModel { ID = x.ID, Name = x.Name, Type = x.Type, NoOfTickets = x.NoOfTickets, DateTime = x.DateTime, Recurring = eventViewModel.SetEventRecurringValue(x.Recurring), Venue = x.Venue, Link = x.Link, Description = x.Description, OrganizationID = x.OrganizationID }).ToList();
            PagedList<EventViewModel> model = new PagedList<EventViewModel>(eventViewModelsList, page, pageSize);
            return View(model);
        }


        // ACTION FOR EVENT DETAIL
        // TAKES IN SINGLE EVENT ID
        // GETS THE EVENT FROM THE DATABASE
        // DESERIALIZES THE EVENT WITH THE EVENTVIEWMODEL AND
        // SENDS THE EVENT TO THE VIEW TO BE DISPLAYED

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


        // ACTION FOR EVENT DETAIL (POST METHOD)
        // USED BY ADMIN AND ORGANIZER TO UPDATE AN EVENT
        // TAKES IN EVENTVIEWMODEL FROM THE FORM
        // UPDATES THE EVENT IN THE DATABASE
        // UPDATES THE CORRESPONDING EVENT TICKET TOO
        // REDIRECTS TO ALL EVENTS PAGE
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


        // ACTION FOR DELETE EVENT
        // EVENTS CAN OLY BE DELETED BY THE ORGANIZER OF THE EVENT AND THE ADMIN
        // TAKES IN THE EVENT ID
        // GETS THE EVENT IN THE DB AND DELETES IT (UPDATES THE DELETED FIELD)
        // REDIRECTS TO ALL EVENTS PAGE
        public ActionResult DeleteEvent(int eventID)
        {
            EventPlusEntities db = new EventPlusEntities();

            Event eventEntityModel = db.Events.SingleOrDefault(x => x.ID == eventID);

            eventEntityModel.Deleted = 1;
            db.SaveChanges();

            return RedirectToAction("AllEvents");

        }


        // ACTION FOR PAY EVENT
        // TAKES IN THE EVENT ID
        // GETS THE TICKET FOR THE EVENT
        // CREATES A PAYMENT ROW IN THE DATABASE FOR THE TICKET OF THE EVENT
        // REDIRECTS TO ALL EVENTS PAGE
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