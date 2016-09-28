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
using Core;
using System.Collections.ObjectModel;

namespace AdvancedSubwayRoutePlanning
{
    /// <summary>
    /// SubwayGraph.xaml 的交互逻辑
    /// </summary>
    public partial class SubwayGraph : UserControl
    {
        #region 字段区域

        private SubwayMap subwayMap;
        private double scrollX = 0;
        private double scrollY = 0;
        private double zoomScale = 1;
        private Point mouseDownPoint;
        private Point mouseLastPoint;
        private ObservableCollection<DisplayRouteUnit> displayRouteUnitList;

        #endregion

        #region 构造区域

        public SubwayGraph()
        {
            InitializeComponent();
            this.subwayMap = BackgroundCore.GetBackgroundCore().SubwayMap;
            this.displayRouteUnitList = ((App)App.Current).DisplayRouteUnitList;
        }

        #endregion

        #region 绘制区域

        protected override void OnRender(DrawingContext dc)
        {
            base.OnRender(dc);

            //绘制背景
            drawBackground(dc);

            //滚动与缩放
            Point graphPoint = Mouse.GetPosition(this);
            dc.PushTransform(new TranslateTransform(scrollX, scrollY));
            dc.PushTransform(new ScaleTransform(zoomScale, zoomScale));

            //绘制地铁线路
            drawSubwayGraph(dc);

            //绘制当前乘车路线
            drawCurRoute(dc);

            //绘制起点和终点
            drawStartAndEndStations(dc);

            //绘制遮挡框架
            drawFrame(dc);

            //绘制线路列表
            drawLineList(dc);

        }

        private void drawBackground(DrawingContext dc)
        {
            Rect rc = new Rect(-scrollX / zoomScale, -scrollY / zoomScale, Math.Abs(this.ActualWidth / zoomScale), Math.Abs(this.ActualHeight / zoomScale));
            dc.DrawRectangle(Brushes.White, new Pen(Brushes.Black, 0), rc);
        }

        private void drawFrame(DrawingContext dc)
        {
            RectangleGeometry rect1 = new RectangleGeometry(new Rect(-scrollX / zoomScale, -scrollY / zoomScale, Math.Abs(this.ActualWidth / zoomScale), Math.Abs(this.ActualHeight / zoomScale)));
            RectangleGeometry rect2 = new RectangleGeometry(new Rect(-scrollX / zoomScale, -scrollY / zoomScale, Math.Abs(((App)App.Current).MainWindow.ActualWidth / zoomScale), Math.Abs(((App)App.Current).MainWindow.ActualHeight / zoomScale)));

            GeometryGroup group = new GeometryGroup();
            group.FillRule = FillRule.EvenOdd;
            group.Children.Add(rect1);
            group.Children.Add(rect2);

            dc.DrawGeometry(Brushes.White, new Pen(Brushes.Black, 0), group);
        }

        private void drawLineList(DrawingContext dc)
        {
            //反向滚动与缩放
            dc.Pop();
            dc.Pop();

            //遮罩层
            Rect rc = new Rect(5, 5, 250, (subwayMap.SubwayLines.Count + 1) * 15);
            dc.DrawRectangle(new SolidColorBrush(Color.FromArgb(180, 245, 245, 245)), new Pen(Brushes.Black, 0.5), rc);

            //线路列表
            double y = rc.Y + 15;
            foreach (SubwayLine line in subwayMap.SubwayLines)
            {
                //绘制线路标示线
                dc.DrawLine(new Pen(new SolidColorBrush(hexToColor(line.Color)), 5), new Point(rc.X + 10, y), new Point(rc.X + 70, y));

                //绘制线路名
                FormattedText formattedText = createFormattedText(line.Name);
                dc.DrawText(formattedText, new Point(rc.X + 80, y - formattedText.Height / 2));

                y += 15;
            }
        }

        private void drawSubwayGraph(DrawingContext dc)
        {
            //绘制地铁路径
            foreach (Connection connection in subwayMap.Connections)
            {
                drawConnection(dc, connection);
            }

            //绘制地铁站点
            foreach (Station station in subwayMap.Stations)
            {
                drawStation(dc, station);
            }
        }

