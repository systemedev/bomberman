using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Bomberman.Models;

namespace Bomberman.Controllers
{
    public class GameRequestController : Controller
    {
        private BOMBERMANEntities db = new BOMBERMANEntities();

        //
        // GET: /GameRequest/

        public ActionResult Index()
        {
          //  var games_requests = db.Games_requests.Include(g => g.Games).Include(g => g.UserProfile).Include(g => g.UserProfile1);
            GameRequestReceivedSentViewModel gamerequest_receivedsent=new GameRequestReceivedSentViewModel();


          //  IList<Games_requests> games_requests = null;
            var querys = (from g in db.Games_requests
                         where g.UserProfile.UserName == User.Identity.Name
                         select g).Include("Games").Include("UserProfile").Include("UserProfile1");

            gamerequest_receivedsent.output = querys.ToList<Games_requests>();

            var queryr = (from g in db.Games_requests
                          where g.UserProfile1.UserName == User.Identity.Name
                          select g).Include("Games").Include("UserProfile").Include("UserProfile1");

            gamerequest_receivedsent.input = queryr.ToList<Games_requests>();


            return View(gamerequest_receivedsent);
        }

     

       
       

        //
        // GET: /GameRequest/Create/5

        public ActionResult Create(int id = 0)
        {

            ICollection<Games> myowngames;
            var query1 = (from g in db.Games
                          where g.UserProfile.UserName == User.Identity.Name
                          where g.remaining_time>0
                          select g);
            myowngames = query1.ToList<Games>();

            ICollection<UserProfile> players;

            var query2 = (from g in db.UserProfile
                          where g.UserName != User.Identity.Name
                          select g);
            players = query2.ToList<UserProfile>();




            ViewBag.id_game = new SelectList(myowngames, "id", "title",id);
            ViewBag.to_id_user = new SelectList(players, "UserId", "UserName");
            return View();
        }






        //
        // POST: /GameRequest/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Games_requests games_requests)
        {
            if (ModelState.IsValid)
            {
                var query = from u in db.UserProfile where u.UserName == User.Identity.Name select u;
              
                games_requests.UserProfile = query.Single();
                 
                db.Games_requests.Add(games_requests);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            IList<Games> myowngames;
            var query1 = (from g in db.Games
                          where g.UserProfile.UserName == User.Identity.Name
                          where g.remaining_time > 0
                          select g).Include("Games").Include("UserProfile");
            myowngames = query1.ToList<Games>();

            IList<UserProfile> players;

            var query2 = (from g in db.UserProfile
                          where g.UserName != User.Identity.Name
                          select g).Include("UserProfile");
            players = query2.ToList<UserProfile>();



            ViewBag.id_game = new SelectList(myowngames, "id", "title", games_requests.id_game);

            ViewBag.to_id_user = new SelectList(players, "UserId", "UserName", games_requests.to_id_user);
            return View(games_requests);
        }

      

        //
        // GET: /GameRequest/Delete/5

        public ActionResult Delete(int id = 0)
        {
            Games_requests games_requests = db.Games_requests.Find(id);
            if (games_requests == null)
            {
                return HttpNotFound();
            }
            return View(games_requests);
        }

        //
        // POST: /GameRequest/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Games_requests games_requests = db.Games_requests.Find(id);
            db.Games_requests.Remove(games_requests);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}