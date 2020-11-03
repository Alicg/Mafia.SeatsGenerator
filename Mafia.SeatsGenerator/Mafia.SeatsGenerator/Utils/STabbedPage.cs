using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using Xamarin.Forms;

namespace Mafia.SeatsGenerator.Utils
{
    public class STabbedPage : TabbedPage
    {
        public enum TabBarPositionType
        {
            Top,
            Bottom
        };

        public static readonly BindableProperty TabBarPositionProperty = BindableProperty.Create(
            nameof(TabBarPosition),
            typeof(TabBarPositionType),
            typeof(STabbedPage),
            TabBarPositionType.Top
            );
        public TabBarPositionType TabBarPosition { get => (TabBarPositionType)this.GetValue(TabBarPositionProperty); set => this.SetValue(TabBarPositionProperty, value); }


        public static readonly BindableProperty TabBarCellTemplateProperty = BindableProperty.Create(
            nameof(TabBarCellTemplate),
            typeof(DataTemplate),
            typeof(STabbedPage)
            );
        public DataTemplate TabBarCellTemplate { get => (DataTemplate)this.GetValue(TabBarCellTemplateProperty); set => this.SetValue(TabBarCellTemplateProperty, value); }

        public static readonly BindableProperty TabBarSelectedCellTemplateProperty = BindableProperty.Create(
            nameof(TabBarSelectedCellTemplate),
            typeof(DataTemplate),
            typeof(STabbedPage)
            );
        public DataTemplate TabBarSelectedCellTemplate { get => (DataTemplate)this.GetValue(TabBarSelectedCellTemplateProperty); set => this.SetValue(TabBarSelectedCellTemplateProperty, value); }

        public static readonly BindableProperty SplitterColorProperty = BindableProperty.Create(
            nameof(SplitterColor),
            typeof(Color),
            typeof(STabbedPage),
            Color.LightGray
            );
        public Color SplitterColor { get => (Color)this.GetValue(SplitterColorProperty); set => this.SetValue(SplitterColorProperty, value); }

        public static readonly BindableProperty SplitterWidthProperty = BindableProperty.Create(
            nameof(SplitterWidth),
            typeof(double),
            typeof(STabbedPage),
            1.0
            );
        public double SplitterWidth { get => (double)this.GetValue(SplitterWidthProperty); set => this.SetValue(SplitterWidthProperty, value); }

        public static readonly BindableProperty TopBarColorProperty = BindableProperty.Create(
            nameof(TopBarColor),
            typeof(Color),
            typeof(STabbedPage),
            Color.LightGray
            );
        public Color TopBarColor { get => (Color)this.GetValue(TopBarColorProperty); set => this.SetValue(TopBarColorProperty, value); }

        public static readonly BindableProperty TopBarHeightProperty = BindableProperty.Create(
            nameof(TopBarHeight),
            typeof(double),
            typeof(STabbedPage),
            1.0
            );
        public double TopBarHeight { get => (double)this.GetValue(TopBarHeightProperty); set => this.SetValue(TopBarHeightProperty, value); }

        public static readonly BindableProperty BottomBarColorProperty = BindableProperty.Create(
            nameof(BottomBarColor),
            typeof(Color),
            typeof(STabbedPage),
            Color.LightGray
            );
        public Color BottomBarColor { get => (Color)this.GetValue(BottomBarColorProperty); set => this.SetValue(BottomBarColorProperty, value); }

        public static readonly BindableProperty BottomBarHeightProperty = BindableProperty.Create(
            nameof(BottomBarHeight),
            typeof(double),
            typeof(STabbedPage),
            1.0
            );
        public double BottomBarHeight { get => (double)this.GetValue(BottomBarHeightProperty); set => this.SetValue(BottomBarHeightProperty, value); }

        public static readonly BindableProperty TabBarHeightProperty = BindableProperty.Create(
            nameof(TabBarHeight),
            typeof(double),
            typeof(STabbedPage),
            70.0
            );
        public double TabBarHeight { get => (double)this.GetValue(TabBarHeightProperty); set => this.SetValue(TabBarHeightProperty, value); }
        
        public static readonly BindableProperty LargeLastCellProperty = BindableProperty.Create(
            nameof(LargeLastCell),
            typeof(bool),
            typeof(STabbedPage),
            true
        );

        public bool LargeLastCell
        {
            get => (bool) this.GetValue(LargeLastCellProperty);
            set => this.SetValue(LargeLastCellProperty, value);
        }

