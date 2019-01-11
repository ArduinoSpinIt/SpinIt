using System.Collections.Generic;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System.Collections.ObjectModel;

namespace Spin_It_App
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class list : ContentPage
	{
		public list ()
		{
			InitializeComponent ();
            listView.ItemTemplate = new DataTemplate(typeof(CustomCell));
            CloudConnection cloud = new CloudConnection();
            //TODO - get name
            string name = "tal";
            List<CloudConnection.JsonItem> itemslist = cloud.GetAllScores(name);
            ObservableCollection<CellViewModel> cellList = new ObservableCollection<CellViewModel>();

            foreach (CloudConnection.JsonItem item in itemslist)
            {
                CellViewModel cell = new CellViewModel { date=item.Date, time =item.Time, distance =item.Distance+" M" };
                
                cellList.Add(cell);
            }
            
            listView.ItemsSource = cellList;
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

    public class CellViewModel
    {
        public string date { get; set; }
        public string time { get; set; }
        public string distance { get; set; }

    }

}
