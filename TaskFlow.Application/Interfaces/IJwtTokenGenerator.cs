using TaskFlow.Domain.Entities;

namespace TaskFlow.Application.Interfaces;

public interface IJwtTokenGenerator
{
    (string Token, DateTime ExpiresAt) Generate(User user);
}