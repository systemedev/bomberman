using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Bomberman.Filters;
using Bomberman.Models;
using System.Runtime.Serialization.Json;
using System.IO;
using System.Text;
using System.Collections;

namespace Bomberman.Controllers
{
    [Authorize]
    [InitializeSimpleMembership]
    public class GameController : Controller
    {

        private BOMBERMANEntities db = new BOMBERMANEntities();


        [AllowAnonymous]
        public ActionResult PublicList()
        {

           

                IList<Games> games = null;



                var query = (from g in db.Games
                             where g.is_public == true
                             select g).Include("UserProfile").Include("Maps");

                games = query.ToList<Games>();


                return View(games);
            



        }



        public ActionResult LetsPlay(int id = 0)
        {

           
                Games games = db.Games.Find(id);
                if (games == null)
                {
                    return HttpNotFound();
                }

                var querycurrentuser = from u in db.UserProfile where u.UserName == User.Identity.Name select u;
                UserProfile upr = querycurrentuser.Single();



                var query = (from g in db.Users_play where g.id_game == id where g.id_user == upr.UserId select g).Include("Users_play").Include("UserProfile");

                if (query.Count() == 0 && games.remaining_time!=0) // l'utilisateur n'a pas encore démarré cette partie 
                {
                    Users_play up = new Users_play();
                    up.id_game = id;
                    up.id_user = upr.UserId;
                    up.bomb = 3;
                    up.level = 1;
                    up.life = 3;
                    up.score = 0;


                    db.Users_play.Add(up);
                    db.SaveChanges();

                    Games_details gd = db.GetARandomEmptyPlace(up.id_game);
                    Locations lc = new Locations();
                    lc.id_user = upr.UserId;
                    lc.isauser = true;
                    lc.id_gamedet = gd.id;


                    db.Locations.Add(lc);
                    db.SaveChanges();



                }

                ViewBag.owner = new SelectList(db.UserProfile, "UserId", "UserName", games.owner);
                ViewBag.id_map = new SelectList(db.Maps, "id", "title", games.id_map);

                return View(games);
            
        }

        public void Left(int id = 0)
        {
            using (BOMBERMANEntities dbc = new BOMBERMANEntities())
            {
                Games games = dbc.Games.Find(id);
                if (games != null && games.remaining_time!=0)
                {
                    var querycurrentuser = from u in dbc.UserProfile where u.UserName == User.Identity.Name select u;
                    UserProfile upr = querycurrentuser.Single();


                   Users_play up=dbc.Users_play.Where(idg =>idg.id_game == id).Where(idu=>idu.id_user==upr.UserId).Single();
                   if (up != null && up.life > 0)
                   {
                       dbc.MoveUserToTheLeft(id, upr.UserId);
                   }
                    ServerResponse ret = new ServerResponse();
                    this.BuildServerResponseGeneralInfo(ret);
                    this.OutputServerResponse(ret);
                }

            } 

        }

        public void Right(int id = 0)
        {
            using (BOMBERMANEntities dbc = new BOMBERMANEntities())
            {
                Games games = dbc.Games.Find(id);
                if (games != null && games.remaining_time != 0)
                {
                    var querycurrentuser = from u in dbc.UserProfile where u.UserName == User.Identity.Name select u;
                    UserProfile upr = querycurrentuser.Single();

                     Users_play up=dbc.Users_play.Where(idg =>idg.id_game == id).Where(idu=>idu.id_user==upr.UserId).Single();
                     if (up != null && up.life > 0)
                     {


                         dbc.MoveUserToTheRight(id, upr.UserId);
                     }
                    ServerResponse ret = new ServerResponse();
                    this.BuildServerResponseGeneralInfo(ret);
                    this.OutputServerResponse(ret);
                }
            }
        }

        public void Up(int id = 0)
        {
            using (BOMBERMANEntities dbc = new BOMBERMANEntities())
            {
                Games games = dbc.Games.Find(id);
                if (games != null && games.remaining_time != 0)
                {
                    var querycurrentuser = from u in dbc.UserProfile where u.UserName == User.Identity.Name select u;
                    UserProfile upr = querycurrentuser.Single();

                     Users_play up=dbc.Users_play.Where(idg =>idg.id_game == id).Where(idu=>idu.id_user==upr.UserId).Single();
                     if (up != null && up.life > 0)
                     {



                         dbc.MoveUserToTheUp(id, upr.UserId);

                     }
                    ServerResponse ret = new ServerResponse();
                    this.BuildServerResponseGeneralInfo(ret);
                    this.OutputServerResponse(ret);
                }
            }

        }

