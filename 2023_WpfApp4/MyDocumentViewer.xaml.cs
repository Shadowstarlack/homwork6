using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;

namespace _2023_WpfApp4
{
    /// <summary>
    /// MyDocumentViewer.xaml 的互動邏輯
    /// </summary>
    public partial class MyDocumentViewer : Window
    {
        Color fontColor = Colors.Black;
        public MyDocumentViewer()
        {
            InitializeComponent();
            fontColorPicker.SelectedColor = fontColor;

            foreach (var fontFamily in Fonts.SystemFontFamilies)
            {
                fontFamilyComboBox.Items.Add(fontFamily);
            }
            fontFamilyComboBox.SelectedIndex = 8;

            fontSizeComboBox.ItemsSource = new List<Double> { 8, 9, 10, 11, 12, 20, 24, 32, 40, 50, 60, 70 };
            fontSizeComboBox.SelectedIndex = 4;
        }

        private void New_Executed(object sender, System.Windows.Input.ExecutedRoutedEventArgs e)
        {
            MyDocumentViewer myDocumentViewer = new MyDocumentViewer();
            myDocumentViewer.Show();
        }

        private void Open_Executed(object sender, System.Windows.Input.ExecutedRoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter = "Rich Text Format (*.rtf)|*.rtf|All files (*.*)|*.*"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                TextRange textRange = new TextRange(rtbEditor.Document.ContentStart, rtbEditor.Document.ContentEnd);

                using (FileStream fileStream = new FileStream(openFileDialog.FileName, FileMode.Open))
                {
                    textRange.Load(fileStream, DataFormats.Rtf);
                }
            }
        }

        private void Save_Executed(object sender, System.Windows.Input.ExecutedRoutedEventArgs e)
        {
            //將RichTextBox的內容存入檔案

            var saveFileDialog = new SaveFileDialog
            {
                Filter = "Rich Text Format (*.rtf)|*.rtf|All files (*.*)|*.*"
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                TextRange textRange = new TextRange(rtbEditor.Document.ContentStart, rtbEditor.Document.ContentEnd);

                using (FileStream fileStream = new FileStream(saveFileDialog.FileName, FileMode.Create))
                {
                    textRange.Save(fileStream, DataFormats.Rtf);
                }
            }
        }

        private void rtbEditor_SelectionChanged(object sender, RoutedEventArgs e)
        {
            //依據所選取文字的字體粗體，同步更新boldButton的狀態
            object property = rtbEditor.Selection.GetPropertyValue(TextElement.FontWeightProperty);
            boldButton.IsChecked = (property is FontWeight && (FontWeight)property == FontWeights.Bold);

            //依據所選取文字的字體是否為斜體，同步更新italicButton的狀態
            property = rtbEditor.Selection.GetPropertyValue(TextElement.FontStyleProperty);
            italicButton.IsChecked = (property is FontStyle && (FontStyle)property == FontStyles.Italic);

            //依據所選取文字的字體是否有加底線，同步更新underlineButton的狀態
            property = rtbEditor.Selection.GetPropertyValue(Inline.TextDecorationsProperty);
            underlineButton.IsChecked = (property != DependencyProperty.UnsetValue && property is TextDecorationCollection decorations && decorations.Contains(TextDecorations.Underline[0]));

            //依據所選取文字的字體大小，同步更新fontSizeComboBox的狀態
            property = rtbEditor.Selection.GetPropertyValue(TextElement.FontSizeProperty);
            fontSizeComboBox.SelectedItem = property;

            //依據所選取文字的字型，同步更新fontFamilyComboBox的狀態
            property = rtbEditor.Selection.GetPropertyValue(TextElement.FontFamilyProperty);
            fontFamilyComboBox.SelectedItem = property;

            //依據所選取文字的顏色，同步更新fontColorPicker的狀態
            SolidColorBrush? forgroundProperty = rtbEditor.Selection.GetPropertyValue(TextElement.ForegroundProperty) as SolidColorBrush;
            if (forgroundProperty != null)
            {
                fontColorPicker.SelectedColor = forgroundProperty.Color;
            }
        }

        private void fontFamilyComboBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (fontFamilyComboBox.SelectedItem != null)
            {
                rtbEditor.Selection.ApplyPropertyValue(TextElement.FontFamilyProperty, fontFamilyComboBox.SelectedItem);
            }
        }

        private void fontSizeComboBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (fontSizeComboBox.SelectedItem != null)
            {
                rtbEditor.Selection.ApplyPropertyValue(TextElement.FontSizeProperty, fontSizeComboBox.SelectedItem);
            }
        }

        private void fontColorPicker_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            fontColor = (Color)fontColorPicker.SelectedColor;
            SolidColorBrush fontBrush = new SolidColorBrush(fontColor);
            rtbEditor.Selection.ApplyPropertyValue(TextElement.ForegroundProperty, fontBrush);
        }

        private void clearButton_Click(object sender, RoutedEventArgs e)
        {
            rtbEditor.Document.Blocks.Clear();
        }
    }
}