        protected Grid _tabBarView = null;

        protected List<View> cells;
        protected List<View> selectedCells;

        protected override void OnChildAdded(Element child)
        {
            base.OnChildAdded(child);
            
            this.createTabBar();
        }

        protected override void OnChildRemoved(Element child, int oldLogicalIndex)
        {
            base.OnChildRemoved(child, oldLogicalIndex);
            
            this.createTabBar();
        }

        protected void createTabBar()
        {
            this._tabBarView = new Grid
            {
                HeightRequest = this.TabBarHeight,
                RowSpacing = 0
            };

            this._tabBarView.RowDefinitions.Add(new RowDefinition
            {
                Height = new GridLength(this.TopBarHeight, GridUnitType.Absolute)
            });
            this._tabBarView.RowDefinitions.Add(new RowDefinition
            {
                Height = new GridLength(1, GridUnitType.Star)
            });
            this._tabBarView.RowDefinitions.Add(new RowDefinition
            {
                Height = new GridLength(this.BottomBarHeight, GridUnitType.Absolute)
            });

            this._tabBarView.Children.Add(new BoxView
            {
                BackgroundColor = this.TopBarColor
            }, 0, 0);

            this._tabBarView.Children.Add(new BoxView
            {
                BackgroundColor = this.BottomBarColor
            }, 0, 2);

            this.cells = new List<View>();
            if(this.TabBarSelectedCellTemplate != null)
            {
                this.selectedCells = new List<View>();
            }

            Grid gridTabs = new Grid()
            {
                ColumnSpacing = 0
            };
            int i = 0;
            foreach (Page page in this.Children)
            {
                if (i > 0)
                {
                    gridTabs.ColumnDefinitions.Add(new ColumnDefinition
                    {
                        Width = new GridLength(this.SplitterWidth, GridUnitType.Absolute)
                    });
                    gridTabs.Children.Add(new BoxView
                    {
                        BackgroundColor = this.SplitterColor
                    }, 2 * i - 1, 0);
                }

                var isLastPage = i == this.Children.Count - 1;
                
                gridTabs.ColumnDefinitions.Add(new ColumnDefinition
                {
                    Width = new GridLength(isLastPage && this.LargeLastCell ? 2 : 1, GridUnitType.Star)
                });

                View cell = (View)this.TabBarCellTemplate.CreateContent();
                cell.BindingContext = page.BindingContext == null ? page : page.BindingContext;
                cell.GestureRecognizers.Add(new TapGestureRecognizer {
                    CommandParameter = cell,
                    Command = this.SelectCellCommand
                });
                gridTabs.Children.Add(cell, 2 * i, 0);
                this.cells.Add(cell);

                if(this.TabBarSelectedCellTemplate != null)
                {
                    View selectedCell = (View)this.TabBarSelectedCellTemplate.CreateContent();
                    selectedCell.BindingContext = cell.BindingContext;
                    selectedCell.IsVisible = false;
                    gridTabs.Children.Add(selectedCell, 2 * i, 0);
                    this.selectedCells.Add(selectedCell);
                }

                i++;
            }

            this._tabBarView.Children.Add(gridTabs, 0, 1);

            this.OnCurrentPageChanged();
        }

        public Grid TabBarView
        {
            get
            {
                if (this._tabBarView == null)
                {
                    this.createTabBar();
                }
                return this._tabBarView;
            }
        }

        protected override void OnCurrentPageChanged()
        {
            base.OnCurrentPageChanged();

            if (this.selectedCells == null)
                return;

            for(int i=0; i<this.Children.Count; i++)
            {
                this.SetViewVisibility(this.cells[i], this.Children[i] != this.CurrentPage);
                this.SetViewVisibility(this.selectedCells[i], !this.cells[i].IsVisible);
            }
        }

        private void SetViewVisibility(View view, bool visibility)
        {
            view.IsVisible = visibility;
            foreach (var child in view.LogicalChildren.OfType<VisualElement>())
            {
                child.IsVisible = visibility;
            }
        }

        public ICommand SelectCellCommand => new Command((v) =>
        {
            try
            {
                int index = this.cells.IndexOf((View)v);
                if (index < 0)
                    return;
                this.CurrentPage = null;
                var page = this.Children[index];
                this.CurrentPage = page;
            }
            catch (Exception e)
            {
                // ignored
            }
        });
    }
}