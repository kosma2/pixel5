using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Transactions;

namespace pixelSort
{
    public class Program
    {
        public static int spacing = 3;
        public static List<ColorPos> sortColPos000 = new List<ColorPos>();
        public static List<ColorPos> sortColPos001 = new List<ColorPos>();
        public static List<ColorPos> sortColPos011 = new List<ColorPos>();
        public static List<ColorPos> sortColPos111 = new List<ColorPos>();
        public static List<ColorPos> sortColPos110 = new List<ColorPos>();
        public static List<ColorPos> sortColPos100 = new List<ColorPos>();
        public static List<ColorPos> sortColPos010 = new List<ColorPos>();
        public static List<ColorPos> sortColPos101 = new List<ColorPos>();
        public static int globClusterSize = 10;
        public static int globDensity = 4;

        public class ColorPos
        {
            public Color color { get; set; }
            public Point point { get; set; }
        }

        /* public static int reduceValue(int val)
         {
             if (val <= 63)
             { return 0; }
             if (val > 63 || val < 127)
             { return 127; }
             if (val >= 127 || val < 190)
             { return 127; }
             if (val >= 190 || val <= 255)
             { return 255; }
             else
             {
                 return 0;
             }
         }
         // reduces to 64 colors
         public static int reduceValPro(int val)
         {
             var interval = 255 / 4;

             if (val <= interval)
             { return 0; }
             if (val > interval && val < interval * 2)
             { return interval; }
             if (val >= interval * 2 && val < interval * 3)
             { return interval * 2; }
             if (val >= interval * 3 && val <= 255)
             { return 255; }
             else
             {
                 return 0;
             }
         }*/
        //reduces to 8 colors
        public static int reduceValSimpl(int val)
        {
            var interval = 255 / 2;

            if (val <= interval)
            { return 0; }
            if (val > interval)
            { return 255; }
            else
            {
                return 0;
            }
        }

        public static Color reduceColor(Color col)
        {
            Color newCol = Color.FromArgb(reduceValSimpl(col.R), reduceValSimpl(col.G), reduceValSimpl(col.B));
            return newCol;
        }

        public static void colorsInBins(ColorPos colorPos, int bin)
        {
            //_---- make this switch or case
            if (bin == 000) { sortColPos000.Add(colorPos); }
            if (bin == 001) { sortColPos001.Add(colorPos); }
            if (bin == 011) { sortColPos011.Add(colorPos); }
            if (bin == 111) { sortColPos111.Add(colorPos); }
            if (bin == 100) { sortColPos100.Add(colorPos); }
            if (bin == 110) { sortColPos110.Add(colorPos); }
            if (bin == 010) { sortColPos010.Add(colorPos); }
            if (bin == 101) { sortColPos101.Add(colorPos); }
        }

        public static int colorSort(ColorPos colPos)
        {
            if (colPos.color.R == 0)
            {
                if (colPos.color.G == 0)
                {
                    if (colPos.color.B == 0)
                    {
                        return 000;
                    }
                    if (colPos.color.B == 255)
                    {
                        return 001;
                    }
                }
                if (colPos.color.G == 255)
                {
                    if (colPos.color.B == 0)
                    {
                        return 010;
                    }
                    if (colPos.color.B == 255)
                    {
                        return 011;
                    }
                }
            }

            else if (colPos.color.R == 255)
            {
                if (colPos.color.G == 0)
                {
                    if (colPos.color.B == 0)
                    {
                        return 100;
                    }
                    if (colPos.color.B == 255)
                    {
                        return 101;
                    }
                }
                if (colPos.color.G == 255)
                {
                    if (colPos.color.B == 0)
                    {
                        return 110;
                    }
                    if (colPos.color.B == 255)
                    {
                        return 111;
                    }
                }
            }
            return 111;
        }

        private static Random rng = new Random();

        public static void Shuffle(Point[] theList)
        {
            int a = theList.Length;

            while (a > 1)
            {
                a--;
                int k = rng.Next(a);
                Point value = theList[k];
                theList[k] = theList[a];
                theList[a] = value;
            }
        }

        public static List<ColorPos> picToPixel(Bitmap img)
        // Loop through the images pixels to create ColorPos objects containing color and x and y.

