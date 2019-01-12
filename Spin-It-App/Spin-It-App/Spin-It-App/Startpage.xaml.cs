using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Spin_It_App
{
    
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class Startpage : ContentPage
	{
		public Startpage ()
		{
            List<string> Players = new List<string> { "player1", "player2" };
            InitializeComponent();
            foreach (string p in Players)
            {
                picker.Items.Add(p);
            }
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
                    
                    this.logo.Margin = new Thickness(5);
                    this.picker.Margin = new Thickness(5, 0, 5, 0);
                    this.nameEntry.Margin = new Thickness(5);
                    this.doneButton.Margin = new Thickness(5);
                    //this.itemslayer.Orientation = StackOrientation.Vertical;
                    //this.inputlayer.Orientation = StackOrientation.Vertical;
                    this.BackgroundImage = "backkk.png";




                }
                else //portrait mode
                {
                    this.logo.Margin = new Thickness(20);
                    this.picker.Margin = new Thickness(10);
                    this.nameEntry.Margin = new Thickness(10);
                    this.doneButton.Margin = new Thickness(20);
                    //this.itemslayer.Orientation = StackOrientation.Vertical;
                    //this.inputlayer.Orientation = StackOrientation.Vertical;
                    this.BackgroundImage = "backk_port.png";

                }
            }
        }
    }
}