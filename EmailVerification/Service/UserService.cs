using EmailVerification.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EmailVerification.Service
{
    public class UserService
    {
        EmailVerificationEntities db = new EmailVerificationEntities();

        public object user_Register(UserRegistration user)
        {
            user.ActivationCode = Guid.NewGuid();
            user.IsEmailVerified = false;
            try
            {     
                if (user != null)
                {
                    // 
                    db.UserRegistrations.Add(new UserRegistration
                    {

                        Fname = user.Fname,
                        Lname = user.Lname,
                        Email = user.Email,
                        Password = user.Password,
                        ActivationCode = user.ActivationCode,
                        IsEmailVerified = user.IsEmailVerified

                    });

                    db.SaveChanges();
                    return "Success";
                }
                else
                {
                    return "Already";
                }
            }
            catch (Exception ex)
            {

                return ex;
            }
        }

        public object Userlogin(UserRegistration user)
        {
            user = db.UserRegistrations.Where(x => x.Email == user.Email && x.Password == user.Password).SingleOrDefault();
            return user;
        }


    }
}