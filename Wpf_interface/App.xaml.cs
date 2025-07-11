using System.Configuration;
using System.Data;
using System.Windows;
using Gma.System.MouseKeyHook;
using e3;
using System.Collections.Concurrent;
using System.Collections.Generic;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using System.Windows.Controls;
using System;
using System.Drawing;
using System.Xml.Linq;
using System.Collections;

using System.Linq;

namespace Wpf_interface
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : System.Windows.Application
    {


        public BlockingCollection<Action<ThreadProcClass>> queue= null;
        public CancellationTokenSource source = null;

        private IKeyboardMouseEvents _globalHook;
        public e3Application e3;

        public MainWindow window = null;


        public App()
        {
            // Создаём очередь событий
            this.queue = new();

            // Подписываемся на глобальные события мыши
            _globalHook = Hook.GlobalEvents();
            _globalHook.MouseDown += OnMouseDown;

        }



        private void OnMouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {

                // Получаем текущее окно WPF
                if (window != null)
                {
                    System.Drawing.Point screenMousePos = System.Windows.Forms.Control.MousePosition;
                    System.Windows.Point wpfMousePos = window.PointFromScreen(new System.Windows.Point(screenMousePos.X, screenMousePos.Y));

                    if (wpfMousePos.X >= -15 && 
                        wpfMousePos.Y >= -15 &&
                        wpfMousePos.X <= window.ActualWidth+15 &&
                        wpfMousePos.Y <= window.ActualHeight+15)
                    {
                        return;
                    }
                }


                this.queue.Add((threadclass) =>
                {
                    //e3.PutMessage("Левый клик мыши зафиксирован!");
                    Thread.Sleep(100);
                    router(threadclass);
                });
            }
        }
        private void router(ThreadProcClass threadclass)
        {
            dynamic[] devids = new dynamic[3]; 
            int cnt = threadclass.job.GetSelectedAllDeviceIds(ref devids);
            //e3.PutMessage(cnt.ToString() + ", " + devids[1].ToString());


            if (cnt == 1)
            {
                threadclass.id_cur = devids[1];

                if (threadclass.id_prev != threadclass.id_cur || this.window == null || threadclass.id_prev == threadclass.id_cur)
                {
                    if (this.window != null)
                    {
                        this.closewin();
                    }


                    dynamic[] devcoords = threadclass.devcoords(threadclass.id_cur);
                    dynamic[] curscoords = threadclass.cursorcoods();
                    //double[] dev_gl_coords = threadclass.get_dev_global_coords(curscoords[0], curscoords[1], devcoords[0], devcoords[1]);
                    double[] curs_gl_coords = new double[2];
                    curs_gl_coords[0] = SystemEvents.getpos().X;
                    curs_gl_coords[1] = SystemEvents.getpos().Y;

                    if (((Math.Abs(devcoords[0] - curscoords[0]) < 60) && Math.Abs(devcoords[1] - curscoords[1]) < 60))
                    {
                        ThreadProcClass.classdev data = threadclass.collectwindata(threadclass.id_cur);
                        MainWindow window = this.createwin(x: curs_gl_coords[0], y: curs_gl_coords[1]);
                        this.load_data(data);
                        threadclass.id_prev = threadclass.id_cur;
                    }
                }
            }

            if (cnt == 0 && this.window != null)
            {
                this.closewin();
            }


        }




        public MainWindow createwin(double x = 0, double y = 0, bool mouse = false)
        {
            this.source = new CancellationTokenSource();
            Dispatcher.Invoke(() =>
            {
                this.window = new MainWindow();
                this.window.settings(x: x, y: y, mouse: mouse);
                this.window.Show();
            });

            return this.window;
        }

        public void closewin()
        {
            Dispatcher.Invoke(() =>
            {
                this.window.Close();
                this.window = null;
            });
            this.source.Cancel();
        }

        public void load_data(ThreadProcClass.classdev data)
        {
            Dispatcher.Invoke(() =>
            {
                if (window != null)
                {
                    window.main_load_data(data);
                }
            });
        }

        public void set_e3(e3Application e3)
        {
            Dispatcher.Invoke(() =>
            {
                Wpf_interface.MainWindow.e3 = e3;
            });
        }

        public void set_app(App app)
        {
            Dispatcher.Invoke(() =>
            {
                Wpf_interface.MainWindow.app = app;
            });
        }

        public void hidewin()
        {
            // Загрузка данных в окно
            Dispatcher.Invoke(() =>
            {
                this.window.Hide();
            });
        }

        public void showwin()
        {
            // Загрузка данных в окно
            Dispatcher.Invoke(() =>
            {
                window.Show();
            });
        }




        protected override void OnExit(ExitEventArgs e)
        {
            // Отписываемся от событий и освобождаем ресурсы
            _globalHook.MouseDown -= OnMouseDown;
            _globalHook.Dispose();
            base.OnExit(e);
        }
    }




    public class ThreadProcClass
    {

        public class classdev
        {
            public int id;
            public Dictionary<string, Dictionary<string, double[]>> dictdev = new();

        }
        public classdev classdev1;


        //public Dictionary<string,        Dictionary<string, double[]>> dictdev;
        //                  version1.name, dictgra1
        //                  version2.name, dictgra2
        //                  version3.name, dictgra3
           

        //public Dictionary<string,              double[]> dictgra;
        //                graphic_element1.name, graphiccoords1
        //                graphic_element2.name, graphiccoords2
        //                graphic_element3.name, graphiccoords3


        //public double[] graphiccoords = new double[3];
        //                graphiccoords[0] = x1
        //                graphiccoords[1] = y1



        public App app;
        public e3Application e3;
        public dynamic job;
        public dynamic sht;
        public dynamic dev;
        public dynamic sym;
        public dynamic gra;

        public dynamic id_prev = 0;
        public dynamic id_cur = 0;

        public dynamic active_shtid = 0;







        public ThreadProcClass(App app)
        {
            this.app = app;
            ThreadProc();
        }

        public void ThreadProc()
        {
            dynamic type = Type.GetTypeFromProgID("CT.Application");
            this.e3 = Activator.CreateInstance(type);
            this.job = e3.CreateJobObject();
            e3.PutMessage("Hello E3 from .NET");
            this.app.e3 = e3;

            this.app.set_app(this.app);
            this.app.set_e3(this.e3);

            this.sht = job.CreateSheetObject();
            this.dev = job.CreateDeviceObject();
            this.sym = job.CreateSymbolObject();
            this.gra = job.CreateGraphObject();




            foreach (var action in this.app.queue.GetConsumingEnumerable())
            {
                action(this);
            }

        }

        public classdev collectwindata(dynamic devid)
        {
            dev.setid(devid);

            dynamic[] symids = new dynamic[3];
            dynamic outs = dev.GetSymbolIds(ref symids);
            sym.setid(symids[1]);


            this.active_shtid = this.job.GetActiveSheetId();
            sht.setid(this.active_shtid);






            
            dynamic[] validchar = new dynamic[10];
            sym.GetValidCharacteristics(ref validchar);
            //foreach (dynamic charr in validchar)
            //{
            //    e3.PutMessage(charr);
            //}


            //цикл по версиям
            this.classdev1 = new classdev();
            this.classdev1.id = devid;
            foreach (string str in validchar.Skip(1))
            {


                string gettype = sym.GetType;
                int lastParagraphIndex = gettype.LastIndexOf('¶');
                if (lastParagraphIndex != -1)
                {
                    gettype = gettype.Substring(0, lastParagraphIndex);
                }
                gettype = gettype + '¶' + str;






                dynamic gr_id = gra.CreateFromSymbol(sht.getid, 0, 0, 0, 1, 0, gettype, sym.GetVersion);
                gra.setid(gr_id);

                dynamic[] graids = new dynamic[3];
                int nGras = gra.GetGraphIds(ref graids);
                //e3.PutMessage(nGras.ToString());







                Dictionary<string, double[]> dictgra = new Dictionary<string, double[]>();
                for (int i = 1; i < graids.Length; i++)
                {
                    dynamic id = graids[i];
                    gra.setid(id);


                    double[] graphiccoords = null;
                    getgraphiccoordsinfo(ref graphiccoords);


                    dictgra.Add(gra.GetType + "_" + i.ToString(), graphiccoords);
                    //break;
                }






                gra.setid(gr_id);
                gra.delete();


                this.classdev1.dictdev.Add(gettype, dictgra);
            }


            //print_dictdev(dictdev);
            //print_classdev(this.classdev1);
            return classdev1;
        }


        private void getgraphiccoordsinfo(ref double[] graphiccoords)
        {
            //e3.PutMessage(gra.GetType);
            if (gra.GetType == 1)
            {
                graphiccoords = new double[4];

                dynamic x1 = null;
                dynamic y1 = null;
                dynamic x2 = null;
                dynamic y2 = null;

                int outt = gra.GetLine(ref x1, ref y1, ref x2, ref y2);
                //e3.PutMessage(outt.GetType().ToString());
                //e3.PutMessage(outt.ToString());
                //e3.PutMessage("x1: " + x1.GetType().ToString());
                //e3.PutMessage("x1: " + x1.ToString());
                //e3.PutMessage("y1: " + y1.ToString());
                //e3.PutMessage("x2: " + x2.ToString());
                //e3.PutMessage("y2: " + y2.ToString());

                graphiccoords[0] = x1;
                graphiccoords[1] = y1;
                graphiccoords[2] = x2;
                graphiccoords[3] = y2;
            }

            /*if (gra.GetType == 2)
            {
                graphiccoords = new double[4];

                dynamic npts = null;
                dynamic xarr = null;
                dynamic yarr = null;

                dynamic outt = gra.GetRectangle(ref npts, ref xarr, ref yarr);

                graphiccoords = new double[npts];
                for (int i = 0; i < npts; i = i + 2)
                {
                    graphiccoords[i] = xarr[i];
                }
                for (int i = 1; i < npts; i = i + 2)
                {
                    graphiccoords[i] = yarr[i];
                }

            }*/

            if (gra.GetType == 3)
            {
                graphiccoords = new double[4];

                dynamic x1 = null;
                dynamic y1 = null;
                dynamic x2 = null;
                dynamic y2 = null;

                int outt = gra.GetRectangle(ref x1, ref y1, ref x2, ref y2);

                graphiccoords[0] = x1;
                graphiccoords[1] = y1;
                graphiccoords[2] = x2;
                graphiccoords[3] = y2;

            }

            if (gra.GetType == 4)
            {
                graphiccoords = new double[3];

                dynamic xm = null;
                dynamic ym = null;
                dynamic rad = null;

                int outt = gra.GetCircle(ref xm, ref ym, ref rad);

                graphiccoords[0] = xm;
                graphiccoords[1] = ym;
                graphiccoords[2] = rad;

            }

            if (gra.GetType == 5)
            {
                graphiccoords = new double[5];

                dynamic xm = null;
                dynamic ym = null;
                dynamic rad = null;
                dynamic startang = null;
                dynamic endang = null;

                int outt = gra.GetArc(ref xm, ref ym, ref rad, ref startang, ref endang);

                graphiccoords[0] = xm;
                graphiccoords[1] = ym;
                graphiccoords[2] = rad;
                graphiccoords[3] = startang;
                graphiccoords[4] = endang;

            }


        }

        private void print_dictdev(Dictionary<string, Dictionary<string, double[]>> dictdev)
        {
            e3.PutMessage("__________________");
            foreach (string vers in dictdev.Keys)
            {
                e3.PutMessage("");
                e3.PutMessage("");
                e3.PutMessage("");
                e3.PutMessage("version: " + vers.ToString());
                foreach (string gra in dictdev[vers].Keys)
                {

                    e3.PutMessage("gra: " + gra.ToString());

                    if (dictdev[vers][gra] != null)
                        {
                        int i = 1;
                        foreach (double coord in dictdev[vers][gra])
                        {
                            e3.PutMessage("param " + i.ToString() + ": " + coord.ToString());
                        }
                    }


                }
            }
        }
        private void print_classdev(classdev classdev)
        {
            e3.PutMessage("__________________");
            foreach (string vers in classdev.dictdev.Keys)
            {
                e3.PutMessage("");
                e3.PutMessage("");
                e3.PutMessage("");
                e3.PutMessage("version: " + vers.ToString());
                foreach (string gra in classdev.dictdev[vers].Keys)
                {

                    e3.PutMessage("gra: " + gra.ToString());

                    if (classdev.dictdev[vers][gra] != null)
                    {
                        int i = 1;
                        foreach (double coord in classdev.dictdev[vers][gra])
                        {
                            e3.PutMessage("param " + i.ToString() + ": " + coord.ToString());
                        }
                    }


                }
            }
        }



        public dynamic[] devcoords(dynamic devid)
        {
            dev.setid(devid);

            dynamic[] ids = new dynamic[3];
            int cntdev = dev.GetSymbolIds(ref ids);
            sym.setid(ids[1]);

            dynamic x = null;
            dynamic y = null;
            dynamic grid = null;
            int symout = sym.GetSchemaLocation(ref x, ref y, ref grid);

            dynamic[] outs = new dynamic[2];
            outs[0] = x;
            outs[1] = y;
            return outs;
        }

        public dynamic[] cursorcoods()
        {

            dynamic xpos = null;
            dynamic ypos = null;
            dynamic[] cursorcoords = new dynamic[2];

            dynamic shtid = job.GetCursorPosition(ref xpos, ref ypos);
            cursorcoords[0] = xpos;
            cursorcoords[1] = ypos;

            return cursorcoords;
        }

        public double[] get_dev_global_coords(double cursx, double cursy, double devx, double devy)
        {
            System.Windows.Point getpos = SystemEvents.getpos();
            double diff_x = getpos.X - cursx;
            double diff_y = getpos.Y - cursy;
            //e3.PutMessage("diff_x: " + diff_x.ToString());
            //e3.PutMessage("diff_y: " + diff_y.ToString());


           // this.active_shtid = this.job.GetActiveSheetId();
           // this.sht.setid(this.active_shtid);
           // dynamic scale= null;
           // dynamic xoff = null;
           // dynamic yoff = null;
           // dynamic outt = this.sht.GetPanelRegion(ref xoff, ref yoff, ref scale);
           // e3.PutMessage(scale.ToString());



            double x = devx + diff_x;
            double y = devy + diff_y;

            double[] globalcoords = new double[2];
            globalcoords[0] = x;
            globalcoords[1] = y;
            return globalcoords;


        }

        public double[] get_scale()
        {
            double[] doubles = new double[2];
            return doubles;
        }
    }




}
