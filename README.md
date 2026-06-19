# FirstSimpleServerHTTP

A simple HTTP server built from scratch in C# using raw TCP sockets. This project was created as a learning exercise to understand how web servers, HTTP requests, and dynamic web pages work internally without using frameworks like ASP.NET.

## Overview

This project implements a minimal HTTP server that is capable of:

- Accepting TCP connections from clients (e.g., web browsers)
- Parsing raw HTTP requests manually
- Serving static files (HTML, CSS, JS, images, fonts)
- Handling basic HTTP methods (GET and POST)
- Routing requests to dynamic page classes using naming conventions
- Rendering simple HTML templates with embedded data
- Managing an in-memory user list as a data source

The main goal is educational: to understand how a web server works under the hood.

## Architecture

The system is structured in a few core components:

- **HttpServer**
  - Listens for incoming TCP connections
  - Reads and processes HTTP requests
  - Routes requests to static files or dynamic pages
  - Builds and sends HTTP responses

- **DataProcessor**
  - Parses raw HTTP request text
  - Extracts method, resource, HTTP version, headers, and parameters
  - Handles both query string and POST body parameters

- **DynamicPage**
  - Abstract base class for dynamic pages
  - Defines `Get` and `Post` methods
  - Provides a basic HTML rendering model

- **Pages (UserPage, RegisterPage)**
  - Implement dynamic behavior based on request parameters
  - Generate HTML responses using simple string templates

- **User Model**
  - Represents a user entity
  - Stores users in a static in-memory dictionary

## How It Works

1. A client sends an HTTP request to the server.
2. The server accepts the connection via TCP.
3. The raw request is read and parsed into a structured format.
4. The requested resource is resolved:
   - If it matches a static file, it is returned directly.
   - If a corresponding dynamic page class exists, it is instantiated via reflection.
5. The appropriate method (`GET` or `POST`) is executed.
6. The server builds an HTTP response manually and sends it back to the client.

## Key Concepts Demonstrated

- Low-level TCP socket programming
- Manual HTTP parsing
- Basic routing mechanism
- Reflection-based dynamic class loading
- Simple template rendering
- In-memory data storage
- Concurrent request handling using tasks

## Limitations

This project is not intended for production use. It has several simplifications:

- No support for full HTTP specification
- No persistent storage (data is lost when the server stops)
- No authentication or security mechanisms
- Limited error handling
- Basic concurrency model
- Fixed buffer size for request reading

## Project Structure

```

FirstSimpleServerHTTP/
│
├── Server/
│   ├── HttpServer.cs
│   ├── DataProcessor.cs
│
├── Pages/
│   ├── DynamicPage.cs
│   ├── UserPage.cs
│   ├── RegisterPage.cs
│
├── Models/
│   └── User.cs
│
├── www/
│   ├── index.html
│   ├── user.html
│   ├── userSearch.html
│
└── Program.cs

```

## Running the Project

1. Clone the repository
2. Open the solution in Visual Studio or any C# IDE
3. Build and run the project
4. Open a browser and go to:

```

[http://localhost:8080](http://localhost:8080)

```

## Example Routes

- `/` → Serves `index.html`
- `/user.html?user=1` → Displays user information
- `/register.html` (POST) → Registers a new user

## Purpose

This project was built to understand:

- How HTTP works under the hood
- How web servers process requests
- How dynamic web pages are generated
- How frameworks like ASP.NET abstract these mechanisms

It intentionally avoids frameworks to focus on core concepts.
