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

namespace AdvancedSubwayRoutePlanning
{
    /// <summary>
    /// SubwayGraph.xaml 的交互逻辑
    /// </summary>
    public partial class SubwayGraph : UserControl
    {
        public SubwayGraph()
        {
            InitializeComponent();
        }

        private int scrollX;
        private int scrollY;
        private float zoomScale = 1;
        private Station startStation;
        private Station endStation;
        private List<Connection> curRoute;

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            //消除锯齿
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

            //滚动和缩放
            e.Graphics.TranslateTransform(this.ScrollX, this.ScrollY);
            e.Graphics.ScaleTransform(this.ZoomScale, this.ZoomScale);

            //绘制地铁线路图
            PaintGraph(e.Graphics, this.Graph);

            //绘制当前乘车路线
            PaintCurPath(e.Graphics);

            //绘制起点和终点
            PaintStartAndEndNodes(e.Graphics);

            //绘制线路列表
            PaintLineList(e.Graphics);
        }

        /// <summary>
        /// 绘制线路列表。
        /// </summary>
        /// <param name="g">绘图图面。</param>
        /// <param name="graph">地铁线路图。</param>
        private void PaintLineList(Graphics g)
        {
            if (this.Graph.Lines.Count == 0) return;

            g.ResetTransform();

            //遮罩层
            Rectangle rc = new Rectangle(10, 10, 150, (this.Graph.Lines.Count + 1) * 15);
            using (Brush brush = new SolidBrush(Color.FromArgb(180, Color.White)))
            {
                g.FillRectangle(brush, rc);
            }
            g.DrawRectangle(Pens.Gray, rc);

            //线路列表
            int y = rc.Y + 15;
            foreach (var line in this.Graph.Lines)
            {
                using (Pen pen = new Pen(line.Color, 5))
                {
                    g.DrawLine(pen, rc.X + 15, y, rc.X + 70, y);
                }

                var sz = g.MeasureString(line.Name, this.Font);
                g.DrawString(line.Name, this.Font, Brushes.Black, rc.X + 80, y - sz.Height / 2);

                y += 15;
            }
        }

        /// <summary>
        /// 绘制地铁线路图。
        /// </summary>
        /// <param name="g">绘图图面。</param>
        /// <param name="graph">地铁线路图。</param>
        private void PaintGraph(Graphics g, MetroGraph graph)
        {
            //绘制地铁路径
            foreach (var link in graph.Links.Where(c => c.Flag >= 0))
            {
                PaintLink(g, link);
            }

            //绘制地铁站点
            foreach (var node in graph.Nodes)
            {
                PaintNode(g, node);
            }
        }

        /// <summary>
        /// 绘制地铁站点。
        /// </summary>
        /// <param name="g">绘图图面。</param>
        /// <param name="node">地铁站点。</param>
        private void PaintNode(Graphics g, MetroNode node)
        {
            //绘制站点圆圈
            Color color = node.Links.Count > 2 ? Color.Black : node.Links[0].Line.Color;
            var rect = GetNodeRect(node);
            g.FillEllipse(Brushes.White, rect);
            using (Pen pen = new Pen(color))
            {
                g.DrawEllipse(pen, rect);
            }

            //绘制站点名称
            var sz = g.MeasureString(node.Name, this.Font).ToSize();
            Point pt = new Point(node.X - sz.Width / 2, node.Y + (rect.Height >> 1) + 4);
            g.DrawString(node.Name, Font, Brushes.Black, pt);
        }

        /// <summary>
        /// 绘制地铁站点间的线路。
        /// </summary>
        /// <param name="g">绘图图面。</param>
        /// <param name="link">地铁站点间的线路。</param>
        private void PaintLink(Graphics g, MetroLink link)
        {
            Point pt1 = new Point(link.From.X, link.From.Y);
            Point pt2 = new Point(link.To.X, link.To.Y);

            using (Pen pen = new Pen(link.Line.Color, 5))
            {
                pen.LineJoin = LineJoin.Round;
                if (link.Flag == 0)
                {//单线
                    g.DrawLine(pen, pt1, pt2);
                }
                else if (link.Flag > 0)
                {//双线并轨（如果是同向，则Flag分别为1和2，否则都为1）
                    float scale = (pen.Width / 2) / Distance(pt1, pt2);

                    float angle = (float)(Math.PI / 2);
                    if (link.Flag == 2) angle *= -1;

                    //平移线段
                    var pt3 = Rotate(pt2, pt1, angle, scale);
                    var pt4 = Rotate(pt1, pt2, -angle, scale);

                    g.DrawLine(pen, pt3, pt4);
                }
            }
        }

