# Reservation Management System

A reservation management desktop application developed as a student project at **Karelia University of Applied Sciences (AMK Karelia)**.  
Created by a **team of five students** to simulate real-world business software with user roles, database integration, and intuitive UI.

---

## Project Overview

This application allows users to manage:

- Customer reservations
- Room and location data
- Services and users
- Invoicing and reports

The application is structured with **role-based access**, where standard users and administrators have different permissions.

---

## Project Details

- **Institution**: Karelia University of Applied Sciences (AMK Karelia)
- **Course Type**: Student project
- **Team**: 6 students
- **Platform**: Desktop application
- **Database**: MySQL / MariaDB (via HeidiSQL)
- **Language**: C# (.NET)

---

## Login Info

| Role  | Username | Password |
| ----- | -------- | -------- |
| Admin | `admin`  | `admin`  |

> ⚠️ The admin account is for setup only and must be deleted before deployment.

---

## Features

### Authentication & Roles

- Secure login via username/password
- Admin and standard user access
- Role-based permissions

### Reservation System

- Make, view, edit, and delete reservations
- Customer, room, time, and service selection
- Notes and creator tracking

### Customer Management

- Add, edit, and delete customers
- View customer reservation history

### Branch & Room Management (Admin)

- Add/edit/delete branches and rooms
- Set pricing, VAT, and capacity

### Services

- Manage services linked to locations
- Add/edit/delete service data

### User Management (Admin)

- Add/edit/delete users
- View employee reservation activity

### Reporting & Invoices

- Filterable reports
- Invoice creation with net/VAT/total fields

---

## Database Setup

- SQL files provided in the `/SQL` folder
- Use **HeidiSQL** or compatible tools
- Default user:
  - Username: `student`
  - Password: `student1`

Make sure the user has appropriate privileges.

---

## Getting Started

1. Clone or download the repository
2. Set up the database using provided SQL scripts
3. Launch the application (Visual Studio / .exe)
4. Log in using provided credentials
5. Start testing features and managing reservations!

---

## License

This is a student project.  
Not intended for production use without further development.

---

## Project Team

This reservation system was developed as a student project at **Karelia University of Applied Sciences (AMK Karelia)**  
by a group of **six students** during the spring semester of 2024.

**Team members:**
- [Berg Ida-Maria](https://github.com/IdaMariaB)
- [Karoliina Sallmen](https://github.com/KaroliinaSallmen)
- [Zamil Yaser Idrees](https://github.com/Idreesyaser)
- [Dmitrii Presniakov](https://github.com/1589presdm)
- Paula Timonen
- Ansa Allt
