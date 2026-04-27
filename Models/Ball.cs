using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Breaking_Blocks.Models
{
    public partial class Ball : ObservableObject
    {
        [ObservableProperty]
        private double _x;

        [ObservableProperty]
        private double _y;

        [ObservableProperty]
        private double _width;

        [ObservableProperty]
        private double _height;

        public double VX { get; set; } = 3;
        public double VY { get; set; } = 3;

        //移動
        public void Move() { 
            X += VX; 
            Y += VY;
        }
    }
}
