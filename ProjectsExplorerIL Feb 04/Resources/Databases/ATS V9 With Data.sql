USE [master]
GO
/****** Object:  Database [ATS]    Script Date: 12/19/2013 6:21:40 PM ******/
CREATE DATABASE [ATS]
 CONTAINMENT = NONE
 ON  PRIMARY 
( NAME = N'ATS', FILENAME = N'c:\Program Files\Microsoft SQL Server\MSSQL11.SQLEXPRESS\MSSQL\DATA\ATS.mdf' , SIZE = 3136KB , MAXSIZE = UNLIMITED, FILEGROWTH = 1024KB )
 LOG ON 
( NAME = N'ATS_log', FILENAME = N'c:\Program Files\Microsoft SQL Server\MSSQL11.SQLEXPRESS\MSSQL\DATA\ATS_log.ldf' , SIZE = 832KB , MAXSIZE = 2048GB , FILEGROWTH = 10%)
GO
ALTER DATABASE [ATS] SET COMPATIBILITY_LEVEL = 110
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [ATS].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [ATS] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [ATS] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [ATS] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [ATS] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [ATS] SET ARITHABORT OFF 
GO
ALTER DATABASE [ATS] SET AUTO_CLOSE ON 
GO
ALTER DATABASE [ATS] SET AUTO_CREATE_STATISTICS ON 
GO
ALTER DATABASE [ATS] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [ATS] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [ATS] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [ATS] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [ATS] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [ATS] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [ATS] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [ATS] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [ATS] SET  ENABLE_BROKER 
GO
ALTER DATABASE [ATS] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [ATS] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [ATS] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [ATS] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [ATS] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [ATS] SET READ_COMMITTED_SNAPSHOT OFF 
GO
ALTER DATABASE [ATS] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [ATS] SET RECOVERY SIMPLE 
GO
ALTER DATABASE [ATS] SET  MULTI_USER 
GO
ALTER DATABASE [ATS] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [ATS] SET DB_CHAINING OFF 
GO
ALTER DATABASE [ATS] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO
ALTER DATABASE [ATS] SET TARGET_RECOVERY_TIME = 0 SECONDS 
GO
USE [ATS]
GO
/****** Object:  Table [dbo].[ATS_FolderCertificate]    Script Date: 12/19/2013 6:21:40 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ATS_FolderCertificate](
	[Rowid] [int] IDENTITY(100000,1) NOT NULL,
	[HierarchyId] [int] NOT NULL,
	[CertificateId] [nvarchar](5) NULL,
	[EffectiveDate] [datetime] NOT NULL,
	[ExpirationDate] [datetime] NULL,
	[LastUpdateTime] [datetime] NOT NULL,
	[LastUpdateUser] [nvarchar](50) NOT NULL,
	[LastUpdateComputer] [nvarchar](50) NOT NULL,
	[LastUpdateAppliation] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_FolderCertificate] PRIMARY KEY CLUSTERED 
(
	[Rowid] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[ATS_Hierarchy]    Script Date: 12/19/2013 6:21:40 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ATS_Hierarchy](
	[Id] [int] IDENTITY(100000,1) NOT NULL,
	[ParentId] [int] NULL,
	[Sequence] [int] NULL,
	[NodeType] [nchar](1) NOT NULL,
	[Name] [nvarchar](50) NOT NULL,
	[Description] [nvarchar](500) NOT NULL,
	[GroupId] [int] NULL,
	[ProjectStep] [nchar](20) NULL,
	[ProjectCode] [nchar](20) NULL,
	[ProjectStatus] [nchar](1) NULL,
	[CreationDate] [datetime] NOT NULL,
	[LastUpdateTime] [datetime] NOT NULL,
	[LastUpdateUser] [nvarchar](50) NOT NULL,
	[LastUpdateComputer] [nvarchar](50) NOT NULL,
	[LastUpdateApplication] [nvarchar](50) NOT NULL,
	[SCDUSASyncInd] [bit] NULL,
 CONSTRAINT [PK_ProjectHierarchy] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[ATS_Messages]    Script Date: 12/19/2013 6:21:40 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ATS_Messages](
	[Id] [int] NOT NULL,
	[Description] [nvarchar](500) NOT NULL,
	[Type] [nchar](2) NOT NULL,
	[LastUpdateTime] [datetime] NOT NULL,
	[LastUpdateUser] [nvarchar](50) NOT NULL,
	[LastUpdateComputer] [nvarchar](50) NOT NULL,
	[LastUpdateApplication] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_Messages] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[ATS_MessageType]    Script Date: 12/19/2013 6:21:40 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ATS_MessageType](
	[Type] [nchar](2) NOT NULL,
	[Description] [nvarchar](500) NOT NULL,
	[LastUpdateTime] [datetime] NOT NULL,
	[LastUpdateUser] [nvarchar](50) NOT NULL,
	[LastUpdateComputer] [nvarchar](50) NOT NULL,
	[LastUpdateApplication] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_MessageType] PRIMARY KEY CLUSTERED 
(
	[Type] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[ATS_Note]    Script Date: 12/19/2013 6:21:40 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ATS_Note](
	[Id] [int] IDENTITY(100000,1) NOT NULL,
	[HierarchyId] [int] NOT NULL,
	[NoteType] [nchar](2) NOT NULL,
	[NoteStatus] [nchar](1) NOT NULL,
	[NoteTitle] [nvarchar](50) NOT NULL,
	[NoteText] [nvarchar](2000) NOT NULL,
	[SpecialInd] [bit] NULL,
	[CreatedByUser] [nvarchar](50) NOT NULL,
	[CreationDate] [datetime] NOT NULL,
	[LastUpdateTime] [datetime] NOT NULL,
	[LastUpdateUser] [nvarchar](50) NOT NULL,
	[LastUpdateComputer] [nvarchar](50) NOT NULL,
	[LastUpdateapplication] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_FolderNote] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[ATS_NoteType]    Script Date: 12/19/2013 6:21:40 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ATS_NoteType](
	[NoteType] [nchar](2) NOT NULL,
	[Description] [nvarchar](500) NOT NULL,
	[LastUpdateTime] [datetime] NULL,
	[LastUpdateUser] [nvarchar](50) NULL,
	[LastUpdateComputer] [nvarchar](50) NULL,
	[LastUpdateapplication] [nvarchar](50) NULL,
 CONSTRAINT [PK_NoteType] PRIMARY KEY CLUSTERED 
(
	[NoteType] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[ATS_Privilege]    Script Date: 12/19/2013 6:21:40 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ATS_Privilege](
	[PrivilegeCode] [nvarchar](50) NOT NULL,
	[PrivilegeDescription] [nvarchar](500) NOT NULL,
	[LastUpdateTime] [datetime] NOT NULL,
	[LastUpdateUser] [nvarchar](50) NOT NULL,
	[LastUpdateComputer] [nvarchar](50) NOT NULL,
	[LastUpdateApplication] [nvarchar](50) NOT NULL,
	[SubSystem] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_Privileges] PRIMARY KEY CLUSTERED 
(
	[PrivilegeCode] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[ATS_Profile]    Script Date: 12/19/2013 6:21:40 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ATS_Profile](
	[RowId] [int] NOT NULL,
	[ProfileDescription] [nvarchar](50) NOT NULL,
	[LastUpdateTime] [datetime] NOT NULL,
	[LastUpdateUser] [nvarchar](50) NOT NULL,
	[LastUpdateComputer] [nvarchar](50) NOT NULL,
	[LastUpdateApplication] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_Profile] PRIMARY KEY CLUSTERED 
(
	[RowId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[ATS_ProfilePrivilege]    Script Date: 12/19/2013 6:21:40 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ATS_ProfilePrivilege](
	[RowId] [int] NOT NULL,
	[ProfileId] [int] NOT NULL,
	[PrivilegeCode] [nvarchar](50) NOT NULL,
	[LastUpdateTime] [datetime] NULL,
	[LastUpdateUser] [nvarchar](50) NULL,
	[LastUpdateComputer] [nvarchar](50) NULL,
	[LastUpdateApplicationr] [nvarchar](50) NULL,
 CONSTRAINT [PK_ProfilePrivilege] PRIMARY KEY CLUSTERED 
(
	[RowId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[ATS_ProjectStep]    Script Date: 12/19/2013 6:21:40 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ATS_ProjectStep](
	[StepCode] [nchar](20) NOT NULL,
	[StepDescription] [nvarchar](500) NULL,
	[LastUpdateTime] [datetime] NOT NULL,
	[LastUpdateUser] [nvarchar](50) NOT NULL,
	[LastUpdateComputer] [nvarchar](50) NOT NULL,
	[LastUpdateapplication] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_ProjectStep] PRIMARY KEY CLUSTERED 
(
	[StepCode] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[ATS_SchemaVersion]    Script Date: 12/19/2013 6:21:40 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ATS_SchemaVersion](
	[VersionNo] [int] NOT NULL,
	[Date] [datetime] NOT NULL
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[ATS_SystemParameters]    Script Date: 12/19/2013 6:21:40 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ATS_SystemParameters](
	[Variable] [nvarchar](50) NOT NULL,
	[Description] [nvarchar](500) NOT NULL,
	[Type] [nvarchar](50) NOT NULL,
	[Value] [nvarchar](500) NOT NULL,
	[LastUpdateTime] [datetime] NOT NULL,
	[LastUpdateUser] [nvarchar](50) NOT NULL,
	[LastUpdateComputer] [nvarchar](50) NOT NULL,
	[LastUpdateapplication] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_VariableName] PRIMARY KEY CLUSTERED 
(
	[Variable] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[ATS_UserProfile]    Script Date: 12/19/2013 6:21:40 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ATS_UserProfile](
	[RowId] [int] NOT NULL,
	[UserId] [nvarchar](50) NOT NULL,
	[ProfileId] [int] NOT NULL,
	[EffectiveDate] [nchar](10) NOT NULL,
	[ExpirationDate] [nchar](10) NULL,
	[LastUpdateTime] [datetime] NOT NULL,
	[LastUpdateUser] [nvarchar](50) NOT NULL,
	[LastUpdateComputer] [nvarchar](50) NOT NULL,
	[LastUpdateApplicationr] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_UserProfile] PRIMARY KEY CLUSTERED 
(
	[RowId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[ATS_Version]    Script Date: 12/19/2013 6:21:40 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ATS_Version](
	[VersionId] [int] IDENTITY(100000,1) NOT NULL,
	[HierarchyId] [int] NOT NULL,
	[VersionName] [nvarchar](50) NOT NULL,
	[VersionSeqNo] [int] NOT NULL,
	[VersionStatus] [nchar](1) NOT NULL,
	[Description] [nvarchar](500) NOT NULL,
	[TargetPath] [nvarchar](max) NOT NULL,
	[CreationDate] [datetime] NOT NULL,
	[LastUpdateTime] [datetime] NOT NULL,
	[LastUpdateUser] [nvarchar](50) NOT NULL,
	[LastUpdateComputer] [nvarchar](50) NOT NULL,
	[LastUpdateapplication] [nvarchar](50) NOT NULL,
	[DefaultTargetPathInd] [bit] NULL,
 CONSTRAINT [PK_ProjectVersion] PRIMARY KEY CLUSTERED 
(
	[VersionId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[ATS_VersionContent]    Script Date: 12/19/2013 6:21:40 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ATS_VersionContent](
	[Rowid] [int] IDENTITY(100000,1) NOT NULL,
	[VersionId] [int] NOT NULL,
	[ContentVersionId] [int] NOT NULL,
	[ContentSeqNo] [int] NOT NULL,
	[CreationDate] [datetime] NOT NULL,
	[LastUpdateTime] [datetime] NOT NULL,
	[LastUpdateUser] [nvarchar](50) NOT NULL,
	[LastUpdateComputer] [nvarchar](50) NOT NULL,
	[LastUpdateApplication] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_VersionContent] PRIMARY KEY CLUSTERED 
(
	[Rowid] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET IDENTITY_INSERT [dbo].[ATS_Hierarchy] ON 

GO
INSERT [dbo].[ATS_Hierarchy] ([Id], [ParentId], [Sequence], [NodeType], [Name], [Description], [GroupId], [ProjectStep], [ProjectCode], [ProjectStatus], [CreationDate], [LastUpdateTime], [LastUpdateUser], [LastUpdateComputer], [LastUpdateApplication], [SCDUSASyncInd]) VALUES (100038, NULL, 10, N'F', N'Folder #1 Mod', N'Folder #1 Long Description', NULL, NULL, NULL, NULL, CAST(0x0000A26400BF4A85 AS DateTime), CAST(0x0000A28500A1E76A AS DateTime), N'SysAdmin', N'ES-W520', N'Explorer', NULL)
GO
INSERT [dbo].[ATS_Hierarchy] ([Id], [ParentId], [Sequence], [NodeType], [Name], [Description], [GroupId], [ProjectStep], [ProjectCode], [ProjectStatus], [CreationDate], [LastUpdateTime], [LastUpdateUser], [LastUpdateComputer], [LastUpdateApplication], [SCDUSASyncInd]) VALUES (100039, NULL, 10, N'F', N'Folder #2', N'Folder #2 Long Description', NULL, NULL, NULL, NULL, CAST(0x0000A26400BF69E5 AS DateTime), CAST(0x0000A26400BF7F00 AS DateTime), N'SysAdmin', N'ES-W520', N'Explorer', NULL)
GO
INSERT [dbo].[ATS_Hierarchy] ([Id], [ParentId], [Sequence], [NodeType], [Name], [Description], [GroupId], [ProjectStep], [ProjectCode], [ProjectStatus], [CreationDate], [LastUpdateTime], [LastUpdateUser], [LastUpdateComputer], [LastUpdateApplication], [SCDUSASyncInd]) VALUES (100040, NULL, 10, N'F', N'Folder #3', N'Folder #3 very very long description', NULL, NULL, NULL, NULL, CAST(0x0000A26400BF83BA AS DateTime), CAST(0x0000A26400BF9E8F AS DateTime), N'SysAdmin', N'ES-W520', N'Explorer', NULL)
GO
INSERT [dbo].[ATS_Hierarchy] ([Id], [ParentId], [Sequence], [NodeType], [Name], [Description], [GroupId], [ProjectStep], [ProjectCode], [ProjectStatus], [CreationDate], [LastUpdateTime], [LastUpdateUser], [LastUpdateComputer], [LastUpdateApplication], [SCDUSASyncInd]) VALUES (100041, 100038, 10, N'F', N'Folder #1A', N'', NULL, NULL, NULL, NULL, CAST(0x0000A26400BFA904 AS DateTime), CAST(0x0000A28500AD992F AS DateTime), N'SysAdmin', N'ES-W520', N'Explorer', NULL)
GO
INSERT [dbo].[ATS_Hierarchy] ([Id], [ParentId], [Sequence], [NodeType], [Name], [Description], [GroupId], [ProjectStep], [ProjectCode], [ProjectStatus], [CreationDate], [LastUpdateTime], [LastUpdateUser], [LastUpdateComputer], [LastUpdateApplication], [SCDUSASyncInd]) VALUES (100042, 100038, 10, N'F', N'Folder #1B', N'', NULL, NULL, NULL, NULL, CAST(0x0000A26400BFBD8D AS DateTime), CAST(0x0000A28500AD67BE AS DateTime), N'SysAdmin', N'ES-W520', N'Explorer', NULL)
GO
INSERT [dbo].[ATS_Hierarchy] ([Id], [ParentId], [Sequence], [NodeType], [Name], [Description], [GroupId], [ProjectStep], [ProjectCode], [ProjectStatus], [CreationDate], [LastUpdateTime], [LastUpdateUser], [LastUpdateComputer], [LastUpdateApplication], [SCDUSASyncInd]) VALUES (100043, 100038, 10, N'F', N'Folder #1C', N'', NULL, NULL, NULL, NULL, CAST(0x0000A26400BFCFA8 AS DateTime), CAST(0x0000A26400C19D85 AS DateTime), N'SysAdmin', N'ES-W520', N'Explorer', NULL)
GO
INSERT [dbo].[ATS_Hierarchy] ([Id], [ParentId], [Sequence], [NodeType], [Name], [Description], [GroupId], [ProjectStep], [ProjectCode], [ProjectStatus], [CreationDate], [LastUpdateTime], [LastUpdateUser], [LastUpdateComputer], [LastUpdateApplication], [SCDUSASyncInd]) VALUES (100044, 100039, 10, N'F', N'Folder #2A', N'', NULL, NULL, NULL, NULL, CAST(0x0000A26400BFDF53 AS DateTime), CAST(0x0000A26400BFF28D AS DateTime), N'SysAdmin', N'ES-W520', N'Explorer', NULL)
GO
INSERT [dbo].[ATS_Hierarchy] ([Id], [ParentId], [Sequence], [NodeType], [Name], [Description], [GroupId], [ProjectStep], [ProjectCode], [ProjectStatus], [CreationDate], [LastUpdateTime], [LastUpdateUser], [LastUpdateComputer], [LastUpdateApplication], [SCDUSASyncInd]) VALUES (100045, 100040, 10, N'F', N'Folder #3A', N'', NULL, NULL, NULL, NULL, CAST(0x0000A26400BFF63D AS DateTime), CAST(0x0000A26400C00372 AS DateTime), N'SysAdmin', N'ES-W520', N'Explorer', NULL)
GO
INSERT [dbo].[ATS_Hierarchy] ([Id], [ParentId], [Sequence], [NodeType], [Name], [Description], [GroupId], [ProjectStep], [ProjectCode], [ProjectStatus], [CreationDate], [LastUpdateTime], [LastUpdateUser], [LastUpdateComputer], [LastUpdateApplication], [SCDUSASyncInd]) VALUES (100046, 100041, 10, N'F', N'Folder #1A.i', N'', NULL, NULL, NULL, NULL, CAST(0x0000A26400C0093C AS DateTime), CAST(0x0000A26400C035F7 AS DateTime), N'SysAdmin', N'ES-W520', N'Explorer', NULL)
GO
INSERT [dbo].[ATS_Hierarchy] ([Id], [ParentId], [Sequence], [NodeType], [Name], [Description], [GroupId], [ProjectStep], [ProjectCode], [ProjectStatus], [CreationDate], [LastUpdateTime], [LastUpdateUser], [LastUpdateComputer], [LastUpdateApplication], [SCDUSASyncInd]) VALUES (100047, 100041, 10, N'F', N'Folder #1A.ii', N'', NULL, NULL, NULL, NULL, CAST(0x0000A26400C03A86 AS DateTime), CAST(0x0000A26400C04981 AS DateTime), N'SysAdmin', N'ES-W520', N'Explorer', NULL)
GO
INSERT [dbo].[ATS_Hierarchy] ([Id], [ParentId], [Sequence], [NodeType], [Name], [Description], [GroupId], [ProjectStep], [ProjectCode], [ProjectStatus], [CreationDate], [LastUpdateTime], [LastUpdateUser], [LastUpdateComputer], [LastUpdateApplication], [SCDUSASyncInd]) VALUES (101038, NULL, 10, N'F', N'Folder #4', N'Folder #4 Description', NULL, NULL, NULL, NULL, CAST(0x0000A26400C2042E AS DateTime), CAST(0x0000A26400C92E66 AS DateTime), N'SysAdmin', N'ES-W520', N'Explorer', NULL)
GO
INSERT [dbo].[ATS_Hierarchy] ([Id], [ParentId], [Sequence], [NodeType], [Name], [Description], [GroupId], [ProjectStep], [ProjectCode], [ProjectStatus], [CreationDate], [LastUpdateTime], [LastUpdateUser], [LastUpdateComputer], [LastUpdateApplication], [SCDUSASyncInd]) VALUES (101039, 101038, 10, N'F', N'Folder #4A', N'', NULL, NULL, NULL, NULL, CAST(0x0000A26400C20458 AS DateTime), CAST(0x0000A26400C23804 AS DateTime), N'SysAdmin', N'ES-W520', N'Explorer', NULL)
GO
INSERT [dbo].[ATS_Hierarchy] ([Id], [ParentId], [Sequence], [NodeType], [Name], [Description], [GroupId], [ProjectStep], [ProjectCode], [ProjectStatus], [CreationDate], [LastUpdateTime], [LastUpdateUser], [LastUpdateComputer], [LastUpdateApplication], [SCDUSASyncInd]) VALUES (101040, 101039, 10, N'F', N'Folder #4A.i', N'', NULL, NULL, NULL, NULL, CAST(0x0000A26400C2047E AS DateTime), CAST(0x0000A26400C23E94 AS DateTime), N'SysAdmin', N'ES-W520', N'Explorer', NULL)
GO
INSERT [dbo].[ATS_Hierarchy] ([Id], [ParentId], [Sequence], [NodeType], [Name], [Description], [GroupId], [ProjectStep], [ProjectCode], [ProjectStatus], [CreationDate], [LastUpdateTime], [LastUpdateUser], [LastUpdateComputer], [LastUpdateApplication], [SCDUSASyncInd]) VALUES (101041, 101039, 10, N'F', N'Folder #4A.ii', N'', NULL, NULL, NULL, NULL, CAST(0x0000A26400C204A3 AS DateTime), CAST(0x0000A26400C2469F AS DateTime), N'SysAdmin', N'ES-W520', N'Explorer', NULL)
GO
INSERT [dbo].[ATS_Hierarchy] ([Id], [ParentId], [Sequence], [NodeType], [Name], [Description], [GroupId], [ProjectStep], [ProjectCode], [ProjectStatus], [CreationDate], [LastUpdateTime], [LastUpdateUser], [LastUpdateComputer], [LastUpdateApplication], [SCDUSASyncInd]) VALUES (101042, 101038, 10, N'F', N'Folder #4B', N'', NULL, NULL, NULL, NULL, CAST(0x0000A26400C204C4 AS DateTime), CAST(0x0000A2650122FF0E AS DateTime), N'SysAdmin', N'ES-W520', N'Explorer', NULL)
GO
INSERT [dbo].[ATS_Hierarchy] ([Id], [ParentId], [Sequence], [NodeType], [Name], [Description], [GroupId], [ProjectStep], [ProjectCode], [ProjectStatus], [CreationDate], [LastUpdateTime], [LastUpdateUser], [LastUpdateComputer], [LastUpdateApplication], [SCDUSASyncInd]) VALUES (101043, 101038, 10, N'F', N'Folder #4C', N'', NULL, NULL, NULL, NULL, CAST(0x0000A26400C204E9 AS DateTime), CAST(0x0000A264011212CC AS DateTime), N'SysAdmin', N'ES-W520', N'Explorer', NULL)
GO
INSERT [dbo].[ATS_Hierarchy] ([Id], [ParentId], [Sequence], [NodeType], [Name], [Description], [GroupId], [ProjectStep], [ProjectCode], [ProjectStatus], [CreationDate], [LastUpdateTime], [LastUpdateUser], [LastUpdateComputer], [LastUpdateApplication], [SCDUSASyncInd]) VALUES (101044, 101038, 10, N'F', N'Folder #4D', N'', NULL, NULL, NULL, NULL, CAST(0x0000A26400C25ED1 AS DateTime), CAST(0x0000A26900BBEC27 AS DateTime), N'SysAdmin', N'ES-W520', N'Explorer', NULL)
GO
INSERT [dbo].[ATS_Hierarchy] ([Id], [ParentId], [Sequence], [NodeType], [Name], [Description], [GroupId], [ProjectStep], [ProjectCode], [ProjectStatus], [CreationDate], [LastUpdateTime], [LastUpdateUser], [LastUpdateComputer], [LastUpdateApplication], [SCDUSASyncInd]) VALUES (101045, 101044, 10, N'F', N'Folder #4d.1', N'', NULL, NULL, NULL, NULL, CAST(0x0000A266012423BA AS DateTime), CAST(0x0000A26601244BAD AS DateTime), N'SysAdmin', N'ES-W520', N'Explorer', NULL)
GO
INSERT [dbo].[ATS_Hierarchy] ([Id], [ParentId], [Sequence], [NodeType], [Name], [Description], [GroupId], [ProjectStep], [ProjectCode], [ProjectStatus], [CreationDate], [LastUpdateTime], [LastUpdateUser], [LastUpdateComputer], [LastUpdateApplication], [SCDUSASyncInd]) VALUES (101046, 101044, 10, N'F', N'Folder 4d.2', N'', NULL, NULL, NULL, NULL, CAST(0x0000A26601289E00 AS DateTime), CAST(0x0000A2660128B0F3 AS DateTime), N'SysAdmin', N'ES-W520', N'Explorer', NULL)
GO
INSERT [dbo].[ATS_Hierarchy] ([Id], [ParentId], [Sequence], [NodeType], [Name], [Description], [GroupId], [ProjectStep], [ProjectCode], [ProjectStatus], [CreationDate], [LastUpdateTime], [LastUpdateUser], [LastUpdateComputer], [LastUpdateApplication], [SCDUSASyncInd]) VALUES (101048, NULL, 10, N'G', N'Group #1', N'', NULL, NULL, NULL, NULL, CAST(0x0000A27100000000 AS DateTime), CAST(0x0000A27100000000 AS DateTime), N'Eli', N'ES', N'ES', NULL)
GO
INSERT [dbo].[ATS_Hierarchy] ([Id], [ParentId], [Sequence], [NodeType], [Name], [Description], [GroupId], [ProjectStep], [ProjectCode], [ProjectStatus], [CreationDate], [LastUpdateTime], [LastUpdateUser], [LastUpdateComputer], [LastUpdateApplication], [SCDUSASyncInd]) VALUES (101049, 100041, 10, N'P', N'Project #1', N'New Project', NULL, NULL, NULL, NULL, CAST(0x0000A27200BBE807 AS DateTime), CAST(0x0000A27200BC141E AS DateTime), N'SysAdmin', N'ES-W520', N'Explorer', NULL)
GO
INSERT [dbo].[ATS_Hierarchy] ([Id], [ParentId], [Sequence], [NodeType], [Name], [Description], [GroupId], [ProjectStep], [ProjectCode], [ProjectStatus], [CreationDate], [LastUpdateTime], [LastUpdateUser], [LastUpdateComputer], [LastUpdateApplication], [SCDUSASyncInd]) VALUES (101050, 100046, 10, N'F', N'QQ', N'QQ', NULL, NULL, NULL, NULL, CAST(0x0000A27200BC868A AS DateTime), CAST(0x0000A27200BCA349 AS DateTime), N'SysAdmin', N'ES-W520', N'Explorer', NULL)
GO
INSERT [dbo].[ATS_Hierarchy] ([Id], [ParentId], [Sequence], [NodeType], [Name], [Description], [GroupId], [ProjectStep], [ProjectCode], [ProjectStatus], [CreationDate], [LastUpdateTime], [LastUpdateUser], [LastUpdateComputer], [LastUpdateApplication], [SCDUSASyncInd]) VALUES (101051, 101050, 10, N'P', N'QQ Project', N'New Project', NULL, NULL, NULL, NULL, CAST(0x0000A27200BCA808 AS DateTime), CAST(0x0000A27200BCBE74 AS DateTime), N'SysAdmin', N'ES-W520', N'Explorer', NULL)
GO
INSERT [dbo].[ATS_Hierarchy] ([Id], [ParentId], [Sequence], [NodeType], [Name], [Description], [GroupId], [ProjectStep], [ProjectCode], [ProjectStatus], [CreationDate], [LastUpdateTime], [LastUpdateUser], [LastUpdateComputer], [LastUpdateApplication], [SCDUSASyncInd]) VALUES (101052, 101051, 10, N'F', N'QQ Project\Folder', N'QQ Project\Folder', NULL, NULL, NULL, NULL, CAST(0x0000A2850126C83A AS DateTime), CAST(0x0000A2850126F88E AS DateTime), N'SysAdmin', N'ES-W520', N'Explorer', NULL)
GO
INSERT [dbo].[ATS_Hierarchy] ([Id], [ParentId], [Sequence], [NodeType], [Name], [Description], [GroupId], [ProjectStep], [ProjectCode], [ProjectStatus], [CreationDate], [LastUpdateTime], [LastUpdateUser], [LastUpdateComputer], [LastUpdateApplication], [SCDUSASyncInd]) VALUES (101053, 101051, 10, N'P', N'QQ Sub-Project', N'QQ Sub-Project', NULL, NULL, NULL, N'O', CAST(0x0000A2850128408B AS DateTime), CAST(0x0000A2850128408B AS DateTime), N'SysAdmin', N'ES-W520', N'Explorer', 0)
GO
SET IDENTITY_INSERT [dbo].[ATS_Hierarchy] OFF
GO
INSERT [dbo].[ATS_Messages] ([Id], [Description], [Type], [LastUpdateTime], [LastUpdateUser], [LastUpdateComputer], [LastUpdateApplication]) VALUES (100, N'The name is already in use. Please specify another name.', N'E ', CAST(0x0000A26400BF4A85 AS DateTime), N'SysAdmin', N'ES-W520', N'Explorer')
GO
INSERT [dbo].[ATS_Messages] ([Id], [Description], [Type], [LastUpdateTime], [LastUpdateUser], [LastUpdateComputer], [LastUpdateApplication]) VALUES (101, N'Are you sure you want to remove the folder?', N'W ', CAST(0x0000A26400BF4A85 AS DateTime), N'SysAdmin', N'ES-W521', N'Explorer')
GO
INSERT [dbo].[ATS_Messages] ([Id], [Description], [Type], [LastUpdateTime], [LastUpdateUser], [LastUpdateComputer], [LastUpdateApplication]) VALUES (102, N'Are you sure you want to disable the note?', N'W ', CAST(0x0000A26400BF4A86 AS DateTime), N'SysAdmin', N'ES-W522', N'Explorer')
GO
INSERT [dbo].[ATS_Messages] ([Id], [Description], [Type], [LastUpdateTime], [LastUpdateUser], [LastUpdateComputer], [LastUpdateApplication]) VALUES (103, N'Are you sure you want to remove the certificate?', N'W ', CAST(0x0000A26400BF4A86 AS DateTime), N'SysAdmin', N'ES-W523', N'Explorer')
GO
INSERT [dbo].[ATS_Messages] ([Id], [Description], [Type], [LastUpdateTime], [LastUpdateUser], [LastUpdateComputer], [LastUpdateApplication]) VALUES (104, N'An object has been updated by another user. Please refresh and try again', N'E ', CAST(0x0000A26400BF4A86 AS DateTime), N'SysAdmin', N'ES-W524', N'Explorer')
GO
INSERT [dbo].[ATS_Messages] ([Id], [Description], [Type], [LastUpdateTime], [LastUpdateUser], [LastUpdateComputer], [LastUpdateApplication]) VALUES (105, N'Unexpected error  occurred. Please try again', N'E ', CAST(0x0000A26400BF4A87 AS DateTime), N'SysAdmin', N'ES-W525', N'Explorer')
GO
INSERT [dbo].[ATS_Messages] ([Id], [Description], [Type], [LastUpdateTime], [LastUpdateUser], [LastUpdateComputer], [LastUpdateApplication]) VALUES (106, N'User %name is not authorized to perform this action.', N'I ', CAST(0x0000A26400BF4A87 AS DateTime), N'SysAdmin', N'ES-W526', N'Explorer')
GO
INSERT [dbo].[ATS_Messages] ([Id], [Description], [Type], [LastUpdateTime], [LastUpdateUser], [LastUpdateComputer], [LastUpdateApplication]) VALUES (107, N'Current workstation is not certified for the project. The following certificates are missing: %list of missing certificates. Would youlike to proceed anyway?', N'I ', CAST(0x0000A26400BF4A87 AS DateTime), N'SysAdmin', N'ES-W527', N'Explorer')
GO
INSERT [dbo].[ATS_Messages] ([Id], [Description], [Type], [LastUpdateTime], [LastUpdateUser], [LastUpdateComputer], [LastUpdateApplication]) VALUES (108, N'Two or more Content versions of the same Content have been added. Please remove and try again', N'E ', CAST(0x0000A26400BF4A88 AS DateTime), N'SysAdmin', N'ES-W528', N'Explorer')
GO
INSERT [dbo].[ATS_Messages] ([Id], [Description], [Type], [LastUpdateTime], [LastUpdateUser], [LastUpdateComputer], [LastUpdateApplication]) VALUES (109, N'Two or more certificates with the same Certificate ID have been added. Please remove and try again. ', N'E ', CAST(0x0000A26400BF4A88 AS DateTime), N'SysAdmin', N'ES-W529', N'Explorer')
GO
INSERT [dbo].[ATS_Messages] ([Id], [Description], [Type], [LastUpdateTime], [LastUpdateUser], [LastUpdateComputer], [LastUpdateApplication]) VALUES (110, N'Project Code and Step combination is not unique. Please update and try again.', N'E ', CAST(0x0000A26400BF4A88 AS DateTime), N'SysAdmin', N'ES-W530', N'Explorer')
GO
INSERT [dbo].[ATS_Messages] ([Id], [Description], [Type], [LastUpdateTime], [LastUpdateUser], [LastUpdateComputer], [LastUpdateApplication]) VALUES (111, N'Project ''%name'' is related to the group. Would you like to update all projects belonging to the group?', N'W ', CAST(0x0000A26400BF4A88 AS DateTime), N'SysAdmin', N'ES-W531', N'Explorer')
GO
INSERT [dbo].[ATS_Messages] ([Id], [Description], [Type], [LastUpdateTime], [LastUpdateUser], [LastUpdateComputer], [LastUpdateApplication]) VALUES (112, N'Version %[active versio name] will be closed. Are you sure you want to reopen version %name?', N'W ', CAST(0x0000A26400BF4A89 AS DateTime), N'SysAdmin', N'ES-W532', N'Explorer')
GO
INSERT [dbo].[ATS_Messages] ([Id], [Description], [Type], [LastUpdateTime], [LastUpdateUser], [LastUpdateComputer], [LastUpdateApplication]) VALUES (114, N'Folder ''%name'' has been created successfully', N'I ', CAST(0x0000A26400BF4A89 AS DateTime), N'SysAdmin', N'ES-W533', N'Explorer')
GO
INSERT [dbo].[ATS_Messages] ([Id], [Description], [Type], [LastUpdateTime], [LastUpdateUser], [LastUpdateComputer], [LastUpdateApplication]) VALUES (115, N'Project ''%name'' has been created successfully', N'I ', CAST(0x0000A26400BF4A89 AS DateTime), N'SysAdmin', N'ES-W534', N'Explorer')
GO
INSERT [dbo].[ATS_Messages] ([Id], [Description], [Type], [LastUpdateTime], [LastUpdateUser], [LastUpdateComputer], [LastUpdateApplication]) VALUES (116, N'New related project ''%name'' has been successfully created and assigned to the group', N'I ', CAST(0x0000A26400BF4A8A AS DateTime), N'SysAdmin', N'ES-W535', N'Explorer')
GO
INSERT [dbo].[ATS_Messages] ([Id], [Description], [Type], [LastUpdateTime], [LastUpdateUser], [LastUpdateComputer], [LastUpdateApplication]) VALUES (117, N'Related projects group has been successfully updated.', N'I ', CAST(0x0000A26400BF4A8A AS DateTime), N'SysAdmin', N'ES-W536', N'Explorer')
GO
INSERT [dbo].[ATS_Messages] ([Id], [Description], [Type], [LastUpdateTime], [LastUpdateUser], [LastUpdateComputer], [LastUpdateApplication]) VALUES (118, N'Are you sure you want to create a new version?', N'W ', CAST(0x0000A26400BF4A8A AS DateTime), N'SysAdmin', N'ES-W537', N'Explorer')
GO
INSERT [dbo].[ATS_Messages] ([Id], [Description], [Type], [LastUpdateTime], [LastUpdateUser], [LastUpdateComputer], [LastUpdateApplication]) VALUES (119, N'Content %name1 is referenced by content %name2. Please remove and try again.', N'E ', CAST(0x0000A26400BF4A8B AS DateTime), N'SysAdmin', N'ES-W538', N'Explorer')
GO
INSERT [dbo].[ATS_Messages] ([Id], [Description], [Type], [LastUpdateTime], [LastUpdateUser], [LastUpdateComputer], [LastUpdateApplication]) VALUES (120, N'Content %name is retired. Do you want to assign retired content to the project', N'W ', CAST(0x0000A26400BF4A8B AS DateTime), N'SysAdmin', N'ES-W539', N'Explorer')
GO
INSERT [dbo].[ATS_Messages] ([Id], [Description], [Type], [LastUpdateTime], [LastUpdateUser], [LastUpdateComputer], [LastUpdateApplication]) VALUES (121, N'There is not enough disk space. Please free disk space and try again.', N'E ', CAST(0x0000A26400BF4A8B AS DateTime), N'SysAdmin', N'ES-W540', N'Explorer')
GO
INSERT [dbo].[ATS_Messages] ([Id], [Description], [Type], [LastUpdateTime], [LastUpdateUser], [LastUpdateComputer], [LastUpdateApplication]) VALUES (122, N'Content %name1 and content %name2 are referenced by content %name3. Please remove %name1 or %name2 and try again.', N'E ', CAST(0x0000A26400BF4A8B AS DateTime), N'SysAdmin', N'ES-W541', N'Explorer')
GO
INSERT [dbo].[ATS_Messages] ([Id], [Description], [Type], [LastUpdateTime], [LastUpdateUser], [LastUpdateComputer], [LastUpdateApplication]) VALUES (123, N'Related projects Group Name is already in use. Please specify another name', N'E ', CAST(0x0000A26400BF4A8C AS DateTime), N'SysAdmin', N'ES-W542', N'Explorer')
GO
INSERT [dbo].[ATS_Messages] ([Id], [Description], [Type], [LastUpdateTime], [LastUpdateUser], [LastUpdateComputer], [LastUpdateApplication]) VALUES (124, N'Version Name is already in use. Please specify another name.', N'E ', CAST(0x0000A26400BF4A8C AS DateTime), N'SysAdmin', N'ES-W543', N'Explorer')
GO
INSERT [dbo].[ATS_MessageType] ([Type], [Description], [LastUpdateTime], [LastUpdateUser], [LastUpdateComputer], [LastUpdateApplication]) VALUES (N'E ', N'Error', CAST(0x0000A28800D91432 AS DateTime), N'SysAdmin', N'MyCmp', N'Explorer')
GO
INSERT [dbo].[ATS_MessageType] ([Type], [Description], [LastUpdateTime], [LastUpdateUser], [LastUpdateComputer], [LastUpdateApplication]) VALUES (N'I ', N'Info', CAST(0x0000A28800D9A05E AS DateTime), N'SysAdmin', N'MyCmp', N'Explorer')
GO
INSERT [dbo].[ATS_MessageType] ([Type], [Description], [LastUpdateTime], [LastUpdateUser], [LastUpdateComputer], [LastUpdateApplication]) VALUES (N'W ', N'Warning', CAST(0x0000A28800D989F8 AS DateTime), N'SysAdmin', N'MyCmp', N'Explorer')
GO
SET IDENTITY_INSERT [dbo].[ATS_Note] ON 

GO
INSERT [dbo].[ATS_Note] ([Id], [HierarchyId], [NoteType], [NoteStatus], [NoteTitle], [NoteText], [SpecialInd], [CreatedByUser], [CreationDate], [LastUpdateTime], [LastUpdateUser], [LastUpdateComputer], [LastUpdateapplication]) VALUES (100000, 100038, N'W ', N'A', N'Test Note', N'sdfgsdfgdsgsdfgsdgdsgsdfgsdfgdsfgdsgsdfgsdfgsdgsdgsdgsdfgsdfgsdfgsdfgsdfgsdfgsdfgsdfg', 0, N'SysAdmin', CAST(0x0000A295009A9342 AS DateTime), CAST(0x0000A295009D0C42 AS DateTime), N'SysAdmin', N'ES-W520', N'Explorer')
GO
INSERT [dbo].[ATS_Note] ([Id], [HierarchyId], [NoteType], [NoteStatus], [NoteTitle], [NoteText], [SpecialInd], [CreatedByUser], [CreationDate], [LastUpdateTime], [LastUpdateUser], [LastUpdateComputer], [LastUpdateapplication]) VALUES (100001, 100038, N'W ', N'A', N'Another Test', N'3456345643564356436345634563463464356345645', 1, N'SysAdmin', CAST(0x0000A295009B27DE AS DateTime), CAST(0x0000A295009B27DE AS DateTime), N'SysAdmin', N'ES-W520', N'Explorer')
GO
INSERT [dbo].[ATS_Note] ([Id], [HierarchyId], [NoteType], [NoteStatus], [NoteTitle], [NoteText], [SpecialInd], [CreatedByUser], [CreationDate], [LastUpdateTime], [LastUpdateUser], [LastUpdateComputer], [LastUpdateapplication]) VALUES (100002, 100038, N'C ', N'A', N'dfhdfghdfgh', N'dfghdfhfdhfdhf', 0, N'SysAdmin', CAST(0x0000A29500D2CC2F AS DateTime), CAST(0x0000A29500D2CC2F AS DateTime), N'SysAdmin', N'ES-W520', N'Explorer')
GO
SET IDENTITY_INSERT [dbo].[ATS_Note] OFF
GO
INSERT [dbo].[ATS_NoteType] ([NoteType], [Description], [LastUpdateTime], [LastUpdateUser], [LastUpdateComputer], [LastUpdateapplication]) VALUES (N'C ', N'Comment', CAST(0x0000A28800E55DC1 AS DateTime), N'SysAdmin', N'MyComp', N'Explorer')
GO
INSERT [dbo].[ATS_NoteType] ([NoteType], [Description], [LastUpdateTime], [LastUpdateUser], [LastUpdateComputer], [LastUpdateapplication]) VALUES (N'W ', N'Work Instructions', CAST(0x0000A28800E569B9 AS DateTime), N'SysAdmin', N'MyComp', N'Explorer')
GO
INSERT [dbo].[ATS_Privilege] ([PrivilegeCode], [PrivilegeDescription], [LastUpdateTime], [LastUpdateUser], [LastUpdateComputer], [LastUpdateApplication], [SubSystem]) VALUES (N'101', N'AddNew', CAST(0x0000A26800000000 AS DateTime), N'Eli', N'Eli', N'Manual', N'Explorer')
GO
INSERT [dbo].[ATS_Privilege] ([PrivilegeCode], [PrivilegeDescription], [LastUpdateTime], [LastUpdateUser], [LastUpdateComputer], [LastUpdateApplication], [SubSystem]) VALUES (N'102', N'MoveHierarchyBranch', CAST(0x0000A26800000000 AS DateTime), N'Eli', N'Eli', N'Manual', N'Explorer')
GO
INSERT [dbo].[ATS_Privilege] ([PrivilegeCode], [PrivilegeDescription], [LastUpdateTime], [LastUpdateUser], [LastUpdateComputer], [LastUpdateApplication], [SubSystem]) VALUES (N'103', N'DeleteFolder', CAST(0x0000A26800000000 AS DateTime), N'Eli', N'Eli', N'Manual', N'Explorer')
GO
INSERT [dbo].[ATS_Privilege] ([PrivilegeCode], [PrivilegeDescription], [LastUpdateTime], [LastUpdateUser], [LastUpdateComputer], [LastUpdateApplication], [SubSystem]) VALUES (N'104', N'ViewDisabledNote', CAST(0x0000A26800000000 AS DateTime), N'Eli', N'Eli', N'Manual', N'Explorer')
GO
INSERT [dbo].[ATS_Privilege] ([PrivilegeCode], [PrivilegeDescription], [LastUpdateTime], [LastUpdateUser], [LastUpdateComputer], [LastUpdateApplication], [SubSystem]) VALUES (N'105', N'DisableNote', CAST(0x0000A26800000000 AS DateTime), N'Eli', N'Eli', N'Manual', N'Explorer')
GO
INSERT [dbo].[ATS_Privilege] ([PrivilegeCode], [PrivilegeDescription], [LastUpdateTime], [LastUpdateUser], [LastUpdateComputer], [LastUpdateApplication], [SubSystem]) VALUES (N'106', N'UpdateVersion', CAST(0x0000A26800000000 AS DateTime), N'Eli', N'Eli', N'Manual', N'Explorer')
GO
INSERT [dbo].[ATS_Privilege] ([PrivilegeCode], [PrivilegeDescription], [LastUpdateTime], [LastUpdateUser], [LastUpdateComputer], [LastUpdateApplication], [SubSystem]) VALUES (N'107', N'AssignRetiredContent', CAST(0x0000A26800000000 AS DateTime), N'Eli', N'Eli', N'Manual', N'Explorer')
GO
INSERT [dbo].[ATS_Privilege] ([PrivilegeCode], [PrivilegeDescription], [LastUpdateTime], [LastUpdateUser], [LastUpdateComputer], [LastUpdateApplication], [SubSystem]) VALUES (N'108', N'AssignCertificate', CAST(0x0000A26800000000 AS DateTime), N'Eli', N'Eli', N'Manual', N'Explorer')
GO
INSERT [dbo].[ATS_Privilege] ([PrivilegeCode], [PrivilegeDescription], [LastUpdateTime], [LastUpdateUser], [LastUpdateComputer], [LastUpdateApplication], [SubSystem]) VALUES (N'109', N'RemoveCertificate', CAST(0x0000A26800000000 AS DateTime), N'Eli', N'Eli', N'Manual', N'Explorer')
GO
INSERT [dbo].[ATS_Privilege] ([PrivilegeCode], [PrivilegeDescription], [LastUpdateTime], [LastUpdateUser], [LastUpdateComputer], [LastUpdateApplication], [SubSystem]) VALUES (N'110', N'AddNote', CAST(0x0000A26800000000 AS DateTime), N'Eli', N'Eli', N'Manual', N'Explorer')
GO
INSERT [dbo].[ATS_Privilege] ([PrivilegeCode], [PrivilegeDescription], [LastUpdateTime], [LastUpdateUser], [LastUpdateComputer], [LastUpdateApplication], [SubSystem]) VALUES (N'111', N'EditNote', CAST(0x0000A26800000000 AS DateTime), N'Eli', N'Eli', N'Manual', N'Explorer')
GO
INSERT [dbo].[ATS_Privilege] ([PrivilegeCode], [PrivilegeDescription], [LastUpdateTime], [LastUpdateUser], [LastUpdateComputer], [LastUpdateApplication], [SubSystem]) VALUES (N'112', N'ExecuteContent', CAST(0x0000A26800000000 AS DateTime), N'Eli', N'Eli', N'Manual', N'Explorer')
GO
INSERT [dbo].[ATS_Privilege] ([PrivilegeCode], [PrivilegeDescription], [LastUpdateTime], [LastUpdateUser], [LastUpdateComputer], [LastUpdateApplication], [SubSystem]) VALUES (N'113', N'DisableProject', CAST(0x0000A26800000000 AS DateTime), N'Eli', N'Eli', N'Manual', N'Explorer')
GO
INSERT [dbo].[ATS_Privilege] ([PrivilegeCode], [PrivilegeDescription], [LastUpdateTime], [LastUpdateUser], [LastUpdateComputer], [LastUpdateApplication], [SubSystem]) VALUES (N'114', N'UpdateDisableProject', CAST(0x0000A26800000000 AS DateTime), N'Eli', N'Eli', N'Manual', N'Explorer')
GO
INSERT [dbo].[ATS_Privilege] ([PrivilegeCode], [PrivilegeDescription], [LastUpdateTime], [LastUpdateUser], [LastUpdateComputer], [LastUpdateApplication], [SubSystem]) VALUES (N'115', N'ResumeProject', CAST(0x0000A26800000000 AS DateTime), N'Eli', N'Eli', N'Manual', N'Explorer')
GO
INSERT [dbo].[ATS_Privilege] ([PrivilegeCode], [PrivilegeDescription], [LastUpdateTime], [LastUpdateUser], [LastUpdateComputer], [LastUpdateApplication], [SubSystem]) VALUES (N'116', N'ExecuteContentClosedVersion', CAST(0x0000A26800000000 AS DateTime), N'Eli', N'Eli', N'Manual', N'Explorer')
GO
INSERT [dbo].[ATS_Privilege] ([PrivilegeCode], [PrivilegeDescription], [LastUpdateTime], [LastUpdateUser], [LastUpdateComputer], [LastUpdateApplication], [SubSystem]) VALUES (N'117', N'ActivateContentDisabledProject', CAST(0x0000A26800000000 AS DateTime), N'Eli', N'Eli', N'Manual', N'Explorer')
GO
INSERT [dbo].[ATS_Privilege] ([PrivilegeCode], [PrivilegeDescription], [LastUpdateTime], [LastUpdateUser], [LastUpdateComputer], [LastUpdateApplication], [SubSystem]) VALUES (N'118', N'ReopenClosedVersion', CAST(0x0000A26800000000 AS DateTime), N'Eli', N'Eli', N'Manual', N'Explorer')
GO
INSERT [dbo].[ATS_Privilege] ([PrivilegeCode], [PrivilegeDescription], [LastUpdateTime], [LastUpdateUser], [LastUpdateComputer], [LastUpdateApplication], [SubSystem]) VALUES (N'119', N'CloneProject', CAST(0x0000A26800000000 AS DateTime), N'Eli', N'Eli', N'Manual', N'Explorer')
GO
INSERT [dbo].[ATS_Privilege] ([PrivilegeCode], [PrivilegeDescription], [LastUpdateTime], [LastUpdateUser], [LastUpdateComputer], [LastUpdateApplication], [SubSystem]) VALUES (N'120', N'CloneRelatedProject', CAST(0x0000A26800000000 AS DateTime), N'Eli', N'Eli', N'Manual', N'Explorer')
GO
INSERT [dbo].[ATS_Privilege] ([PrivilegeCode], [PrivilegeDescription], [LastUpdateTime], [LastUpdateUser], [LastUpdateComputer], [LastUpdateApplication], [SubSystem]) VALUES (N'121', N'EditProjectCode', CAST(0x0000A26800000000 AS DateTime), N'Eli', N'Eli', N'Manual', N'Explorer')
GO
INSERT [dbo].[ATS_Privilege] ([PrivilegeCode], [PrivilegeDescription], [LastUpdateTime], [LastUpdateUser], [LastUpdateComputer], [LastUpdateApplication], [SubSystem]) VALUES (N'122', N'EditProjectStep', CAST(0x0000A26800000000 AS DateTime), N'Eli', N'Eli', N'Manual', N'Explorer')
GO
INSERT [dbo].[ATS_Privilege] ([PrivilegeCode], [PrivilegeDescription], [LastUpdateTime], [LastUpdateUser], [LastUpdateComputer], [LastUpdateApplication], [SubSystem]) VALUES (N'123', N'EditProjectProperties', CAST(0x0000A26800000000 AS DateTime), N'Eli', N'Eli', N'Manual', N'Explorer')
GO
INSERT [dbo].[ATS_Privilege] ([PrivilegeCode], [PrivilegeDescription], [LastUpdateTime], [LastUpdateUser], [LastUpdateComputer], [LastUpdateApplication], [SubSystem]) VALUES (N'124', N'UpdateRelatedProject', CAST(0x0000A26800000000 AS DateTime), N'Eli', N'Eli', N'Manual', N'Explorer')
GO
INSERT [dbo].[ATS_Privilege] ([PrivilegeCode], [PrivilegeDescription], [LastUpdateTime], [LastUpdateUser], [LastUpdateComputer], [LastUpdateApplication], [SubSystem]) VALUES (N'125', N'SplitProjectFromGroup', CAST(0x0000A26800000000 AS DateTime), N'Eli', N'Eli', N'Manual', N'Explorer')
GO
INSERT [dbo].[ATS_Privilege] ([PrivilegeCode], [PrivilegeDescription], [LastUpdateTime], [LastUpdateUser], [LastUpdateComputer], [LastUpdateApplication], [SubSystem]) VALUES (N'126', N'ActivateProjectFromUncertifiedWorkstation', CAST(0x0000A26800000000 AS DateTime), N'Eli', N'Eli', N'Manual', N'Explorer')
GO
INSERT [dbo].[ATS_Privilege] ([PrivilegeCode], [PrivilegeDescription], [LastUpdateTime], [LastUpdateUser], [LastUpdateComputer], [LastUpdateApplication], [SubSystem]) VALUES (N'127', N'UpdateTargetDirectory', CAST(0x0000A26800000000 AS DateTime), N'Eli', N'Eli', N'Manual', N'Explorer')
GO
INSERT [dbo].[ATS_Privilege] ([PrivilegeCode], [PrivilegeDescription], [LastUpdateTime], [LastUpdateUser], [LastUpdateComputer], [LastUpdateApplication], [SubSystem]) VALUES (N'999', N'AllActivities', CAST(0x0000A26800000000 AS DateTime), N'Eli', N'Eli', N'Manual', N'Explorer')
GO
INSERT [dbo].[ATS_Profile] ([RowId], [ProfileDescription], [LastUpdateTime], [LastUpdateUser], [LastUpdateComputer], [LastUpdateApplication]) VALUES (1, N'System Administrators', CAST(0x0000A26800000000 AS DateTime), N'Eli', N'ES', N'Manual')
GO
INSERT [dbo].[ATS_Profile] ([RowId], [ProfileDescription], [LastUpdateTime], [LastUpdateUser], [LastUpdateComputer], [LastUpdateApplication]) VALUES (2, N'Default', CAST(0x0000A26800000000 AS DateTime), N'Eli', N'ES', N'Manual')
GO
INSERT [dbo].[ATS_ProfilePrivilege] ([RowId], [ProfileId], [PrivilegeCode], [LastUpdateTime], [LastUpdateUser], [LastUpdateComputer], [LastUpdateApplicationr]) VALUES (1, 1, N'999', CAST(0x0000A26800000000 AS DateTime), N'Eli', N'ES', N'Manual')
GO
INSERT [dbo].[ATS_ProfilePrivilege] ([RowId], [ProfileId], [PrivilegeCode], [LastUpdateTime], [LastUpdateUser], [LastUpdateComputer], [LastUpdateApplicationr]) VALUES (2, 2, N'101', CAST(0x0000A26800000000 AS DateTime), N'Eli', N'ES', N'Manual')
GO
INSERT [dbo].[ATS_ProfilePrivilege] ([RowId], [ProfileId], [PrivilegeCode], [LastUpdateTime], [LastUpdateUser], [LastUpdateComputer], [LastUpdateApplicationr]) VALUES (3, 2, N'102', CAST(0x0000A26800000000 AS DateTime), N'Eli', N'ES', N'Manual')
GO
INSERT [dbo].[ATS_ProjectStep] ([StepCode], [StepDescription], [LastUpdateTime], [LastUpdateUser], [LastUpdateComputer], [LastUpdateapplication]) VALUES (N'Kadel               ', N'Kadel', CAST(0x0000A27C00DDA185 AS DateTime), N'Sys', N'Comp', N'PPP')
GO
INSERT [dbo].[ATS_ProjectStep] ([StepCode], [StepDescription], [LastUpdateTime], [LastUpdateUser], [LastUpdateComputer], [LastUpdateapplication]) VALUES (N'Step1               ', N'Step 1', CAST(0x0000A27C00DDC4E4 AS DateTime), N'Sys', N'Comp', N'PPP')
GO
INSERT [dbo].[ATS_ProjectStep] ([StepCode], [StepDescription], [LastUpdateTime], [LastUpdateUser], [LastUpdateComputer], [LastUpdateapplication]) VALUES (N'Step1a              ', N'Step 1a', CAST(0x0000A27400B9A2A6 AS DateTime), N'Admin', N'MyComp', N'Explorer')
GO
INSERT [dbo].[ATS_ProjectStep] ([StepCode], [StepDescription], [LastUpdateTime], [LastUpdateUser], [LastUpdateComputer], [LastUpdateapplication]) VALUES (N'Step2               ', N'Step 2', CAST(0x0000A27400B9DDF1 AS DateTime), N'Admin', N'MyComp', N'Explorer')
GO
INSERT [dbo].[ATS_SchemaVersion] ([VersionNo], [Date]) VALUES (4, CAST(0x0000A27800A59F7C AS DateTime))
GO
INSERT [dbo].[ATS_SchemaVersion] ([VersionNo], [Date]) VALUES (3, CAST(0x0000A27800CD83A3 AS DateTime))
GO
INSERT [dbo].[ATS_SchemaVersion] ([VersionNo], [Date]) VALUES (5, CAST(0x0000A27900C64DC7 AS DateTime))
GO
INSERT [dbo].[ATS_SchemaVersion] ([VersionNo], [Date]) VALUES (6, CAST(0x0000A27E01230A5E AS DateTime))
GO
INSERT [dbo].[ATS_SchemaVersion] ([VersionNo], [Date]) VALUES (7, CAST(0x0000A2830143FFDE AS DateTime))
GO
INSERT [dbo].[ATS_SchemaVersion] ([VersionNo], [Date]) VALUES (8, CAST(0x0000A28801454D19 AS DateTime))
GO
INSERT [dbo].[ATS_SchemaVersion] ([VersionNo], [Date]) VALUES (9, CAST(0x0000A28A01200F90 AS DateTime))
GO
INSERT [dbo].[ATS_SystemParameters] ([Variable], [Description], [Type], [Value], [LastUpdateTime], [LastUpdateUser], [LastUpdateComputer], [LastUpdateapplication]) VALUES (N'CMConnectionString', N'Content Management DB Connection String', N'string', N'Data Source=(local)\SQLExpress;Initial Catalog=GenPR_Test;Integrated Security=true;', CAST(0x0000A27800A49C1C AS DateTime), N'Admin', N'MyComp', N'Explorer')
GO
INSERT [dbo].[ATS_SystemParameters] ([Variable], [Description], [Type], [Value], [LastUpdateTime], [LastUpdateUser], [LastUpdateComputer], [LastUpdateapplication]) VALUES (N'ProjectLocalPath', N'Regular Projects Files Local Path', N'String', N'C:\TestSCD', CAST(0x0000A28800FB6C20 AS DateTime), N'SysAdmin', N'MyComp', N'Explorer')
GO
INSERT [dbo].[ATS_SystemParameters] ([Variable], [Description], [Type], [Value], [LastUpdateTime], [LastUpdateUser], [LastUpdateComputer], [LastUpdateapplication]) VALUES (N'RelatedProjectLocalPath', N'Related Projects Files Local Path', N'String', N'C:\TestSCD', CAST(0x0000A28800FB495B AS DateTime), N'SysAdmin', N'MyComp', N'Explorer')
GO
INSERT [dbo].[ATS_SystemParameters] ([Variable], [Description], [Type], [Value], [LastUpdateTime], [LastUpdateUser], [LastUpdateComputer], [LastUpdateapplication]) VALUES (N'RequiredDiskSpace', N'Disk space required for Content files', N'String', N'100', CAST(0x0000A28800FBE336 AS DateTime), N'SysAdmin', N'MyComp', N'Explorer')
GO
INSERT [dbo].[ATS_SystemParameters] ([Variable], [Description], [Type], [Value], [LastUpdateTime], [LastUpdateUser], [LastUpdateComputer], [LastUpdateapplication]) VALUES (N'RMConnectionString', N'Resource Management DB Connection String', N'string', N'Data Source=(local)\SQLExpress;Initial Catalog=GenPR_Test;Integrated Security=true;', CAST(0x0000A27800A49C1F AS DateTime), N'Admin', N'MyComp', N'Explorer')
GO
INSERT [dbo].[ATS_UserProfile] ([RowId], [UserId], [ProfileId], [EffectiveDate], [ExpirationDate], [LastUpdateTime], [LastUpdateUser], [LastUpdateComputer], [LastUpdateApplicationr]) VALUES (1, N'SysAdmin', 1, N'2013-01-01', NULL, CAST(0x0000A26800000000 AS DateTime), N'Eli', N'ES', N'Manual')
GO
INSERT [dbo].[ATS_UserProfile] ([RowId], [UserId], [ProfileId], [EffectiveDate], [ExpirationDate], [LastUpdateTime], [LastUpdateUser], [LastUpdateComputer], [LastUpdateApplicationr]) VALUES (2, N'EliSt', 2, N'2013-01-01', NULL, CAST(0x0000A26800000000 AS DateTime), N'Eli', N'ES', N'Manual')
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [IXUQ_HierarchyGroupName]    Script Date: 12/19/2013 6:21:40 PM ******/
CREATE UNIQUE NONCLUSTERED INDEX [IXUQ_HierarchyGroupName] ON [dbo].[ATS_Hierarchy]
(
	[Name] ASC
)
WHERE ([NodeType]='G')
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [IXUQ_HierarchyParentIdName]    Script Date: 12/19/2013 6:21:40 PM ******/
CREATE UNIQUE NONCLUSTERED INDEX [IXUQ_HierarchyParentIdName] ON [dbo].[ATS_Hierarchy]
(
	[ParentId] ASC,
	[Name] ASC
)
WHERE ([NodeType] IN ('F', 'P'))
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [UQ_VersionName]    Script Date: 12/19/2013 6:21:40 PM ******/
ALTER TABLE [dbo].[ATS_Version] ADD  CONSTRAINT [UQ_VersionName] UNIQUE NONCLUSTERED 
(
	[HierarchyId] ASC,
	[VersionName] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
ALTER TABLE [dbo].[ATS_FolderCertificate]  WITH CHECK ADD  CONSTRAINT [FK_FolderCertificate_Hierarchy] FOREIGN KEY([HierarchyId])
REFERENCES [dbo].[ATS_Hierarchy] ([Id])
GO
ALTER TABLE [dbo].[ATS_FolderCertificate] CHECK CONSTRAINT [FK_FolderCertificate_Hierarchy]
GO
ALTER TABLE [dbo].[ATS_Hierarchy]  WITH CHECK ADD  CONSTRAINT [FK_Hierarchy_Hierarchy] FOREIGN KEY([ParentId])
REFERENCES [dbo].[ATS_Hierarchy] ([Id])
GO
ALTER TABLE [dbo].[ATS_Hierarchy] CHECK CONSTRAINT [FK_Hierarchy_Hierarchy]
GO
ALTER TABLE [dbo].[ATS_Hierarchy]  WITH CHECK ADD  CONSTRAINT [FK_Hierarchy_Hierarchy1] FOREIGN KEY([GroupId])
REFERENCES [dbo].[ATS_Hierarchy] ([Id])
GO
ALTER TABLE [dbo].[ATS_Hierarchy] CHECK CONSTRAINT [FK_Hierarchy_Hierarchy1]
GO
ALTER TABLE [dbo].[ATS_Messages]  WITH CHECK ADD  CONSTRAINT [FK_Messages_MessageType] FOREIGN KEY([Type])
REFERENCES [dbo].[ATS_MessageType] ([Type])
GO
ALTER TABLE [dbo].[ATS_Messages] CHECK CONSTRAINT [FK_Messages_MessageType]
GO
ALTER TABLE [dbo].[ATS_Note]  WITH CHECK ADD  CONSTRAINT [FK_FolderNote_NoteType] FOREIGN KEY([HierarchyId])
REFERENCES [dbo].[ATS_Hierarchy] ([Id])
GO
ALTER TABLE [dbo].[ATS_Note] CHECK CONSTRAINT [FK_FolderNote_NoteType]
GO
ALTER TABLE [dbo].[ATS_Note]  WITH CHECK ADD  CONSTRAINT [FK_Note_NoteType] FOREIGN KEY([NoteType])
REFERENCES [dbo].[ATS_NoteType] ([NoteType])
GO
ALTER TABLE [dbo].[ATS_Note] CHECK CONSTRAINT [FK_Note_NoteType]
GO
ALTER TABLE [dbo].[ATS_ProfilePrivilege]  WITH CHECK ADD  CONSTRAINT [FK_ProfilePrivilege_Privilege] FOREIGN KEY([PrivilegeCode])
REFERENCES [dbo].[ATS_Privilege] ([PrivilegeCode])
GO
ALTER TABLE [dbo].[ATS_ProfilePrivilege] CHECK CONSTRAINT [FK_ProfilePrivilege_Privilege]
GO
ALTER TABLE [dbo].[ATS_ProfilePrivilege]  WITH CHECK ADD  CONSTRAINT [FK_ProfilePrivilege_Profile] FOREIGN KEY([ProfileId])
REFERENCES [dbo].[ATS_Profile] ([RowId])
GO
ALTER TABLE [dbo].[ATS_ProfilePrivilege] CHECK CONSTRAINT [FK_ProfilePrivilege_Profile]
GO
ALTER TABLE [dbo].[ATS_UserProfile]  WITH CHECK ADD  CONSTRAINT [FK_UserProfile_Profile] FOREIGN KEY([ProfileId])
REFERENCES [dbo].[ATS_Profile] ([RowId])
GO
ALTER TABLE [dbo].[ATS_UserProfile] CHECK CONSTRAINT [FK_UserProfile_Profile]
GO
ALTER TABLE [dbo].[ATS_Version]  WITH CHECK ADD  CONSTRAINT [FK_GroupVersion_Group] FOREIGN KEY([HierarchyId])
REFERENCES [dbo].[ATS_Hierarchy] ([Id])
GO
ALTER TABLE [dbo].[ATS_Version] CHECK CONSTRAINT [FK_GroupVersion_Group]
GO
ALTER TABLE [dbo].[ATS_VersionContent]  WITH CHECK ADD  CONSTRAINT [FK_VersionContent_GroupVersion] FOREIGN KEY([VersionId])
REFERENCES [dbo].[ATS_Version] ([VersionId])
GO
ALTER TABLE [dbo].[ATS_VersionContent] CHECK CONSTRAINT [FK_VersionContent_GroupVersion]
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'PK, unique row Id' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ATS_FolderCertificate', @level2type=N'COLUMN',@level2name=N'Rowid'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Hierarchy node id of the folder' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ATS_FolderCertificate', @level2type=N'COLUMN',@level2name=N'HierarchyId'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Certificate Id (received from RM system)' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ATS_FolderCertificate', @level2type=N'COLUMN',@level2name=N'CertificateId'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Certificate effective date' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ATS_FolderCertificate', @level2type=N'COLUMN',@level2name=N'EffectiveDate'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Certificate expiration date' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ATS_FolderCertificate', @level2type=N'COLUMN',@level2name=N'ExpirationDate'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Control fields' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ATS_FolderCertificate', @level2type=N'COLUMN',@level2name=N'LastUpdateTime'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Node unique id' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ATS_Hierarchy', @level2type=N'COLUMN',@level2name=N'Id'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Node parent Id, FK, references Id' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ATS_Hierarchy', @level2type=N'COLUMN',@level2name=N'ParentId'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Node sequence number within hierarchy branch' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ATS_Hierarchy', @level2type=N'COLUMN',@level2name=N'Sequence'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Tree node type: Folder (F), Project(P), Group (G)' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ATS_Hierarchy', @level2type=N'COLUMN',@level2name=N'NodeType'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Node name to be displayed' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ATS_Hierarchy', @level2type=N'COLUMN',@level2name=N'Name'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Folder/Project/Group description. A text box on properties tab of the folder, populated by user, optional' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ATS_Hierarchy', @level2type=N'COLUMN',@level2name=N'Description'
GO
EXEC sys.sp_addextendedproperty @name=N'Description', @value=N'The value is set to ''Y'' if node is a related project' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ATS_Hierarchy', @level2type=N'COLUMN',@level2name=N'GroupId'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Multi MAKAT GroupId, FK, references Id column. Will be null for not Related projects' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ATS_Hierarchy', @level2type=N'COLUMN',@level2name=N'GroupId'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Project Step. Will be null for Related projects. Project Step will be populated for referenced Group record.' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ATS_Hierarchy', @level2type=N'COLUMN',@level2name=N'ProjectStep'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Project Code.' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ATS_Hierarchy', @level2type=N'COLUMN',@level2name=N'ProjectCode'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Project status. O(open), D (disabled)' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ATS_Hierarchy', @level2type=N'COLUMN',@level2name=N'ProjectStatus'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Node creation date' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ATS_Hierarchy', @level2type=N'COLUMN',@level2name=N'CreationDate'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Control Field' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ATS_Hierarchy', @level2type=N'COLUMN',@level2name=N'LastUpdateTime'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Control Field' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ATS_Hierarchy', @level2type=N'COLUMN',@level2name=N'LastUpdateUser'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Control Field' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ATS_Hierarchy', @level2type=N'COLUMN',@level2name=N'LastUpdateComputer'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Control Field' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ATS_Hierarchy', @level2type=N'COLUMN',@level2name=N'LastUpdateApplication'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Message Id. Message test is retrived by Message Id' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ATS_Messages', @level2type=N'COLUMN',@level2name=N'Id'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Message text' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ATS_Messages', @level2type=N'COLUMN',@level2name=N'Description'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'E, W, I' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ATS_MessageType', @level2type=N'COLUMN',@level2name=N'Type'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'E-Error, W-Warning, I-Info' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ATS_MessageType', @level2type=N'COLUMN',@level2name=N'Description'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Note id' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ATS_Note', @level2type=N'COLUMN',@level2name=N'Id'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'FK, references Hierarchy.Id column  (Group record in case of Related project)' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ATS_Note', @level2type=N'COLUMN',@level2name=N'HierarchyId'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Note type - Work Instruction(W), Comment(C)' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ATS_Note', @level2type=N'COLUMN',@level2name=N'NoteType'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Note status - Disabled (D), Active (A)' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ATS_Note', @level2type=N'COLUMN',@level2name=N'NoteStatus'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Note title, mandatory when creating new note' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ATS_Note', @level2type=N'COLUMN',@level2name=N'NoteTitle'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Note text' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ATS_Note', @level2type=N'COLUMN',@level2name=N'NoteText'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Indicates whether the note is special (1) or not (0)' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ATS_Note', @level2type=N'COLUMN',@level2name=N'SpecialInd'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'User that created the note' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ATS_Note', @level2type=N'COLUMN',@level2name=N'CreatedByUser'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Note creation date' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ATS_Note', @level2type=N'COLUMN',@level2name=N'CreationDate'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Control fields' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ATS_Note', @level2type=N'COLUMN',@level2name=N'LastUpdateTime'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Note type - W or C' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ATS_NoteType', @level2type=N'COLUMN',@level2name=N'NoteType'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Type description. Work Instruction (W) or Comment (C)' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ATS_NoteType', @level2type=N'COLUMN',@level2name=N'Description'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Control fields' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ATS_NoteType', @level2type=N'COLUMN',@level2name=N'LastUpdateTime'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Step code (KADEL, STEP1, etc.), populated by SCD' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ATS_ProjectStep', @level2type=N'COLUMN',@level2name=N'StepCode'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Step description' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ATS_ProjectStep', @level2type=N'COLUMN',@level2name=N'StepDescription'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Control fields' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ATS_ProjectStep', @level2type=N'COLUMN',@level2name=N'LastUpdateTime'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Variable name (LocalPath, etc.)' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ATS_SystemParameters', @level2type=N'COLUMN',@level2name=N'Variable'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Variable description' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ATS_SystemParameters', @level2type=N'COLUMN',@level2name=N'Description'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Variable type (int, string, etc.)' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ATS_SystemParameters', @level2type=N'COLUMN',@level2name=N'Type'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Variable value (C:\Users\user\Desktop\Projects\)' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ATS_SystemParameters', @level2type=N'COLUMN',@level2name=N'Value'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Unique version Id, PK' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ATS_Version', @level2type=N'COLUMN',@level2name=N'VersionId'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'FK, references Hierarchy.Id column (Group record in case of Related project)' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ATS_Version', @level2type=N'COLUMN',@level2name=N'HierarchyId'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Version Name, populated upon creation of any new version' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ATS_Version', @level2type=N'COLUMN',@level2name=N'VersionName'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Version Sequence number within project branch' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ATS_Version', @level2type=N'COLUMN',@level2name=N'VersionSeqNo'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Version Status - Active(A), Closed (C)' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ATS_Version', @level2type=N'COLUMN',@level2name=N'VersionStatus'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Version description. Mandatory, will be populated upon version creation' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ATS_Version', @level2type=N'COLUMN',@level2name=N'Description'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Target path for version files' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ATS_Version', @level2type=N'COLUMN',@level2name=N'TargetPath'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Version Creation date' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ATS_Version', @level2type=N'COLUMN',@level2name=N'CreationDate'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Control fields' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ATS_Version', @level2type=N'COLUMN',@level2name=N'LastUpdateTime'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'PK, unique row Id' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ATS_VersionContent', @level2type=N'COLUMN',@level2name=N'Rowid'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Version Unique Id, FK, references Version table' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ATS_VersionContent', @level2type=N'COLUMN',@level2name=N'VersionId'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Unique ContentVersionId of the content associated to the project' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ATS_VersionContent', @level2type=N'COLUMN',@level2name=N'ContentVersionId'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Content execution sequence number' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ATS_VersionContent', @level2type=N'COLUMN',@level2name=N'ContentSeqNo'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'The date when the content was assigned to the project' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ATS_VersionContent', @level2type=N'COLUMN',@level2name=N'CreationDate'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Control Field' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ATS_VersionContent', @level2type=N'COLUMN',@level2name=N'LastUpdateTime'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Control Field' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ATS_VersionContent', @level2type=N'COLUMN',@level2name=N'LastUpdateUser'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Control Field' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ATS_VersionContent', @level2type=N'COLUMN',@level2name=N'LastUpdateComputer'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Control Field' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ATS_VersionContent', @level2type=N'COLUMN',@level2name=N'LastUpdateApplication'
GO
USE [master]
GO
ALTER DATABASE [ATS] SET  READ_WRITE 
GO
