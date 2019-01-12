using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Bluetooth;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Views.InputMethods;
using Java.Util;
using System.IO;
using System.Runtime.CompilerServices;
using SpinItApp.Droid;

[assembly: Xamarin.Forms.Dependency(typeof(BluetoothConnectionDroid))]
namespace SpinItApp.Droid
{
    class BluetoothConnectionDroid : IBluetoothConnection
    {
        const int REQUEST_ENABLE_BT = 1;

        #region Properties
        private string connectionName;
        public BluetoothAdapter btAdapter;
        public BluetoothSocket socket;
        #endregion

        #region Methods
        public void SetConnectionName(string name)
        {
            connectionName = name;
        }


        public async Task<string> Connect()
        {
            try
            {
                btAdapter = BluetoothAdapter.DefaultAdapter;
                if (btAdapter == null)
                {
                    return "Bluetooth Is Not Available";
                }

                if (!btAdapter.IsEnabled)
                {
                    return "Bluetooth Is Not Enabled";
                }

                BluetoothDevice device = (from bd in btAdapter.BondedDevices
                                          where bd.Name == connectionName
                                          select bd).FirstOrDefault();

                if (device == null)
                {
                    return "No Paired Device Named " + connectionName;
                }
                socket = device.CreateRfcommSocketToServiceRecord(UUID.FromString("00001101-0000-1000-8000-00805f9b34fb"));
                socket.Connect();
                return "connected";
            }
            catch (Exception e)
            {
                return "Could Not Connect To " + connectionName;
            }
        }

        public async Task<string> Disconnect()
        {
            try
            {
                socket.Close();
                return "disconnected";
            }
            catch (Exception e)
            {
                return e.ToString();
            }
        }


        public bool SendDataOrEndCommand(string command)
        {
            if (command == "data")
            {
                var buffer = new byte[] { (byte)'d', (byte)'a', (byte)'t', (byte)'a' };
                socket.OutputStream.Write(buffer, 0, buffer.Length);
            }
            else
            {
                var buffer = new byte[] { (byte)'e', (byte)'n', (byte)'d' };
                socket.OutputStream.Write(buffer, 0, buffer.Length);
            }
            return true;
        }

        public string RecieveDataOneByte()
        {
            var buffer = new byte[] { 0x00 };
            if (socket.InputStream.IsDataAvailable())
            {
                int count = socket.InputStream.Read(buffer, 0, buffer.Length);
                if(count == 0)
                {
                    return "";
                }
                return ((char)buffer[0]).ToString();
            }
            return "";
        }

        #endregion
    }
}