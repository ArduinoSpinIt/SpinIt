using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

/*this is the entrance screen - where we need to pick a player ,one of 2 (for the right bluetooth connection),
and a user name*/
namespace SpinItApp
{
	public partial class MainPage : ContentPage
	{
        /*the picker list*/
        List<string> Players = new List<string> { "player1", "player2" };

        public MainPage()
		{
            InitializeComponent();
            foreach (string p in Players)
            {
                picker.Items.Add(p);
            }
        }

        private double width = 0;
        private double height = 0;

        /*this method is called upon a screen rotation - and it initializes the right properties for each mode */
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

                    this.logo.Margin = new Thickness(4);
                    this.picker.Margin = new Thickness(3, 0, 3, 0);
                    this.nameEntry.Margin = new Thickness(5);
                    this.doneButton.Margin = new Thickness(3);
                    this.BackgroundImage = "backkk.png";




                }
                else //portrait mode
                {
                    this.logo.Margin = new Thickness(20);
                    this.picker.Margin = new Thickness(10);
                    this.nameEntry.Margin = new Thickness(10);
                    this.doneButton.Margin = new Thickness(20);
                    this.BackgroundImage = "backk_port.png";

                }
            }
        }

        /*invoked when done button is clicked*/
        private async void onDoneButtonClicked(object sender, EventArgs e)
        {
            if (picker.SelectedIndex == -1 && nameEntry.Text == "")
            {
                await DisplayAlert("Missing Fields!", "Please fill all the fields", "Ok");
            }
            else if (picker.SelectedIndex == -1)
            {
                await DisplayAlert("Missing Fields!", "Player Number Not Selected", "Ok");
            }
            else if (nameEntry.Text == "")
            {
                await DisplayAlert("Missing Fields!", "No Player Name", "Ok");
            }
            else
            {
                string selectedItem = picker.Items[picker.SelectedIndex];
                var specificPlayerP = new specificPlayerPage(selectedItem, nameEntry.Text);
                await Navigation.PushModalAsync(specificPlayerP);
            }
        }

    }
}
