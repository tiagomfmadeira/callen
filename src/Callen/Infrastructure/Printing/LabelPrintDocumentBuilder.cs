using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Markup;
using System.Windows.Media;
using Callen;

namespace Callen.Infrastructure.Printing
{
    internal sealed class LabelPrintDocumentBuilder
    {
        public const int ItemsPerPage = 36;
        private const int LabelsPerRow = 2;
        private const int LabelRowsPerPage = 18;
        private const double DefaultPageWidthCm = 21.0;
        private const double DefaultPageHeightCm = 29.7;
        private const double TopRowFontSize = 10.0;
        private const double TopRowLineHeight = 12.0;
        private const double NameRowFontSize = 11.0;
        private const double NameLineHeight = 12.0;
        private const int NameMaxLines = 2;
        private static readonly Thickness LabelPadding = new Thickness(4, 2, 4, 2);
        private static readonly DoubleCollection GridLineDash = new DoubleCollection { 2.0, 2.0 };

        public FixedDocument Build(IList<Calendar> calendars, Size requestedPageSize)
        {
            var pageSize = NormalizePageSize(requestedPageSize);
            var document = new FixedDocument();
            document.DocumentPaginator.PageSize = pageSize;

            if (calendars == null || calendars.Count == 0)
                return document;

            var pages = (int)Math.Ceiling(calendars.Count / (double)ItemsPerPage);
            var index = 0;

            for (var pageNumber = 0; pageNumber < pages; pageNumber++)
            {
                var page = CreatePage(calendars, index, pageSize);
                index += ItemsPerPage;

                var pageContent = new PageContent();
                ((IAddChild)pageContent).AddChild(page);
                document.Pages.Add(pageContent);
            }

            return document;
        }

        private static FixedPage CreatePage(IList<Calendar> calendars, int startIndex, Size pageSize)
        {
            var fixedPage = new FixedPage
            {
                Width = pageSize.Width,
                Height = pageSize.Height,
                Background = Brushes.White
            };

            var printableWidth = Math.Max(1.0, pageSize.Width);
            var printableHeight = Math.Max(1.0, pageSize.Height);
            var labelWidth = printableWidth / LabelsPerRow;
            var labelHeight = printableHeight / LabelRowsPerPage;
            var count = Math.Min(ItemsPerPage, calendars.Count - startIndex);
            var usedRows = Math.Max(1, (int)Math.Ceiling(count / (double)LabelsPerRow));
            fixedPage.Children.Add(CreateGridOverlay(printableWidth, labelWidth, labelHeight, usedRows));

            for (var slot = 0; slot < count; slot++)
            {
                var row = slot / LabelsPerRow;
                var col = slot % LabelsPerRow;
                var x = col * labelWidth;
                var y = row * labelHeight;
                var rect = new Rect(x, y, labelWidth, labelHeight);
                var label = CreateLabel(calendars[startIndex + slot], rect);
                fixedPage.Children.Add(label);
            }

            return fixedPage;
        }

        private static FrameworkElement CreateLabel(Calendar calendar, Rect area)
        {
            var item = calendar ?? new Calendar();
            var container = new Grid
            {
                Width = area.Width,
                Height = area.Height,
                ClipToBounds = true
            };

            var content = new Grid
            {
                Margin = LabelPadding,
                ClipToBounds = true
            };
            content.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            content.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1.0, GridUnitType.Star) });
            content.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

            for (var i = 0; i < 4; i++)
                content.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1.0, GridUnitType.Star) });

            AddTopField(content, "Nº " + item.id, 0, HorizontalAlignment.Left, TextAlignment.Left);
            AddTopField(content, item.matrix, 1, HorizontalAlignment.Left, TextAlignment.Left);
            AddTopField(content, item.collection, 2, HorizontalAlignment.Right, TextAlignment.Right);
            AddTopField(content, item.year, 3, HorizontalAlignment.Right, TextAlignment.Right);

            var name = new TextBlock
            {
                Text = item.name ?? string.Empty,
                FontSize = NameRowFontSize,
                VerticalAlignment = VerticalAlignment.Center,
                TextWrapping = TextWrapping.Wrap,
                LineStackingStrategy = LineStackingStrategy.BlockLineHeight,
                LineHeight = NameLineHeight,
                Height = NameLineHeight * NameMaxLines,
                TextTrimming = TextTrimming.CharacterEllipsis,
                TextAlignment = TextAlignment.Left,
                ClipToBounds = true
            };
            Grid.SetRow(name, 2);
            Grid.SetColumn(name, 0);
            Grid.SetColumnSpan(name, 4);
            content.Children.Add(name);

            container.Children.Add(content);
            FixedPage.SetLeft(container, area.Left);
            FixedPage.SetTop(container, area.Top);
            return container;
        }

        private static void AddTopField(
            Grid parent,
            string value,
            int column,
            HorizontalAlignment horizontalAlignment,
            TextAlignment textAlignment)
        {
            var text = new TextBlock
            {
                Text = value ?? string.Empty,
                FontSize = TopRowFontSize,
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = horizontalAlignment,
                TextWrapping = TextWrapping.NoWrap,
                TextTrimming = TextTrimming.CharacterEllipsis,
                TextAlignment = textAlignment,
                LineStackingStrategy = LineStackingStrategy.BlockLineHeight,
                LineHeight = TopRowLineHeight,
                ClipToBounds = true
            };
            Grid.SetRow(text, 0);
            Grid.SetColumn(text, column);
            parent.Children.Add(text);
        }

        private static FrameworkElement CreateGridOverlay(double width, double labelWidth, double labelHeight, int usedRows)
        {
            var usedHeight = usedRows * labelHeight;
            var canvas = new Canvas
            {
                Width = width,
                Height = usedHeight,
                IsHitTestVisible = false
            };

            for (var col = 0; col <= LabelsPerRow; col++)
            {
                var x = col * labelWidth;
                var verticalLine = new System.Windows.Shapes.Line
                {
                    X1 = x,
                    Y1 = 0,
                    X2 = x,
                    Y2 = usedHeight,
                    Stroke = Brushes.Black,
                    StrokeThickness = 1.0,
                    StrokeDashArray = GridLineDash
                };
                canvas.Children.Add(verticalLine);
            }

            for (var row = 0; row <= usedRows; row++)
            {
                var y = row * labelHeight;
                var horizontalLine = new System.Windows.Shapes.Line
                {
                    X1 = 0,
                    Y1 = y,
                    X2 = width,
                    Y2 = y,
                    Stroke = Brushes.Black,
                    StrokeThickness = 1.0,
                    StrokeDashArray = GridLineDash
                };
                canvas.Children.Add(horizontalLine);
            }

            return canvas;
        }

        private static Size NormalizePageSize(Size requested)
        {
            var width = requested.Width > 0 ? requested.Width : CmToDip(DefaultPageWidthCm);
            var height = requested.Height > 0 ? requested.Height : CmToDip(DefaultPageHeightCm);
            return new Size(width, height);
        }

        private static double CmToDip(double centimeters)
        {
            return centimeters * 96.0 / 2.54;
        }
    }
}
