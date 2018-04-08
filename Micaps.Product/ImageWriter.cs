using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.IO;
using Esri.ArcGISRuntime.Controls;
using System.Windows.Controls;
using Esri.ArcGISRuntime.Geometry;

namespace Pmss.Micaps.Product
{
    public enum ImageFormat { JPG, BMP, PNG, GIF, TIF }


    public class ImageWriter
    {
        //public bool IsValidCountryMap(MapView view)
        //{
        //    MapPoint minLonLat = new MapPoint(70, 17, SpatialReferences.Wgs84);
        //    MapPoint maxLonLat = new MapPoint(137, 56, SpatialReferences.Wgs84);

        //    MapPoint minXY = GeometryEngine.Project(minLonLat, SpatialReferences.WebMercator) as MapPoint;
        //    MapPoint maxXY = GeometryEngine.Project(maxLonLat, SpatialReferences.WebMercator) as MapPoint;

        //    var envelope = GetCurrentEnvelop(view);

        //    return false;
        //}
        public ImageWriter(ProductSaveOption option, string title, string startDate, string endDate, string publishDate, int templateType = 0)
        {
            this.saveOption = option;
            this.startDate = startDate;
            this.endDate = endDate;
            this.publishDate = publishDate;
            this.title = title;
            if (templateType == 20)
                this.isTemplate20 = true;
            else if (templateType == 8)
                this.isTemplate08 = true;
        }

        public void Write(string folderPath, MapView view, bool drawImage = true)
        {
            if (!Directory.Exists(folderPath))
            {
                throw new DirectoryNotFoundException(folderPath);
            }

            folderPath = folderPath.TrimEnd('\\');
            //using (var stream = new FileStream(folderPath + "\\tmp.png", FileMode.OpenOrCreate))
            using (var stream = new MemoryStream())
            {
                var control = view as Control;
                var rtb = RenderVisaulToBitmap(view, Convert.ToInt32(control.ActualWidth), Convert.ToInt32(control.ActualHeight));
                GenerateImage(rtb, ImageFormat.GIF, stream);

                stream.Position = 0;
                System.Drawing.Image initialImg = System.Drawing.Image.FromStream(stream, true);
                // Cut initialImg to 4:3
                //var envelope = GetCurrentEnvelop(view);
                //double xDiff = Math.Abs(Constants.XMin - envelope.XMin + envelope.XMax - Constants.XMax);
                //double yDiff = Math.Abs(Constants.YMin - envelope.YMin + envelope.YMax - Constants.YMax);
                double width = initialImg.Width;
                double height = initialImg.Height;
                System.Drawing.Rectangle fromRect;
                System.Drawing.Rectangle toRect;
                if ((width / height) > ((double)Constants.ImageWidth / (double)Constants.ImageHeight))// Cut X
                {
                    //width = Convert.ToInt32(initialImg.Width * (Constants.XMax - Constants.XMin) / (envelope.XMax - envelope.XMin));
                    width = height * Constants.ImageWidth / Constants.ImageHeight;
                    fromRect = new System.Drawing.Rectangle(Convert.ToInt32((initialImg.Width - width) / 2), 0, (int)width, (int)height);
                }
                else// Cut Y
                {
                    //height = Convert.ToInt32(initialImg.Height * (Constants.YMax - Constants.YMin) / (envelope.YMax - envelope.YMin));
                    height = width * Constants.ImageHeight / Constants.ImageWidth;
                    fromRect = new System.Drawing.Rectangle(0, Convert.ToInt32((initialImg.Height - height) / 2), (int)width, (int)height);
                }
                toRect = new System.Drawing.Rectangle(0, 0, Constants.ImageWidth, Constants.ImageHeight);
                var cuttedImg = new System.Drawing.Bitmap(Constants.ImageWidth, Constants.ImageHeight);

                // Draw on the new Image
                this.graphics = System.Drawing.Graphics.FromImage(cuttedImg);
                this.graphics.DrawImage(initialImg, toRect, fromRect, System.Drawing.GraphicsUnit.Pixel);
                initialImg.Dispose();

                // Add Text
                DrawBorder();
                DrawLogo();
                DrawTitle();
                DrawStartEndDate();
                DrawAuthor();
                if (drawImage)
                {
                    DrawImage();
                }
                DrawPublishDate();
                // Add Picture

                string fileName = $"{folderPath}\\{DateTime.Now.ToString("yyyyMMdd_HHmmss")}.gif";
                cuttedImg.Save(fileName);
                cuttedImg.Dispose();
                this.graphics.Dispose();
            }
        }

