using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using  Blog.Models;
using System.Text ;
using System.Web.Routing;
using System.ServiceModel.Syndication;
using PagedList;

namespace Blog.Controllers
{
    public class PostsController : Controller
    {
        //
        // GET: /Posts/
        private BlogModel model = new BlogModel();
        private const int PostPerPage = 4;
        private const int PostPerFeed = 25;
        private  int xpost;

        public ActionResult Index(int? id)
        {
            int pageNumber = id ?? 0;
            IEnumerable<Post> posts = (from post in model.Posts
                                       where post.DateTime < DateTime.Now
                                       orderby post.DateTime descending
                                       select post).Skip(pageNumber * PostPerPage).Take(PostPerPage + 1);

            ViewBag.IsPreviousLinkVisible = pageNumber > 0;
            ViewBag.IsNextLinkVisible = posts.Count() > PostPerPage;
            ViewBag.PageNumber = pageNumber;
            ViewBag.IsAdmin = IsAdmin;
            return View(posts.Take(PostPerPage));
        }
        [HttpGet]
        public ActionResult Details(int id)

        {
            Post post = GetPost(id);
            xpost = id;
            ViewBag.IsAdmin = IsAdmin;
            return View(post);
        }

        [ValidateInput(true)]
      
        public ActionResult Comments(int id, string name, string email, string body)
          {
            Post post = GetPost(id);
            Comment comment = new Comment();

            comment.Post = post;
            comment.DateTime = DateTime.Now;
            comment.Name = name;
            comment.Email = email;
            comment.Body = body;
            if (((comment.Name == "") && (comment.Body == "")) || (comment.Body == " "))
            {
                return RedirectToAction("Details", new
                {
                    id = id

                });
            }
           
                model.Comments.Add(comment);
                model.SaveChanges();
                
            
           // Here user object with updated data
                 return RedirectToAction("Details", new {    id = id            
               
           });
  
        }

        public ActionResult Tags(string id)
        {
            Tag tag = GetTag(id);
            ViewBag.IsAdmin = IsAdmin;
            return View("Index", tag.Posts);

        }

        public ActionResult Delete(int id)
        {
            if (IsAdmin)
            {
                Post post = GetPost(id);
                model.Posts.Remove(post);
                model.SaveChanges();

            }
            return RedirectToAction("Index");
        }

        public ActionResult DeleteComment(int id) {
           // int idcomm = id;
            //Post post = (model.Posts.Where(x => x.Comments == idcomm).First());
            if (IsAdmin)
            {
                Comment com = model.Comments.Where(c => c.Id == id).First();
                model.Comments.Remove(com);
                model.SaveChanges();

            }
            return RedirectToAction("Index");
           
        }


        public ActionResult RSS()
        {
            System.Collections.Generic.IEnumerable<SyndicationItem> posts =
                (from post in model.Posts
                 where post.DateTime < DateTime.Now
                 orderby post.DateTime descending
                 select post).Take(PostPerFeed).ToList().Select(x => GetSyndicationItem(x));

            SyndicationFeed feed = new SyndicationFeed("Tatiana Matiunina", "Tatiana Matiunina's Blog", new Uri("http://www.tanianatiunina.com"), posts);
            Rss20FeedFormatter formater = new Rss20FeedFormatter(feed);
            return new FeedResult(formater);
        }

        private SyndicationItem GetSyndicationItem(Post post)

        {
            return new SyndicationItem(post.Title, post.Body, new Uri("http://www.tanianatiunina.com/posts/detail/" + post.Id));
        }
        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /Restaurant/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
   
        public ActionResult Create(Blog.Models.Post post)
        {
            if (ModelState.IsValid)
            {
                model.Posts.Add(post);
                model.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(post);
        }



        [ValidateInput(false)]
        public ActionResult Update(int? id, string title, string body, DateTime? dateTime, string tags)
        {

            if (!IsAdmin)
            {
                return RedirectToAction("Index");
            }

            Post post = GetPost(id);

            post.Title = title;
            post.Body = body;
            if (dateTime.HasValue) { post.DateTime = dateTime.Value;}
            
            post.Tags.Clear();

            tags = tags ?? string.Empty;
             string[] tagNames = tags.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
             foreach (string tagName in tagNames)
             {
                 post.Tags.Add(GetTag(tagName));
             }
            if ((post.Title == "") && (post.Body == "") && (post.DateTime == null) || (post.Title == "") || (post.Body == "") || (post.DateTime == null))
             {
                 return RedirectToAction("Edit", new { id = id });
             }
             if (!id.HasValue)
             {
                 model.Posts.Add(post);
             }
             
             model.SaveChanges();
             return RedirectToAction("Details", new  { id = post.Id});
         }

         public ActionResult Edit(int? id) {
             Post post = GetPost(id);
             StringBuilder tagList = new StringBuilder();
             foreach(Tag tag in post.Tags){
                 tagList.AppendFormat("{0}", tag.Name);
             }

             ViewBag.Tags = tagList.ToString();
            return View(post);
        }

        private Tag GetTag(string tagN)
        {
            return model.Tags.Where(x => x.Name == tagN).FirstOrDefault() ?? new Tag() { Name = tagN };
            
        }

        private Post GetPost(int? id)
        {
            return id.HasValue ? model.Posts.Where(x => x.Id == id).First() : new Post() { Id = -1};
            
        }


        public bool IsAdmin { get {  return Session["IsAdmin"] != null && (bool)Session["IsAdmin"]; } }
    }
}