        private void drawConnection(DrawingContext dc, Connection connection)
        {
            Point pt1 = new Point(connection.BeginStation.X, connection.BeginStation.Y);
            Point pt2 = new Point(connection.EndStation.X, connection.EndStation.Y);

            Pen pen = new Pen(new SolidColorBrush((hexToColor(((SubwayLine)subwayMap.SubwayLines.Find((SubwayLine line) => line.Name == connection.LineName)).Color))), 5);
            pen.LineJoin = PenLineJoin.Round;

            if (connection.Type == 0)
                dc.DrawLine(pen, pt1, pt2);
            //双线并轨，Type = 1 或 2 为不同方向的平移
            else if (connection.Type > 0)
            {
                double scale = (pen.Thickness / 2) / Distance(pt1, pt2);

                double angle = (double)(Math.PI / 2);
                if (connection.Type == 2)
                    angle *= -1;

                //平移线段
                Point pt3 = Rotate(pt2, pt1, angle, scale);
                Point pt4 = Rotate(pt1, pt2, -angle, scale);

                dc.DrawLine(pen, pt3, pt4);
            }
        }

        private void drawStation(DrawingContext dc, Station station)
        {
            //绘制地铁站圆圈
            Pen pen = new Pen(new SolidColorBrush(Colors.Black), station.IsTransfer ? 1 : 0.5);
            double r = station.IsTransfer ? 7 : 5;
            dc.DrawEllipse(Brushes.White, pen, new Point(station.X, station.Y), r, r);

            //绘制地铁站名
            FormattedText formattedText = createFormattedText(station.Name);
            dc.DrawText(formattedText, new Point(station.X - formattedText.Width / 2, station.Y + formattedText.Height - 8));
        }

        private void drawStartAndEndStations(DrawingContext dc)
        {
            //绘制起点
            if (subwayMap.StartStation != null)
            {
                ImageSource startStationImage = BitmapToBitmapSource(Properties.Resources.StartStation);
                double sx = subwayMap.StartStation.X - startStationImage.Width / 2;
                double sy = subwayMap.StartStation.Y - startStationImage.Height;
                dc.DrawImage(startStationImage, new Rect(sx, sy, startStationImage.Width, startStationImage.Height));
            }

            //绘制终点
            if (subwayMap.EndStation != null)
            {
                ImageSource endStationImage = BitmapToBitmapSource(Properties.Resources.EndStation);
                double sx = subwayMap.EndStation.X - endStationImage.Width / 2;
                double sy = subwayMap.EndStation.Y - endStationImage.Height;
                dc.DrawImage(endStationImage, new Rect(sx, sy, endStationImage.Width, endStationImage.Height));
            }
        }

        private void drawCurRoute(DrawingContext dc)
        {
            if (subwayMap.CurRoute == null || subwayMap.CurRoute.Count == 0)
                return;

            //绘制白色遮罩层
            Rect rc = new Rect(-scrollX / zoomScale, -scrollY / zoomScale, Math.Abs(((App)App.Current).MainWindow.ActualWidth / zoomScale), Math.Abs(((App)App.Current).MainWindow.ActualHeight / zoomScale));
            dc.DrawRectangle(new SolidColorBrush(Color.FromArgb(200, 245, 245, 245)), new Pen(Brushes.Black, 0), rc);

            //绘制当前乘车路线
            foreach (Connection connection in subwayMap.CurRoute)
            {
                //绘制路径
                if (connection.Type >= 0)
                {
                    drawConnection(dc, connection);
                }
                else
                {
                    //如果是隐藏的路径，则取反向的可见路径
                    Connection visibleConnection = subwayMap.Connections.Find((Connection curConnection) => curConnection.Type >= 0 && curConnection.BeginStation.Name.Equals(connection.EndStation.Name) && curConnection.EndStation.Name.Equals(connection.BeginStation.Name));
                    if (visibleConnection != null)
                        drawConnection(dc, visibleConnection);
                }

                //绘制站点
                drawStation(dc, connection.BeginStation);
                drawStation(dc, connection.EndStation);
            }
        }

        #endregion

        #region 支撑函数区域

        //以o为源点，旋转v，角度为angle，缩放为scale
        private Point Rotate(Point v, Point o, double angle, double scale)
        {
            v.X -= o.X;
            v.Y -= o.Y;
            double rx = scale * Math.Cos(angle);
            double ry = scale * Math.Sin(angle);
            double x = o.X + v.X * rx - v.Y * ry;
            double y = o.Y + v.X * ry + v.Y * rx;
            return new Point((int)x, (int)y);
        }

