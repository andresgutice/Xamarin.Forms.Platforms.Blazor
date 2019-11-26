using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms.Internals;

namespace Xamarin.Forms.Platform.Blazor
{
    public interface IStyle
    {
        string Name { get; set; }
        string GetStyleCSS();
    }

    public class Style : IStyle
    {
        public Style(string name = "", string refe = ".")
        {
            this.Name = name;
            this.Refe = refe;
        }

        public string Name { get; set; }
        public string Refe { get; set; }
        public Dictionary<string, string> Properties { get; } = new Dictionary<string, string>();

        public string GetStyleCSS()
        {
            string styleCSS = string.Empty;
            string parameters = string.Empty;

            parameters = string.Concat(
                    this.Properties.Select(k => $"{k.Key}: {k.Value};\r"));

            styleCSS = $@"{this.Refe}{this.Name}
                    {{
                        all: initial;
                        {parameters}
                    }}";
            return styleCSS;
        }
    }

    public class AnimationStyle : IStyle
    {
        public AnimationStyle(string name)
        {
            this.Name = name;
        }
        public string Name { get; set; }

        public Dictionary<string, Dictionary<string, string>> Animations { get; } = new Dictionary<string, Dictionary<string, string>>();

        public string GetStyleCSS()
        {
            string styleCSS = string.Empty;
            List<string> animations = new List<string>();

            foreach (var animation in this.Animations)
            {
                var parametersCSS = string.Concat(animation.Value.Select(k => $"{k.Key}: {k.Value};\r"));
                var animationCSS = $"{animation.Key} {{ {parametersCSS} }}\r";
                animations.Add(animationCSS);
            };

            var animationsCSS = string.Concat(animations.Select(x=>x));
            styleCSS = string.Concat($"@keyframes {this.Name} {{ {animationsCSS} }}\r");

            return styleCSS;
        }
    }
}
