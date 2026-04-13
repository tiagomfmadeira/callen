using System;
using System.Collections.Concurrent;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Callen
{
    public class TintedIcon : Image
    {
        private static readonly ConcurrentDictionary<string, ImageSource> Cache =
            new ConcurrentDictionary<string, ImageSource>(StringComparer.Ordinal);

        public static readonly DependencyProperty MaskSourceProperty =
            DependencyProperty.Register(
                nameof(MaskSource),
                typeof(ImageSource),
                typeof(TintedIcon),
                new PropertyMetadata(null, OnTintInputChanged));

        public static readonly DependencyProperty TintProperty =
            DependencyProperty.Register(
                nameof(Tint),
                typeof(Color),
                typeof(TintedIcon),
                new PropertyMetadata(Colors.White, OnTintInputChanged));

        public ImageSource MaskSource
        {
            get => (ImageSource)GetValue(MaskSourceProperty);
            set => SetValue(MaskSourceProperty, value);
        }

        public Color Tint
        {
            get => (Color)GetValue(TintProperty);
            set => SetValue(TintProperty, value);
        }

        private static void OnTintInputChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var icon = d as TintedIcon;
            icon?.UpdateTintedSource();
        }

        private void UpdateTintedSource()
        {
            var bitmap = MaskSource as BitmapSource;
            if (bitmap == null)
            {
                Source = MaskSource;
                return;
            }

            var frozenMask = bitmap.IsFrozen ? bitmap : bitmap.GetAsFrozen() as BitmapSource;
            if (frozenMask == null)
            {
                Source = bitmap;
                return;
            }

            var key = GetCacheKey(frozenMask, Tint);
            if (Cache.TryGetValue(key, out var cached))
            {
                Source = cached;
                return;
            }

            var tinted = CreateTintedBitmap(frozenMask, Tint);
            Cache[key] = tinted;
            Source = tinted;
        }

        private static string GetCacheKey(BitmapSource source, Color tint)
        {
            string sourceKey;

            if (source is BitmapImage bitmapImage && bitmapImage.UriSource != null)
                sourceKey = bitmapImage.UriSource.ToString();
            else
                sourceKey = $"{source.PixelWidth}x{source.PixelHeight}:{source.Format}";

            return $"{sourceKey}|{tint.A:X2}{tint.R:X2}{tint.G:X2}{tint.B:X2}";
        }

        private static ImageSource CreateTintedBitmap(BitmapSource source, Color tint)
        {
            var converted = source.Format == PixelFormats.Bgra32
                ? source
                : new FormatConvertedBitmap(source, PixelFormats.Bgra32, null, 0);

            var width = converted.PixelWidth;
            var height = converted.PixelHeight;
            var stride = width * 4;
            var pixels = new byte[stride * height];
            converted.CopyPixels(pixels, stride, 0);

            for (var i = 0; i < pixels.Length; i += 4)
            {
                var alpha = pixels[i + 3];
                if (alpha == 0)
                    continue;

                pixels[i] = tint.B;
                pixels[i + 1] = tint.G;
                pixels[i + 2] = tint.R;
                pixels[i + 3] = (byte)(alpha * tint.A / 255);
            }

            var writeable = new WriteableBitmap(width, height, converted.DpiX, converted.DpiY, PixelFormats.Bgra32, null);
            writeable.WritePixels(new Int32Rect(0, 0, width, height), pixels, stride, 0);
            writeable.Freeze();

            return writeable;
        }
    }
}
