using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace RenderInfo
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static string Filename = null;

        protected override void OnStartup( StartupEventArgs e )
        {
            if ( e.Args != null && e.Args.Length > 0 )
            {
                Filename = e.Args[0];
            }

            base.OnStartup( e );
        }
    }
}
