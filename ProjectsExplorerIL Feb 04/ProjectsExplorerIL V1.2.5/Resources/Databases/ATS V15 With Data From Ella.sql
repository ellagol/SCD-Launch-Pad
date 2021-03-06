USE [master]
GO
/****** Object:  Database [ATS]    Script Date: 12/19/2013 12:38:33 ******/
CREATE DATABASE [ATS]
GO
ALTER DATABASE [ATS] SET COMPATIBILITY_LEVEL = 100
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
ALTER DATABASE [ATS] SET AUTO_CLOSE OFF
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
ALTER DATABASE [ATS] SET  DISABLE_BROKER
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
ALTER DATABASE [ATS] SET  READ_WRITE
GO
ALTER DATABASE [ATS] SET RECOVERY SIMPLE
GO
ALTER DATABASE [ATS] SET  MULTI_USER
GO
ALTER DATABASE [ATS] SET PAGE_VERIFY CHECKSUM
GO
ALTER DATABASE [ATS] SET DB_CHAINING OFF
GO
USE [ATS]
GO
/****** Object:  Table [dbo].[ATS_Profile]    Script Date: 12/19/2013 12:38:36 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[ATS_Profile](
	[RowId] [int] NOT NULL,
	[ProfileDescription] [varchar](400) NULL,
	[LastUpdateTime] [datetime] NOT NULL,
	[LastUpdateUser] [nvarchar](50) NOT NULL,
	[LastUpdateComputer] [nvarchar](50) NOT NULL,
	[LastUpdateApplication] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_Profile] PRIMARY KEY CLUSTERED 
(
	[RowId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY],
 CONSTRAINT [UQ_ATS_ProfileDesc] UNIQUE NONCLUSTERED 
(
	[ProfileDescription] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Unique profile Id' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ATS_Profile', @level2type=N'COLUMN',@level2name=N'RowId'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Profile (Role) description' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ATS_Profile', @level2type=N'COLUMN',@level2name=N'ProfileDescription'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Control fields' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ATS_Profile', @level2type=N'COLUMN',@level2name=N'LastUpdateTime'
GO
INSERT [dbo].[ATS_Profile] ([RowId], [ProfileDescription], [LastUpdateTime], [LastUpdateUser], [LastUpdateComputer], [LastUpdateApplication]) VALUES (1, N'System Administrators', CAST(0x0000A26800000000 AS DateTime), N'Eli', N'ES', N'Manual')
INSERT [dbo].[ATS_Profile] ([RowId], [ProfileDescription], [LastUpdateTime], [LastUpdateUser], [LastUpdateComputer], [LastUpdateApplication]) VALUES (2, N'Default', CAST(0x0000A26800000000 AS DateTime), N'Eli', N'ES', N'Manual')
INSERT [dbo].[ATS_Profile] ([RowId], [ProfileDescription], [LastUpdateTime], [LastUpdateUser], [LastUpdateComputer], [LastUpdateApplication]) VALUES (3, N'ella', CAST(0x0000A26800000000 AS DateTime), N'el', N'my', N'my')
/****** Object:  Table [dbo].[ATS_Privilege]    Script Date: 12/19/2013 12:38:36 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[ATS_Privilege](
	[PrivilegeCode] [varchar](4) NOT NULL,
	[PrivilegeDescription] [varchar](400) NULL,
	[SubSystem] [nvarchar](50) NOT NULL,
	[LastUpdateTime] [datetime] NOT NULL,
	[LastUpdateUser] [nvarchar](50) NOT NULL,
	[LastUpdateComputer] [nvarchar](50) NOT NULL,
	[LastUpdateApplication] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_Privileges] PRIMARY KEY CLUSTERED 
(
	[PrivilegeCode] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY],
 CONSTRAINT [UQ_ATS_PrivilegeDesc] UNIQUE NONCLUSTERED 
(
	[PrivilegeDescription] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Activity Code. The column lists all activities that are subject for security validation.' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ATS_Privilege', @level2type=N'COLUMN',@level2name=N'PrivilegeCode'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Activity description' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ATS_Privilege', @level2type=N'COLUMN',@level2name=N'PrivilegeDescription'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Relevant application (Explorer, CM, RM)' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ATS_Privilege', @level2type=N'COLUMN',@level2name=N'SubSystem'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Control fields' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ATS_Privilege', @level2type=N'COLUMN',@level2name=N'LastUpdateTime'
GO
INSERT [dbo].[ATS_Privilege] ([PrivilegeCode], [PrivilegeDescription], [SubSystem], [LastUpdateTime], [LastUpdateUser], [LastUpdateComputer], [LastUpdateApplication]) VALUES (N'101', N'AddNew', N'Explorer', CAST(0x0000A26800000000 AS DateTime), N'Eli', N'Eli', N'Manual')
INSERT [dbo].[ATS_Privilege] ([PrivilegeCode], [PrivilegeDescription], [SubSystem], [LastUpdateTime], [LastUpdateUser], [LastUpdateComputer], [LastUpdateApplication]) VALUES (N'102', N'MoveHierarchyBranch', N'Explorer', CAST(0x0000A26800000000 AS DateTime), N'Eli', N'Eli', N'Manual')
INSERT [dbo].[ATS_Privilege] ([PrivilegeCode], [PrivilegeDescription], [SubSystem], [LastUpdateTime], [LastUpdateUser], [LastUpdateComputer], [LastUpdateApplication]) VALUES (N'103', N'DeleteFolder', N'Explorer', CAST(0x0000A26800000000 AS DateTime), N'Eli', N'Eli', N'Manual')
INSERT [dbo].[ATS_Privilege] ([PrivilegeCode], [PrivilegeDescription], [SubSystem], [LastUpdateTime], [LastUpdateUser], [LastUpdateComputer], [LastUpdateApplication]) VALUES (N'104', N'ViewDisabledNote', N'Explorer', CAST(0x0000A26800000000 AS DateTime), N'Eli', N'Eli', N'Manual')
INSERT [dbo].[ATS_Privilege] ([PrivilegeCode], [PrivilegeDescription], [SubSystem], [LastUpdateTime], [LastUpdateUser], [LastUpdateComputer], [LastUpdateApplication]) VALUES (N'105', N'DisableNote', N'Explorer', CAST(0x0000A26800000000 AS DateTime), N'Eli', N'Eli', N'Manual')
INSERT [dbo].[ATS_Privilege] ([PrivilegeCode], [PrivilegeDescription], [SubSystem], [LastUpdateTime], [LastUpdateUser], [LastUpdateComputer], [LastUpdateApplication]) VALUES (N'106', N'UpdateVersion', N'Explorer', CAST(0x0000A26800000000 AS DateTime), N'Eli', N'Eli', N'Manual')
INSERT [dbo].[ATS_Privilege] ([PrivilegeCode], [PrivilegeDescription], [SubSystem], [LastUpdateTime], [LastUpdateUser], [LastUpdateComputer], [LastUpdateApplication]) VALUES (N'107', N'AssignRetiredContent', N'Explorer', CAST(0x0000A26800000000 AS DateTime), N'Eli', N'Eli', N'Manual')
INSERT [dbo].[ATS_Privilege] ([PrivilegeCode], [PrivilegeDescription], [SubSystem], [LastUpdateTime], [LastUpdateUser], [LastUpdateComputer], [LastUpdateApplication]) VALUES (N'108', N'AssignCertificate', N'Explorer', CAST(0x0000A26800000000 AS DateTime), N'Eli', N'Eli', N'Manual')
INSERT [dbo].[ATS_Privilege] ([PrivilegeCode], [PrivilegeDescription], [SubSystem], [LastUpdateTime], [LastUpdateUser], [LastUpdateComputer], [LastUpdateApplication]) VALUES (N'109', N'RemoveCertificate', N'Explorer', CAST(0x0000A26800000000 AS DateTime), N'Eli', N'Eli', N'Manual')
INSERT [dbo].[ATS_Privilege] ([PrivilegeCode], [PrivilegeDescription], [SubSystem], [LastUpdateTime], [LastUpdateUser], [LastUpdateComputer], [LastUpdateApplication]) VALUES (N'110', N'AddNote', N'Explorer', CAST(0x0000A26800000000 AS DateTime), N'Eli', N'Eli', N'Manual')
INSERT [dbo].[ATS_Privilege] ([PrivilegeCode], [PrivilegeDescription], [SubSystem], [LastUpdateTime], [LastUpdateUser], [LastUpdateComputer], [LastUpdateApplication]) VALUES (N'111', N'EditNote', N'Explorer', CAST(0x0000A26800000000 AS DateTime), N'Eli', N'Eli', N'Manual')
INSERT [dbo].[ATS_Privilege] ([PrivilegeCode], [PrivilegeDescription], [SubSystem], [LastUpdateTime], [LastUpdateUser], [LastUpdateComputer], [LastUpdateApplication]) VALUES (N'112', N'ExecuteContent', N'Explorer', CAST(0x0000A26800000000 AS DateTime), N'Eli', N'Eli', N'Manual')
INSERT [dbo].[ATS_Privilege] ([PrivilegeCode], [PrivilegeDescription], [SubSystem], [LastUpdateTime], [LastUpdateUser], [LastUpdateComputer], [LastUpdateApplication]) VALUES (N'113', N'DisableProject', N'Explorer', CAST(0x0000A26800000000 AS DateTime), N'Eli', N'Eli', N'Manual')
INSERT [dbo].[ATS_Privilege] ([PrivilegeCode], [PrivilegeDescription], [SubSystem], [LastUpdateTime], [LastUpdateUser], [LastUpdateComputer], [LastUpdateApplication]) VALUES (N'114', N'UpdateDisableProject', N'Explorer', CAST(0x0000A26800000000 AS DateTime), N'Eli', N'Eli', N'Manual')
INSERT [dbo].[ATS_Privilege] ([PrivilegeCode], [PrivilegeDescription], [SubSystem], [LastUpdateTime], [LastUpdateUser], [LastUpdateComputer], [LastUpdateApplication]) VALUES (N'115', N'ResumeProject', N'Explorer', CAST(0x0000A26800000000 AS DateTime), N'Eli', N'Eli', N'Manual')
INSERT [dbo].[ATS_Privilege] ([PrivilegeCode], [PrivilegeDescription], [SubSystem], [LastUpdateTime], [LastUpdateUser], [LastUpdateComputer], [LastUpdateApplication]) VALUES (N'116', N'ExecuteContentClosedVersion', N'Explorer', CAST(0x0000A26800000000 AS DateTime), N'Eli', N'Eli', N'Manual')
INSERT [dbo].[ATS_Privilege] ([PrivilegeCode], [PrivilegeDescription], [SubSystem], [LastUpdateTime], [LastUpdateUser], [LastUpdateComputer], [LastUpdateApplication]) VALUES (N'117', N'ActivateContentDisabledProject', N'Explorer', CAST(0x0000A26800000000 AS DateTime), N'Eli', N'Eli', N'Manual')
INSERT [dbo].[ATS_Privilege] ([PrivilegeCode], [PrivilegeDescription], [SubSystem], [LastUpdateTime], [LastUpdateUser], [LastUpdateComputer], [LastUpdateApplication]) VALUES (N'118', N'ReopenClosedVersion', N'Explorer', CAST(0x0000A26800000000 AS DateTime), N'Eli', N'Eli', N'Manual')
INSERT [dbo].[ATS_Privilege] ([PrivilegeCode], [PrivilegeDescription], [SubSystem], [LastUpdateTime], [LastUpdateUser], [LastUpdateComputer], [LastUpdateApplication]) VALUES (N'119', N'CloneProject', N'Explorer', CAST(0x0000A26800000000 AS DateTime), N'Eli', N'Eli', N'Manual')
INSERT [dbo].[ATS_Privilege] ([PrivilegeCode], [PrivilegeDescription], [SubSystem], [LastUpdateTime], [LastUpdateUser], [LastUpdateComputer], [LastUpdateApplication]) VALUES (N'120', N'CloneRelatedProject', N'Explorer', CAST(0x0000A26800000000 AS DateTime), N'Eli', N'Eli', N'Manual')
INSERT [dbo].[ATS_Privilege] ([PrivilegeCode], [PrivilegeDescription], [SubSystem], [LastUpdateTime], [LastUpdateUser], [LastUpdateComputer], [LastUpdateApplication]) VALUES (N'121', N'EditProjectCode', N'Explorer', CAST(0x0000A26800000000 AS DateTime), N'Eli', N'Eli', N'Manual')
INSERT [dbo].[ATS_Privilege] ([PrivilegeCode], [PrivilegeDescription], [SubSystem], [LastUpdateTime], [LastUpdateUser], [LastUpdateComputer], [LastUpdateApplication]) VALUES (N'122', N'EditProjectStep', N'Explorer', CAST(0x0000A26800000000 AS DateTime), N'Eli', N'Eli', N'Manual')
INSERT [dbo].[ATS_Privilege] ([PrivilegeCode], [PrivilegeDescription], [SubSystem], [LastUpdateTime], [LastUpdateUser], [LastUpdateComputer], [LastUpdateApplication]) VALUES (N'123', N'EditProjectProperties', N'Explorer', CAST(0x0000A26800000000 AS DateTime), N'Eli', N'Eli', N'Manual')
INSERT [dbo].[ATS_Privilege] ([PrivilegeCode], [PrivilegeDescription], [SubSystem], [LastUpdateTime], [LastUpdateUser], [LastUpdateComputer], [LastUpdateApplication]) VALUES (N'124', N'UpdateRelatedProject', N'Explorer', CAST(0x0000A26800000000 AS DateTime), N'Eli', N'Eli', N'Manual')
INSERT [dbo].[ATS_Privilege] ([PrivilegeCode], [PrivilegeDescription], [SubSystem], [LastUpdateTime], [LastUpdateUser], [LastUpdateComputer], [LastUpdateApplication]) VALUES (N'125', N'SplitProjectFromGroup', N'Explorer', CAST(0x0000A26800000000 AS DateTime), N'Eli', N'Eli', N'Manual')
INSERT [dbo].[ATS_Privilege] ([PrivilegeCode], [PrivilegeDescription], [SubSystem], [LastUpdateTime], [LastUpdateUser], [LastUpdateComputer], [LastUpdateApplication]) VALUES (N'126', N'ActivateProjectFromUncertifiedWorkstation', N'Explorer', CAST(0x0000A26800000000 AS DateTime), N'Eli', N'Eli', N'Manual')
INSERT [dbo].[ATS_Privilege] ([PrivilegeCode], [PrivilegeDescription], [SubSystem], [LastUpdateTime], [LastUpdateUser], [LastUpdateComputer], [LastUpdateApplication]) VALUES (N'127', N'UpdateTargetDirectory', N'Explorer', CAST(0x0000A26800000000 AS DateTime), N'Eli', N'Eli', N'Manual')
INSERT [dbo].[ATS_Privilege] ([PrivilegeCode], [PrivilegeDescription], [SubSystem], [LastUpdateTime], [LastUpdateUser], [LastUpdateComputer], [LastUpdateApplication]) VALUES (N'999', N'AllActivities', N'Explorer', CAST(0x0000A26800000000 AS DateTime), N'Eli', N'Eli', N'Manual')
/****** Object:  Table [dbo].[ATS_NoteType]    Script Date: 12/19/2013 12:38:36 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[ATS_NoteType](
	[NoteType] [nchar](2) NOT NULL,
	[Description] [varchar](200) NULL,
	[LastUpdateTime] [datetime] NULL,
	[LastUpdateUser] [varchar](50) NULL,
	[LastUpdateComputer] [nvarchar](50) NULL,
	[LastUpdateapplication] [nvarchar](50) NULL,
 CONSTRAINT [PK_NoteType] PRIMARY KEY CLUSTERED 
(
	[NoteType] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY],
 CONSTRAINT [UQ_NoteTypeDescription] UNIQUE NONCLUSTERED 
(
	[Description] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Note type - W or C' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ATS_NoteType', @level2type=N'COLUMN',@level2name=N'NoteType'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Type description. Work Instruction (W) or Comment (C)' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ATS_NoteType', @level2type=N'COLUMN',@level2name=N'Description'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Control fields' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ATS_NoteType', @level2type=N'COLUMN',@level2name=N'LastUpdateTime'
GO
INSERT [dbo].[ATS_NoteType] ([NoteType], [Description], [LastUpdateTime], [LastUpdateUser], [LastUpdateComputer], [LastUpdateapplication]) VALUES (N'C ', N'Comment', CAST(0x0000A28800E55DC1 AS DateTime), N'SysAdmin', N'MyComp', N'Explorer')
INSERT [dbo].[ATS_NoteType] ([NoteType], [Description], [LastUpdateTime], [LastUpdateUser], [LastUpdateComputer], [LastUpdateapplication]) VALUES (N'W ', N'Work Instructions', CAST(0x0000A28800E569B9 AS DateTime), N'SysAdmin', N'MyComp', N'Explorer')
/****** Object:  Table [dbo].[ATS_MessageType]    Script Date: 12/19/2013 12:38:36 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[ATS_MessageType](
	[Type] [nchar](2) NOT NULL,
	[Description] [varchar](200) NULL,
	[LastUpdateTime] [datetime] NOT NULL,
	[LastUpdateUser] [varchar](50) NULL,
	[LastUpdateComputer] [nvarchar](50) NOT NULL,
	[LastUpdateApplication] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_MessageType] PRIMARY KEY CLUSTERED 
(
	[Type] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY],
 CONSTRAINT [UQ_ATS_MessageTypeDescription] UNIQUE NONCLUSTERED 
(
	[Description] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'E, W, I' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ATS_MessageType', @level2type=N'COLUMN',@level2name=N'Type'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'E-Error, W-Warning, I-Info' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ATS_MessageType', @level2type=N'COLUMN',@level2name=N'Description'
GO
INSERT [dbo].[ATS_MessageType] ([Type], [Description], [LastUpdateTime], [LastUpdateUser], [LastUpdateComputer], [LastUpdateApplication]) VALUES (N'E ', N'Error', CAST(0x0000A28800D91432 AS DateTime), N'SysAdmin', N'MyCmp', N'Explorer')
INSERT [dbo].[ATS_MessageType] ([Type], [Description], [LastUpdateTime], [LastUpdateUser], [LastUpdateComputer], [LastUpdateApplication]) VALUES (N'I ', N'Info', CAST(0x0000A28800D9A05E AS DateTime), N'SysAdmin', N'MyCmp', N'Explorer')
INSERT [dbo].[ATS_MessageType] ([Type], [Description], [LastUpdateTime], [LastUpdateUser], [LastUpdateComputer], [LastUpdateApplication]) VALUES (N'W ', N'Warning', CAST(0x0000A28800D989F8 AS DateTime), N'SysAdmin', N'MyCmp', N'Explorer')
/****** Object:  Table [dbo].[ATS_SystemParameters]    Script Date: 12/19/2013 12:38:36 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[ATS_SystemParameters](
	[Variable] [nvarchar](50) NOT NULL,
	[Description] [nvarchar](500) NOT NULL,
	[Type] [nvarchar](50) NOT NULL,
	[Value] [nvarchar](500) NOT NULL,
	[LastUpdateTime] [datetime] NOT NULL,
	[LastUpdateUser] [varchar](50) NULL,
	[LastUpdateComputer] [nvarchar](50) NOT NULL,
	[LastUpdateapplication] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_VariableName] PRIMARY KEY CLUSTERED 
(
	[Variable] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Variable name (LocalPath, etc.)' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ATS_SystemParameters', @level2type=N'COLUMN',@level2name=N'Variable'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Variable description' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ATS_SystemParameters', @level2type=N'COLUMN',@level2name=N'Description'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Variable type (int, string, etc.)' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ATS_SystemParameters', @level2type=N'COLUMN',@level2name=N'Type'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Variable value (C:\Users\user\Desktop\Projects\)' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ATS_SystemParameters', @level2type=N'COLUMN',@level2name=N'Value'
GO
INSERT [dbo].[ATS_SystemParameters] ([Variable], [Description], [Type], [Value], [LastUpdateTime], [LastUpdateUser], [LastUpdateComputer], [LastUpdateapplication]) VALUES (N'CMConnectionString', N'Content Management DB Connection String', N'string', N'Data Source=WST40010478\sqlexpress;Initial Catalog=GenPR_Test;Integrated Security=True', CAST(0x0000A27D010958A6 AS DateTime), N'Admin', N'MyComp', N'Explorer')
INSERT [dbo].[ATS_SystemParameters] ([Variable], [Description], [Type], [Value], [LastUpdateTime], [LastUpdateUser], [LastUpdateComputer], [LastUpdateapplication]) VALUES (N'ProjectLocalPath', N'Regular Projects Files Local Path', N'String', N'C:/Temp/Projects', CAST(0x0000A28800FB6C20 AS DateTime), N'SysAdmin', N'MyComp', N'Explorer')
INSERT [dbo].[ATS_SystemParameters] ([Variable], [Description], [Type], [Value], [LastUpdateTime], [LastUpdateUser], [LastUpdateComputer], [LastUpdateapplication]) VALUES (N'RelatedProjectLocalPath', N'Related Projects Files Local Path', N'String', N'C:/Temp/Groups', CAST(0x0000A28800FB495B AS DateTime), N'SysAdmin', N'MyComp', N'Explorer')
INSERT [dbo].[ATS_SystemParameters] ([Variable], [Description], [Type], [Value], [LastUpdateTime], [LastUpdateUser], [LastUpdateComputer], [LastUpdateapplication]) VALUES (N'RequiredDiskSpace', N'Disk space required for Content files', N'String', N'0', CAST(0x0000A28800FBE336 AS DateTime), N'SysAdmin', N'MyComp', N'Explorer')
INSERT [dbo].[ATS_SystemParameters] ([Variable], [Description], [Type], [Value], [LastUpdateTime], [LastUpdateUser], [LastUpdateComputer], [LastUpdateapplication]) VALUES (N'RMConnectionString', N'Resource Management DB Connection String', N'string', N'Data Source=WST40010478\sqlexpress;Initial Catalog=GenPR_Test;Integrated Security=True', CAST(0x0000A27D010958A7 AS DateTime), N'Admin', N'MyComp', N'Explorer')
/****** Object:  Table [dbo].[ATS_SchemaVersion]    Script Date: 12/19/2013 12:38:36 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ATS_SchemaVersion](
	[VersionNo] [int] NOT NULL,
	[Date] [datetime] NOT NULL
) ON [PRIMARY]
GO
INSERT [dbo].[ATS_SchemaVersion] ([VersionNo], [Date]) VALUES (2, CAST(0x0000A26B00B54F68 AS DateTime))
INSERT [dbo].[ATS_SchemaVersion] ([VersionNo], [Date]) VALUES (1, CAST(0x0000A25D00AB9B29 AS DateTime))
INSERT [dbo].[ATS_SchemaVersion] ([VersionNo], [Date]) VALUES (3, CAST(0x0000A2730106D2D2 AS DateTime))
INSERT [dbo].[ATS_SchemaVersion] ([VersionNo], [Date]) VALUES (4, CAST(0x0000A27901632DB5 AS DateTime))
INSERT [dbo].[ATS_SchemaVersion] ([VersionNo], [Date]) VALUES (5, CAST(0x0000A279016373EE AS DateTime))
INSERT [dbo].[ATS_SchemaVersion] ([VersionNo], [Date]) VALUES (6, CAST(0x0000A27D0109E9C0 AS DateTime))
INSERT [dbo].[ATS_SchemaVersion] ([VersionNo], [Date]) VALUES (7, CAST(0x0000A28100D8AAD8 AS DateTime))
INSERT [dbo].[ATS_SchemaVersion] ([VersionNo], [Date]) VALUES (8, CAST(0x0000A28800FF2CA1 AS DateTime))
INSERT [dbo].[ATS_SchemaVersion] ([VersionNo], [Date]) VALUES (9, CAST(0x0000A28A00F8D59E AS DateTime))
INSERT [dbo].[ATS_SchemaVersion] ([VersionNo], [Date]) VALUES (10, CAST(0x0000A28B0097CF36 AS DateTime))
INSERT [dbo].[ATS_SchemaVersion] ([VersionNo], [Date]) VALUES (15, CAST(0x0000A28F00FD7467 AS DateTime))
INSERT [dbo].[ATS_SchemaVersion] ([VersionNo], [Date]) VALUES (11, CAST(0x0000A28E00A7779C AS DateTime))
INSERT [dbo].[ATS_SchemaVersion] ([VersionNo], [Date]) VALUES (12, CAST(0x0000A28E00AB1846 AS DateTime))
INSERT [dbo].[ATS_SchemaVersion] ([VersionNo], [Date]) VALUES (13, CAST(0x0000A28E00AB5691 AS DateTime))
INSERT [dbo].[ATS_SchemaVersion] ([VersionNo], [Date]) VALUES (14, CAST(0x0000A28F00A6A6F2 AS DateTime))
/****** Object:  Table [dbo].[ATS_ProjectStep]    Script Date: 12/19/2013 12:38:36 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[ATS_ProjectStep](
	[StepCode] [nvarchar](20) NOT NULL,
	[StepDescription] [varchar](200) NOT NULL,
	[LastUpdateTime] [datetime] NOT NULL,
	[LastUpdateUser] [varchar](50) NULL,
	[LastUpdateComputer] [nvarchar](50) NOT NULL,
	[LastUpdateapplication] [nvarchar](50) NOT NULL,
 CONSTRAINT [ProjectStep_PK] PRIMARY KEY CLUSTERED 
(
	[StepCode] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY],
 CONSTRAINT [UQ_ATS_ProjectStepDesc] UNIQUE NONCLUSTERED 
(
	[StepDescription] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Step code (KADEL, STEP1, etc.), populated by SCD' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ATS_ProjectStep', @level2type=N'COLUMN',@level2name=N'StepCode'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Step description' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ATS_ProjectStep', @level2type=N'COLUMN',@level2name=N'StepDescription'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Control fields' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ATS_ProjectStep', @level2type=N'COLUMN',@level2name=N'LastUpdateTime'
GO
INSERT [dbo].[ATS_ProjectStep] ([StepCode], [StepDescription], [LastUpdateTime], [LastUpdateUser], [LastUpdateComputer], [LastUpdateapplication]) VALUES (N'Kadel               ', N'Kadel', CAST(0x0000A27C00DDA185 AS DateTime), N'Sys', N'Comp', N'PPP')
INSERT [dbo].[ATS_ProjectStep] ([StepCode], [StepDescription], [LastUpdateTime], [LastUpdateUser], [LastUpdateComputer], [LastUpdateapplication]) VALUES (N'Step1               ', N'Step 1', CAST(0x0000A27C00DDC4E4 AS DateTime), N'Sys', N'Comp', N'PPP')
INSERT [dbo].[ATS_ProjectStep] ([StepCode], [StepDescription], [LastUpdateTime], [LastUpdateUser], [LastUpdateComputer], [LastUpdateapplication]) VALUES (N'Step1a              ', N'Step 1a', CAST(0x0000A27400B9A2A6 AS DateTime), N'Admin', N'MyComp', N'Explorer')
INSERT [dbo].[ATS_ProjectStep] ([StepCode], [StepDescription], [LastUpdateTime], [LastUpdateUser], [LastUpdateComputer], [LastUpdateapplication]) VALUES (N'Step2               ', N'Step 2', CAST(0x0000A27400B9DDF1 AS DateTime), N'Admin', N'MyComp', N'Explorer')
/****** Object:  Table [dbo].[ATS_ProfilePrivilege]    Script Date: 12/19/2013 12:38:36 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[ATS_ProfilePrivilege](
	[RowId] [int] NOT NULL,
	[ProfileId] [int] NOT NULL,
	[PrivilegeCode] [varchar](4) NOT NULL,
	[LastUpdateTime] [datetime] NULL,
	[LastUpdateUser] [nvarchar](50) NULL,
	[LastUpdateComputer] [nvarchar](50) NULL,
	[LastUpdateApplicationr] [nvarchar](50) NULL,
 CONSTRAINT [PK_ProfilePrivilege] PRIMARY KEY CLUSTERED 
(
	[RowId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Unique row Id' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ATS_ProfilePrivilege', @level2type=N'COLUMN',@level2name=N'RowId'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Profile Id, FK, references ATS_Profile.RowId column' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ATS_ProfilePrivilege', @level2type=N'COLUMN',@level2name=N'ProfileId'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Privilege code, FK, references ATS_Privilege.PrivilegeCode' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ATS_ProfilePrivilege', @level2type=N'COLUMN',@level2name=N'PrivilegeCode'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Control fields' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ATS_ProfilePrivilege', @level2type=N'COLUMN',@level2name=N'LastUpdateTime'
GO
INSERT [dbo].[ATS_ProfilePrivilege] ([RowId], [ProfileId], [PrivilegeCode], [LastUpdateTime], [LastUpdateUser], [LastUpdateComputer], [LastUpdateApplicationr]) VALUES (1, 1, N'999', CAST(0x0000A26800000000 AS DateTime), N'Eli', N'ES', N'Manual')
INSERT [dbo].[ATS_ProfilePrivilege] ([RowId], [ProfileId], [PrivilegeCode], [LastUpdateTime], [LastUpdateUser], [LastUpdateComputer], [LastUpdateApplicationr]) VALUES (2, 2, N'101', CAST(0x0000A26800000000 AS DateTime), N'Eli', N'ES', N'Manual')
INSERT [dbo].[ATS_ProfilePrivilege] ([RowId], [ProfileId], [PrivilegeCode], [LastUpdateTime], [LastUpdateUser], [LastUpdateComputer], [LastUpdateApplicationr]) VALUES (3, 2, N'102', CAST(0x0000A26800000000 AS DateTime), N'Eli', N'ES', N'Manual')
INSERT [dbo].[ATS_ProfilePrivilege] ([RowId], [ProfileId], [PrivilegeCode], [LastUpdateTime], [LastUpdateUser], [LastUpdateComputer], [LastUpdateApplicationr]) VALUES (4, 2, N'120', CAST(0x0000A26800000000 AS DateTime), N'Eli', N'ES', N'Manual')
INSERT [dbo].[ATS_ProfilePrivilege] ([RowId], [ProfileId], [PrivilegeCode], [LastUpdateTime], [LastUpdateUser], [LastUpdateComputer], [LastUpdateApplicationr]) VALUES (5, 2, N'105', CAST(0x0000A26800000000 AS DateTime), N'Eli', N'ES', N'Manual')
INSERT [dbo].[ATS_ProfilePrivilege] ([RowId], [ProfileId], [PrivilegeCode], [LastUpdateTime], [LastUpdateUser], [LastUpdateComputer], [LastUpdateApplicationr]) VALUES (6, 2, N'110', CAST(0x0000A26800000000 AS DateTime), N'Eli', N'ES', N'Manual')
INSERT [dbo].[ATS_ProfilePrivilege] ([RowId], [ProfileId], [PrivilegeCode], [LastUpdateTime], [LastUpdateUser], [LastUpdateComputer], [LastUpdateApplicationr]) VALUES (7, 2, N'111', CAST(0x0000A26800000000 AS DateTime), N'Eli', N'ES', N'Manual')
/****** Object:  Table [dbo].[ATS_UserProfile]    Script Date: 12/19/2013 12:38:36 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ATS_UserProfile](
	[RowId] [int] NOT NULL,
	[UserId] [nvarchar](50) NOT NULL,
	[ProfileId] [int] NOT NULL,
	[SubSystem] [nvarchar](50) NULL,
	[EffectiveDate] [nchar](10) NOT NULL,
	[ExpirationDate] [nchar](10) NULL,
	[LastUpdateTime] [datetime] NOT NULL,
	[LastUpdateUser] [nvarchar](50) NOT NULL,
	[LastUpdateComputer] [nvarchar](50) NOT NULL,
	[LastUpdateApplicationr] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_UserProfile] PRIMARY KEY CLUSTERED 
(
	[RowId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Unique row Id' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ATS_UserProfile', @level2type=N'COLUMN',@level2name=N'RowId'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'User Id' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ATS_UserProfile', @level2type=N'COLUMN',@level2name=N'UserId'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Profile Id, FK references ATS_Profile.RowId column' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ATS_UserProfile', @level2type=N'COLUMN',@level2name=N'ProfileId'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Relevant application (Explorer, CM, RM)' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ATS_UserProfile', @level2type=N'COLUMN',@level2name=N'SubSystem'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Profile assignment effective date' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ATS_UserProfile', @level2type=N'COLUMN',@level2name=N'EffectiveDate'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Profile assignment expiration date' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ATS_UserProfile', @level2type=N'COLUMN',@level2name=N'ExpirationDate'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Control Fields' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ATS_UserProfile', @level2type=N'COLUMN',@level2name=N'LastUpdateTime'
GO
INSERT [dbo].[ATS_UserProfile] ([RowId], [UserId], [ProfileId], [SubSystem], [EffectiveDate], [ExpirationDate], [LastUpdateTime], [LastUpdateUser], [LastUpdateComputer], [LastUpdateApplicationr]) VALUES (1, N'SysAdmin', 1, N'Explorer', N'2013-01-01', NULL, CAST(0x0000A26800000000 AS DateTime), N'Eli', N'ES', N'Manual')
INSERT [dbo].[ATS_UserProfile] ([RowId], [UserId], [ProfileId], [SubSystem], [EffectiveDate], [ExpirationDate], [LastUpdateTime], [LastUpdateUser], [LastUpdateComputer], [LastUpdateApplicationr]) VALUES (2, N'EliSt', 2, N'Explorer', N'2013-01-01', NULL, CAST(0x0000A26800000000 AS DateTime), N'Eli', N'ES', N'Manual')
/****** Object:  Table [dbo].[ATS_Messages]    Script Date: 12/19/2013 12:38:36 ******/
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
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Message Id. Message text is retrived by Message Id' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ATS_Messages', @level2type=N'COLUMN',@level2name=N'Id'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Message text' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ATS_Messages', @level2type=N'COLUMN',@level2name=N'Description'
GO
INSERT [dbo].[ATS_Messages] ([Id], [Description], [Type], [LastUpdateTime], [LastUpdateUser], [LastUpdateComputer], [LastUpdateApplication]) VALUES (100, N'The name is already in use. Please specify another name.', N'E ', CAST(0x0000A26400BF4A85 AS DateTime), N'SysAdmin', N'ES-W520', N'Explorer')
INSERT [dbo].[ATS_Messages] ([Id], [Description], [Type], [LastUpdateTime], [LastUpdateUser], [LastUpdateComputer], [LastUpdateApplication]) VALUES (101, N'Are you sure you want to remove the folder?', N'W ', CAST(0x0000A26400BF4A85 AS DateTime), N'SysAdmin', N'ES-W521', N'Explorer')
INSERT [dbo].[ATS_Messages] ([Id], [Description], [Type], [LastUpdateTime], [LastUpdateUser], [LastUpdateComputer], [LastUpdateApplication]) VALUES (102, N'Are you sure you want to disable the note?', N'W ', CAST(0x0000A26400BF4A86 AS DateTime), N'SysAdmin', N'ES-W522', N'Explorer')
INSERT [dbo].[ATS_Messages] ([Id], [Description], [Type], [LastUpdateTime], [LastUpdateUser], [LastUpdateComputer], [LastUpdateApplication]) VALUES (103, N'Are you sure you want to remove the certificate?', N'W ', CAST(0x0000A26400BF4A86 AS DateTime), N'SysAdmin', N'ES-W523', N'Explorer')
INSERT [dbo].[ATS_Messages] ([Id], [Description], [Type], [LastUpdateTime], [LastUpdateUser], [LastUpdateComputer], [LastUpdateApplication]) VALUES (104, N'An object has been updated by another user. Please refresh and try again', N'E ', CAST(0x0000A26400BF4A86 AS DateTime), N'SysAdmin', N'ES-W524', N'Explorer')
INSERT [dbo].[ATS_Messages] ([Id], [Description], [Type], [LastUpdateTime], [LastUpdateUser], [LastUpdateComputer], [LastUpdateApplication]) VALUES (105, N'Unexpected error  occurred. Please try again', N'E ', CAST(0x0000A26400BF4A87 AS DateTime), N'SysAdmin', N'ES-W525', N'Explorer')
INSERT [dbo].[ATS_Messages] ([Id], [Description], [Type], [LastUpdateTime], [LastUpdateUser], [LastUpdateComputer], [LastUpdateApplication]) VALUES (106, N'User %name is not authorized to perform this action.', N'I ', CAST(0x0000A26400BF4A87 AS DateTime), N'SysAdmin', N'ES-W526', N'Explorer')
INSERT [dbo].[ATS_Messages] ([Id], [Description], [Type], [LastUpdateTime], [LastUpdateUser], [LastUpdateComputer], [LastUpdateApplication]) VALUES (107, N'Current workstation is not certified for the project. The following certificates are missing: %list of missing certificates. Would youlike to proceed anyway?', N'I ', CAST(0x0000A26400BF4A87 AS DateTime), N'SysAdmin', N'ES-W527', N'Explorer')
INSERT [dbo].[ATS_Messages] ([Id], [Description], [Type], [LastUpdateTime], [LastUpdateUser], [LastUpdateComputer], [LastUpdateApplication]) VALUES (108, N'Two or more Content versions of the same Content have been added. Please remove and try again', N'E ', CAST(0x0000A26400BF4A88 AS DateTime), N'SysAdmin', N'ES-W528', N'Explorer')
INSERT [dbo].[ATS_Messages] ([Id], [Description], [Type], [LastUpdateTime], [LastUpdateUser], [LastUpdateComputer], [LastUpdateApplication]) VALUES (109, N'Two or more certificates with the same Certificate ID have been added. Please remove and try again. ', N'E ', CAST(0x0000A26400BF4A88 AS DateTime), N'SysAdmin', N'ES-W529', N'Explorer')
INSERT [dbo].[ATS_Messages] ([Id], [Description], [Type], [LastUpdateTime], [LastUpdateUser], [LastUpdateComputer], [LastUpdateApplication]) VALUES (110, N'Project Code and Step combination is not unique. Please update and try again.', N'E ', CAST(0x0000A26400BF4A88 AS DateTime), N'SysAdmin', N'ES-W530', N'Explorer')
INSERT [dbo].[ATS_Messages] ([Id], [Description], [Type], [LastUpdateTime], [LastUpdateUser], [LastUpdateComputer], [LastUpdateApplication]) VALUES (111, N'Project ''%name'' is related to the group. Would you like to update all projects belonging to the group?', N'W ', CAST(0x0000A26400BF4A88 AS DateTime), N'SysAdmin', N'ES-W531', N'Explorer')
INSERT [dbo].[ATS_Messages] ([Id], [Description], [Type], [LastUpdateTime], [LastUpdateUser], [LastUpdateComputer], [LastUpdateApplication]) VALUES (112, N'Version %[active versio name] will be closed. Are you sure you want to reopen version %name?', N'W ', CAST(0x0000A26400BF4A89 AS DateTime), N'SysAdmin', N'ES-W532', N'Explorer')
INSERT [dbo].[ATS_Messages] ([Id], [Description], [Type], [LastUpdateTime], [LastUpdateUser], [LastUpdateComputer], [LastUpdateApplication]) VALUES (114, N'Folder ''%name'' has been created successfully', N'I ', CAST(0x0000A26400BF4A89 AS DateTime), N'SysAdmin', N'ES-W533', N'Explorer')
INSERT [dbo].[ATS_Messages] ([Id], [Description], [Type], [LastUpdateTime], [LastUpdateUser], [LastUpdateComputer], [LastUpdateApplication]) VALUES (115, N'Project ''%name'' has been created successfully', N'I ', CAST(0x0000A26400BF4A89 AS DateTime), N'SysAdmin', N'ES-W534', N'Explorer')
INSERT [dbo].[ATS_Messages] ([Id], [Description], [Type], [LastUpdateTime], [LastUpdateUser], [LastUpdateComputer], [LastUpdateApplication]) VALUES (116, N'New related project ''%name'' has been successfully created and assigned to the group', N'I ', CAST(0x0000A26400BF4A8A AS DateTime), N'SysAdmin', N'ES-W535', N'Explorer')
INSERT [dbo].[ATS_Messages] ([Id], [Description], [Type], [LastUpdateTime], [LastUpdateUser], [LastUpdateComputer], [LastUpdateApplication]) VALUES (117, N'Related projects group has been successfully updated.', N'I ', CAST(0x0000A26400BF4A8A AS DateTime), N'SysAdmin', N'ES-W536', N'Explorer')
INSERT [dbo].[ATS_Messages] ([Id], [Description], [Type], [LastUpdateTime], [LastUpdateUser], [LastUpdateComputer], [LastUpdateApplication]) VALUES (118, N'Are you sure you want to create a new version?', N'W ', CAST(0x0000A26400BF4A8A AS DateTime), N'SysAdmin', N'ES-W537', N'Explorer')
INSERT [dbo].[ATS_Messages] ([Id], [Description], [Type], [LastUpdateTime], [LastUpdateUser], [LastUpdateComputer], [LastUpdateApplication]) VALUES (119, N'Content %name1 is referenced by content %name2. Please remove and try again.', N'E ', CAST(0x0000A26400BF4A8B AS DateTime), N'SysAdmin', N'ES-W538', N'Explorer')
INSERT [dbo].[ATS_Messages] ([Id], [Description], [Type], [LastUpdateTime], [LastUpdateUser], [LastUpdateComputer], [LastUpdateApplication]) VALUES (120, N'Content %name is retired. Do you want to assign retired content to the project', N'W ', CAST(0x0000A26400BF4A8B AS DateTime), N'SysAdmin', N'ES-W539', N'Explorer')
INSERT [dbo].[ATS_Messages] ([Id], [Description], [Type], [LastUpdateTime], [LastUpdateUser], [LastUpdateComputer], [LastUpdateApplication]) VALUES (121, N'There is not enough disk space. Please free disk space and try again.', N'E ', CAST(0x0000A26400BF4A8B AS DateTime), N'SysAdmin', N'ES-W540', N'Explorer')
INSERT [dbo].[ATS_Messages] ([Id], [Description], [Type], [LastUpdateTime], [LastUpdateUser], [LastUpdateComputer], [LastUpdateApplication]) VALUES (122, N'Content %name1 and content %name2 are referenced by content %name3. Please remove %name1 or %name2 and try again.', N'E ', CAST(0x0000A26400BF4A8B AS DateTime), N'SysAdmin', N'ES-W541', N'Explorer')
INSERT [dbo].[ATS_Messages] ([Id], [Description], [Type], [LastUpdateTime], [LastUpdateUser], [LastUpdateComputer], [LastUpdateApplication]) VALUES (123, N'Related projects Group Name is already in use. Please specify another name', N'E ', CAST(0x0000A26400BF4A8C AS DateTime), N'SysAdmin', N'ES-W542', N'Explorer')
INSERT [dbo].[ATS_Messages] ([Id], [Description], [Type], [LastUpdateTime], [LastUpdateUser], [LastUpdateComputer], [LastUpdateApplication]) VALUES (124, N'Version Name is already in use. Please specify another name.', N'E ', CAST(0x0000A26400BF4A8C AS DateTime), N'SysAdmin', N'ES-W543', N'Explorer')
/****** Object:  Table [dbo].[ATS_Hierarchy]    Script Date: 12/19/2013 12:38:36 ******/
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
	[ProjectStep] [nvarchar](20) NULL,
	[ProjectCode] [nvarchar](20) NULL,
	[ProjectStatus] [nchar](1) NULL,
	[SCDUSASyncInd] [bit] NULL,
	[CreationDate] [datetime] NOT NULL,
	[LastUpdateTime] [datetime] NOT NULL,
	[LastUpdateUser] [nvarchar](50) NOT NULL,
	[LastUpdateComputer] [nvarchar](50) NOT NULL,
	[LastUpdateApplication] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_ProjectHierarchy] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
