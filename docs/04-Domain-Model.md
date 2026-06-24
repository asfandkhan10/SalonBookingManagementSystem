# Domain Model

## Overview

This document defines the domain entities, relationships, and business rules for the Salon Booking System.

All business entities inherit from BaseEntity.

---

# BaseEntity

## Properties

* Id
* CreatedAt
* CreatedBy
* UpdatedAt
* UpdatedBy
* IsDeleted

## Audit Rules

### Create

Populate:

* CreatedAt
* CreatedBy

### Update

Populate:

* UpdatedAt
* UpdatedBy

### Soft Delete

Populate:

* UpdatedAt
* UpdatedBy

Set:

* IsDeleted = true

Records shall never be physically deleted.

---

# Customer

Represents a salon customer.

## Properties

* Id
* FirstName
* LastName
* Email
* PhoneNumber

## Relationships

One Customer can have many Appointments.

---

# Barber

Represents a barber providing services.

## Properties

* Id
* FirstName
* LastName
* PhoneNumber
* IsActive

## Relationships

One Barber can have many Appointments.

One Barber can have many Schedules.

One Barber can provide many Services.

---

# Service

Represents a salon service.

## Properties

* Id
* Name
* Description
* Price
* DurationMinutes

## Examples

* Hair Cut
* Beard Trim
* Hair Wash
* Hair Color

## Relationships

One Service can be assigned to many Barbers.

One Service can belong to many Appointment Services.

---

# BarberService

Represents services assigned to a barber.

## Properties

* BarberId
* ServiceId

## Purpose

Many-to-Many relationship between:

* Barber
* Service

---

# BarberSchedule

Represents barber working hours.

## Properties

* Id
* BarberId
* DayOfWeek
* StartTime
* EndTime

## Example

Monday

09:00

18:00

## Relationships

One Barber can have many schedules.

---

# Appointment

Represents a booking.

## Properties

* Id
* CustomerId
* BarberId
* AppointmentDate
* StartTime
* EndTime
* Status
* TotalAmount
* TotalDurationMinutes

## Relationships

One Customer can have many Appointments.

One Barber can have many Appointments.

One Appointment can have many Appointment Services.

---

# AppointmentService

Represents services selected for an appointment.

## Properties

* Id
* AppointmentId
* ServiceId
* Price
* DurationMinutes

## Purpose

Stores service information at booking time.

Changes to service pricing must not affect historical appointments.

---

# ApplicationUser

Represents authenticated users.

## Roles

* Administrator
* Receptionist
* Customer

---

# AppointmentStatus

## Enum Values

* Pending
* Confirmed
* Completed
* Cancelled

---

# Business Rules

## Rule 1

A customer must exist before creating an appointment.

---

## Rule 2

A barber must be active before receiving appointments.

---

## Rule 3

A barber can only perform services assigned to them.

---

## Rule 4

An appointment must contain at least one service.

---

## Rule 5

Appointment duration shall be calculated automatically.

Example:

Hair Cut = 30 Minutes

Beard Trim = 15 Minutes

Total Duration = 45 Minutes

---

## Rule 6

Appointment end time shall be calculated automatically.

Example:

Start Time = 10:00

Duration = 45 Minutes

End Time = 10:45

---

## Rule 7

Double booking is not allowed.

Appointments for the same barber shall not overlap.

---

## Rule 8

Available slots shall be generated using:

* Barber Schedule
* Existing Appointments
* Service Duration

Reserved slots shall not be displayed.

---

# Relationship Summary

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

BarberService