        {
            List<ColorPos> mixColPos = new List<ColorPos>();

            for (int x = 0; x < img.Width; x++)
            {
                for (int y = 0; y < img.Height; y++)
                {
                    Color pixelColor = img.GetPixel(x, y);
                    ColorPos pixColPos = new ColorPos();
                    pixColPos.color = reduceColor(pixelColor);
                    pixColPos.point = new Point(x, y);
                    mixColPos.Add(pixColPos);
                }
            }
            return mixColPos;
        }
        public static int ceiling(int total, int clumpSize)
        {
            double div = (double)total / clumpSize;
            double ceiling = Math.Ceiling(div);
            int ceilInt = (int)ceiling;
            return ceilInt;
        }

        // splitting a list of ColorPos into smaller cluster lists based on batchSize

        public static List<List<Point>> clusterize(List<ColorPos> orgList, int batchSize)
        {
            List<Point> orgPoints = new List<Point>();
            //create a Points array from colPos orgList WITH SPACING
            for (int i = 0; i < orgList.Count - 1; i++)
            {
                ColorPos clr = orgList[i];
                Point pt = new(clr.point.X * spacing, clr.point.Y * spacing);
                orgPoints.Add(pt);
            }


            List<List<Point>> clusterList = new List<List<Point>>();

            while (orgPoints.Count != 0)
            {
/// put cur in the cluster loop
                Point cur = orgPoints[0];
                List<Point> tempCluster = new List<Point>();
                tempCluster.Add(cur);
                orgPoints.RemoveAt(0);


                for (int i = 0; i < orgPoints.Count - 1; i++)
                {
                    if (tempCluster.Count < batchSize)
                    {
                        Point proxPoint = orgPoints[i];
                        if (proxPoint.X - cur.X <= globDensity && proxPoint.Y - cur.Y <= globDensity)
                        {
                            if (proxPoint.X - cur.X >= -globDensity && proxPoint.Y - cur.Y >= -globDensity)
                            {
                                tempCluster.Add(proxPoint);
                                orgPoints.RemoveAt(i);
                            }
                        }
                    }
                }
                clusterList.Add(tempCluster);
            }
            return clusterList;
        }

        public static void Draw(List<ColorPos> colList, Graphics g)
        {
            Color colr = colList[0].color;
            // splitting a list of ColorPos into cluster lists
            int colListLength = colList.Count;
            List<List<Point>> pointClusterList = new List<List<Point>>();
            pointClusterList = clusterize(colList, globClusterSize);
            for (int i = 0; i < pointClusterList.Count; i++)
            {
                Point[] tempCluster = pointClusterList[i].ToArray();
    /// change. include single points
                if (tempCluster.Length > 1)
                {
                    using Pen peni = new(colr,7);
                    g.DrawLines(peni, tempCluster);
                }
            }
        }



        public static void Main()
        {
            try
            {
                // Retrieve the image.
                using Bitmap image = new Bitmap("C:/Users/User/Pictures/elk.jpg", true);
                using Bitmap newImage = new Bitmap(image, 50, 50);
                using Bitmap piktImage = new Bitmap(50 * spacing, 50 * spacing); //PixelFormat.Format16bppGrayScale);
                using Graphics g = Graphics.FromImage(piktImage);
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

                List<Color> pixList = new List<Color>();
                List<Color> reducPixList = new List<Color>();

                int pixCount = 0;

                // Loop through the images pixels to create ColorPos objects containing color and x and y.
                List<ColorPos> mixColPos = picToPixel(newImage);

                /// sort items by color into Lists
                for (int i = 0; i < mixColPos.Count - 1; i++)
                {
                    ColorPos col = mixColPos[i];
                    int bin = colorSort(col);
                    colorsInBins(col, bin);
                }
                // build new bitmap with spacing
                for (int l = 0; l < newImage.Width; l++)
                {
                    for (int o = 0; o < newImage.Height; o++)
                    {
                        ColorPos pixl = mixColPos[pixCount];
                        piktImage.SetPixel(pixl.point.X * spacing, pixl.point.Y * spacing, pixl.color);
                        pixCount++;
                    }
                }


                Draw(sortColPos111, g);
                Draw(sortColPos000, g);
                Draw(sortColPos110, g);
                Draw(sortColPos100, g);

                //newImage.Save("images/face1.jpg");
                piktImage.Save("images/faceReduc.jpg");
            }
            catch (Exception e) { System.Console.WriteLine(e.Message); }
        }
    }
}