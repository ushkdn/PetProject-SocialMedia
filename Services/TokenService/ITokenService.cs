﻿namespace SocialNetwork.Services.TokenService
{
    public interface ITokenService
    {
        Task<ServiceResponse<string>> RefreshToken();
        RefreshToken CreateRefreshToken(int userId);
        Task SetRefreshToken(RefreshToken newRefreshToken, MetaData metaData);
        string CreateToken(MetaData metaData);
    }
}
