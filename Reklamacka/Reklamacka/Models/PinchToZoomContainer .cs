/**
 * @brief Implementaion of pinch to zoom gesture
 * 
 * @file PinchToZoomContainer .cs
 * @author Kedra David (xkedra00)
 * @date 05/12/2021
 * 
 * This application serves as submission 
 * for a group project of class ITU at FIT, BUT 2021/2022
 * 
 * Source: https://docs.microsoft.com/cs-cz/xamarin/xamarin-forms/app-fundamentals/gestures/pinch
 */
using System;
using Xamarin.Forms;
using Xamarin.Forms.Internals;

namespace Reklamacka.Models
{
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
				// Store the current scale factor applied to the wrapped user interface element,
				// and zero the components for the center point of the translate transform.
				startScale = Content.Scale;
				Content.AnchorX = 0;
				Content.AnchorY = 0;
			}
			if (e.Status == GestureStatus.Running)
			{
				// Calculate the scale factor to be applied.
				currentScale += (e.Scale - 1) * startScale;
				currentScale = Math.Max(1, currentScale);

				// The ScaleOrigin is in relative coordinates to the wrapped user interface element,
				// so get the X pixel coordinate.
				double renderedX = Content.X + xOffset;
				double deltaX = renderedX / Width;
				double deltaWidth = Width / (Content.Width * startScale);
				double originX = (e.ScaleOrigin.X - deltaX) * deltaWidth;

				// The ScaleOrigin is in relative coordinates to the wrapped user interface element,
				// so get the Y pixel coordinate.
				double renderedY = Content.Y + yOffset;
				double deltaY = renderedY / Height;
				double deltaHeight = Height / (Content.Height * startScale);
				double originY = (e.ScaleOrigin.Y - deltaY) * deltaHeight;

				// Calculate the transformed element pixel coordinates.	
				double targetX = xOffset - originX * Content.Width * (currentScale - startScale);
				double targetY = yOffset - originY * Content.Height * (currentScale - startScale);

				// Apply translation based on the change in origin.
				Content.TranslationX = targetX.Clamp(-Content.Width * (currentScale - 1), 0);
				Content.TranslationY = targetY.Clamp(-Content.Height * (currentScale - 1), 0);

				// Apply scale factor.
				Content.Scale = currentScale;
			}
			if (e.Status == GestureStatus.Completed)
			{
				// Store the translation delta's of the wrapped user interface element.
				xOffset = Content.TranslationX;
				yOffset = Content.TranslationY;
			}
		}
	}
}
