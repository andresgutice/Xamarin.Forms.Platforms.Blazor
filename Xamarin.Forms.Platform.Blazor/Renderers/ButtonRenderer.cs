using System;
using System.Collections.Generic;
using System.ComponentModel;

using Xamarin.Forms;

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components.Web;
using System.Threading.Tasks;

[assembly: Xamarin.Forms.Platform.Blazor.ExportRenderer(
	typeof(Button),
	typeof(Xamarin.Forms.Platform.Blazor.Renderers.ButtonRenderer))]

namespace Xamarin.Forms.Platform.Blazor.Renderers
{
	public class ButtonRenderer : ViewRenderer<Button>
	{
		bool _needsTextMeasure = true;
		Size _textSize;

		static HashSet<string> _renderProperties = new HashSet<string>
		{
			nameof(Button.BackgroundColor),
			nameof(Button.BorderColor),
			nameof(Button.BorderWidth),
			nameof(Button.CornerRadius),
			nameof(Button.FontAttributes),
			nameof(Button.FontFamily),
			nameof(Button.FontSize),
			nameof(Button.IsEnabled),
			nameof(Button.Text),
			nameof(Button.TextColor),
		};

		protected override void OnElementPropertyChanged(
			object sender,
			PropertyChangedEventArgs e)
		{
			switch (e.PropertyName)
			{
				case nameof(Button.Text):
				case nameof(Button.FontFamily):
				case nameof(Button.FontSize):
					this.Element.NativeSizeChanged();
					_needsTextMeasure = true;
					break;
			}
			base.OnElementPropertyChanged(sender, e);
		}

		protected override bool AffectsRender(string propertyName)
		{
			return base.AffectsRender(propertyName) ||
				_renderProperties.Contains(propertyName);
		}

		protected override void SetBasicStyles()
		{
			base.SetBasicStyles();

			//var style = new Style($"button[type={GetStyleName("buttonConfig")}]", "");
			var style = new Style(GetStyleName("buttonConfig"));
			style.Properties["line-height"] = "1";
			style.Properties["cursor"] = "pointer";
			style.Properties["padding"] = $"{this.Element.Padding.Top}px";
			style.Properties["font"] = this.Element.FontFamily;

			if (string.IsNullOrEmpty(this.Element.FontFamily))
				this.MainStyle.Properties["font-family"] = "unset";
			else
				this.MainStyle.Properties["font-family"] = this.Element.FontFamily;

			double fs = this.Element.FontSize;
			if (!double.IsNaN(fs))
				style.Properties["font-size"] = fs.ToString() + "px";

			style.Properties["display"] = "block";
			style.Properties["width"] = "100%";
			style.Properties["color"] = this.Element.TextColor.ToHTMLColor();
			style.Properties["background-color"] = Element.BackgroundColor.ToHTMLColor();
			style.Properties["border"] = $"{this.ActualBorderWidth}px";
			style.Properties["border-style"] = "solid";
			style.Properties["border-color"] = $"{Element.BorderColor.ToHTMLColor()}";
			style.Properties["border-radius"] = $"{Element.CornerRadius}px";
			this.Styles.Add(style);

			var style2 = new Style(style.Name + ":hover");
			style2.Properties["background-color"] = "#ddd";
			style2.Properties["color"] = "black";
			//this.Styles.Add(style2);

		}

		protected override void AddAdditionalAttributes(RenderTreeBuilder builder)
		{
            builder.AddAttribute(
                this.RenderCounter++,
                "onmousedown",
                EventCallback.Factory.Create<MouseEventArgs>(this, (e) =>
                {
                    this.Element.SendPressed();
                }));
			builder.AddAttribute(
				this.RenderCounter++,
				"onmouseup",
				EventCallback.Factory.Create<MouseEventArgs>(this, (e) =>
				{
					this.Element.SendReleased();
				}));
			builder.AddAttribute(
				this.RenderCounter++, 
				"onclick",
				EventCallback.Factory.Create<MouseEventArgs>(this, (e) =>
				{
					this.Element.SendClicked();
				}));
		}

		protected override void RenderContent(RenderTreeBuilder builder)
		{
			builder.OpenElement(RenderCounter++, "button");
			builder.AddAttribute(RenderCounter++, "class", $"{GetStyleName("buttonConfig")}");
			if (!this.Element.IsEnabled)
				builder.AddAttribute(RenderCounter++, "disabled", "disabled");
			builder.AddContent(this.RenderCounter++, this.Element.Text);
			builder.CloseElement();
		}

		private double ActualBorderWidth => 
			this.Element.BorderWidth == -1 ? 0 : this.Element.BorderWidth;

		private string ActualFontFamily => 
			string.IsNullOrEmpty(this.Element.FontFamily) ? "Times New Roman" : this.Element.FontFamily;

		protected override Size MeasureOverride(Size availableSize)
		{
			if (_needsTextMeasure)
			{
				_needsTextMeasure = false;
                XFUilities.MeasureText(
					this.Element.Text,
					this.ActualFontFamily,
					this.Element.FontSize).ContinueWith(t =>
					{
						_textSize = new Size(t.Result, this.Element.FontSize);
						this.Element.NativeSizeChanged();
                        this.InvalidateRender();
					});
				return new Size(
					this.Element.Bounds.Width
						//+ this.ActualBorderWidth * 2
						,
					this.Element.FontSize // +
						//this.Element.Bounds.Height
						//+ this.ActualBorderWidth * 2
						);
			}
			else
			{
				return new Size
				{
					Width = _textSize.Width + this.Element.Bounds.Width
						//+ this.ActualBorderWidth * 2
						,
					Height = _textSize.Height + this.Element.Bounds.Height
					//(this.Element.Bounds.Height 
					//+ this.ActualBorderWidth * 2
				};
			}
		}
	}
}
