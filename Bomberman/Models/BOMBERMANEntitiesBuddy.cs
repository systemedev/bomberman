using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Bomberman.Models
{
    using System;
    using System.Collections;
    using System.Data;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    using System.Data.SqlClient;
    using System.Transactions;

    public partial class BOMBERMANEntities : DbContext
    {


        public Cell_types GetARandomCellType()
        {
            return this.Cell_types.SqlQuery("SELECT TOP 1 * FROM Cell_types ORDER BY NEWID()").Single<Cell_types>();
        }


        public Games_details GetARandomEmptyPlace(int game_id)
        {

            return this.Games_details.SqlQuery("SELECT TOP 1 * FROM Games_details WHERE game_id=@game_id AND cell_type_id=2 ORDER BY NEWID() ", new SqlParameter("game_id", game_id)).Single<Games_details>();

        }



        public Hashtable GetGameConfiguration(int game_id)
        {

            Hashtable configuration = new Hashtable();
            Games g = this.Games.Find(game_id);
            if (g != null)
            {
                Maps map = this.Maps.Find(g.id_map);
                string[] conftokens = map.configuration.Split(';');

                foreach (string conftoken in conftokens)
                {
                    if (conftoken.Length != 0)
                    {
                        string[] valkey = conftoken.Split(':');
                        configuration.Add(valkey[0].ToUpper(), valkey[1]);
                    }
                }
                configuration.Add("MAPNAME", map.title);
                configuration.Add("REMAININGTIME", g.remaining_time);


            }
            return configuration;

        }




        public List<EntityInfo> GetAllEntitiesOfAGame(int game_id)
        {



            String sqlquery = "select Games_Details.id AS Id,Games_Details.location_row AS EntityRow,Games_Details.location_column AS EntityColumn ,  IIf (Locations.isabomb=1, 'bomb.png',IIf (Cell_types.name='empty_place',null,Cell_types.representation)) AS Picture  From ((Games_Details JOIN Cell_types ON Games_Details.cell_type_id=Cell_types.id) LEFT JOIN Locations ON Locations.id_gamedet=Games_Details.id) where Games_Details.game_id = @game_id ";

            return this.Database.SqlQuery<EntityInfo>(sqlquery, new SqlParameter("game_id", game_id)).ToList();


        }



        public void MoveUserToTheLeft(int game_id, int user_id)
        {

            try
            {
                String sqlquery = "select UserProfile.UserId AS Id,UserProfile.UserName AS Name,Users_play.score AS Score,Users_play.life AS Life,Users_play.level AS Level,Users_play.bomb AS Bomb, Games_Details.location_row AS UserRow , Games_Details.location_column AS UserColumn,Locations.id AS Location_id,Locations.id_gamedet AS Gamedetail_id  From (((Games_Details JOIN Locations ON Locations.id_gamedet=Games_Details.id) JOIN Users_play ON Locations.id_user=Users_play.id_user) JOIN UserProfile ON UserProfile.UserId=Users_play.id_user)   where Games_Details.game_id = @game_id and Users_play.id_game = @game_id and Locations.isauser=1 and Locations.id_user= @user_id";
                UserInfo ui = this.Database.SqlQuery<UserInfo>(sqlquery, new SqlParameter("game_id", game_id), new SqlParameter("user_id", user_id)).Single();

                sqlquery = "select Games_details.id from Games_details JOIN Cell_types ON Games_details.cell_type_id=Cell_types.id WHERE Games_details.game_id=@game_id  AND location_row=@previous_location_row AND  location_column=@previous_location_column-1 AND walkable=1";
                Int32 next_game_detail_id = this.Database.SqlQuery<Int32>(sqlquery, new SqlParameter("game_id", game_id), new SqlParameter("previous_location_row", ui.UserRow), new SqlParameter("previous_location_column", ui.UserColumn)).SingleOrDefault();

                if (next_game_detail_id != 0) // the user is allowed to go there
                {

                    Locations oldloc = Locations.Where(idl => idl.id == ui.Location_id).Single();
                    Locations lc = new Locations();
                    lc.id_user = user_id;
                    lc.isauser = true;
                    lc.isabomb = false;
                    lc.id_gamedet = next_game_detail_id;

                    this.Locations.Add(lc);
                    
                    DoWalk(game_id, lc);
                    Locations.Remove(oldloc);
                    this.SaveChanges();


                }
            }
            catch (Exception e)
            {
                //all change will be rollbacked thanks to EMF
            } 

        }

        public void MoveUserToTheRight(int game_id, int user_id)
        {
            try
            {
                String sqlquery = "select UserProfile.UserId AS Id,UserProfile.UserName AS Name,Users_play.score AS Score,Users_play.life AS Life,Users_play.level AS Level,Users_play.bomb AS Bomb, Games_Details.location_row AS UserRow , Games_Details.location_column AS UserColumn,Locations.id AS Location_id,Locations.id_gamedet AS Gamedetail_id  From (((Games_Details JOIN Locations ON Locations.id_gamedet=Games_Details.id) JOIN Users_play ON Locations.id_user=Users_play.id_user) JOIN UserProfile ON UserProfile.UserId=Users_play.id_user)   where Games_Details.game_id = @game_id and Users_play.id_game = @game_id and Locations.isauser=1 and Locations.id_user= @user_id";
                UserInfo ui = this.Database.SqlQuery<UserInfo>(sqlquery, new SqlParameter("game_id", game_id), new SqlParameter("user_id", user_id)).Single();

                sqlquery = "select Games_details.id from Games_details JOIN Cell_types ON Games_details.cell_type_id=Cell_types.id WHERE Games_details.game_id=@game_id  AND location_row=@previous_location_row AND  location_column=@previous_location_column+1 AND walkable=1";
                Int32 next_game_detail_id = this.Database.SqlQuery<Int32>(sqlquery, new SqlParameter("game_id", game_id), new SqlParameter("previous_location_row", ui.UserRow), new SqlParameter("previous_location_column", ui.UserColumn)).SingleOrDefault();

                if (next_game_detail_id != 0) // the user is allowed to go there
                {

                    Locations oldloc = Locations.Where(idl => idl.id == ui.Location_id).Single();
                    Locations lc = new Locations();
                    lc.id_user = user_id;
                    lc.isauser = true;
                    lc.isabomb = false;
                    lc.id_gamedet = next_game_detail_id;

                    this.Locations.Add(lc);

                    DoWalk(game_id, lc);
                    Locations.Remove(oldloc);
                    this.SaveChanges();

                }
            }
            catch (Exception e)
            {
                //all change will be rollbacked thanks to EMF
            } 


        }

        public void MoveUserToTheUp(int game_id, int user_id)
        {
            try
            {
                String sqlquery = "select UserProfile.UserId AS Id,UserProfile.UserName AS Name,Users_play.score AS Score,Users_play.life AS Life,Users_play.level AS Level,Users_play.bomb AS Bomb, Games_Details.location_row AS UserRow , Games_Details.location_column AS UserColumn,Locations.id AS Location_id,Locations.id_gamedet AS Gamedetail_id  From (((Games_Details JOIN Locations ON Locations.id_gamedet=Games_Details.id) JOIN Users_play ON Locations.id_user=Users_play.id_user) JOIN UserProfile ON UserProfile.UserId=Users_play.id_user)   where Games_Details.game_id = @game_id and Users_play.id_game = @game_id and Locations.isauser=1 and Locations.id_user= @user_id";
                UserInfo ui = this.Database.SqlQuery<UserInfo>(sqlquery, new SqlParameter("game_id", game_id), new SqlParameter("user_id", user_id)).Single();

                sqlquery = "select Games_details.id from Games_details JOIN Cell_types ON Games_details.cell_type_id=Cell_types.id WHERE Games_details.game_id=@game_id  AND location_row=@previous_location_row-1 AND  location_column=@previous_location_column AND walkable=1";
                Int32 next_game_detail_id = this.Database.SqlQuery<Int32>(sqlquery, new SqlParameter("game_id", game_id), new SqlParameter("previous_location_row", ui.UserRow), new SqlParameter("previous_location_column", ui.UserColumn)).SingleOrDefault();

                if (next_game_detail_id != 0) // the user is allowed to go there
                {
                    Locations oldloc = Locations.Where(idl => idl.id == ui.Location_id).Single();
                    Locations lc = new Locations();
                    lc.id_user = user_id;
                    lc.isauser = true;
                    lc.isabomb = false;
                    lc.id_gamedet = next_game_detail_id;

                    this.Locations.Add(lc);
                   
                    DoWalk(game_id, lc);
                    Locations.Remove(oldloc);
                    
                    this.SaveChanges();

                }
            }
            catch (Exception e)
            {
                //all change will be rollbacked thanks to EMF
            } 

        }

        public void MoveUserToTheDown(int game_id, int user_id)
        {
           
           
            try
            {
                String sqlquery = "select UserProfile.UserId AS Id,UserProfile.UserName AS Name,Users_play.score AS Score,Users_play.life AS Life,Users_play.level AS Level,Users_play.bomb AS Bomb, Games_Details.location_row AS UserRow , Games_Details.location_column AS UserColumn,Locations.id AS Location_id,Locations.id_gamedet AS Gamedetail_id  From (((Games_Details JOIN Locations ON Locations.id_gamedet=Games_Details.id) JOIN Users_play ON Locations.id_user=Users_play.id_user) JOIN UserProfile ON UserProfile.UserId=Users_play.id_user)   where Games_Details.game_id = @game_id and Users_play.id_game = @game_id  and Locations.isauser=1 and Locations.id_user= @user_id";
                UserInfo ui = this.Database.SqlQuery<UserInfo>(sqlquery, new SqlParameter("game_id", game_id), new SqlParameter("user_id", user_id)).Single();

                sqlquery = "select Games_details.id from Games_details JOIN Cell_types ON Games_details.cell_type_id=Cell_types.id WHERE Games_details.game_id=@game_id  AND location_row=@previous_location_row+1 AND  location_column=@previous_location_column AND walkable=1";
                Int32 next_game_detail_id = this.Database.SqlQuery<Int32>(sqlquery, new SqlParameter("game_id", game_id), new SqlParameter("previous_location_row", ui.UserRow), new SqlParameter("previous_location_column", ui.UserColumn)).SingleOrDefault();

                if (next_game_detail_id != 0) // the user is allowed to go there
                {
                    Locations oldloc = Locations.Where(idl => idl.id == ui.Location_id).Single();
                    Locations lc = new Locations();
                    lc.id_user = user_id;
                    lc.isauser = true;
                    lc.isabomb = false;
                    lc.id_gamedet = next_game_detail_id;

                    this.Locations.Add(lc);
                   
                    DoWalk(game_id, lc);


                    Locations.Remove(oldloc);
                    
                      

                    this.SaveChanges();

                }
            }
            catch(Exception e)
            {
                //all change will be rollbacked thanks to EMF
            } 

        }


        public void UserHasDroppedABomb(int game_id, int user_id)
        {
            try
            {
                String sqlquery = "select UserProfile.UserId AS Id,UserProfile.UserName AS Name,Users_play.score AS Score,Users_play.life AS Life,Users_play.level AS Level,Users_play.bomb AS Bomb, Games_Details.location_row AS UserRow , Games_Details.location_column AS UserColumn,Locations.id AS Location_id,Locations.id_gamedet AS Gamedetail_id  From (((Games_Details JOIN Locations ON Locations.id_gamedet=Games_Details.id) JOIN Users_play ON Locations.id_user=Users_play.id_user) JOIN UserProfile ON UserProfile.UserId=Users_play.id_user)   where Games_Details.game_id = @game_id and Users_play.id_game = @game_id and Locations.isauser=1 and Locations.id_user= @user_id";
                UserInfo ui = this.Database.SqlQuery<UserInfo>(sqlquery, new SqlParameter("game_id", game_id), new SqlParameter("user_id", user_id)).Single();
                if (ui.Bomb != 0) //the user is allowed to drop a bomb
                {
                    Locations lc = new Locations();
                    lc.id_user = user_id;
                    lc.isauser = false;
                    lc.isabomb = true;
                    lc.bomb_level = ui.Level;
                    lc.id_gamedet = ui.Gamedetail_id;
                    lc.bomb_timeout = 3; //3 secondes pour toutes les bombes 

                    this.Locations.Add(lc);
                    


                    Users_play up = this.Users_play.Where(idg => idg.id_game == game_id).Where(idu => idu.id_user == user_id).Single();
                    up.bomb = up.bomb - 1;

                    this.SaveChanges();
                 
                }

            }
            catch (Exception e)
            {
                //all change will be rollbacked thanks to EMF
            } 


        }


        public void DoWalk(int game_id, Locations l)
        {
            Games_details gd = Games_details.Find(l.id_gamedet);


            Users_play up = this.Users_play.Where(idg => idg.id_game == game_id).Where(idu => idu.id_user == l.id_user).Single();

            //old way
            // Users_play up = this.Database.SqlQuery<Users_play>("select * from Users_play where id_game=@id_game and id_user=@id_user", new SqlParameter("id_game", game_id), new SqlParameter("id_user", l.id_user)).Single();          

            String sqlquerydest = "select Cell_dest.* From (Cell_types AS Cell_orig JOIN Cell_types AS Cell_dest ON Cell_orig.walked_type_to=Cell_dest.id ) where Cell_orig.id=@cell_orig_id";
            String sqlqueryorig = "select * From Cell_types where Cell_types.id=@cell_orig_id";


            Cell_types ctdest = this.Database.SqlQuery<Cell_types>(sqlquerydest, new SqlParameter("cell_orig_id", gd.cell_type_id)).SingleOrDefault();
            Cell_types ctorig = this.Database.SqlQuery<Cell_types>(sqlqueryorig, new SqlParameter("cell_orig_id", gd.cell_type_id)).SingleOrDefault();




            if (ctdest != null)
            {
                gd.cell_type_id = ctdest.id;
                up.bomb = up.bomb + ctorig.powerups_morebomb;
                up.level = up.level + ctorig.powerups_morelevel;
                up.life = up.life + ctorig.powerups_morelife;
                up.score = up.score + ctorig.score;

            }

        }

        public void UpdateBomb(Locations l,int indexcache)
        {
            Users_play up = null;
            Boolean gameover = false;

            Games_details explosioncore = Games_details.Find(l.id_gamedet);
            int game_id = explosioncore.game_id;

            if (Games.Find(game_id).remaining_time == 0)
            {
                gameover = true;
                return;
            }




            l.bomb_timeout = l.bomb_timeout - 1;
            this.SaveChanges();

            if (l.bomb_timeout == 0) // nuke them all ;-)
            {

               

                String sqltargets = "select * from Games_details where game_id=@gameid and (location_row <= @row + @level and location_row >= @row - @level) and (location_column <= @column + @level and location_column >= @column - @level)";
                List<Games_details> impactedcells = this.Database.SqlQuery<Games_details>(sqltargets, new SqlParameter("gameid", explosioncore.game_id), new SqlParameter("row", explosioncore.location_row), new SqlParameter("column", explosioncore.location_column), new SqlParameter("level", l.bomb_level)).ToList();
                String sqlmutationquery = "select Cell_dest.* From (Cell_types AS Cell_orig JOIN Cell_types AS Cell_dest ON Cell_orig.exploded_type_to=Cell_dest.id ) where Cell_orig.id=@cell_orig_id";
                
                //on va cacher 2 seconde les cells impactée pour que les joueurs puissent voir les explosions
                HttpRuntime.Cache.Insert(game_id + "GAMEIDLOC" + indexcache,
                impactedcells, null, DateTime.Now.AddSeconds(2), TimeSpan.Zero);
              
                
                foreach (Games_details impactedcell in impactedcells)
                {
                    
                  
                    
                    Locations locimpact = Locations.Where(idl => idl.isauser == true).Where(idg => idg.id_gamedet == impactedcell.id).SingleOrDefault();
                    if (locimpact != null)
                    {
                        up = this.Users_play.Where(idg => idg.id_game == game_id).Where(idu => idu.id_user == locimpact.id_user).Single();
                        up.life = up.life - 1;
                        
                    }

                    Cell_types ctdest = this.Database.SqlQuery<Cell_types>(sqlmutationquery, new SqlParameter("cell_orig_id", impactedcell.cell_type_id)).SingleOrDefault();
                    if (ctdest != null)
                    {
                        this.Games_details.Find(impactedcell.id).cell_type_id = ctdest.id;
                     
                    }
                    this.SaveChanges();

                }
                Locations.Remove(l);
                this.SaveChanges();
              
                
               
            }





        }

        // background task (see the Global.asax.cs)
        public void LookForBombToExplode()
        {
            try
            {
                int i = 0;
                List<Locations> bombtargets = Locations.Where(idl => idl.isabomb == true).ToList();
                foreach (Locations bombtarget in bombtargets)
                {

                    this.UpdateBomb(bombtarget,i);
                    i++;

                }
            }
            catch (Exception e)
            {
                String msg = e.Message;//all change will be rollbacked thanks to EMF
            } 
            
        }

        public void LookForGameToUpdate()
        {

            try
            {
                List<Games> gametargets = Games.Where(idg=>idg.remaining_time>0).ToList();
                foreach (Games gametarget in gametargets)
                {
                    gametarget.remaining_time = gametarget.remaining_time - 1;
                    this.SaveChanges();
                }
            }
            catch (Exception e)
            {
                //all change will be rollbacked thanks to EMF
            } 





        }




        public List<UserInfo> GetAllUsersOfAGame(int game_id)
        {
            /**
                [DataMember(Name = "USER_ID")]
            public int Id { get; set; }


            [DataMember(Name = "USER_NAME")]
            public string Name { get; set; }


            [DataMember(Name = "USER_SCORE")]
            public string Score { get; set; }

            [DataMember(Name = "USER_LIFE")]
            public int Life { get; set; }


            [DataMember(Name = "USER_LEVEL")]
            public int Level { get; set; }



            [DataMember(Name = "USER_BOMB")]
            public int Bomb { get; set; }
                Location_id

            [DataMember(Name = "USER_ROW")]
            public int Row { get; set; }

            [DataMember(Name = "USER_COLUMN")]
            public int Column { get; set; }
             * */

            String sqlquery = "select UserProfile.UserId AS Id,UserProfile.UserName AS Name,Users_play.score AS Score,Users_play.life AS Life,Users_play.level AS Level,Users_play.bomb AS Bomb, Games_Details.location_row AS UserRow , Games_Details.location_column AS UserColumn,Locations.id AS Location_id,Locations.id_gamedet AS Gamedetail_id From (((Games_Details JOIN Locations ON Locations.id_gamedet=Games_Details.id) JOIN Users_play ON Locations.id_user=Users_play.id_user) JOIN UserProfile ON UserProfile.UserId=Users_play.id_user)   where Games_Details.game_id = @game_id and Users_play.id_game = @game_id  and Locations.isauser=1";

            return this.Database.SqlQuery<UserInfo>(sqlquery, new SqlParameter("game_id", game_id)).ToList();


        }








        public GameInfo GetGameInfo(int game_id)
        {

            GameInfo ret = new GameInfo();


            Hashtable conf = this.GetGameConfiguration(game_id);

            ret.MAPNAME = (String)conf["MAPNAME"];
            ret.MAPBACKGROUND = (String)conf["MAPBACKGROUND"];
            ret.MAPNUMBEROFCOLUMNS = (String)conf["MAPNUMBEROFCOLUMNS"];
            ret.MAPNUMBEROFROWS = (String)conf["MAPNUMBEROFROWS"];
            ret.REMAININGTIME = (int)conf["REMAININGTIME"];


            /**     
            
               [DataMember(Name = "MAPNAME")]
        public string MAPNAME { get; set; }

        [DataMember(Name = "MAPBACKGROUND")]
        public string MAPBACKGROUND { get; set; }

        [DataMember(Name = "MAPPICTURES")]
        public string MAPPICTURES { get; set; }

        
        
        [DataMember(Name = "MAPNUMBEROFCOLUMNS")]
        public string MAPNUMBEROFCOLUMNS { get; set; }

        [DataMember(Name = "MAPNUMBEROFROWS")]
        public string MAPNUMBEROFROWS { get; set; }

        [DataMember(Name = "REMAININGTIME")]
        public string REMAININGTIME { get; set; }
               **/
            String sqlquery = "select distinct IIf (Locations.isabomb=1, 'bomb.png',IIf (Cell_types.name='empty_place',null,Cell_types.representation)) AS ENTITY_PICTURE  From ((Games_Details JOIN Cell_types ON Games_Details.cell_type_id=Cell_types.id) LEFT JOIN Locations ON Locations.id_gamedet=Games_Details.id) where Games_Details.game_id = @game_id ";

            List<String> picturelist = this.Database.SqlQuery<String>(sqlquery, new SqlParameter("game_id", game_id)).ToList();
            picturelist.Add("boum.png");
            ret.MAPPICTURES = picturelist;
            return ret;

        }




    }
}