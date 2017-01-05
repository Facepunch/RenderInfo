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
        public Facepunch.Unity.RenderInfo.RendererInstance[] Data;

        public MainWindow()
        {
            Data = new Facepunch.Unity.RenderInfo.RendererInstance[0];
            DataContext = this;

            InitializeComponent();

            if ( App.Filename != null )
            {
                OpenFile( App.Filename );
            }            
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

        private void GroupByEntity( object sender, RoutedEventArgs e )
        {
            dataGrid.ItemsSource = Data.Where( x => x.EntityName != null )
                .GroupBy( x => x.EntityName )
                .Select( x => new
                {
                    Type = x.Key,
                    Count = x.GroupBy( y => y.EntityId ).Count(),
                    Active = x.GroupBy( y => y.EntityId ).Count( y => y.Any( z => z.Enabled && z.IsVisible ) ),
                    TotalVertex = x.Sum( y => y.VertexCount ),
                    TotalVertexVisible = x.Where( y => y.IsVisible ).Sum( y => y.VertexCount ),
                    TotalParticlesVisible = x.Where( y => y.IsVisible ).Sum( y => y.ParticleCount ),
                    MinDistance = x.Where( y => y.IsVisible ).DefaultIfEmpty().Min( y => (int)y.Distance ),
                    MaxDistance = x.Where( y => y.IsVisible ).DefaultIfEmpty().Max( y => (int)y.Distance ),
                } )
                .OrderByDescending( x => x.Count );
        }

        private void GroupByType( object sender, RoutedEventArgs e )
        {
            dataGrid.ItemsSource = Data.GroupBy( x => x.RenderType )
                .Select( x => new
                {
                    Type = x.Key,
                    Count = x.Count(),
                    Active = x.Count( y => y.Enabled && y.IsVisible ),
                    VertexVisible = x.Where( y => y.IsVisible ).Sum( y => y.VertexCount ),
                    MinDistance = x.Where( y => y.IsVisible ).DefaultIfEmpty().Min( y => (int)y.Distance ),
                    MaxDistance = x.Where( y => y.IsVisible ).DefaultIfEmpty().Max( y => (int)y.Distance ),
                } )
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
                    VertexSingle = x.First().VertexCount,
                    MinDistance = x.Where( y => y.IsVisible ).DefaultIfEmpty().Min( y => (int)y.Distance ),
                    MaxDistance = x.Where( y => y.IsVisible ).DefaultIfEmpty().Max( y => (int)y.Distance ),
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
                    VertexSingle = x.First().VertexCount,
                    MinDistance = x.Where( y => y.IsVisible ).DefaultIfEmpty().Min( y => (int)y.Distance ),
                    MaxDistance = x.Where( y => y.IsVisible ).DefaultIfEmpty().Max( y => (int)y.Distance ),
                } )
                .OrderByDescending( x => x.Count );
        }

        private void GroupByParticleSystem( object sender, RoutedEventArgs e )
        {
            dataGrid.ItemsSource = Data
                .Where( x => x.RenderType == "ParticleSystemRenderer" )
                .GroupBy( x => x.MeshName )
                .Select( x => new
                {
                    Type = x.Key,
                    Count = x.Count(),
                    Active = x.Count( y => y.Enabled && y.IsVisible ),
                    Particles = x.Sum( y => y.ParticleCount ),
                    MinDistance = x.Where( y => y.IsVisible ).DefaultIfEmpty().Min( y => (int)y.Distance ),
                    MaxDistance = x.Where( y => y.IsVisible ).DefaultIfEmpty().Max( y => (int)y.Distance ),
                } )
                .OrderByDescending( x => x.Count );
        }

        private void GroupByBillboards( object sender, RoutedEventArgs e )
        {
            dataGrid.ItemsSource = Data
                .Where( x => x.RenderType == "BillboardRenderer" )
                .GroupBy( x => x.EntityName == null ? x.ObjectName : x.EntityName )
                .Select( x => new
                {
                    Type = x.Key,
                    Count = x.Count(),
                    Active = x.Count( y => y.Enabled && y.IsVisible ),
                    MinDistance = x.Where( y => y.IsVisible ).DefaultIfEmpty().Min( y => (int)y.Distance ),
                    MaxDistance = x.Where( y => y.IsVisible ).DefaultIfEmpty().Max( y => (int)y.Distance ),
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

        private void MediumDistance( object sender, RoutedEventArgs e )
        {
            dataGrid.ItemsSource = Data
                .Where( x => x.IsVisible && x.Distance > 250 && x.Distance < 500 )
                .GroupBy( x => x.MeshName )
                .Select( x => new
                {
                    Type = x.Key,
                    Count = x.Count(),
                    VertexVisible = x.Where( y => y.IsVisible ).Sum( y => y.VertexCount ),
                    VertexSingle = x.First().VertexCount,
                    MinDistance = x.Where( y => y.IsVisible ).DefaultIfEmpty().Min( y => (int)y.Distance ),
                    MaxDistance = x.Where( y => y.IsVisible ).DefaultIfEmpty().Max( y => (int)y.Distance ),
                    CastsShadows = x.Any( y => y.CastShadows )
                } )
                .OrderByDescending( x => x.Count );
        }

        private void FarDistance( object sender, RoutedEventArgs e )
        {
            dataGrid.ItemsSource = Data
                .Where( x => x.IsVisible && x.Distance > 500 && x.Distance < 1500 )
                .GroupBy( x => x.MeshName )
                .Select( x => new
                {
                    Type = x.Key,
                    Count = x.Count(),
                    VertexVisible = x.Where( y => y.IsVisible ).Sum( y => y.VertexCount ),
                    VertexSingle = x.First().VertexCount,
                    MinDistance = x.Where( y => y.IsVisible ).DefaultIfEmpty().Min( y => (int)y.Distance ),
                    MaxDistance = x.Where( y => y.IsVisible ).DefaultIfEmpty().Max( y => (int)y.Distance ),
                    CastsShadows = x.Any( y => y.CastShadows )
                } )
                .OrderByDescending( x => x.Count );
        }

        private void VeryFarDistance( object sender, RoutedEventArgs e )
        {
            dataGrid.ItemsSource = Data
                .Where( x => x.IsVisible && x.Distance > 1500 )
                .GroupBy( x => x.MeshName )
                .Select( x => new
                {
                    Type = x.Key,
                    Count = x.Count(),
                    VertexVisible = x.Where( y => y.IsVisible ).Sum( y => y.VertexCount ),
                    VertexSingle = x.First().VertexCount,
                    MinDistance = x.Where( y => y.IsVisible ).DefaultIfEmpty().Min( y => (int)y.Distance ),
                    MaxDistance = x.Where( y => y.IsVisible ).DefaultIfEmpty().Max( y => (int)y.Distance ),
                    CastsShadows = x.Any( y => y.CastShadows )
                } )
                .OrderByDescending( x => x.Count );
        }

        private void OpenFile( string v )
        {
            try
            {
                Data = new Facepunch.Unity.RenderInfo.RendererInstance[0];
                dataGrid.ItemsSource = Data;

                GC.Collect();

                var str = System.IO.File.ReadAllText( v );
                Data = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Facepunch.Unity.RenderInfo.RendererInstance>>( str ).ToArray();
                Title = "RenderInfo - " + System.IO.Path.GetFileName( v );

                listBox.UnselectAll();
                listBox.SelectedIndex = 0;

            }
            catch ( System.Exception )
            {
                
            }
        }

        private void GenerateColumn( object sender, DataGridAutoGeneratingColumnEventArgs e )
        {
            e.Column.CellStyle = dataGrid.FindResource( "IntegerTemplate" ) as Style;
        }
    }
}
