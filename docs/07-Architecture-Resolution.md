# Architecture Resolution Addendum

## Purpose

This document records the 12 documentation contradictions and gaps identified during analysis of the Salon Booking System documentation set, the decision approved for each, and the resulting rule going forward.

This document is an **addendum**, not a replacement. The original documents (00 through 10) remain the source of truth for everything not addressed here. Where a rule below narrows, clarifies, or extends an original document, this document takes precedence for that specific point only; everything else in the original document stands unchanged.

No code is generated or implied by this document. No existing document has been edited.

Status: **Approved** - all 12 resolutions approved as proposed.

---

## Resolution 1 — BarberServices Primary Key

**Issue**
05-Database-Design.md states that *all* tables use an `Id int IDENTITY(1,1)` primary key, but separately defines `BarberServices` with a composite primary key (`BarberId`, `ServiceId`) and no `Id` column. These two statements contradict each other.

**Approved Decision**
The universal `Id IDENTITY(1,1)` rule applies to entity tables. Pure many-to-many junction tables that carry no payload columns of their own (i.e. `BarberServices`) are an explicit, documented exception and keep their composite primary key.

**Reason**
A composite key on the two foreign keys is the correct and idiomatic design for a junction table with no independent identity or lifecycle. Forcing an unused surrogate `Id` onto it would add a meaningless column and a redundant unique constraint.

**Affected Documents**
05-Database-Design.md (Tables → BarberServices; Primary Key Strategy)

**Final Rule**
All entity tables use `Id int IDENTITY(1,1)` as primary key. Pure many-to-many junction tables without payload columns (currently: `BarberServices`) use a composite primary key of their two foreign keys instead, and are exempt from the universal `Id` rule.

---

## Resolution 2 — Audit / Soft Delete on Junction and Line-Item Tables

**Issue**
`BarberServices` and `AppointmentServices` have no audit columns and no `IsDeleted` column, even though 03-Architecture.md lists `BarberService` and `AppointmentService` as full "Business Entities," and 01-Functional-Requirements.md / 02-NonFunctional-Requirements.md require that *all* entities be audited and soft-deleted with no hard deletes. As schemed, "Remove Service From Barber" and "Remove Service From Appointment" would have to be hard deletes, violating the no-hard-delete rule.

**Approved Decision**
Two different rules, because the two tables play different roles:
- `BarberServices` (capability mapping, no financial/time data): remains without audit/soft-delete columns. Removing a mapping row is a physical delete. This is a documented exception to the universal soft-delete rule.
- `AppointmentServices` (a financial/duration line item snapshotted at booking time): gains `IsDeleted`, `UpdatedAt`, `UpdatedBy` columns. "Remove Service From Appointment" is a soft delete. All cost/duration calculations filter on `IsDeleted = false`.

**Reason**
`BarberServices` has no monetary or historical significance — it is current-state configuration ("can this barber currently perform this service"), not a business record. `AppointmentServices` is the opposite: it is part of the historical, auditable record of what a customer was booked and charged for, so it falls squarely under the no-hard-delete rule.

**Affected Documents**
05-Database-Design.md (Tables → BarberServices, AppointmentServices), 03-Architecture.md (Business Entities), 01-Functional-Requirements.md (Soft Delete Requirements), 04-Domain-Model.md (AppointmentService)

**Final Rule**
`BarberServices` is exempt from audit/soft-delete columns and may be physically deleted. `AppointmentServices` gains `IsDeleted`, `UpdatedAt`, `UpdatedBy` columns; "Remove Service From Appointment" is implemented as a soft delete, and all duration/cost calculations exclude rows where `IsDeleted = true`.

---

## Resolution 3 — Where Overlap-Checking and Calculation Logic Lives

**Issue**
04-Domain-Model.md (Rules 5–7) and the Service Layer pattern require duration calculation, cost calculation, and double-booking prevention to be business rules owned by the Application service layer. 05-Database-Design.md simultaneously requires `sp_CreateAppointment`, `sp_CancelAppointment`, and `sp_RescheduleAppointment` stored procedures for the same operations. Repositories are forbidden from containing business logic, creating a conflict over where the rule is actually enforced.

