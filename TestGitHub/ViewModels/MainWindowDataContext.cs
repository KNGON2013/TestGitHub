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
            this.model = this.Model;
        }

        public string Title => this.model.Title;

        public IEnumerable<Item> Items => this.model.Items;

        public bool IsBoolToValueConverter { get => this.model.IsBoolToValueConverter; set => this.model.IsBoolToValueConverter = value; }

        public ICommand CommandTestBoolToValueConverter => this.model.CommandTestBoolToValueConverter;

        public ICommand CommandDeviceChanged => this.model.CommandDeviceChanged;

        public ICommand CommandSizeChanged => this.model.CommandSizeChanged;

        public ICommand CommandLocationChanged => this.model.CommandLocationChanged;

        public ICommand CommandClosed => this.model.CommandClosed;

        public string ViewSizeChanged => this.model.ViewSizeChanged;

        public string ViewLocationChanged => this.model.ViewLocationChanged;

        public int ValidationInteger { get => this.model.ValidationInteger; set => this.model.ValidationInteger = value; }

        public double ValidationDouble { get => this.model.ValidationDouble; set => this.model.ValidationDouble = value; }

        public BitmapSource ImageBitmapSource => this.model.ImageBitmapSource;

        private ICommand _CommandTestDialog;
        public ICommand CommandTestDialog => this._CommandTestDialog ??= new RelayCommand(() =>
        {
            Debug.WriteLine(MethodBase.GetCurrentMethod().Name);
            var result = DialogService.Show("Title", "message");

            Debug.WriteLine(result.Value);
        });

        private ICommand _CommandTestFolder;
        public ICommand CommandTestFolder => this._CommandTestFolder ??= new RelayCommand(() =>
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

        public bool EnableCommand => this.model.EnableCommand;

        public ICommand CommandAdd => this.model.CommandAdd;

        public ICommand CommandRun => this.model.CommandRun;

        public ICommand CommandCancel => this.model.CommandCancel;

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
            this.IsAllowed(sender as TextBox, e, new Regex("[^0-9]+"));
        }

        /// <summary>
        /// テキスト入力許可:0～9、ピリオド(.)、マイナス符号(-).
        /// </summary>
        /// <param name="sender">送信元コントロール(TextBox).</param>
        /// <param name="e">EventArgs.</param>
        public void IsAllowedNumericalInput(object sender, TextCompositionEventArgs e)
        {
            this.IsAllowed(sender as TextBox, e, new Regex("[^0-9.-]+"));
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
