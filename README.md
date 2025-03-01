# CommentApp

CommentApp is a web application that allows users to leave comments, reply to existing comments, edit or delete their own comments, and attach files. The app features a dynamic user interface with real-time updates, ensuring a smooth and interactive user experience.        

## Project Overview

CommentApp provides a comment management system built with modern architectural principles. The project is divided into two main components:

Backend: A .NET 9-based service that employs Clean Architecture principles.
Frontend: A Vue.js client delivering a responsive interface with real-time communication.

## Backend

### 1. Platform & Core Architecture
- **.NET 9:** The foundation of the backend.
- **Clean Architecture:** Enforces a clear separation of concerns through dedicated layers:
- **CommentApp.Domain:** Contains core business entities and domain logic.
- **CommentApp.Application:** Implements business use cases and orchestration.
- **CommentApp.Infrastructure:** Handles persistence (using Entity Framework) and external integrations.
- **CommentApp.API:** Exposes web endpoints via Carter.
- **CommentApp.Shared:** Provides shared utilities and common components.

### 2. Design Patterns & Principles
- **CQRS (Command Query Responsibility Segregation):** Distinguishes between command operations (modifications) and query operations (data retrieval).
- **DDD (Domain-Driven Design):** Focuses on aligning the system design with business requirements and core domain logic.

### 3. Core Libraries & Tools
- **MediatR:** Implements in-process messaging to decouple request handling.
- **Carter:** Offers a modular approach to API endpoint creation.
- **Entity Framework:** Facilitates ORM-based data persistence.
- **Fluent Validation: **Provides a fluent interface for robust input validation.

### 4. Messaging, Caching & Utilities
- **RabbitMQ:** Manages asynchronous messaging and event-driven communication.
- **Redis:** Implements caching mechanisms to improve performance.
- **Mapster:** Simplifies object-to-object mapping.
- **HTMLSanitizer:** Ensures the safety and cleanliness of HTML content.

### 5. Real-Time Communication
- **SignalR:** Enables real-time, bi-directional communication between the server and connected clients.

## Frontend

### 1. Core Framework
- **Vue.js:** A progressive JavaScript framework for building reactive and component-driven user interfaces.
### 2. Real-Time Integration
- **SignalR:** Integrates with the backend to deliver live updates and real-time interactions directly in the user interface.

# Deployment

## Prerequisites
***Ensure you have the following installed:***

- Docker, Docker Compose & Docker Desktop
- .NET 9 SDK (for manual backend runs without Docker)
- Node.js (for manual frontend runs without Docker)

## Running with Docker

### Clone the repository:

    git clone https://github.com/samper-bit/CommentApp-TestAssignment.git
    cd CommentApp-TestAssignment

### Steps to add the certificate:
Before deploying the application in Docker, you need to run Docker Desktop Application

Before deploying the application in Docker, you need to create an HTTPS certificate. This is required for the backend to run securely over HTTPS.

Run the following command in terminal to generate developer certificate:

    dotnet dev-certs https -ep $env:APPDATA\ASP.NET\Https\CommentApp.API.pfx -p password
	dotnet dev-certs https --trust

**YOU WILL NEED TO RESTART YOUR BROWSER FOR THE CHANGES TO TAKE EFFECT.**

### Run Docker Compose:

    docker-compose up --build

The backend will be available at: https://localhost:6060

The frontend will be available at: http://localhost:6061

## Running Without Docker (Optional)


### Clone the repository:
    git clone https://github.com/samper-bit/CommentApp-TestAssignment.git
    cd CommentApp-TestAssignment
	
### Start Redis:

Install [Memurai](http://https://www.memurai.com/get-memurai "Memurai") or [Redis](https://redis.io/downloads/ "Redis").

Ensure Redis is running on port 6379.

### Start RabbitMQ:

Install [RabbitMQ](https://www.rabbitmq.com/docs/download "RabbitMQ").

Ensure RabbitMQ is running on port 5672.

### Run the backend:
    cd src/CommentApp.Backend/CommentApp.API
    dotnet run --launch-profile https
The backend will be running at: https://localhost:5050

### Run the frontend:
    cd src/CommentApp.Frontend/commentappfrontend.web
    npm install
    npm run dev
The frontend will be running at: http://localhost:5051

