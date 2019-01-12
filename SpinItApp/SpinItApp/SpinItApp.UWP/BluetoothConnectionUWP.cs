using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth.Rfcomm;
using Windows.Devices.Enumeration;
using Windows.Networking.Sockets;
using Windows.Storage.Streams;

namespace SpinItApp.UWP
{
    public class BluetoothConnectionUWP : IBluetoothConnection
    {
        #region Properties
        private string connectionName;
        public StreamSocket socket;
        public DataReader dataReader;
        public DataWriter dataWriter;
        public RfcommDeviceService service;
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
                var devices =
                      await DeviceInformation.FindAllAsync(
                        RfcommDeviceService.GetDeviceSelector(
                          RfcommServiceId.SerialPort));
                var device = devices.Single(x => x.Name == connectionName);
                service = await RfcommDeviceService.FromIdAsync(device.Id);
                socket = new StreamSocket();
                await socket.ConnectAsync(
                     service.ConnectionHostName,
                     service.ConnectionServiceName);
                dataReader = new DataReader(socket.InputStream);
                dataWriter = new DataWriter(socket.OutputStream);
                return "connected";
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                socket.Dispose();
                socket = null;
                service.Dispose();
                service = null;
                return "error occured";
            }
        }

        public async Task<string> Disconnect()
        {
            try
            {
                await socket.CancelIOAsync();
                socket.Dispose();
                socket = null;
                service.Dispose();
                service = null;
                return "disconnected";
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return "error occured";
            }
        }


        public bool SendDataOrEndCommand(string command)
        {
            try
            {
                var writer = dataWriter;
                if (command == "data")
                {
                    writer.WriteString("data");
                }
                else
                {
                    writer.WriteString("end");
                }
                if (writer == null)
                {
                    Debug.WriteLine("you have a problem with the writer");
                    return false;
                }
                var store = writer.StoreAsync();
                return true;
            }
            catch (Exception ex)
            {
                if (socket != null)
                {
                    Debug.Write(ex.Message);
                }
                else
                {
                    Debug.Write("Socket is null");
                }
                return false;
            }
        }

        public string RecieveDataOneByte()
        {
            DataReader bluetoothReader = dataReader;
            //var byteToRead = await bluetoothReader.LoadAsync(1);
            var byteToRead = bluetoothReader.ReadByte();
            //var oneByte = bluetoothReader.ReadString(byteToRead);
            return byteToRead.ToString();
        }

        #endregion
    }
}
