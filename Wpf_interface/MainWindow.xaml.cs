using e3;
using Microsoft.Win32;
using System.Data.Common;
using System.Runtime.ConstrainedExecution;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using static System.Net.Mime.MediaTypeNames;
//using System.Linq;

namespace Wpf_interface
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : System.Windows.Window
    {
        public static App app;
        public static e3Application e3;


        Grid grid;
        List<Canvas> canvases;

        double rowHeight = 100-25;
        double colWidth = 100-25;
        double thick = 0.7;
        double thick2 = 1;
        double top = 0;
        double left =0;

        public MainWindow()
        {
            InitializeComponent();
            // Устанавливаем окно поверх всех других окон
            this.Topmost = true;

            // Не показывать окно в панели задач
            this.ShowInTaskbar = false;

        }




        public void settings(double x = 0, double y = 0, bool mouse = false)
        {


            //Вычсиление координат появления окна
            float addx = -20;
            float addy = 40;

            if (mouse)
            {
                System.Windows.Point getpos = SystemEvents.getpos();
                x = getpos.X;
                y = getpos.Y;
            }
            this.Left = x + addx;
            this.Top = y + addy;




            //Сетка данных
            this.grid = new Grid();
            //this.grid.Background = System.Windows.Media.Brushes.Gray;
            this.grid.Margin = new Thickness(5, 0, 0, 0);
            //this.grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(colWidth) });
            //this.grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(rowHeight) });
            this.grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(colWidth) });
            //border.Child = this.grid;
            this.Content = grid;

            //Скролл
            ScrollViewer scrollViewer = new ScrollViewer();
            scrollViewer.Content = grid; // Помещаем Grid внутрь ScrollViewer
            scrollViewer.HorizontalScrollBarVisibility = ScrollBarVisibility.Hidden;
            scrollViewer.VerticalScrollBarVisibility = ScrollBarVisibility.Hidden;
            Content = scrollViewer;


            //Лист холстов
            this.canvases = new();


            //измененеие окна
            this.SizeChanged += MainWindow_SizeChanged;



            // Запуск трэда вычисления дистанции
            double x_corner = x + addx;
            double y_corner = y + addy;
            double h = this.Height;
            double w = this.Width;

            //Thread thread3 = new Thread(() => thread3_calculate_distance(h, w, x_corner, y_corner, app.source.Token));
            //thread3.Start();
        }
        public void thread3_calculate_distance(double height, double width, double x_corner, double y_corner, CancellationToken token)
        {
            double x, y;
            double dist = 0;
            //e3.PutMessage("th0: " + Thread.CurrentThread.ManagedThreadId.ToString());

            x = x_corner + width / 2;
            y = y_corner + height / 2;

            double x2 = x_corner + width / 2;
            double y2 = y_corner - height / 6;


            while (!token.IsCancellationRequested)
            {
                System.Windows.Point getpos = SystemEvents.getpos();


                dist = Math.Sqrt(Math.Pow((getpos.X - x), 2) + Math.Pow((getpos.Y - y), 2));


                if (dist > 160)
                {
                    app.closewin();
                }

                Thread.Sleep(200);
            }
        }




        public void main_load_data(ThreadProcClass.classdev data)
        {
            int i = 1;
            foreach (string str in data.dictdev.Keys)
            {
                Canvas canv = new Canvas();
                canv = DrawNewChar(this.grid, canv, data.dictdev[str], i);
                canvases.Add(canv);
                i++;
            }


            //for (int i=1; i < 10; i++) 
            //{
            //    string str = data.dictdev.Keys.ToList()[0];
            //    Canvas canv = new Canvas();
            //    canv = DrawNewChar(this.grid, canv, data.dictdev[str], i);
            //    canvases.Add(canv);
            //}

        }



        private Canvas DrawNewChar(Grid grid, Canvas canvass, Dictionary<string, double[]> ugo, int i)
        {

            ////Создаём новую строчку в существующем grid для нового холста
            grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(rowHeight) } );
            int rowCount = grid.RowDefinitions.Count;
            int columnCount = grid.ColumnDefinitions.Count;






            ////Нстройка холста
            canvass.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
            canvass.VerticalAlignment = VerticalAlignment.Center;
            //canvass.Margin = new Thickness(left, top, 0, 0);




            //Создаём рамку для ячейки
            Border border = new Border
            {
                HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch, // Занимает всю ширину
                VerticalAlignment = VerticalAlignment.Stretch,   // Занимает всю высоту
                BorderBrush = System.Windows.Media.Brushes.Black,                     // Цвет рамки
                BorderThickness = new Thickness(1)               // Толщина рамки (обязательно)
            };
            Grid.SetRow(border, rowCount - 1);
            Grid.SetColumn(border, columnCount - 1);
            grid.Children.Add( border );



            
            //Помещаем пустой холст на созданную ячейку
            Grid.SetRow(canvass, rowCount - 1);
            Grid.SetColumn(canvass, columnCount - 1);





            //Начало координат
            //var line01 = new Line { X1 = 0, Y1 = 0, X2 = 0, Y2 = 5, Stroke = System.Windows.Media.Brushes.Black, StrokeThickness = thick2 };
            //canvass.Children.Add(line01);
            //var line02 = new Line { X1 = 0, Y1 = 0, X2 = 5, Y2 = 0, Stroke = System.Windows.Media.Brushes.Black, StrokeThickness = thick2 };
            //canvass.Children.Add(line02);
            //var line03 = new Line { X1 = 0, Y1 = 0, X2 = 0, Y2 = -5, Stroke = System.Windows.Media.Brushes.Black, StrokeThickness = thick2 };
            //canvass.Children.Add(line03);
            //var line04 = new Line { X1 = 0, Y1 = 0, X2 = -5, Y2 = 0, Stroke = System.Windows.Media.Brushes.Black, StrokeThickness = thick2 };
            //canvass.Children.Add(line04);




            //TextBlock textBlock = new TextBlock
            //{
            //    Text = i.ToString(), // Устанавливаем текст равным i
            //    Foreground = System.Windows.Media.Brushes.Black, // Цвет текста
            //    FontSize = 16,         // Размер шрифта
            //    FontWeight = FontWeights.Bold,  // Жирный шрифт (опционально)
            //    VerticalAlignment = VerticalAlignment.Center,
            //    HorizontalAlignment = System.Windows.HorizontalAlignment.Center
            //};
            //canvass.Children.Add(textBlock);
            //this.Content = canvass;
            //grid.Children.Add(canvass);



            double cenx = 0;
            double ceny = 0;
            double maxx = this.grid.ColumnDefinitions[columnCount - 1].ActualWidth;
            double maxy = this.grid.ColumnDefinitions[columnCount - 1].ActualWidth;

            //Получаем размерные параметры УГО
            ugo = changedata(ugo);
            List<List<double>> points = minmaxarr(ugo);
            var (verminx, verminy, scale, vercenx, verceny) = ScaleEtc(points, maxx, maxy);
            //var (verminx, verminy, scale, vercenx, verceny) = (0, 0, 2, 0, 0);



            foreach (string gra in ugo.Keys)
            {
                int type = int.Parse(gra.Split('_')[0]);

                if (type == 1)
                {
                    double x1 = ugo[gra][0];
                    double y1 = ugo[gra][1];
                    double x2 = ugo[gra][2];
                    double y2 = ugo[gra][3];

                    var line = new Line
                    {
                        X1 = scale * (x1 - verminx - vercenx) + cenx,
                        Y1 = scale * (y1 - verminy - verceny) + ceny,
                        X2 = scale * (x2 - verminx - vercenx) + cenx,
                        Y2 = scale * (y2 - verminy - verceny) + ceny,
                        Stroke = System.Windows.Media.Brushes.Black,
                        StrokeThickness = thick
                    };
                    canvass.Children.Add(line);

                }

                /*if (type == 2)
                {
                    //double x1 = ugo[gra][0];
                    //double y1 = ugo[gra][1];
                    //double x2 = ugo[gra][2];
                    //double y2 = ugo[gra][3];

                    // Масштабируем координаты
                    //double scaledX1 = scale * (x1 - verminx - vercenx) + cenx;
                    //double scaledY1 = scale * (y1 - verminy - verceny) + ceny;
                    //double scaledX2 = scale * (x2 - verminx - vercenx) + cenx;
                    //double scaledY2 = scale * (y2 - verminy - verceny) + ceny;

                    // Вычисляем координаты верхнего левого угла, ширину и высоту
                    //double left = Math.Min(scaledX1, scaledX2);
                    //double top = Math.Min(scaledY1, scaledY2);
                    //double width = Math.Abs(scaledX2 - scaledX1);
                    //double height = Math.Abs(scaledY2 - scaledY1);

                    var polygon = new System.Windows.Shapes.Polygon
                    {
                        Stroke = System.Windows.Media.Brushes.Black, // Цвет границы
                        StrokeThickness = thick,                     // Толщина границы
                        Fill = System.Windows.Media.Brushes.LightBlue, // Цвет заливки
                    };

                    // Задаем точки полигона (координаты X и Y)

                    for (int j = 0; i < ugo[gra].Length; j = j + 2)
                    {
                        double x1 = ugo[gra][j];
                        double y1 = ugo[gra][j+1];
                        double scaledX1 = scale * (x1 - verminx - vercenx) + cenx;
                        double scaledY1 = scale * (y1 - verminy - verceny) + ceny;
                        polygon.Points.Add(new System.Windows.Point(scaledX1, scaledY1));
                    }
                    //polygon.Points.Add(new System.Windows.Point(50, 50));

                    // Добавляем полигон на Canvas
                    canvass.Children.Add(polygon);
                }*/

                if (type == 3)
                {
                    double x1 = ugo[gra][0];
                    double y1 = ugo[gra][1];
                    double x2 = ugo[gra][2];
                    double y2 = ugo[gra][3];

                    // Масштабируем координаты
                    double scaledX1 = scale * (x1 - verminx - vercenx) + cenx;
                    double scaledY1 = scale * (y1 - verminy - verceny) + ceny;
                    double scaledX2 = scale * (x2 - verminx - vercenx) + cenx;
                    double scaledY2 = scale * (y2 - verminy - verceny) + ceny;

                    // Вычисляем координаты верхнего левого угла, ширину и высоту
                    double left = Math.Min(scaledX1, scaledX2);
                    double top = Math.Min(scaledY1, scaledY2);
                    double width = Math.Abs(scaledX2 - scaledX1);
                    double height = Math.Abs(scaledY2 - scaledY1);

                    var rec = new System.Windows.Shapes.Rectangle
                    {
                        Width = width,
                        Height = height,
                        Stroke = System.Windows.Media.Brushes.Black,
                        StrokeThickness = thick
                    };

                    Canvas.SetLeft(rec, left); // Устанавливаем X координату
                    Canvas.SetTop(rec, top);   // Устанавливаем Y координату

                    canvass.Children.Add(rec);
                }

                if (type == 4)
                {
                    double x1 = ugo[gra][0];
                    double y1 = ugo[gra][1];
                    double r = ugo[gra][2];


                    Ellipse circle = new Ellipse
                    {
                        Width = scale * r * 2,  // Диаметр круга
                        Height = scale * r * 2, // Диаметр круга
                        Fill = System.Windows.Media.Brushes.Transparent, // Заливка круга
                        Stroke = System.Windows.Media.Brushes.Black, // Цвет обводки
                        StrokeThickness = thick // Толщина обводки
                    };

                    // Позиционируем круг на Canvas
                    Canvas.SetLeft(circle, scale * (x1 - verminx - vercenx - r) + cenx); // Позиция по X
                    Canvas.SetTop(circle, scale * (y1 - verminy - verceny - r) + ceny);  // Позиция по Y

                    // Добавляем круг на Canvas
                    canvass.Children.Add(circle);
                }

                if (type == 5)
                {
                    double x1 = ugo[gra][0];
                    double y1 = ugo[gra][1];
                    double r = ugo[gra][2];
                    double st = ugo[gra][3];
                    double end = ugo[gra][4];

                    // Нормализация углов
                    if (st < 0) st += 360;
                    if (end < 0) end += 360;

                    // Нормализация углов
                    if (st > 360) st -= 360;
                    if (end > 360) end -= 360;

                    x1 = scale * (x1 - verminx - vercenx) + cenx;
                    y1 = scale * (y1 - verminy - verceny) + ceny;
                    r = scale * r;

                    // Создаем Path для дуги
                    Path arcPath = new Path();
                    arcPath.Stroke = System.Windows.Media.Brushes.Black;
                    arcPath.StrokeThickness = thick;

                    // Создаем PathGeometry
                    PathGeometry pathGeometry = new PathGeometry();
                    PathFigure pathFigure = new PathFigure();

                    // Начальная точка дуги
                    System.Windows.Point startPoint = new System.Windows.Point(x1 + r * Math.Sin((st * Math.PI / 180) - Math.PI / 2), y1 + r * Math.Cos((st * Math.PI / 180) - Math.PI / 2));
                    pathFigure.StartPoint = startPoint;

                    // Создаем ArcSegment
                    ArcSegment arcSegment = new ArcSegment(
                        new System.Windows.Point(x1 + r * Math.Sin((end * Math.PI / 180) - Math.PI / 2), y1 + r * Math.Cos((end * Math.PI / 180) - Math.PI / 2)),// Конечная точка
                        new System.Windows.Size(r, r), // Радиус
                        0, // Угол поворота
                        end - st > 180, // Флаг, указывающий, является ли дуга большой (больше 180 градусов)
                        SweepDirection.Clockwise, // Направление дуги (по часовой стрелке)
                        true // Флаг, указывающий, что дуга должна быть нарисована
                    );

                    // Добавляем ArcSegment в PathFigure
                    pathFigure.Segments.Add(arcSegment);

                    // Добавляем PathFigure в PathGeometry
                    pathGeometry.Figures.Add(pathFigure);

                    // Устанавливаем PathGeometry в Path
                    arcPath.Data = pathGeometry;

                    // Добавляем дугу на Canvas
                    canvass.Children.Add(arcPath);
                }

            }

            this.grid.Children.Add(canvass);

            return canvass;
        }

        private Dictionary<string, double[]> changedata(Dictionary<string, double[]> ugo)
        {
            foreach (string gra in ugo.Keys)
            {
                if (int.Parse(gra.Split("_")[0]) == 1)
                {
                    ugo[gra][1] = -ugo[gra][1];
                    ugo[gra][3] = -ugo[gra][3];
                }

                /*if (int.Parse(gra.Split("_")[0]) == 2)
                {
                    for (int i =1; i < ugo[gra].Length; i = i + 2)
                    {
                        ugo[gra][i] = -ugo[gra][i];
                    }
                }*/

                if (int.Parse(gra.Split("_")[0]) == 3)
                {
                    ugo[gra][1] = -ugo[gra][1];
                    ugo[gra][3] = -ugo[gra][3];
                }

                if (int.Parse(gra.Split("_")[0]) == 4)
                {
                    ugo[gra][1] = -ugo[gra][1];
                }

                if (int.Parse(gra.Split("_")[0]) == 5)
                {
                    ugo[gra][1] = -ugo[gra][1];
                }
            }
            return ugo;
        }
        private List<List<double>> minmaxarr(Dictionary<string, double[]> ugo)
        {
            List<List<double>> points;
            List<double> pointsx;
            List<double> pointsy;

            pointsx = new List<double>();
            pointsy = new List<double>();
            points = new List<List<double>>();
            foreach (string gra in ugo.Keys)
            {
                string[] gra_ = gra.Split('_');
                if (gra_[0] == "1")
                {
                    pointsx.Add(ugo[gra][0]);
                    pointsy.Add(ugo[gra][1]);
                    pointsx.Add(ugo[gra][2]);
                    pointsy.Add(ugo[gra][3]);
                }

                /*if (gra_[0] == "2")
                {
                    for (int i = 0; i < ugo[gra].Length; i = +2)
                    {
                        pointsx.Add(ugo[gra][i]);
                        pointsy.Add(ugo[gra][i + 1]);
                    }
                }*/

                if (gra_[0] == "3")
                {
                    pointsx.Add(ugo[gra][0]);
                    pointsy.Add(ugo[gra][1]);
                    pointsx.Add(ugo[gra][2]);
                    pointsy.Add(ugo[gra][3]);
                }

                if (gra_[0] == "4")
                {
                    pointsx.Add(ugo[gra][0] + ugo[gra][2]);
                    pointsy.Add(ugo[gra][1]);
                    pointsx.Add(ugo[gra][0] - ugo[gra][2]);
                    pointsy.Add(ugo[gra][1]);
                    pointsx.Add(ugo[gra][0]);
                    pointsy.Add(ugo[gra][1] + ugo[gra][2]);
                    pointsx.Add(ugo[gra][0]);
                    pointsy.Add(ugo[gra][1] - ugo[gra][2]);
                }

                if (gra_[0] == "5")
                {
                    pointsx.Add(ugo[gra][0] + ugo[gra][2]);
                    pointsy.Add(ugo[gra][1]);
                    pointsx.Add(ugo[gra][0] - ugo[gra][2]);
                    pointsy.Add(ugo[gra][1]);
                    pointsx.Add(ugo[gra][0]);
                    pointsy.Add(ugo[gra][1] + ugo[gra][2]);
                    pointsx.Add(ugo[gra][0]);
                    pointsy.Add(ugo[gra][1] - ugo[gra][2]);
                }
            }
            points.Add(pointsx);
            points.Add(pointsy);


            //double[][][][] vers2 = new double[data.dictdev.Count][][][];
            //for (int i = 0; i < data.dictdev.Count; i++)
            //{
            //    vers2[i] = new double[10][][];
            //    string ver = data.dictdev.Keys.ToList()[i];
            //    foreach (string gra in data.dictdev[ver].Keys)
            //    {
            //        string[] gra_ = gra.Split('_');
            //        int type = int.Parse(gra_[0]);

            //        for (int j = 0; j < data.dictdev[ver][gra].Length; j=j+2)
            //        {
            //            if (type == 1)
            //            {
            //                vers2[i][type] = new double[2][];
            //                vers2[i][type][0] = new double[];
            //                vers2[i][type][0][j] = data.dictdev[ver][gra][j];
            //                vers2[i][type][1][j] = data.dictdev[ver][gra][j++];
            //            }




            //        }
            //    }
            //}

            //e3.PutMessage(vers2[0][1][0].Max().ToString());
            return points;
        }
        private (double verminx, double verminy, double scale, double vercenx, double verceny) ScaleEtc(List<List<double>> points, double maxx, double maxy)
        {
            double vermaxx = points[0].Max();
            double verminx = points[0].Min();
            double vermaxy = points[1].Max();
            double verminy = points[1].Min();

            double vercenx = (vermaxx - verminx) / 2;
            double verceny = (vermaxy - verminy) / 2;

            double verdiffx = vermaxx - verminx;
            double verdiffy = vermaxy - verminy;

            double scalex = maxx / (verdiffx + (verdiffx / 3));
            double scaley = maxy / (verdiffy + (verdiffy / 3));
            double scale = Math.Min(scalex, scaley);

            return (verminx, verminy, scale, vercenx, verceny);
        }





        private void MainWindow_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            int rowCount = grid.RowDefinitions.Count;

            double windowWidth = e.NewSize.Width;

            // Определяем количество столбцов в зависимости от ширины окна
            int newColumnCount = (int)(windowWidth / colWidth); // Например, новый столбец каждые 100 пикселей

            if (newColumnCount < 1) newColumnCount = 1; // Минимум 1 столбец




            // Если количество столбцов изменилось
            if (newColumnCount != grid.ColumnDefinitions.Count)
            {
                UpdateGridColumns(newColumnCount);
                RedistributeButtons();
            }


        }

        private void UpdateGridColumns(int newColumnCount)
        {
            // Очищаем текущие столбцы
            grid.ColumnDefinitions.Clear();

            // Добавляем новые столбцы
            for (int i = 0; i < newColumnCount; i++)
            {
                grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(colWidth) });
            }
        }

        private void RedistributeButtons()
        {
            int columnCount = grid.ColumnDefinitions.Count;
            int rowCount = (int)Math.Ceiling((double)canvases.Count / columnCount);

            // Очищаем текущие строки
            grid.RowDefinitions.Clear();
            var bordersToRemove = grid.Children.OfType<Border>().ToList();
            foreach (var border in bordersToRemove)
            {
                grid.Children.Remove(border);
            }

            // Добавляем новые строки
            for (int i = 0; i < rowCount; i++)
            {
                grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(rowHeight) });
            }

            // Перераспределяем элементы по новым строкам и столбцам
            for (int i = 0; i < canvases.Count; i++)
            {
                int row = i / columnCount;
                int column = i % columnCount;

                // Устанавливаем строку и столбец для Canvas
                Grid.SetRow(canvases[i], row);
                Grid.SetColumn(canvases[i], column);

                // Добавляем рамку только для ячеек, где есть Canvas
                Border border = new Border
                {
                    HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch, // Занимает всю ширину
                    VerticalAlignment = VerticalAlignment.Stretch,   // Занимает всю высоту
                    BorderBrush = System.Windows.Media.Brushes.Black,                     // Цвет рамки
                    BorderThickness = new Thickness(1)               // Толщина рамки
                };
                Grid.SetRow(border, row);
                Grid.SetColumn(border, column);
                grid.Children.Add(border);
            }
        }





    }
}