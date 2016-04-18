using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Globalization;
using System.Web.Mvc;
using System.Web.Security;
using System.Runtime.Serialization;
/*
 * This class intends to be the medium for all the communication between client and server during a game 
 * it will be turned into JSON 
 */




namespace Bomberman.Models
{
  
    /*
     * Owns all the information about the "static" entities  present in the map : stone,wall,bomb,gift
     */
    [DataContract]
    public class EntityInfo
    {


        [DataMember(Name = "ENTITY_ID")]
        public int Id { get; set; }


        [DataMember(Name = "ENTITY_PICTURE")]
        public string Picture { get; set; }

        [DataMember(Name = "ENTITY_ROW")]
        public byte EntityRow { get; set; }

        [DataMember(Name = "ENTITY_COLUMN")]
        public byte EntityColumn { get; set; }
       
    }

    /*
    * Owns all the information about the users  present in the map 
    */
    [DataContract]
    public class UserInfo
    {


        [DataMember(Name = "USER_ID")]
        public int Id { get; set; }


        [DataMember(Name = "USER_NAME")]
        public string Name { get; set; }


        [DataMember(Name = "USER_SCORE")]
        public int Score { get; set; }

        [DataMember(Name = "USER_LIFE")]
        public int Life { get; set; }


        [DataMember(Name = "USER_LEVEL")]
        public int Level { get; set; }



        [DataMember(Name = "USER_BOMB")]
        public int Bomb { get; set; }


        [DataMember(Name = "USER_LOCATION_ID")]
        public int Location_id { get; set; }

        [DataMember(Name = "USER_GAMEDETAIL_ID")]
        public int Gamedetail_id { get; set; }


        [DataMember(Name = "USER_ROW")]
        public byte UserRow { get; set; }

        [DataMember(Name = "USER_COLUMN")]
        public byte UserColumn { get; set; }

    }

    /* game info */
    [DataContract]
    public class GameInfo
    {

        [DataMember(Name = "GAMEINFO_MAPNAME")]
        public string MAPNAME { get; set; }

        [DataMember(Name = "GAMEINFO_MAPBACKGROUND")]
        public string MAPBACKGROUND { get; set; }

        [DataMember(Name = "GAMEINFO_MAPPICTURES")]
        public List<string> MAPPICTURES { get; set; }



        [DataMember(Name = "GAMEINFO_MAPNUMBEROFCOLUMNS")]
        public string MAPNUMBEROFCOLUMNS { get; set; }

        [DataMember(Name = "GAMEINFO_MAPNUMBEROFROWS")]
        public string MAPNUMBEROFROWS { get; set; }

        [DataMember(Name = "GAMEINFO_REMAININGTIME")]
        public int REMAININGTIME { get; set; }
        
    }
    
    
    
    
    
    
    
    
    /* Owns general information */
    [DataContract]
    public class GeneralInfo
    {

        [DataMember(Name = "GENERALINFO_REQUESTEDBY")]
        public string RequestedBy { get; set; } // identification de l'utilisateur ayant effectué la requête 


        [DataMember(Name = "GENERALINFO_DATETIME")]
        public string DateTime { get; set; }

       



    }


        





    [DataContract]
    public class ServerResponse
    {
        [DataMember(Name = "SERVERRESPONSE_ENTITIES")]
        public List<EntityInfo> Entities { get; set; }


        [DataMember(Name = "SERVERRESPONSE_USERS")]
        public List<UserInfo> Users { get; set; }

        [DataMember(Name = "SERVERRESPONSE_GENERALINFO")]
        public GeneralInfo Generalinfo { get; set; }

        [DataMember(Name = "SERVERRESPONSE_GAMEINFO")]
        public GameInfo GameInfo { get; set; }

        public ServerResponse()
        {
            this.Generalinfo = new GeneralInfo();
            this.GameInfo = new GameInfo();
            this.Entities = new List<EntityInfo>();
            this.Users = new List<UserInfo>();

        }
         
    
        
    
    
    
    
    
    
    }



    




     

   


}
