using Microsoft.AspNetCore.Components.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;

[assembly: Xamarin.Forms.Platform.Blazor.ExportRenderer(
    typeof(ActivityIndicator),
    typeof(Xamarin.Forms.Platform.Blazor.Renderers.ActivityIndicatorRenderer))]

namespace Xamarin.Forms.Platform.Blazor.Renderers
{
	public class ActivityIndicatorRenderer : ViewRenderer<ActivityIndicator>
	{
		static HashSet<string> _renderProperties = new HashSet<string>
		{
			nameof(ActivityIndicator.Color),
			nameof(ActivityIndicator.IsRunning)
		};


		protected override bool AffectsRender(string propertyName)
		{
			return base.AffectsRender(propertyName) ||
				_renderProperties.Contains(propertyName);
		}

		protected override void OnElementPropertyChanged(
			object sender,
			PropertyChangedEventArgs e)
		{
			switch (e.PropertyName)
			{
				case nameof(ActivityIndicator.Color):
				case nameof(ActivityIndicator.IsRunning):
					this.Element.NativeSizeChanged();
					break;
			}
			base.OnElementPropertyChanged(sender, e);
		}


		protected override Size MeasureOverride(Size constraint)
		{
			//this.Element.NativeSizeChanged();
			//this.InvalidateRender();
			int proportion = Convert.ToInt32(this.Element.Width / 30);
			return new Size()
			{
				Width = this.Element.Bounds.Width + 6*proportion,
				Height = this.Element.Bounds.Height + 6*proportion
			};
		}

		protected override void RenderContent(RenderTreeBuilder builder)
		{
			builder.OpenElement(RenderCounter++, "div");
			builder.AddAttribute(RenderCounter++, "class", GetStyleName("loaderconfig"));
			builder.AddAttribute(RenderCounter++, "style",
				$"position: absolute; ");
			builder.CloseElement();
		}

		protected override void SetBasicStyles()
		{
			base.SetBasicStyles();

			int proportion = Convert.ToInt32(this.Element.Width/30);

			var style = new Style(GetStyleName("loaderconfig"));
			style.Properties["border"] = $"{6*proportion}px solid #f3f3f3";
			style.Properties["border-top"] = $"{6*proportion}px solid " + this.Element.Color.ToHTMLColor();
			style.Properties["border-radius"] = "50%";
			style.Properties["position"] = "absolute";
			style.Properties["width"] = $"50%";
			style.Properties["height"] = $"50%";
			style.Properties["top"] = $"{Element.Margin.Top}";
			this.MainStyle.Properties["background-color"] = this.Element.BackgroundColor.ToHTMLColor();
			style.Properties["animation"] = $"{GetStyleName("spin")} 1s linear infinite";
			this.Styles.Add(style);

			//Add animation
			var spin = new AnimationStyle(GetStyleName("spin"));
			spin.Animations["0%"] = new Dictionary<string, string> {
				{"transform", "rotate(0deg)" }
			};
			spin.Animations["100%"] = new Dictionary<string, string> {
				{"transform", "rotate(360deg)" }
			};
			this.Styles.Add(spin);

		}
	}
}