**Approved Decision**
- `AppointmentService` (Application layer) performs validation, duration/cost calculation, and the **primary** overlap check via a query, before calling the repository.
- The repository's only job is to invoke the stored procedure as its data-access mechanism — it contains no decision logic.
- The stored procedure may still include a defensive overlap check at the database level, but only as a **concurrency safety net** for near-simultaneous bookings, not as the documented home of the rule.

**Reason**
This keeps "Services shall contain business rules" and "Repositories shall not contain business logic" both true and non-contradictory, while still using the stored procedure for what stored procedures are good at: atomic writes and a last-line-of-defense integrity check against race conditions.

**Affected Documents**
03-Architecture.md (Service Layer, Repository Pattern), 04-Domain-Model.md (Rules 5–8), 05-Database-Design.md (Stored Procedures), 06-Backend-Specification.md (Repository Pattern, Service Layer)

**Final Rule**
Business rules (validation, duration/cost calculation, primary overlap check) are implemented in the Application service layer and run before any persistence call. Stored procedures perform the data write/read and may include a defensive, secondary overlap check purely as a concurrency safety net. Repositories invoke stored procedures only; they never branch on business outcomes.

---

## Resolution 4 — Customer ↔ ApplicationUser Relationship

**Issue**
No document defines a relationship between the `Customer` domain entity and the Identity `ApplicationUser`. Self-service booking requires resolving a logged-in user to their own `Customer` record, but no foreign key or link was specified anywhere.

**Approved Decision**
Add a nullable `ApplicationUserId` (string) column to `Customers`, unique-indexed, referencing `AspNetUsers.Id`. On Registration, the `AspNetUser` and the linked `Customer` row are created together in a single workflow.

**Reason**
This is the standard, minimal pattern for linking a richer business profile to an Identity login while keeping the two concerns (authentication vs. customer profile) in separate tables. Nullable because customers created directly by a Receptionist/Administrator may not have a self-service login.

**Affected Documents**
04-Domain-Model.md (Customer), 05-Database-Design.md (Customers table), 06-Backend-Specification.md (Authentication Module, Customer Module)

**Final Rule**
`Customer` gains a nullable `ApplicationUserId` (string) foreign key to `AspNetUsers.Id`, unique-indexed. Registration creates the `AspNetUser` and the linked `Customer` row in the same operation.

---

## Resolution 5 — Authentication Endpoints Missing from API Contracts

**Issue**
06-Backend-Specification.md requires Login, Logout, Register, Forgot Password, Reset Password, and Change Password as Authentication features, but 08-API-Contracts.md defines zero endpoints for any of them — no routes, request/response shapes, or status codes.

**Approved Decision**
Add explicit endpoint definitions following the existing API conventions: `POST /api/v1/auth/register`, `/login`, `/logout`, `/forgot-password`, `/reset-password`, `/change-password`, each returning the standard success/error envelope.

**Reason**
This is a direct documentation gap, not new scope — the capability is already required by 06-Backend-Specification.md; the API contract simply never caught up.

**Affected Documents**
08-API-Contracts.md (new Authentication Endpoints section)

**Final Rule**
08-API-Contracts.md is extended with versioned, envelope-returning endpoints for register, login, logout, forgot-password, reset-password, and change-password, consistent with the conventions already established for other modules.

---

## Resolution 6 — Forgot/Reset/Change Password Absent from Functional Requirements

**Issue**
06-Backend-Specification.md lists Forgot Password, Reset Password, and Change Password as Authentication features, but 01-Functional-Requirements.md only lists Login, Logout, and Registration. project-rules.md states "do not invent requirements," so this needed reconciliation rather than silent code generation.

**Approved Decision**
01-Functional-Requirements.md is extended with three corresponding requirement statements (Forgot Password, Reset Password, Change Password) under Authentication, to match what 06-Backend-Specification.md already specifies.

