using System.Collections.Generic;
using System.Net.Http.Headers;

namespace ConvexHull
{
    public partial class Form1 : Form
    {
        private Bitmap drawingBitmap;
        private Graphics g;
        private List<Point> points;
        public Form1()
        {
            InitializeComponent();
            drawingBitmap = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            g = Graphics.FromImage(drawingBitmap);
            pictureBox1.Image = drawingBitmap;
            points = new List<Point>();
        }

        public class PointComparer : IComparer<Point>
        {
            public int Compare(Point p1, Point p2)
            {
                if (p1.X == p2.X)
                {
                    return p1.Y - p2.Y;
                }
                else
                {
                    return p1.X - p2.X;
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (points.Count > 1)
            {
                g.DrawPolygon(new Pen(Color.Green), convexAndrewHull().ToArray());
                pictureBox1.Invalidate();
            }
        }
        public static long cross(Point O, Point A, Point B)
        {
            return (A.X - O.X) * (long)(B.Y - O.Y) - (A.Y - O.Y) * (long)(B.X - O.X);
        }

        private List<Point> convexAndrewHull() 
        {

            points = points.Distinct().ToList();
            points.Sort(new PointComparer());

            if (points.Count <= 1)
            {
                return points;
            }

            // 2D cross product of OA and OB vectors, i.e. z-component of their 3D cross product.
            // Returns a positive value, if OAB makes a counter-clockwise turn,
            // negative for clockwise turn, and zero if the points are collinear.


            // Build lower hull 
            List<Point> lower = new List<Point>();
            foreach (var p in points)
            {
                while (lower.Count >= 2 && cross(lower[lower.Count - 2], lower[lower.Count - 1], p) <= 0)
                {
                    lower.RemoveAt(lower.Count - 1);
                }
                lower.Add(p);
            }

            // Build upper hull
            List<Point> upper = new List<Point>();
            foreach (var p in points.AsEnumerable().Reverse())
            {
                while (upper.Count >= 2 && cross(upper[upper.Count - 2], upper[upper.Count - 1], p) <= 0)
                {
                    upper.RemoveAt(upper.Count - 1);
                }
                upper.Add(p);
            }

            // Concatenation of the lower and upper hulls gives the convex hull.
            // Last point of each list is omitted because it is repeated at the beginning of the other list. 
            lower.RemoveAt(lower.Count - 1);
            upper.RemoveAt(upper.Count - 1);
            lower.AddRange(upper);
            return lower;
        }

        private void pictureBox1_MouseClick(object sender, MouseEventArgs e)
        {
            points.Add(new Point(e.X, e.Y));
            SolidBrush brush = new SolidBrush(Color.Black);
            g.FillEllipse(brush, e.X, e.Y, 5, 5);
            pictureBox1.Invalidate();

        }

        private void button2_Click(object sender, EventArgs e)
        {
            Clear();
        }

        public void Clear() 
        {
            points.Clear();
            g.Clear(Color.White);
            pictureBox1.Invalidate();
        }
    }
}