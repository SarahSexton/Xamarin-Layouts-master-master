using System;
using System.Threading.Tasks;
using Xamarin.Forms;
using FFImageLoading.Forms;

namespace Xamarin_Layouts
{
	public class RootPageViewModel : BaseViewModel
	{
		public RootPageViewModel()
		{
			initialSecondDelay = 60 - DateTime.Now.Second;
			Timer();
		}

		string hospitalName = "MyHospital";
		public string HospitalName
		{
			get { return hospitalName; }
			set
			{
				if (hospitalName == value)
					return;
				hospitalName = value;
				OnPropertyChanged();
			}
		}

		public string ClockTime
		{
			get
			{
				return DateTime.Now.ToString("HH:mm");
			}
		}

		int initialSecondDelay = 0;

		async void Timer()
		{
			while (true)
			{
				await Task.Delay(TimeSpan.FromSeconds(initialSecondDelay));
				OnPropertyChanged("ClockTime");
				if (initialSecondDelay != 60)
					initialSecondDelay = 60;
			}
		}

		bool topBarButtonLock;
		public void TopMenuButtonHandler(object sender, EventArgs e)
		{
			if (topBarButtonLock)
				return;
			topBarButtonLock = true;

			var buttonPressed = sender as CachedImage;
			switch (buttonPressed.StyleId)
			{
				case "Settings":
					Console.WriteLine("Settings button was pressed");
					break;
				case "Wifi":
					Console.WriteLine("Wifi button was pressed");
					break;
				case "Volume":
					Console.WriteLine("Sound button was pressed");
					break;
				case "Help":
					Console.WriteLine("Help button was pressed");
					break;
			}
			topBarButtonLock = false;
		}

		bool sideButtonLock;
		public void SideMenuNavigationHandler(object sender, EventArgs e)
		{
			if (sideButtonLock)
				return;
			sideButtonLock = true;

			var buttonPressed = sender as StackLayout;
			switch (buttonPressed.StyleId)
			{
				case "Water":
					Console.WriteLine("Water button cliced");
					break;
				case "Bathroom":
					Console.WriteLine("Bathroom button cliced");
					break;
				case "Pain":
					Console.WriteLine("Pain button cliced");
					break;
				case "OtherMedical":
					Console.WriteLine("OtherMedical button cliced");
					break;
				case "OtherNonMedical":
					Console.WriteLine("OtherNonMedical button cliced");
					break;
				case "Emergency":
					Console.WriteLine("Emergency button cliced");
					break;
			}
			sideButtonLock = false;
		}
	}
}
