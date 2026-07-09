using CompanyHub.Application.Common.Interfaces;
using CompanyHub.Application.Common.Setting;
using CompanyHub.Application.Payment.DTOs;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace CompanyHub.Infrastructure.Paymob
{
    public class PayMobService : IPaymentGateway
    {
        private readonly PayMobSetting payMobSetting;
        private readonly HttpClient httpClient;

        public string IFrameId => payMobSetting.IFrameId;

        public PayMobService(IOptions<PayMobSetting> options, HttpClient httpClient)
        {
            this.payMobSetting = options.Value;
            this.httpClient = httpClient;
        }

        public async Task<string> GetAuthTokenAsync()
        {
            var body = new { api_key = payMobSetting.ApiKey };
            var response = await httpClient.PostAsJsonAsync("auth/tokens", body); // ✅ relative
            response.EnsureSuccessStatusCode();
            var res = await response.Content.ReadFromJsonAsync<JsonElement>();
            return res.GetProperty("token").GetString()!;
        }

        public async Task<string> CreateOrderAsync(string authToken, decimal amount, string orderId)
        {
            var body = new
            {
                auth_token = authToken,
                delivery_needed = false,
                merchant_order_id = orderId,
                currency = "EGP",
                items = Array.Empty<object>(),
                amount_cents = (int)(amount * 100),
            };
            var response = await httpClient.PostAsJsonAsync("ecommerce/orders", body); // ✅ relative
            response.EnsureSuccessStatusCode();
            var res = await response.Content.ReadFromJsonAsync<JsonElement>();
            return res.GetProperty("id").GetInt32().ToString();
        }

        public async Task<string> GetPaymentKeyAsync(string authToken, string paymobOrderId,
                                                      decimal amount, string billingEmail)
        {
            var body = new
            {
                auth_token = authToken,
                amount_cents = (int)(amount * 100),
                expiration = 3600,
                order_id = paymobOrderId,
                billing_data = new
                {
                    email = billingEmail,
                    first_name = "N/A",
                    last_name = "N/A",
                    phone_number = "N/A",
                    apartment = "N/A",
                    floor = "N/A",
                    street = "N/A",
                    building = "N/A",
                    shipping_method = "N/A",
                    postal_code = "N/A",
                    city = "N/A",
                    country = "N/A",
                    state = "N/A"
                },
                currency = "EGP",
                integration_id = payMobSetting.IntegrationId
            };
            var response = await httpClient.PostAsJsonAsync("acceptance/payment_keys", body); // ✅ relative
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadFromJsonAsync<JsonElement>();
            return result.GetProperty("token").GetString()!;
        }

        public bool VerifyHmac(Dictionary<string, string> callbackData, string receivedHmac)
        {
            var fields = new[]
            {
                "amount_cents", "created_at", "currency", "error_occured",
                "has_parent_transaction", "id", "integration_id", "is_3d_secure",
                "is_auth", "is_capture", "is_refunded", "is_standalone_payment",
                "is_voided", "order", "owner", "pending",
                "source_data_pan", "source_data_sub_type", "source_data_type",
                "success"
            };

            var concatenated = string.Concat(
                fields.Select(f => callbackData.GetValueOrDefault(f, "")));

            using var hmac = new HMACSHA512(Encoding.UTF8.GetBytes(payMobSetting.HmacSecret));
            var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(concatenated));
            var computed = Convert.ToHexString(hash).ToLower();

            return computed == receivedHmac.ToLower();
        }
    }
}
