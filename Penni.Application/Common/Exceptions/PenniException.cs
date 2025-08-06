using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Penni.Application.Common.Exceptions
{
    public class PenniException:Exception
    {
        public int StatusCode { get; }
        public string MessageCode { get; }
        public PenniException(int statusCode, string messageCode, string message)
        : base(message)
        {
            StatusCode = statusCode;
            MessageCode = messageCode;
        }
    }
}
