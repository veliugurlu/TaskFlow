using TaskFlow.Application.DTOs.Auth;
using TaskFlow.Application.Interfaces;
using TaskFlow.Domain.Entities;
using TaskFlow.Domain.Enums;

namespace TaskFlow.Application.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository _users;
    private readonly IPasswordHasher _hasher;
    private readonly IJwtTokenGenerator _tokenGenerator;

    public AuthService(
        IUserRepository users,
        IPasswordHasher hasher,
        IJwtTokenGenerator tokenGenerator)
    {
        _users = users;
        _hasher = hasher;
        _tokenGenerator = tokenGenerator;
    }

    public async Task<AuthResponseDto?> RegisterAsync(RegisterDto dto)
    {
        var email = dto.Email.Trim().ToLowerInvariant();

        if (await _users.EmailExistsAsync(email))
            return null;

        var user = new User
        {
            FullName = dto.FullName.Trim(),
            Email = email,
            PasswordHash = _hasher.Hash(dto.Password),
            Role = UserRole.Member
        };

        await _users.AddAsync(user);
        await _users.SaveChangesAsync();

        return CreateResponse(user);
    }

    public async Task<AuthResponseDto?> LoginAsync(LoginDto dto)
    {
        var email = dto.Email.Trim().ToLowerInvariant();

        var user = await _users.GetByEmailAsync(email);
        if (user is null || !_hasher.Verify(dto.Password, user.PasswordHash))
            return null;

        return CreateResponse(user);
    }

    private AuthResponseDto CreateResponse(User user)
    {
        var (token, expiresAt) = _tokenGenerator.Generate(user);
        return new AuthResponseDto(token, expiresAt, user.FullName, user.Email, user.Role.ToString());
    }
}