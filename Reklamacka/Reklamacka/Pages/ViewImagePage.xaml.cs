using Reklamacka.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Reklamacka.Pages
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class ViewImagePage : ContentPage
	{

		private ImageSource imgBill;
		/// <summary>
		/// Zdrojova data obrazku
		/// </summary>
		public ImageSource ImgBill
		{
			get => imgBill;
			set
			{
				imgBill = value;
				OnPropertyChanged(nameof(ImgBill));
			}
		}

		public PinchToZoomContainer pinchToZoom;

		public ViewImagePage(Bill bill)
		{
			InitializeComponent();
			BindingContext = this;
			pinchToZoom = new PinchToZoomContainer();
			ImgBill = bill.GetImage();
		}

		private void OnPinchUpdated(object sender, PinchGestureUpdatedEventArgs e)
		{
			pinchToZoom.OnPinchUpdated(sender, e);
		}

		private async void Cancel_Clicked(object sender, EventArgs e)
		{
			await App.Current.MainPage.Navigation.PopAsync();
		}
	}
}