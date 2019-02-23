using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;


/*this screen shows the previous rides list of a current user*/
namespace SpinItApp
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class MyRidesPage : ContentPage
	{
        private string name = "";
		public MyRidesPage (string playername)
		{
            CloudConnection cloud = new CloudConnection();
            InitializeComponent ();

            name = playername;
            try
            {
                List<CloudConnection.JsonItem> itemslist = cloud.GetAllScores(name);
                ObservableCollection<CellViewModel> cellList = new ObservableCollection<CellViewModel>();

                foreach (CloudConnection.JsonItem item in itemslist)
                {
                    string newDate = cloud.getDate(item);
                    CellViewModel cell = new CellViewModel { date = newDate, time = item.Time, distance = item.Distance + " M" };

                    cellList.Add(cell);
                }
                listView.ItemTemplate = new DataTemplate(typeof(CustomCell));
                listView.ItemsSource = cellList;
                this.scroll.IsVisible = true;
                this.entrynone.IsVisible = false;
            }catch (Exception e)
            {
                this.scroll.IsVisible = false;
                this.entrynone.IsVisible = true;
            }

        }

        private async void OnBackToSpinningClicked(object sender, EventArgs e)
        {
            //The previous page will always be the specificPlayerPage
            await Navigation.PopModalAsync();
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
                }
                else //portrait mode
                {
                    this.BackgroundImage = "backk_port.png";
                }
            }
        }
    }

    /*a class for the lines content*/
    public class CellViewModel
    {
        public string date { get; set; }
        public string time { get; set; }
        public string distance { get; set; }

    }
}
