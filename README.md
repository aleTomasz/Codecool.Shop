# 🛍️ CodecoolShop

A dynamic online shop built with ASP.NET Core MVC, featuring category-based browsing, cart management, and checkout flow.
This project demonstrates concepts such as layered architecture (Controllers, Services, DAOs), session handling, cart management, and user-friendly frontend components built with Razor Views.

## 📚 Overview

CodecoolShop is a webshop simulation that allows users to:

- Browse products by category or supplier
- Add products to the cart
- Manage quantities within the cart
- Checkout by submitting contact details
- Complete a mock payment process
- View a confirmation page

The app uses session storage to manage the cart and follows a modular architecture with services, data access layers, and Razor views.

## 🧰 Technologies Used

- ASP.NET Core MVC
- Razor Views (CSHTML)
- Entity Framework Core (in-memory DAOs in this version)
- Bootstrap (for styling)
- Newtonsoft.Json (for session serialization)

## 🚀 Features

### 🛒 Product Browsing
- Browse all products
- Filter by category or supplier
- View product details with image, price, and description

### 🛍️ Cart Management
- Add to cart from product list
- Increase/decrease quantity
- Remove items
- View total order amount

### 📦 Checkout
- Collect customer name, email, and phone
- Proceed to payment mock screen

### 💳 Payment (Mock)
- Enter card number, expiry, CVV
- Simulate successful payment
- Display order confirmation

## 💡 How to Run

1. Clone the repository
   ```bash
   git clone https://github.com/aleTomasz/Codecool.Shop.git