        private void AddTextToCountryImage(System.Drawing.Graphics graphic)
        {
            // Title
            System.Drawing.Font font = new System.Drawing.Font("宋体", 36, System.Drawing.FontStyle.Bold);
            System.Drawing.Brush brush = new System.Drawing.SolidBrush(System.Drawing.Color.Black);
            System.Drawing.RectangleF rect = new System.Drawing.RectangleF(390, 110, 800, 60);
            System.Drawing.StringFormat format = new System.Drawing.StringFormat
            {
                Alignment = System.Drawing.StringAlignment.Center
            };
            graphic.DrawString("哈哈", font, brush, rect, format);
            // Date
            font = new System.Drawing.Font("宋体", 26);
            rect = new System.Drawing.RectangleF(560, 180, 450, 55);
            graphic.DrawString("哈哈", font, brush, rect, format);
            // Author
            font = new System.Drawing.Font("宋体", 26, System.Drawing.FontStyle.Bold);
            rect = new System.Drawing.RectangleF(650, 240, 300, 55);
            graphic.DrawString("中国气象台", font, brush, rect, format);
            // Date
            rect = new System.Drawing.RectangleF(1250, 1120, 300, 55);
            graphic.DrawString("haha", font, brush, rect, format);
            // Image
            using (System.Drawing.Image img = System.Drawing.Image.FromFile("Img/Product/SouthSea.PNG"))
            {
                System.Drawing.Rectangle fromRect = new System.Drawing.Rectangle(0, 0, img.Width, img.Height);
                System.Drawing.Rectangle toRect = new System.Drawing.Rectangle(15, 930, 200, 240);
                graphic.DrawImage(img, toRect, fromRect, System.Drawing.GraphicsUnit.Pixel);
            }
            using (System.Drawing.Image img = System.Drawing.Image.FromFile("Img/Product/FloodLevel.PNG"))
            {
                System.Drawing.Rectangle fromRect = new System.Drawing.Rectangle(0, 0, img.Width, img.Height);
                System.Drawing.Rectangle toRect = new System.Drawing.Rectangle(215, 930, 200, 240);
                graphic.DrawImage(img, toRect, fromRect, System.Drawing.GraphicsUnit.Pixel);
            }

        }

