# Project Structure

## Overview

This document defines the physical folder structure, project structure, and file organization standards for the Salon Booking System.

All developers and AI tools must follow this structure.

---

# Solution Structure

SalonBookingSystem.sln


├── SalonBookingSystem.API

├── SalonBookingSystem.Application

├── SalonBookingSystem.Domain

├── SalonBookingSystem.Infrastructure

├── SalonBookingSystem.Persistence

└── SalonBookingSystem.Web

tests

├── SalonBookingSystem.UnitTests

└── SalonBookingSystem.IntegrationTests

docs

---

# Domain Project

SalonBookingSystem.Domain

## Folder Structure

Domain

├── Common

├── Entities

├── Enums

├── Interfaces

└── ValueObjects

---

## Common

Contains:

* BaseEntity

---

## Entities

Contains:

* Customer
* Barber
* Service
* BarberService
* BarberSchedule
* Appointment
* AppointmentService

---

## Enums

Contains:

* AppointmentStatus

---

# Application Project

SalonBookingSystem.Application

## Folder Structure

Application

├── DTOs

├── Interfaces

├── Services

├── Validators

├── Mappings

└── Features

---

## DTOs

DTOs

├── Customer

├── Barber

├── Service

├── Appointment

---

Example:

Customer

├── CreateCustomerRequest
├── UpdateCustomerRequest
└── CustomerResponse

---

## Interfaces

Contains:

* ICustomerService
* IBarberService
* IServiceService
* IAppointmentService

---

## Services

Contains:

* CustomerService
* BarberService
* ServiceService
* AppointmentService

---

## Validators

Contains:

* CreateCustomerValidator
* CreateAppointmentValidator
* CreateServiceValidator

---

# Persistence Project

SalonBookingSystem.Persistence

## Folder Structure

Persistence

├── Context

├── Configurations

├── Repositories

├── Migrations

└── StoredProcedures

---

## Context

Contains:

* ApplicationDbContext

---

## Configurations

Contains:

* CustomerConfiguration
* BarberConfiguration
* ServiceConfiguration
* AppointmentConfiguration

---

## Repositories

Contains:

* CustomerRepository
* BarberRepository
* ServiceRepository
* AppointmentRepository

---

## StoredProcedures

Contains:

* sp_CreateAppointment
* sp_GetAvailableSlots
* sp_CancelAppointment
* sp_RescheduleAppointment

---

# Infrastructure Project

SalonBookingSystem.Infrastructure

## Folder Structure

Infrastructure

├── Identity

├── Services

├── Logging

└── DependencyInjection

---

## Identity

Contains:

* ApplicationUser
* Identity Configuration

---

## Services

Contains:

* CurrentUserService
* DateTimeProvider

---

# API Project

SalonBookingSystem.API

## Folder Structure

API

├── Controllers

├── Middleware

├── Filters

└── DependencyInjection

---

## Controllers

Contains:

* CustomersController
* BarbersController
* ServicesController
* AppointmentsController

---

## Middleware

Contains:

* GlobalExceptionMiddleware

---

# Web Project

SalonBookingSystem.Web

## Folder Structure

Web

├── Controllers

├── Views

├── ViewModels

├── wwwroot

└── Areas

---

## Views

Views

├── Customer

├── Barber

├── Service

├── Appointment

├── Shared

---

# Test Projects

## Unit Tests

SalonBookingSystem.UnitTests

Contains:

* Service Tests
* Validator Tests
* Business Rule Tests

---

## Integration Tests

SalonBookingSystem.IntegrationTests

Contains:

* Repository Tests
* API Tests
* Database Tests

---

# Dependency Rules

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

Domain shall not depend on any project.

---

# File Placement Rules

Entities:

Domain/Entities

DTOs:

Application/DTOs

Validators:

Application/Validators

Repositories:

Persistence/Repositories

EF Configurations:

Persistence/Configurations

Controllers:

API/Controllers

Views:

Web/Views

Tests:

UnitTests or IntegrationTests

Do not place files outside their designated folders.
