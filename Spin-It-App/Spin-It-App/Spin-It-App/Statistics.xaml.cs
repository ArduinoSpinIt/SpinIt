
using System;
using System.Collections.Generic;
using System.Text;
using Microcharts;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using SkiaSharp;


namespace Spin_It_App
{

[XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Statistics : ContentPage
	{
        List<Microcharts.Entry> entries = new List<Microcharts.Entry>();
            
        public Statistics ()

		{
            InitializeComponent();
            CreateEntriesList();
            this.chartView.Chart = new LineChart() { Entries = entries };

        }

        void CreateEntriesList()
        {
            //TODO - collect data from cloud here
            //for i in data from cloud  - add entry to list in the following format :
            var ent = new Microcharts.Entry(200)
            {
                Label = "22.4",
                ValueLabel = "200 M",
                Color = SKColor.Parse("#3498db")
            };
            this.entries.Add(ent);
            ent = new Microcharts.Entry(248)
            {
                Label = "26.4",
                ValueLabel = "248 M",
                Color = SKColor.Parse("#3498db")
            };
            this.entries.Add(ent);
            ent = new Microcharts.Entry(1280)
            {
                Label = "30.4",
                ValueLabel = "1280 M",
                Color = SKColor.Parse("#3498db")
            };
            this.entries.Add(ent);
            ent = new Microcharts.Entry(1340)
            {
                Label = "2.5",
                ValueLabel = "1340 M",
                Color = SKColor.Parse("#3498db")
            };
            this.entries.Add(ent);
        }


    }


}