# Student Admission System

The Student Admission System is a web-based application designed to streamline the student admission process for educational institutions.
The system enables students to register online and check their admission status while providing administrators with tools to manage applications efficiently.

## Features 

### Admin Feature
- Authentication
- View all student application
- Search students by first name, last name, application number and course
- Manage students application (Accept, Reject, Pending)
- Pagination for large dataset
- Create,view,update and delete student records
- 

### Student Feature
- Submit Admission application
- Check admission status using application number
- View personalized details

## Data and Storage
- Entity Framework core
- SQL Server Management for storing data

## Tech Stack
- Framework: ASP.NET Core MVC (C#)
- Database: SQL Server 
- Frontend: HTML5, CSS3, JavaScript, Bootstrap
- Authentication: ASP.NET Core Identity 

## Dependencies
- Microsoft.EntityFrameworkCore
- Microsoft.EntityFrameworkCore.SqlServer
- Microsoft.EntityFrameworkCore.Tools
- Microsoft.AspNetCore.Identity.EntityFrameworkCore
- X.PagedList

## Student Workflow
- Creates account on the website
- Log into the application using Email and password
- Register for admission by clicking on the registration form
- Submit personal details, academic information and also selects course of choice
- Recieve an automatically generated application number after successful submission
- Use the check status page to track their admission status using the application number

## Admin Workflow
- Log in to access the secure admin dashboard
- View all students application with pagination and search functionality
- Accept or reject student application using the action buttons in the admin dashboard
- Automatically updates the student's application status
- Manage students application record (View, update and delete student records)
- Create new student record

