using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;


/*this class represent a line of the "rides" list*/
namespace SpinItApp
{
   public class CustomCell : ViewCell
    {

        public CustomCell()
        {
            //instantiate each of our views
            var image = new Image();
            StackLayout cellWrapper = new StackLayout();
            StackLayout horizontalLayout = new StackLayout();
            Label date = new Label();
            Label distance = new Label();
            Label time = new Label();

            //set bindings
            date.SetBinding(Label.TextProperty, "date");
            time.SetBinding(Label.TextProperty, "time");
            distance.SetBinding(Label.TextProperty, "distance");
            image.SetBinding(Image.SourceProperty, "image");

            //Set properties for desired design
            cellWrapper.BackgroundColor = Color.FromHex("#e3f4fb");
            horizontalLayout.Orientation = StackOrientation.Horizontal;
            horizontalLayout.Margin = new Thickness(0, 5, 0, 5);
            horizontalLayout.HeightRequest = 35;
            horizontalLayout.VerticalOptions = LayoutOptions.Center;
            image.Margin = new Thickness(5, 0, 0, 0);
            date.Margin = new Thickness(10, 0);
            date.FontSize = 20;
            time.Margin = new Thickness(10, 0);
            time.FontSize = 20;
            distance.FontSize = 20;
            date.TextColor = Color.FromHex("#7a7a7a");
            time.TextColor = Color.FromHex("#008cb4");
            distance.Margin = new Thickness(10, 0);
            distance.TextColor = Color.FromHex("#585858");

            time.VerticalOptions = LayoutOptions.CenterAndExpand;
            distance.VerticalOptions = LayoutOptions.EndAndExpand;

            //values initializing
            image.Source = "wheel_small.png";


            //add views to the view hierarchy
            horizontalLayout.Children.Add(image);
            horizontalLayout.Children.Add(date);
            horizontalLayout.Children.Add(time);
            horizontalLayout.Children.Add(distance);
            cellWrapper.Children.Add(horizontalLayout);
            View = cellWrapper;
        }
        

     
    }
}
