\# Functional Requirements



\## Authentication



\### Login



The system shall allow registered users to login.



\### Logout



The system shall allow authenticated users to logout.



\### Registration



The system shall allow customers to register an account.



\---



\# Customer Management



\### Create Customer



The system shall allow creating customers.



\### Update Customer



The system shall allow updating customer information.



\### Search Customer



The system shall allow searching customers.



\### Soft Delete Customer



The system shall support customer soft deletion.



\---



\# Barber Management



\### Create Barber



The system shall allow creating barbers.



\### Update Barber



The system shall allow updating barbers.



\### Assign Services



The system shall allow assigning services to barbers.



\### Assign Schedule



The system shall allow assigning schedules to barbers.



\---



\# Service Management



\### Create Service



The system shall allow creating services.



\### Update Service



The system shall allow updating services.



\### Soft Delete Service



The system shall support service soft deletion.



\---



\# Barber Schedule Management



\### Create Schedule



The system shall allow assigning working schedules.



\### Update Schedule



The system shall allow updating schedules.



\### View Schedule



The system shall allow viewing schedules.



\---



\# Appointment Management



\### Create Appointment



The system shall allow creating appointments.



\### View Appointment



The system shall allow viewing appointments.



\### Cancel Appointment



The system shall allow cancelling appointments.



\### Reschedule Appointment



The system shall allow rescheduling appointments.



\### Complete Appointment



The system shall allow marking appointments as completed.



\---



\# Appointment Service Management



\### Multiple Services



One appointment may contain multiple services.



\### Duration Calculation



Appointment duration shall be calculated automatically.



\### Cost Calculation



Appointment cost shall be calculated automatically.



\---



\# Available Slot Management



\### Generate Available Slots



The system shall generate available slots based on:



\* Barber Schedule

\* Existing Appointments

\* Service Duration



\### Hide Reserved Slots



Reserved slots shall not be displayed.



\### Prevent Double Booking



The system shall prevent overlapping appointments.



\---



\# Audit Requirements



All entities shall track:



\* CreatedAt

\* CreatedBy

\* UpdatedAt

\* UpdatedBy

\* IsDeleted



\---



\# Soft Delete Requirements



Records shall never be physically deleted.



When a record is deleted:



\* IsDeleted = true

\* UpdatedAt updated

\* UpdatedBy updated



