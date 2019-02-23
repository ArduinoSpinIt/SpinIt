using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

/*this is main screen for each player - shows the time,distance and speed of the current ride .
from this screen we can move to the statistics page, the rides list page, and to start and stop a current ride*/

namespace SpinItApp
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    //public partial class specificPlayerPage : ContentPage, INotifyPropertyChanged
    public partial class specificPlayerPage : ContentPage
    {
        private string currPlayer; //used to define the bluetooth device to be connected to.
        private string playerName; 
        private IBluetoothConnection btConnection;
        private string currSpeed;
        private string currDistance;
        private string currReciever; //"D" or "S". defines if the next value to be read is distance or speed.
        private string currTime;
        private bool timerRunning; //defines if the counter (and the bluetooth thread) is running
        private bool askToQuit; //a flag which is used to end the bluetooth thrad (ends when it's true)
        private Int64 lastUpdateTime;
        private Int64 startButtonClickTime;
        private Stopwatch sw;
        private bool isBTConnected;
        private string btError;
        private CloudConnection cloud;

        /***
         * Page Constructor
         * Initializes the page and all the relevant class properties.
         * params: 
         * player - player1/player2, indicates the bluetooth device to be connected to.
         * name - the player's name
         */
        public specificPlayerPage(string player, string name)
        {
            InitializeComponent();
            currPlayer = player;
            playerName = name;
            playerNameLabel.Text = playerName;
            currDistance = "0";
            currSpeed = "0";
            timerRunning = false;
            lastUpdateTime = 0;
            askToQuit = false;
            sw = new Stopwatch();
            isBTConnected = false;
            btError = "";
            cloud = new CloudConnection();
        }

        //***********************************************************************************************************************************
        //***********************************************************************************************************************************
        //****************************************      BLUETOOTH THREAD      ***************************************************************
        //***********************************************************************************************************************************
        //***********************************************************************************************************************************

        /***
         * The Main part of the bluetooth input/output thread.
         * It sends the arduino the "data" command for it to begin sending data.
         * Then, it repeatedly calls the getWholeValue method which is responsible for reading the data from the arduino.
         * Every 100 milliSeconds it invokes a UI update method on the MainThread.
         * when the askToQuit flag is updated to false (in the MainThread), it stops reading data from the arduino,
         * sends it a "end" command, and clears the bluetooth buffer.
         */
        public void bluetoothManager()
        {
            bool result = btConnection.SendDataOrEndCommand("data");
            if (!result)
            {
                //failed sending command to arduino. exit the app
                DependencyService.Get<ICloseApp>().closeApplication();
            }
            string recieved = btConnection.RecieveDataOneByte();
            while (recieved == "")
            {
                //Currently there is Nothing to read
                recieved = btConnection.RecieveDataOneByte();
            }
            currReciever = recieved;
            while (!askToQuit)
            {
                getWholeValue();
                Int64 currTime = sw.ElapsedMilliseconds;
                if (currTime - lastUpdateTime > 100)
                {
                    lastUpdateTime = currTime;
                    Device.BeginInvokeOnMainThread(updateSpeedDistTime);
                }
            }
            result = btConnection.SendDataOrEndCommand("end");
            if (!result)
            {
                //failed sending command to arduino. exit the app
                DependencyService.Get<ICloseApp>().closeApplication();
            }
            emptyBTBuffer();
        }

        /***
         * This method keeps reading bytes from the bluetooth connection, until it has a whole value. 
         * A whole value is the string representing the current speed/current distance.
         * The data from the arduino is sent in the format "D0S0" where D indicates "distance"
         * and S indicates "speed". The numbers are the values themselves (the are not always 0).
         */
        private void getWholeValue()
        {
            string combinedStr = "";
            string currChar = btConnection.RecieveDataOneByte();
            while (currChar == "")
            {
                //Currently there is Nothing to read
                currChar = btConnection.RecieveDataOneByte();
            }
            while (currChar != "D" && currChar != "S")
            {
                combinedStr += currChar;
                currChar = btConnection.RecieveDataOneByte();
                while (currChar == "")
                {
                    //Currently there is Nothing to read
                    currChar = btConnection.RecieveDataOneByte();
                }
            }
            if (currReciever == "D")
            {
                currDistance = combinedStr;
            }
            else
            {
                currSpeed = combinedStr;
            }
            currReciever = currChar;
        }

        /***
         * This method keeps reading bytes from the bluetooth buffer until it is empty.
         */
        private void emptyBTBuffer()
        {
            string currByte = btConnection.RecieveDataOneByte();
            while (currByte != "")
            {
                currByte = btConnection.RecieveDataOneByte();
            }
        }


        //***********************************************************************************************************************************
        //***********************************************************************************************************************************
        //****************************************        MAIN THREAD         ***************************************************************
        //***********************************************************************************************************************************
        //***********************************************************************************************************************************


        /***
         * This method updates the UI: speed, distance and time labels. 
         */
        private void updateSpeedDistTime()
        {
            Int64 diff = (sw.ElapsedMilliseconds - startButtonClickTime)/1000;
            int hours = (int)(diff / 3600);
            int minutes = (int)((diff - hours*3600)/60);
            int seconds = (int)(diff - hours * 3600 - minutes * 60);
            currTime = String.Format("{0:00}:{1:00}:{2:00}", hours, minutes, seconds);
            speedLabel.Text = currSpeed;
            distanceLabel.Text = currDistance;
            timeLabel.Text = currTime;
        }

        /***
         * This method tries to connect to the bluetooth device which is defined by currPlayer.
         * if it succeeds - sets the isBTConnected flag to true.
         * otherwise - isBTConnected is false, and btError is updated with the error string.
         */
        public async void connectToBT()
        {
            string res = await btConnection.Connect();
            if (res != "connected")
            {
                isBTConnected = false;
                btError = res;
            } 
            else
            {
                isBTConnected = true;
            }
        }

        /***
         * This method is always called when the screen is appearing (right before it happens).
         * sets the bluetooth's connectionName depending on the value of currPlayer.
         * tries to connect to the device which was set.
         */
        protected override void OnAppearing()
        {
            btConnection = DependencyService.Get<IBluetoothConnection>();
            string connectTo;
            if (currPlayer == "player1")
            {
                connectTo = "spinit";
            }
            else
            {
                connectTo = "spinit2";
            }
            btConnection.SetConnectionName(connectTo);
            connectToBT();
        }


        /***
         * This method is always called when the screen is disappearing (right before it happens).
         * If the counter is running (meaning the bluetooth thread is active, and reads data from the arduino),
         * updates askToQuit to "true" in order to end the thread, and stops the counter (updated the relevant UI).
         * Always disconnects from the bluetooth device.
         */
        protected override async void OnDisappearing()
        {
            if (timerRunning == true)
            {
                //in case the counter and bluetooth are on
                askToQuit = true;
                timerRunning = false;
                startButton.Text = "start riding";
                statButton.IsVisible = true;
                myRidesButton.IsVisible = true;
                changeRiderButton.IsVisible = true;
            }
            if (isBTConnected)
            {
                await btConnection.Disconnect();
            }
        }

        /***
         * Navigates to the statistics page.
         */
        private async void StatButtonClicked(object sender, EventArgs e)
        {
            var statPage = new StatisticsPage(playerName);
            await Navigation.PushModalAsync(statPage);
        }

        /***
         * This button invokes the bluetooth thread.
         * It is used to start the counter (and the data from the arduino), and also to stop it.
         * 
         */
        private void StartButtonClicked(object sender, EventArgs e)
        {
            if (timerRunning == false)
            {
                //start riding was clicked
               
                if (!isBTConnected)
                {
                    connectToBT();
                }
                currDistance = "0";
                currSpeed = "0";
                speedLabel.Text = currSpeed;
                distanceLabel.Text = currDistance;
                timerRunning = true;
                startButton.Text = "stop riding";
                sw.Start();
                startButtonClickTime = sw.ElapsedMilliseconds;
                askToQuit = false;
                statButton.IsVisible = false;
                myRidesButton.IsVisible = false;
                changeRiderButton.IsVisible = false;
                  
                if (isBTConnected)
                {
                    //starts the bluetooth thread
                    Task.Run(() => { bluetoothManager(); });
                }
                
                else
                { 
                    //bluetooth is not connected
                    DisplayAlert("Bluetooth Error!", btError, "Ok");
                    timerRunning = false;
                    startButton.Text = "start riding";
                    statButton.IsVisible = true;
                    myRidesButton.IsVisible = true;
                    changeRiderButton.IsVisible = true;
                } 
            }

            else if (timerRunning == true)
            {
                //stop riding was clicked

                //upload score to cloud
                string date = DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss");
                cloud.AddScore(playerName, currTime, date, currDistance);              
                askToQuit = true;
                timerRunning = false;
                startButton.Text = "start riding";
                statButton.IsVisible = true;
                myRidesButton.IsVisible = true;
                changeRiderButton.IsVisible = true;

                // pop up an alert for the score acheived
                var StringAlert = "You cycled for " + currDistance + " meters in " + currTime;
                DisplayAlert("Well Done!", StringAlert, "OK");
            }
        }

        /***
         * Navigates to the "my rides" page.
         */
        private async void RidesButtonClicked(object sender, EventArgs e)
        {
            var ridesPage = new MyRidesPage(playerName);
            await Navigation.PushModalAsync(ridesPage);
        }

        /***
         * Navigates to the starting page.
         */
        private async void ChangeRiderButtonClicked(object sender, EventArgs e)
        {
            //Go back to MainPage
            await Navigation.PopModalAsync();
        }

        private double width = 0;
        private double height = 0;
        /***
        * keep the design of both orientation to work
        */
        protected override void OnSizeAllocated(double width, double height)
        {
            base.OnSizeAllocated(width, height); //must be called
            if (this.width != width || this.height != height)
            {
                this.width = width;
                this.height = height;
                //reconfigure layout
                if (this.width > this.height) // landscape mode
                {
                    this.BackgroundImage = "backkk.png";
                    this.buttonslayer.Orientation = StackOrientation.Horizontal;
                    this.playerNameLabel.Margin = new Thickness(0, 10, 0, 0);
                    this.grid.Margin = new Thickness(0, 3);





                }
                else //portrait mode
                {
                    this.playerNameLabel.Margin = new Thickness(0, 13, 0, 0);
                    this.BackgroundImage = "backk_port.png";
                    this.buttonslayer.Orientation = StackOrientation.Vertical;
                    this.grid.Margin = new Thickness(0, 10);
                }
            }
        }

    }
}
