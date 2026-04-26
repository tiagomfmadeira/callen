using System;
using System.Collections.Concurrent;
using System.Runtime.CompilerServices;
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
            get => ResolveTintColor(GetValue(TintProperty));
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
            if (bitmap != null)
            {
                var frozenMask = bitmap.IsFrozen ? bitmap : bitmap.GetAsFrozen() as BitmapSource;
                if (frozenMask == null)
                {
                    Source = bitmap;
                    return;
                }

                var bitmapKey = GetBitmapCacheKey(frozenMask, Tint);
                if (Cache.TryGetValue(bitmapKey, out var cachedBitmap))
                {
                    Source = cachedBitmap;
                    return;
                }

                var tintedBitmap = CreateTintedBitmap(frozenMask, Tint);
                Cache[bitmapKey] = tintedBitmap;
                Source = tintedBitmap;
                return;
            }

            var drawingImage = MaskSource as DrawingImage;
            if (drawingImage != null)
            {
                var drawingKey = GetDrawingCacheKey(drawingImage, Tint);
                if (Cache.TryGetValue(drawingKey, out var cachedDrawing))
                {
                    Source = cachedDrawing;
                    return;
                }

                var tintedDrawing = CreateTintedDrawing(drawingImage, Tint);
                Cache[drawingKey] = tintedDrawing;
                Source = tintedDrawing;
                return;
            }

            Source = MaskSource;
        }

        private static string GetBitmapCacheKey(BitmapSource source, Color tint)
        {
            string sourceKey;

            if (source is BitmapImage bitmapImage && bitmapImage.UriSource != null)
                sourceKey = bitmapImage.UriSource.ToString();
            else
                sourceKey = $"{source.PixelWidth}x{source.PixelHeight}:{source.Format}";

            return $"{sourceKey}|{tint.A:X2}{tint.R:X2}{tint.G:X2}{tint.B:X2}";
        }

        private static string GetDrawingCacheKey(DrawingImage source, Color tint)
        {
            var drawingId = source.Drawing == null ? 0 : RuntimeHelpers.GetHashCode(source.Drawing);
            return $"{drawingId}|{tint.A:X2}{tint.R:X2}{tint.G:X2}{tint.B:X2}";
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

        private static ImageSource CreateTintedDrawing(DrawingImage source, Color tint)
        {
            if (source.Drawing == null)
                return source;

            var clonedDrawing = source.Drawing.CloneCurrentValue();
            ApplyTintToDrawing(clonedDrawing, tint);
            clonedDrawing.Freeze();

            var image = new DrawingImage(clonedDrawing);
            image.Freeze();
            return image;
        }

        private static void ApplyTintToDrawing(Drawing drawing, Color tint)
        {
            if (drawing == null)
                return;

            var group = drawing as DrawingGroup;
            if (group != null)
            {
                foreach (var child in group.Children)
                    ApplyTintToDrawing(child, tint);
                return;
            }

            var geometry = drawing as GeometryDrawing;
            if (geometry != null)
            {
                geometry.Brush = CreateTintedBrush(geometry.Brush, tint);
                if (geometry.Pen != null)
                {
                    var pen = geometry.Pen.CloneCurrentValue();
                    pen.Brush = CreateTintedBrush(pen.Brush, tint);
                    pen.Freeze();
                    geometry.Pen = pen;
                }
                return;
            }

            var glyph = drawing as GlyphRunDrawing;
            if (glyph != null)
                glyph.ForegroundBrush = CreateTintedBrush(glyph.ForegroundBrush, tint);
        }

        private static Brush CreateTintedBrush(Brush sourceBrush, Color tint)
        {
            byte sourceAlpha;
            if (sourceBrush is SolidColorBrush solid)
                sourceAlpha = solid.Color.A;
            else
                sourceAlpha = 255;

            var alpha = (byte)(sourceAlpha * tint.A / 255);
            var brush = new SolidColorBrush(Color.FromArgb(alpha, tint.R, tint.G, tint.B))
            {
                Opacity = sourceBrush?.Opacity ?? 1.0
            };
            brush.Freeze();
            return brush;
        }

        private static Color ResolveTintColor(object value)
        {
            if (value is Color color)
                return color;

            if (value is SolidColorBrush brush)
                return brush.Color;

            if (value is string text)
            {
                try
                {
                    var parsed = new ColorConverter().ConvertFromInvariantString(text);
                    if (parsed is Color parsedColor)
                        return parsedColor;
                }
                catch
                {
                    // Ignore invalid color text and fall back.
                }
            }

            return Colors.White;
        }
    }
}
