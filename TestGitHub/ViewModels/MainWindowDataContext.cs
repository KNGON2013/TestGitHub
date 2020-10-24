using Microsoft.WindowsAPICodePack.Dialogs;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using TestGitHub.Interfaces;
using TestGitHub.Libraries.Templates;
using TestGitHub.Models;

namespace TestGitHub.ViewModels
{
    public class MainWindowDataContext : ViewModelBase<IMainWindow>, IMainWindow
    {
        private readonly IMainWindow model;

        public MainWindowDataContext() : base(new MainWindow())
        {
            model = Model;
        }

        public string Title => model.Title;

        public IEnumerable<Item> Items => model.Items;

        public bool IsBoolToValueConverter { get => model.IsBoolToValueConverter; set => model.IsBoolToValueConverter = value; }

        public ICommand CommandTestBoolToValueConverter => model.CommandTestBoolToValueConverter;

        public ICommand CommandDeviceChanged => model.CommandDeviceChanged;

        public ICommand CommandSizeChanged => model.CommandSizeChanged;

        public ICommand CommandLocationChanged => model.CommandLocationChanged;

        public ICommand CommandClosed => model.CommandClosed;

        public string ViewSizeChanged => model.ViewSizeChanged;

        public string ViewLocationChanged => model.ViewLocationChanged;

        public int ValidationInteger { get => model.ValidationInteger; set => model.ValidationInteger = value; }

        public double ValidationDouble { get => model.ValidationDouble; set => model.ValidationDouble = value; }

        public BitmapSource ImageBitmapSource => model.ImageBitmapSource;

        private ICommand _CommandTestDialog;
        public ICommand CommandTestDialog => _CommandTestDialog ??= new RelayCommand(() =>
        {
            Debug.WriteLine(MethodBase.GetCurrentMethod().Name);
            var result = DialogService.Show("Title", "message");

            Debug.WriteLine(result.Value);
        });

        private ICommand _CommandTestFolder;
        public ICommand CommandTestFolder => _CommandTestFolder ??= new RelayCommand(() =>
        {
            Debug.WriteLine(MethodBase.GetCurrentMethod().Name);

            var dialog = new CommonOpenFileDialog("フォルダーの選択")
            {
                IsFolderPicker = true,
            };

            if (dialog.ShowDialog() != CommonFileDialogResult.Ok)
            {
                return;
            }

            Debug.WriteLine(dialog.FileName);
        });

        public bool EnableCommand => model.EnableCommand;

        public ICommand CommandAdd => model.CommandAdd;

        public ICommand CommandRun => model.CommandRun;

        public ICommand CommandCancel => model.CommandCancel;

        /// <summary>
        /// マウスホイール.
        /// </summary>
        /// <param name="sender">送信元コントロール.</param>
        /// <param name="e">EventArgs.</param>
        public void MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (sender is TextBox textBox)
            {
                // PreviewTextInputには割り当てられないため、注意.
                if (int.TryParse(textBox.Text, out var result))
                {
                    result += e.Delta > 0 ? 1 : -1;
                    textBox.Text = $"{result}";
                }
            }
        }

        /// <summary>
        /// テキスト入力許可:0～9.
        /// </summary>
        /// <param name="sender">送信元コントロール(TextBox).</param>
        /// <param name="e">EventArgs.</param>
        public void IsAllowedUnsignedInput(object sender, TextCompositionEventArgs e)
        {
            IsAllowed(sender as TextBox, e, new Regex("[^0-9]+"));
        }

        /// <summary>
        /// テキスト入力許可:0～9、ピリオド(.)、マイナス符号(-).
        /// </summary>
        /// <param name="sender">送信元コントロール(TextBox).</param>
        /// <param name="e">EventArgs.</param>
        public void IsAllowedNumericalInput(object sender, TextCompositionEventArgs e)
        {
            IsAllowed(sender as TextBox, e, new Regex("[^0-9.-]+"));
        }

        /// <summary>
        /// テキスト入力許可.
        /// </summary>
        /// <param name="sender">TextBox.</param>
        /// <param name="e">EventArgs.</param>
        /// <param name="regex">入力可否判別.</param>
        private void IsAllowed(TextBox textBox, TextCompositionEventArgs e, Regex regex)
        {
            var text = textBox.Text + e.Text;
            e.Handled = regex.IsMatch(text);
        }
    }
}
