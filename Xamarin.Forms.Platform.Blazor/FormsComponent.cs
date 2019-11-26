using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;

using Microsoft.JSInterop;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Xamarin.Forms.Platform.Blazor.Interop;
using System.Threading;
using System.Threading.Tasks;

namespace Xamarin.Forms.Platform.Blazor
{
	public abstract class FormsComponent : ComponentBase, INotifyPropertyChanged
	{
		bool _isRenderInitialized;
		private Size _DesiredSize;
        SynchronizationContext _syncContext;

        public FormsComponent()
        {
            _syncContext = SynchronizationContext.Current;
        }

        public event PropertyChangedEventHandler PropertyChanged;

		protected int RenderCounter { get; set; }

        IJSRuntime _JSRuntime;
        [Inject]
        private IJSRuntime JSRuntime
        {
            get => _JSRuntime;
            set
            {
                _JSRuntime = value;
                this.XFUilities = null;
            }
        }

        XFUtilities _XFUilities;
        internal XFUtilities XFUilities
        {
            get => _XFUilities ?? (_XFUilities = new XFUtilities(this.JSRuntime));
            private set => _XFUilities = value;
        }

		[Parameter]
		public VisualElement Element
		{
			get => GetElement();
			set => SetElement(value);
		}

		protected Style MainStyle { get; } = new Style();
		protected List<IStyle> Styles { get; } = new List<IStyle>();

		public Size DesiredSize
		{
			get
			{
				return _DesiredSize;
			}
			protected set
			{
				if (_DesiredSize != value)
				{
					_DesiredSize = value;
					OnPropertyChanged();
				}
			}
		}

		public FormsComponent VisualParent { get; private set; }

		protected abstract VisualElement GetElement();

		protected abstract void SetElement(VisualElement element);

		public void Measure(Size constraint)
		{
			var sz = this.MeasureOverride(constraint);
			this.DesiredSize = sz;
		}

		protected virtual Size MeasureOverride(Size constraint)
		{
			return new Size(1, 1);
		}

		public virtual void InvalidateMeasure()
		{
		}

		public void InvalidateRender()
		{
			if (!_isRenderInitialized)
				return;
            this.SafeInvoke(() =>
            {
                this.StateHasChanged();
            });			
		}

		protected void OnPropertyChanged(
			[CallerMemberName]
			string name = null)
		{
			this.PropertyChanged?.Invoke(
				this,
				new PropertyChangedEventArgs(name));
		}

		protected virtual void BuildSelectorStyles(RenderTreeBuilder builder, string baseStyle)
		{
		}

		public string GetStyleName(string name)
		{
			return $"{name}{this.Element.GetHashCode()}";
		}

		protected override sealed void BuildRenderTree(RenderTreeBuilder builder)
		{
			RenderCounter = 0;
			this._isRenderInitialized = true;

			this.SetBasicStyles();

			this.MainStyle.Name = GetStyleName("style");
			this.Styles.Add(this.MainStyle);

			base.BuildRenderTree(builder);

			builder.OpenElement(RenderCounter++, "style");
			this.Styles.ForEach(x => builder.AddContent(RenderCounter++, x.GetStyleCSS()));
			builder.CloseElement();

			BuildSelectorStyles(builder, this.MainStyle.Name);

			//Add stylesheet css reference
			builder.OpenElement(RenderCounter++, "link");
			builder.AddAttribute(RenderCounter++, "rel", "stylesheet");
			builder.AddAttribute(RenderCounter++, "href", "https://www.w3schools.com/w3css/4/w3.css");
			builder.CloseElement();

			builder.OpenElement(RenderCounter++, "div");
			builder.AddAttribute(RenderCounter++, "class", this.MainStyle.Name);
			AddAdditionalAttributes(builder);
			builder.AddContent(RenderCounter++, RenderContent);
			builder.CloseElement();
		}

		private string ConstructCSS(IDictionary<string, string> parameters)
		{
			return string.Concat(
				parameters.Select(k => $"{k.Key}: {k.Value};\r"));
		}

		protected virtual void AddAdditionalAttributes(RenderTreeBuilder builder)
		{
		}

		protected virtual void RenderContent(RenderTreeBuilder builder)
		{
		}

		protected virtual void SetBasicStyles()
		{
		}

		protected void RenderChild(RenderTreeBuilder builder, VisualElement child)
		{
			if (child == null)
				return;
			Type t = Platform.GetOrCreateRendererType(child);
			builder.OpenComponent(0, t);
			builder.AddAttribute(1, nameof(FormsComponent.Element), child);
			builder.CloseComponent();
		}

        internal void SafeInvoke(Action action)
        {
            if (SynchronizationContext.Current != _syncContext)
                InvokeAsync(() => action());
            else
                action();
        }
    }
}