        /// <summary>
        /// 绘制起点和终点。
        /// </summary>
        /// <param name="g">绘图图面。</param>
        private void PaintStartAndEndNodes(Graphics g)
        {
            //绘制起点
            if (this.StartNode != null)
            {
                var startNodeImage = Properties.Resources.StartNode;
                int sx = this.StartNode.X - startNodeImage.Width / 2;
                int sy = this.StartNode.Y - startNodeImage.Height;
                g.DrawImage(startNodeImage, sx, sy);
            }

            //绘制终点
            if (this.EndNode != null)
            {
                var endNodeImage = Properties.Resources.EndNode;
                int ex = this.EndNode.X - endNodeImage.Width / 2;
                int ey = this.EndNode.Y - endNodeImage.Height;
                g.DrawImage(endNodeImage, ex, ey);
            }
        }

        /// <summary>
        /// 绘制当前乘车路线。
        /// </summary>
        /// <param name="g">绘图图面。</param>
        private void PaintCurPath(Graphics g)
        {
            if (this.CurPath.Links.Count == 0) return;

            //绘制白色遮罩层
            RectangleF rcMask = ClientToMetro(this.ClientRectangle);
            using (Brush brush = new SolidBrush(Color.FromArgb(200, Color.White)))
            {
                g.FillRectangle(brush, rcMask);
            }

            //绘制当前乘车路线
            foreach (var link in this.CurPath.Links)
            {
                //绘制路径
                if (link.Flag >= 0)
                {
                    PaintLink(g, link);
                }
                else
                {
                    //如果是隐藏的路径，则取反向的可见路径
                    var visibleLink = link.To.Links.FirstOrDefault(c => c.Flag >= 0 && c.To == link.From);
                    if (visibleLink != null)
                        PaintLink(g, visibleLink);
                }

                //绘制站点
                PaintNode(g, link.From);
                PaintNode(g, link.To);
            }
        }

        /// <summary>
        /// 获取地铁站点的矩形区域。
        /// </summary>
        /// <param name="node">地铁站点。</param>
        /// <returns></returns>
        private Rectangle GetNodeRect(MetroNode node)
        {
            int r = node.Links.Count > 2 ? 7 : 5;
            return new Rectangle(node.X - r, node.Y - r, (r << 1) + 1, (r << 1) + 1);
        }

        /// <summary>
        /// 矢量v以o为中心点，旋转angle角度，并缩放scale倍。
        /// </summary>
        /// <param name="v">要旋转的点。</param>
        /// <param name="o">中心点。</param>
        /// <param name="angle">旋转角度（以弧度为单位）。</param>
        /// <param name="scale">缩放比例。</param>
        /// <returns>旋转后的点。</returns>
        private Point Rotate(Point v, Point o, float angle, float scale)
        {
            v.X -= o.X;
            v.Y -= o.Y;
            double rx = scale * Math.Cos(angle);
            double ry = scale * Math.Sin(angle);
            double x = o.X + v.X * rx - v.Y * ry;
            double y = o.Y + v.X * ry + v.Y * rx;
            return new Point((int)x, (int)y);
        }

        /// <summary>
        /// 获取两点之间的距离。
        /// </summary>
        /// <param name="pt1">点1。</param>
        /// <param name="pt2">点2。</param>
        /// <returns>两点之间的距离。</returns>
        private float Distance(Point pt1, Point pt2)
        {
            return (float)Math.Sqrt((pt1.X - pt2.X) * (pt1.X - pt2.X) + (pt1.Y - pt2.Y) * (pt1.Y - pt2.Y));
        }
    }
}