        private void DrawBorder()
        {
            System.Drawing.Pen pen = new System.Drawing.Pen(System.Drawing.Color.Black);
            System.Drawing.Rectangle rect = new System.Drawing.Rectangle(7, 7, Constants.ImageWidth - 14, Constants.ImageHeight - 14);
            this.graphics.DrawRectangle(pen, rect);

            rect = new System.Drawing.Rectangle(11, 11, Constants.ImageWidth - 22, Constants.ImageHeight - 22);
            this.graphics.DrawRectangle(pen, rect);

            rect = new System.Drawing.Rectangle(15, 15, Constants.ImageWidth - 30, Constants.ImageHeight - 30);
            this.graphics.DrawRectangle(pen, rect);
        }
        private void DrawLogo()
        {
            if (this.saveOption.ProductRegion == ProductRegionEnum.Country && this.isTemplate20 && (this.saveOption.ProductType == ProductTypeEnum.Disaster || this.saveOption.ProductType == ProductTypeEnum.Flood))
            {
                return;
            }

            using (System.Drawing.Image img = System.Drawing.Image.FromFile("Img/Product/qixiangjuLogo.png"))
            {
                System.Drawing.Rectangle fromRect = new System.Drawing.Rectangle(0, 0, img.Width, img.Height);
                System.Drawing.Rectangle toRect = new System.Drawing.Rectangle(50, 50, 146, 146);
                this.graphics.DrawImage(img, toRect, fromRect, System.Drawing.GraphicsUnit.Pixel);
            }
        }
        private void DrawTitle()
        {
            System.Drawing.Font font = new System.Drawing.Font("宋体", 48, System.Drawing.FontStyle.Bold);
            var size = this.graphics.MeasureString(this.title, font);
            float startX = (Constants.ImageWidth - size.Width) / 2;

            System.Drawing.Brush brush = new System.Drawing.SolidBrush(System.Drawing.Color.Black);
            System.Drawing.RectangleF rect = new System.Drawing.RectangleF(startX, 110, size.Width, 80);
            System.Drawing.StringFormat format = new System.Drawing.StringFormat
            {
                Alignment = System.Drawing.StringAlignment.Center
            };

            System.Drawing.RectangleF rectBg = new System.Drawing.RectangleF(startX, 110 - 20, size.Width, 80 + 20);
            this.graphics.FillRectangle(new System.Drawing.SolidBrush(System.Drawing.Color.White), rectBg);
            this.graphics.DrawString(this.title, font, brush, rect, format);
        }
        private void DrawStartEndDate()
        {
            string startEndDate = $"{this.startDate}--{this.endDate}";
            System.Drawing.Font font = new System.Drawing.Font("宋体", 34, System.Drawing.FontStyle.Bold);
            var size = this.graphics.MeasureString(startEndDate, font);
            float startX = (Constants.ImageWidth - size.Width) / 2;

            System.Drawing.Brush brush = new System.Drawing.SolidBrush(System.Drawing.Color.Black);
            System.Drawing.RectangleF rect = new System.Drawing.RectangleF(startX, 210, size.Width, 60);
            System.Drawing.StringFormat format = new System.Drawing.StringFormat
            {
                Alignment = System.Drawing.StringAlignment.Center
            };

            System.Drawing.RectangleF rectBg = new System.Drawing.RectangleF(startX, 210 - 20, size.Width, 60 + 20);
            this.graphics.FillRectangle(new System.Drawing.SolidBrush(System.Drawing.Color.White), rectBg);
            this.graphics.DrawString(startEndDate, font, brush, rect, format);
        }
        private void DrawAuthor()
        {
            System.Drawing.Font font = new System.Drawing.Font("宋体", 42, System.Drawing.FontStyle.Bold);
            System.Drawing.Brush brush = new System.Drawing.SolidBrush(System.Drawing.Color.Black);
            System.Drawing.StringFormat format = new System.Drawing.StringFormat
            {
                Alignment = System.Drawing.StringAlignment.Center
            };
            System.Drawing.RectangleF rect = new System.Drawing.RectangleF(0, 280, Constants.ImageWidth, 60);

            if (this.saveOption.ProductRegion == ProductRegionEnum.Region
                || (this.saveOption.ProductRegion == ProductRegionEnum.Country && this.isTemplate20 && (this.saveOption.ProductType == ProductTypeEnum.Disaster || this.saveOption.ProductType == ProductTypeEnum.Flood)))
            {
                rect = new System.Drawing.RectangleF(1600, 1453, Constants.ImageWidth - 1600, 60);
                font = new System.Drawing.Font("宋体", 28, System.Drawing.FontStyle.Bold);
            }

            string author = "中央气象台";
            if (this.saveOption.ProductRegion == ProductRegionEnum.Country && this.isTemplate20 && this.saveOption.ProductType == ProductTypeEnum.Disaster)
            {
                author = "国土资源部 中国气象局";
            }
            if (this.saveOption.ProductRegion == ProductRegionEnum.Country && this.isTemplate20 && this.saveOption.ProductType == ProductTypeEnum.Flood)
            {
                author = "水利部 中国气象局";
            }

            this.graphics.DrawString(author, font, brush, rect, format);
        }
        private void DrawImage()
        {
            int yStart = 1230;
            string levelImgPath = "Img/Product/Flood.PNG";
            if (saveOption.ProductType == ProductTypeEnum.Disaster)
            {
                levelImgPath = "Img/Product/Disaster.PNG";
                yStart = 1256;
            }
            else if (saveOption.ProductType == ProductTypeEnum.Zilao)
            {
                levelImgPath = "Img/Product/Zilao.PNG";
            }
            else if (saveOption.ProductType == ProductTypeEnum.River)
            {
                levelImgPath = "Img/Product/River.PNG";
            }
            else if (saveOption.ProductType == ProductTypeEnum.Flood && this.isTemplate08)
            {
                levelImgPath = "Img/Product/Flood08.png";
            }

            if (saveOption.ProductRegion == ProductRegionEnum.Country)
            {
                using (System.Drawing.Image img = System.Drawing.Image.FromFile("Img/Product/SouthSea.PNG"))
                {
                    System.Drawing.Rectangle fromRect = new System.Drawing.Rectangle(0, 0, img.Width, img.Height);
                    System.Drawing.Rectangle toRect = new System.Drawing.Rectangle(53, 1230, img.Width, img.Height);
                    this.graphics.DrawImage(img, toRect, fromRect, System.Drawing.GraphicsUnit.Pixel);
                }
                using (System.Drawing.Image img = System.Drawing.Image.FromFile(levelImgPath))
                {
                    System.Drawing.Rectangle fromRect = new System.Drawing.Rectangle(0, 0, img.Width, img.Height);
                    System.Drawing.Rectangle toRect = new System.Drawing.Rectangle(343, yStart, img.Width, img.Height);
                    this.graphics.DrawImage(img, toRect, fromRect, System.Drawing.GraphicsUnit.Pixel);
                }
            }
            else
            {
                using (System.Drawing.Image img = System.Drawing.Image.FromFile(levelImgPath))
                {
                    System.Drawing.Rectangle fromRect = new System.Drawing.Rectangle(0, 0, img.Width, img.Height);
                    System.Drawing.Rectangle toRect = new System.Drawing.Rectangle(53, yStart, img.Width, img.Height);
                    this.graphics.DrawImage(img, toRect, fromRect, System.Drawing.GraphicsUnit.Pixel);
                }

            }
        }
        private void DrawPublishDate()
        {
            System.Drawing.Font font = new System.Drawing.Font("宋体", 28, System.Drawing.FontStyle.Bold);
            System.Drawing.Brush brush = new System.Drawing.SolidBrush(System.Drawing.Color.Black);
            System.Drawing.RectangleF rect = new System.Drawing.RectangleF(1600, 1513, Constants.ImageWidth - 1600, 60);
            System.Drawing.StringFormat format = new System.Drawing.StringFormat
            {
                Alignment = System.Drawing.StringAlignment.Center
            };

            this.graphics.DrawString(this.publishDate, font, brush, rect, format);
        }

