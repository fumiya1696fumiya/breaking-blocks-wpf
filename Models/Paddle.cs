using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using System.Windows;
using System.Windows.Controls;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Breaking_Blocks.Models
{
    public partial class Paddle : ObservableObject 
    {
        [ObservableProperty]
        private double _x;

        [ObservableProperty]
        private double _y;

        [ObservableProperty]
        private double _width;

        [ObservableProperty]
        private double _height;

        //当たり判定
        public void WallCollision(double screenWidth) {
            if (X < 0) {
                X = 0;
            }
            if(X + Width > screenWidth) {
                //X + Width = screenWidth (XからWidthまでがXからscreenWidthまでに収まるようにする)
                X = screenWidth - Width;
            }
        }
    }
}
