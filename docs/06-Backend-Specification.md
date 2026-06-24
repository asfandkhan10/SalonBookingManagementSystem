# Backend Specification

## Overview

This document defines the backend implementation requirements for the Salon Booking System.

The backend shall be implemented using:

* .NET 8
* ASP.NET Core MVC
* Entity Framework Core
* SQL Server
* ASP.NET Core Identity
* FluentValidation
* Clean Architecture

---

# Project Structure

SalonBookingSystem

├── SalonBookingSystem.API

├── SalonBookingSystem.Application

├── SalonBookingSystem.Domain

├── SalonBookingSystem.Infrastructure

├── SalonBookingSystem.Persistence

└── SalonBookingSystem.Web

---

# Authentication Module

## Features

* Login
* Logout
* Register
* Forgot Password
* Reset Password
* Change Password

## Identity

Use ASP.NET Core Identity.

## Roles

* Administrator
* Receptionist
* Customer

---

# Customer Module

## Entity

Customer

## DTOs

* CreateCustomerRequest
* UpdateCustomerRequest
* CustomerResponse

## Repository

* ICustomerRepository
* CustomerRepository

## Service

* ICustomerService
* CustomerService

## Validator

* CreateCustomerValidator
* UpdateCustomerValidator

## Operations

* Create Customer
* Update Customer
* Get Customer By Id
* Get Customers
* Soft Delete Customer

---

# Barber Module

## Entity

Barber

## DTOs

* CreateBarberRequest
* UpdateBarberRequest
* BarberResponse

## Repository

* IBarberRepository
* BarberRepository

## Service

* IBarberService
* BarberService

## Validator

* CreateBarberValidator
* UpdateBarberValidator

## Operations

* Create Barber
* Update Barber
* Get Barber By Id
* Get Barbers
* Activate Barber
* Deactivate Barber

---

# Service Module

## Entity

Service

## DTOs

* CreateServiceRequest
* UpdateServiceRequest
* ServiceResponse

## Repository

* IServiceRepository
* ServiceRepository

## Service Layer

* IServiceService
* ServiceService

## Validator

* CreateServiceValidator
* UpdateServiceValidator

## Operations

* Create Service
* Update Service
* Get Service By Id
* Get Services
* Soft Delete Service

---

# Barber Service Module

## Entity

BarberService

## Operations

* Assign Service To Barber
* Remove Service From Barber
* Get Barber Services

---

# Barber Schedule Module

## Entity

BarberSchedule

## DTOs

* CreateBarberScheduleRequest
* UpdateBarberScheduleRequest

## Operations

* Create Schedule
* Update Schedule
* Get Schedule
* Get Barber Schedule

---

# Appointment Module

## Entity

Appointment

## DTOs

* CreateAppointmentRequest
* UpdateAppointmentRequest
* AppointmentResponse

## Repository

* IAppointmentRepository
* AppointmentRepository

## Service

* IAppointmentService
* AppointmentService

## Validator

* CreateAppointmentValidator
* UpdateAppointmentValidator

## Operations

* Create Appointment
* View Appointment
* Cancel Appointment
* Reschedule Appointment
* Complete Appointment
* Get Appointment By Id
* Get Appointments

---

# Appointment Service Module

## Entity

AppointmentService

## Operations

* Add Service To Appointment
* Remove Service From Appointment
* Calculate Total Amount
* Calculate Total Duration

---

# Available Slot Module

## Operations

* Generate Available Slots
* Hide Reserved Slots
* Prevent Double Booking

## Business Rules

Available slots shall be generated using:

* Barber Schedule
* Existing Appointments
* Service Duration

---

# Repository Pattern

Repositories shall only perform:

* CRUD Operations
* Stored Procedure Calls
* Query Execution

Repositories shall not contain business logic.

---

# Service Layer

Services shall contain:

* Business Rules
* Validation Logic
* Workflow Logic

Controllers shall not contain business logic.

---

# Validation

FluentValidation shall be used.

Each request DTO shall have a corresponding validator.

Examples:

* CreateCustomerValidator
* CreateAppointmentValidator
* CreateServiceValidator

---

# Stored Procedures

## Appointment Procedures

* sp_CreateAppointment
* sp_CancelAppointment
* sp_RescheduleAppointment
* sp_GetAppointmentDetails

## Slot Procedures

* sp_GetAvailableSlots

## Customer Procedures

* sp_GetCustomerAppointments

## Barber Procedures

* sp_GetBarberSchedule

---

# Audit Requirements

All entities shall inherit from BaseEntity.

Audit Fields:

* Id
* CreatedAt
* CreatedBy
* UpdatedAt
* UpdatedBy
* IsDeleted

---

# Soft Delete

Records shall never be physically deleted.

When deleting:

* IsDeleted = true
* UpdatedAt updated
* UpdatedBy updated

---

# Logging

Use:

* Serilog
* Structured Logging

Log:

* Errors
* Warnings
* Business Events

---

# Error Handling

Use global exception handling.

Return standardized error responses.

---

# Async Programming

All database operations shall use:

* async
* await

Examples:

* GetByIdAsync
* GetAllAsync
* CreateAsync
* UpdateAsync

---

# Dependency Injection

All repositories and services shall be registered using ASP.NET Core Dependency Injection.

Example:

* ICustomerRepository → CustomerRepository
* ICustomerService → CustomerService
* IAppointmentRepository → AppointmentRepository
* IAppointmentService → AppointmentService
