namespace TaskFlow.Application.DTOs.Auth;

public record AuthResponseDto(
    string Token,
    DateTime ExpiresAt,
    string FullName,
    string Email,
    string Role);