using System;
using System.Collections.Generic;
using System.Text;
using Logger;

namespace ScreenActivator.Buisness
{
    public class Logging
    {
        public ILogger Log
        {
            get { return Logger.Log.Logger; }
        }
    }
}
