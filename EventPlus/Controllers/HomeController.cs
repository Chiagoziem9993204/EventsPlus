using EventPlus.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EventPlus.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Logout()
        {

            Session.Clear();

            return RedirectToAction("Index");
        }


        public ActionResult SignIn()
        {

            return View();
        }


        [HttpPost]
        public ActionResult SignIn(UserViewModel userViewModel)
        {
            if (userViewModel.Email == "Admin" && userViewModel.Password == "Administrator123")
            {
                Session["LoggedInUserType"] = "Admin";
                Session["LoggedInUserFirstName"] = "Admin";
                return RedirectToAction("AllEvents", "Event", new { area = "" });
            }


            EventPlusEntities db = new EventPlusEntities();
            User user = db.Users.SingleOrDefault(u => u.Email == userViewModel.Email);

            if (user != null && user.Password == userViewModel.Password)
            {
                if(user.Organizations.Count > 0)
                {
                    Session["LoggedInUserType"] = "Organizer";
                    Organization organization = db.Organizations.SingleOrDefault(x => x.UserID == user.ID);
                    Session["OrganizationID"] = organization.ID.ToString();
                } else
                {
                    Session["LoggedInUserType"] = "Attendee";
                    Attendee attendee = db.Attendees.SingleOrDefault(x => x.UserID == user.ID);
                    Session["AttendeeID"] = attendee.ID.ToString();

                }
                Session["LoggedInUserFirstName"] = user.FName;
                Session["ID"] = user.ID.ToString();
                return RedirectToAction("AllEvents", "Event", new { area = "" });
            }

            return View();
        }

        public ActionResult SignUp()
        {

            return View();
        }

        

        [HttpPost]
        public ActionResult SignUp(UserViewModel userViewModel)
        {
            try
            {
                EventPlusEntities db = new EventPlusEntities();
                User user = new User();

                user.FName = userViewModel.FName;
                user.LName = userViewModel.LName;
                user.Email = userViewModel.Email;
                user.Password = userViewModel.Password;
                user.Address = userViewModel.Address;
                user.Phone = userViewModel.Phone;
                user.Deleted = 0;
                if(userViewModel.Gender == Gender.Female)
                {
                    user.Gender = "Female";
                }
                else
                {
                    user.Gender = "Male";
                }
                user.DateOfBirth = userViewModel.DateOfBirth;

                if (db.Users.SingleOrDefault(x=>x.Email==userViewModel.Email) == null)
                {
                    db.Users.Add(user);
                    db.SaveChanges();

                    int lastUserID = user.ID;

                    Attendee attendee = new Attendee();
                    attendee.UserID = lastUserID;
                    attendee.NoOfEventsAttended = 0;
                    db.Attendees.Add(attendee);
                    db.SaveChanges();

                    if (userViewModel.OrganizationName != null && userViewModel.OrganizationName != "")
                    {
                        Organization organization = new Organization();
                        organization.UserID = lastUserID;
                        organization.Name = userViewModel.OrganizationName;
                        db.Organizations.Add(organization);
                        db.SaveChanges();
                    }

                    return RedirectToAction("SignIn");
                }
               
            }
            catch (Exception ex)
            {

                throw ex;
            }
            return View();
        }
    }
}