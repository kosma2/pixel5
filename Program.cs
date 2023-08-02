using System;
using System.Drawing;
using System.Drawing.Imaging;

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

        public class ColorPos
        {
            public Color color { get; set; }
            public Point point { get; set; }
        }

        public static int reduceValue(int val)
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
        }
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
            //if(bin == 000)
            //{
            //   sortColPos000.Add(colorPos);}
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

        public static void generatePointList(List<List<ColorPos>> colPosLisList)
        {
            // create points list for all separated colors
            for (int s = 0; s < colPosLisList.Count; s++)
            {
                List<ColorPos> colPosList = colPosLisList[s];
                Point[] points = new Point[colPosList.Count() - 1];
                using Pen pen = new(Color.Red);

                //if (lik.Count > 0 && lik.Count < 1000)
                //{
                for (int i = 0; i < colPosList.Count() - 1; i++)
                {
                    ColorPos clr = colPosList[i];
                    pen.Color = clr.color;
                    Point pt = new(clr.point.X * spacing, clr.point.Y * spacing);
                    points[i] = pt;
                }
                //}
                //for(int i=0;i < listOfLists.Count;i++){
                //   Shuffle(listOfLists[i]);
                //}
                //g.DrawLines(pen, points);
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
        public static int ceiling(List<ColorPos> list, int clumpSize)
        // divide the list by five 
        {
            double div = (double)list.Count / clumpSize;
            double ceiling = Math.Ceiling(div);
            int ceilInt = (int)ceiling;
            return ceilInt;
        }

        // splitting a list of ColorPos into smaller cluster lists based on clumpSize
        public static List<List<ColorPos>> clusterize(List<ColorPos> orgList, int clumpSize)
        {
            List<List<ColorPos>> clusterList = new List<List<ColorPos>>();
            //dividing size of orgList by clumpSize
            int rang = 0;
            int listLenght = ceiling(orgList, clumpSize) - 1;
            for (int l = 0; l < listLenght ; l++)
            {
                List<ColorPos> output = orgList.GetRange(rang, clumpSize).ToList();
                clusterList.Add(output);
                rang = rang + clumpSize;
            }
            return clusterList;
        }

        // converting ListList of colPos into a single list of Points
        public static Point[] clusterPointize(List<List<ColorPos>> clusters)
        {
            //count total sum of items in the list
            int total = 0;
            for (int i = 0; i < clusters.Count ; i++)
            {
                for (int j = 0; j < clusters[i].Count ; j++)
                    total++;
            }
            System.Console.WriteLine("color total is "+ total);

            Point[] clusterPoints = new Point[total];
            int cnt = 0;
            for (int o = 0; o < clusters.Count(); o++)
            {
                for (int i = 0; i < clusters[o].Count(); i++)
                {
                    ColorPos clr = clusters[o][i];
                    Point pt = new(clr.point.X * spacing, clr.point.Y * spacing);
                    clusterPoints[cnt] = pt;
                    cnt++;
                }
            }
            for (int n = 0; n < clusterPoints.Count(); n++)
            {
                Shuffle(clusterPoints[n]);
            }

            return clusterPoints;
        }
        public static void Draw(List<ColorPos> colList, Graphics g){
                Color colr = colList[0].color;
                // splitting a list of ColorPos into cluster lists
                List<List<ColorPos>> clusters = clusterize(colList,20);
                // creating a Points array from cluster colorPos lists
                System.Console.WriteLine("clusters"+clusters.Count);
                Point[] clusterPoints = clusterPointize(clusters);
                //Shuffle(clusterPoints);

                using Pen peni = new(colr);
                g.DrawLines(peni, clusterPoints);
                }



        public static void Main()
        {
            try
            {
                // Retrieve the image.
                using Bitmap image = new Bitmap("images/face.jpg", true);
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

                // put sorted colorPos lists in a master list for drawing
                /*List<List<ColorPos>> listOfLists = new List<List<ColorPos>>();
                listOfLists.Add(sortColPos111);
                listOfLists.Add(sortColPos110);
                listOfLists.Add(sortColPos100);
                //listOfLists.Add(sortColPos000);*/

                Draw(sortColPos111,g);
                Draw(sortColPos000,g);
                Draw(sortColPos110,g);
                Draw(sortColPos100,g);
                


                //System.Console.WriteLine(clusterPoints.Count());
                //for (int i = 0; i < output.Count; i++)
                //{
                //    System.Console.WriteLine(output[i]);
                //}

                //newImage.Save("images/face1.jpg");
                piktImage.Save("images/faceReduc.jpg");
            }
            catch (Exception e) { System.Console.WriteLine(e.Message); }
        }
    }
}