**Reason**
The capability is already documented elsewhere in the set; this is a synchronization fix between two documents describing the same module, not the introduction of new scope.

**Affected Documents**
01-Functional-Requirements.md (Authentication)

**Final Rule**
01-Functional-Requirements.md's Authentication section includes Forgot Password, Reset Password, and Change Password alongside Login, Logout, and Registration.

---

## Resolution 7 — AppointmentStatus.Confirmed Has No Triggering Operation

**Issue**
04-Domain-Model.md defines `AppointmentStatus` with a `Confirmed` value, but no Functional Requirement, Backend Specification operation, or API endpoint ever transitions an appointment into that state. There is no documented manual confirmation step or approval workflow.

**Approved Decision**
`CreateAppointmentAsync` sets the new appointment's status directly to `Confirmed`. `Pending` is not used in the current scope; it is reserved for a possible future workflow (e.g. payment-pending) and is not assigned automatically anywhere today.

**Reason**
No business rule anywhere in the documentation describes a manual approval step between creation and confirmation, so inventing one would violate "do not invent requirements." Setting status to `Confirmed` directly is the simplest behavior consistent with everything that is documented.

**Affected Documents**
04-Domain-Model.md (AppointmentStatus, Rule references), 06-Backend-Specification.md (Appointment Module operations)

**Final Rule**
Newly created appointments are assigned `AppointmentStatus.Confirmed` directly. `Pending` remains a defined enum value but is not produced by any current workflow. Introducing a manual confirmation step in the future requires a new documentation addendum.

---

## Resolution 8 — Barber `IsActive` vs. Soft Delete

**Issue**
`Barber` has both an `IsActive` business flag and an inherited `IsDeleted` soft-delete flag, but 01-Functional-Requirements.md never lists a "Soft Delete Barber" operation — only Activate/Deactivate appear in 06-Backend-Specification.md. It was unclear whether Barbers are ever soft-deleted at all, or only deactivated.

**Approved Decision**
Both concepts are kept, with distinct meaning:
- `IsActive` — a reversible, day-to-day toggle ("not taking bookings right now"), which also gates Domain Rule 2 ("a barber must be active before receiving appointments").
- `IsDeleted` — permanent removal of the barber from the roster (e.g. the barber has left the business).
"Soft Delete Barber" is added to 01-Functional-Requirements.md for symmetry with Customer and Service, and the existing `DELETE /api/v1/barbers/{id}` endpoint maps to this soft delete, separate from Activate/Deactivate.

**Reason**
The two flags answer different questions ("is this barber currently bookable" vs. "does this barber record still represent an active relationship with the business") and collapsing them into one would lose information needed by Domain Rule 2.

**Affected Documents**
01-Functional-Requirements.md (Barber Management), 06-Backend-Specification.md (Barber Module Operations), 08-API-Contracts.md (Barber Endpoints)

**Final Rule**
`Barber.IsActive` is a reversible operational toggle, distinct from `Barber.IsDeleted`. "Soft Delete Barber" is a defined Functional Requirement. `DELETE /api/v1/barbers/{id}` performs the soft delete; Activate/Deactivate is a separate action with its own endpoint.

---

## Resolution 9 — Barber Schedule Delete Missing from Functional Requirements

**Issue**
01-Functional-Requirements.md lists only Create, Update, and View for Barber Schedule Management, but 08-API-Contracts.md already defines `DELETE /api/v1/barbers/{barberId}/schedules/{scheduleId}`, and 06-Backend-Specification.md's CRUD pattern implies deletion is expected.

**Approved Decision**
Add "Delete Schedule" to 01-Functional-Requirements.md's Barber Schedule Management section.

**Reason**
This is a synchronization fix, not new scope — the capability is already implied by the existing API contract and backend specification; the functional requirements document simply omitted the explicit line item.

**Affected Documents**
01-Functional-Requirements.md (Barber Schedule Management)

