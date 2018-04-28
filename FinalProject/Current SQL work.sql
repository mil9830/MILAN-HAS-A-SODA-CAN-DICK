DROP SEQUENCE AdminPKSequence;
DROP sequence BRANDSPKSequence;
DROP sequence CATEGORIESPKSequence;
DROP sequence COMMENTSPKSequence;
DROP sequence INVOICEPKSequence;
DROP sequence ITEMSPKSequence;
DROP sequence RATINGSPKSequence; 
DROP sequence USERSPKSequence;

/*SELECT current_value From SYS.sequences where name = 'AdminPKSequence';*/
/* For checking to see if my sequences are working */ 

create sequence AdminPKSequence
AS INTEGER 
START WITH 0
INCREMENT BY 1; 

create sequence BRANDSPKSequence
AS INTEGER 
START WITH 0
INCREMENT BY 1;

create sequence CATEGORIESPKSequence
AS INTEGER 
START WITH 0
INCREMENT BY 1;

create sequence COMMENTSPKSequence
AS INTEGER 
START WITH 0
INCREMENT BY 1;

create sequence INVOICEPKSequence
AS INTEGER 
START WITH 0
INCREMENT BY 1;

create sequence ITEMSPKSequence
AS INTEGER 
START WITH 0
INCREMENT BY 1;

create sequence RATINGSPKSequence
AS INTEGER 
START WITH 0
INCREMENT BY 1;

create sequence USERSPKSequence
AS INTEGER 
START WITH 0
INCREMENT BY 1;

CREATE TABLE [dbo].[USERS] (
    [USER_ID]     INT           NOT NULL,
    [Username]    NVARCHAR (50) NULL,
    [password]    NVARCHAR (50) NULL,
    [age]         INT           NULL,
    [User_F_Name] NVARCHAR (50) NULL,
    [User_L_Name] NVARCHAR (50) NULL,
    CONSTRAINT [PK_USERS] PRIMARY KEY CLUSTERED ([USER_ID] ASC)
);

CREATE TABLE [dbo].[Admin] (
    [Admin_ID]       INT           NOT NULL,
    [admin_username] NVARCHAR (50) NULL,
    [admin_password] NVARCHAR (50) NULL,
    [Admin_lvl]      INT           NULL,
    PRIMARY KEY CLUSTERED ([Admin_ID] ASC)
);

CREATE TABLE [dbo].[ITEMS] (
    [ITEM_ID]   INT            NOT NULL,
    [BRAND_ID]  INT            NULL,
    [CAT_ID]    INT            NULL,
    [Price]     INT            NULL,
    [Item_DESC] NVARCHAR (MAX) NULL,
    [Horseys]   INT            NULL,
    [MPG]       INT            NULL,
    PRIMARY KEY CLUSTERED ([ITEM_ID] ASC)
);

CREATE TABLE [dbo].[INVOICE] (
    [INVOICE_ID]  INT NOT NULL,
    [USER_ID]     INT NULL,
    [Price_total] INT NULL,
    [ITEM_ID]     INT NOT NULL,
    CONSTRAINT [INVOICE_DUB_PRIMARY] PRIMARY KEY CLUSTERED ([INVOICE_ID] ASC),
    CONSTRAINT [INVOICE_ITEM_ID_FK] FOREIGN KEY ([ITEM_ID]) REFERENCES [dbo].[ITEMS] ([ITEM_ID]),
    CONSTRAINT [INVOICE_USER_ID_FK] FOREIGN KEY ([USER_ID]) REFERENCES [dbo].[USERS] ([USER_ID])
);

CREATE TABLE [dbo].[BRANDS] (
    [Brand_ID]   INT  NOT NULL,
    [ITEM_ID]    INT  NULL,
    [Brand_Name] TEXT NULL,
    PRIMARY KEY CLUSTERED ([Brand_ID] ASC),
    CONSTRAINT [BRANDS_ITEM_ID_FK] FOREIGN KEY ([ITEM_ID]) REFERENCES [dbo].[ITEMS] ([ITEM_ID])
);

CREATE TABLE [dbo].[CATEGORIES] (
    [CAT_ID]   INT           NOT NULL,
    [CAT_Name] NVARCHAR (50) NULL,
    [ITEM_ID]  INT           NULL,
    PRIMARY KEY CLUSTERED ([CAT_ID] ASC),
    CONSTRAINT [CATAGORIES_ITEM_ID_FK] FOREIGN KEY ([ITEM_ID]) REFERENCES [dbo].[ITEMS] ([ITEM_ID])
);

