\# Salon Booking System



\## Project Overview



The Salon Booking System is a web-based application designed to manage salon operations including customer management, barber management, service management, scheduling, and appointment booking.



The system enables customers to book appointments online while allowing salon staff to efficiently manage daily operations. The application ensures accurate scheduling, prevents double bookings, and maintains a complete audit trail of all business activities.



The application follows Clean Architecture, SOLID Principles, and industry-standard software engineering practices to ensure maintainability, scalability, and testability.



\---



\# Business Objectives



\* Manage Customers

\* Manage Barbers

\* Manage Services

\* Manage Barber Schedules

\* Manage Appointments

\* Prevent Double Bookings

\* Support Multiple Services Per Appointment

\* Support Customer Self-Service Booking

\* Support Appointment Cancellation and Rescheduling

\* Maintain Audit History

\* Support Soft Delete

\* Support Role-Based Security



\---



\# Target Users



\## Customer



\* Register

\* Login

\* Book Appointment

\* View Appointments

\* Cancel Appointment

\* Reschedule Appointment



\## Receptionist



\* Manage Customers

\* Create Appointments

\* Modify Appointments

\* Cancel Appointments

\* View Available Slots



\## Administrator



\* Manage Users

\* Manage Barbers

\* Manage Services

\* Manage Schedules

\* Manage Appointments



\---



\# Technology Stack



\## Backend



\* .NET 8

\* ASP.NET Core MVC

\* Entity Framework Core

\* SQL Server



\## Frontend



\* Razor Views

\* Bootstrap 5

\* JavaScript

\* jQuery



\## Authentication



\* ASP.NET Core Identity

\* Cookie Authentication



\---



\# Audit Strategy



All business entities shall contain:



\* CreatedAt

\* CreatedBy

\* UpdatedAt

\* UpdatedBy

\* IsDeleted



Soft delete shall be implemented using IsDeleted.



Records shall never be physically removed from the database.



\---



\# Architecture



The application follows Clean Architecture.



Layers:



\* Presentation Layer

\* Application Layer

\* Domain Layer

\* Infrastructure Layer

\* Persistence Layer



