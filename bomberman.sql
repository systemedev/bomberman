
-- Tables Section
-- _____________ 

IF OBJECT_ID('webpages_OAuthMembership', 'U') IS NOT NULL
DROP TABLE webpages_OAuthMembership;

IF OBJECT_ID('webpages_Membership', 'U') IS NOT NULL
DROP TABLE webpages_Membership;


IF OBJECT_ID('webpages_UsersInRoles', 'U') IS NOT NULL
DROP TABLE webpages_UsersInRoles;




IF OBJECT_ID('webpages_Roles', 'U') IS NOT NULL
DROP TABLE webpages_Roles;


IF OBJECT_ID('Locations', 'U') IS NOT NULL
DROP TABLE Locations;


IF OBJECT_ID('Games_details', 'U') IS NOT NULL
DROP TABLE Games_details;

IF OBJECT_ID('Cell_types', 'U') IS NOT NULL
DROP TABLE Cell_types;

IF OBJECT_ID('Games_requests', 'U') IS NOT NULL
DROP TABLE Games_requests;


IF OBJECT_ID('Users_play', 'U') IS NOT NULL
DROP TABLE Users_play;





IF OBJECT_ID('Games', 'U') IS NOT NULL
DROP TABLE Games;

IF OBJECT_ID('Maps', 'U') IS NOT NULL
DROP TABLE Maps;


IF OBJECT_ID('UserProfile', 'U') IS NOT NULL
DROP TABLE UserProfile;

CREATE TABLE UserProfile (
    UserId   INT           IDENTITY (1, 1) NOT NULL,
    UserName NVARCHAR (56) NOT NULL,
    PRIMARY KEY CLUSTERED (UserId ASC),
    UNIQUE NONCLUSTERED (UserName ASC)
);

CREATE TABLE webpages_Roles (
    RoleId   INT            IDENTITY (1, 1) NOT NULL,
    RoleName NVARCHAR (256) NOT NULL,
    PRIMARY KEY CLUSTERED (RoleId ASC),
    UNIQUE NONCLUSTERED (RoleName ASC)
);

CREATE TABLE webpages_UsersInRoles (
    UserId INT NOT NULL,
    RoleId INT NOT NULL,
    PRIMARY KEY CLUSTERED (UserId ASC, RoleId ASC),
    CONSTRAINT fk_UserId FOREIGN KEY (UserId) REFERENCES UserProfile (UserId),
    CONSTRAINT fk_RoleId FOREIGN KEY (RoleId) REFERENCES webpages_Roles (RoleId)
);


CREATE TABLE webpages_Membership (
    UserId                                  INT            NOT NULL,
    CreateDate                              DATETIME       NULL,
    ConfirmationToken                       NVARCHAR (128) NULL,
    IsConfirmed                             BIT            DEFAULT ((0)) NULL,
    LastPasswordFailureDate                 DATETIME       NULL,
    PasswordFailuresSinceLastSuccess        INT            DEFAULT ((0)) NOT NULL,
    Password                                NVARCHAR (128) NOT NULL,
    PasswordChangedDate                     DATETIME       NULL,
    PasswordSalt                            NVARCHAR (128) NOT NULL,
    PasswordVerificationToken               NVARCHAR (128) NULL,
    PasswordVerificationTokenExpirationDate DATETIME       NULL,
    PRIMARY KEY CLUSTERED (UserId ASC)
);

CREATE TABLE webpages_OAuthMembership (
    Provider       NVARCHAR (30)  NOT NULL,
    ProviderUserId NVARCHAR (100) NOT NULL,
    UserId         INT            NOT NULL,
    PRIMARY KEY CLUSTERED (Provider ASC, ProviderUserId ASC)
);






create table Cell_types(
     id int IDENTITY,
     exploded_type_to int,
	 walked_type_to int,
     name varchar(30) not null,
     representation varchar(100) not null,
     explodable bit not null,
     walkable bit not null,
     powerups_morebomb int not null,
     powerups_morelevel int not null,
     powerups_morelife int not null,
     score int not null,
     constraint ID_Cell_type primary key (id));

ALTER TABLE Cell_types
ADD CONSTRAINT Cell_typesexplosion_FK_Cell_types
 FOREIGN KEY(exploded_type_to) REFERENCES Cell_types(id);

ALTER TABLE Cell_types
ADD CONSTRAINT Cell_typeswalkon_FK_Cell_types
 FOREIGN KEY(walked_type_to) REFERENCES Cell_types(id);
 
 
 