CREATE TABLE [dbo].[COMMENTS] (
    [COMMENT_ID] INT            NOT NULL,
    [INVOICE_ID] INT            NULL,
    [comment]    NVARCHAR (MAX) NULL,
    PRIMARY KEY CLUSTERED ([COMMENT_ID] ASC),
    CONSTRAINT [FK_COMMENTS_ToTable] FOREIGN KEY ([INVOICE_ID]) REFERENCES [dbo].[INVOICE] ([INVOICE_ID])
);


CREATE TABLE [dbo].[RATINGS] (
    [RAITING_ID] INT NOT NULL,
    [rating]     INT NULL,
    [INVOICE_ID] INT NULL,
    PRIMARY KEY CLUSTERED ([RAITING_ID] ASC),
    CONSTRAINT [RATINGS_INV_NUM_FK] FOREIGN KEY ([INVOICE_ID]) REFERENCES [dbo].[INVOICE] ([INVOICE_ID])
);

INSERT INTO Admin
( Admin_ID,admin_username,admin_password,Admin_lvl)
values
(NEXT VALUE FOR AdminPKSequence,'Admin1','admin1',1);

INSERT INTO Admin
( Admin_ID,admin_username,admin_password,Admin_lvl)
values
(NEXT VALUE FOR AdminPKSequence,'Admin1','admin1',1);

INSERT INTO USERS
(USER_ID,Username,password,age,User_F_Name,User_L_Name)
values
(NEXT VALUE FOR USERSPKSequence,'Hamms','Hamms',20,'Matthew','Yauch'); 

INSERT INTO USERS
(USER_ID,Username,password,age,User_F_Name,User_L_Name)
values
(NEXT VALUE FOR USERSPKSequence,'CLAP','CLAP',30,'Robert','Wheeler');

INSERT INTO USERS
(USER_ID,Username,password,age,User_F_Name,User_L_Name)
values
(NEXT VALUE FOR USERSPKSequence,'HighFunctioningAutism','HighFunctioningAutism',32,'Robert','Milan');

INSERT INTO ITEMS
(ITEM_ID,BRAND_ID,CAT_ID,Price,Item_DESC,Horseys,MPG)
VALUES
(NEXT VALUE FOR ITEMSPKSequence,1,1,46000,'Chevy Silverado',400,12);

INSERT INTO ITEMS
(ITEM_ID,BRAND_ID,CAT_ID,Price,Item_DESC,Horseys,MPG)
VALUES
(NEXT VALUE FOR ITEMSPKSequence,1,1,80000,'Tesla model S',400,12);

INSERT INTO ITEMS
(ITEM_ID,BRAND_ID,CAT_ID,Price,Item_DESC,Horseys,MPG)
VALUES
(NEXT VALUE FOR ITEMSPKSequence, 1,1,80000,'Ford F-150',400,12);

INSERT INTO ITEMS
(ITEM_ID,BRAND_ID,CAT_ID,Price,Item_DESC,Horseys,MPG)
VALUES
(NEXT VALUE FOR ITEMSPKSequence,1,1,80000,'Tesla model 3',400,12);

INSERT INTO INVOICE 
(INVOICE_ID,USER_ID,Price_total,ITEM_ID)
VALUES
();

INSERT INTO BRANDS
(Brand_ID, Brand_Name)
VALUES
(NEXT VALUE FOR BRANDSPKSequence,'Chevorlet');

INSERT INTO BRANDS
(Brand_ID, Brand_Name)
VALUES
(NEXT VALUE FOR BRANDSPKSequence,'Tesla');

INSERT INTO BRANDS
(Brand_ID, Brand_Name)
VALUES
(NEXT VALUE FOR BRANDSPKSequence,'Ford');

INSERT INTO CATEGORIES
(CAT_ID,CAT_Name)
VALUES 
(NEXT VALUE FOR CATEGORIESPKSequence,'Trucks'); 

INSERT INTO CATEGORIES
(CAT_ID,CAT_Name)
VALUES 
(NEXT VALUE FOR CATEGORIESPKSequence,'Preformance'); 

INSERT INTO CATEGORIES
(CAT_ID,CAT_Name)
VALUES 
(NEXT VALUE FOR CATEGORIESPKSequence,'sedan'); 

INSERT INTO COMMENTS
(COMMENT_ID, INVOICE_ID,comment)
VALUES
();

INSERT INTO RATINGS
(RAITING_ID,rating,INVOICE_ID)
VALUES
();