        public void Down(int id = 0)
        {
            using (BOMBERMANEntities dbc = new BOMBERMANEntities())
            {
                Games games = dbc.Games.Find(id);
                if (games != null && games.remaining_time != 0)
                {
                    var querycurrentuser = from u in dbc.UserProfile where u.UserName == User.Identity.Name select u;
                    UserProfile upr = querycurrentuser.Single();

                     Users_play up=dbc.Users_play.Where(idg =>idg.id_game == id).Where(idu=>idu.id_user==upr.UserId).Single();
                     if (up != null && up.life > 0)
                     {


                         dbc.MoveUserToTheDown(id, upr.UserId);
                     }
                    ServerResponse ret = new ServerResponse();
                    this.BuildServerResponseGeneralInfo(ret);
                    this.OutputServerResponse(ret);
                }
            }

        }

        public void Bomb(int id = 0)
        {
            using (BOMBERMANEntities dbc = new BOMBERMANEntities())
            {
                Games games = dbc.Games.Find(id);
                if (games != null && games.remaining_time != 0)
                {
                    var querycurrentuser = from u in dbc.UserProfile where u.UserName == User.Identity.Name select u;
                    UserProfile upr = querycurrentuser.Single();


                     Users_play up=dbc.Users_play.Where(idg =>idg.id_game == id).Where(idu=>idu.id_user==upr.UserId).Single();
                     if (up != null && up.life > 0)
                     {

                         dbc.UserHasDroppedABomb(id, upr.UserId);
                     }
                    ServerResponse ret = new ServerResponse();
                    this.BuildServerResponseGeneralInfo(ret);
                    this.OutputServerResponse(ret);
                }
            }

        }

        public void GameInit(int id = 0)
        {
              ServerResponse ret = new ServerResponse();
             this.BuildServerResponseGeneralInfo(ret);

             this.BuildServerResponseGameInfo(ret, id);
             this.OutputServerResponse(ret);

        }



        public void ServerState(int id = 0)
        {
            ServerResponse ret = new ServerResponse();

            this.BuildServerResponseGeneralInfo(ret);
            this.BuildServerResponseGameInfo(ret, id);
            this.BuildServerResponseEntitiesInfo(ret, id);


            this.BuildServerResponseUsersInfo(ret, id);

            this.OutputServerResponse(ret);


        }





        //
        // GET: /Game/

        public ActionResult Index()
        {
           
                IList<Games> games = null;



                var query = (from g in db.Games
                             where g.UserProfile.UserName == User.Identity.Name
                             select g).Include("UserProfile").Include("Maps");

                games = query.ToList<Games>();


                return View(games);
           




        }

        //
        // GET: /Game/Details/5

        //public ActionResult Details(int id = 0)
        //{
            
        //        Games games = db.Games.Find(id);
        //        if (games == null)
        //        {
        //            return HttpNotFound();
        //        }
        //        return View(games);
            
        //}

        //
        // GET: /Game/Create

        public ActionResult Create()
        {
            
                ICollection<Maps> maps;

                var query = (from g in db.Maps
                             select g);
                maps = query.ToList<Maps>();







                ViewBag.id_map = new SelectList(maps, "id", "title");
                return View();
            
        }

        //
        // POST: /Game/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Games games)
        {
            using (BOMBERMANEntities dbc = new BOMBERMANEntities())
            {
                if (ModelState.IsValid)
                {
                    var query = from u in dbc.UserProfile where u.UserName == User.Identity.Name select u;
                    games.UserProfile = query.Single();
                    dbc.Games.Add(games);
                    dbc.SaveChanges();

                    Hashtable configuration = dbc.GetGameConfiguration(games.id);
                    int nbrows = Convert.ToInt32(configuration["MAPNUMBEROFROWS"]);
                    int nbcolumns = Convert.ToInt32(configuration["MAPNUMBEROFCOLUMNS"]);



                    for (int row = 0; row < nbrows; row++)
                    {

                        for (int col = 0; col < nbcolumns; col++)
                        {

                            Cell_types ct = dbc.GetARandomCellType();
                            Games_details gd = new Games_details();
                            gd.game_id = games.id;
                            gd.cell_type_id = ct.id;
                            gd.location_column = (byte)col;
                            gd.location_row = (byte)row;
                            dbc.Games_details.Add(gd);



                        }


                    }







                    dbc.SaveChanges();










                    return RedirectToAction("Index");

                }

                ViewBag.id_map = new SelectList(db.Maps, "id", "title", games.id_map);
                return View(games);
            }
        }

