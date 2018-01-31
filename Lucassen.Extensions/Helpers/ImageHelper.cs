namespace Lucassen.Extensions.Helpers
{
    using System;
    using System.Drawing;
    using System.Drawing.Drawing2D;
    using System.Drawing.Imaging;

    public class ImageHelper
    {
        internal static Image Combine(Image[] images, int width, int height, Color backgroundColor)
        {
            var canvas = Resize(images[0], width, height, ImageResizeMode.Fill, backgroundColor);
            var graphics = Graphics.FromImage(canvas);
            for (var i = 1; i < images.Length; i++)
            {
                var overlay = new Bitmap(Resize(images[i], width, height, ImageResizeMode.Fill, backgroundColor));
                graphics.DrawImage(overlay, new Point(0, 0));
            }

            return canvas;
        }

        internal static Image Resize(string image, int width, int height, ImageResizeMode resizeMode, Color backgroundColor)
        {
            var i = Image.FromFile(image);
            return Resize(i, width, height, resizeMode, backgroundColor);
        }

        internal static Image Resize(Image image, int width, int height, ImageResizeMode resizeMode, Color backgroundColor)
        {
            var sourceWidth = image.Width;
            var sourceHeight = image.Height;

            var targetWidth = width;
            var targetHeight = height;

            float ratio;
            var ratioWidth = targetWidth / (float) sourceWidth;
            var ratioHeight = targetHeight / (float) sourceHeight;

            if (ratioHeight < ratioWidth)
                ratio = ratioHeight;
            else
                ratio = ratioWidth;

            Bitmap newImage = null;

            switch (resizeMode)
            {
                case ImageResizeMode.Normal:
                {
                    var destWidth = (int) (sourceWidth * ratio);
                    var destHeight = (int) (sourceHeight * ratio);

                    newImage = new Bitmap(destWidth, destHeight);
                    using (var graphics = Graphics.FromImage(newImage))
                    {
                        graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                        graphics.DrawImage(image, 0, 0, destWidth, destHeight);
                    }

                    break;
                }
                case ImageResizeMode.Crop:
                {
                    ratio = ratioHeight > ratioWidth ? ratioHeight : ratioWidth;

                    var destWidth = (int) Math.Ceiling(sourceWidth * ratio);
                    var destHeight = (int) Math.Ceiling(sourceHeight * ratio);

                    newImage = new Bitmap(targetWidth, targetHeight);

                    var startX = 0;
                    var startY = 0;

                    if (destWidth > targetWidth)
                        startX = 0 - (destWidth - targetWidth) / 2;

                    if (destHeight > targetHeight)
                        startY = 0 - (destHeight - targetHeight) / 2;

                    using (var graphics = Graphics.FromImage(newImage))
                    {
                        graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                        graphics.Clear(backgroundColor);
                        graphics.DrawImage(image, startX, startY, destWidth, destHeight);
                    }

                    break;
                }
                case ImageResizeMode.Stretch:
                {
                    newImage = new Bitmap(targetWidth, targetHeight);
                    using (var graphics = Graphics.FromImage(newImage))
                    {
                        graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                        graphics.DrawImage(image, 0, 0, targetWidth, targetHeight);
                    }

                    break;
                }
                case ImageResizeMode.Fill:
                {
                    newImage = new Bitmap(targetWidth, targetHeight);

                    var destWidth = (int) (sourceWidth * ratio);
                    var destHeight = (int) (sourceHeight * ratio);

                    var startX = 0;
                    var startY = 0;

                    if (destWidth < targetWidth)
                        startX = 0 + (targetWidth - destWidth) / 2;

                    if (destHeight < targetHeight)
                        startY = 0 + (targetHeight - destHeight) / 2;

                    using (var graphics = Graphics.FromImage(newImage))
                    {
                        graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                        graphics.Clear(backgroundColor);
                        graphics.DrawImage(image, startX, startY, destWidth, destHeight);
                    }

                    break;
                }
            }

            return newImage;
        }

        internal static string GetExtension(string filename)
        {
            var pieces = filename.Split('.');
            return pieces[pieces.Length - 1].ToLower();
        }

        internal static ImageFormat GetImageFormatByExtension(string filename)
        {
            var extension = GetExtension(filename);
            switch (extension)
            {
                case "jpg":
                case "jpeg":
                    return ImageFormat.Jpeg;
                case "bmp":
                    return ImageFormat.Bmp;
                case "gif":
                    return ImageFormat.Gif;
                case "png":
                    return ImageFormat.Png;
                case "tiff":
                    return ImageFormat.Tiff;
                default:
                    return null;
            }
        }

        internal static bool IsImage(string filename)
        {
            var extension = GetExtension(filename);
            switch (extension)
            {
                case "jpg":
                case "jpeg":
                case "bmp":
                case "gif":
                case "png":
                case "tiff":
                    return true;
            }

            return false;
        }
    }

    /// <summary>
    ///     Modes available for the resize method of images.
    /// </summary>
    public enum ImageResizeMode
    {
        /// <summary>
        ///     This will resize images to the resolution nearest to the target resolution. Images can become smaller when using
        ///     this option
        /// </summary>
        Normal = 1,

        /// <summary>
        ///     This will stretch an image so it always is the exact dimensions of the target resolution
        /// </summary>
        Stretch = 2,

        /// <summary>
        ///     This will size an image to the exact dimensions of the target resolution, keeping ratio in mind and cropping parts
        ///     that can't
        ///     fit in the picture.
        /// </summary>
        Crop = 3,

        /// <summary>
        ///     This will size an image to the exact dimensions of the target resolution, keeping ratio in mind and filling up the
        ///     image
        ///     with black bars when some parts remain empty.
        /// </summary>
        Fill = 4
    }
}