CREATE UNIQUE NONCLUSTERED INDEX [IXUQ_HierarchyGroupName] ON [dbo].[ATS_Hierarchy] 
(
	[Name] ASC
)
WHERE ([NodeType]='G')
WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
CREATE UNIQUE NONCLUSTERED INDEX [IXUQ_HierarchyParentIdName] ON [dbo].[ATS_Hierarchy] 
(
	[ParentId] ASC,
	[Name] ASC
)
WHERE ([NodeType] IN ('F', 'P'))
WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
CREATE UNIQUE NONCLUSTERED INDEX [IXUQ_HierarchyProjectCodeStep] ON [dbo].[ATS_Hierarchy] 
(
	[ProjectCode] ASC,
	[ProjectStep] ASC
)
WHERE ([ProjectCode] IS NOT NULL)
WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
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
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Indicator that specifies whether the project should be synchronized with SCD USA' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ATS_Hierarchy', @level2type=N'COLUMN',@level2name=N'SCDUSASyncInd'
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
SET IDENTITY_INSERT [dbo].[ATS_Hierarchy] ON
INSERT [dbo].[ATS_Hierarchy] ([Id], [ParentId], [Sequence], [NodeType], [Name], [Description], [GroupId], [ProjectStep], [ProjectCode], [ProjectStatus], [SCDUSASyncInd], [CreationDate], [LastUpdateTime], [LastUpdateUser], [LastUpdateComputer], [LastUpdateApplication]) VALUES (100038, 320107, 10, N'F', N'Folder #1', N'Folder #1 Long Description', NULL, NULL, NULL, NULL, NULL, CAST(0x0000A26400BF4A85 AS DateTime), CAST(0x0000A28A01151BC0 AS DateTime), N'SysAdmin', N'WST40010478', N'Explorer')
INSERT [dbo].[ATS_Hierarchy] ([Id], [ParentId], [Sequence], [NodeType], [Name], [Description], [GroupId], [ProjectStep], [ProjectCode], [ProjectStatus], [SCDUSASyncInd], [CreationDate], [LastUpdateTime], [LastUpdateUser], [LastUpdateComputer], [LastUpdateApplication]) VALUES (100039, NULL, 10, N'F', N'Folder #2', N'Folder #2 Long Description', NULL, NULL, NULL, NULL, NULL, CAST(0x0000A26400BF69E5 AS DateTime), CAST(0x0000A26400BF7F00 AS DateTime), N'SysAdmin', N'ES-W520', N'Explorer')
INSERT [dbo].[ATS_Hierarchy] ([Id], [ParentId], [Sequence], [NodeType], [Name], [Description], [GroupId], [ProjectStep], [ProjectCode], [ProjectStatus], [SCDUSASyncInd], [CreationDate], [LastUpdateTime], [LastUpdateUser], [LastUpdateComputer], [LastUpdateApplication]) VALUES (100040, NULL, 10, N'F', N'Folder #3', N'Folder #3 very very long description', NULL, NULL, NULL, NULL, NULL, CAST(0x0000A26400BF83BA AS DateTime), CAST(0x0000A26400BF9E8F AS DateTime), N'SysAdmin', N'ES-W520', N'Explorer')
INSERT [dbo].[ATS_Hierarchy] ([Id], [ParentId], [Sequence], [NodeType], [Name], [Description], [GroupId], [ProjectStep], [ProjectCode], [ProjectStatus], [SCDUSASyncInd], [CreationDate], [LastUpdateTime], [LastUpdateUser], [LastUpdateComputer], [LastUpdateApplication]) VALUES (100041, 100038, 10, N'F', N'Folder #1A', N'', NULL, NULL, NULL, NULL, NULL, CAST(0x0000A26400BFA904 AS DateTime), CAST(0x0000A26A00E2D12C AS DateTime), N'SysAdmin', N'ES-W520', N'Explorer')
INSERT [dbo].[ATS_Hierarchy] ([Id], [ParentId], [Sequence], [NodeType], [Name], [Description], [GroupId], [ProjectStep], [ProjectCode], [ProjectStatus], [SCDUSASyncInd], [CreationDate], [LastUpdateTime], [LastUpdateUser], [LastUpdateComputer], [LastUpdateApplication]) VALUES (100042, 100038, 10, N'F', N'Folder #1B', N'', NULL, NULL, NULL, NULL, NULL, CAST(0x0000A26400BFBD8D AS DateTime), CAST(0x0000A26A00D7A09F AS DateTime), N'SysAdmin', N'ES-W520', N'Explorer')
INSERT [dbo].[ATS_Hierarchy] ([Id], [ParentId], [Sequence], [NodeType], [Name], [Description], [GroupId], [ProjectStep], [ProjectCode], [ProjectStatus], [SCDUSASyncInd], [CreationDate], [LastUpdateTime], [LastUpdateUser], [LastUpdateComputer], [LastUpdateApplication]) VALUES (100043, 100038, 10, N'F', N'Folder #1C', N'', NULL, NULL, NULL, NULL, NULL, CAST(0x0000A26400BFCFA8 AS DateTime), CAST(0x0000A26400C19D85 AS DateTime), N'SysAdmin', N'ES-W520', N'Explorer')
INSERT [dbo].[ATS_Hierarchy] ([Id], [ParentId], [Sequence], [NodeType], [Name], [Description], [GroupId], [ProjectStep], [ProjectCode], [ProjectStatus], [SCDUSASyncInd], [CreationDate], [LastUpdateTime], [LastUpdateUser], [LastUpdateComputer], [LastUpdateApplication]) VALUES (100044, 100039, 10, N'F', N'Folder #2A', N'', NULL, NULL, NULL, NULL, NULL, CAST(0x0000A26400BFDF53 AS DateTime), CAST(0x0000A26400BFF28D AS DateTime), N'SysAdmin', N'ES-W520', N'Explorer')
INSERT [dbo].[ATS_Hierarchy] ([Id], [ParentId], [Sequence], [NodeType], [Name], [Description], [GroupId], [ProjectStep], [ProjectCode], [ProjectStatus], [SCDUSASyncInd], [CreationDate], [LastUpdateTime], [LastUpdateUser], [LastUpdateComputer], [LastUpdateApplication]) VALUES (100045, 100040, 10, N'F', N'Folder #3A', N'', NULL, NULL, NULL, NULL, NULL, CAST(0x0000A26400BFF63D AS DateTime), CAST(0x0000A26400C00372 AS DateTime), N'SysAdmin', N'ES-W520', N'Explorer')
INSERT [dbo].[ATS_Hierarchy] ([Id], [ParentId], [Sequence], [NodeType], [Name], [Description], [GroupId], [ProjectStep], [ProjectCode], [ProjectStatus], [SCDUSASyncInd], [CreationDate], [LastUpdateTime], [LastUpdateUser], [LastUpdateComputer], [LastUpdateApplication]) VALUES (100046, 100041, 10, N'F', N'Folder #1A.i', N'', NULL, NULL, NULL, NULL, NULL, CAST(0x0000A26400C0093C AS DateTime), CAST(0x0000A26400C035F7 AS DateTime), N'SysAdmin', N'ES-W520', N'Explorer')
INSERT [dbo].[ATS_Hierarchy] ([Id], [ParentId], [Sequence], [NodeType], [Name], [Description], [GroupId], [ProjectStep], [ProjectCode], [ProjectStatus], [SCDUSASyncInd], [CreationDate], [LastUpdateTime], [LastUpdateUser], [LastUpdateComputer], [LastUpdateApplication]) VALUES (100047, 100041, 10, N'F', N'Folder #1A.ii', N'', NULL, NULL, NULL, NULL, NULL, CAST(0x0000A26400C03A86 AS DateTime), CAST(0x0000A26400C04981 AS DateTime), N'SysAdmin', N'ES-W520', N'Explorer')
INSERT [dbo].[ATS_Hierarchy] ([Id], [ParentId], [Sequence], [NodeType], [Name], [Description], [GroupId], [ProjectStep], [ProjectCode], [ProjectStatus], [SCDUSASyncInd], [CreationDate], [LastUpdateTime], [LastUpdateUser], [LastUpdateComputer], [LastUpdateApplication]) VALUES (101038, NULL, 10, N'F', N'Folder #4', N'Folder #4 Description', NULL, NULL, NULL, NULL, NULL, CAST(0x0000A26400C2042E AS DateTime), CAST(0x0000A26400C92E66 AS DateTime), N'SysAdmin', N'ES-W520', N'Explorer')
INSERT [dbo].[ATS_Hierarchy] ([Id], [ParentId], [Sequence], [NodeType], [Name], [Description], [GroupId], [ProjectStep], [ProjectCode], [ProjectStatus], [SCDUSASyncInd], [CreationDate], [LastUpdateTime], [LastUpdateUser], [LastUpdateComputer], [LastUpdateApplication]) VALUES (101039, 101038, 10, N'F', N'Folder #4A', N'', NULL, NULL, NULL, NULL, NULL, CAST(0x0000A26400C20458 AS DateTime), CAST(0x0000A26400C23804 AS DateTime), N'SysAdmin', N'ES-W520', N'Explorer')
INSERT [dbo].[ATS_Hierarchy] ([Id], [ParentId], [Sequence], [NodeType], [Name], [Description], [GroupId], [ProjectStep], [ProjectCode], [ProjectStatus], [SCDUSASyncInd], [CreationDate], [LastUpdateTime], [LastUpdateUser], [LastUpdateComputer], [LastUpdateApplication]) VALUES (101040, 101039, 10, N'F', N'Folder #4A.i', N'', NULL, NULL, NULL, NULL, NULL, CAST(0x0000A26400C2047E AS DateTime), CAST(0x0000A26400C23E94 AS DateTime), N'SysAdmin', N'ES-W520', N'Explorer')
INSERT [dbo].[ATS_Hierarchy] ([Id], [ParentId], [Sequence], [NodeType], [Name], [Description], [GroupId], [ProjectStep], [ProjectCode], [ProjectStatus], [SCDUSASyncInd], [CreationDate], [LastUpdateTime], [LastUpdateUser], [LastUpdateComputer], [LastUpdateApplication]) VALUES (101041, 101039, 10, N'F', N'Folder #4A.ii', N'', NULL, NULL, NULL, NULL, NULL, CAST(0x0000A26400C204A3 AS DateTime), CAST(0x0000A26400C2469F AS DateTime), N'SysAdmin', N'ES-W520', N'Explorer')
INSERT [dbo].[ATS_Hierarchy] ([Id], [ParentId], [Sequence], [NodeType], [Name], [Description], [GroupId], [ProjectStep], [ProjectCode], [ProjectStatus], [SCDUSASyncInd], [CreationDate], [LastUpdateTime], [LastUpdateUser], [LastUpdateComputer], [LastUpdateApplication]) VALUES (101042, 101038, 10, N'F', N'Folder #4B', N'', NULL, NULL, NULL, NULL, NULL, CAST(0x0000A26400C204C4 AS DateTime), CAST(0x0000A2650122FF0E AS DateTime), N'SysAdmin', N'ES-W520', N'Explorer')
INSERT [dbo].[ATS_Hierarchy] ([Id], [ParentId], [Sequence], [NodeType], [Name], [Description], [GroupId], [ProjectStep], [ProjectCode], [ProjectStatus], [SCDUSASyncInd], [CreationDate], [LastUpdateTime], [LastUpdateUser], [LastUpdateComputer], [LastUpdateApplication]) VALUES (101043, 101038, 10, N'F', N'Folder #4C', N'', NULL, NULL, NULL, NULL, NULL, CAST(0x0000A26400C204E9 AS DateTime), CAST(0x0000A264011212CC AS DateTime), N'SysAdmin', N'ES-W520', N'Explorer')
INSERT [dbo].[ATS_Hierarchy] ([Id], [ParentId], [Sequence], [NodeType], [Name], [Description], [GroupId], [ProjectStep], [ProjectCode], [ProjectStatus], [SCDUSASyncInd], [CreationDate], [LastUpdateTime], [LastUpdateUser], [LastUpdateComputer], [LastUpdateApplication]) VALUES (101044, 101038, 10, N'F', N'Folder #4D', N'', NULL, NULL, NULL, NULL, NULL, CAST(0x0000A26400C25ED1 AS DateTime), CAST(0x0000A26900BBEC27 AS DateTime), N'SysAdmin', N'ES-W520', N'Explorer')
INSERT [dbo].[ATS_Hierarchy] ([Id], [ParentId], [Sequence], [NodeType], [Name], [Description], [GroupId], [ProjectStep], [ProjectCode], [ProjectStatus], [SCDUSASyncInd], [CreationDate], [LastUpdateTime], [LastUpdateUser], [LastUpdateComputer], [LastUpdateApplication]) VALUES (101045, 101044, 10, N'F', N'Folder #4d.1', N'', NULL, NULL, NULL, NULL, NULL, CAST(0x0000A266012423BA AS DateTime), CAST(0x0000A26601244BAD AS DateTime), N'SysAdmin', N'ES-W520', N'Explorer')
INSERT [dbo].[ATS_Hierarchy] ([Id], [ParentId], [Sequence], [NodeType], [Name], [Description], [GroupId], [ProjectStep], [ProjectCode], [ProjectStatus], [SCDUSASyncInd], [CreationDate], [LastUpdateTime], [LastUpdateUser], [LastUpdateComputer], [LastUpdateApplication]) VALUES (101046, 101044, 10, N'F', N'Folder 4d.2', N'', NULL, NULL, NULL, NULL, NULL, CAST(0x0000A26601289E00 AS DateTime), CAST(0x0000A2660128B0F3 AS DateTime), N'SysAdmin', N'ES-W520', N'Explorer')
INSERT [dbo].[ATS_Hierarchy] ([Id], [ParentId], [Sequence], [NodeType], [Name], [Description], [GroupId], [ProjectStep], [ProjectCode], [ProjectStatus], [SCDUSASyncInd], [CreationDate], [LastUpdateTime], [LastUpdateUser], [LastUpdateComputer], [LastUpdateApplication]) VALUES (120055, 100042, 10, N'P', N'Project #1', N'Folder #1 Long Description', NULL, NULL, NULL, NULL, NULL, CAST(0x0000A26400BF4A85 AS DateTime), CAST(0x0000A26A00E5B3C6 AS DateTime), N'SysAdmin', N'ES-W520', N'Explorer')
INSERT [dbo].[ATS_Hierarchy] ([Id], [ParentId], [Sequence], [NodeType], [Name], [Description], [GroupId], [ProjectStep], [ProjectCode], [ProjectStatus], [SCDUSASyncInd], [CreationDate], [LastUpdateTime], [LastUpdateUser], [LastUpdateComputer], [LastUpdateApplication]) VALUES (120056, 100042, 10, N'P', N'Project #2', N'Folder #2 Long Description', NULL, N'Kadel               ', N'PP1                 ', N'O', NULL, CAST(0x0000A26400BF69E5 AS DateTime), CAST(0x0000A26400BF7F00 AS DateTime), N'SysAdmin', N'ES-W520', N'Explorer')
INSERT [dbo].[ATS_Hierarchy] ([Id], [ParentId], [Sequence], [NodeType], [Name], [Description], [GroupId], [ProjectStep], [ProjectCode], [ProjectStatus], [SCDUSASyncInd], [CreationDate], [LastUpdateTime], [LastUpdateUser], [LastUpdateComputer], [LastUpdateApplication]) VALUES (120058, 100042, 1, N'P', N'Project #1A', N'', NULL, N'Step1a              ', N'PP12                ', N'O', NULL, CAST(0x0000A26400BFA904 AS DateTime), CAST(0x0000A26A00E2D12C AS DateTime), N'SysAdmin', N'ES-W520', N'Explorer')
INSERT [dbo].[ATS_Hierarchy] ([Id], [ParentId], [Sequence], [NodeType], [Name], [Description], [GroupId], [ProjectStep], [ProjectCode], [ProjectStatus], [SCDUSASyncInd], [CreationDate], [LastUpdateTime], [LastUpdateUser], [LastUpdateComputer], [LastUpdateApplication]) VALUES (120059, 100042, 2, N'P', N'Project #1B', N'', NULL, NULL, NULL, NULL, NULL, CAST(0x0000A26400BFBD8D AS DateTime), CAST(0x0000A26A00D7A09F AS DateTime), N'SysAdmin', N'ES-W520', N'Explorer')
INSERT [dbo].[ATS_Hierarchy] ([Id], [ParentId], [Sequence], [NodeType], [Name], [Description], [GroupId], [ProjectStep], [ProjectCode], [ProjectStatus], [SCDUSASyncInd], [CreationDate], [LastUpdateTime], [LastUpdateUser], [LastUpdateComputer], [LastUpdateApplication]) VALUES (130058, 100042, 1, N'P', N'Project #1As', N'', NULL, N'Step1a              ', N'PP1 d               ', N'O', NULL, CAST(0x0000A26400BFA904 AS DateTime), CAST(0x0000A26A00E2D12C AS DateTime), N'SysAdmin', N'ES-W520', N'Explorer')
INSERT [dbo].[ATS_Hierarchy] ([Id], [ParentId], [Sequence], [NodeType], [Name], [Description], [GroupId], [ProjectStep], [ProjectCode], [ProjectStatus], [SCDUSASyncInd], [CreationDate], [LastUpdateTime], [LastUpdateUser], [LastUpdateComputer], [LastUpdateApplication]) VALUES (130059, 130058, 1, N'P', N'Project #1Ad', N'', NULL, N'Step1a              ', N'PP1  f              ', N'O', NULL, CAST(0x0000A26400BFA904 AS DateTime), CAST(0x0000A26A00E2D12C AS DateTime), N'SysAdmin', N'ES-W520', N'Explorer')
INSERT [dbo].[ATS_Hierarchy] ([Id], [ParentId], [Sequence], [NodeType], [Name], [Description], [GroupId], [ProjectStep], [ProjectCode], [ProjectStatus], [SCDUSASyncInd], [CreationDate], [LastUpdateTime], [LastUpdateUser], [LastUpdateComputer], [LastUpdateApplication]) VALUES (140059, NULL, 1, N'G', N'Group56', N'', NULL, N'Step1a              ', N'PP14                ', N'O', NULL, CAST(0x0000A26400BFA904 AS DateTime), CAST(0x0000A26A00E2D12C AS DateTime), N'SysAdmin', N'ES-W520', N'Explorer')
INSERT [dbo].[ATS_Hierarchy] ([Id], [ParentId], [Sequence], [NodeType], [Name], [Description], [GroupId], [ProjectStep], [ProjectCode], [ProjectStatus], [SCDUSASyncInd], [CreationDate], [LastUpdateTime], [LastUpdateUser], [LastUpdateComputer], [LastUpdateApplication]) VALUES (140060, NULL, 1, N'G', N'Group60', N'', NULL, N'Step1a              ', N'PP16                ', N'O', NULL, CAST(0x0000A26400BFA904 AS DateTime), CAST(0x0000A26A00E2D12C AS DateTime), N'SysAdmin', N'ES-W520', N'Explorer')
INSERT [dbo].[ATS_Hierarchy] ([Id], [ParentId], [Sequence], [NodeType], [Name], [Description], [GroupId], [ProjectStep], [ProjectCode], [ProjectStatus], [SCDUSASyncInd], [CreationDate], [LastUpdateTime], [LastUpdateUser], [LastUpdateComputer], [LastUpdateApplication]) VALUES (150075, 100047, 1, N'P', N'Proj30', N'', NULL, N'Step1a', N'PP25', N'O', 0, CAST(0x0000A26400BFA904 AS DateTime), CAST(0x0000A29900C61A00 AS DateTime), N'SysAdmin', N'WST40010478', N'Explorer')
INSERT [dbo].[ATS_Hierarchy] ([Id], [ParentId], [Sequence], [NodeType], [Name], [Description], [GroupId], [ProjectStep], [ProjectCode], [ProjectStatus], [SCDUSASyncInd], [CreationDate], [LastUpdateTime], [LastUpdateUser], [LastUpdateComputer], [LastUpdateApplication]) VALUES (150077, 100047, 1, N'P', N'Proj35', N'', NULL, N'Step1a              ', N'PP257               ', N'O', NULL, CAST(0x0000A26400BFA904 AS DateTime), CAST(0x0000A26A00E2D12C AS DateTime), N'SysAdmin', N'ES-W520', N'Explorer')
INSERT [dbo].[ATS_Hierarchy] ([Id], [ParentId], [Sequence], [NodeType], [Name], [Description], [GroupId], [ProjectStep], [ProjectCode], [ProjectStatus], [SCDUSASyncInd], [CreationDate], [LastUpdateTime], [LastUpdateUser], [LastUpdateComputer], [LastUpdateApplication]) VALUES (320064, 100042, 1, N'P', N'Project #1ggb', N'', NULL, N'Step1a              ', N'PP1345              ', N'O', NULL, CAST(0x0000A26400BFA904 AS DateTime), CAST(0x0000A26A00E2D12C AS DateTime), N'SysAdmin', N'ES-W520', N'Explorer')
INSERT [dbo].[ATS_Hierarchy] ([Id], [ParentId], [Sequence], [NodeType], [Name], [Description], [GroupId], [ProjectStep], [ProjectCode], [ProjectStatus], [SCDUSASyncInd], [CreationDate], [LastUpdateTime], [LastUpdateUser], [LastUpdateComputer], [LastUpdateApplication]) VALUES (320067, NULL, 1, N'P', N'Project #1ggb', N'', NULL, N'Step1a              ', N'PP1ttt              ', N'O', NULL, CAST(0x0000A26400BFA904 AS DateTime), CAST(0x0000A26A00E2D12C AS DateTime), N'SysAdmin', N'ES-W520', N'Explorer')
INSERT [dbo].[ATS_Hierarchy] ([Id], [ParentId], [Sequence], [NodeType], [Name], [Description], [GroupId], [ProjectStep], [ProjectCode], [ProjectStatus], [SCDUSASyncInd], [CreationDate], [LastUpdateTime], [LastUpdateUser], [LastUpdateComputer], [LastUpdateApplication]) VALUES (320069, 100046, 1, N'P', N'Project #1ggb', N'', NULL, N'Step1a              ', N'PP1trt              ', N'O', NULL, CAST(0x0000A26400BFA904 AS DateTime), CAST(0x0000A26A00E2D12C AS DateTime), N'SysAdmin', N'ES-W520', N'Explorer')
INSERT [dbo].[ATS_Hierarchy] ([Id], [ParentId], [Sequence], [NodeType], [Name], [Description], [GroupId], [ProjectStep], [ProjectCode], [ProjectStatus], [SCDUSASyncInd], [CreationDate], [LastUpdateTime], [LastUpdateUser], [LastUpdateComputer], [LastUpdateApplication]) VALUES (320070, 100046, 1, N'P', N'Project #1grrb', N'', NULL, NULL, N'PP1trt              ', N'O', NULL, CAST(0x0000A26400BFA904 AS DateTime), CAST(0x0000A26A00E2D12C AS DateTime), N'SysAdmin', N'ES-W520', N'Explorer')
INSERT [dbo].[ATS_Hierarchy] ([Id], [ParentId], [Sequence], [NodeType], [Name], [Description], [GroupId], [ProjectStep], [ProjectCode], [ProjectStatus], [SCDUSASyncInd], [CreationDate], [LastUpdateTime], [LastUpdateUser], [LastUpdateComputer], [LastUpdateApplication]) VALUES (320072, 100046, 1, N'P', N'Project #1gtrb', N'', NULL, N'Step2               ', N'PP1trt              ', N'O', NULL, CAST(0x0000A26400BFA904 AS DateTime), CAST(0x0000A26A00E2D12C AS DateTime), N'SysAdmin', N'ES-W520', N'Explorer')
INSERT [dbo].[ATS_Hierarchy] ([Id], [ParentId], [Sequence], [NodeType], [Name], [Description], [GroupId], [ProjectStep], [ProjectCode], [ProjectStatus], [SCDUSASyncInd], [CreationDate], [LastUpdateTime], [LastUpdateUser], [LastUpdateComputer], [LastUpdateApplication]) VALUES (320073, 100046, 1, N'P', N'Project #1gtgggb', N'', NULL, NULL, NULL, N'O', NULL, CAST(0x0000A26400BFA904 AS DateTime), CAST(0x0000A26A00E2D12C AS DateTime), N'SysAdmin', N'ES-W520', N'Explorer')
INSERT [dbo].[ATS_Hierarchy] ([Id], [ParentId], [Sequence], [NodeType], [Name], [Description], [GroupId], [ProjectStep], [ProjectCode], [ProjectStatus], [SCDUSASyncInd], [CreationDate], [LastUpdateTime], [LastUpdateUser], [LastUpdateComputer], [LastUpdateApplication]) VALUES (320074, 100046, 1, N'P', N'Project #1gtgkgb', N'', NULL, NULL, NULL, N'O', NULL, CAST(0x0000A26400BFA904 AS DateTime), CAST(0x0000A26A00E2D12C AS DateTime), N'SysAdmin', N'ES-W520', N'Explorer')
INSERT [dbo].[ATS_Hierarchy] ([Id], [ParentId], [Sequence], [NodeType], [Name], [Description], [GroupId], [ProjectStep], [ProjectCode], [ProjectStatus], [SCDUSASyncInd], [CreationDate], [LastUpdateTime], [LastUpdateUser], [LastUpdateComputer], [LastUpdateApplication]) VALUES (320075, 100046, 1, N'P', N'Project #1gtekgb', N'', NULL, NULL, NULL, N'O', NULL, CAST(0x0000A26400BFA904 AS DateTime), CAST(0x0000A26A00E2D12C AS DateTime), N'SysAdmin', N'ES-W520', N'Explorer')
INSERT [dbo].[ATS_Hierarchy] ([Id], [ParentId], [Sequence], [NodeType], [Name], [Description], [GroupId], [ProjectStep], [ProjectCode], [ProjectStatus], [SCDUSASyncInd], [CreationDate], [LastUpdateTime], [LastUpdateUser], [LastUpdateComputer], [LastUpdateApplication]) VALUES (320076, 101038, 1, N'P', N'Project #1gtekgb', N'', NULL, NULL, N'PPC34               ', N'O', NULL, CAST(0x0000A26400BFA904 AS DateTime), CAST(0x0000A26A00E2D12C AS DateTime), N'SysAdmin', N'ES-W520', N'Explorer')
INSERT [dbo].[ATS_Hierarchy] ([Id], [ParentId], [Sequence], [NodeType], [Name], [Description], [GroupId], [ProjectStep], [ProjectCode], [ProjectStatus], [SCDUSASyncInd], [CreationDate], [LastUpdateTime], [LastUpdateUser], [LastUpdateComputer], [LastUpdateApplication]) VALUES (320077, 101038, 1, N'P', N'Project #1g33', N'', NULL, N'Step1a              ', N'PPC34               ', N'O', NULL, CAST(0x0000A26400BFA904 AS DateTime), CAST(0x0000A26A00E2D12C AS DateTime), N'SysAdmin', N'ES-W520', N'Explorer')
INSERT [dbo].[ATS_Hierarchy] ([Id], [ParentId], [Sequence], [NodeType], [Name], [Description], [GroupId], [ProjectStep], [ProjectCode], [ProjectStatus], [SCDUSASyncInd], [CreationDate], [LastUpdateTime], [LastUpdateUser], [LastUpdateComputer], [LastUpdateApplication]) VALUES (320079, 101038, 1, N'P', N'Project #13213', N'', NULL, N'Step2               ', N'PPC34               ', N'O', NULL, CAST(0x0000A26400BFA904 AS DateTime), CAST(0x0000A26A00E2D12C AS DateTime), N'SysAdmin', N'ES-W520', N'Explorer')
INSERT [dbo].[ATS_Hierarchy] ([Id], [ParentId], [Sequence], [NodeType], [Name], [Description], [GroupId], [ProjectStep], [ProjectCode], [ProjectStatus], [SCDUSASyncInd], [CreationDate], [LastUpdateTime], [LastUpdateUser], [LastUpdateComputer], [LastUpdateApplication]) VALUES (320081, NULL, 1, N'G', N'Group225', N'Group225', NULL, NULL, NULL, NULL, NULL, CAST(0x0000A26400BFA904 AS DateTime), CAST(0x0000A26A00E2D12C AS DateTime), N'SysAdmin', N'ES-W520', N'Explorer')
INSERT [dbo].[ATS_Hierarchy] ([Id], [ParentId], [Sequence], [NodeType], [Name], [Description], [GroupId], [ProjectStep], [ProjectCode], [ProjectStatus], [SCDUSASyncInd], [CreationDate], [LastUpdateTime], [LastUpdateUser], [LastUpdateComputer], [LastUpdateApplication]) VALUES (320083, NULL, 1, N'G', N'Group226', N'Group225', NULL, NULL, NULL, NULL, NULL, CAST(0x0000A26400BFA904 AS DateTime), CAST(0x0000A26A00E2D12C AS DateTime), N'SysAdmin', N'ES-W520', N'Explorer')
INSERT [dbo].[ATS_Hierarchy] ([Id], [ParentId], [Sequence], [NodeType], [Name], [Description], [GroupId], [ProjectStep], [ProjectCode], [ProjectStatus], [SCDUSASyncInd], [CreationDate], [LastUpdateTime], [LastUpdateUser], [LastUpdateComputer], [LastUpdateApplication]) VALUES (320087, NULL, 1, N'F', N'F226', N'Folder225s', NULL, NULL, NULL, NULL, NULL, CAST(0x0000A26400BFA904 AS DateTime), CAST(0x0000A29700F51500 AS DateTime), N'SysAdmin', N'WST40010478', N'Explorer')
INSERT [dbo].[ATS_Hierarchy] ([Id], [ParentId], [Sequence], [NodeType], [Name], [Description], [GroupId], [ProjectStep], [ProjectCode], [ProjectStatus], [SCDUSASyncInd], [CreationDate], [LastUpdateTime], [LastUpdateUser], [LastUpdateComputer], [LastUpdateApplication]) VALUES (320089, NULL, 1, N'G', N'G226', N'Gr225', NULL, NULL, NULL, NULL, NULL, CAST(0x0000A26400BFA904 AS DateTime), CAST(0x0000A26A00E2D12C AS DateTime), N'SysAdmin', N'ES-W520', N'Explorer')
INSERT [dbo].[ATS_Hierarchy] ([Id], [ParentId], [Sequence], [NodeType], [Name], [Description], [GroupId], [ProjectStep], [ProjectCode], [ProjectStatus], [SCDUSASyncInd], [CreationDate], [LastUpdateTime], [LastUpdateUser], [LastUpdateComputer], [LastUpdateApplication]) VALUES (320092, 100043, 1, N'P', N'Project #1A', N'', NULL, N'Step1a              ', N'PP1665              ', N'O', NULL, CAST(0x0000A26400BFA904 AS DateTime), CAST(0x0000A26A00E2D12C AS DateTime), N'SysAdmin', N'ES-W520', N'Explorer')
INSERT [dbo].[ATS_Hierarchy] ([Id], [ParentId], [Sequence], [NodeType], [Name], [Description], [GroupId], [ProjectStep], [ProjectCode], [ProjectStatus], [SCDUSASyncInd], [CreationDate], [LastUpdateTime], [LastUpdateUser], [LastUpdateComputer], [LastUpdateApplication]) VALUES (320096, NULL, 1, N'F', N'F22', N'', NULL, NULL, NULL, NULL, NULL, CAST(0x0000A26400BFA904 AS DateTime), CAST(0x0000A26A00E2D12C AS DateTime), N'SysAdmin', N'ES-W520', N'Explorer')
INSERT [dbo].[ATS_Hierarchy] ([Id], [ParentId], [Sequence], [NodeType], [Name], [Description], [GroupId], [ProjectStep], [ProjectCode], [ProjectStatus], [SCDUSASyncInd], [CreationDate], [LastUpdateTime], [LastUpdateUser], [LastUpdateComputer], [LastUpdateApplication]) VALUES (320097, NULL, 1, N'G', N'F22', N'', NULL, NULL, NULL, NULL, NULL, CAST(0x0000A26400BFA904 AS DateTime), CAST(0x0000A26A00E2D12C AS DateTime), N'SysAdmin', N'ES-W520', N'Explorer')
INSERT [dbo].[ATS_Hierarchy] ([Id], [ParentId], [Sequence], [NodeType], [Name], [Description], [GroupId], [ProjectStep], [ProjectCode], [ProjectStatus], [SCDUSASyncInd], [CreationDate], [LastUpdateTime], [LastUpdateUser], [LastUpdateComputer], [LastUpdateApplication]) VALUES (320101, 120056, 1, N'F', N'Project #1A', N'', NULL, NULL, NULL, NULL, NULL, CAST(0x0000A26400BFA904 AS DateTime), CAST(0x0000A26A00E2D12C AS DateTime), N'SysAdmin', N'ES-W520', N'Explorer')
INSERT [dbo].[ATS_Hierarchy] ([Id], [ParentId], [Sequence], [NodeType], [Name], [Description], [GroupId], [ProjectStep], [ProjectCode], [ProjectStatus], [SCDUSASyncInd], [CreationDate], [LastUpdateTime], [LastUpdateUser], [LastUpdateComputer], [LastUpdateApplication]) VALUES (320104, 120056, 1, N'P', N'Project #1555A', N'', NULL, NULL, NULL, NULL, NULL, CAST(0x0000A26400BFA904 AS DateTime), CAST(0x0000A26A00E2D12C AS DateTime), N'SysAdmin', N'ES-W520', N'Explorer')
INSERT [dbo].[ATS_Hierarchy] ([Id], [ParentId], [Sequence], [NodeType], [Name], [Description], [GroupId], [ProjectStep], [ProjectCode], [ProjectStatus], [SCDUSASyncInd], [CreationDate], [LastUpdateTime], [LastUpdateUser], [LastUpdateComputer], [LastUpdateApplication]) VALUES (320107, NULL, 1, N'F', N'Project #1555A', N'', NULL, NULL, NULL, NULL, NULL, CAST(0x0000A26400BFA904 AS DateTime), CAST(0x0000A26A00E2D12C AS DateTime), N'SysAdmin', N'ES-W520', N'Explorer')
INSERT [dbo].[ATS_Hierarchy] ([Id], [ParentId], [Sequence], [NodeType], [Name], [Description], [GroupId], [ProjectStep], [ProjectCode], [ProjectStatus], [SCDUSASyncInd], [CreationDate], [LastUpdateTime], [LastUpdateUser], [LastUpdateComputer], [LastUpdateApplication]) VALUES (320109, NULL, 1, N'G', N'Project #1555A', N'', NULL, NULL, NULL, NULL, NULL, CAST(0x0000A26400BFA904 AS DateTime), CAST(0x0000A26A00E2D12C AS DateTime), N'SysAdmin', N'ES-W520', N'Explorer')
INSERT [dbo].[ATS_Hierarchy] ([Id], [ParentId], [Sequence], [NodeType], [Name], [Description], [GroupId], [ProjectStep], [ProjectCode], [ProjectStatus], [SCDUSASyncInd], [CreationDate], [LastUpdateTime], [LastUpdateUser], [LastUpdateComputer], [LastUpdateApplication]) VALUES (320113, 120056, 1, N'F', N'Project #5671A', N'', NULL, N'Step1a              ', N'PP1353              ', N'O', NULL, CAST(0x0000A26400BFA904 AS DateTime), CAST(0x0000A26A00E2D12C AS DateTime), N'SysAdmin', N'ES-W520', N'Explorer')
INSERT [dbo].[ATS_Hierarchy] ([Id], [ParentId], [Sequence], [NodeType], [Name], [Description], [GroupId], [ProjectStep], [ProjectCode], [ProjectStatus], [SCDUSASyncInd], [CreationDate], [LastUpdateTime], [LastUpdateUser], [LastUpdateComputer], [LastUpdateApplication]) VALUES (320115, NULL, 1, N'F', N'Project #5671Af', N'tyrtyr7h', NULL, N'Step1a              ', N'PP876               ', N'O', NULL, CAST(0x0000A26400BFA904 AS DateTime), CAST(0x0000A29700F4E9E0 AS DateTime), N'SysAdmin', N'WST40010478', N'Explorer')
INSERT [dbo].[ATS_Hierarchy] ([Id], [ParentId], [Sequence], [NodeType], [Name], [Description], [GroupId], [ProjectStep], [ProjectCode], [ProjectStatus], [SCDUSASyncInd], [CreationDate], [LastUpdateTime], [LastUpdateUser], [LastUpdateComputer], [LastUpdateApplication]) VALUES (320117, NULL, 1, N'G', N'Project #5671A', N'', NULL, NULL, NULL, NULL, NULL, CAST(0x0000A26400BFA904 AS DateTime), CAST(0x0000A26A00E2D12C AS DateTime), N'SysAdmin', N'ES-W520', N'Explorer')
INSERT [dbo].[ATS_Hierarchy] ([Id], [ParentId], [Sequence], [NodeType], [Name], [Description], [GroupId], [ProjectStep], [ProjectCode], [ProjectStatus], [SCDUSASyncInd], [CreationDate], [LastUpdateTime], [LastUpdateUser], [LastUpdateComputer], [LastUpdateApplication]) VALUES (320120, 100042, 1, N'P', N'Project #5671A', N'', NULL, N'Step1a              ', N'PP1                 ', N'O', NULL, CAST(0x0000A26400BFA904 AS DateTime), CAST(0x0000A26A00E2D12C AS DateTime), N'SysAdmin', N'ES-W520', N'Explorer')
INSERT [dbo].[ATS_Hierarchy] ([Id], [ParentId], [Sequence], [NodeType], [Name], [Description], [GroupId], [ProjectStep], [ProjectCode], [ProjectStatus], [SCDUSASyncInd], [CreationDate], [LastUpdateTime], [LastUpdateUser], [LastUpdateComputer], [LastUpdateApplication]) VALUES (320124, 100038, 10, N'F', N'New Folder', N'New Folder', NULL, NULL, NULL, NULL, NULL, CAST(0x0000A287012A4544 AS DateTime), CAST(0x0000A287012A4544 AS DateTime), N'SysAdmin', N'WST40010478', N'Explorer')
INSERT [dbo].[ATS_Hierarchy] ([Id], [ParentId], [Sequence], [NodeType], [Name], [Description], [GroupId], [ProjectStep], [ProjectCode], [ProjectStatus], [SCDUSASyncInd], [CreationDate], [LastUpdateTime], [LastUpdateUser], [LastUpdateComputer], [LastUpdateApplication]) VALUES (320126, 320096, 10, N'F', N'Test1a', N'Test 1a folder', NULL, NULL, NULL, NULL, NULL, CAST(0x0000A287012A8996 AS DateTime), CAST(0x0000A287012B877B AS DateTime), N'SysAdmin', N'WST40010478', N'Explorer')
INSERT [dbo].[ATS_Hierarchy] ([Id], [ParentId], [Sequence], [NodeType], [Name], [Description], [GroupId], [ProjectStep], [ProjectCode], [ProjectStatus], [SCDUSASyncInd], [CreationDate], [LastUpdateTime], [LastUpdateUser], [LastUpdateComputer], [LastUpdateApplication]) VALUES (320127, 320115, 10, N'F', N'fff', N'New fdg', NULL, NULL, NULL, NULL, NULL, CAST(0x0000A287012A9036 AS DateTime), CAST(0x0000A29700F4CF9E AS DateTime), N'SysAdmin', N'WST40010478', N'Explorer')
INSERT [dbo].[ATS_Hierarchy] ([Id], [ParentId], [Sequence], [NodeType], [Name], [Description], [GroupId], [ProjectStep], [ProjectCode], [ProjectStatus], [SCDUSASyncInd], [CreationDate], [LastUpdateTime], [LastUpdateUser], [LastUpdateComputer], [LastUpdateApplication]) VALUES (320128, 101038, 10, N'F', N'New Folder', N'New Folder', NULL, NULL, NULL, NULL, NULL, CAST(0x0000A287012A9536 AS DateTime), CAST(0x0000A287012A9536 AS DateTime), N'SysAdmin', N'WST40010478', N'Explorer')
INSERT [dbo].[ATS_Hierarchy] ([Id], [ParentId], [Sequence], [NodeType], [Name], [Description], [GroupId], [ProjectStep], [ProjectCode], [ProjectStatus], [SCDUSASyncInd], [CreationDate], [LastUpdateTime], [LastUpdateUser], [LastUpdateComputer], [LastUpdateApplication]) VALUES (320129, 320126, 10, N'F', N'New Folder', N'New Folder', NULL, NULL, NULL, NULL, NULL, CAST(0x0000A287012ABEDC AS DateTime), CAST(0x0000A287012ABEDC AS DateTime), N'SysAdmin', N'WST40010478', N'Explorer')
INSERT [dbo].[ATS_Hierarchy] ([Id], [ParentId], [Sequence], [NodeType], [Name], [Description], [GroupId], [ProjectStep], [ProjectCode], [ProjectStatus], [SCDUSASyncInd], [CreationDate], [LastUpdateTime], [LastUpdateUser], [LastUpdateComputer], [LastUpdateApplication]) VALUES (320130, 320096, 10, N'F', N'New Folder', N'New Folderd', NULL, NULL, NULL, NULL, NULL, CAST(0x0000A28701371EE8 AS DateTime), CAST(0x0000A297010F7928 AS DateTime), N'SysAdmin', N'WST40010478', N'Explorer')
INSERT [dbo].[ATS_Hierarchy] ([Id], [ParentId], [Sequence], [NodeType], [Name], [Description], [GroupId], [ProjectStep], [ProjectCode], [ProjectStatus], [SCDUSASyncInd], [CreationDate], [LastUpdateTime], [LastUpdateUser], [LastUpdateComputer], [LastUpdateApplication]) VALUES (320131, 100039, 10, N'P', N'E229', N'New Project E229', NULL, N'Step2               ', N'EE44                ', N'O', 1, CAST(0x0000A28800C1F86C AS DateTime), CAST(0x0000A28800C29CED AS DateTime), N'SysAdmin', N'WST40010478', N'Explorer')
INSERT [dbo].[ATS_Hierarchy] ([Id], [ParentId], [Sequence], [NodeType], [Name], [Description], [GroupId], [ProjectStep], [ProjectCode], [ProjectStatus], [SCDUSASyncInd], [CreationDate], [LastUpdateTime], [LastUpdateUser], [LastUpdateComputer], [LastUpdateApplication]) VALUES (320132, 100039, 10, N'F', N'Hello', N'New folder for demo', NULL, NULL, NULL, NULL, NULL, CAST(0x0000A28E01851E68 AS DateTime), CAST(0x0000A28E01855B4A AS DateTime), N'SysAdmin', N'WST40010478', N'Explorer')
INSERT [dbo].[ATS_Hierarchy] ([Id], [ParentId], [Sequence], [NodeType], [Name], [Description], [GroupId], [ProjectStep], [ProjectCode], [ProjectStatus], [SCDUSASyncInd], [CreationDate], [LastUpdateTime], [LastUpdateUser], [LastUpdateComputer], [LastUpdateApplication]) VALUES (320133, 320087, 10, N'F', N'Hello.Hello.Hello.Hello.Hello.', N'Hello.Hello.Hello.Hello.Hello.Hello.Hello.Hello.Hello.Hello.Hello.Hello.Hello.Hello.Hello.Hello.dfs', NULL, NULL, NULL, NULL, NULL, CAST(0x0000A29700F33E6B AS DateTime), CAST(0x0000A29700F54E5C AS DateTime), N'SysAdmin', N'WST40010478', N'Explorer')
INSERT [dbo].[ATS_Hierarchy] ([Id], [ParentId], [Sequence], [NodeType], [Name], [Description], [GroupId], [ProjectStep], [ProjectCode], [ProjectStatus], [SCDUSASyncInd], [CreationDate], [LastUpdateTime], [LastUpdateUser], [LastUpdateComputer], [LastUpdateApplication]) VALUES (320134, 320096, 10, N'F', N'New Folderdfg', N'New Folderdfgdfd', NULL, NULL, NULL, NULL, NULL, CAST(0x0000A297010F097D AS DateTime), CAST(0x0000A297010F1247 AS DateTime), N'SysAdmin', N'WST40010478', N'Explorer')
SET IDENTITY_INSERT [dbo].[ATS_Hierarchy] OFF
/****** Object:  Table [dbo].[ATS_Note]    Script Date: 12/19/2013 12:38:36 ******/
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
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
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
SET IDENTITY_INSERT [dbo].[ATS_Note] ON
INSERT [dbo].[ATS_Note] ([Id], [HierarchyId], [NoteType], [NoteStatus], [NoteTitle], [NoteText], [SpecialInd], [CreatedByUser], [CreationDate], [LastUpdateTime], [LastUpdateUser], [LastUpdateComputer], [LastUpdateapplication]) VALUES (100000, 101038, N'C ', N'A', N'Hi', N'NNNddd dddd kkkkkkkkNNNddd dddd kkkkkkkkNNNddd dddd kkkkkkkkNNNddd dddd kkkkkkkkNNNddd dddd kkkkkkkkNNNddd dddd kkkkkkkkNNNddd dddd kkkkkkkkNNNddd dddd kkkkkkkkNNNddd dddd kkkkkkkkNNNddd dddd kkkkkkkkNNNddd dddd kkkkkkkkNNNddd dddd kkkkkkkkNNNddd dddd kkkkkkkkNNNddd dddd kkkkkkkkNNNddd dddd kkkkkkkkNNNddd dddd kkkkkkkkNNNddd dddd kkkkkkkkNNNddd dddd kkkkkkkkNNNddd dddd kkkkkkkkNNNddd dddd kkkkkkkkNNNddd dddd kkkkkkkkNNNddd dddd kkkkkkkkNNNddd dddd kkkkkkkkNNNddd dddd kkkkkkkkNNNddd dddd kkkkkkkkNNNddd dddd kkkkkkkkNNNddd dddd kkkkkkkkNNNddd dddd kkkkkkkkNNNddd dddd kkkkkkkkNNNddd dddd kkkkkkkkNNNddd dddd kkkkkkkkNNNddd dddd kkkkkkkkNNNddd dddd kkkkkkkkNNNddd dddd kkkkkkkkNNNddd dddd kkkkkkkkNNNddd dddd kkkkkkkkNNNddd dddd kkkkkkkkNNNddd dddd kkkkkkkkNNNddd dddd kkkkkkkkNNNddd dddd kkkkkkkkNNNddd dddd kkkkkkkkNNNddd dddd kkkkkkkkNNNddd dddd kkkkkkkkNNNddd dddd kkkkkkkkNNNddd dddd kkkkkkkkNNNddd dddd kkkkkkkkNNNddd dddd kkkkkkkkNNNddd dddd kkkkkkkkNNNddd dddd kkkkkkkkNNNddd dddd kkkkkkkkNNNddd dddd kkkkkkkkNNNddd dddd kkkkkkkkNNNddd dddd kkkkkkkkNNNddd dddd kkkkkkkkNNNddd dddd kkkkkkkkNNNddd dddd kkkkkkkkNNNddd dddd kkkkkkkkNNNddd dddd kkkkkkkkNNNddd dddd kkkkkkkkNNNddd dddd kkkkkkkkNNNddd dddd kkkkkkkkNNNddd dddd kkkkkkkkNNNddd dddd kkkkkkkkNNNddd dddd kkkkkkkkNNNddd dddd kkkkkkkkNNNddd dddd kkkkkkkkNNNddd dddd kkkkkkkkNNNddd dddd kkkkkkkkNNNddd dddd kkkkkkkkNNNddd dddd kkkkkkkkNNNddd dddd kkkkkkkkNNNddd dddd kkkkkkkkNNNddd dddd kkkkkkkkNNNddd dddd kkkkkkkkNNNddd dddd kkkkkkkkNNNddd dddd kkkkkkkkNNNddd dddd kkkkkkkkNNNddd dddd kkkkkkkkNNNddd dddd kkkkkkkkNNNddd dddd kkkkkkkk', 1, N'Me', CAST(0x0000A28E017E98AF AS DateTime), CAST(0x0000A28E017F909A AS DateTime), N'SysAdmin', N'WST40010478', N'Explorer')
INSERT [dbo].[ATS_Note] ([Id], [HierarchyId], [NoteType], [NoteStatus], [NoteTitle], [NoteText], [SpecialInd], [CreatedByUser], [CreationDate], [LastUpdateTime], [LastUpdateUser], [LastUpdateComputer], [LastUpdateapplication]) VALUES (100001, 101038, N'C ', N'A', N'Hello', N'Nfffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffff', 1, N'Me', CAST(0x0000A28E017F0915 AS DateTime), CAST(0x0000A28E017F3F4C AS DateTime), N'SysAdmin', N'WST40010478', N'Explorer')
INSERT [dbo].[ATS_Note] ([Id], [HierarchyId], [NoteType], [NoteStatus], [NoteTitle], [NoteText], [SpecialInd], [CreatedByUser], [CreationDate], [LastUpdateTime], [LastUpdateUser], [LastUpdateComputer], [LastUpdateapplication]) VALUES (100002, 320087, N'W ', N'D', N'Hi', N'kkksfdsiiefr', 1, N'SysAdmin', CAST(0x0000A29700EEA973 AS DateTime), CAST(0x0000A29700EF68AB AS DateTime), N'SysAdmin', N'WST40010478', N'Explorer')
INSERT [dbo].[ATS_Note] ([Id], [HierarchyId], [NoteType], [NoteStatus], [NoteTitle], [NoteText], [SpecialInd], [CreatedByUser], [CreationDate], [LastUpdateTime], [LastUpdateUser], [LastUpdateComputer], [LastUpdateapplication]) VALUES (100003, 320087, N'W ', N'D', N'Bye', N'Create New Folder', 1, N'SysAdmin', CAST(0x0000A29700EEC628 AS DateTime), CAST(0x0000A29701021E4C AS DateTime), N'SysAdmin', N'WST40010478', N'Explorer')
INSERT [dbo].[ATS_Note] ([Id], [HierarchyId], [NoteType], [NoteStatus], [NoteTitle], [NoteText], [SpecialInd], [CreatedByUser], [CreationDate], [LastUpdateTime], [LastUpdateUser], [LastUpdateComputer], [LastUpdateapplication]) VALUES (100004, 320087, N'C ', N'A', N'note note', N'Create New Folder
1.	Insert into Hierarchy, NodeType=’F’. GroupId, ProjectStep, ProjectCode, ProjectStatus are null.
Add content to folder
1.	Update Hierarchy set NodeType=’P’
2.	Insert into Version, HierarchyId from 1.
3.	Insert into VersionContent, VersionId from 2
Create new project
1.	Insert into Hierarchy, NodeType=’P’. GroupId is null.
2.	Insert into Version, HierarchyId from 1.
3.	Insert into VersionContent, VersionId from 2
Clone Project?Related, New group
1.	Insert into Hierarchy, NodeType=’G’; ParentId, Sequence, GroupId, ProjectCode and ProjectStatus are null; Name and Description are populated with Group Name and Group Description from GUI; ProjectStep is populated with parent projects value if any.
2.	Insert into Hierarchy, NodeType=’P’; GroupId from 1
3.	Update Hierarchy set GroupId from 1 for parent project.
4.	Update Version set HierarchyId=from 1 where HierarchyId=id of parent project (all versions will be linked to Group Node), set TargetPath to be identified with the group
5.	Update Note set HierarchyId=from 1 where HierarchyId=id of parent project (all notes will be linked to Group Node Id)
Clone Project?Related, Existing group
Insert into Hierarchy, NodeType=’P’, GroupId of the parent project
Update related project ?all related projects
1.	Version: 
Update/insert into Version, HierarchyId= GroupId of the project
2.	Note: 
Update/Insert into Note, HierarchyId= GroupId of the project
3.	Update Step
Update Hierarchy set ProjectStep where id= GroupId of the project
Update related project ?Separate project
1.	Update Hierarchy set ProjectStep=ProjectStep of the Group (or from GUI if updated)
2.	Update Hierarchy set GroupId null for separated project
3.	Versions:
Insert into Version , HierachyId =Id of the selected project, TargetPath=default, all other attributes are selected from Version where HierarchyId=Id of the Group (or from GUI if updated).
4.	Notes:
Insert into Note , HierachyId =I', 0, N'SysAdmin', CAST(0x0000A29700EFB211 AS DateTime), CAST(0x0000A29700EFCDB0 AS DateTime), N'SysAdmin', N'WST40010478', N'Explorer')
INSERT [dbo].[ATS_Note] ([Id], [HierarchyId], [NoteType], [NoteStatus], [NoteTitle], [NoteText], [SpecialInd], [CreatedByUser], [CreationDate], [LastUpdateTime], [LastUpdateUser], [LastUpdateComputer], [LastUpdateapplication]) VALUES (100006, 100038, N'C ', N'A', N'', N'', 1, N'SysAdmin', CAST(0x0000A29700F6173F AS DateTime), CAST(0x0000A29700FA4288 AS DateTime), N'SysAdmin', N'WST40010478', N'Explorer')
INSERT [dbo].[ATS_Note] ([Id], [HierarchyId], [NoteType], [NoteStatus], [NoteTitle], [NoteText], [SpecialInd], [CreatedByUser], [CreationDate], [LastUpdateTime], [LastUpdateUser], [LastUpdateComputer], [LastUpdateapplication]) VALUES (100008, 100038, N'W ', N'A', N'dfwe', N'werewrwrr', 1, N'SysAdmin', CAST(0x0000A29700F8B936 AS DateTime), CAST(0x0000A29700F8B936 AS DateTime), N'SysAdmin', N'WST40010478', N'Explorer')
SET IDENTITY_INSERT [dbo].[ATS_Note] OFF
/****** Object:  Table [dbo].[ATS_FolderCertificate]    Script Date: 12/19/2013 12:38:36 ******/
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
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
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
SET IDENTITY_INSERT [dbo].[ATS_FolderCertificate] ON
INSERT [dbo].[ATS_FolderCertificate] ([Rowid], [HierarchyId], [CertificateId], [EffectiveDate], [ExpirationDate], [LastUpdateTime], [LastUpdateUser], [LastUpdateComputer], [LastUpdateAppliation]) VALUES (100008, 320087, N'aaa', CAST(0x0000A28A010CCDB3 AS DateTime), NULL, CAST(0x0000A28A010CCDB3 AS DateTime), N'Me', N'My', N'App')
INSERT [dbo].[ATS_FolderCertificate] ([Rowid], [HierarchyId], [CertificateId], [EffectiveDate], [ExpirationDate], [LastUpdateTime], [LastUpdateUser], [LastUpdateComputer], [LastUpdateAppliation]) VALUES (100009, 320087, N'XGA', CAST(0x0000A28A010A3A0D AS DateTime), NULL, CAST(0x0000A28A010A3A0D AS DateTime), N'Me', N'My', N'App')
INSERT [dbo].[ATS_FolderCertificate] ([Rowid], [HierarchyId], [CertificateId], [EffectiveDate], [ExpirationDate], [LastUpdateTime], [LastUpdateUser], [LastUpdateComputer], [LastUpdateAppliation]) VALUES (100010, 320087, N'Pel', CAST(0x0000A28A010CCDB3 AS DateTime), NULL, CAST(0x0000A28A010CCDB3 AS DateTime), N'Me', N'My', N'App')
SET IDENTITY_INSERT [dbo].[ATS_FolderCertificate] OFF
/****** Object:  Table [dbo].[ATS_Version]    Script Date: 12/19/2013 12:38:36 ******/
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
	[DefaultTargetPathInd] [bit] NULL,
	[CreationDate] [datetime] NOT NULL,
	[LastUpdateTime] [datetime] NOT NULL,
	[LastUpdateUser] [nvarchar](50) NOT NULL,
	[LastUpdateComputer] [nvarchar](50) NOT NULL,
	[LastUpdateapplication] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_ProjectVersion] PRIMARY KEY CLUSTERED 
