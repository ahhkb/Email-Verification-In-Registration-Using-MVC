﻿using EmailVerification.Models;
using EmailVerification.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;

namespace EmailVerification.Controllers
{
    public class UserController : Controller
    {
        UserService service = new UserService();
        // GET: User
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(UserRegistration login)
        {
           var u = service.Userlogin(login);
            if (u != null)
            {

                return RedirectToAction("Profile");
            }
            else
            {
                ViewBag.msg = "no such record or user account found";
            }
            return View();
        }


        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]

        public ActionResult Register([Bind(Exclude = "IsEmailVerified,ActivationCode")] UserRegistration user)
        {
            var msg = service.user_Register(user);
            string message = "";
            if (msg == "Success")
            {
                bool Status = false;

                //
                // Model Validation 
                if (ModelState.IsValid)
                {

                    SendVerificationLinkEmail(user.Email, user.ActivationCode.ToString());
                    message = "Registration successfully done. Account activation link " + " has been sent to your email id:" + user.Email;
                    Status = true;
                    
                    return View("SuccessPage");

                }

            }
            else
            {
                message = "Invalid registration Request";
                return View();
            }

            return RedirectToAction("SuccessPage");

        }

        [NonAction]
        public bool IsEmailExist(string email)
        {
            using (EmailVerificationEntities db = new EmailVerificationEntities())
            {
                var emailExist = db.UserRegistrations.Where(x => x.Email == email).FirstOrDefault();
                return emailExist != null;
            }
        }


        public void SendVerificationLinkEmail(string emailID, string activationCode)
        {
            using (MailMessage mm = new MailMessage("syedshah6921@gmail.com", emailID))
            {
                var verifyUrl = "/User/VerifyAccount/" + activationCode;
                var link = Request.Url.AbsoluteUri.Replace(Request.Url.PathAndQuery, verifyUrl);
                mm.Subject = "ASP.NET MVC";
                mm.Body = "<br/><br/>We are excited to tell you that your  account is" +
        " successfully created. Please click on the below link to verify your account " +
        " <br/><br/><a href='" + link + "'>" + link + "</a> ";

                mm.IsBodyHtml = true;
                SmtpClient smtp = new SmtpClient();
                smtp.Host = "smtp.gmail.com";

                smtp.UseDefaultCredentials = false;
                smtp.Credentials = new System.Net.NetworkCredential("syedshah6921@gmail.com", "******");
                smtp.EnableSsl = true;

                smtp.Port = 587;
                smtp.Send(mm);

            }
        }


        [HttpGet]
        public ActionResult VerifyAccount(string id)
        {
            bool status = false;
            using (EmailVerificationEntities db = new EmailVerificationEntities())
            {
                // db.Configuration.ValidateOnSaveEnabled = false;
                var v = db.UserRegistrations.Where(x => x.ActivationCode == new Guid(id)).FirstOrDefault();
                if (v != null)
                {
                    v.IsEmailVerified = true;
                    db.SaveChanges();
                    status = true;
                }
                else
                {
                    ViewBag.message = "Invalid Reuest";
                }

            }
            ViewBag.status = status;
            return View();

        }


        public ActionResult SuccessPage()
        {
            return View();
        }

        public ActionResult Profile ()
        {

            return View();
        }
    }
}