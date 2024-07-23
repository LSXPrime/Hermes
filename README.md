## Hermes E-Commerce Platform

Hermes is a robust and scalable e-commerce platform built with .NET 8 and ASP.NET Core, providing a comprehensive
solution for managing products, orders, users, and various other aspects of an online store.

**Table of Contents**

* [Features](#features)
* [Architecture](#architecture)
* [Tech Stack](#tech-stack)
* [Concepts](#concepts)
* [Endpoints](#endpoints)
* [Installation](#installation)
* [Unit Testing](#unit-testing)
* [Further Development](#further-development)
* [Contributing](#contributing)
* [License](#license)

## Features

* **Product Management:**
    * Create, read, update, and delete products with multiple variants and options.
    * Manage product categories and subcategories.
    * Upload product images and manage image assets.
    * Search products by name, description, category, price, tags, and seller.

* **Order Management:**
    * Create and manage customer orders.
    * Track order status and history.
    * Calculate shipping costs and tax amounts.
    * Cancel orders and process refunds.
    * Send order confirmation and shipping update emails.

* **User Management:**
    * User registration and authentication with JWT tokens.
    * User profile management with address and order history.
    * Role-based access control with admin, seller, and user roles.
    * Password reset functionality.

* **Inventory Management:**
    * Track product inventory levels and stock status.
    * Reserve stock during order placement.
    * Release stock for cancelled orders.

* **Coupon Management:**
    * Create and manage coupons with various discount types and restrictions.
    * Apply and remove coupons from shopping carts.

* **Payment Processing:**
    * Integrate with Stripe for secure payment processing.
    * Create payment intents and checkout sessions.
    * Process refunds for cancelled orders.

* **Shipping Integration:**
    * Support for multiple shipping providers (Currently acts as a placeholder).
    * Calculate shipping rates and estimated delivery times.
    * Create and track shipments.

* **Reviews and Ratings:**
    * Allow customers to review and rate products.
    * Display product ratings and reviews.

## Architecture

Hermes follows a layered architecture with clean separation of concerns:

* **Presentation Layer (Hermes.API):**  ASP.NET Core Web API project responsible for handling HTTP requests and
  responses, exposing RESTful endpoints for the application's functionality.
    * **Controllers:**  Handle incoming requests, validate input, interact with services, and return appropriate
      responses.
    * **Filters:**  Intercept requests and responses to handle exceptions, validate model state, and perform
      authorization.
    * **Middlewares:**  Perform tasks like authorization and logging on incoming requests.
    * **Mappers:**  Handle mapping between data transfer objects (DTOs) and domain entities.
    * **Validators:**  FluentValidation validators to enforce business rules and data integrity on DTOs.

* **Application Layer (Hermes.Application):**  Contains the business logic of the application.
    * **Services:**  Implement business rules and orchestrate interactions between repositories and other services.
    * **DTOs:**  Data Transfer Objects for transferring data between layers.
    * **Interfaces:**  Define contracts for services and repositories.
    * **Exceptions:**  Custom exceptions for handling application-specific errors.

* **Domain Layer (Hermes.Domain):**  Contains the core business entities and rules of the application.
    * **Entities:**  Represent the core concepts of the business domain, like Product, User, Order, etc.
    * **Enums:**  Represent enumerations used in the domain.
    * **Interfaces:**  Define contracts for repositories.
    * **Settings:**  Holds configuration settings related to JWT, email, Azure storage, Stripe, etc.

* **Infrastructure Layer (Hermes.Infrastructure):**  Provides data access, services, and utilities.
    * **Repositories:**  Implement data access logic for interacting with the database.
    * **UnitOfWork:**  Provides a single point of access to all repositories, ensuring data consistency.
    * **Services:**  Implement infrastructure-level services like email sending, payment processing, and shipping
      integration.
    * **Utilities:**  Contain helper classes for tasks like image processing, cloud storage interaction, and data
      seeding.

* **Database:**  Uses SQL Server as the database for storing application data. Entity Framework Core is used as the ORM.

## Tech Stack

* **Backend:**
    * .NET 8
    * ASP.NET Core Web API
    * Entity Framework Core
    * SQL Server
    * FluentValidation
    * AutoMapper
    * MailKit
    * Stripe.NET
    * Azure Blob Storage SDK
    * EasyCaching
    * EFCoreSecondLevelCacheInterceptor

## Concepts

* **Layered Architecture:**  Clear separation of concerns between presentation, application, domain, and infrastructure
  layers.

* **Dependency Injection:**  Used throughout the application for loose coupling and testability.

* **Repository Pattern:**  Abstraction of data access logic, allowing for easy testing and switching between different
  data sources.

* **Unit of Work Pattern:**  Provides a consistent way to manage transactions and ensure data integrity.

* **Data Transfer Objects (DTOs):**  Used to transfer data between layers, minimizing data coupling.

* **JWT Authentication:**  Securely authenticate users and authorize access to protected resources.

* **FluentValidation:**  Provides a clean and expressive way to define validation rules for DTOs.

* **AutoMapper:**  Simplifies mapping between DTOs and domain entities.

* **Stripe Integration:**  Handles payment processing, including payment intents, checkout sessions, and refunds.

* **Shipping Integration:**  Provides a way to calculate shipping rates and track shipments from multiple providers (
  Currently acts as a placeholder).

* **Cloud Storage:**  Uses Azure Blob Storage for storing product images and other media files.

* **Email Services:**  Uses MailKit to send email notifications like order confirmations, shipping updates, and password
  reset emails.

* **Caching:**  Uses EasyCaching library to implement in-memory caching to improve application performance and reduce
  database load.
* **Second Level Cache Interceptor:**  Used for caching database queries using EFCoreSecondLevelCacheInterceptor to
  further enhance performance.

## Endpoints

The Hermes E-commerce platform provides a comprehensive set of RESTful API endpoints to manage all aspects of the online
store. Here's a summary of the key endpoints:

**Authentication:**

* `POST /api/Authentication/register`: Registers a new user.
* `POST /api/Authentication/login`: Logs in an existing user and returns JWT tokens.
* `POST /api/Authentication/refresh`: Refreshes the JWT token using a refresh token.
* `POST /api/Authentication/forgot`: Sends a password reset email.
* `POST /api/Authentication/reset/{token}/{newPassword}`: Resets the password using a password reset token.

**Cart:**

* `GET /api/Cart`: Retrieves the current user's shopping cart.
* `POST /api/Cart/items/{productVariantId}/{quantity}`: Adds an item to the cart.
* `PUT /api/Cart/items/{productId}/{quantity}`: Updates the quantity of an item in the cart.
* `DELETE /api/Cart/items/{productId}`: Removes an item from the cart.
* `DELETE /api/Cart/clear`: Clears the cart.
* `POST /api/Cart/coupons/{couponCode}`: Applies a coupon to the cart.
* `DELETE /api/Cart/coupons`: Removes the applied coupon from the cart.

**Categories:**

* `GET /api/Categories`: Retrieves all categories.
* `GET /api/Categories/{id}`: Retrieves a category by ID.
* `GET /api/Categories/{id}/subcategories`: Retrieves subcategories for a category.
* `POST /api/Categories`: Creates a new category.
* `PUT /api/Categories/{id}`: Updates a category.
* `DELETE /api/Categories/{id}`: Deletes a category.

**Coupons:**

* `POST /api/Coupons`: Creates a new coupon.
* `PUT /api/Coupons/{id}`: Updates a coupon.
* `DELETE /api/Coupons/{id}`: Deletes a coupon.
* `GET /api/Coupons/active`: Retrieves all active coupons.
* `GET /api/Coupons/expired`: Retrieves all expired coupons.
* `GET /api/Coupons/{id}`: Retrieves a coupon by ID.

**Orders:**

* `POST /api/Orders/preview`: Returns an order preview with shipping rates.
* `POST /api/Orders`: Creates a new order.
* `GET /api/Orders`: Retrieves all orders (admin only).
* `GET /api/Orders/user`: Retrieves orders for the current user.
* `GET /api/Orders/{id}`: Retrieves an order by ID.
* `POST /api/Orders/{id}/cancel`: Cancels an order.
* `PUT /api/Orders/{id}/status`: Updates the order status.
* `DELETE /api/Orders/{id}`: Deletes an order (admin only).

**Payments:**

* `POST /api/Payments/create-checkout-session/{orderId}`: Creates a Stripe checkout session for an order.
* `POST /api/Payments/create-payment-intent/{orderId}`: Creates a Stripe payment intent for an order.
* `POST /api/Payments/webhook`: Stripe webhook endpoint for handling payment events.

**Products:**

* `GET /api/Products`: Retrieves all products.
* `GET /api/Products/{id}`: Retrieves a product by ID.
* `GET /api/Products/category/{categoryId}`: Retrieves products by category.
* `GET /api/Products/search`: Searches for products based on various criteria.
* `GET /api/Products/top-selling`: Retrieves top-selling products.
* `GET /api/Products/latest`: Retrieves the latest products.
* `POST /api/Products`: Creates a new product.
* `PUT /api/Products/{id}`: Updates a product.
* `DELETE /api/Products/{id}`: Deletes a product.
* `POST /api/Products/variants/{productId}`: Creates a new product variant.
* `PUT /api/Products/variants/{variantId}`: Updates a product variant.
* `DELETE /api/Products/variants/{variantId}`: Deletes a product variant.
* `POST /api/Products/upload-image`: Uploads a product image.

**Reviews:**

* `POST /api/Reviews/products/{productId}`: Creates a review for a product.
* `GET /api/Reviews/products/{productId}`: Retrieves reviews for a product.
* `GET /api/Reviews/{id}`: Retrieves a review by ID.
* `PUT /api/Reviews/{id}`: Updates a review.
* `DELETE /api/Reviews/{id}`: Deletes a review.

**Shipping:**

* `POST /api/Shipping/rates`: Calculates shipping rates for a list of shipping requests.
* `POST /api/Shipping/create-shipment`: Creates a shipment.
* `GET /api/Shipping/track/{trackingNumber}`: Tracks a shipment.
* `POST /api/Shipping/cancel/{trackingNumber}`: Cancels a shipment.

**Users:**

* `GET /api/Users/me`: Retrieves the current user's profile.
* `PUT /api/Users/me`: Updates the current user's profile.
* `GET /api/Users/{userId}`: Retrieves a user by ID (admin only).
* `GET /api/Users`: Retrieves all users (admin only).

## Installation

1. **Prerequisites:**
    * .NET 8 SDK
    * SQL Server
    * Stripe Account
    * Azure Storage Account

2. **Clone the repository:**
   ```
   git clone https://github.com/your-username/hermes.git
   ```

3. **Configure connection strings:**
    * Update the `DefaultConnection` connection string in `Hermes.API/appsettings.json` with your SQL Server connection
      details.
    * Configure the necessary settings for JWT, email, Azure storage, Stripe, and warehouse address in
      the `appsettings.json` file.

4. **Run database migrations:**
    * Open a terminal in the `Hermes.API` directory.
    * Run the following command to create and apply database migrations:
      ```
      dotnet ef database update
      ```

5. **Seed the database (optional):**
    * Uncomment the data seeding section in `Hermes.API/Program.cs` to populate the database with sample data.

6. **Build and run the application:**
    * Run the following command in the `Hermes.API` directory:
      ```
      dotnet run
      ```

7. **Access the API:**
    * The API will be accessible at `https://localhost:5001/api`.
    * Use a tool like Swagger UI (accessible at `https://localhost:5001/swagger`) to explore the API endpoints and test
      the functionality.

## Unit Testing

The project includes a Unit Testing project (`Hermes.API.Tests.Integration`) to test all endpoints and ensure the
functionality and correctness of the application. These tests have a coverage percent of 82% that cover:

* **Controllers:** Test the API endpoints exposed by the controllers, including validation and authorization logic.
* **Services:** Test the business logic implemented in the application services, including interactions with
  repositories.
* **Repositories:** Test the data access logic implemented in the repositories, ensuring data integrity and correctness.
* **Integration Tests:** Test the interaction between different layers of the application, including the database and
  external services like Stripe.

## Further Development

As a user of e-commerce platforms, I've used them once or twice to purchase AirPods. I was too lazy to leave my room and
buy them from a store. Because I don't have extensive experience with e-commerce, there are some things I haven't
implemented yet due to lack of understanding. I've listed these in the TODO section.

**1. Shipping Integration:**

* The current `MultiShippingService` is a placeholder and needs to be replaced with a proper integration with a shipping
  API like Shippo, EasyPost, or direct carrier APIs.

**2. Tax Calculation:**

* The `CalculateTax` method needs to be implemented with accurate tax calculation logic based on regional tax
  regulations and potentially integrated with a tax calculation service.

**3. CBAC Authorization:**

* Implement a more robust authorization system based on claims-based access control (CBAC) to fine-tune user
  permissions and access to resources.

## Contributing

Contributions are welcome!

* Fork the repository.
* Create a new branch for your feature or bug fix.
* Make your changes and commit them.
* Submit a pull request.

## License

This project is licensed under the MIT License.