(
	[VersionId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY],
 CONSTRAINT [UQ_VersionName] UNIQUE NONCLUSTERED 
(
	[HierarchyId] ASC,
	[VersionName] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
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
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Indicates whether Target path is default (1) or user defined (0)' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ATS_Version', @level2type=N'COLUMN',@level2name=N'DefaultTargetPathInd'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Version Creation date' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ATS_Version', @level2type=N'COLUMN',@level2name=N'CreationDate'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Control fields' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ATS_Version', @level2type=N'COLUMN',@level2name=N'LastUpdateTime'
GO
SET IDENTITY_INSERT [dbo].[ATS_Version] ON
INSERT [dbo].[ATS_Version] ([VersionId], [HierarchyId], [VersionName], [VersionSeqNo], [VersionStatus], [Description], [TargetPath], [DefaultTargetPathInd], [CreationDate], [LastUpdateTime], [LastUpdateUser], [LastUpdateComputer], [LastUpdateapplication]) VALUES (100005, 150075, N'Version1', 1, N'C', N'Desc', N'C:\\Temp', NULL, CAST(0x0000A28100C32EB3 AS DateTime), CAST(0x0000A28100C32EB3 AS DateTime), N'Avi', N'comp', N'App')
INSERT [dbo].[ATS_Version] ([VersionId], [HierarchyId], [VersionName], [VersionSeqNo], [VersionStatus], [Description], [TargetPath], [DefaultTargetPathInd], [CreationDate], [LastUpdateTime], [LastUpdateUser], [LastUpdateComputer], [LastUpdateapplication]) VALUES (100007, 150075, N'Version2', 2, N'C', N'Desc', N'C:\\Temp', NULL, CAST(0x0000A28100C36051 AS DateTime), CAST(0x0000A28100C36051 AS DateTime), N'Avi', N'comp', N'App')
INSERT [dbo].[ATS_Version] ([VersionId], [HierarchyId], [VersionName], [VersionSeqNo], [VersionStatus], [Description], [TargetPath], [DefaultTargetPathInd], [CreationDate], [LastUpdateTime], [LastUpdateUser], [LastUpdateComputer], [LastUpdateapplication]) VALUES (100009, 150075, N'Version3', 3, N'A', N'Desc', N'C:\Temp\Projects\Project1555', 0, CAST(0x0000A28100C3785A AS DateTime), CAST(0x0000A29900C61A00 AS DateTime), N'SysAdmin', N'WST40010478', N'Explorer')
INSERT [dbo].[ATS_Version] ([VersionId], [HierarchyId], [VersionName], [VersionSeqNo], [VersionStatus], [Description], [TargetPath], [DefaultTargetPathInd], [CreationDate], [LastUpdateTime], [LastUpdateUser], [LastUpdateComputer], [LastUpdateapplication]) VALUES (100013, 150077, N'Version55', 2, N'A', N'Desc', N'C:\\Temp', NULL, CAST(0x0000A28100CD842E AS DateTime), CAST(0x0000A28100CD842E AS DateTime), N'Avi', N'comp', N'App')
INSERT [dbo].[ATS_Version] ([VersionId], [HierarchyId], [VersionName], [VersionSeqNo], [VersionStatus], [Description], [TargetPath], [DefaultTargetPathInd], [CreationDate], [LastUpdateTime], [LastUpdateUser], [LastUpdateComputer], [LastUpdateapplication]) VALUES (100015, 150077, N'Version6', 2, N'A', N'Desc', N'C:\\Temp', NULL, CAST(0x0000A28101839B1F AS DateTime), CAST(0x0000A28101839B1F AS DateTime), N'Avi', N'comp', N'App')
SET IDENTITY_INSERT [dbo].[ATS_Version] OFF
/****** Object:  Table [dbo].[ATS_VersionContent]    Script Date: 12/19/2013 12:38:36 ******/
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
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
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
SET IDENTITY_INSERT [dbo].[ATS_VersionContent] ON
INSERT [dbo].[ATS_VersionContent] ([Rowid], [VersionId], [ContentVersionId], [ContentSeqNo], [CreationDate], [LastUpdateTime], [LastUpdateUser], [LastUpdateComputer], [LastUpdateApplication]) VALUES (100000, 100009, 128, 1, CAST(0x0000A297010DC08D AS DateTime), CAST(0x0000A297010DC08D AS DateTime), N'Me', N'My', N'this')
INSERT [dbo].[ATS_VersionContent] ([Rowid], [VersionId], [ContentVersionId], [ContentSeqNo], [CreationDate], [LastUpdateTime], [LastUpdateUser], [LastUpdateComputer], [LastUpdateApplication]) VALUES (100001, 100009, 129, 2, CAST(0x0000A297010DC6A4 AS DateTime), CAST(0x0000A297010DC6A4 AS DateTime), N'Me', N'My', N'this')
SET IDENTITY_INSERT [dbo].[ATS_VersionContent] OFF
/****** Object:  ForeignKey [FK_ProfilePrivilege_Privilege]    Script Date: 12/19/2013 12:38:36 ******/
ALTER TABLE [dbo].[ATS_ProfilePrivilege]  WITH CHECK ADD  CONSTRAINT [FK_ProfilePrivilege_Privilege] FOREIGN KEY([PrivilegeCode])
REFERENCES [dbo].[ATS_Privilege] ([PrivilegeCode])
GO
ALTER TABLE [dbo].[ATS_ProfilePrivilege] CHECK CONSTRAINT [FK_ProfilePrivilege_Privilege]
GO
/****** Object:  ForeignKey [FK_ProfilePrivilege_Profile]    Script Date: 12/19/2013 12:38:36 ******/
ALTER TABLE [dbo].[ATS_ProfilePrivilege]  WITH NOCHECK ADD  CONSTRAINT [FK_ProfilePrivilege_Profile] FOREIGN KEY([ProfileId])
REFERENCES [dbo].[ATS_Profile] ([RowId])
GO
ALTER TABLE [dbo].[ATS_ProfilePrivilege] CHECK CONSTRAINT [FK_ProfilePrivilege_Profile]
GO
/****** Object:  ForeignKey [FK_UserProfile_Profile]    Script Date: 12/19/2013 12:38:36 ******/
ALTER TABLE [dbo].[ATS_UserProfile]  WITH NOCHECK ADD  CONSTRAINT [FK_UserProfile_Profile] FOREIGN KEY([ProfileId])
REFERENCES [dbo].[ATS_Profile] ([RowId])
GO
ALTER TABLE [dbo].[ATS_UserProfile] CHECK CONSTRAINT [FK_UserProfile_Profile]
GO
/****** Object:  ForeignKey [FK_Messages_MessageType]    Script Date: 12/19/2013 12:38:36 ******/
ALTER TABLE [dbo].[ATS_Messages]  WITH NOCHECK ADD  CONSTRAINT [FK_Messages_MessageType] FOREIGN KEY([Type])
REFERENCES [dbo].[ATS_MessageType] ([Type])
GO
ALTER TABLE [dbo].[ATS_Messages] CHECK CONSTRAINT [FK_Messages_MessageType]
GO
/****** Object:  ForeignKey [FK_Hierarchy_Hierarchy]    Script Date: 12/19/2013 12:38:36 ******/
ALTER TABLE [dbo].[ATS_Hierarchy]  WITH NOCHECK ADD  CONSTRAINT [FK_Hierarchy_Hierarchy] FOREIGN KEY([ParentId])
REFERENCES [dbo].[ATS_Hierarchy] ([Id])
GO
ALTER TABLE [dbo].[ATS_Hierarchy] CHECK CONSTRAINT [FK_Hierarchy_Hierarchy]
GO
/****** Object:  ForeignKey [FK_Hierarchy_Hierarchy1]    Script Date: 12/19/2013 12:38:36 ******/
ALTER TABLE [dbo].[ATS_Hierarchy]  WITH CHECK ADD  CONSTRAINT [FK_Hierarchy_Hierarchy1] FOREIGN KEY([GroupId])
REFERENCES [dbo].[ATS_Hierarchy] ([Id])
GO
ALTER TABLE [dbo].[ATS_Hierarchy] CHECK CONSTRAINT [FK_Hierarchy_Hierarchy1]
GO
/****** Object:  ForeignKey [ProjectStep_FK]    Script Date: 12/19/2013 12:38:36 ******/
ALTER TABLE [dbo].[ATS_Hierarchy]  WITH CHECK ADD  CONSTRAINT [ProjectStep_FK] FOREIGN KEY([ProjectStep])
REFERENCES [dbo].[ATS_ProjectStep] ([StepCode])
GO
ALTER TABLE [dbo].[ATS_Hierarchy] CHECK CONSTRAINT [ProjectStep_FK]
GO
/****** Object:  ForeignKey [FK_Note_Hierarchy]    Script Date: 12/19/2013 12:38:36 ******/
ALTER TABLE [dbo].[ATS_Note]  WITH NOCHECK ADD  CONSTRAINT [FK_Note_Hierarchy] FOREIGN KEY([HierarchyId])
REFERENCES [dbo].[ATS_Hierarchy] ([Id])
GO
ALTER TABLE [dbo].[ATS_Note] CHECK CONSTRAINT [FK_Note_Hierarchy]
GO
/****** Object:  ForeignKey [FK_Note_NoteType]    Script Date: 12/19/2013 12:38:36 ******/
ALTER TABLE [dbo].[ATS_Note]  WITH NOCHECK ADD  CONSTRAINT [FK_Note_NoteType] FOREIGN KEY([NoteType])
REFERENCES [dbo].[ATS_NoteType] ([NoteType])
GO
ALTER TABLE [dbo].[ATS_Note] CHECK CONSTRAINT [FK_Note_NoteType]
GO
/****** Object:  ForeignKey [FK_FolderCertificate_Hierarchy]    Script Date: 12/19/2013 12:38:36 ******/
ALTER TABLE [dbo].[ATS_FolderCertificate]  WITH NOCHECK ADD  CONSTRAINT [FK_FolderCertificate_Hierarchy] FOREIGN KEY([HierarchyId])
REFERENCES [dbo].[ATS_Hierarchy] ([Id])
GO
ALTER TABLE [dbo].[ATS_FolderCertificate] CHECK CONSTRAINT [FK_FolderCertificate_Hierarchy]
GO
/****** Object:  ForeignKey [FK_Version_Hierarchy]    Script Date: 12/19/2013 12:38:36 ******/
ALTER TABLE [dbo].[ATS_Version]  WITH NOCHECK ADD  CONSTRAINT [FK_Version_Hierarchy] FOREIGN KEY([HierarchyId])
REFERENCES [dbo].[ATS_Hierarchy] ([Id])
GO
ALTER TABLE [dbo].[ATS_Version] CHECK CONSTRAINT [FK_Version_Hierarchy]
GO
/****** Object:  ForeignKey [FK_VersionContent_GroupVersion]    Script Date: 12/19/2013 12:38:36 ******/
ALTER TABLE [dbo].[ATS_VersionContent]  WITH NOCHECK ADD  CONSTRAINT [FK_VersionContent_GroupVersion] FOREIGN KEY([VersionId])
REFERENCES [dbo].[ATS_Version] ([VersionId])
GO
ALTER TABLE [dbo].[ATS_VersionContent] CHECK CONSTRAINT [FK_VersionContent_GroupVersion]
GO
