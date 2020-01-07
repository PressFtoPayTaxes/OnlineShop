using System;
using System.Collections.Generic;
using System.Text;

namespace OnlineShop.DTO
{
    public class UserDTO
    {
        public string PhoneNumber { get; set; }
        public string FullName { get; set; }
        public string NotificationDeviceId { get; set; }
        public string VerificationCode{ get; set; }
    }
}
