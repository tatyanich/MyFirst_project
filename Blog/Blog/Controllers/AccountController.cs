using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Blog.Models;

namespace Blog.Controllers
{
    public class AccountController : Controller
    {
        private BlogModel db = new BlogModel();

       
        
        public ActionResult Login(Administrator user)
        {
            if(user.Name == null && user.Password == null)
            {
                return View();
            }

            Administrator admin = db.Administrators.Where(x => x.Name == user.Name).FirstOrDefault();
            if (admin != null)
            {
                using (MD5 md5Hash = MD5.Create())
                {
                    string hash = GetMd5Hash(md5Hash, user.Password);

                                     
                  
                   
                    var isAuth = admin.Password.Equals(hash);
                   
                    // Если авторизован
                    if (isAuth)
                    {
                        // елсли админ
                        Session["IsAdmin"] = admin.Name.Equals("admin") && admin.Password.Equals("21232f297a57a5a743894a0e4a801fc3");

                        
                         //Session["IsAdmin"] = admin.Role.Equals("Admin");
                    }
                }
            }
        

            return RedirectToAction("Index","Posts");
            
        }


        public ActionResult Logout() {

            Session["Nonce"] = null;
            Session["IsAdmin"] = null;
            return RedirectToAction("Index", "Posts");
        }

        private string GetMd5Hash(MD5 md5Hash, string input)
        {

            // Convert the input string to a byte array and compute the hash.
            byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(input));

            // Create a new Stringbuilder to collect the bytes
            // and create a string.
            StringBuilder sBuilder = new StringBuilder();

            // Loop through each byte of the hashed data 
            // and format each one as a hexadecimal string.
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }

            // Return the hexadecimal string.
            return sBuilder.ToString();
        }

        public ActionResult Edit(int id = 0)
        {
            Administrator administrator = db.Administrators.Find(id);
            if (administrator == null)
            {
                return HttpNotFound();
            }
            return View(administrator);
        }

       

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}