        private RenderTargetBitmap RenderVisaulToBitmap(Visual vsual, int width, int height)
        {
            int factor = 1;
            double dpi = 96;
            var rtb = new RenderTargetBitmap(width * factor, height * factor, dpi, dpi, PixelFormats.Default);
            rtb.Render(vsual);

            return rtb;
        }

        private void GenerateImage(BitmapSource bitmap, ImageFormat format, Stream destStream)
        {
            BitmapEncoder encoder = null;

            switch (format)
            {
                case ImageFormat.JPG:
                    encoder = new JpegBitmapEncoder();
                    break;
                case ImageFormat.PNG:
                    encoder = new PngBitmapEncoder();
                    break;
                case ImageFormat.BMP:
                    encoder = new BmpBitmapEncoder();
                    break;
                case ImageFormat.GIF:
                    encoder = new GifBitmapEncoder();
                    break;
                case ImageFormat.TIF:
                    encoder = new TiffBitmapEncoder();
                    break;
                default:
                    throw new InvalidOperationException();
            }

            encoder.Frames.Add(BitmapFrame.Create(bitmap));
            encoder.Save(destStream);
        }

        private Envelope GetCurrentEnvelop(MapView view)
        {
            Envelope newExtent = null;

            // Get current viewpoints extent from the MapView
            var currentViewpoint = view.GetCurrentViewpoint(ViewpointType.BoundingGeometry);
            var viewpointExtent = currentViewpoint.TargetGeometry.Extent;

            if (view.WrapAround)
            {
                var normalizedExtent = GeometryEngine.NormalizeCentralMeridian(viewpointExtent);
                if (normalizedExtent is Polygon)
                {
                    var normalizedPolygon = (Polygon)normalizedExtent;

                    if (normalizedPolygon.Parts.Count == 1)
                        newExtent = normalizedPolygon.Extent;
                    else
                    {
                        var newExtentBuilder = new EnvelopeBuilder(view.SpatialReference);

                        foreach (var p in normalizedPolygon.Parts[0].GetPoints())
                        {
                            if (p.X < newExtentBuilder.XMin || double.IsNaN(newExtentBuilder.XMin))
                                newExtentBuilder.XMin = p.X;
                            if (p.Y < newExtentBuilder.YMin || double.IsNaN(newExtentBuilder.YMin))
                                newExtentBuilder.YMin = p.Y;
                        }

                        foreach (var p in normalizedPolygon.Parts[1].GetPoints())
                        {
                            if (p.X > newExtentBuilder.XMax || double.IsNaN(newExtentBuilder.XMax))
                                newExtentBuilder.XMax = p.X;
                            if (p.Y > newExtentBuilder.YMax || double.IsNaN(newExtentBuilder.YMax))
                                newExtentBuilder.YMax = p.Y;
                        }
                        newExtent = newExtentBuilder.ToGeometry();
                    }
                }
                else if (normalizedExtent is Envelope)
                    newExtent = normalizedExtent as Envelope;
            }
            else
                newExtent = viewpointExtent;

            return newExtent;
        }

        private ProductSaveOption saveOption;
        private string startDate;
        private string endDate;
        private string publishDate;
        private string title;
        private bool isTemplate20 = false;
        private bool isTemplate08 = false;
        private System.Drawing.Graphics graphics;
    }
}
