using System;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;
using static System.Net.WebRequestMethods;

class OtpSender
{

    private readonly string toNumber;
    public OtpSender(string toNumber)
    {

        this.toNumber = toNumber;

    }

    public void SendOtp(string toNumber)
    {
        Random rnd = new Random();
        int Otp = rnd.Next(100000, 999999);

        var accountSid = "AC080d88413f87163109a58d2302df0a73";
        var authToken = "0bcfdfef2d2ba2298fe8d3a3846042b4";
        TwilioClient.Init(accountSid, authToken);

        var messageOptions = new CreateMessageOptions( new PhoneNumber(toNumber));
        messageOptions.From = new PhoneNumber("+16073605409");
        messageOptions.Body = "Your OTP is: " + Otp.ToString();


        var message = MessageResource.Create(messageOptions);
        Console.WriteLine(message.Body);
    }
}
