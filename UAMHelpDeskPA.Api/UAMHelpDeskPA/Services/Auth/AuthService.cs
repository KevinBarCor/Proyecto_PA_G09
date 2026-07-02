using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BCrypt.Net;
using UamHelpDeskPA.Api.DTOs;
using UamHelpDeskPA.Api.Interfaces;
using UamHelpDeskPA.Api.Models;
using UamHelpDeskPA.Api.Services.Auth;
using Microsoft.Extensions.Localization;

namespace UamHelpDeskPA.Api.Services.Auth;

public class AuthService : IAuthService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IConfiguration _configuration;
    private readonly IEmailService _emailService;
    private readonly IStringLocalizer<AuthService> _localizer;
    public AuthService(
    IUnitOfWork unitOfWork,
    IConfiguration configuration,
    IEmailService emailService,
    IStringLocalizer<AuthService> localizer)
    {
        _unitOfWork = unitOfWork;
        _configuration = configuration;
        _emailService = emailService;
        _localizer = localizer;
    }
    private ApiOperationResultDto<object> ErrorResult(
    int statusCode,
    string message)
    {
        return new ApiOperationResultDto<object>
        {
            Success = false,
            Code = statusCode.ToString(),
            Message = message
        };
    }
    private ApiOperationResultDto<object> SuccessResult(
    string message)
    {
        return new ApiOperationResultDto<object>
        {
            Success = true,
            Code = StatusCodes.Status200OK.ToString(),
            Message = message
        };
    }
    public async Task<ApiOperationResultDto<LoginOtpResponseDto>> LoginAsync(LoginRequestDto request)
    {
        var user = await _unitOfWork.Users.GetByEmailAsync(request.Email);

        if (user == null || !user.IsActive)
        {
            return new ApiOperationResultDto<LoginOtpResponseDto>
            {
                Success = false,
                Code = StatusCodes.Status401Unauthorized.ToString(),
                Message = _localizer["InvalidCredentials"]
            };
        }

        if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
        {
            return new ApiOperationResultDto<LoginOtpResponseDto>
            {
                Success = false,
                Code = StatusCodes.Status401Unauthorized.ToString(),
                Message = "Credenciales inválidas"
            };
        }

        var otp = GenerateOtp();

        var otpCode = new OtpCode
        {
            UserId = user.Id,
            Code = otp,
            ExpiresAtUtc = DateTime.UtcNow.AddMinutes(
    _configuration.GetValue<int>("OtpExpirationMinutes")),
            IsUsed = false,
            CreatedAtUtc = DateTime.UtcNow
        };

        var session = new PendingSession
        {
            UserId = user.Id,
            SessionToken = Guid.NewGuid().ToString(),
            ExpiresAtUtc = DateTime.UtcNow.AddMinutes(
    _configuration.GetValue<int>("SessionTokenExpirationMinutes")),
            IsUsed = false,
            CreatedAtUtc = DateTime.UtcNow
        };

        await _unitOfWork.OtpCodes.AddAsync(otpCode);

        await _unitOfWork.PendingSessions.AddAsync(session);

        await _unitOfWork.SaveChangesAsync();

        var emailSent =
    await _emailService.SendEmailAsync(
        user.Email,
        "Código de verificación UAM Help Desk",
        $"Su código OTP es: <b>{otp}</b>");

        if (!emailSent)
        {
            return new ApiOperationResultDto<LoginOtpResponseDto>
            {
                Success = false,
                Code = StatusCodes.Status500InternalServerError.ToString(),
                Message = _localizer["EmailSendError"]
            };
        }

        return new ApiOperationResultDto<LoginOtpResponseDto>
        {
            Success = true,
            Code = StatusCodes.Status200OK.ToString(),
            Message = _localizer["OtpSent"],
            Result = new LoginOtpResponseDto
            {
                SessionToken = session.SessionToken
            }
        };
    }
    public async Task<ApiOperationResultDto<object>> ResetPasswordAsync(
        ResetPasswordRequestDto request)
    {
        var passwordResetRequest = await _unitOfWork.PasswordResetRequests
            .GetValidRequestAsync(request.SessionToken, request.Code);

        if (passwordResetRequest == null)
        {
            return ErrorResult(
                StatusCodes.Status400BadRequest,
                _localizer["InvalidPasswordResetRequest"]);
        }

        var user = await _unitOfWork.Users.GetByIdAsync(passwordResetRequest.UserId);

        if (user == null || !user.IsActive)
        {
            return ErrorResult(
                StatusCodes.Status400BadRequest,
                _localizer["InvalidPasswordResetRequest"]);
        }

        if (request.NewPassword != request.ConfirmPassword)
        {
            return ErrorResult(
                StatusCodes.Status400BadRequest,
                _localizer["PasswordsDoNotMatch"]);
        }

        if (BCrypt.Net.BCrypt.Verify(request.NewPassword, user.PasswordHash))
        {
            return ErrorResult(
                StatusCodes.Status400BadRequest,
                _localizer["PasswordMustBeDifferent"]);
        }

        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);

        await _unitOfWork.PasswordResetRequests
            .MarkAsUsedAsync(passwordResetRequest);

        await _unitOfWork.RefreshTokens
            .RevokeAllByUserIdAsync(
                user.Id,
                _localizer["PasswordResetRevocationReason"]);

        await _unitOfWork.SaveChangesAsync();

        return SuccessResult(_localizer["PasswordResetSuccess"]);
    }

    public async Task<ApiOperationResultDto<object>> ChangePasswordAsync(
    int userId,
    string currentRefreshToken,
    ChangePasswordRequestDto request)
    {
        var user = await _unitOfWork.Users.GetByIdAsync(userId);

        if (user == null || !user.IsActive)
        {
            return ErrorResult(
                StatusCodes.Status400BadRequest,
                _localizer["UserNotFound"]);
        }

        if (!BCrypt.Net.BCrypt.Verify(request.CurrentPassword, user.PasswordHash))
        {
            return ErrorResult(
                StatusCodes.Status400BadRequest,
                _localizer["InvalidCurrentPassword"]);
        }

        if (request.CurrentPassword == request.NewPassword)
        {
            return ErrorResult(
                StatusCodes.Status400BadRequest,
                _localizer["PasswordMustBeDifferent"]);
        }

        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);

        await _unitOfWork.RefreshTokens
            .RevokeAllExceptCurrentAsync(
                user.Id,
                currentRefreshToken,
                _localizer["PasswordChangeRevocationReason"]);

        await _unitOfWork.SaveChangesAsync();

        return SuccessResult(_localizer["PasswordChangeSuccess"]);
    }

    public async Task<ApiOperationResultDto<List<MySessionDto>>> GetMySessionsAsync(
        int userId)
    {
        var sessions = await _unitOfWork.RefreshTokens
            .GetActiveSessionsByUserIdAsync(userId);

        var result = sessions.Select(x => new MySessionDto(
            x.Id,
            x.Token,
            x.CreatedAtUtc,
            x.ExpiresAtUtc,
            x.RevokedAtUtc,
            x.RevokedReason
        )).ToList();

        return new ApiOperationResultDto<List<MySessionDto>>
        {
            Success = true,
            Code = StatusCodes.Status200OK.ToString(),
            Message = _localizer["SessionsRetrieved"],
            Result = result
        };
    }

    public async Task<ApiOperationResultDto<object>> RevokeSessionAsync(
        int userId,
        int refreshTokenId)
    {
        var token = await _unitOfWork.RefreshTokens
            .GetByIdAsync(refreshTokenId);

        if (token == null)
        {
            return ErrorResult(
                StatusCodes.Status404NotFound,
                _localizer["SessionNotFound"]);
        }

        if (token.UserId != userId)
        {
            return ErrorResult(
                StatusCodes.Status403Forbidden,
                _localizer["UnauthorizedSessionAccess"]);
        }

        if (token.IsRevoked)
        {
            return ErrorResult(
                StatusCodes.Status400BadRequest,
                _localizer["SessionAlreadyRevoked"]);
        }

        await _unitOfWork.RefreshTokens.RevokeByIdAsync(
            token,
            _localizer["ManualSessionRevocationReason"]);

        await _unitOfWork.SaveChangesAsync();

        return SuccessResult(_localizer["SessionRevoked"]);
    }

    public async Task<ApiOperationResultDto<object>> RevokeAllSessionsAsync(
        int userId)
    {
        var user = await _unitOfWork.Users.GetByIdAsync(userId);

        if (user == null || !user.IsActive)
        {
            return ErrorResult(
                StatusCodes.Status400BadRequest,
                _localizer["UserNotFound"]);
        }

        await _unitOfWork.RefreshTokens
            .RevokeAllByUserIdAsync(
                userId,
                _localizer["ManualBulkSessionRevocationReason"]);

        await _unitOfWork.SaveChangesAsync();

        return SuccessResult(_localizer["AllSessionsRevoked"]);
    }
    public async Task<ApiOperationResultDto<LoginResponseDto>>
    VerifyOtpAsync(VerifyOtpRequestDto request)
    {
        var session = await _unitOfWork.PendingSessions
            .GetByTokenAsync(request.SessionToken);

        if (session == null ||
            session.IsUsed ||
            session.ExpiresAtUtc < DateTime.UtcNow)
        {
            return new ApiOperationResultDto<LoginResponseDto>
            {
                Success = false,
                Code = StatusCodes.Status401Unauthorized.ToString(),
                Message = "Sesión inválida."
            };
        }

        var otp = await _unitOfWork.OtpCodes
            .GetValidOtpAsync(
                session.UserId,
                request.Code);

        if (otp == null ||
            otp.ExpiresAtUtc < DateTime.UtcNow)
        {
            return new ApiOperationResultDto<LoginResponseDto>
            {
                Success = false,
                Code = StatusCodes.Status401Unauthorized.ToString(),
                Message = "Código OTP inválido."
            };
        }

        otp.IsUsed = true;
        session.IsUsed = true;

        var user = await _unitOfWork.Users
            .GetByIdAsync(session.UserId);

        if (user == null)
        {
            return new ApiOperationResultDto<LoginResponseDto>
            {
                Success = false,
                Code = StatusCodes.Status401Unauthorized.ToString(),
                Message = _localizer["UserNotFound"]
            };
        }

        var accessToken = GenerateJwtToken(user);

        var refreshToken = new RefreshToken
        {
            UserId = user.Id,
            Token = Guid.NewGuid().ToString(),
            ExpiresAtUtc = DateTime.UtcNow.AddDays(
                _configuration.GetValue<int>(
                    "Jwt:RefreshTokenExpirationDays")),
            IsRevoked = false,
            CreatedAtUtc = DateTime.UtcNow
        };

        await _unitOfWork.RefreshTokens
            .AddAsync(refreshToken);

        await _unitOfWork.SaveChangesAsync();

        return new ApiOperationResultDto<LoginResponseDto>
        {
            Success = true,
            Code = StatusCodes.Status200OK.ToString(),
            Message = _localizer["LoginSuccess"],
            Result = new LoginResponseDto
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken.Token,
                ExpiresIn =
                    _configuration.GetValue<int>(
                        "Jwt:TokenExpirationMinutes") * 60
            }
        };
    }

    public async Task<ApiOperationResultDto<LoginResponseDto>> RefreshTokenAsync(RefreshTokenRequestDto request)
    {
        var token = await _unitOfWork.RefreshTokens.GetByTokenAsync(request.RefreshToken);

        if (token == null || token.IsRevoked || token.ExpiresAtUtc < DateTime.UtcNow)
        {
            return new ApiOperationResultDto<LoginResponseDto>
            {
                Success = false,
                Code = StatusCodes.Status401Unauthorized.ToString(),
                Message = _localizer["InvalidRefreshToken"]
            };
        }

        token.IsRevoked = true;

        var user = await _unitOfWork.Users.GetByIdAsync(token.UserId);

        if (user == null)
        {
            return new ApiOperationResultDto<LoginResponseDto>
            {
                Success = false,
                Code = StatusCodes.Status401Unauthorized.ToString(),
                Message = "Usuario no encontrado."
            };
        }

        var newAccessToken = GenerateJwtToken(user);

        var newRefreshToken = new RefreshToken
        {
            UserId = user.Id,
            Token = Guid.NewGuid().ToString(),
            ExpiresAtUtc = DateTime.UtcNow.AddDays(
                _configuration.GetValue<int>("Jwt:RefreshTokenExpirationDays")),
            IsRevoked = false,
            CreatedAtUtc = DateTime.UtcNow
        };

        await _unitOfWork.RefreshTokens.AddAsync(newRefreshToken);
        await _unitOfWork.SaveChangesAsync();

        return new ApiOperationResultDto<LoginResponseDto>
        {
            Success = true,
            Code = StatusCodes.Status200OK.ToString(),
            Message = _localizer["TokenRenewed"],
            Result = new LoginResponseDto
            {
                AccessToken = newAccessToken,
                ExpiresIn = _configuration.GetValue<int>("Jwt:TokenExpirationMinutes") * 60,
                RefreshToken = newRefreshToken.Token
            }
        };
    }

    public async Task<ApiOperationResultDto<object>> LogoutAsync(LogoutRequestDto request)
    {
        var token = await _unitOfWork.RefreshTokens.GetByTokenAsync(request.RefreshToken);

        if (token == null)
        {
            return new ApiOperationResultDto<object>
            {
                Success = false,
                Code = StatusCodes.Status400BadRequest.ToString(),
                Message = _localizer["InvalidToken"]
            };
        }

        token.IsRevoked = true;
        await _unitOfWork.SaveChangesAsync();

        return new ApiOperationResultDto<object>
        {
            Success = true,
            Code = StatusCodes.Status200OK.ToString(),
            Message = _localizer["LogoutSuccess"]
        };
    }

    private string GenerateJwtToken(User user)
    {
        var issuer = _configuration["Jwt:Issuer"]!;
        var audience = _configuration["Jwt:Audience"]!;
        var secretKey = _configuration["Jwt:SecretKey"]!;
        var expirationMinutes = _configuration.GetValue<int>("Jwt:TokenExpirationMinutes");

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim("UserId", user.Id.ToString()),
            new(ClaimTypes.Email, user.Email),
            new(ClaimTypes.Role, user.Role.Name)
        };

        var token = new JwtSecurityToken(
            issuer,
            audience,
            claims,
            expires: DateTime.UtcNow.AddMinutes(expirationMinutes),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
    public async Task<ApiOperationResultDto<object>> ForgotPasswordAsync(
    ForgotPasswordRequestDto request)
    {

        var user = await _unitOfWork.Users.GetByEmailAsync(request.Email);

        if (user != null && user.IsActive)
        {
            await _unitOfWork.PasswordResetRequests
                .InvalidatePendingRequestsAsync(user.Id);

            var otp = GenerateOtp();

            var passwordResetRequest = new PasswordResetRequest
            {
                UserId = user.Id,
                SessionToken = Guid.NewGuid().ToString(),
                Code = otp,
                ExpiresAtUtc = DateTime.UtcNow.AddMinutes(
                    _configuration.GetValue<int>("PasswordReset:CodeExpirationMinutes")),
                IsUsed = false,
                CreatedAtUtc = DateTime.UtcNow
            };

            await _unitOfWork.PasswordResetRequests
                .AddAsync(passwordResetRequest);

            await _unitOfWork.SaveChangesAsync();
            var resetUrl =
            $"{_configuration["Mvc:BaseUrl"].TrimEnd('/')}/Auth/ResetPassword?sessionToken={passwordResetRequest.SessionToken}";


            var emailSent = await _emailService.SendEmailAsync(
                user.Email,
                _localizer["PasswordResetEmailSubject"],
                _localizer["PasswordResetEmailBody", otp, resetUrl]);

            if (!emailSent)
            {
                return new ApiOperationResultDto<object>
                {
                    Success = false,
                    Code = StatusCodes.Status500InternalServerError.ToString(),
                    Message = _localizer["PasswordResetEmailError"]
                };
            }
        }

        return new ApiOperationResultDto<object>
        {
            Success = true,
            Code = StatusCodes.Status200OK.ToString(),
            Message = _localizer["ForgotPasswordResponse"]
        };
    }
    private string GenerateOtp()
    {
        return System.Security.Cryptography
            .RandomNumberGenerator
            .GetInt32(100000, 1000000)
            .ToString();
    }
}