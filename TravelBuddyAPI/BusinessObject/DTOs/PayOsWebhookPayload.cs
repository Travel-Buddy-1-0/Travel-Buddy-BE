using System;
using System.Text.Json.Serialization;

namespace BusinessObject.DTOs
{
    public class WebhookDataDetail
    {
        public long orderCode { get; set; }
        public int amount { get; set; }
        public string description { get; set; }
        public string accountNumber { get; set; }
        public string reference { get; set; }
        public string transactionDateTime { get; set; }
        public string currency { get; set; }
        public string paymentLinkId { get; set; }
        public string code { get; set; }
        public string desc { get; set; }
    }

    public class PayOsWebhookPayload
    {
        public string code { get; set; }
        public string desc { get; set; }
        public bool success { get; set; }
        public WebhookDataDetail data { get; set; }
        public string signature { get; set; }

    }
}