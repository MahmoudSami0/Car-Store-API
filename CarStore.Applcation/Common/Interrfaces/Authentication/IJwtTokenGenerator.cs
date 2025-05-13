using CarStore.Domain.Entities;

namespace CarStore.Application.Common.Interrfaces;

public interface IJwtTokenGenerator
{
    Task<string> GenerateJwtToken(User user);
    RefreshToken GenerateRefreshToken();
}