create table Maps (
	id int IDENTITY,
    title VARCHAR(50) NOT NULL DEFAULT 'map title' , 
    maplevel int  NOT NULL DEFAULT 0,  
    configuration  VARCHAR(8000) NOT NULL DEFAULT 'map struct' , 
	constraint ID_Maps_ID primary key (id));


create table Games (
     id int IDENTITY,
     id_map int,
	 remaining_time int not null, -- remaining time in seconds
	 is_public bit not null,
     owner int not null,
	 title VARCHAR(50) NOT NULL DEFAULT 'game title' , 
     constraint ID_Games_ID primary key (id));
	 
alter table Games add constraint Games_FK_Maps
     foreign key (id_map)
     references Maps(id)
	 ON DELETE CASCADE;	 

	 
	 
alter table Games add constraint Games_FK_UserProfile
     foreign key (owner)
     references UserProfile(UserId);

create table Games_details (
     id int IDENTITY,
     location_row tinyint not null,
     location_column tinyint not null,
     cell_type_id int not null,
     game_id int not null,
     constraint ID_Games_details_ID primary key (id));

alter table Games_details add constraint Games_details_FK_Cell_types     foreign key (cell_type_id)
     references Cell_types(id)
	 ON DELETE CASCADE;

alter table Games_details add constraint Games_details_FK_Games
     foreign key (game_id)
     references Games(id)
	 ON DELETE CASCADE;

create table Locations (
     id int IDENTITY,
	 id_gamedet int not null,
     id_user int not null ,
     isabomb bit not null DEFAULT 0,
     bomb_timeout int not null DEFAULT 0,
     bomb_level int not null DEFAULT 0,
     isauser bit not null DEFAULT 0,
     constraint Locations_PK_gamedetuser primary key (id));

alter table Locations add constraint Locations_FK_Games_details     
	foreign key (id_gamedet)
     references Games_details(id)
	 ON DELETE CASCADE;

alter table Locations add constraint Locations_FK_UserProfile
     foreign key (id_user)
     references UserProfile(UserId)
	 

create table Users_play (
     id int IDENTITY,
	 id_game int not null,
     id_user int not null,
     life int not null,
     level int not null,
     bomb int not null,
     score int not null,
     constraint  Locations_PK_gameuser primary key (id));

alter table Users_play add constraint Users_play_FK_UserProfile
     foreign key (id_user)
     references UserProfile(UserId)
	 

alter table Users_play add constraint Users_play_FK_Games
     foreign key (id_game)
     references Games(id)
	 ON DELETE CASCADE;




create table Games_requests (
     id int IDENTITY,
	 from_id_user int not null,
	 to_id_user int not null,
	 id_game int not null,
     constraint Games_requests_PK primary key (id));



alter table Games_requests add constraint Games_requests_from_FK_UserProfile
     foreign key (from_id_user)
      references UserProfile(UserId)
	  

alter table Games_requests add constraint Games_requests_to_FK_UserProfile
     foreign key (to_id_user)
      references UserProfile(UserId);

alter table Games_requests add constraint  Games_requests_FK_Games
     foreign key (id_game)
     references Games(id)
	 ON DELETE CASCADE;

	 
SET IDENTITY_INSERT [dbo].[Maps] ON
INSERT INTO [dbo].[Maps] ([id], [title], [maplevel], [configuration]) VALUES (1, N'guerre des boutons', 1, N'MAPNUMBEROFROWS:10;MAPNUMBEROFCOLUMNS:10;MAPBACKGROUND:background.png')
INSERT INTO [dbo].[Maps] ([id], [title], [maplevel], [configuration]) VALUES (2, N'guerre du vietnam', 2, N'MAPNUMBEROFROWS:15;MAPNUMBEROFCOLUMNS:15;MAPBACKGROUND:background.png')
INSERT INTO [dbo].[Maps] ([id], [title], [maplevel], [configuration]) VALUES (3, N'guerre d''afghanistan', 3, N'MAPNUMBEROFROWS:20;MAPNUMBEROFCOLUMNS:20;MAPBACKGROUND:background.png')
SET IDENTITY_INSERT [dbo].[Maps] OFF
	 
