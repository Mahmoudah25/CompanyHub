using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompanyHub.Application.TwoFactor.DTOs
{
    public class Enable2FAResponse
    {
        public string Secret { get; set; } = string.Empty;
        public string QrCodeImageBase64 { get; set; } = string.Empty; // <img src="data:image/png;base64,...">
        public string ManualEntryKey { get; set; } = string.Empty;    // لو مش عايز يمسح QR
    }
}
