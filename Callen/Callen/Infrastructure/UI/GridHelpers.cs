using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;

namespace Callen
{
    public class GridHelpers : DependencyObject
    {
        public static readonly DependencyProperty RowCountProperty =
            DependencyProperty.RegisterAttached(
                "RowCount",
                typeof(int),
                typeof(GridHelpers),
                new PropertyMetadata(-1, RowCountChanged));

        public static readonly DependencyProperty ColumnCountProperty =
            DependencyProperty.RegisterAttached(
                "ColumnCount",
                typeof(int),
                typeof(GridHelpers),
                new PropertyMetadata(-1, ColumnCountChanged));

        public static readonly DependencyProperty StarRowsProperty =
            DependencyProperty.RegisterAttached(
                "StarRows",
                typeof(string),
                typeof(GridHelpers),
                new PropertyMetadata(string.Empty, StarRowsChanged));

        public static readonly DependencyProperty StarColumnsProperty =
            DependencyProperty.RegisterAttached(
                "StarColumns",
                typeof(string),
                typeof(GridHelpers),
                new PropertyMetadata(string.Empty, StarColumnsChanged));

        public static int GetRowCount(DependencyObject obj)
        {
            return (int)obj.GetValue(RowCountProperty);
        }

        public static void SetRowCount(DependencyObject obj, int value)
        {
            obj.SetValue(RowCountProperty, value);
        }

        public static int GetColumnCount(DependencyObject obj)
        {
            return (int)obj.GetValue(ColumnCountProperty);
        }

        public static void SetColumnCount(DependencyObject obj, int value)
        {
            obj.SetValue(ColumnCountProperty, value);
        }

        public static string GetStarRows(DependencyObject obj)
        {
            return (string)obj.GetValue(StarRowsProperty);
        }

        public static void SetStarRows(DependencyObject obj, string value)
        {
            obj.SetValue(StarRowsProperty, value);
        }

        public static string GetStarColumns(DependencyObject obj)
        {
            return (string)obj.GetValue(StarColumnsProperty);
        }

        public static void SetStarColumns(DependencyObject obj, string value)
        {
            obj.SetValue(StarColumnsProperty, value);
        }

        private static void RowCountChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            var grid = obj as Grid;
            if (grid == null || (int)e.NewValue < 0)
                return;

            grid.RowDefinitions.Clear();
            for (var i = 0; i < (int)e.NewValue; i++)
                grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

            ApplyStarRows(grid);
        }

        private static void ColumnCountChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            var grid = obj as Grid;
            if (grid == null || (int)e.NewValue < 0)
                return;

            grid.ColumnDefinitions.Clear();
            for (var i = 0; i < (int)e.NewValue; i++)
                grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });

            ApplyStarColumns(grid);
        }

        private static void StarRowsChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            var grid = obj as Grid;
            if (grid == null)
                return;

            ApplyStarRows(grid);
        }

        private static void StarColumnsChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            var grid = obj as Grid;
            if (grid == null)
                return;

            ApplyStarColumns(grid);
        }

        private static void ApplyStarRows(Grid grid)
        {
            var starIndexes = ParseIndexes(GetStarRows(grid));
            for (var i = 0; i < grid.RowDefinitions.Count; i++)
            {
                if (starIndexes.Contains(i))
                    grid.RowDefinitions[i].Height = new GridLength(1, GridUnitType.Star);
            }
        }

        private static void ApplyStarColumns(Grid grid)
        {
            var starIndexes = ParseIndexes(GetStarColumns(grid));
            for (var i = 0; i < grid.ColumnDefinitions.Count; i++)
            {
                if (starIndexes.Contains(i))
                    grid.ColumnDefinitions[i].Width = new GridLength(1, GridUnitType.Star);
            }
        }

        private static HashSet<int> ParseIndexes(string csvIndexes)
        {
            var indexes = new HashSet<int>();
            if (string.IsNullOrWhiteSpace(csvIndexes))
                return indexes;

            foreach (var part in csvIndexes.Split(','))
            {
                int index;
                if (int.TryParse(part.Trim(), NumberStyles.Integer, CultureInfo.InvariantCulture, out index))
                    indexes.Add(index);
            }

            return indexes;
        }
    }
}
