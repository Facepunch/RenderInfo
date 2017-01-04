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

namespace RenderInfo
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public RendererInstance[] Data;

        public MainWindow()
        {
            Data = new RendererInstance[0];
            DataContext = this;

            if ( App.Filename != null )
            {
                OpenFile( App.Filename );
            }

            InitializeComponent();
        }

        private void Stats( object sender, RoutedEventArgs e )
        {
            dataGrid.ItemsSource = new[]
            {
                new
                {
                    Name = "All Renderers",
                    Value = $"{Data.Length:N0} ({Data.Count( x => x.IsVisible ):N0} visible)"
                },

                new
                {
                    Name = "Mesh Renderers",
                    Value = $"{Data.Count( x => x.RenderType == "MeshRenderer" ):N0} ({Data.Count( x => x.RenderType == "MeshRenderer" && x.IsVisible ):N0} visible)"
                },

                new
                {
                    Name = "Mesh Renderers",
                    Value = $"{Data.Count( x => x.RenderType == "SkinnedMeshRenderer" ):N0} ({Data.Count( x => x.RenderType == "SkinnedMeshRenderer" && x.IsVisible ):N0} visible)"
                }
            };
        }

        private void GroupByType( object sender, RoutedEventArgs e )
        {
            dataGrid.ItemsSource = Data.GroupBy( x => x.RenderType )
                .Select( x => new
                {
                  Type = x.Key,
                  Count = x.Count(),
                  Active = x.Count( y => y.Enabled && y.IsVisible )
                })
                .OrderByDescending( x => x.Count );
        }

        private void GroupByMesh( object sender, RoutedEventArgs e )
        {
            dataGrid.ItemsSource = Data
                .Where( x => x.RenderType == "MeshRenderer" )
                .GroupBy( x => x.MeshName )
                .Select( x => new
                {
                    Type = x.Key,
                    Count = x.Count(),
                    Active = x.Count( y => y.Enabled && y.IsVisible ),
                    VertexVisible = x.Where( y => y.IsVisible ).Sum( y => y.VertexCount ),
                    VertexSingle = x.First().VertexCount
                } )
                .OrderByDescending( x => x.Count );
        }

        private void GroupBySkinnedMesh( object sender, RoutedEventArgs e )
        {
            dataGrid.ItemsSource = Data
                .Where( x => x.RenderType == "SkinnedMeshRenderer" )
                .GroupBy( x => x.MeshName )
                .Select( x => new
                {
                    Type = x.Key,
                    Count = x.Count(),
                    Active = x.Count( y => y.Enabled && y.IsVisible ),
                    VertexVisible = x.Where( y => y.IsVisible ).Sum( y => y.VertexCount ),
                    VertexSingle = x.First().VertexCount
                } )
                .OrderByDescending( x => x.Count );
        }

        private void OnDroppedFile( object sender, DragEventArgs e )
        {
            if ( e.Data.GetDataPresent( DataFormats.FileDrop ) )
            {
                // Note that you can have more than one file.
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

                // Assuming you have one file that you care about, pass it off to whatever
                // handling code you have defined.
                OpenFile( files[0] );
            }
        }

        private void OpenFile( string v )
        {
            try
            {
                var str = System.IO.File.ReadAllText( v );
                Data = Newtonsoft.Json.JsonConvert.DeserializeObject<List<RendererInstance>>( str ).ToArray();
                Title = "RenderInfo - " + System.IO.Path.GetFileName( v );

                listBox.UnselectAll();
                listBox.SelectedIndex = 0;

            }
            catch ( System.Exception e )
            {
                
            }
        }
    }
}