SET IDENTITY_INSERT [dbo].[Cell_types] ON
INSERT INTO [dbo].[Cell_types] ([id], [exploded_type_to], [walked_type_to], [name], [representation], [explodable], [walkable], [powerups_morebomb], [powerups_morelevel], [powerups_morelife], [score]) VALUES (1, NULL, NULL, N'empty_place', N'empty.png', 1, 1, 0, 0, 0, 0)
INSERT INTO [dbo].[Cell_types] ([id], [exploded_type_to], [walked_type_to], [name], [representation], [explodable], [walkable], [powerups_morebomb], [powerups_morelevel], [powerups_morelife], [score]) VALUES (2, NULL, NULL, N'empty_place', N'empty.png', 1, 1, 0, 0, 0, 0)
INSERT INTO [dbo].[Cell_types] ([id], [exploded_type_to], [walked_type_to], [name], [representation], [explodable], [walkable], [powerups_morebomb], [powerups_morelevel], [powerups_morelife], [score]) VALUES (3, NULL, NULL, N'empty_place', N'empty.png', 1, 1, 0, 0, 0, 0)
INSERT INTO [dbo].[Cell_types] ([id], [exploded_type_to], [walked_type_to], [name], [representation], [explodable], [walkable], [powerups_morebomb], [powerups_morelevel], [powerups_morelife], [score]) VALUES (10, 2, 2, N'morebomb', N'giftbomb.png', 1, 1, 1, 0, 0, 10)
INSERT INTO [dbo].[Cell_types] ([id], [exploded_type_to], [walked_type_to], [name], [representation], [explodable], [walkable], [powerups_morebomb], [powerups_morelevel], [powerups_morelife], [score]) VALUES (11, 2, 2, N'morelevel', N'giftlevel.png', 1, 1, 0, 1, 0, 10)
INSERT INTO [dbo].[Cell_types] ([id], [exploded_type_to], [walked_type_to], [name], [representation], [explodable], [walkable], [powerups_morebomb], [powerups_morelevel], [powerups_morelife], [score]) VALUES (12, 2, 2, N'morelife', N'giftlife.png', 1, 1, 0, 0, 1, 10)
INSERT INTO [dbo].[Cell_types] ([id], [exploded_type_to], [walked_type_to], [name], [representation], [explodable], [walkable], [powerups_morebomb], [powerups_morelevel], [powerups_morelife], [score]) VALUES (4, 2, NULL, N'regular_block', N'wall.png', 1, 0, 0, 0, 0, 1)
INSERT INTO [dbo].[Cell_types] ([id], [exploded_type_to], [walked_type_to], [name], [representation], [explodable], [walkable], [powerups_morebomb], [powerups_morelevel], [powerups_morelife], [score]) VALUES (5, 10, NULL, N'morebomb_block', N'blockgift.png', 1, 0, 0, 0, 0, 1)
INSERT INTO [dbo].[Cell_types] ([id], [exploded_type_to], [walked_type_to], [name], [representation], [explodable], [walkable], [powerups_morebomb], [powerups_morelevel], [powerups_morelife], [score]) VALUES (7, 11, NULL, N'morelevel_block', N'blockgift.png', 1, 0, 0, 0, 0, 1)
INSERT INTO [dbo].[Cell_types] ([id], [exploded_type_to], [walked_type_to], [name], [representation], [explodable], [walkable], [powerups_morebomb], [powerups_morelevel], [powerups_morelife], [score]) VALUES (9, 12, NULL, N'morelife_block', N'blockgift.png', 1, 0, 0, 0, 0, 1)
INSERT INTO [dbo].[Cell_types] ([id], [exploded_type_to], [walked_type_to], [name], [representation], [explodable], [walkable], [powerups_morebomb], [powerups_morelevel], [powerups_morelife], [score]) VALUES (13, NULL, NULL, N'empty_place', N'empty.png', 1, 1, 0, 0, 0, 0)
INSERT INTO [dbo].[Cell_types] ([id], [exploded_type_to], [walked_type_to], [name], [representation], [explodable], [walkable], [powerups_morebomb], [powerups_morelevel], [powerups_morelife], [score]) VALUES (14, NULL, NULL, N'empty_place', N'empty.png', 1, 1, 0, 0, 0, 0)
INSERT INTO [dbo].[Cell_types] ([id], [exploded_type_to], [walked_type_to], [name], [representation], [explodable], [walkable], [powerups_morebomb], [powerups_morelevel], [powerups_morelife], [score]) VALUES (15, NULL, NULL, N'empty_place', N'empty.png', 1, 1, 0, 0, 0, 0)
INSERT INTO [dbo].[Cell_types] ([id], [exploded_type_to], [walked_type_to], [name], [representation], [explodable], [walkable], [powerups_morebomb], [powerups_morelevel], [powerups_morelife], [score]) VALUES (16, NULL, NULL, N'empty_place', N'empty.png', 1, 1, 0, 0, 0, 0)
INSERT INTO [dbo].[Cell_types] ([id], [exploded_type_to], [walked_type_to], [name], [representation], [explodable], [walkable], [powerups_morebomb], [powerups_morelevel], [powerups_morelife], [score]) VALUES (17, NULL, NULL, N'empty_place', N'empty.png', 1, 1, 0, 0, 0, 0)
INSERT INTO [dbo].[Cell_types] ([id], [exploded_type_to], [walked_type_to], [name], [representation], [explodable], [walkable], [powerups_morebomb], [powerups_morelevel], [powerups_morelife], [score]) VALUES (18, NULL, NULL, N'empty_place', N'empty.png', 1, 1, 0, 0, 0, 0)
INSERT INTO [dbo].[Cell_types] ([id], [exploded_type_to], [walked_type_to], [name], [representation], [explodable], [walkable], [powerups_morebomb], [powerups_morelevel], [powerups_morelife], [score]) VALUES (19, NULL, NULL, N'empty_place', N'empty.png', 1, 1, 0, 0, 0, 0)
INSERT INTO [dbo].[Cell_types] ([id], [exploded_type_to], [walked_type_to], [name], [representation], [explodable], [walkable], [powerups_morebomb], [powerups_morelevel], [powerups_morelife], [score]) VALUES (20, NULL, NULL, N'empty_place', N'empty.png', 1, 1, 0, 0, 0, 0)
INSERT INTO [dbo].[Cell_types] ([id], [exploded_type_to], [walked_type_to], [name], [representation], [explodable], [walkable], [powerups_morebomb], [powerups_morelevel], [powerups_morelife], [score]) VALUES (21, NULL, NULL, N'empty_place', N'empty.png', 1, 1, 0, 0, 0, 0)
INSERT INTO [dbo].[Cell_types] ([id], [exploded_type_to], [walked_type_to], [name], [representation], [explodable], [walkable], [powerups_morebomb], [powerups_morelevel], [powerups_morelife], [score]) VALUES (22, NULL, NULL, N'empty_place', N'empty.png', 1, 1, 0, 0, 0, 0)
INSERT INTO [dbo].[Cell_types] ([id], [exploded_type_to], [walked_type_to], [name], [representation], [explodable], [walkable], [powerups_morebomb], [powerups_morelevel], [powerups_morelife], [score]) VALUES (23, NULL, NULL, N'empty_place', N'empty.png', 1, 1, 0, 0, 0, 0)
INSERT INTO [dbo].[Cell_types] ([id], [exploded_type_to], [walked_type_to], [name], [representation], [explodable], [walkable], [powerups_morebomb], [powerups_morelevel], [powerups_morelife], [score]) VALUES (24, NULL, NULL, N'empty_place', N'empty.png', 1, 1, 0, 0, 0, 0)
INSERT INTO [dbo].[Cell_types] ([id], [exploded_type_to], [walked_type_to], [name], [representation], [explodable], [walkable], [powerups_morebomb], [powerups_morelevel], [powerups_morelife], [score]) VALUES (25, NULL, NULL, N'empty_place', N'empty.png', 1, 1, 0, 0, 0, 0)
INSERT INTO [dbo].[Cell_types] ([id], [exploded_type_to], [walked_type_to], [name], [representation], [explodable], [walkable], [powerups_morebomb], [powerups_morelevel], [powerups_morelife], [score]) VALUES (26, NULL, NULL, N'empty_place', N'empty.png', 1, 1, 0, 0, 0, 0)
INSERT INTO [dbo].[Cell_types] ([id], [exploded_type_to], [walked_type_to], [name], [representation], [explodable], [walkable], [powerups_morebomb], [powerups_morelevel], [powerups_morelife], [score]) VALUES (27, NULL, NULL, N'empty_place', N'empty.png', 1, 1, 0, 0, 0, 0)
INSERT INTO [dbo].[Cell_types] ([id], [exploded_type_to], [walked_type_to], [name], [representation], [explodable], [walkable], [powerups_morebomb], [powerups_morelevel], [powerups_morelife], [score]) VALUES (28, NULL, NULL, N'empty_place', N'empty.png', 1, 1, 0, 0, 0, 0)
INSERT INTO [dbo].[Cell_types] ([id], [exploded_type_to], [walked_type_to], [name], [representation], [explodable], [walkable], [powerups_morebomb], [powerups_morelevel], [powerups_morelife], [score]) VALUES (29, NULL, NULL, N'empty_place', N'empty.png', 1, 1, 0, 0, 0, 0)
INSERT INTO [dbo].[Cell_types] ([id], [exploded_type_to], [walked_type_to], [name], [representation], [explodable], [walkable], [powerups_morebomb], [powerups_morelevel], [powerups_morelife], [score]) VALUES (30, NULL, NULL, N'empty_place', N'empty.png', 1, 1, 0, 0, 0, 0)



