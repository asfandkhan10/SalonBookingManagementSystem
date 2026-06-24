# API Contracts

## Overview

This document defines API standards, endpoint conventions, request contracts, response contracts, status codes, validation rules, security requirements, and versioning strategy for the Salon Booking System.

The API shall follow REST principles and industry-standard API design practices.

---

# API Design Principles

## RESTful Endpoints

Use HTTP verbs correctly.

### GET

Retrieve data.

Example:

GET /api/v1/customers

GET /api/v1/customers/{id}

---

### POST

Create data.

Example:

POST /api/v1/customers

---

### PUT

Update data.

Example:

PUT /api/v1/customers/{id}

---

### DELETE

Soft delete data.

Example:

DELETE /api/v1/customers/{id}

---

# Controller Guidelines

Controllers shall:

* Be thin
* Delegate business logic to services
* Return appropriate HTTP responses
* Never access DbContext directly
* Never contain business logic

Example Flow:

Controller

↓

Service

↓

Repository

↓

Database

---

# DTO Strategy

Entities shall never be exposed directly.

Use:

* Request DTOs
* Response DTOs

Examples:

* CreateCustomerRequest
* UpdateCustomerRequest
* CustomerResponse

---

# API Versioning

All APIs shall be versioned.

Example:

/api/v1/customers

/api/v1/barbers

/api/v1/services

/api/v1/appointments

---

# Standard Response Format

## Success Response

{
"success": true,
"message": "Operation completed successfully.",
"data": {}
}

---

## Error Response

{
"success": false,
"message": "Validation failed.",
"errors": []
}

---

# HTTP Status Codes

| Scenario         | Status Code               |
| ---------------- | ------------------------- |
| Success          | 200 OK                    |
| Created          | 201 Created               |
| Validation Error | 400 Bad Request           |
| Unauthorized     | 401 Unauthorized          |
| Forbidden        | 403 Forbidden             |
| Not Found        | 404 Not Found             |
| Server Error     | 500 Internal Server Error |

---

# Authentication

Authentication shall use:

* ASP.NET Core Identity
* Cookie Authentication

Protected endpoints require authentication.

---

# Authorization

Role-Based Authorization shall be used.

Roles:

* Administrator
* Receptionist
* Customer

Example:

Administrator only endpoints.

Receptionist management endpoints.

Customer self-service endpoints.

---

# Pagination

Large datasets shall support pagination.

Example:

GET /api/v1/customers?pageNumber=1&pageSize=10

Response:

{
"pageNumber": 1,
"pageSize": 10,
"totalRecords": 100,
"totalPages": 10,
"data": []
}

---

# Customer Endpoints

## Get Customers

GET /api/v1/customers

Response:

200 OK

---

## Get Customer By Id

GET /api/v1/customers/{id}

Response:

200 OK

404 Not Found

---

## Create Customer

POST /api/v1/customers

Request:

{
"firstName": "John",
"lastName": "Smith",
"email": "[john@email.com](mailto:john@email.com)",
"phoneNumber": "123456789"
}

Response:

201 Created

---

## Update Customer

PUT /api/v1/customers/{id}

Response:

200 OK

404 Not Found

---

## Delete Customer

DELETE /api/v1/customers/{id}

Response:

200 OK

Soft Delete Only

---

# Barber Endpoints

GET /api/v1/barbers

GET /api/v1/barbers/{id}

POST /api/v1/barbers

PUT /api/v1/barbers/{id}

DELETE /api/v1/barbers/{id}

---

# Service Endpoints

GET /api/v1/services

GET /api/v1/services/{id}

POST /api/v1/services

PUT /api/v1/services/{id}

DELETE /api/v1/services/{id}

---

# Barber Service Endpoints

Assign Service To Barber

POST /api/v1/barbers/{barberId}/services

Remove Service From Barber

DELETE /api/v1/barbers/{barberId}/services/{serviceId}

Get Barber Services

GET /api/v1/barbers/{barberId}/services

---

# Barber Schedule Endpoints

GET /api/v1/barbers/{barberId}/schedules

POST /api/v1/barbers/{barberId}/schedules

PUT /api/v1/barbers/{barberId}/schedules/{scheduleId}

DELETE /api/v1/barbers/{barberId}/schedules/{scheduleId}

---

# Appointment Endpoints

## Create Appointment

POST /api/v1/appointments

Response:

201 Created

---

## Get Appointment

GET /api/v1/appointments/{id}

---

## Get Appointments

GET /api/v1/appointments

---

## Cancel Appointment

POST /api/v1/appointments/{id}/cancel

---

## Reschedule Appointment

POST /api/v1/appointments/{id}/reschedule

---

## Complete Appointment

POST /api/v1/appointments/{id}/complete

---

# Available Slot Endpoints

## Get Available Slots

GET /api/v1/appointments/available-slots

Query Parameters:

* barberId
* appointmentDate
* durationMinutes

Example:

GET /api/v1/appointments/available-slots?barberId=1&appointmentDate=2026-07-01&durationMinutes=45

Response:

200 OK

[
"09:00",
"09:15",
"11:00",
"11:15"
]

---

# Validation

Use FluentValidation.

Invalid requests shall return:

400 Bad Request

---

# Logging

All API requests shall be logged using:

* Serilog
* Structured Logging

---

# Exception Handling

Global exception handling shall be used.

Controllers shall not contain try-catch blocks unless required.

---

# Async Programming

All API operations shall use:

* async
* await

Examples:

GetByIdAsync

GetAllAsync

CreateAsync

UpdateAsync

DeleteAsync
