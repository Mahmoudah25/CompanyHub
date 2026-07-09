using CompanyHub.Application.Auth.DTOs;
using CompanyHub.Application.Auth;
using CompanyHub.Application.Common;
using CompanyHub.Application.Common.Interfaces;
using CompanyHub.Application.TwoFactor;
using CompanyHub.Application.TwoFactor.DTOs;
using Microsoft.EntityFrameworkCore;
using OtpNet;
using QRCoder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompanyHub.Infrastructure.TwoFactor
{
    public class TwoFactorService : ITwoFactorService
    {   
        private readonly IApplicationDBContext context;
        private readonly IJwtProvider provider;
        private readonly IRefreshTokenProvider refreshTokenProvider;
        public TwoFactorService(IApplicationDBContext context, IJwtProvider provider, IRefreshTokenProvider refreshTokenProvider)
        {
            this.context = context;
            this.provider = provider;
            this.refreshTokenProvider = refreshTokenProvider;
        }
       
        public async Task<Result<Enable2FAResponse>> EnableAsync(Guid userId)
        {
            var user = await context.users.FirstOrDefaultAsync(x=>x.Id == userId);
            if (user == null)
                return Result<Enable2FAResponse>.Failure(" User Not Found");
            var secretKey =  KeyGeneration.GenerateRandomKey(20);
            var base32Sccret = Base32Encoding.ToString(secretKey);
            user.TwoFactorSecret = base32Sccret;
            await context.SaveChangesAsync();
            var issuer = "CompanyHub";
            var otpUri = $"otpauth://totp/{issuer}:{user.Email}?secret={base32Sccret}&issuer={issuer}&digits=6";

            var qrGenrator = new QRCodeGenerator();
            var qrCodeData = qrGenrator.CreateQrCode(otpUri, QRCodeGenerator.ECCLevel.Q);
            var qrCode = new PngByteQRCode(qrCodeData);
            var qrBytes = qrCode.GetGraphic(20);
            var qrBase64= Convert.ToBase64String(qrBytes);

            return Result<Enable2FAResponse>.Success(new Enable2FAResponse
            {
                Secret = base32Sccret,
                QrCodeImageBase64 = qrBase64,
                ManualEntryKey = base32Sccret
            });
        }

        public bool ValidateCode(string secret, string code)
        {
            var secretBytes = Base32Encoding.ToBytes(secret);   
            var totp = new Totp(secretBytes);
            // بيقبل الكود الحالي + نافذة زمنية بسيطة (VerificationWindow) عشان فرق التوقيت بين الموبايل والسيرفر
            return totp.VerifyTotp(code, out _, new VerificationWindow(2, 2));
        }

        public async Task<Result> VerifyAsync(Guid userId, Verify2FARequest request)
        {
            var user = await context.users.FirstOrDefaultAsync(x => x.Id == userId);
            if (user == null)
                return Result.Failure(" User Not Found ");
            var isvalid = ValidateCode(user.TwoFactorSecret, request.Code);
            if (!isvalid)
                return Result.Failure(" Invalid verification ");
            user.TwoFactorEnabled = true;
            await context.SaveChangesAsync();
            return Result<Enable2FAResponse>.Success();
        }
    }
}
