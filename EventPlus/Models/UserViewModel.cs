using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EventPlus.Models
{
    public class UserViewModel
    {
        public int ID { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string First_Name { get; set; }
        public string Last_Name { get; set; }
        public string Address { get; set; }
        public String Phone { get; set; }
        public Gender Gender { get; set; }
        public System.DateTime DateOfBirth { get; set; }

        public int IsDeleted { get; set; }

        public string OrganizationName { get; set; }

        public String GetUserGenderValue(Gender gender)
        {
            if (gender == Gender.Male)
            {
                return "Male";
            }
            return "Female";
        }

        public Gender SetUserGenderValue(String value)
        {
            if (value == "Male")
            {
                return Gender.Male;
            }
            return Gender.Female;
        }

    }

    public enum Gender
    {
        Male,
        Female
    }
}