// ParticipantService/Clients/IHrManagerApi.cs

using Refit;
using Entities;

namespace ParticipantService.Clients;
public interface IHrManagerApi
{
    [Post("/api/hr_manager/participant")]
    Task<ApiResponse<ApiResponseMessage>> AddParticipantAsync([Body] ParticipantInputModel participant);
    
    [Post("/api/hr_manager/wishlist")]
    Task<ApiResponse<ApiResponseMessage>> AddWishlistAsync([Body] WishlistInputModel wishlist);
    
    [Get("/api/hr_manager/health")]
    Task<ApiResponse<HealthCheckResponse>> HealthCheckAsync();
}

public class ApiResponseMessage
{
    public string Message { get; set; }
    public int? TotalParticipants { get; set; }
    public int? TotalWishlists { get; set; }
}

public class HealthCheckResponse
{
    public string Status { get; set; }
    public int ParticipantsNumber { get; set; }
}