SET IDENTITY_INSERT [dbo].[Cell_types] OFF	 

SET IDENTITY_INSERT [dbo].[UserProfile] ON 
INSERT [dbo].[UserProfile] ([UserId], [UserName]) VALUES (1, N'david')
INSERT [dbo].[UserProfile] ([UserId], [UserName]) VALUES (2, N'nicolas')
INSERT [dbo].[UserProfile] ([UserId], [UserName]) VALUES (3, N'jean-francois')
INSERT [dbo].[UserProfile] ([UserId], [UserName]) VALUES (4, N'jacques')


SET IDENTITY_INSERT [dbo].[UserProfile] OFF
INSERT [dbo].[webpages_Membership] ([UserId], [CreateDate], [ConfirmationToken], [IsConfirmed], [LastPasswordFailureDate], [PasswordFailuresSinceLastSuccess], [Password], [PasswordChangedDate], [PasswordSalt], [PasswordVerificationToken], [PasswordVerificationTokenExpirationDate]) VALUES (1, CAST(0x0000A24E00FAC476 AS DateTime), NULL, 1, NULL, 0, N'AINMWJkh0w+R0bqnmoDctDIEOnvl/AQS5E/1fWJPDeY0n+r3JmfOtQSHNh17NGFnbg==', CAST(0x0000A24E00FAC476 AS DateTime), N'', NULL, NULL)
INSERT [dbo].[webpages_Membership] ([UserId], [CreateDate], [ConfirmationToken], [IsConfirmed], [LastPasswordFailureDate], [PasswordFailuresSinceLastSuccess], [Password], [PasswordChangedDate], [PasswordSalt], [PasswordVerificationToken], [PasswordVerificationTokenExpirationDate]) VALUES (2, CAST(0x0000A24E0117787B AS DateTime), NULL, 1, NULL, 0, N'ACpkr/0bs0fPai4Z9KnVrRblyakbIqc5JWMO3JTVKtpkldpeJE9NbODQdfbUpb4FVw==', CAST(0x0000A24E0117787B AS DateTime), N'', NULL, NULL)
INSERT [dbo].[webpages_Membership] ([UserId], [CreateDate], [ConfirmationToken], [IsConfirmed], [LastPasswordFailureDate], [PasswordFailuresSinceLastSuccess], [Password], [PasswordChangedDate], [PasswordSalt], [PasswordVerificationToken], [PasswordVerificationTokenExpirationDate]) VALUES (3, CAST(0x0000A24E0117787B AS DateTime), NULL, 1, NULL, 0, N'ACpkr/0bs0fPai4Z9KnVrRblyakbIqc5JWMO3JTVKtpkldpeJE9NbODQdfbUpb4FVw==', CAST(0x0000A24E0117787B AS DateTime), N'', NULL, NULL)
INSERT [dbo].[webpages_Membership] ([UserId], [CreateDate], [ConfirmationToken], [IsConfirmed], [LastPasswordFailureDate], [PasswordFailuresSinceLastSuccess], [Password], [PasswordChangedDate], [PasswordSalt], [PasswordVerificationToken], [PasswordVerificationTokenExpirationDate]) VALUES (4, CAST(0x0000A24E0117787B AS DateTime), NULL, 1, NULL, 0, N'ACpkr/0bs0fPai4Z9KnVrRblyakbIqc5JWMO3JTVKtpkldpeJE9NbODQdfbUpb4FVw==', CAST(0x0000A24E0117787B AS DateTime), N'', NULL, NULL)


	 
