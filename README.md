# 🚀 CompanyHub

> A production-style Multi-Tenant B2B SaaS Platform built with ASP.NET Core and Clean Architecture.

---

# 📖 Overview

CompanyHub is a modern multi-tenant SaaS backend designed to simulate real-world enterprise applications.

The project focuses on scalability, security, maintainability, and clean software architecture by implementing production-ready features such as multi-tenancy, authentication, payments, background jobs, caching, reporting, and more.

It was built as a portfolio project to practice enterprise backend development using the .NET ecosystem.

---

# ✨ Features

## 🔐 Authentication & Security

- JWT Authentication
- Refresh Tokens
- Role-Based Authorization
- Permission-Based Authorization (Custom Policies)
- Two-Factor Authentication (TOTP)
- Email Verification
- Session Management
- API Key Authentication
- Password Reset

---

## 🏢 Multi-Tenancy

- Tenant Isolation using EF Core Global Query Filters
- Tenant Management
- Tenant Onboarding
- Subscription Management
- Plan-Based Usage Limits

---

## 💳 Payments

- Paymob Integration
- Secure Webhook Processing
- HMAC Validation
- Payment History
- Invoice Generation
- PDF Export (QuestPDF)

---

## ⚡ Performance

- Redis Distributed Cache
- Background Jobs using Hangfire
- Health Checks
- Optimized EF Core Queries

---

## 📡 Real-Time Features

- SignalR Notifications
- Notification Management

---

## 📊 Reporting & Analytics

- Revenue Reports
- Subscription Reports
- Dashboard Statistics
- Audit Logging
- Usage Tracking

---

## 📂 Project Structure

```
CompanyHub
│
├── CompanyHub.API
│
├── CompanyHub.Application
│
├── CompanyHub.Domain
│
├── CompanyHub.Infrastructure
│
├── CompanyHub.Persistence
```

---

# 🏗️ Architecture

The project follows **Clean Architecture**, ensuring a clear separation of responsibilities.

```
Presentation (API)

↓

Application
(Business Logic)

↓

Domain
(Entities & Rules)

↓

Infrastructure
(External Services)

↓

Persistence
(Database)
```

Key Design Principles:

- Clean Architecture
- SOLID Principles
- Dependency Injection
- Repository Pattern
- CQRS-inspired Service Layer
- Interface-based Design

---

# 🛠️ Technologies

### Backend

- ASP.NET Core
- C#
- Entity Framework Core
- SQL Server

### Authentication

- JWT
- ASP.NET Authorization Policies
- BCrypt

### Performance

- Redis
- Hangfire

### Real-Time

- SignalR

### Payments

- Paymob

### Documents

- QuestPDF

### API Documentation

- Swagger / OpenAPI

### Testing

- xUnit
- Moq
- FluentAssertions

---

# 🚀 Getting Started

### Clone Repository

```bash
git clone https://github.com/yourusername/CompanyHub.git
```

### Navigate

```bash
cd CompanyHub
```

### Restore Packages

```bash
dotnet restore
```

### Update Database

```bash
dotnet ef database update
```

### Run

```bash
dotnet run
```

Open Swagger:

```
https://localhost:xxxx/swagger
```

---

# 🔐 Authentication & Authorization

CompanyHub implements multiple authentication and authorization mechanisms:

- JWT Authentication
- Refresh Tokens
- Permission-Based Authorization
- Role-Based Authorization
- Two-Factor Authentication (TOTP)
- API Keys
- Email Verification

---

# 🌐 Multi-Tenancy

Each tenant has complete data isolation using EF Core Global Query Filters.

Supported features:

- Tenant Registration
- Tenant Management
- Tenant Logo Upload
- Usage Limits
- Subscription Plans

---

# 💳 Payment Integration

Integrated with **Paymob**.

Payment Flow:

```
Client

↓

Create Order

↓

Generate Payment Key

↓

Payment

↓

Webhook

↓

Verify HMAC

↓

Update Payment Status
```

---

# 📊 Database

Database is built using **Entity Framework Core Code First**.

Main Modules:

- Users
- Roles
- Permissions
- Tenants
- Plans
- Subscriptions
- Payments
- Invoices
- API Keys
- Notifications
- Audit Logs
- Usage Records

---

# 📸 Screenshots

### Swagger API

_Add Screenshot_

### Database Diagram

_Add Screenshot_

### Dashboard

_Add Screenshot_



---

# 📌 Roadmap

- Docker Support
- Kubernetes Deployment
- CI/CD Pipeline
- Azure Deployment
- API Versioning
- Integration Tests
- Distributed Caching Improvements
- Metrics & Monitoring

---

# 👨‍💻 Author

**Mahmoud Abdelsamed**

Backend .NET Developer

- LinkedIn
- GitHub
- Email

---

## ⭐ Support

If you found this project useful, consider giving it a ⭐ on GitHub.
