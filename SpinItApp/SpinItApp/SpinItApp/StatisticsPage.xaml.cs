using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SkiaSharp;
using Microcharts;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SpinItApp
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class StatisticsPage : ContentPage
	{
        private List<Microcharts.Entry> entries = new List<Microcharts.Entry>();
        private List<Microcharts.Entry> entries2 = new List<Microcharts.Entry>();
        private string name = "";

        public StatisticsPage (string playername)
		{
			InitializeComponent ();
            name = playername;
            CreateEntriesList(name);
            if (entries.Count != 0) //there is data about the player
            {
                this.graphs.IsVisible = true;
                this.entrynone.IsVisible = false;
                this.chartView.Chart = new LineChart() { Entries = entries };
                this.chartView2.Chart = new LineChart() { Entries = entries2 };
            }
            else //when there are no user results in the cloud
            {
                this.graphs.IsVisible = false;
                this.entrynone.IsVisible = true;
            }

        }


        private async void OnBackToSpinningClicked(object sender, EventArgs e)
        {
            //The previous page will always be the specificPlayerPage
            await Navigation.PopModalAsync();
        }

        void CreateEntriesList(string name)
        {
            int ent_limit = 6;
            //collect data from cloud here
            CloudConnection Cloud = new CloudConnection();
            try
            {
                List<CloudConnection.JsonItem> scores = Cloud.GetXScores(name, ent_limit);


                foreach (CloudConnection.JsonItem item in scores)
                {
                    int numVal = Int32.Parse(item.Distance);

                    string newDate = Cloud.getDate(item);
                    Microcharts.Entry ent = new Microcharts.Entry(numVal)
                    {
                        Label = newDate,
                        ValueLabel = numVal + " M",
                        Color = SKColor.Parse("#3498db")
                    };
                    this.entries.Add(ent);

                    string currtime = item.Time;
                    Microcharts.Entry ent2 = new Microcharts.Entry(FromStringToIntTime(currtime))
                    {
                        Label = newDate,
                        ValueLabel = FromStringToIntTime(currtime).ToString() + " min",
                        Color = SKColor.Parse("#3498db")

                    };
                    this.entries2.Add(ent2);

                }
            }
            catch (Exception e)
            {
                //we got here if there is no data from the cloud
            }
        }

        private int FromStringToIntTime(string time)
        {
            string[] splitTime = time.Split(':');
            int res = Int32.Parse(splitTime[0]) * 60 + Int32.Parse(splitTime[1]);
            return res;
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
                    this.graphs.Orientation = StackOrientation.Horizontal;
                    this.chartView.WidthRequest = 280;
                    this.chartView.HeightRequest = 180;
                    this.chartView2.WidthRequest = 280;
                    this.chartView2.HeightRequest = 180;
                    this.frame1.Margin = 5;
                    this.frame2.Margin = 5;



                }
                else //portrait mode
                {
                    this.BackgroundImage = "backk_port.png";
                    this.graphs.Orientation = StackOrientation.Vertical;
                    this.chartView.WidthRequest = 300;
                    this.chartView.HeightRequest = 195;
                    this.chartView2.WidthRequest = 300;
                    this.chartView2.HeightRequest = 195;
                    this.frame1.Margin = 5;
                    this.frame2.Margin = 5;
                }
            }
        }


    }
}