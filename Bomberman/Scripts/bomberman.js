var Game = {};
var KeyBoard = {};
var Local = {};
var Server = {};
var server_users = [];
var server_entities = [];
Game.entities = [];
Game.users = [];

function Entity(picture,row,column)
{
    
    this.picture = picture;
    this.row = row;
    this.column = column;
    this.img = new Image();
}


function User(id,name,score,life,level,bomb,row,column) {
    
    this.id = id;
    this.name = name;
    this.score = score;
    this.life = life;
    this.level = level;
    this.bomb = bomb;
    this.row = row;
    this.column = column;
}

function clearusers() {
    
    $("#tableUsers tbody").html("");

}








    


    Game.gridwidth = 2;
    Game.fps = 10;
    Game.id = 0;
    Game.background = '';
    Game.columncount = 0;
    Game.rowcount = 0;
    Game.remainingtime = 0;
    Game.initialize = function (idgame) {
        Game.id = idgame;
        
      
        $.getJSON("/Game/GameInit/", { id: Game.id }, function (data) {

            var items = [];
            $.each(data, function (key, val) {

                
                if (key == "SERVERRESPONSE_GAMEINFO") {


                   // alert(val.GAMEINFO_MAPNAME);

                    Game.background = '../../Images/' + val.GAMEINFO_MAPBACKGROUND + '?v=' + Math.random(); // to avoid the ceasing of load event


                    var hidden = $('body').append('<div id="img-cache" style="display:none/>').children('#img-cache');
                    for (var i = 0; i < val.GAMEINFO_MAPPICTURES.length; i++) {
                        if (val.GAMEINFO_MAPPICTURES[i] != null) {   // Add hidden element

                            $('<img/>').attr('src', '../../Images/' + val.GAMEINFO_MAPPICTURES[i]).appendTo(hidden);
                            
                        }

                        }

                        Game.columncount=val.GAMEINFO_MAPNUMBEROFCOLUMNS;

                        Game.rowcount=val.GAMEINFO_MAPNUMBEROFROWS;

                        Game.remainingtime = val.GAMEINFO_REMAININGTIME; 

                      
                        Game.canvas = document.getElementById("gamezone");
                        Game.canvaswidth = Game.canvas.width;
                        Game.canvasheight = Game.canvas.height;
                        Game.context = document.getElementById("gamezone").getContext("2d");
                        Game.bordertileheight = Game.canvasheight / Game.rowcount;
                        Game.bordertilewidth = Game.canvaswidth / Game.columncount;
                        Game.tileheight = Game.canvasheight / Game.rowcount - (Game.gridwidth) - (Game.gridwidth / Game.rowcount);
                        Game.tilewidth = Game.canvaswidth / Game.columncount - (Game.gridwidth) - (Game.gridwidth / Game.columncount);
                        Game.boardpieces = new Array(Game.rowcount);



                    //normally, it should be the last to be loaded... thus its end of loading will be the trigger  for initializeboard
                        Game.backgroundimg = new Image();
                        Game.backgroundimg.src = Game.background;
                        Game.backgroundimg.addEventListener('load', Game.initializeboard, false);
                    
                }
            });
        });



       

       
      


      


      
   
    };

    Game.initializeboard = function () {
    
        //row
        for (var i = 0; i < Game.rowcount;i++) {
            Game.boardpieces[i] = new Array(Game.columncount);

            //columns
            for (var j = 0; j < Game.columncount; j++) {
                Game.boardpieces[i][j] = new Object;

            }
        }
        Game._intervalId = setInterval(Game.run, 1000 / Game.fps);
        Game._monitorintervalId = setInterval(Game.monitor, 5000);
        
      

    };
   
    Game.monitor = function () {
        if (Game.remainingtime == 0) {
            alert('Game is over');
            clearInterval(Game._intervalId);
            clearInterval(Game._monitorintervalId);


        }
    }



    //dessin du plateau 
    Game.draw = function () {
       
        Game.context.clearRect(0, 0, Game.canvaswidth, Game.canvasheight);
        Game.context.drawImage(Game.backgroundimg, 0, 0, Game.canvaswidth, Game.canvasheight);
        Game.drawusers();
        Game.drawentities();

       
        
        
    }

    Game.drawentities = function () {
        Game.context.lineWidth = Game.gridwidth;
        for (var i = 0; i < server_entities.length; i++) {
            if (server_entities[i].picture != null) {
              
                server_entities[i].img.src = '../../Images/' + server_entities[i].picture;
                Game.context.drawImage(server_entities[i].img, (server_entities[i].column ) * Game.tilewidth + (server_entities[i].column ) * Game.context.lineWidth, (server_entities[i].row) * Game.tileheight + (server_entities[i].row) * Game.context.lineWidth, Game.tilewidth, Game.tileheight);
            }
        }
    }


    Game.drawusers = function () {

        for (var i = 0; i < server_users.length; i++) {

            // A stroked tile is a player
            // next we draw the tile

            Game.context.strokeStyle = "#0000ff";
            Game.context.strokeRect( (server_users[i].column) * Game.bordertilewidth,  (server_users[i].row ) * Game.bordertileheight, Game.bordertilewidth, Game.bordertileheight);
            Game.context.strokeStyle = "#dddddd";
            Game.context.strokeRect( (server_users[i].column) * Game.bordertilewidth,  (server_users[i].row) * Game.bordertileheight, Game.bordertilewidth, Game.bordertileheight);
            Game.context.font = 'bold 12pt';
            Game.context.lineWidth = 1;
            // stroke color
            Game.context.strokeStyle = '#dddddd';
            Game.context.strokeText(server_users[i].name, (server_users[i].column ) * Game.bordertilewidth + 3, (server_users[i].row) * Game.bordertileheight + 10);

        }


    }

















    // logique du jeu
    //retourne true s'il ya eu un changement quelconque 
    //retourne false sinon
    Game.update = function () {
        var ret = false;
        // this.entities[0].update(this.context);

        Server.queryforupdate();
        if (Server.statushasbeenupdated) {
            ret = true;
            Server.statushasbeenupdated = false;
        }
       


        return ret;



    };

    //boucle principale du jeu 
    //l'algorithme suivant permet dans le cadre d'une exécution non continue de la boucle (grâce à window.setInterval)
    //d'exécuter plus souvent dans un rapport constant le dessin que la logique du jeu  
    Game.run = (function () {
        var loops = 0, skipTicks = 1000 / Game.fps, maxFrameSkip = 10, nextGameTick = (new Date).getTime(), updated=false;

        return function () {
            loops = 0;

            while ((new Date).getTime() > nextGameTick && loops < maxFrameSkip) {
                updated=Game.update();
                nextGameTick += skipTicks;
                if(updated)loops++;
            }

            if (loops != 0 ) Game.draw();
            

        };
    })();



    KeyBoard.left = function () {
        $.getJSON("/Game/Left/", { id: Game.id });
       

        
    };


    KeyBoard.right = function () {
        $.getJSON("/Game/Right/", { id: Game.id });
        
    };


    KeyBoard.up = function () {
        $.getJSON("/Game/Up/", { id: Game.id });
        
    };


    KeyBoard.down = function () {
        $.getJSON("/Game/Down/", { id: Game.id });
        
    };


    KeyBoard.bomb = function () {
        $.getJSON("/Game/Bomb/", { id: Game.id });
        
    };











    $(document).keydown(function (event) {


        var keycode = (event.keyCode ? event.keyCode : event.which);
    
        switch (keycode) {
            case 40:
                KeyBoard.down();
                event.preventDefault();
                break;
            case 38:
                KeyBoard.up();
                event.preventDefault();
                break;
            case 37:
                KeyBoard.left();
                event.preventDefault();
                break;
        
            case 39:
                KeyBoard.right();
                event.preventDefault();
                break;
        
            case 17:
                KeyBoard.bomb();
                event.preventDefault();
                break;

     

        }
    });

    
    

    Server.statushasbeenupdated = false;
    Server.queryforupdate = function () {

        

        $.getJSON("/Game/ServerState/", { id: Game.id }, function (data) {

            var items = [];
            $.each(data, function (key, val) {

                if (key == "SERVERRESPONSE_ENTITIES") {

             
                    if (val.length != 0) {
                        Game.entities = [];
                        for (i = 0; i < val.length; i++) {

                            Game.entities.push(new Entity(val[i].ENTITY_PICTURE, val[i].ENTITY_ROW,val[i].ENTITY_COLUMN));
                 
                        }

                    }
                }
                if (key == "SERVERRESPONSE_USERS") {
                    
                    if (val.length != 0) {

                        Game.users = [];
                        //id, name, score, life, level, bomb, row, column
                        for (i = 0; i < val.length; i++) {

                        Game.users.push(new User(val[i].USER_ID, val[i].USER_NAME, val[i].USER_SCORE, val[i].USER_LIFE, val[i].USER_LEVEL, val[i].USER_BOMB, val[i].USER_ROW, val[i].USER_COLUMN));
                        }
                        
                        }

                }
                if (key == "SERVERRESPONSE_GAMEINFO") {


                    Game.remainingtime = val.GAMEINFO_REMAININGTIME;
                   


                }


            });
        });



       
       


        Server.statushasbeenupdated = false;
        if (Game.remainingtime == 0) Server.statushasbeenupdated = true;





        if (Game.entities.length != server_entities.length) {
            Server.statushasbeenupdated = true;
          
        }
        else {
            for (var i = 0; i < Game.entities.length; i++) {

                if (Game.entities[i].picture != server_entities[i].picture || Game.entities[i].row != server_entities[i].row || Game.entities[i].column != server_entities[i].column) {
                    Server.statushasbeenupdated = true;
                
                    break;
                }

            }
        }



        if (Server.statushasbeenupdated) {
            server_entities = [];
            for (var i = 0; i < Game.entities.length; i++) {
                server_entities.push(Game.entities[i]);


            }

        }
       


        if (Game.users.length != server_users.length) {
            Server.statushasbeenupdated = true;
          
        }
        else {
            for (var i = 0; i < Game.users.length; i++) {

                if (Game.users[i].name != server_users[i].name || Game.users[i].row != server_users[i].row || Game.users[i].column != server_users[i].column) {
                    Server.statushasbeenupdated = true;
                   
                    break;
                }

            }
        }


       
        if (Server.statushasbeenupdated) {
            server_users = [];
            for (var i = 0; i < Game.users.length; i++) {
                server_users.push(Game.users[i]);
                

            }
            
        }


        displayUsers();
        displayTime();






    }

    function clearusers() {

        $("#tableUsers tbody").html("");

    }




    function displayUsers() {
        clearusers();
       
       
                       for (var i = 0; i < server_users.length; i++) {
                        

                           $("#tableUsers tbody").append("<tr><td>" +  server_users[i].id + "</td><td>" + server_users[i].name + "</td><td>" + server_users[i].score + "</td><td>" + server_users[i].life+ "</td><td>" + server_users[i].level + "</td><td>" + server_users[i].bomb + "</td></tr>");

             }             
                       
         
    }

    function cleartime() {

        $("#remainingtime").html("");

    }


    function displayTime() {
        cleartime();
        $("#remainingtime").append("<h2>" + Game.remainingtime + "</h2>");
        
    }