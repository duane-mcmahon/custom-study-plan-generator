USE [custom_study_plan_generator]
GO

/****** Object:  Database [custom_study_plan_generator]    Script Date: 23/11/2015 1:42:12 PM ******/
DROP DATABASE [custom_study_plan_generator]
GO

/****** Object:  Database [custom_study_plan_generator]    Script Date: 23/11/2015 1:42:12 PM ******/
CREATE DATABASE [custom_study_plan_generator]
 CONTAINMENT = NONE
 ON  PRIMARY 
( NAME = N'custom_study_plan_generator', FILENAME = N'c:\databases\custom_study_plan_generator.mdf' , SIZE = 3072KB , MAXSIZE = UNLIMITED, FILEGROWTH = 1024KB )
 LOG ON 
( NAME = N'custom_study_plan_generator_log', FILENAME = N'c:\databases\custom_study_plan_generator_log.ldf' , SIZE = 1024KB , MAXSIZE = 2048GB , FILEGROWTH = 10%)
GO

ALTER DATABASE [custom_study_plan_generator] SET COMPATIBILITY_LEVEL = 110
GO

IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [custom_study_plan_generator].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO

ALTER DATABASE [custom_study_plan_generator] SET ANSI_NULL_DEFAULT OFF 
GO

ALTER DATABASE [custom_study_plan_generator] SET ANSI_NULLS OFF 
GO

ALTER DATABASE [custom_study_plan_generator] SET ANSI_PADDING OFF 
GO

ALTER DATABASE [custom_study_plan_generator] SET ANSI_WARNINGS OFF 
GO

ALTER DATABASE [custom_study_plan_generator] SET ARITHABORT OFF 
GO

ALTER DATABASE [custom_study_plan_generator] SET AUTO_CLOSE OFF 
GO

ALTER DATABASE [custom_study_plan_generator] SET AUTO_CREATE_STATISTICS ON 
GO

ALTER DATABASE [custom_study_plan_generator] SET AUTO_SHRINK OFF 
GO

ALTER DATABASE [custom_study_plan_generator] SET AUTO_UPDATE_STATISTICS ON 
GO

ALTER DATABASE [custom_study_plan_generator] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO

ALTER DATABASE [custom_study_plan_generator] SET CURSOR_DEFAULT  GLOBAL 
GO

ALTER DATABASE [custom_study_plan_generator] SET CONCAT_NULL_YIELDS_NULL OFF 
GO

ALTER DATABASE [custom_study_plan_generator] SET NUMERIC_ROUNDABORT OFF 
GO

ALTER DATABASE [custom_study_plan_generator] SET QUOTED_IDENTIFIER OFF 
GO

ALTER DATABASE [custom_study_plan_generator] SET RECURSIVE_TRIGGERS OFF 
GO

ALTER DATABASE [custom_study_plan_generator] SET  DISABLE_BROKER 
GO

ALTER DATABASE [custom_study_plan_generator] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO

ALTER DATABASE [custom_study_plan_generator] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO

ALTER DATABASE [custom_study_plan_generator] SET TRUSTWORTHY OFF 
GO

ALTER DATABASE [custom_study_plan_generator] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO

ALTER DATABASE [custom_study_plan_generator] SET PARAMETERIZATION SIMPLE 
GO

ALTER DATABASE [custom_study_plan_generator] SET READ_COMMITTED_SNAPSHOT OFF 
GO

ALTER DATABASE [custom_study_plan_generator] SET HONOR_BROKER_PRIORITY OFF 
GO

ALTER DATABASE [custom_study_plan_generator] SET RECOVERY SIMPLE 
GO

ALTER DATABASE [custom_study_plan_generator] SET  MULTI_USER 
GO

ALTER DATABASE [custom_study_plan_generator] SET PAGE_VERIFY CHECKSUM  
GO

ALTER DATABASE [custom_study_plan_generator] SET DB_CHAINING OFF 
GO

ALTER DATABASE [custom_study_plan_generator] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO

ALTER DATABASE [custom_study_plan_generator] SET TARGET_RECOVERY_TIME = 0 SECONDS 
GO

ALTER DATABASE [custom_study_plan_generator] SET  READ_WRITE 
GO


