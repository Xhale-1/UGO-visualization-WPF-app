using Wpf_interface;
using e3;
using System.Threading;
using System.Collections.Concurrent;


namespace script
{
    class Program
    {

        [STAThread]
        public static void Main(string[] args)
        {
            App app = new App();
            app.InitializeComponent();


            Thread thread = new Thread(() => new Wpf_interface.ThreadProcClass(app));
            thread.Start();


            app.Run();
        }



    }
}
