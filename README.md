# Craftly — E-Commerce Backend API

Craftly is a backend e-commerce system built with **ASP.NET Core Web API and .NET 8**.

The project is designed with a focus on **clean architecture, separation of concerns, maintainability, data consistency, and safe inventory handling during checkout and payment**.

---

## 🛠️ Tech Stack

* **C#**
* **.NET 8**
* **ASP.NET Core Web API**
* **Entity Framework Core**
* **SQL Server**
* **ASP.NET Core Identity**
* **JWT Authentication**
* **FluentValidation**
* **Swagger / OpenAPI**
* **Git & GitHub**

### Development Components

* Repository Pattern
* Service Layer
* Clean Architecture
* Result Pattern
* Explicit Database Transactions
* Inventory Reservation
* Atomic Stock Updates
* Mock Payment Provider
* User Secrets for sensitive configuration

---

## 🏗️ Architecture

Craftly follows a **Clean Architecture** approach with four main layers:

```text
ECommerce.API
      ↓
ECommerce.Application
      ↓
ECommerce.Domain

ECommerce.Infrastructure
      ↓
Application + Domain
```

### Layers

#### API

Responsible for:

* HTTP requests and responses
* Controllers
* Authentication/authorization
* Mapping application results to HTTP responses

#### Application

Contains the application's business logic and abstractions.

Examples:

* Services
* DTOs
* Repository interfaces
* Payment provider abstraction
* Transaction abstraction
* Validation

The Application layer does not directly depend on Entity Framework Core or SQL Server.

#### Domain

Contains the core business entities and domain rules.

Examples:

* Product
* Category
* Cart
* CartItem
* Order
* OrderItem
* InventoryReservation
* InventoryItem
* PaymentAttempt

#### Infrastructure

Responsible for implementation details such as:

* Entity Framework Core
* SQL Server
* Repository implementations
* ASP.NET Core Identity
* Payment provider implementation

---

## 🔐 Authentication & Authorization

Craftly uses **ASP.NET Core Identity and JWT authentication**.

Supported authentication features include:

* User registration
* Login
* JWT authentication
* Refresh tokens
* Logout
* Role-based authorization
* Email confirmation
* Google authentication

Sensitive configuration such as:

* Database connection strings
* JWT secret key

is stored using **ASP.NET Core User Secrets** instead of being committed to source control.

---

## 🛍️ Main Features

### Category Management

* Create category
* Update category
* Archive category
* Unarchive category
* Get all categories
* Get active categories

### Product Management

* Create products
* Update products
* Archive/unarchive products
* Product availability validation
* Category validation
* Stock management
* Reserved stock tracking

### Shopping Cart

* Create cart when needed
* Add products to cart
* Update quantities
* Remove items
* Clear cart
* Calculate cart totals

The cart does **not** reserve inventory.

Inventory is reserved during checkout.

### Orders

* Checkout from cart
* Create pending orders
* Store purchase-time product prices
* Calculate order totals on the server
* Track order status

Order lifecycle:

```text
Pending
   ↓
Confirmed
   ↓
Processing
   ↓
Shipped
   ↓
Delivered
```

Orders can also be cancelled according to the business rules.

### Inventory Reservation

During checkout, inventory is temporarily reserved before payment.

A reservation contains:

* Order
* Expiration time
* Reservation status
* Reserved products
* Reserved quantities

This allows payment retries without losing the reserved inventory.

### Payments

Craftly uses a payment provider abstraction:

```text
IPaymentProvider
       ↓
MockPaymentProvider
```

The current implementation uses a mock provider for development and testing.

The abstraction allows a real payment provider to be introduced later without changing the core payment business logic.

Multiple payment attempts are supported for the same order.

---

## ⚡ Inventory Concurrency

One important part of Craftly is preventing **overselling** when multiple customers try to purchase the same product simultaneously.

A simple stock check is not enough because two requests can read the same stock before either request updates it.

Craftly uses an atomic conditional database update:

```sql
UPDATE Products
SET ReservedQuantity = ReservedQuantity + @quantity
WHERE Id = @productId
  AND StockQuantity - ReservedQuantity >= @quantity;
```

The application checks the number of affected rows:

```text
1 row affected → reservation successful

0 rows affected → reservation failed
```

This makes the stock reservation operation safe against the race condition involved in simultaneous checkout attempts.

---

## 💳 Checkout & Payment Flow

The checkout process is divided into two stages.

### 1. Checkout

```text
Cart
 ↓
Validate Products
 ↓
Begin Transaction
 ↓
Reserve Inventory
 ↓
Create Pending Order
 ↓
Create Inventory Reservation
 ↓
Create Reservation Items
 ↓
Save Changes
 ↓
Commit Transaction
```

