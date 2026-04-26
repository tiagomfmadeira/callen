using System;
using System.Windows;
using System.Windows.Input;
using Callen.Windows;

namespace Callen.Windows.Forms
{
    public partial class SplitArchiveWindow : Window
    {
        private readonly WindowOverlaySync overlaySync;

        public string SourceCode { get; }
        public string TargetCodeA { get; private set; }
        public string TargetRangeA { get; private set; }
        public string TargetCodeB { get; private set; }
        public string TargetRangeB { get; private set; }

        public SplitArchiveWindow(
            string sourceCode,
            string sourceRange,
            string suggestedCodeA,
            string suggestedRangeA,
            string suggestedCodeB,
            string suggestedRangeB)
        {
            InitializeComponent();
            overlaySync = new WindowOverlaySync(this);
            SourceCode = sourceCode ?? string.Empty;

            source_folder_box.Text = BuildSourceFolderDisplay(SourceCode, sourceRange);
            target_code_a_box.Text = suggestedCodeA ?? string.Empty;
            target_code_b_box.Text = suggestedCodeB ?? string.Empty;
            InitializeBounds(sourceRange, suggestedRangeA, suggestedRangeB);
            UpdateLiveRanges();

            PreviewKeyDown += HandleEsc;
            SourceInitialized += Win_SourceInitialized;
            Closed += Win_Closed;
        }

        private void Win_SourceInitialized(object sender, EventArgs e)
        {
            overlaySync.Attach();
        }

        private void Win_Closed(object sender, EventArgs e)
        {
            PreviewKeyDown -= HandleEsc;
            overlaySync.Detach();
        }

        private void HandleEsc(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
                Close();
        }

        private void btn_split_Click(object sender, RoutedEventArgs e)
        {
            var codeA = (target_code_a_box.Text ?? string.Empty).Trim();
            var codeB = (target_code_b_box.Text ?? string.Empty).Trim();

            if (codeA.Length == 0 || codeB.Length == 0)
            {
                ShowValidationDialog(Loc.T("ManageArchive.SplitMissingFields"));
                return;
            }

            string normalizedRangeA;
            string normalizedRangeB;
            string error;
            if (!FolderRangeHelper.TryNormalizeRange(bound_start_box.Text + "/" + bound_middle_box.Text, out normalizedRangeA, out error))
            {
                ShowValidationDialog(error);
                return;
            }

            if (!FolderRangeHelper.TryNormalizeRange(bound_middle_box.Text + "/" + bound_end_box.Text, out normalizedRangeB, out error))
            {
                ShowValidationDialog(error);
                return;
            }

            TargetCodeA = codeA;
            TargetRangeA = normalizedRangeA;
            TargetCodeB = codeB;
            TargetRangeB = normalizedRangeB;

            DialogResult = true;
        }

        private void bound_box_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            UpdateLiveRanges();
        }

        private void btn_close_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Close();
        }

        private void InitializeBounds(string sourceRange, string suggestedRangeA, string suggestedRangeB)
        {
            var normalizedSourceRange = FolderRangeHelper.NormalizeRangeOrEmpty(sourceRange);
            if (string.IsNullOrWhiteSpace(normalizedSourceRange))
                normalizedSourceRange = "A/Z";

            var parts = normalizedSourceRange.Split('/');
            var startText = parts.Length > 0 ? parts[0] : "A";
            var endText = parts.Length > 1 ? parts[1] : "Z";

            var normalizedA = FolderRangeHelper.NormalizeRangeOrEmpty(suggestedRangeA);
            var normalizedB = FolderRangeHelper.NormalizeRangeOrEmpty(suggestedRangeB);
            var middleText = "M";
            if (!string.IsNullOrWhiteSpace(normalizedA) && !string.IsNullOrWhiteSpace(normalizedB))
            {
                var aParts = normalizedA.Split('/');
                if (aParts.Length == 2)
                    middleText = aParts[1];
            }

            bound_start_box.Text = startText;
            bound_middle_box.Text = middleText;
            bound_end_box.Text = endText;
        }

        private void UpdateLiveRanges()
        {
            string normalizedRangeA;
            string normalizedRangeB;
            string error;

            if (FolderRangeHelper.TryNormalizeRange(bound_start_box.Text + "/" + bound_middle_box.Text, out normalizedRangeA, out error))
                target_range_a_tag.Text = "(" + normalizedRangeA + ")";
            else
                target_range_a_tag.Text = string.Empty;

            if (FolderRangeHelper.TryNormalizeRange(bound_middle_box.Text + "/" + bound_end_box.Text, out normalizedRangeB, out error))
                target_range_b_tag.Text = "(" + normalizedRangeB + ")";
            else
                target_range_b_tag.Text = string.Empty;
        }

        private void ShowValidationDialog(string message)
        {
            var dialog = new ActionDialogWindow(
                Loc.T("ManageArchive.SplitErrorTitle"),
                string.Empty,
                message ?? string.Empty,
                Loc.T("Dlg.Close"));

            DialogHelper.ShowOwnedDialog(dialog, this);
        }

        private static string BuildSourceFolderDisplay(string code, string range)
        {
            var baseCode = (code ?? string.Empty).Trim();
            if (baseCode.Length == 0)
                return string.Empty;

            var normalizedRange = FolderRangeHelper.NormalizeRangeOrEmpty(range);
            return normalizedRange.Length == 0 ? baseCode : baseCode + " (" + normalizedRange + ")";
        }
    }
}

