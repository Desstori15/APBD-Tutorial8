using APBD_Tutorial8.Models.DTOs;

namespace APBD_Tutorial8.Services;

public interface ITripsService
{
    Task<List<TripDTO>> GetTrips();
}