﻿using ArcCreate.Jklss.BetonQusetEditor.ViewModel.Market;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ArcCreate.Jklss.BetonQusetEditor.Windows.Market
{
    /// <summary>
    /// MarketWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MarketWindow : Window
    {
        public MarketWindow()
        {
            this.DataContext = new MarketWindowViewModel(this);
            InitializeComponent();
        }

        private void Grid_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                this.DragMove();
            }
        }
    }
}
