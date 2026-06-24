# Coding Standards

## Purpose

These coding standards ensure code is:

* Readable
* Maintainable
* Testable
* Consistent
* Scalable

All developers and AI coding tools must follow these standards.

---

# Package Management

The solution uses Central Package Management.

File:

* Directory.Packages.props

Rules:

* Package versions shall be defined only in Directory.Packages.props.
* Do not specify package versions in .csproj files.
* Use latest stable versions unless otherwise specified.

---

# Build Configuration

The solution uses centralized build configuration.

File:

* Directory.Build.props

Rules:

* Common build settings must be maintained centrally.
* Enable Nullable Reference Types.
* Enable Implicit Usings.
* Avoid duplicate configuration across projects.

---

# Code Style Configuration

The solution uses centralized code style rules.

File:

* .editorconfig

Rules:

* Follow naming conventions.
* Follow formatting standards.
* Maintain consistent code style across the solution.

---

# Naming Conventions

## Classes

PascalCase

Examples:

* CustomerService
* AppointmentRepository

## Interfaces

Prefix with I

Examples:

* ICustomerRepository
* IAppointmentService

## Methods

PascalCase

Examples:

* GetCustomerByIdAsync
* CreateAppointmentAsync

## Variables

camelCase

Examples:

* customerId
* appointmentDate

## Private Fields

_camelCase

Examples:

* _customerRepository
* _logger

---

# Method Design

Methods shall:

* Have a single responsibility.
* Be small and readable.
* Be easy to test.
* Avoid excessive nesting.

---

# SOLID Principles

The application shall follow:

* Single Responsibility Principle
* Open Closed Principle
* Liskov Substitution Principle
* Interface Segregation Principle
* Dependency Inversion Principle

---

# DRY Principle

Do not duplicate code.

Reuse logic through:

* Services
* Extension Methods
* Shared Components

---

# Dependency Injection

Use Constructor Injection only.

Good:

CustomerService(ICustomerRepository repository)

Avoid:

new CustomerRepository()

Avoid:

Service Locator Pattern

---

# DTO Standards

Entities shall never be exposed directly.

Use:

* Request DTOs
* Response DTOs

Examples:

* CreateCustomerRequest
* UpdateCustomerRequest
* CustomerResponse

---

# Async Programming

Use async/await for all I/O operations.

Examples:

* GetByIdAsync()
* GetAllAsync()
* CreateAsync()

Avoid:

* .Result
* .Wait()

---

# Exception Handling

Use Global Exception Handling.

Log exceptions.

Do not swallow exceptions.

Use middleware for centralized exception handling.

---

# Logging

Use:

* Serilog
* ILogger

Log:

* Errors
* Warnings
* Business Events

Do not log:

* Passwords
* Sensitive Information

---

# Validation

Use FluentValidation.

Every Request DTO shall have a corresponding validator.

Examples:

* CreateCustomerValidator
* CreateAppointmentValidator

---

# Configuration Management

Do not hardcode:

* Connection Strings
* API Keys
* Secrets

Use:

* appsettings.json
* Environment Variables

---

# Clean Architecture Rules

Controllers:

* Thin Controllers
* No Business Logic
* No DbContext Access

Services:

* Business Logic
* Workflow Logic

Repositories:

* Data Access Logic
* Stored Procedure Execution

---

# Repository Standards

Repositories shall:

* Access the database
* Execute EF Core queries
* Execute Stored Procedures

Repositories shall not:

* Contain business rules
* Perform validation

---

# Service Layer Standards

Services shall:

* Implement business rules
* Coordinate repositories
* Execute workflows

Services shall not:

* Access UI components
* Access Views

---

# Entity Standards

All entities inherit from BaseEntity.

Properties:

* Id (int)
* CreatedAt (DateTime)
* CreatedBy (int)
* UpdatedAt (DateTime?)
* UpdatedBy (int?)
* IsDeleted (bool)

---

# Database Standards

Primary Keys:

* INT IDENTITY(1,1)

Tables:

* Use plural names

Examples:

* Customers
* Barbers
* Appointments

Stored Procedures:

Prefix:

* sp_

Examples:

* sp_CreateAppointment
* sp_GetAvailableSlots

---

# API Standards

Use REST conventions.

Examples:

GET /api/v1/customers

POST /api/v1/customers

PUT /api/v1/customers/{id}

DELETE /api/v1/customers/{id}

Use proper HTTP status codes.

---

# Security Standards

Use:

* ASP.NET Core Identity
* Cookie Authentication
* Role-Based Authorization

Roles:

* Administrator
* Receptionist
* Customer

Do not:

* Store plain text passwords
* Expose sensitive information

---

# Performance Standards

Use:

* AsNoTracking() for read-only queries
* Pagination
* Proper indexing
* Async database operations

Avoid:

* N+1 Queries
* Unnecessary data loading

---

# Code Formatting

Use:

* Proper indentation
* Consistent spacing
* Meaningful names

Follow .editorconfig.

---

# Comments

Comments shall explain WHY.

Avoid comments that explain WHAT.

---

# Testing Standards

Minimum Tests:

* Unit Tests
* Integration Tests

Test:

* Services
* Repositories
* Business Rules

---

# AI Development Rules

Cursor, Claude, GitHub Copilot, and ChatGPT must:

* Follow Clean Architecture
* Follow SOLID Principles
* Use Constructor Injection
* Use DTOs
* Use Async/Await
* Use FluentValidation
* Use Soft Delete
* Use Stored Procedures where specified
* Follow .editorconfig
* Follow Directory.Build.props
* Follow Directory.Packages.props
* Never place business logic in Controllers

Project documentation is the source of truth.
