using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;

namespace SpinItApp.UWP
{
    class CloseAppUWP : ICloseApp
    {
        public void closeApplication()
        {
            CoreApplication.Exit();
        }
    }
}
