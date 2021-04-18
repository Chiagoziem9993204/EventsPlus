using EventPlus.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;


namespace EventPlus.Controllers
{
    public class UserController : Controller
    {
    

        // ACTION FOR ALL USERS
        // IT GETS THE USERS FROM THE DATABASE VIA THE ENTITY MODEL
        // AND THEN IT IS PARSED AS A LIST OF USERVIEW MODEL TO THE VIEW
        // THERE IS A CHECK TO MAKE SURE THE USER HASN'T BEEN DELETED BY THE ADMIN
        public ActionResult AllUsers()
        {
            EventPlusEntities db = new EventPlusEntities();
            List<User> usersList = db.Users.ToList();
            UserViewModel userViewModel = new UserViewModel();
            List<UserViewModel> userViewModelsList = usersList.Where(x=>x.Deleted==0).Select(
                x=> new UserViewModel {
                ID=x.ID,
                Email=x.Email,
                Password=x.Password,
                FName=x.FName,
                LName=x.LName,
                Address=x.Address,
                Phone=x.Phone,
                Gender=userViewModel.SetUserGenderValue(x.Gender),
                DateOfBirth=x.DateOfBirth,
                }
                ).ToList();

            return View(userViewModelsList);
        }


        // ACTION FOR USER DETAIL
        // IT TAKES IN A SINGLE USER ID
        // GETS THE DETAILS OF THAT PARTICULAR USER FROM THE DB
        // DESERIALIZES WITH THE USERVIEW MODEL AND PARSED TO THE VIEW
        public ActionResult UserDetail(int userID)
        {
            EventPlusEntities db = new EventPlusEntities();
            User singleUser = db.Users.SingleOrDefault(x => x.ID == userID);
            UserViewModel userViewModel = new UserViewModel();

            userViewModel.ID = singleUser.ID;
            userViewModel.Email = singleUser.Email;
            userViewModel.FName = singleUser.FName;
            userViewModel.LName = singleUser.LName;
            userViewModel.Address = singleUser.Address;
            userViewModel.Phone = singleUser.Phone;
            userViewModel.Gender = userViewModel.SetUserGenderValue( singleUser.Gender);
            userViewModel.DateOfBirth = singleUser.DateOfBirth;

            if(singleUser.Organizations.Count > 0)
            {
                userViewModel.OrganizationName = db.Organizations.SingleOrDefault(x => x.UserID == userID).Name;
            }


            return View(userViewModel);
        }


        // ACTION FOR USER DETAIL (POST METHOD)
        // THIS IS USED BY THE ADMIN TO UPDATE A PARTICULAR USER
        // IT TAKES IN THE UPDATED USERVIEWMODEL FROM THE FORM
        // AND IT REDIRECTS TO ALL USERS VIEW
        [HttpPost]
        public ActionResult UserDetail(UserViewModel userViewModel)
        {
            EventPlusEntities db = new EventPlusEntities();

            User user = db.Users.SingleOrDefault(x => x.ID == userViewModel.ID);

            user.ID = userViewModel.ID;
            if(user.Email != userViewModel.Email && db.Users.SingleOrDefault(u => u.Email == userViewModel.Email) == null)
            {
                user.Email = userViewModel.Email;
            }
           
            user.FName = userViewModel.FName;
            user.LName = userViewModel.LName;
            user.Address = userViewModel.Address;
            user.Phone = userViewModel.Phone;
            user.Gender = userViewModel.GetUserGenderValue(userViewModel.Gender);
            if(userViewModel.DateOfBirth.Year != 1)
            {
                user.DateOfBirth = userViewModel.DateOfBirth;
            }
            db.SaveChanges();
            if (userViewModel.OrganizationName != null && userViewModel.OrganizationName != "")
            {
                Organization organization = db.Organizations.SingleOrDefault(x => x.UserID == userViewModel.ID);
                organization.Name = userViewModel.OrganizationName;
                db.SaveChanges();
            }

            return RedirectToAction("AllUsers");
        }

        // ACTION FOR DELETE USER
        // TAKES IN A SINGLE USER ID
        // GETS THE USER IN THE DATABASE AND THEN DELETES (UPDATE THE DETELED ROW)
        // THEN REDIRECTS TO ALL USERS PAGE
        public ActionResult DeleteUser(int userID)
        {
            EventPlusEntities db = new EventPlusEntities();

            User user = db.Users.SingleOrDefault(x => x.ID == userID);
            user.Deleted = 1;
            db.SaveChanges();

            return RedirectToAction("AllUsers");

        }

        // ACTION FOR CREATE USER
        public ActionResult CreateUser()
        {

            return View();
        }


        // ACTION FOR CREATE USER (POST METHOD)
        // TAKES IN USERVIEWMODEL FROM THE FORM
        // STORES THE USER TO THE DATABASE
        // AND REDIRECTS TO ALL USERS SCREEN
        // ATTENDEES AND ORGANIZATIONS/ORGANIZERS ARE DIFFERENTIATED BY THE ORGANIZATION NAME FIELD
        // IF A USER FILLS THIS FIELD, THEN HE/SHE IS AN ORGANIZATION WHICH CREATES A CORRESPONDING ORGANIZATION FOR THE USER IN THE DATABASE
        // ELSE THE USER IS AN ATTENDEE WHICH CREATES AN ATTENDEE IN THE DATABASE FOR THE USER
        [HttpPost]
        public ActionResult CreateUser(UserViewModel userViewModel)
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
                if (userViewModel.Gender == Gender.Female)
                {
                    user.Gender = "Female";
                }
                else
                {
                    user.Gender = "Male";
                }
                user.DateOfBirth = userViewModel.DateOfBirth;

                if (db.Users.SingleOrDefault(x => x.Email == userViewModel.Email) == null)
                {
                    db.Users.Add(user);
                    db.SaveChanges();

                    int lastUserID = user.ID;

                    Attendee attendee = new Attendee();
                    attendee.UserID = lastUserID;
                    attendee.NoOfEventsAttended = 0;
                    db.Attendees.Add(attendee);
                    db.SaveChanges();

                    if (userViewModel.OrganizationName != null || userViewModel.OrganizationName != "")
                    {
                        Organization organization = new Organization();
                        organization.UserID = lastUserID;
                        organization.Name = userViewModel.OrganizationName;
                        db.Organizations.Add(organization);
                        db.SaveChanges();
                    }

                    return RedirectToAction("AllUsers");
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
