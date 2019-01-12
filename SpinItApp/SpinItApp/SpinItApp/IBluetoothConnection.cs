using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SpinItApp
{
    public interface IBluetoothConnection
    {
        /**
         * sets a property name "connectionName" which represents the device name to be connected to
         */
        void SetConnectionName(string name);

        /**
         * tries to connect to the device named "connectionName".
         * On success, returns: "success".
         * On Failure, returns: "Bluetooth Is Not Available"/"Bluetooth Is Not Enabled"/"No Paired Device Named "/"Could Not Connect To -connectionName-"
         */
        Task<string> Connect();

        /**
         * tries to disconnect from the device.
         * On success, returns: "success".
         * On Failure, returns the string represents the thrown exception.
         */
        Task<string> Disconnect();

        /**
         * sends to the arduino via bluetooth the string "data"/"end" which represents the wanted command.
         */
        bool SendDataOrEndCommand(string command);

        /**
         * If there are available bytes to read from the bluetooth buffer, reads one byte (which is a char)
         * and returns it as a string.
         * If there is nothing to read, returns "".
         */
        string RecieveDataOneByte();
    }
}
