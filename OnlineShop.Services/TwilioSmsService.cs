using Microsoft.Extensions.Configuration;
using OnlineShop.DTO;
using OnlineShop.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Twilio;
using Twilio.Rest.Api.V2010.Account;

namespace OnlineShop.Services
{
    public class TwilioSmsService : ISmsService
    {
        private readonly IConfiguration configuration;

        public TwilioSmsService(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public Task<SmsServiceResponseDTO> SendVerificationCode(string phoneNumber, string code)
        {
            var accountSid = configuration.GetSection("Twilio").GetValue<string>("AccountSid");
            var authToken = configuration.GetSection("Twilio").GetValue<string>("AuthToken");
            var twilioPhoneNumber = configuration.GetSection("Twilio").GetValue<string>("PhoneNumber");

            TwilioClient.Init(accountSid, authToken);

            var message = MessageResource.Create(
                body: "Your verification code for an online shop: " + code,
                from: new Twilio.Types.PhoneNumber(twilioPhoneNumber),
                to: new Twilio.Types.PhoneNumber(phoneNumber)
            );

            if(message.ErrorCode != null)
            {
                return Task.FromResult(new SmsServiceResponseDTO
                {
                    StatusCode = (int)message.ErrorCode,
                    Message = message.ErrorMessage
                });
            }

            return Task.FromResult(new SmsServiceResponseDTO {
                StatusCode = 200,
                Message = "Сообщение успешно отправлено"
            });
        }
    }
}
