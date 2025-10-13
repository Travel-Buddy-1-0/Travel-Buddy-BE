using BusinessObject.DTOs;
using Microsoft.Extensions.Options;
using Net.payOS;
using Net.payOS.Types;
using System.Security.Cryptography;
using System.Text;

public class PayOsService
{
    private readonly PayOS _payOS;
    private readonly string _returnUrl;
    private readonly string _cancelUrl;
    private readonly string _checksumKey;

    public PayOsService(IOptions<PayOsSettings> options)
    {
        if (options.Value == null)
        {
            throw new InvalidOperationException("PayOS configuration section is missing or invalid.");
        }
        var settings = options.Value;
        _checksumKey = settings.ChecksumKey ??
                       throw new InvalidOperationException("PayOS ChecksumKey is NULL. Check appsettings.json/user secrets.");

        var clientId = settings.ClientId ??
                       throw new InvalidOperationException("PayOS ClientId is NULL. Check appsettings.json/user secrets.");

        var apiKey = settings.ApiKey ??
                     throw new InvalidOperationException("PayOS ApiKey is NULL. Check appsettings.json/user secrets.");

        _returnUrl = settings.ReturnUrl;
        _cancelUrl = settings.CancelUrl;

        // Nếu ChecksumKey bị null, lỗi sẽ xảy ra ở dòng trên, ngăn việc khởi tạo PayOS với khóa rỗng.
        _payOS = new PayOS(clientId, apiKey, _checksumKey);
    }

    public async Task<string> CreatePaymentLink(string description, int amount, long orderCode)
    {
        //var orderCodeLong = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

        var items = new List<ItemData>
        {
            new ItemData(description, 1, amount)
        };

        var paymentRequest = new PaymentData(
            orderCode: orderCode,
            amount: amount,
            description: description,
            items: items,
            returnUrl: _returnUrl,
            cancelUrl: _cancelUrl
        );

        var paymentLink = await _payOS.createPaymentLink(paymentRequest);
        return paymentLink.checkoutUrl;
    }

    public async Task ConfigWebhookUrl(string newWebhookUrl)
    {
        try
        {
            // newWebhookUrl phải là đường dẫn công khai (Public HTTPS URL) của Endpoint bạn đã tạo ở Bước 1
            string result = await _payOS.confirmWebhook(newWebhookUrl);

            // `result` sẽ là một chuỗi JSON chứa kết quả xác thực và cấu hình
            Console.WriteLine($"Webhook confirmation result: {result}");

            // Nếu thành công, URL Webhook của bạn sẽ được thiết lập hoặc cập nhật.
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error confirming webhook: {ex.Message}");
        }
    }
    public bool VerifySignature(string rawBody, string signature)
    {
        using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(_checksumKey));
        var hashBytes = hmac.ComputeHash(Encoding.UTF8.GetBytes(rawBody));
        var computedSignature = BitConverter.ToString(hashBytes).Replace("-", "").ToLower();

        return computedSignature == signature.ToLower();
    }
}