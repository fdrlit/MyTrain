using System;
using Xamarin.Forms;
using System.IO;

namespace MyTrain
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            MainPage = new NavigationPage(new Login())
            {
                BarBackgroundColor = Color.Transparent,
                BarTextColor = Color.Transparent
            };
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
