# Software II Scheduling Application 

## Overview

This project is a desktop scheduling application developed in C# using Windows Forms and MySQL. The application allows users to manage customers and appointments while enforcing scheduling rules, business-hour constraints, and data validation requirements.

The system was designed to simulate a real-world appointment scheduling environment where users must authenticate, manage customer records, schedule appointments, generate reports, and work with time-sensitive data across multiple time zones.

## Technologies Used

* C#
* .NET Framework
* Windows Forms (WinForms)
* MySQL
* ADO.NET
* SQL
* Visual Studio

## Key Features

### Customer Management

* Create, view, update, and delete customer records
* Store customer information including address, city, country, and phone number
* Maintain relational database integrity across customer-related tables
* Validation and exception handling for invalid customer data

### Appointment Management

* Create, modify, and delete appointments
* Associate appointments with customers and users
* Store appointment details in a MySQL database
* Prevent invalid scheduling scenarios through business rule validation

### User Authentication

* Login screen with credential verification against the database
* Logging of successful and failed login attempts
* User activity tracking through an external log file

### Internationalization

* Automatic language detection using system culture settings
* English and Spanish localization support on the login screen
* Dynamic translation of login prompts and error messages

### Calendar Views

* View all appointments
* View appointments by week
* View appointments by month
* Select a specific calendar date to view appointments scheduled for that day

### Time Zone Management

* Store appointment data in UTC
* Convert appointment times to the user's local system time zone for display
* Handle daylight saving time automatically through .NET DateTime functionality

### Scheduling Rules

* Prevent appointments from being scheduled outside business hours
* Detect and prevent overlapping appointments
* Validate appointment start and end times
* Enforce scheduling constraints before database updates occur

### Reporting and Analytics

Generated reports include:

* Appointment counts by month and appointment type
* Consultant (user) schedules
* Customer appointment statistics

Reports are displayed directly within the application using DataGridView controls and LINQ-based data aggregation.

## Database Integration

The application uses MySQL as the backend data store and ADO.NET for database access.

Implemented functionality includes:

* Parameterized SQL queries
* CRUD operations
* Relational database design
* Foreign key management
* Data retrieval and updates through MySQL commands
* Protection against SQL injection through parameter usage

Core database entities include:

* User
* Customer
* Address
* City
* Country
* Appointment

## Screenshots

### Login Screen
![Login Screen](Screenshots/login.png)

### Calendar View
![Calendar View](Screenshots/calendar.png)

### Add Customer
![Add Customer](Screenshots/addcst.png)

### Add Appointment
![Add Appointment](Screenshots/addapt.png)

### Update Appointment
![Update Appointment](Screenshots/updateapt.png)

## Software Engineering Concepts Demonstrated

### Object-Oriented Programming

The project demonstrates:

* Encapsulation
* Abstraction
* Class-based design
* Separation of concerns
* Event-driven programming

### Application Architecture

The application separates responsibilities into:

* User Interface Forms
* Database Access Layer
* Business Logic Validation
* Data Models

Database operations were centralized into reusable helper methods to reduce code duplication and improve maintainability.

### Data Validation

Implemented validation includes:

* Required field validation
* Customer data validation
* Appointment time validation
* Business hour enforcement
* Appointment overlap detection
* Login credential validation

### Exception Handling

Robust exception handling was implemented throughout the application to:

* Handle database errors gracefully
* Prevent application crashes
* Provide meaningful feedback to users
* Maintain database integrity during failed operations

## Development Challenges

Several real-world software engineering challenges were addressed during development:

### Time Zone Conversion

Appointment times were stored in UTC and converted to local time zones for display. This required careful handling of DateTime conversions and daylight saving time behavior.

### Relational Database Constraints

Managing relationships between customers, addresses, cities, countries, and appointments required maintaining foreign key integrity and handling dependency-related deletion scenarios.

### Scheduling Validation

Appointment overlap detection required comparing proposed appointment times against existing database records while accounting for time zone conversion and business hour constraints.

### Internationalization

Supporting multiple languages required dynamically changing interface text and validation messages based on the operating system's culture settings.

## Skills Demonstrated

This project highlights experience with:

* C# Development
* Desktop Application Development
* Object-Oriented Programming
* SQL and Relational Databases
* ADO.NET
* MySQL Integration
* Data Validation
* Business Rule Enforcement
* Authentication Systems
* Internationalization and Localization
* Time Zone Handling
* Exception Handling
* Debugging and Troubleshooting
* CRUD Application Development
* Software Architecture Principles
* User Interface Design

## What Makes This Project Valuable

Beyond basic CRUD functionality, this project demonstrates the ability to build software that enforces business rules, integrates with a relational database, handles real-world scheduling constraints, supports multiple languages, and manages time-sensitive data across different time zones. These are common challenges encountered in enterprise software development and provide practical experience with designing maintainable business applications. 
