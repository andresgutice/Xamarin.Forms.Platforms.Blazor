using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;

[assembly: Xamarin.Forms.Platform.Blazor.ExportRenderer(
    typeof(Image),
    typeof(Xamarin.Forms.Platform.Blazor.Renderers.ImageRenderer))]
namespace Xamarin.Forms.Platform.Blazor.Renderers
{
    public class ImageRenderer : ViewRenderer<Image>
    {
    }
}
