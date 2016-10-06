using System;

using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using FFImageLoading.Forms.Droid;

namespace Xamarin_Layouts.Droid
{
	[Activity(Label = "Xamarin_Layouts", Icon = "@drawable/icon", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
	public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsApplicationActivity//FormsAppCompatActivity
	{
		protected override void OnCreate(Bundle bundle)
		{
			base.OnCreate(bundle);

			global::Xamarin.Forms.Forms.Init(this, bundle);
			CachedImageRenderer.Init();

			//Material Design Blow Post
			//https://blog.xamarin.com/android-tips-hello-appcompatactivity-goodbye-actionbaractivity/
			//Material Design Color Helper Website
			//https://www.materialpalette.com/indigo/blue
			//ToolbarResource = Resource.Layout.my_toolbar;
			//TabLayoutResource = Resource.Layout.my_tabs;

			LoadApplication(new Xamarin_Layouts.App());
		}
	}
}

