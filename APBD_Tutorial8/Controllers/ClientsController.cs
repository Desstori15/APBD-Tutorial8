using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using APBD_Tutorial8.Models.DTOs;

namespace APBD_Tutorial8.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ClientsController : ControllerBase
    {
        private readonly string _connectionString;

        public ClientsController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        // GET /api/trips: Retrieve all available trips with their basic information
        [HttpGet("/api/trips")]
        public IActionResult GetAllTrips()
        {
            var trips = new List<object>();
            using (var connection = new SqlConnection(_connectionString))
            using (var command = new SqlCommand("SELECT TripId, Name, Description, DateFrom, DateTo, MaxPeople, Country FROM Trip JOIN Country ON Trip.CountryId = Country.CountryId", connection))
            {
                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        trips.Add(new
                        {
                            TripId = reader.GetInt32(0),
                            Name = reader.GetString(1),
                            Description = reader.GetString(2),
                            DateFrom = reader.GetDateTime(3),
                            DateTo = reader.GetDateTime(4),
                            MaxPeople = reader.GetInt32(5),
                            Country = reader.GetString(6)
                        });
                    }
                }
            }
            return Ok(trips);
        }

        // GET /api/clients/{id}/trips: Retrieve all trips associated with a specific client
        [HttpGet("/api/clients/{id}/trips")]
        public IActionResult GetClientTrips(int id)
        {
            var trips = new List<object>();
            using (var connection = new SqlConnection(_connectionString))
            using (var command = new SqlCommand("SELECT Trip.TripId, Name, Description, DateFrom, DateTo, MaxPeople, Country, PaymentDate, RegisteredAt FROM Client_Trip JOIN Trip ON Client_Trip.TripId = Trip.TripId JOIN Country ON Trip.CountryId = Country.CountryId WHERE ClientId = @ClientId", connection))
            {
                command.Parameters.AddWithValue("@ClientId", id);
                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    if (!reader.HasRows)
                        return NotFound("Client not found or has no trips.");

                    while (reader.Read())
                    {
                        trips.Add(new
                        {
                            TripId = reader.GetInt32(0),
                            Name = reader.GetString(1),
                            Description = reader.GetString(2),
                            DateFrom = reader.GetDateTime(3),
                            DateTo = reader.GetDateTime(4),
                            MaxPeople = reader.GetInt32(5),
                            Country = reader.GetString(6),
                            PaymentDate = reader.IsDBNull(7) ? (DateTime?)null : reader.GetDateTime(7),
                            RegisteredAt = reader.GetDateTime(8)
                        });
                    }
                }
            }
            return Ok(trips);
        }

        // POST /api/clients: Create a new client record
        [HttpPost("/api/clients")]
        public IActionResult CreateClient([FromBody] ClientDto client)
        {
            if (string.IsNullOrEmpty(client.FirstName) || string.IsNullOrEmpty(client.LastName) || string.IsNullOrEmpty(client.Email) || string.IsNullOrEmpty(client.Telephone) || string.IsNullOrEmpty(client.Pesel))
                return BadRequest("All fields are required.");

            int newClientId;
            using (var connection = new SqlConnection(_connectionString))
            using (var command = new SqlCommand("INSERT INTO Client (FirstName, LastName, Email, Telephone, Pesel) OUTPUT INSERTED.ClientId VALUES (@FirstName, @LastName, @Email, @Telephone, @Pesel)", connection))
            {
                command.Parameters.AddWithValue("@FirstName", client.FirstName);
                command.Parameters.AddWithValue("@LastName", client.LastName);
                command.Parameters.AddWithValue("@Email", client.Email);
                command.Parameters.AddWithValue("@Telephone", client.Telephone);
                command.Parameters.AddWithValue("@Pesel", client.Pesel);
                connection.Open();
                newClientId = (int)command.ExecuteScalar();
            }
            return Created($"/api/clients/{newClientId}", new { ClientId = newClientId });
        }

        // PUT /api/clients/{id}/trips/{tripId}: Register a client for a specific trip
        [HttpPut("/api/clients/{id}/trips/{tripId}")]
        public IActionResult RegisterClientToTrip(int id, int tripId)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                using (var checkCommand = new SqlCommand("SELECT COUNT(*) FROM Client WHERE ClientId = @ClientId", connection))
                {
                    checkCommand.Parameters.AddWithValue("@ClientId", id);
                    if ((int)checkCommand.ExecuteScalar() == 0)
                        return NotFound("Client not found.");
                }

                using (var checkCommand = new SqlCommand("SELECT COUNT(*) FROM Trip WHERE TripId = @TripId", connection))
                {
                    checkCommand.Parameters.AddWithValue("@TripId", tripId);
                    if ((int)checkCommand.ExecuteScalar() == 0)
                        return NotFound("Trip not found.");
                }

                using (var checkCommand = new SqlCommand("SELECT COUNT(*) FROM Client_Trip WHERE TripId = @TripId", connection))
                {
                    checkCommand.Parameters.AddWithValue("@TripId", tripId);
                    int participants = (int)checkCommand.ExecuteScalar();

                    using (var maxCommand = new SqlCommand("SELECT MaxPeople FROM Trip WHERE TripId = @TripId", connection))
                    {
                        maxCommand.Parameters.AddWithValue("@TripId", tripId);
                        int max = (int)maxCommand.ExecuteScalar();

                        if (participants >= max)
                            return BadRequest("Maximum number of participants reached.");
                    }
                }

                using (var insertCommand = new SqlCommand("INSERT INTO Client_Trip (ClientId, TripId, RegisteredAt) VALUES (@ClientId, @TripId, GETDATE())", connection))
                {
                    insertCommand.Parameters.AddWithValue("@ClientId", id);
                    insertCommand.Parameters.AddWithValue("@TripId", tripId);
                    insertCommand.ExecuteNonQuery();
                }
            }
            return Ok("Client registered to trip.");
        }

        // DELETE /api/clients/{id}/trips/{tripId}: Remove a client's registration from a trip
        [HttpDelete("/api/clients/{id}/trips/{tripId}")]
        public IActionResult RemoveClientFromTrip(int id, int tripId)
        {
            using (var connection = new SqlConnection(_connectionString))
            using (var checkCommand = new SqlCommand("SELECT COUNT(*) FROM Client_Trip WHERE ClientId = @ClientId AND TripId = @TripId", connection))
            {
                checkCommand.Parameters.AddWithValue("@ClientId", id);
                checkCommand.Parameters.AddWithValue("@TripId", tripId);
                connection.Open();

                if ((int)checkCommand.ExecuteScalar() == 0)
                    return NotFound("Registration not found.");

                using (var deleteCommand = new SqlCommand("DELETE FROM Client_Trip WHERE ClientId = @ClientId AND TripId = @TripId", connection))
                {
                    deleteCommand.Parameters.AddWithValue("@ClientId", id);
                    deleteCommand.Parameters.AddWithValue("@TripId", tripId);
                    deleteCommand.ExecuteNonQuery();
                }
            }
            return Ok("Client removed from trip.");
        }
    }
}