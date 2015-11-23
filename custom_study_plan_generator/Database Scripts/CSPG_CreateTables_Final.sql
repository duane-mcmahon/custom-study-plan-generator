USE [custom_study_plan_generator]
GO

/* *** DROP TABLES *** */
DROP TABLE [dbo].[Course]
DROP TABLE [dbo].[DefaultPlan]
DROP TABLE [dbo].[Student]
DROP TABLE [dbo].[StudentExemptions]
DROP TABLE [dbo].[StudentPlan]
DROP TABLE [dbo].[StudentPlanUnits]
DROP TABLE [dbo].[Unit]
DROP TABLE [dbo].[UnitPrerequisites]
DROP TABLE [dbo].[UnitType]

GO


/* *** CREATE TABLES *** */
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

/* Course Table */
CREATE TABLE [dbo].[Course](
	[course_code] [nvarchar](10) NOT NULL,
	[name] [nvarchar](50) NOT NULL,
	[num_units] [int] NOT NULL,
	[max_credit] [int] NOT NULL,
 CONSTRAINT [PK_Course_1] PRIMARY KEY CLUSTERED 
(
	[course_code] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO


/* Default Plan Table */
CREATE TABLE [dbo].[DefaultPlan](
	[course_code] [nvarchar](10) NOT NULL,
	[unit_no] [int] NOT NULL,
	[unit_code] [nvarchar](10) NOT NULL,
	[semester] [int] NOT NULL,
 CONSTRAINT [PK_DefaultPlan] PRIMARY KEY CLUSTERED 
(
	[course_code] ASC,
	[unit_no] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO


/* Student Table */
CREATE TABLE [dbo].[Student](
	[student_id] [int] NOT NULL,
	[firstname] [nvarchar](50) NOT NULL,
	[lastname] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_Student] PRIMARY KEY CLUSTERED 
(
	[student_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO


/* Student Exemption Table */
CREATE TABLE [dbo].[StudentExemptions](
	[student_id] [int] NOT NULL,
	[unit_code] [nvarchar](10) NOT NULL,
	[exempt] [bit] NOT NULL,
 CONSTRAINT [PK_StudentExemptions] PRIMARY KEY CLUSTERED 
(
	[student_id] ASC,
	[unit_code] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO


/* Student Plan Table */
CREATE TABLE [dbo].[StudentPlan](
	[plan_id] [int] NOT NULL,
	[student_id] [int] NOT NULL,
	[course_code] [nvarchar](10) NOT NULL,
	[start_semester] [int] NULL,
 CONSTRAINT [PK_StudentPlan] PRIMARY KEY CLUSTERED 
(
	[plan_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO


/* Student Plan Units Table */
CREATE TABLE [dbo].[StudentPlanUnits](
	[plan_id] [int] NOT NULL,
	[unit_no] [int] NOT NULL,
	[unit_code] [nvarchar](10) NOT NULL,
	[semester] [int] NOT NULL,
 CONSTRAINT [PK_StudentPlanUnits] PRIMARY KEY CLUSTERED 
(
	[plan_id] ASC,
	[unit_no] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO


/* Units Table */
CREATE TABLE [dbo].[Unit](
	[unit_code] [nvarchar](10) NOT NULL,
	[name] [nvarchar](80) NOT NULL,
	[type_code] [nvarchar](10) NOT NULL,
	[semester1] [bit] NOT NULL,
	[semester2] [bit] NOT NULL,
 CONSTRAINT [PK_Unit] PRIMARY KEY CLUSTERED 
(
	[unit_code] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO


/* Unit Prerequisites Table */
CREATE TABLE [dbo].[UnitPrerequisites](
	[course_code] [nvarchar](10) NOT NULL,
	[unit_code] [nvarchar](10) NOT NULL,
	[prereq_code] [nvarchar](10) NOT NULL,
	[mutiple_required] [bit] NOT NULL,
 CONSTRAINT [PK_UnitPrerequisites] PRIMARY KEY CLUSTERED 
(
	[course_code] ASC,
	[unit_code] ASC,
	[prereq_code] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO


/* Unit Type Table */
CREATE TABLE [dbo].[UnitType](
	[type_code] [nvarchar](10) NOT NULL,
	[Description] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_UnitType] PRIMARY KEY CLUSTERED 
(
	[type_code] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO



/* *** ADD FOREIGN KEYS *** */
ALTER TABLE [dbo].[DefaultPlan]  WITH CHECK ADD FOREIGN KEY([course_code])
REFERENCES [dbo].[Course] ([course_code])

ALTER TABLE [dbo].[DefaultPlan]  WITH CHECK ADD FOREIGN KEY([unit_code])
REFERENCES [dbo].[Unit] ([unit_code])

ALTER TABLE [dbo].[StudentExemptions]  WITH CHECK ADD FOREIGN KEY([student_id])
REFERENCES [dbo].[Student] ([student_id])

ALTER TABLE [dbo].[StudentExemptions]  WITH CHECK ADD FOREIGN KEY([unit_code])
REFERENCES [dbo].[Unit] ([unit_code])

ALTER TABLE [dbo].[StudentPlan]  WITH CHECK ADD FOREIGN KEY([course_code])
REFERENCES [dbo].[Course] ([course_code])

ALTER TABLE [dbo].[StudentPlan]  WITH CHECK ADD FOREIGN KEY([student_id])
REFERENCES [dbo].[Student] ([student_id])

ALTER TABLE [dbo].[StudentPlanUnits]  WITH CHECK ADD FOREIGN KEY([plan_id])
REFERENCES [dbo].[StudentPlan] ([plan_id])

ALTER TABLE [dbo].[StudentPlanUnits]  WITH CHECK ADD FOREIGN KEY([unit_code])
REFERENCES [dbo].[Unit] ([unit_code])

ALTER TABLE [dbo].[Unit]  WITH CHECK ADD FOREIGN KEY([type_code])
REFERENCES [dbo].[UnitType] ([type_code])

ALTER TABLE [dbo].[UnitPrerequisites]  WITH CHECK ADD FOREIGN KEY([course_code])
REFERENCES [dbo].[Course] ([course_code])

ALTER TABLE [dbo].[UnitPrerequisites]  WITH CHECK ADD FOREIGN KEY([unit_code])
REFERENCES [dbo].[Unit] ([unit_code])

ALTER TABLE [dbo].[UnitPrerequisites]  WITH CHECK ADD FOREIGN KEY([prereq_code])
REFERENCES [dbo].[Unit] ([unit_code])

GO

