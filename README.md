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

## How to Run

1. Clone the repository:
```bash
git clone https://github.com/Desstori15/APBD-Tutorial8.git
Navigate to the project folder:
cd APBD-Tutorial8/Tutorial8
Update appsettings.json with your database connection string:
"ConnectionStrings": {
    "DefaultConnection": "Server=YOUR_SERVER;Database=YOUR_DATABASE;User Id=YOUR_USER;Password=YOUR_PASSWORD;"
}
Run the project:
dotnet run
Access the API:
GET all trips → /api/trips
GET client trips → /api/clients/{id}/trips
POST create client → /api/clients
PUT register client to trip → /api/clients/{id}/trips/{tripId}
DELETE remove client from trip → /api/clients/{id}/trips/{tripId}
Tools for Testing

Postman
cURL
Rider or Visual Studio .http files
Author

Vladislav Dobriyan
GitHub: Desstori15
