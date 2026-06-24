# Database Design

## Overview

This document defines the database structure, relationships, constraints, indexing strategy, and stored procedure requirements for the Salon Booking System.

Database Provider:

* SQL Server

ORM:

* Entity Framework Core

Migration Strategy:

* Code First

Query Strategy:

* Entity Framework Core
* Stored Procedures for complex business operations and reporting

---

# Primary Key Strategy

All tables shall use:

* Id

Data Type:

* int

Identity:

* IDENTITY(1,1)

Primary Key:

* Yes

Auto Increment:

* Yes

---

# Audit Columns

All business tables shall contain:

* CreatedAt
* CreatedBy
* UpdatedAt
* UpdatedBy
* IsDeleted

---

# Soft Delete Strategy

Records shall never be physically deleted.

When deleting:

* IsDeleted = 1
* UpdatedAt updated
* UpdatedBy updated

---

# Tables

## Customers

Columns:

* Id
* FirstName
* LastName
* Email
* PhoneNumber
* CreatedAt
* CreatedBy
* UpdatedAt
* UpdatedBy
* IsDeleted

Indexes:

* Email
* PhoneNumber

---

## Barbers

Columns:

* Id
* FirstName
* LastName
* PhoneNumber
* IsActive
* CreatedAt
* CreatedBy
* UpdatedAt
* UpdatedBy
* IsDeleted

Indexes:

* IsActive

---

## Services

Columns:

* Id
* Name
* Description
* Price
* DurationMinutes
* CreatedAt
* CreatedBy
* UpdatedAt
* UpdatedBy
* IsDeleted

Indexes:

* Name

---

## BarberServices

Purpose:

Many-to-Many relationship between:

* Barber
* Service

Columns:

* BarberId
* ServiceId

Composite Primary Key:

* BarberId
* ServiceId

---

## BarberSchedules

Columns:

* Id
* BarberId
* DayOfWeek
* StartTime
* EndTime
* CreatedAt
* CreatedBy
* UpdatedAt
* UpdatedBy
* IsDeleted

Foreign Keys:

* BarberId → Barbers.Id

---

## Appointments

Columns:

* Id
* CustomerId
* BarberId
* AppointmentDate
* StartTime
* EndTime
* Status
* TotalAmount
* TotalDurationMinutes
* CreatedAt
* CreatedBy
* UpdatedAt
* UpdatedBy
* IsDeleted

Foreign Keys:

* CustomerId → Customers.Id
* BarberId → Barbers.Id

Indexes:

* AppointmentDate
* BarberId
* CustomerId

---

## AppointmentServices

Columns:

* Id
* AppointmentId
* ServiceId
* Price
* DurationMinutes

Foreign Keys:

* AppointmentId → Appointments.Id
* ServiceId → Services.Id

---

# Identity Tables

Managed by ASP.NET Core Identity:

* AspNetUsers
* AspNetRoles
* AspNetUserRoles
* AspNetUserClaims
* AspNetRoleClaims
* AspNetUserLogins
* AspNetUserTokens

---

# Relationships

Customer

1 → Many

Appointments

---

Barber

1 → Many

Appointments

---

Barber

1 → Many

BarberSchedules

---

Appointment

1 → Many

AppointmentServices

---

Service

1 → Many

AppointmentServices

---

Barber

Many ↔ Many

Service

Using:

BarberServices

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

# Join Strategy

Common joins:

## Appointment Listing

Appointments

JOIN Customers

JOIN Barbers

---

## Appointment Details

Appointments

JOIN Customers

JOIN Barbers

JOIN AppointmentServices

JOIN Services

---

## Barber Services

Barbers

JOIN BarberServices

JOIN Services

---

# Business Constraints

* A customer must exist before creating an appointment.
* A barber must be active before receiving appointments.
* A barber can only perform assigned services.
* An appointment must contain at least one service.
* Double booking is not allowed.
* Appointment duration is calculated automatically.
* Appointment end time is calculated automatically.

---

# Indexing Strategy

Create indexes for:

* Customer Email
* Customer PhoneNumber
* Appointment Date
* Barber Id
* Customer Id
* Service Name

---

# Migration Strategy

All schema changes shall be managed through Entity Framework Core migrations.

Direct database modifications are not permitted.
