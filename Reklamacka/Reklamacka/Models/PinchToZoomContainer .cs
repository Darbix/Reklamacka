using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;
using Xamarin.Forms.Internals;

namespace Reklamacka.Models
{
	/// <summary>
	/// Trida pro zpracovani gesta priblizeni obrazku pri jeho zobrazeni
	/// </summary>
	public class PinchToZoomContainer: ContentView
	{
		public PinchToZoomContainer()
		{
			var pinchGesture = new PinchGestureRecognizer();
			pinchGesture.PinchUpdated += OnPinchUpdated;
			GestureRecognizers.Add(pinchGesture);
		}

		public double startScale = 1;
		public double currentScale = 1;
		public double xOffset = 0;
		public double yOffset = 0;

		// funkce zavolana objektem contentview s obrazkem
		public void OnPinchUpdated(object sender, PinchGestureUpdatedEventArgs e)
		{
			if (e.Status == GestureStatus.Started)
			{
				// Ulozeni aktualni velikosti a vycentrovani
				startScale = Content.Scale;
				Content.AnchorX = 0;
				Content.AnchorY = 0;
			}
			if (e.Status == GestureStatus.Running)
			{
				// Vypocet velikosti vektoru
				currentScale += (e.Scale - 1) * startScale;
				currentScale = Math.Max(1, currentScale);

				// Zisk X hodnoty z relativniho ScaleOrigin 
				double renderedX = Content.X + xOffset;
				double deltaX = renderedX / Width;
				double deltaWidth = Width / (Content.Width * startScale);
				double originX = (e.ScaleOrigin.X - deltaX) * deltaWidth;

				// Zisk Y hodnoty z relativniho ScaleOrigin 
				double renderedY = Content.Y + yOffset;
				double deltaY = renderedY / Height;
				double deltaHeight = Height / (Content.Height * startScale);
				double originY = (e.ScaleOrigin.Y - deltaY) * deltaHeight;

				// Vypocet pretransformovanych souradnic
				double targetX = xOffset - (originX * Content.Width) * (currentScale - startScale);
				double targetY = yOffset - (originY * Content.Height) * (currentScale - startScale);

				// Aplikace posunu
				Content.TranslationX = targetX.Clamp(-Content.Width * (currentScale - 1), 0);
				Content.TranslationY = targetY.Clamp(-Content.Height * (currentScale - 1), 0);

				// Aplikace zvetseni
				Content.Scale = currentScale;
			}
			if (e.Status == GestureStatus.Completed)
			{
				// Ulozeni rozdilu posunu
				xOffset = Content.TranslationX;
				yOffset = Content.TranslationY;
			}
		}
	}
}
