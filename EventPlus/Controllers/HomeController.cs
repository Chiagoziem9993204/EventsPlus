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

        // ACTION FOR HOME PAGE
        public ActionResult Index()
        {
            return View();
        }


        // ACTION FOR LOGOUT
        // WHEN A USER LOGS OUT, THIS CLEARS THE SESSION (REMOVES SAVED VARIABLES, USERTYPE, AND IDS) AND THEN REDIRECTS TO HOME PAGE
        public ActionResult Logout()
        {

            Session.Clear();

            return RedirectToAction("Index");
        }

        // ACTION FOR SIGN IN
        public ActionResult SignIn()
        {

            return View();
        }


        // THIS HANDLES WHEN A USER FILLS THE SIGN IN FORM ON THE SIGN IN PAGE
        // IT USES THE POST METHOD ON THE FORM
        // ADMIN CAN LOG IN WITH THE CUSTOM CREDENTIALS PROVIDED BELOW (EMAIL => Admin, PASSWORD = Administrator123)

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

        // ACTION FOR SIGN UP
        public ActionResult SignUp()
        {

            return View();
        }

        
        // ACTION FOR SIGN UP (POST METHOD)
        // THIS HANDLES WHEN A USER FILLS THE REGISTRATION FORM
        // IT SAVES THE USER TO THE DATABASE AND REDIRECTS TO THE EVENTS PAGE
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