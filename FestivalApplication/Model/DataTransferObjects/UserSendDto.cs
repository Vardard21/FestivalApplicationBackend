using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FestivalApplication.Model.DataTransferObjects
{
    public class UserSendDto
    {
        public int UserID { get; set; }
        public string UserName { get; set; }
        public string UserRole { get; set; }
    }
}
