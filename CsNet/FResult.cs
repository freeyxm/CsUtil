using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FNet
{
    enum FResult
    {
        Exception = -2,
        Error = -1,
        Success = 0,
        SocketError,
        SocketException,
        SocketClosed,
        WouldBlock,
    }
}
