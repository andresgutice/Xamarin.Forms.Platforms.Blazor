using Microsoft.AspNetCore.Components.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;

[assembly: Xamarin.Forms.Platform.Blazor.ExportRenderer(
    typeof(Entry),
    typeof(Xamarin.Forms.Platform.Blazor.Renderers.EntryRenderer))]
namespace Xamarin.Forms.Platform.Blazor.Renderers
{
    public class EntryRenderer : ViewRenderer<Entry>
    {

        bool _needsTextMeasure = true;
        Size _textSize;

        static HashSet<string> _renderProperties = new HashSet<string>
        {
            nameof(Entry.Placeholder),
            nameof(Entry.PlaceholderColor),
            nameof(Entry.IsPassword),
            nameof(Entry.Text),
            nameof(Entry.TextColor)
        };

        private int paddingEntry { get; set; } = 8;

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
                    //_needsTextMeasure = true;
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

            //add text style 
            // style = new Style($"input[type={GetStyleName("textconfig")}]", "");
            var style = new Style(GetStyleName("textconfig"));
            style.Properties["color"] = this.Element.TextColor.ToHTMLColor();
            style.Properties["background-color"] = this.Element.BackgroundColor.ToHTMLColor();
            style.Properties["padding"] = $"{paddingEntry}px";
            style.Properties["width"] = "100%";
            //style.Properties["height"] = "100%";
            style.Properties["position"] = "absolute";
            style.Properties["box-sizing"] = "border-box";
            style.Properties["font-size"] = this.Element.FontSize.ToString() + "px";
            
            /*if (string.IsNullOrEmpty(this.Element.FontFamily))
                style.Properties["font-family"] = "unset";
            else
                style.Properties["font-family"] = this.Element.FontFamily;*/

            this.Styles.Add(style);

            var style2 = new Style($"{GetStyleName("placeholder")}::placeholder", ".");
            style2.Properties["color"] = (this.Element.PlaceholderColor == Color.Default)?Color.Gray.ToHTMLColor() :
                this.Element.PlaceholderColor.ToHTMLColor();
            this.Styles.Add(style2);
        }

        protected override void RenderContent(RenderTreeBuilder builder)
        {
            //Add input
            builder.OpenElement(RenderCounter++, "input");
            builder.AddAttribute(RenderCounter++, "class", $"{GetStyleName("textconfig")} w3-border {GetStyleName("placeholder")}");
            //builder.AddAttribute(RenderCounter++, "style", GetStyleName("textconfig"));
            builder.AddAttribute(RenderCounter++, "type", (this.Element.IsPassword) ? "password" : "text");
            builder.AddAttribute(RenderCounter++, "placeholder", this.Element.Placeholder);
            if(!this.Element.IsEnabled)
                builder.AddAttribute(RenderCounter++, "disabled", "disabled");

            builder.CloseElement();
        }

            
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
                    _textSize.Width +
                        8 * 2
                        ,
                    _textSize.Height +
                        8 * 2
                        //+ this.ActualBorderWidth * 2
                        );
            }
            else
            {
                return new Size
                {
                    Width = _textSize.Width +
                       this.Element.Bounds.Width
                        //+ this.ActualBorderWidth * 2
                        ,
                    Height = _textSize.Height +
                        this.Element.Bounds.Height
                    //+ this.ActualBorderWidth * 2
                };
            }
        }
    }
}
