# APBD-Tutorial8

This is a REST API project built in C# using ASP.NET Core, implementing ADO.NET to interact with a SQL Server database for a travel agency system.

## Features

- Retrieve all trips with basic information  
- Retrieve all trips associated with a specific client  
- Create a new client record  
- Register a client for a specific trip  
- Remove a client's registration from a trip

---

## Project Structure

- **Controllers** → API endpoints (`ClientsController`, `TripsController`)  
- **Models/DTOs** → Data transfer objects like `ClientDto`, `TripDto`  
- **Services** → Service layer with interfaces and implementations  
- **appsettings.json** → Configuration, including the database connection string

---

## Technologies Used

- C#  
- ASP.NET Core  
- ADO.NET (SqlConnection, SqlCommand)  
- SQL Server  
- REST API design

---

## Author

Vladislav Dobriyan
GitHub: Desstori15
