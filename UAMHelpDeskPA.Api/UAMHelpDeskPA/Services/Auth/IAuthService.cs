using UamHelpDeskPA.Api.DTOs;

namespace UamHelpDeskPA.Api.Services.Auth
{
    public interface IAuthService
    {
        Task<ApiOperationResultDto<LoginOtpResponseDto>>
            LoginAsync(LoginRequestDto request);

        Task<ApiOperationResultDto<LoginResponseDto>>
            VerifyOtpAsync(VerifyOtpRequestDto request);

        Task<ApiOperationResultDto<LoginResponseDto>>
            RefreshTokenAsync(RefreshTokenRequestDto request);

        Task<ApiOperationResultDto<object>>
            LogoutAsync(LogoutRequestDto request);
        Task<ApiOperationResultDto<object>>
            ForgotPasswordAsync(ForgotPasswordRequestDto request);

        Task<ApiOperationResultDto<object>>
            ResetPasswordAsync(ResetPasswordRequestDto request);

        Task<ApiOperationResultDto<object>>
            ChangePasswordAsync(
                int userId,
                string currentRefreshToken,
                ChangePasswordRequestDto request
                );

        Task<ApiOperationResultDto<List<MySessionDto>>>
            GetMySessionsAsync(int userId);

        Task<ApiOperationResultDto<object>>
            RevokeSessionAsync(int userId, int refreshTokenId);

        Task<ApiOperationResultDto<object>>
            RevokeAllSessionsAsync(int userId);
    }
}