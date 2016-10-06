using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Xamarin_Layouts
{
	public partial class GridLayoutXaml : ContentPage
	{
		public GridLayoutXaml()
		{
			var viewModel = new RootPageViewModel();
			BindingContext = viewModel;

			InitializeComponent();

			var sideMenuGesture = new TapGestureRecognizer();
			sideMenuGesture.Tapped += viewModel.SideMenuNavigationHandler;

			Water.GestureRecognizers.Add(sideMenuGesture);
			Bathroom.GestureRecognizers.Add(sideMenuGesture);
			Pain.GestureRecognizers.Add(sideMenuGesture);
			OtherMedical.GestureRecognizers.Add(sideMenuGesture);
			OtherNonMedical.GestureRecognizers.Add(sideMenuGesture);
			Emergency.GestureRecognizers.Add(sideMenuGesture);

			var topButtonsGesture = new TapGestureRecognizer();
			topButtonsGesture.Tapped += viewModel.TopMenuButtonHandler;

			Settings.GestureRecognizers.Add(topButtonsGesture);
			Wifi.GestureRecognizers.Add(topButtonsGesture);
			Volume.GestureRecognizers.Add(topButtonsGesture);
			Help.GestureRecognizers.Add(topButtonsGesture);
		}
	}
}