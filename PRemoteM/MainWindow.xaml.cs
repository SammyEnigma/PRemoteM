﻿using System;
using System.Drawing;
using System.Threading;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using PRM.Core.Model;
using PRM.View;
using PRM.ViewModel;
using Shawn.Utils;
using KeyEventArgs = System.Windows.Input.KeyEventArgs;
using MouseEventArgs = System.Windows.Input.MouseEventArgs;
using TextBox = System.Windows.Controls.TextBox;

namespace PRM
{
    public partial class MainWindow : WindowChromeBase
    {
        public VmMain Vm { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            Vm = new VmMain(this);
            this.DataContext = Vm;
            Title = SystemConfig.AppName;
            this.Width = SystemConfig.Instance.Locality.MainWindowWidth;
            this.Height = SystemConfig.Instance.Locality.MainWindowHeight;
            this.SizeChanged += (sender, args) =>
            {
                if (this.WindowState == WindowState.Normal)
                {
                    SystemConfig.Instance.Locality.MainWindowHeight = this.Height;
                    SystemConfig.Instance.Locality.MainWindowWidth = this.Width;
                    SystemConfig.Instance.Locality.Save();
                    Console.WriteLine($"main window w = {this.Width}, h = {this.Height}");
                }
            };

            WinTitleBar.PreviewMouseDown += WinTitleBar_MouseDown;
            WinTitleBar.MouseUp += WinTitleBar_OnMouseUp;
            WinTitleBar.PreviewMouseMove += WinTitleBar_OnPreviewMouseMove;




            // Startup Location
            WindowStartupLocation = WindowStartupLocation.CenterScreen;


            BtnClose.Click += (sender, args) =>
            {
#if DEV
                HideMe();
                App.Close();
                return;
#else
                if (Shawn.Utils.ConsoleManager.HasConsole)
                    Shawn.Utils.ConsoleManager.Hide();
#endif
                HideMe();
            };
            this.Closing += (sender, args) =>
            {
                if (this.ShowInTaskbar)
                {
                    HideMe();
                    args.Cancel = true;
                }
            };
            BtnMaximize.Click += (sender, args) => this.WindowState = (this.WindowState == WindowState.Normal) ? WindowState.Maximized : WindowState.Normal;
            BtnMinimize.Click += (sender, args) => { this.WindowState = WindowState.Minimized; };
        }

        public void ActivateMe(bool isForceActivate = false)
        {
            if (isForceActivate)
                HideMe(false);
            Dispatcher?.Invoke(() =>
            {
                this.Visibility = Visibility.Visible;
                Topmost = true;
                this.ShowInTaskbar = true;
                this.Activate();
                Topmost = false;
            });
        }

        public void HideMe(bool isShowTip = true)
        {
            Dispatcher?.Invoke(() =>
            {
                this.ShowInTaskbar = false;
                this.Visibility = Visibility.Hidden;
                if (isShowTip && !string.IsNullOrEmpty(App.TaskTrayIcon?.BalloonTipText))
                    App.TaskTrayIcon?.ShowBalloonTip(1000);
            });
        }

        public void CloseMe()
        {
            HideMe();
            Close();
        }

        private void CommandFocusFilter_OnExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            TbFilter.Focus();
        }

        private void TbFilter_OnKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape
                && sender is TextBox textBox)
            {
                textBox.Text = "";
                // Kill logical focus
                FocusManager.SetFocusedElement(FocusManager.GetFocusScope(textBox), null);
                // Kill keyboard focus
                Keyboard.ClearFocus();
                this.Focus();
            }
        }

        private void ButtonExit_OnClick(object sender, RoutedEventArgs e)
        {
            CloseMe();
        }

        private void MainWindow_OnKeyDown(object sender, KeyEventArgs e)
        {
            //if(Vm.BottomPage == null
            //   && Vm.DispPage == null 
            //   && TbFilter.IsFocused == false)
            //    TbFilter.Focus();

            if (Keyboard.FocusedElement is TextBox)
            {
            }
            else if (e.Key == Key.Escape)
            {
                if (Vm.TopPage != null)
                {
                    Vm.TopPage = null;
                }
                else if (Vm.DispPage?.Page is SystemConfigPage scp)
                {
                    Vm.DispPage = null;
                }
            }
            else
            {
                TbFilter.Focus();
            }
        }

        private void ButtonToggleServerListViewUi_OnClick(object sender, RoutedEventArgs e)
        {
            SystemConfig.Instance.Theme.CmdToggleServerListPageUI?.Execute();
            PopupMenu.IsOpen = false;
        }
    }
}
