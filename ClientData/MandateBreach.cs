using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace M4.ClientData
{
    [Serializable]
    class MandateBreach
    {
        public Mandate mandate;



        public MandateBreach()
        {
            mandate = new Mandate();
        }
    }
}