The cart is intentionally **not cleared** during checkout.

### 2. Payment

```text
Pending Order
      ↓
Check Reservation
      ↓
Process Payment
      ↓
Payment Successful?
     / \
   Yes  No
   ↓     ↓
Confirm  PaymentAttempt = Failed
Order    Order remains Pending
   ↓
Finalize Stock
   ↓
Confirm Reservation
   ↓
Clear Cart
```

If payment fails:

* The payment attempt is marked as failed.
* The order remains pending.
* The reservation remains active until expiration.
* Stock remains reserved.
* The customer can retry payment.

If payment succeeds:

* The order becomes confirmed.
* Reserved stock is finalized.
* The reservation becomes confirmed.
* The cart is cleared.

All final database changes are protected by a transaction.

---

## 🗃️ Main Database Relationships

```text
User
 ├── Cart
 │    └── CartItem ─── Product
 │
 └── Order
      ├── OrderItem ─── Product
      │
      ├── PaymentAttempt
      │
      └── InventoryReservation
              └── InventoryItem ─── Product

Category
    └── Product
```

---

## 📡 API Endpoints

### Authentication

```text
POST /api/Auth/register
POST /api/Auth/login
POST /api/Auth/refresh-token
POST /api/Auth/logout
```

### Categories

```text
POST   /api/Category
PUT    /api/Category/{id}
GET    /api/Category
GET    /api/Category/active
PATCH  /api/Category/{id}/archive
PATCH  /api/Category/{id}/unarchive
```

### Products

Product endpoints support product creation, updates, availability, and archive management.

### Cart

Cart endpoints support:

```text
GET    /api/Cart
POST   /api/Cart/items
PUT    /api/Cart/items
DELETE /api/Cart/items/{productId}
DELETE /api/Cart
```

### Orders

```text
POST /api/Order/checkout
```

### Payments

```text
POST /api/Payment/{orderId}
```

Swagger/OpenAPI can be used to explore the complete API.

---

## 🔄 Data Consistency

Craftly separates temporary inventory reservation from final stock deduction.

```text
Checkout:

StockQuantity
     ↓
ReservedQuantity += requested quantity


Successful Payment:

StockQuantity -= requested quantity
ReservedQuantity -= requested quantity
```

This prevents the cart from acting as an inventory reservation mechanism and keeps inventory state consistent with the payment lifecycle.

---

## 📁 Project Structure

```text
ECommerce/
│
├── ECommerce.API/
│   ├── Controllers/
│   └── ...
│
├── ECommerce.Application/
│   ├── Abstractions/
│   ├── DTOs/
│   ├── Services/
│   └── ...
│
├── ECommerce.Domain/
│   ├── Entities/
│   ├── Enums/
│   └── ...
│
├── ECommerce.Infrastructure/
│   ├── Data/
│   ├── Repositories/
│   ├── Identity/
│   └── ...
│
├── .gitignore
└── ECommerce.sln
```

---

## 🚀 Getting Started

### Prerequisites

Make sure you have:

* .NET 8 SDK
* SQL Server
* Visual Studio 2022 or another compatible IDE

### Clone the Repository

```bash
git clone https://github.com/mahmoud-147-dev/Craftly.git
cd Craftly
```

### Configure User Secrets

Sensitive configuration is stored using ASP.NET Core User Secrets.

Example:

```bash
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "YOUR_CONNECTION_STRING"

dotnet user-secrets set "Jwt:Key" "YOUR_SECRET_KEY"
```

Do not commit real secrets to GitHub.

### Apply Database Migrations

```bash
dotnet ef database update
```

### Run the API

```bash
dotnet run --project ECommerce.API
```

Then open the Swagger URL provided by ASP.NET Core.

---

## 🧪 Testing Scenarios

The current system has been manually tested against important business scenarios, including:

* Successful checkout
* Successful payment
* Failed payment
* Payment retry
* Inventory reservation
* Stock finalization after successful payment
* Cart preservation after failed payment
* Cart clearing after successful payment
* Insufficient stock during concurrent reservation attempts

---

## 🔮 Potential Future Improvements

Possible future improvements include:

* Replace the mock payment provider with a real payment gateway
* Background job for expired inventory reservations
* Redis caching
* Docker containerization
* Automated integration tests
* More advanced observability and logging
* Rate limiting
* Production deployment

---

## 👨‍💻 Author

**Mahmoud Mohamed**

Backend Developer focused on **C#, .NET, ASP.NET Core, SQL Server, and backend system design**.

GitHub:
https://github.com/mahmoud-147-dev