**Final Rule**
01-Functional-Requirements.md's Barber Schedule Management section includes Create, Update, View, and Delete Schedule.

---

## Resolution 10 — Customer Search Missing from API Contract

**Issue**
01-Functional-Requirements.md requires "Search Customer," but 08-API-Contracts.md's `GET /api/v1/customers` only documents pagination parameters, with no search or filter query parameter defined.

**Approved Decision**
Add an optional `?search=` query parameter to `GET /api/v1/customers`, matching against FirstName, LastName, Email, and PhoneNumber, combinable with the existing pagination parameters.

**Reason**
Closes a direct gap between a stated functional requirement and its corresponding endpoint definition, using the same query-parameter style already established for pagination.

**Affected Documents**
08-API-Contracts.md (Customer Endpoints → Get Customers)

**Final Rule**
`GET /api/v1/customers` accepts an optional `search` query parameter that matches against FirstName, LastName, Email, and PhoneNumber, in addition to existing pagination parameters.

---

## Resolution 11 — Cookie Authentication with a Separate API Project

**Issue**
The solution has a standalone `SalonBookingSystem.API` project alongside `SalonBookingSystem.Web` (Razor), while authentication is specified throughout as cookie-based. The intended consumption topology between the two presentation projects was not documented.

**Approved Decision**
`SalonBookingSystem.Web` (Razor) references `Application`/`Domain` directly, in-process, for its own server-rendered pages. `SalonBookingSystem.API` is the headless REST surface for external/JS/mobile consumers. Both share the same authentication cookie since they are same-site; there is no HTTP hop from Web to API.

**Reason**
This avoids a self-referential HTTP call from Web to API, keeps both as parallel Presentation-layer projects per Clean Architecture (each depending only on Application/Domain), and matches the documented cookie-authentication model without requiring token-passing between the two front doors.

**Affected Documents**
03-Architecture.md (Layer Responsibilities → Presentation Layer, Dependency Flow)

**Final Rule**
`SalonBookingSystem.Web` and `SalonBookingSystem.API` are independent Presentation-layer projects, each depending only on Application/Domain. Web does not call API over HTTP; both authenticate via the same shared, same-site cookie.

---

## Resolution 12 — Directory.Packages.props Incompleteness

**Issue**
Directory.Packages.props pins only `Swashbuckle.AspNetCore`, while the documentation implies many other packages (EF Core, Identity, FluentValidation, Serilog, etc.) that are not yet version-pinned.

**Approved Decision**
Not treated as a contradiction. Package versions are added incrementally to Directory.Packages.props as each module's implementation requires them, keeping the central package management rule intact throughout.

**Reason**
Central package management only requires that *whatever is referenced* be versioned centrally — it does not require pre-declaring every package the whole solution will eventually need before any code exists.

**Affected Documents**
Directory.Packages.props (informational only — no document edit required by this resolution itself)

**Final Rule**
Directory.Packages.props is updated incrementally, in the same pull request/commit as the code that introduces each new package dependency, and never bypassed with per-project version numbers.

---

## Summary Table

| # | Topic | Resolution |
|---|-------|------------|
| 1 | BarberServices PK | Composite key exception for pure junction tables |
| 2 | Junction/line-item audit | BarberServices exempt; AppointmentServices soft-deletes |
| 3 | Overlap/calculation logic | Lives in Application services; stored procs are a safety net only |
| 4 | Customer ↔ ApplicationUser | Nullable `ApplicationUserId` FK on Customer |
| 5 | Auth endpoints | Added to 08-API-Contracts.md |
| 6 | Password ops in FR | Added to 01-Functional-Requirements.md |
| 7 | Confirmed status | Set directly on creation; no manual step |
| 8 | Barber IsActive vs delete | Both kept, distinct meaning |
| 9 | Schedule delete in FR | Added to 01-Functional-Requirements.md |
| 10 | Customer search in API | `?search=` query parameter added |
| 11 | Web/API topology | Both depend on Application directly; no HTTP hop |
| 12 | Package versions | Added incrementally per module |
