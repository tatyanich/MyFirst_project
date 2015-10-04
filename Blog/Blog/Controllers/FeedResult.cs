using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Syndication;
using System.Text;
using System.Web.Mvc;
using System.Xml;

namespace Blog.Controllers
{
    class FeedResult : ActionResult
    {
        private SyndicationFeedFormatter formattedfeed;
        public SyndicationFeed Feed { get; set; }

        public FeedResult(SyndicationFeedFormatter formattedfeed)
        {
            this.formattedfeed = formattedfeed;
        }

        public override void ExecuteResult(ControllerContext context)
        {
            context.HttpContext.Response.ContentType = "application/rss+xml";
            //Rss20FeedFormatter rssFormatter = new Rss20FeedFormatter(Feed);
            using (XmlWriter writer = XmlWriter.Create(context.HttpContext.Response.Output))
            {
                formattedfeed.WriteTo(writer);
            }
        }
    }
}