        //计算两点距离
        private double Distance(Point pt1, Point pt2)
        {
            return (double)Math.Sqrt((pt1.X - pt2.X) * (pt1.X - pt2.X) + (pt1.Y - pt2.Y) * (pt1.Y - pt2.Y));
        }

        //简化设置FormattedText
        private FormattedText createFormattedText(string text)
        {
            return new FormattedText(text, new System.Globalization.CultureInfo(0x0804), FlowDirection.LeftToRight, new Typeface("Verdana"), 12, Brushes.Black);
        }

        //16进制转颜色
        private Color hexToColor(string color)
        {
            return (Color)ColorConverter.ConvertFromString(color);
        }

        //Bitmap转BitmapSource
        private BitmapSource BitmapToBitmapSource(System.Drawing.Bitmap bitmap)
        {
            IntPtr ptr = bitmap.GetHbitmap();
            BitmapSource result = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(ptr, IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());

            return result;
        }

        private Station GetStationAt(Point pt)
        {
            Point graphPt = ClientToGraph(pt);
            return subwayMap.Stations.FirstOrDefault((Station station) => station.GetStationRect().Contains(graphPt));
        }

        private Point ClientToGraph(Point pt)
        {
            int x = (int)((pt.X - scrollX) / zoomScale);
            int y = (int)((pt.Y - scrollY) / zoomScale);
            return new Point(x, y);
        }

        private Rect ClientToGraph(Rect rect)
        {
            Point pt = ClientToGraph(rect.Location);
            return new Rect(pt.X, pt.Y, (int)(rect.Width / zoomScale), (int)(rect.Height / zoomScale));
        }

        #endregion

        #region 事件区域

        private void UserControl_MouseDown(object sender, MouseButtonEventArgs e)
        {
            mouseDownPoint = e.GetPosition(this);
            mouseLastPoint = mouseDownPoint;
        }

        private void UserControl_MouseUp(object sender, MouseButtonEventArgs e)
        {
            Station station = GetStationAt(e.MouseDevice.GetPosition(this));

            if (station != null)
            {
                if (subwayMap.StartStation == null)
                {
                    subwayMap.StartStation = station;
                    ((MainWindow)((App)App.Current).MainWindow).comboBox_StartStation.Text = station.Name;
                }
                else
                {
                    subwayMap.EndStation = station;
                    ((MainWindow)((App)App.Current).MainWindow).comboBox_EndStation.Text = station.Name;

                    //查找乘车线路
                    Cursor = Cursors.Wait;
                    try
                    {
                        string mode;

                        if (((App)App.Current).IsShortestPlaning)
                            mode = "-b";
                        else
                            mode = "-c";

                        subwayMap.CurRoute = subwayMap.GetDirections(subwayMap.StartStation.Name, subwayMap.EndStation.Name, mode);

                        if ((subwayMap.CurRoute == null || subwayMap.CurRoute.Count == 0))
                            return;

                        displayRouteUnitList.Clear();

                        displayRouteUnitList.Add(new DisplayRouteUnit(subwayMap.CurRoute[0].BeginStation.Name, subwayMap.CurRoute[0].LineName));
                        foreach (Connection connection in subwayMap.CurRoute)
                        {
                            displayRouteUnitList.Add(new DisplayRouteUnit(connection.EndStation.Name, connection.LineName));
                        }

                        InvalidateVisual();
                    }
                    finally
                    {
                        Cursor = Cursors.AppStarting;
                    }
                }
            }
            else if (Distance(e.MouseDevice.GetPosition(this), mouseDownPoint) < 1)//是否发生拖拽
            {
                subwayMap.StartStation = null;
                subwayMap.EndStation = null;
                ((MainWindow)((App)App.Current).MainWindow).comboBox_StartStation.Text = "";
                ((MainWindow)((App)App.Current).MainWindow).comboBox_EndStation.Text = "";
                if (subwayMap.CurRoute != null)
                    subwayMap.CurRoute.Clear();
                displayRouteUnitList.Clear();
            }

            InvalidateVisual();
        }

        private void UserControl_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                scrollX += (e.GetPosition(this).X - mouseLastPoint.X);
                scrollY += (e.GetPosition(this).Y - mouseLastPoint.Y);
                mouseLastPoint = e.GetPosition(this);

                InvalidateVisual();
            }
        }

        private void UserControl_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            zoomScale += (e.Delta > 0 ? 0.1f : -0.1f);

            InvalidateVisual();
        }

        #endregion
    }
}
