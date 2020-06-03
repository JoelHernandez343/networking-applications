using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Store.Helpers.Net
{
    enum Instruction
    {
        DISCONNECT = 0,
        OK = 1,
        REQUEST_DB = 2,
        REQUEST_IMAGES = 3,
        REQUEST_ONE_IMAGE = 4,
        FINISHED_PROCESS = 5,
        POST_PURCHASE = 6
    }
}
