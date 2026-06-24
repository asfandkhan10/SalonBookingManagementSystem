\# Project Rules



\## Documentation First



The documentation under the docs folder is the source of truth.



Always read and follow:



\* 00-Overview.md

\* 01-Functional-Requirements.md

\* 02-NonFunctional-Requirements.md

\* 03-Architecture.md

\* 04-Domain-Model.md

\* 05-Database-Design.md

\* 06-Backend-Specification.md

\* 07-Architecture-Resolution.md

\* 08-API-Contracts.md

\* 09-Coding-Standards.md

\* 10-Project-Structure.md



Do not invent requirements.



Do not modify architecture decisions without approval.



\---



\# Architecture Rules



Follow:



\* Clean Architecture

\* SOLID Principles

\* Separation of Concerns

\* Repository Pattern

\* Service Layer Pattern

\* Dependency Injection



Business logic must never be placed in Controllers.



\---



\# Project Structure



SalonBookingSystem



├── SalonBookingSystem.API



├── SalonBookingSystem.Application



├── SalonBookingSystem.Domain



├── SalonBookingSystem.Infrastructure



├── SalonBookingSystem.Persistence



└── SalonBookingSystem.Web



Do not create additional projects unless approved.



\---



\# Dependency Rules



Allowed Dependencies:



Presentation

↓

Application

↓

Domain



Infrastructure

↓

Application



Persistence

↓

Application



Domain



No Dependencies



Do not violate dependency direction.



\---



\# Dependency Injection Rules



Use Constructor Injection only.



Do not use:



\* Service Locator Pattern

\* Manual Dependency Creation

\* Static Service Access



Good:



CustomerService(ICustomerRepository repository)



Bad:



new CustomerRepository()



\---



\# Domain Rules



Domain Layer shall contain:



\* Entities

\* Enums

\* Domain Interfaces

\* Domain Rules



Domain shall never reference:



\* Entity Framework Core

\* ASP.NET Core

\* Infrastructure Libraries

\* SQL Server Libraries



\---



\# Entity Rules



All entities shall inherit from BaseEntity.



BaseEntity contains:



\* Id (int)

\* CreatedAt (DateTime)

\* CreatedBy (int)

\* UpdatedAt (DateTime?)

\* UpdatedBy (int?)

\* IsDeleted (bool)



Primary Key Type:



\* int



Identity:



\* IDENTITY(1,1)



\---



\# Audit Rules



Audit field types:



\* CreatedBy — int

\* UpdatedBy — int?



On Create:



\* CreatedAt

\* CreatedBy



On Update:



\* UpdatedAt

\* UpdatedBy



On Soft Delete:



\* IsDeleted = true

\* UpdatedAt updated

\* UpdatedBy updated



Records shall never be physically deleted.



\---



\# Database Rules



Database:



\* SQL Server



ORM:



\* Entity Framework Core



Use:



\* Fluent API Configurations

\* Migrations



Stored Procedures shall be used where specified.



Examples:



\* sp\_CreateAppointment

\* sp\_GetAvailableSlots

\* sp\_CancelAppointment

\* sp\_RescheduleAppointment



\---



\# Repository Rules



Repositories shall contain:



\* Data Access Logic

\* Query Logic

\* Stored Procedure Calls



Repositories shall not contain:



\* Business Logic

\* Validation Logic



Examples:



\* ICustomerRepository

\* IAppointmentRepository



\---



\# Service Rules



Services shall contain:



\* Business Rules

\* Validation Workflow

\* Application Logic



Examples:



\* CustomerService

\* AppointmentService

\* BarberService



Services shall not access UI components.



\---



\# DTO Rules



Entities shall never be exposed directly.



Use DTOs for:



\* Requests

\* Responses



Examples:



\* CreateCustomerRequest

\* UpdateCustomerRequest

\* CustomerResponse



\---



\# Validation Rules



Use FluentValidation.



Every Request DTO shall have a corresponding Validator.



Examples:



\* CreateCustomerValidator

\* CreateAppointmentValidator

\* CreateServiceValidator



\---



\# Controller Rules



Controllers shall:



\* Be Thin

\* Call Services Only

\* Return HTTP Responses



Controllers shall not:



\* Access DbContext

\* Access Repositories

\* Contain Business Logic



\---



\# API Rules



Follow REST conventions.



Examples:



GET /api/v1/customers



POST /api/v1/customers



PUT /api/v1/customers/{id}



DELETE /api/v1/customers/{id}



Use proper HTTP Status Codes.



\---



\# Async Rules



All database operations shall use:



\* async

\* await



Examples:



\* GetByIdAsync()

\* GetAllAsync()

\* CreateAsync()

\* UpdateAsync()



Avoid:



\* .Result

\* .Wait()



\---



\# Logging Rules



Use:



\* Serilog

\* ILogger



Log:



\* Errors

\* Warnings

\* Business Events



Do not log sensitive information.



\---



\# Security Rules



Use:



\* ASP.NET Core Identity

\* Cookie Authentication

\* Role-Based Authorization



Roles:



\* Administrator

\* Receptionist

\* Customer



\---



\# Naming Conventions



Classes:



PascalCase



Examples:



\* CustomerService

\* AppointmentRepository



Interfaces:



Prefix with I



Examples:



\* ICustomerRepository

\* IAppointmentService



Private Fields:



\_camelCase



Example:



\_customerRepository



Variables:



camelCase



Example:



customerId



\---



\# Testing Rules



Minimum Tests:



\* Unit Tests

\* Integration Tests



Test:



\* Services

\* Repositories

\* Business Rules



\---



\# AI Code Generation Rules



Before generating code:



\* Read documentation.

\* Follow architecture.

\* Follow coding standards.

\* Follow database design.



Generate code module-by-module.



Recommended Order:



1\. Authentication

2\. Customer

3\. Barber

4\. Service

5\. BarberService

6\. BarberSchedule

7\. Appointment

8\. Available Slots



Never generate the entire application in a single step.



Documentation is the source of truth.



