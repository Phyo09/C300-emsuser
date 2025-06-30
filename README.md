# C300 EMS User Management System

This project is a C# ASP.NET Core web application for managing EMS (Environmental Monitoring System) users, preferences, and data readings such as temperature, humidity, and more.

## ⚠️ Important Notice

**Login and email activation are no longer functional.**  
This is because the email notification feature relies on an SMTP server that is no longer in service.

As a result:
- New users cannot activate their accounts.
- Password reset via email is disabled.

You can still explore the codebase and understand the system structure and functionality.
The system is live and please use http://c300.azurewebsites.net/ to test out.
- Use "admin1@gmail.com" for Admin role with "password" as password
- Use "staff1@gmail.com" for Staff role with "password" as password
- Use "member1@gmail.com" for Member role with "password" as password

## Features

- User Registration and Authentication
- Preference Management for Sensors
- Environmental Data Logging
- Admin Dashboard
- Feedback and Forum Modules

## Tech Stack

- ASP.NET Core MVC
- Entity Framework Core
- SQL Server
- Bootstrap
- SMTP (decommissioned)

## Setup Instructions

> ⚠️ *Please do not include real SMTP credentials in `appsettings.json`. Use `appsettings.sample.json` as a template.*

1. Clone the repository
2. Set up your database connection string in `appsettings.json`
3. Restore NuGet packages
4. Run the project via Visual Studio or `dotnet run`

