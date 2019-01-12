using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;


namespace Spin_It_App
{
    public partial class MainPage : ContentPage
    {
        //public event EventHandler Clicked;
        public MainPage()
        {
            InitializeComponent();

            /*void ShowScore(object o, EventArgs e)
            {
                var Distance = "2.5 M";
                var Time = "5 min";
                var StringAlert = "You cycled for " + Distance + "in " + Time;
                DisplayAlert("Well Done!", StringAlert, "OK");
            }*/
        }

        private double width = 0;
        private double height = 0;

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



                }
                else //portrait mode
                {
                    this.BackgroundImage = "backk_port.png";
                    this.buttonslayer.Orientation = StackOrientation.Vertical;
                }
            }
        }

    }
}
