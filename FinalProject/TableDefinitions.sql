﻿create sequence AdminPKSequence
AS INTEGER 
START WITH 1
INCREMENT BY 1; 

create sequence BRANDSPKSequence
AS INTEGER 
START WITH 1
INCREMENT BY 1;

create sequence CATEGORIESPKSequence
AS INTEGER 
START WITH 1
INCREMENT BY 1;

create sequence COMMENTSPKSequence
AS INTEGER 
START WITH 1
INCREMENT BY 1;

create sequence INVOICEPKSequence
AS INTEGER 
START WITH 1
INCREMENT BY 1;

create sequence ITEMSPKSequence
AS INTEGER 
START WITH 1
INCREMENT BY 1;

create sequence RATINGSPKSequence
AS INTEGER 
START WITH 1
INCREMENT BY 1;

create sequence USERSPKSequence
AS INTEGER 
START WITH 1
INCREMENT BY 1;


CREATE TABLE [dbo].[Admin] (
    [Admin_ID]       INT           NOT NULL,
    [admin_username] NVARCHAR (50) NULL,
    [admin_password] NVARCHAR (50) NULL,
    [Admin_lvl]      INT           NULL,
    PRIMARY KEY CLUSTERED ([Admin_ID] ASC)
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

CREATE TABLE [dbo].[INVOICE] (
    [INVOICE_ID]  INT NOT NULL,
    [USER_ID]     INT NULL,
    [Price_total] INT NULL,
    [ITEM_ID]     INT NOT NULL,
    CONSTRAINT [INVOICE_DUB_PRIMARY] PRIMARY KEY CLUSTERED ([INVOICE_ID] ASC),
    CONSTRAINT [INVOICE_ITEM_ID_FK] FOREIGN KEY ([ITEM_ID]) REFERENCES [dbo].[ITEMS] ([ITEM_ID]),
    CONSTRAINT [INVOICE_USER_ID_FK] FOREIGN KEY ([USER_ID]) REFERENCES [dbo].[USERS] ([USER_ID])
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

CREATE TABLE [dbo].[RATINGS] (
    [RAITING_ID] INT NOT NULL,
    [rating]     INT NULL,
    [INVOICE_ID] INT NULL,
    PRIMARY KEY CLUSTERED ([RAITING_ID] ASC),
    CONSTRAINT [RATINGS_INV_NUM_FK] FOREIGN KEY ([INVOICE_ID]) REFERENCES [dbo].[INVOICE] ([INVOICE_ID])
);

CREATE TABLE [dbo].[USERS] (
    [USER_ID]     INT           NOT NULL,
    [Username]    NVARCHAR (50) NULL,
    [password]    NVARCHAR (50) NULL,
    [age]         INT           NULL,
    [User_F_Name] NVARCHAR (50) NULL,
    [User_L_Name] NVARCHAR (50) NULL,
    CONSTRAINT [PK_USERS] PRIMARY KEY CLUSTERED ([USER_ID] ASC)
);

