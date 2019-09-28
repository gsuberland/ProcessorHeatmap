using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace ProcessorHeatmap
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly CpuUsage _cpuUsage;
        private readonly Label[] _blocks;
        private readonly int _colCount;
        private readonly int _rowCount;
        private readonly DispatcherTimer _refreshTimer;
        private readonly ContextMenu _contextMenu;
        private readonly MenuItem _colourSchemeMenu;
        private readonly MenuItem _refreshRateMenu;
        private ColourScheme _colourScheme;

        public MainWindow()
        {
            InitializeComponent();

            _cpuUsage = new CpuUsage();

            // work out how many columns and rows we need (closest square values)
            double idealWidthAndHeight = Math.Sqrt(_cpuUsage.CoreCount);
            int cols = (int)Math.Truncate(idealWidthAndHeight);
            if (cols < idealWidthAndHeight)
            {
                cols++;
            }
            int rows = (int)Math.Truncate(idealWidthAndHeight);

            _rowCount = rows;
            _colCount = cols;

            // build the grid
            for (int c = 0; c < cols; c++)
            {
                ProcessorGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
            }
            for (int r = 0; r < rows; r++)
            {
                ProcessorGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Star) });
            }

            // generate the labels
            var blockList = new List<Label>();
            for (int p = 0; p < _cpuUsage.CoreCount; p++)
            {
                int col = p % cols;
                int row = (p - col) / cols;
                var block = new Label() { Name = $"CpuCell{p:d4}", Content = "0%", HorizontalContentAlignment = HorizontalAlignment.Center, VerticalContentAlignment = VerticalAlignment.Center };
                blockList.Add(block);
                ProcessorGrid.Children.Add(block);
                Grid.SetColumn(block, col);
                Grid.SetRow(block, row);
            }
            _blocks = blockList.ToArray();

            // set the default colour scheme
            _colourScheme = ColourScheme.Schemes.First(cs => cs.Name == "Red");

            // create the context menu
            _contextMenu = new ContextMenu();
            foreach (var block in _blocks)
            {
                block.ContextMenu = _contextMenu;
            }
            // set up colour schemes menu
            _colourSchemeMenu = new MenuItem() { Header = "Colour Scheme" };
            foreach (var scheme in ColourScheme.Schemes)
            {
                var menuItem = new MenuItem()
                {
                    Header = scheme,
                    IsCheckable = true,
                    IsChecked = scheme.Name == "Red",
                };
                menuItem.Click += SetColourScheme;
                _colourSchemeMenu.Items.Add(menuItem);
            }
            _contextMenu.Items.Add(_colourSchemeMenu);
            // set up the timer menu
            _refreshRateMenu = new MenuItem() { Header = "Refresh Rate" };
            double[] times = new double[] { 0.5, 1.0, 2.0, 3.0, 4.0, 5.0, 10.0 };
            foreach (double time in times)
            {
                var menuItem = new MenuItem()
                {
                    Header = time,
                    IsCheckable = true,
                    IsChecked = time == 1.0
                };
                menuItem.Click += SetRefreshRate;
                _refreshRateMenu.Items.Add(menuItem);
            }
            _contextMenu.Items.Add(_refreshRateMenu);

            // set up the refresh timer
            _refreshTimer = new DispatcherTimer()
            {
                Interval = TimeSpan.FromSeconds(1.0),
            };
            _refreshTimer.Tick += UpdateValues;
            _refreshTimer.Start();
        }

        private void UpdateValues(object sender, EventArgs e)
        {
            for (int p = 0; p < _cpuUsage.CoreCount; p++)
            {
                double usage = _cpuUsage.GetCoreUsage(p);
                _blocks[p].Background = new SolidColorBrush(_colourScheme.GetColour(usage));
                _blocks[p].Content = $"{usage:0.00}%";
            }
        }

        private void SetColourScheme(object sender, EventArgs e)
        {
            var menuItem = (MenuItem)sender;
            var scheme = (ColourScheme)menuItem.Header;
            _colourScheme = scheme;
            foreach (MenuItem menu in _colourSchemeMenu.Items)
            {
                menu.IsChecked = menu.Header == scheme;
            }
        }

        private void SetRefreshRate(object sender, EventArgs e)
        {
            var menuItem = (MenuItem)sender;
            var rate = (double)menuItem.Header;
            foreach (MenuItem menu in _refreshRateMenu.Items)
            {
                menu.IsChecked = (double)menu.Header == rate;
            }
            _refreshTimer.Interval = TimeSpan.FromSeconds(rate);
        }
    }
}
