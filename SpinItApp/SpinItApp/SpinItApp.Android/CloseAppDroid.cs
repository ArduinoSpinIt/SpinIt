using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Java.Lang;
using SpinItApp.Droid;
using Xamarin.Forms;

[assembly: Xamarin.Forms.Dependency(typeof(CloseAppDroid))]
namespace SpinItApp.Droid
{
    public class CloseAppDroid : ICloseApp
    {
        public void closeApplication()
        {
            JavaSystem.Exit(0);
            /*try
            {
                //var activity = (Activity)Android.App.Application.Context;
                //activity.FinishAffinity();
                
            }
            catch (Exception e)
            {
                throw new Exception("caught it where i thought");
            }*/

        }
    }
}