        ////
        //// GET: /Game/Edit/5

        //public ActionResult Edit(int id = 0)
        //{
           
        //        Games games = db.Games.Find(id);
        //        if (games == null)
        //        {
        //            return HttpNotFound();
        //        }
        //        ViewBag.owner = new SelectList(db.UserProfile, "UserId", "UserName", games.owner);
        //        ViewBag.id_map = new SelectList(db.Maps, "id", "title", games.id_map);
        //        return View(games);
            
        //}

        //
        // POST: /Game/Edit/5

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Edit(Games games)
        //{
            
        //        if (ModelState.IsValid)
        //        {
        //            db.Entry(games).State = EntityState.Modified;
        //            db.SaveChanges();
        //            return RedirectToAction("Index");
        //        }
        //        ViewBag.owner = new SelectList(db.UserProfile, "UserId", "UserName", games.owner);
        //        ViewBag.id_map = new SelectList(db.Maps, "id", "title", games.id_map);
        //        return View(games);
            
        //}

        //
        // GET: /Game/Delete/5

        public ActionResult Delete(int id = 0)
        {
           
                Games games = db.Games.Find(id);
                if (games == null)
                {
                    return HttpNotFound();
                }
                return View(games);
            
        }

        //
        // POST: /Game/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            
                Games games = db.Games.Find(id);
                db.Games.Remove(games);
                db.SaveChanges();
                db.Dispose();
                return RedirectToAction("Index");
            
        }

      


        private void BuildServerResponseGeneralInfo(ServerResponse input)
        {
            using (BOMBERMANEntities dbc = new BOMBERMANEntities())
            {
                var querycurrentuser = from u in dbc.UserProfile where u.UserName == User.Identity.Name select u;
                UserProfile upr = querycurrentuser.Single();
                input.Generalinfo.RequestedBy = upr.UserId + " " + upr.UserName;
                input.Generalinfo.DateTime = DateTime.Now.ToLongDateString();
            }
        }


        private void BuildServerResponseEntitiesInfo(ServerResponse input, int id_game)
        {
             using (BOMBERMANEntities dbc = new BOMBERMANEntities())
            {
            input.Entities = dbc.GetAllEntitiesOfAGame(id_game);

                //to retrieve all the exploded elements
            List<Games_details> impactedcells;
            for (int i = 0; i < 25; i++) // 25 est un nombre arbitraire pour récupérer toutes les entrée  que le bomb timer aura déposé 
            {
                Object imo = HttpRuntime.Cache.Get(id_game + "GAMEIDLOC" + i);
                if (imo != null)
                {
                    impactedcells = (List<Games_details>)imo;

                    foreach (Games_details impactedcell in impactedcells)
                    {

                        EntityInfo extraentitytodraw = new EntityInfo();
                        extraentitytodraw.EntityColumn = impactedcell.location_column;
                        extraentitytodraw.EntityRow = impactedcell.location_row;
                        extraentitytodraw.Id = 0;
                        extraentitytodraw.Picture = "boum.png";
                        input.Entities.Add(extraentitytodraw);

                    }


                }

            }
             
             
             
             }
        }

        private void BuildServerResponseGameInfo(ServerResponse input, int id_game)
        {
            using (BOMBERMANEntities dbc = new BOMBERMANEntities())
            {
                input.GameInfo = dbc.GetGameInfo(id_game);
            }
        }

        private void BuildServerResponseUsersInfo(ServerResponse input, int id_game)
        {
            using (BOMBERMANEntities dbc = new BOMBERMANEntities())
            {
                input.Users = dbc.GetAllUsersOfAGame(id_game);
            }
        }


        private void OutputServerResponse(ServerResponse input)
        {

            DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(ServerResponse));
            MemoryStream ms = new MemoryStream();
            serializer.WriteObject(ms, input);
            Response.Write(Encoding.UTF8.GetString(ms.ToArray()));


        }

    }
}