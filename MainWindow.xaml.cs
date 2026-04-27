using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using Breaking_Blocks.ViewModels;

namespace Breaking_Blocks
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private bool isLeftPressed;
        private bool isRightPressed;

        public MainWindow()
        {
            InitializeComponent();
            ViewModel vm = new ViewModel();
            //ViewModelの処理をXamlにバインディング
            this.DataContext = vm;

            //wpfでゲームを作るならRenderingを使うとよい
            CompositionTarget.Rendering += GameLoop;
        }

        //キーボード処理
        private void Window_KeyDown(object sender, KeyEventArgs e) {
            if (DataContext is not ViewModel vm) return;
            switch (e.Key) {
                case Key.Left:
                    isLeftPressed = true; break;
                case Key.Right:
                    isRightPressed = true; break;
                case Key.Space:
                    if (vm.State == GameState.Ready || vm.State == GameState.GameOver || vm.State == GameState.GameClear)
                        vm.GameInit();
                    break;
            }
        }
        
        private void Window_KeyUp(object sender, KeyEventArgs e) {            
            switch (e.Key) {
                case Key.Left:
                    isLeftPressed = false; break;
                case Key.Right:
                    isRightPressed = false; break;
            }
        }

        private void GameLoop(object? sender, EventArgs e) {
            if (DataContext is not ViewModel vm) return;

            //スクリーンサイズを取得　GameCanvasはXamlのCanvasに付けたName
            double screenWidth = GameCanvas.ActualWidth;
            double screenHeight = GameCanvas.ActualHeight;

            if (vm.State == GameState.Playing) 
            { 
                if (isLeftPressed){
                    vm.MoveLeft(screenWidth);
                }
                else if (isRightPressed){
                    vm.MoveRight(screenWidth);
                }
            }

            vm.Update(screenWidth, screenHeight);
        }
    }
}