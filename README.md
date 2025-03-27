# Student Information System (SIS) 📚

Welcome to the Student Information System! This web-based application allows for simple management of student data, including registration, login, course enrollment, and administrative management of students, courses, and enrollments.

---

## Table of Contents

- [Overview](#overview)
- [Features ✨](#features-)
- [User Manual](#user-manual)
  - [Registration 📝](#registration-📝)
  - [Login 🔐](#login-🔐)
  - [Course Enrollment (Student) 🎓](#course-enrollment-student-🎓)
  - [Administration (Admin) 👨‍💼](#administration-admin-👨💼)
- [System Design Document 🧠](#system-design-document-🧠)
- [Deployment 🚀](#deployment-🚀)
- [Security Considerations 🔒](#security-considerations-🔒)

---

## Overview

The **Student Information System (SIS)** is built on ASP.NET Web Forms with Entity Framework Code First, deployed on Azure App Service, and uses Supabase PostgreSQL for data storage. The system handles student registration, login, course enrollment, and administration of student records.

---

## Features ✨

- **User Registration & Automatic Sign-In**:  
  Users register with their first name, last name, email, and password. After registration, the system automatically logs them in.
- **Dynamic Role-Based Navigation**:  
  Depending on the user's role (Admin or Student), the "Get Started" button redirects to the appropriate dashboard.
- **Course Enrollment**:  
  Students can enroll in courses with available seats. Duplicate enrollments and capacity limits are enforced.
- **Administration**:  
  Admin users can manage students, courses, and enrollments through dedicated management pages.
- **Email Notifications**:  
  Enrollment confirmation emails are sent to students if enabled in account settings.
- **Responsive Design**:  
  The application uses Bootstrap for a responsive layout across devices.

---

## User Manual

### Registration 📝

1. **Navigate to the Registration Page**  
   Click on the "Register" link in the navigation bar.<br />
   (The Get Started Button also redirects to the login page)
   
3. **Fill in the Form**  
   Enter your first name, last name, email, password, and confirm your password.  
   *Note: Names are automatically converted to title case*
   
4. **Submit the Form**  
   Click the **Register** button.  
   - **Success:** You are automatically signed in and redirected to your student dashboard.  
   - **Failure:** If an account with that email already exists, you'll see:  
     *"An account with that email already exists." or any other errors*


### Login 🔐

1. **Navigate to the Login Page**  
   Click on the "Log in" link in the navigation bar.
   
2. **Enter Credentials**  
   Provide your email and password.  
   - If you select "Remember me," your login session will persist.
   
3. **Submit the Form**  
   Click the **Log in** button.  
   - **Success:** You are given access to your role-specific dashboard (Admins get Manage Students/Courses/Enrollments)<br />
(Students get Student-Dashboard and Course Enrollments).  
   - **Failure:** An inline error message (e.g., "Invalid login attempt.") is displayed.


### Course Enrollment (Student) 🎓

1. **Navigate to Enrollment**  
   From the Student Dashboard, click on "Enroll in Course."
   
2. **Select a Course**  
   Choose a course from the dropdown (each option displays the course name and available seats).
   
3. **Click Enroll**  
   Click the **Enroll** button.  
   - **Success:** A toast notification confirms "Enrolled successfully!" and the enrollment appears in your list.
   - **Failure:**  
     - If already enrolled: "You are already enrolled in this course."  
     - If the course is full: "Course capacity reached. Cannot enroll more students."

### Administration (Admin) 👨‍💼

> **Testing Admin Account:**  
> Use the following account for testing admin functionality:  
> **Email:** admin@example.com  
> **Password:** AdminPassword123!

Admins can:
- **Manage Students:** View, add, edit, or delete student records (Includes automatic Website login creation).
- **Manage Courses:** Edit course details and manage capacities.
- **Manage Enrollments:** Monitor and adjust student course enrollments.

---

## System Design Document 🧠

### Architectural Overview

- **Architecture:**  
  - ASP.NET Web Forms on .NET Framework 4.7.2  
  - Entity Framework Code First with migrations  
  - Deployed on Azure App Service  
  - Supabase PostgreSQL for data storage

- **Key Layers:**
  - **Presentation Layer:**  
    Web Forms (Default.aspx, Register.aspx, Login.aspx, dashboards, management pages)
  - **Business Logic Layer:**  
    ASP.NET Identity for user management, custom code for student/enrollment management, EmailHelper for notifications.
  - **Data Access Layer:**  
    Entity Framework Code First with migrations; uses a single connection string ("SupabaseConnection") for both Identity and custom tables.

### Data Model

- **Identity Tables:**  
  Managed by ASP.NET Identity (AspNetUsers, AspNetRoles, etc.)
  
- **Custom Tables:**
  - **students:**  
    Columns: id, identity_user_id, first_name, last_name, email, enrollment_date
  - **courses:**  
    Columns: course_id, course_name, capacity, etc.
  - **enrollments:**  
    Columns: enrollment_id, student_id, course_id, enrollment_date

### Workflows

- **Registration Workflow:**  
  User fills registration form → ASP.NET Identity creates user → Student record is inserted → User is auto-signed in → Redirect to homepage

- **Login Workflow:**  
  User enters credentials → System authenticates via ASP.NET Identity → Redirect based on role

- **Enrollment Workflow:**  
  Student selects a course → System validates enrollment (duplicate, capacity) → Inserts enrollment record → Sends email notification if enabled

---

## Deployment 🚀

- **Environment:**  
  Deployed to Azure App Service. <br />
`Site URL:` https://studentinformationsystemweb20250319154148-ejechneaaddyg4c3.canadacentral-01.azurewebsites.net/
  
- **Configuration:**  
  Web.config includes connection strings, SMTP settings, binding redirects, and EF provider settings.
  
- **Database:**  
  Supabase PostgreSQL is used for storing both ASP.NET Identity data and custom data.

---

## Security Considerations 🔒

- **Authentication & Authorization:**  
  Handled by ASP.NET Identity.
- **Input Validation:**  
  Both client-side (HTML5/ASP.NET validators) and server-side validations are in place.
- **Data Protection:**  
  Sensitive settings (e.g., SMTP credentials) are stored in web.config and secured appropriately.
- **SQL Injection Prevention:**  
  Parameterized queries are used in all database operations.

---
