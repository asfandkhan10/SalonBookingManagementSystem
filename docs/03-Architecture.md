\# Architecture



\## Architecture Style



The application shall follow Clean Architecture.



The architecture must enforce separation of concerns, maintainability, testability, and scalability.



The application shall follow:



\* Clean Architecture

\* SOLID Principles

\* Repository Pattern

\* Service Layer Pattern

\* Dependency Injection

\* Entity Framework Core



\---



\# Solution Structure



SalonBookingSystem



├── SalonBookingSystem.API



├── SalonBookingSystem.Application



├── SalonBookingSystem.Domain



├── SalonBookingSystem.Infrastructure



├── SalonBookingSystem.Persistence



└── SalonBookingSystem.Web



\---



\# Layer Responsibilities



\## Domain Layer



Contains:



\* Entities

\* Enums

\* Domain Rules

\* Domain Interfaces

\* Common Classes



Must not depend on any other layer.



\---



\## Application Layer



Contains:



\* DTOs

\* Interfaces

\* Services

\* Validators

\* Business Logic



May depend only on Domain Layer.



\---



\## Infrastructure Layer



Contains:



\* External Services

\* Identity Services

\* Email Services

\* File Storage Services



May depend on Application and Domain.



\---



\## Persistence Layer



Contains:



\* DbContext

\* Entity Configurations

\* Repositories

\* Migrations



May depend on Application and Domain.



\---



\## Presentation Layer



Contains:



\* Controllers

\* Razor Views

\* ViewModels



Must not contain business logic.



\---



\# Domain Structure



Domain



├── Common

│

├── Entities

│

├── Enums

│

└── Interfaces



\---



\# Common



Contains:



\* BaseEntity



\---



\# BaseEntity



All business entities shall inherit from BaseEntity.



Properties:



\* Id

\* CreatedAt

\* CreatedBy

\* UpdatedAt

\* UpdatedBy

\* IsDeleted



\---



\# Business Entities



\* Customer

\* Barber

\* Service

\* BarberService

\* BarberSchedule

\* Appointment

\* AppointmentService



\---



\# Repository Pattern



Each aggregate shall have its own repository interface.



Examples:



\* ICustomerRepository

\* IBarberRepository

\* IServiceRepository

\* IAppointmentRepository



Repositories shall only handle data access.



Repositories shall not contain business logic.



\---



\# Service Layer



Each module shall have its own service.



Examples:



\* CustomerService

\* BarberService

\* AppointmentService



Services shall contain business rules and workflows.



\---



\# DTO Strategy



Entities shall never be exposed directly.



Use DTOs for:



\* Requests

\* Responses



Examples:



\* CreateCustomerRequest

\* UpdateCustomerRequest

\* CustomerResponse



\---



\# Validation



FluentValidation shall be used.



Examples:



\* CreateCustomerValidator

\* CreateAppointmentValidator



\---



\# Soft Delete Strategy



The application uses soft delete.



Records are never physically deleted.



When deleting:



\* IsDeleted = true

\* UpdatedAt updated

\* UpdatedBy updated



\---



\# Audit Strategy



When creating:



\* CreatedAt populated

\* CreatedBy populated



When updating:



\* UpdatedAt populated

\* UpdatedBy populated



When soft deleting:



\* UpdatedAt populated

\* UpdatedBy populated

\* IsDeleted = true



\---



\# Dependency Flow



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



Domain must never depend on any other layer.



