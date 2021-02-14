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
        // GET: User
        public ActionResult Index()
        {
            return View();
        }


        public ActionResult AllUsers()
        {
            EventPlusEntities db = new EventPlusEntities();
            List<User> usersList = db.Users.ToList();
            UserViewModel userViewModel = new UserViewModel();
            List<UserViewModel> userViewModelsList = usersList.Where(x=>x.IsDeleted==0).Select(
                x=> new UserViewModel {
                ID=x.ID,
                Email=x.Email,
                Password=x.Password,
                First_Name=x.First_Name,
                Last_Name=x.Last_Name,
                Address=x.Address,
                Phone=x.Phone,
                Gender=userViewModel.SetUserGenderValue(x.Gender),
                DateOfBirth=x.DateOfBirth,
                }
                ).ToList();

            return View(userViewModelsList);
        }

        public ActionResult UserDetail(int userID)
        {
            EventPlusEntities db = new EventPlusEntities();
            User singleUser = db.Users.SingleOrDefault(x => x.ID == userID);
            UserViewModel userViewModel = new UserViewModel();

            userViewModel.ID = singleUser.ID;
            userViewModel.Email = singleUser.Email;
            userViewModel.First_Name = singleUser.First_Name;
            userViewModel.Last_Name = singleUser.Last_Name;
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
           
            user.First_Name = userViewModel.First_Name;
            user.Last_Name = userViewModel.Last_Name;
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

        public ActionResult DeleteUser(int userID)
        {
            EventPlusEntities db = new EventPlusEntities();

            User user = db.Users.SingleOrDefault(x => x.ID == userID);
            user.IsDeleted = 1;
            db.SaveChanges();

            return RedirectToAction("AllUsers");

        }

        public ActionResult CreateUser()
        {

            return View();
        }


        [HttpPost]
        public ActionResult CreateUser(UserViewModel userViewModel)
        {
            try
            {
                EventPlusEntities db = new EventPlusEntities();
                User user = new User();

                user.First_Name = userViewModel.First_Name;
                user.Last_Name = userViewModel.Last_Name;
                user.Email = userViewModel.Email;
                user.Password = userViewModel.Password;
                user.Address = userViewModel.Address;
                user.Phone = userViewModel.Phone;
                user.IsDeleted = 0;
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
