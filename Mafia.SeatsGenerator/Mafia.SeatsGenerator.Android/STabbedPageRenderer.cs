using System.ComponentModel;
using Android.Content;
using Android.Support.Design.Widget;
using Android.Support.V4.View;
using Mafia.SeatsGenerator.Android;
using Mafia.SeatsGenerator.Utils;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using Xamarin.Forms.Platform.Android.AppCompat;
using View = Android.Views.View;

[assembly: ExportRenderer(typeof(STabbedPage), typeof(STabbedPageRenderer))]
namespace Mafia.SeatsGenerator.Android
{
    public class STabbedPageRenderer : TabbedPageRenderer
    {
        protected TabLayout TabBar
        {
            get
            {
                for (int i = 0; i < this.ChildCount; i++)
                {
                    if (this.GetChildAt(i) is TabLayout)
                    {
                        return this.GetChildAt(i) as TabLayout;
                    }
                }
                return null;
            }
        }

        protected ViewPager Pager
        {
            get
            {
                for (int i = 0; i < this.ChildCount; i++)
                {
                    if (this.GetChildAt(i) is ViewPager)
                    {
                        return this.GetChildAt(i) as ViewPager;
                    }
                }
                return null;
            }
        }

        protected IVisualElementRenderer _tabBarRenderer;

        public STabbedPageRenderer(Context context) : base(context)
        {

        }

        protected override void OnElementChanged(ElementChangedEventArgs<TabbedPage> e)
        {
            base.OnElementChanged(e);
            this.RedrawTabBar();
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            if (e.PropertyName == nameof(TabbedPage.CurrentPage))
            {
                this.ChangedCurrentPage();
            }
        }

        void ChangedCurrentPage()
        {
            if (this.Element.CurrentPage == null)
                return;
        }

        protected override void OnLayout(bool changed, int l, int t, int r, int b)
        {
            this.RedrawTabBar();
            
            this.TabBar.Visibility = global::Android.Views.ViewStates.Gone;
        
            if ((this.Element as STabbedPage).TabBarPosition == STabbedPage.TabBarPositionType.Top)
            {
                base.OnLayout
                    (changed,
                    l,
                    t + (int)((this.Element as STabbedPage).TabBarHeight * this.Context.Resources.DisplayMetrics.Density),
                    r,
                    b);
            
                this.Pager.Layout(
                    l,
                    t + (int)((this.Element as STabbedPage).TabBarHeight * this.Context.Resources.DisplayMetrics.Density),
                    r,
                    b);
            
                Xamarin.Forms.Layout.LayoutChildIntoBoundingRegion(
                    this._tabBarRenderer.Element,
                    new Rectangle(
                        l,
                        t,
                        this.Context.FromPixels(r - l),
                        (this.Element as STabbedPage).TabBarHeight));
            }
            else
            {
                base.OnLayout
                    (changed,
                    l,
                    0,
                    r,
                    b - +(int)((this.Element as STabbedPage).TabBarHeight * this.Context.Resources.DisplayMetrics.Density) - this.Top);
            
                this.Pager.Layout(
                    l,
                    0,
                    r,
                    b - +(int)((this.Element as STabbedPage).TabBarHeight * this.Context.Resources.DisplayMetrics.Density) - this.Top);
            
                Xamarin.Forms.Layout.LayoutChildIntoBoundingRegion(
                    this._tabBarRenderer.Element,
                    new Rectangle(
                        l,
                        this.Context.FromPixels(b - t) - (this.Element as STabbedPage).TabBarHeight,
                        this.Context.FromPixels(r - l),
                        (this.Element as STabbedPage).TabBarHeight));
            }
            
            this._tabBarRenderer.UpdateLayout();
        }

        private void RedrawTabBar()
        {
            if (this._tabBarRenderer != null)
            {
                this.RemoveView(this._tabBarRenderer.View);
            }
            
            this._tabBarRenderer = Xamarin.Forms.Platform.Android.Platform.GetRenderer((this.Element as STabbedPage).TabBarView);
            if(this._tabBarRenderer == null)
            {
                this._tabBarRenderer = Xamarin.Forms.Platform.Android.Platform.CreateRendererWithContext((this.Element as STabbedPage).TabBarView, this.Context);
                Xamarin.Forms.Platform.Android.Platform.SetRenderer((this.Element as STabbedPage).TabBarView, this._tabBarRenderer);
            }

            this.AddView(this._tabBarRenderer.View);

            this.TabBar.Visibility = global::Android.Views.ViewStates.Gone;

            this.ChangedCurrentPage();
        }
    }
}