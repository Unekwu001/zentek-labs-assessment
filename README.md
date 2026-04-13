# zentek-labs-assessment

This project is a **.NET 10 Web API** built using Clean Architecture principles. It provides a simple product management system with authentication, logging, and structured API responses.

---

## Features

- JWT Authentication (Register & Login)
- Product Management (Create & Retrieve)
- Filter Products by Colour
- Pagination, Sorting, and Filtering support
- Global API Response Wrapper
- Structured Logging with Serilog
- Entity Framework Core (SQLite)

---

## Project Structure

- **Api** → Controllers and API configuration
- **Core** → Business logic and services
- **Data** → Database models, DbContext, repositories
- **Common** → Shared DTOs and response models

---

## Authentication

The API uses JWT tokens.

Claims included:
- `sub` → User ID
- `email` → User email

---

## Sample Endpoints

### Auth
- `POST /api/v1/auth/register`
- `POST /api/v1/auth/login`

### Products
- `POST /api/v1/products` → Create product
- `GET /api/v1/products/{colour}` → Filter by colour
- `GET /api/v1/products/all` → Get paginated products

---

## Sample Product Payload

```json
{
  "name": "Nike Air Max",
  "description": "Comfortable running shoes",
  "price": 120.99,
  "stockQuantity": 50,
  "colour": "black",
  "imageUrl": "https://example.com/image.jpg"
}
