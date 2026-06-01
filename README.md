<div align="center">

# 🛒 PrimeMarket — Backend API

**ASP.NET Core · .NET 10 · EF Core · SQL Server · SignalR · Stripe · Hangfire**

[![.NET](https://img.shields.io/badge/.NET-10.0-512BD4?style=flat-square&logo=dotnet)](https://dotnet.microsoft.com/)
[![ASP.NET Core](https://img.shields.io/badge/ASP.NET_Core-Web_API-512BD4?style=flat-square&logo=dotnet)](https://dotnet.microsoft.com/apps/aspnet)
[![EF Core](https://img.shields.io/badge/EF_Core-SQL_Server-CC2927?style=flat-square&logo=microsoftsqlserver)](https://docs.microsoft.com/ef/)
[![Stripe](https://img.shields.io/badge/Stripe-Payment-635BFF?style=flat-square&logo=stripe)](https://stripe.com/)
[![SignalR](https://img.shields.io/badge/SignalR-Real--Time-512BD4?style=flat-square)](https://dotnet.microsoft.com/apps/aspnet/signalr)

A production-grade multi-role e-commerce marketplace API supporting Customer, Seller, and Admin workflows.
Built as a capstone project for the **ITI Professional Development & BI-infused CRM** track.

[GitHub — Frontend](https://github.com/Omar-Nabil2/PrimeMarket.git) · [Live API](https://primemarket.runasp.net/swagger/index.html) · [Demo Video](https://drive.google.com/drive/folders/10MTw88EuSTDAkqmA8uTsj0_GNZw7rPj4?usp=sharing) · [Documentation](https://drive.google.com/drive/folders/10MTw88EuSTDAkqmA8uTsj0_GNZw7rPj4?usp=sharing)

</div>

---

## 📋 Table of Contents

- [Overview](#-overview)
- [Tech Stack](#-tech-stack)
- [Architecture](#-architecture)
- [Features](#-features)
- [API Reference](#-api-reference)
- [Data Models](#-data-models)
- [Getting Started](#-getting-started)
- [Configuration](#-configuration)
- [Project Structure](#-project-structure)
- [Team](#-team)

---

## 🌐 Overview

PrimeMarket is a RESTful Web API for a multi-role e-commerce marketplace. It handles three distinct user roles — **Customer**, **Seller**, and **Admin** — each with dedicated endpoints, role-gated authorization, and isolated data views.

The API integrates real-world production services:

- **Stripe** for PCI-compliant payment processing via webhooks
- **Cloudinary** for image hosting and CDN delivery
- **SignalR** for real-time order and notification push
- **Hangfire** for background job scheduling
- **SendGrid / MailKit** for transactional email

---

## 🛠 Tech Stack

| Layer | Technology | Purpose |
|---|---|---|
| Framework | .NET 10 / ASP.NET Core Web API | HTTP pipeline, DI, middleware |
| ORM | Entity Framework Core | Database access and migrations |
| Database | SQL Server | Primary data store |
| Identity | ASP.NET Core Identity | User management, password hashing, roles |
| Auth | JWT Bearer + Refresh Tokens | Stateless auth with sliding session renewal |
| Real-Time | SignalR | Order updates and notification push |
| Background Jobs | Hangfire + SQL Server | Async job queue and dashboard |
| Logging | Serilog | Structured logging with sink configuration |
| Mapping | Mapster | High-performance DTO mapping |
| Validation | FluentValidation | Declarative request validation |
| Payments | Stripe + Webhooks | Payment intents and server-side fulfilment |
| Images | Cloudinary | Upload, transform, CDN delivery |
| Email | SendGrid / MailKit | Confirmation, password reset, notifications |
| Social Auth | Google.Apis.Auth | Google OAuth ID token validation |
| Docs | Swagger / OpenAPI | Auto-generated API documentation |

---

## 🏗 Architecture

The project follows a **Clean Layered Architecture** with clear separation of concerns:

```
PrimeMarket/
├── Controllers/          # Thin HTTP layer — receives requests, returns responses
├── Services/             # All business logic (interface + implementation pairs)
├── Entities/             # EF Core domain models
├── Contracts/            # Request / Response DTOs (API contract)
├── Persistence/          # DbContext, EntityConfiguration, Migrations
├── Authentication/       # JWT generation and validation
├── Hubs/                 # SignalR NotificationHub
├── Mapping/              # Mapster profiles
├── Helpers/              # Cloudinary, email, slug, order utilities
├── Errors/               # Domain-specific error definitions
├── Settings/             # Configuration POCOs (JWT, Mail, Cloudinary)
└── Templates/            # Email templates
```

### Key Design Decisions

**JWT + Refresh Token Rotation**
Short-lived access tokens (JWT) paired with rotating refresh tokens. Each refresh token use invalidates the previous one — a replayed stolen token will cause the legitimate user's next refresh to fail, signalling a potential breach.

**Stripe Webhooks over Client Callbacks**
Order creation happens exclusively inside the Stripe webhook handler (`POST /api/Payment/webhook`), not from a client success callback. A user closing the browser tab after payment but before the callback fires would otherwise leave a paid order unfulfilled.

**Role-Scoped Endpoints**
The same data is exposed through role-specific endpoints. `/api/Orders` returns the customer's own orders, `/api/Orders/seller` returns only orders containing the seller's products, and `/api/Orders/admin` returns all orders. This avoids a single fat endpoint with conditional branching and keeps authorization clean.

**SignalR Role Promotion**
When an admin approves a seller request, a SignalR event is broadcast to that specific user. The Angular client receives it and refreshes its auth state immediately — no "log out and log back in" required.

**Mapster over AutoMapper**
Mapster's source-generation support eliminates reflection overhead at runtime, which matters when mapping large product lists or paginated order histories.

---

## ✨ Features

### 🔐 Authentication & User Management
- Email/password registration with **email confirmation** (MailKit/SendGrid)
- JWT login with short-lived access token
- **Refresh token rotation** — sliding session without re-login
- **Google OAuth** — ID token validated via `Google.Apis.Auth`
- Password reset via tokenized email link
- Profile update and Cloudinary profile image upload
- Role-based access: `Customer`, `Seller`, `Admin`

### 🛍 Marketplace & Catalog
- Public product listing with filtering and pagination
- Category-based and brand-based product browsing
- Seller product CRUD with multi-image upload (Cloudinary)
- Primary image selection per product
- Admin-level product oversight across all sellers

### 🛒 Cart & Wishlist
- Server-side cart — persists across sessions and devices
- Add, update quantity, remove cart items
- Wishlist add/remove and list

### 💳 Orders & Checkout
- Place orders with saved address and optional promo code
- Promo code validation before order placement
- **Stripe webhook** as the authoritative payment confirmation
- Customer, Seller, and Admin order views
- Order status updates with real-time SignalR notification to customer

### 🏪 Seller & Brand Management
- Seller registration request flow
- Admin approval/rejection — promotes user to `Seller` role live via SignalR
- Brand status tracking

### 📦 Inventory
- Stock level listing per product
- Manual stock adjustment

### 🔔 Notifications
- Per-user notification history
- Mark individual or all notifications as read
- **Admin broadcast** to all connected clients via SignalR
- Real-time delivery via `NotificationHub`

### ⚙️ Background Jobs
- Hangfire dashboard at `/jobs` (basic auth protected)
- SQL Server-backed job queue
- Decouples long-running tasks from the HTTP request lifecycle

---

## 📡 API Reference

> Full Swagger documentation available at `/swagger` in Development mode.

### Auth

| Method | Endpoint | Auth | Description |
|---|---|---|---|
| POST | `/api/Auth` | ❌ | Login — returns JWT + refresh token |
| POST | `/api/Auth/Register` | ❌ | Register new account |
| POST | `/api/Auth/new-refresh` | ❌ | Exchange refresh token for new access token |
| POST | `/api/Auth/revoke-refresh-token` | ✅ | Revoke refresh token on logout |
| POST | `/api/Auth/confirm-email` | ❌ | Confirm email with tokenized link |
| POST | `/api/Auth/resend-confirmation-email` | ❌ | Resend confirmation email |
| POST | `/api/Auth/ForgetPassword-Confirm` | ❌ | Request password reset email |
| POST | `/api/Auth/reset-password` | ❌ | Submit new password |
| POST | `/api/Auth/LoginWithGoogle` | ❌ | Authenticate via Google ID token |

### Account

| Method | Endpoint | Auth | Description |
|---|---|---|---|
| GET | `/api/Account/Info` | ✅ | Get authenticated user's profile |
| POST | `/api/Account/Info` | ✅ | Update profile info |
| PUT | `/api/Account/Change-Password` | ✅ | Change password |
| POST | `/api/Account/Profile-Image` | ✅ | Upload profile picture |

### Products

| Method | Endpoint | Role | Description |
|---|---|---|---|
| GET | `/api/Products` | Public | List products with filtering & pagination |
| GET | `/api/Products/{id}` | Public | Product detail |
| GET | `/api/Products/category/{categoryId}` | Public | Products by category |
| GET | `/api/Products/seller` | Seller | Own products only |
| GET | `/api/Products/admin` | Admin | All products |
| POST | `/api/Products` | Seller | Create product |
| PUT | `/api/Products/{id}` | Seller | Update product |
| DELETE | `/api/Products/{id}` | Seller/Admin | Delete product |
| POST | `/api/Products/{id}/images` | Seller | Upload images |
| DELETE | `/api/Products/{id}/images/{imageId}` | Seller | Delete image |
| PUT | `/api/Products/{id}/images/{imageId}/set-primary` | Seller | Set primary image |

### Cart

| Method | Endpoint | Role | Description |
|---|---|---|---|
| GET | `/api/Cart` | Customer | Fetch cart |
| POST | `/api/Cart/{productId}` | Customer | Add to cart |
| PUT | `/api/Cart/{cartItemId}` | Customer | Update quantity |
| DELETE | `/api/Cart/{cartItemId}` | Customer | Remove item |

### Wishlist

| Method | Endpoint | Role | Description |
|---|---|---|---|
| GET | `/api/WishList` | Customer | Fetch wishlist |
| POST | `/api/WishList/{productId}` | Customer | Add to wishlist |
| DELETE | `/api/WishList/{productId}` | Customer | Remove from wishlist |

### Orders

| Method | Endpoint | Role | Description |
|---|---|---|---|
| POST | `/api/Orders` | Customer | Place an order |
| POST | `/api/Orders/validate-promo` | Customer | Validate promo code |
| GET | `/api/Orders` | Customer | Customer order history |
| GET | `/api/Orders/{id}` | Customer | Order detail |
| GET | `/api/Orders/seller` | Seller | Seller's incoming orders |
| GET | `/api/Orders/seller/{orderId}` | Seller | Seller order detail |
| PUT | `/api/Orders/{orderId}/status` | Seller | Update order status |
| GET | `/api/Orders/admin` | Admin | All orders |
| GET | `/api/Orders/admin/{orderId}` | Admin | Admin order detail |

### Users (Admin)

| Method | Endpoint | Role | Description |
|---|---|---|---|
| GET | `/api/User` | Admin | List all users |
| GET | `/api/User/{id}` | Admin | User detail |
| POST | `/api/User` | Admin | Create user |
| PUT | `/api/User/{id}` | Admin | Update user |
| PUT | `/api/User/{id}/toggle-status` | Admin | Enable / disable account |
| PUT | `/api/User/{id}/unlock` | Admin | Unlock locked account |

### Categories

| Method | Endpoint | Role | Description |
|---|---|---|---|
| GET | `/api/Categories` | Public | List categories |
| GET | `/api/Categories/{id}` | Public | Category detail |
| POST | `/api/Categories` | Admin | Create category |
| PUT | `/api/Categories/{id}` | Admin | Update category |
| DELETE | `/api/Categories/{id}` | Admin | Delete category |

### Brands

| Method | Endpoint | Role | Description |
|---|---|---|---|
| GET | `/api/Brands` | Public | List brands |
| POST | `/api/Brands/register` | Customer | Request seller / register brand |
| GET | `/api/Brands/status` | Seller | Check own brand approval status |
| GET | `/api/Brands/seller-requests` | Admin | List pending seller requests |
| PUT | `/api/Brands/{id}/approve` | Admin | Approve seller — promotes role via SignalR |
| DELETE | `/api/Brands/{id}/reject` | Admin | Reject seller request |

### Notifications

| Method | Endpoint | Role | Description |
|---|---|---|---|
| GET | `/api/Notification` | ✅ | List notifications |
| PATCH | `/api/Notification/{id}/read` | ✅ | Mark one as read |
| PATCH | `/api/Notification/read-all` | ✅ | Mark all as read |
| POST | `/api/Notification/broadcast` | Admin | Push to all connected users |

### Other

| Method | Endpoint | Description |
|---|---|---|
| POST | `/api/Payment/webhook` | Stripe webhook — creates order on payment confirmation |
| GET | `/api/Addresses` | List saved shipping addresses |
| POST | `/api/Addresses` | Save new address |
| GET | `/api/Inventory` | Stock levels (Seller) |
| POST | `/api/Inventory/adjust` | Adjust stock (Seller) |
| GET | `/api/PromoCode/All` | List promo codes (Seller) |
| POST | `/api/PromoCode` | Create promo code (Seller) |
| PUT | `/api/PromoCode/{id}` | Update promo code (Seller) |
| POST | `/api/Reviews/{productId}` | Submit product review (Customer) |

### SignalR Hub

| Hub | Endpoint | Description |
|---|---|---|
| NotificationHub | `/hubs/notifications` | Real-time notifications — requires JWT via `accessTokenFactory` |

---

## 🗄 Data Models

| Entity | Key Relationships | Purpose |
|---|---|---|
| `ApplicationUser` | → Orders, CartItems, Wishlist, Products, Reviews, RefreshTokens | Central identity entity extending IdentityUser |
| `Product` | → ProductCategory (M:M), ProductImages, CartItems, OrderItems, Reviews | Seller-owned catalog item |
| `Order` | → OrderItems, Address, PromoCode, ApplicationUser | Purchase record created by Stripe webhook |
| `OrderItem` | → Order, Product | Price snapshot line item |
| `CartItem` | → ApplicationUser, Product | Active shopping cart — cleared after order creation |
| `PromoCode` | → Orders, ApplicationUser (seller) | Discount code with usage limit and expiry |
| `Notification` | → ApplicationUser | Persisted notification — order updates, broadcasts |
| `RefreshToken` | → ApplicationUser | Tracked refresh tokens with revocation support |
| `Brand` | → ApplicationUser (seller) | Seller marketplace identity — requires admin approval |
| `Review` | → Product, ApplicationUser | Customer product rating and comment |
| `Address` | → ApplicationUser, Orders | Saved shipping address reused at checkout |

---

## 🚀 Getting Started

### Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- SQL Server (local, Docker, or Azure)
- Stripe account
- Cloudinary account
- SendGrid or SMTP credentials

### Installation

```bash
# 1. Clone the repository
git clone https://github.com/iibrahimshaban/PrimeMarket.git
cd PrimeMarket

# 2. Restore dependencies
dotnet restore

# 3. Apply database migrations
dotnet ef database update

# 4. Run the API
dotnet run
```

The API starts at `https://localhost:7240`.
Swagger UI is available at `https://localhost:7240/swagger` in Development mode.

---

## ⚙️ Configuration

Copy `appsettings.json` and populate the following sections:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=...;Database=PrimeMarket;..."
  },
  "JwtSettings": {
    "Key": "<your-256-bit-secret>",
    "Issuer": "PrimeMarket",
    "Audience": "PrimeMarket",
    "ExpiryMinutes": 60
  },
  "CloudinarySettings": {
    "CloudName": "<cloud-name>",
    "ApiKey": "<api-key>",
    "ApiSecret": "<api-secret>"
  },
  "StripeSettings": {
    "SecretKey": "sk_test_...",
    "WebhookSecret": "whsec_..."
  },
  "MailSettings": {
    "Host": "smtp.sendgrid.net",
    "Port": 587,
    "UserName": "apikey",
    "Password": "<sendgrid-api-key>",
    "FromEmail": "noreply@primemarket.com"
  }
}
```

### Stripe Webhook (Local Development)

```bash
# Install Stripe CLI and forward events to your local API
stripe listen --forward-to https://localhost:7240/api/Payment/webhook
```

Copy the webhook signing secret from the CLI output into `StripeSettings.WebhookSecret`.

Use test card `4242 4242 4242 4242` for checkout testing.

---

## 📁 Project Structure

```
PrimeMarket/
│
├── Controllers/
│   ├── AuthController.cs
│   ├── ProductsController.cs
│   ├── OrdersController.cs
│   ├── CartController.cs
│   ├── WishListController.cs
│   ├── UserController.cs
│   ├── BrandsController.cs
│   ├── CategoriesController.cs
│   ├── NotificationController.cs
│   ├── InventoryController.cs
│   ├── PromoCodeController.cs
│   ├── ReviewsController.cs
│   ├── AddressesController.cs
│   ├── AccountController.cs
│   └── PaymentController.cs
│
├── Services/
│   ├── Interfaces/          # Service contracts
│   └── Implementations/     # Business logic
│
├── Entities/
│   ├── ApplicationUser.cs
│   ├── Product.cs
│   ├── Order.cs / OrderItem.cs
│   ├── CartItem.cs
│   ├── Wishlist.cs
│   ├── PromoCode.cs
│   ├── Notification.cs
│   ├── RefreshToken.cs
│   ├── Brand.cs
│   ├── Review.cs
│   └── Address.cs
│
├── Persistence/
│   ├── ApplicationDbContext.cs
│   ├── EntityConfiguration/  # Fluent API per entity
│   └── Migrations/
│
├── Authentication/
│   ├── JwtOptions.cs
│   └── JwtService.cs
│
├── Hubs/
│   └── NotificationHub.cs
│
├── Contracts/               # Request / Response DTOs
├── Mapping/                 # Mapster profiles
├── Helpers/                 # Cloudinary, email, slug utilities
├── Errors/                  # Domain error definitions
├── Settings/                # Configuration POCOs
│
├── Program.cs               # App builder and middleware pipeline
└── DependencyInjection.cs   # Central service registration
```

---

## 👥 Team

| Code | Name | Contribution |
|---|---|---|
| 6 | **Ibrahim Khaled** | Cart · Wishlist · Orders (customer) · Checkout · Stripe webhook · Addresses · Reviews |
| 15 | **Mohamed ElMassry** | Products (seller) · Product images · Orders (seller) · Brands · Inventory · Promo codes · Hangfire |
| 24 | **Omar Nabil** | Auth system · JWT/Refresh tokens · Google OAuth · User management · Categories · Notifications · SignalR hub · Admin APIs |

---

## 🔗 Related

| Resource | Link |
|---|---|
| Frontend Repository | [github.com/Omar-Nabil2/PrimeMarket](https://github.com/Omar-Nabil2/PrimeMarket.git) |
| Live API | _Coming soon_ |
| Live Frontend | _Coming soon_ |
| Demo Video | _Coming soon_ |

---

<div align="center">

Made with ❤️ by the PrimeMarket team · ITI 2024/2025

</div>
