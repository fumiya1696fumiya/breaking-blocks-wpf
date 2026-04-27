using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Breaking_Blocks.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Windows;
using static System.Formats.Asn1.AsnWriter;

namespace Breaking_Blocks.ViewModels
{
    //状態定数 実体は整数が入っている
    public enum GameState
    { 
        Ready,
        Playing,
        GameOver,
        GameClear
    }

    public partial class ViewModel : ObservableObject
    {
        public GameState State;

        //ブロックのlist
        public ObservableCollection<Block> Blocks { get; set; } = new ObservableCollection<Block>();

        //パドルのプロパティ
        public Paddle Paddle { get; set; } = new Paddle
        {
            X = 200,
            Y = 500,
            Width = 100,
            Height = 20
        };

        //ボールのプロパティ
        public Ball Ball { get; set; } = new Ball
        {
            X = 600,
            Y = 200,
            Width = 20,
            Height = 20
        };

        //ゲーム終了時のテキスト出力
        [ObservableProperty]
        private string _gameSet = "Space Key Pressed to Start\n←・→　Key Pressed to Operate";
        [ObservableProperty]
        private string _totalScore = "SCORE:0";

        //スコアの初期化
        private int score = 0;

        //スコア最大
        private const int scoreMax = 300;

        //ブロックの行と列
        private const int rows = 5;  //defaultが５
        private const int cols = 6;  //defaultが６


        public ViewModel()
        {
            State = GameState.Ready;     
        }

        //パドルの移動
        public void MoveLeft(double screenWidth)
        {
            Paddle.X -= 10;

            Paddle.WallCollision(screenWidth);
        }
        public void MoveRight(double screenWidth)
        {
            Paddle.X += 10;

            Paddle.WallCollision(screenWidth);
        }

        //ボールと壁の当たり判定
        public void BallWallCollision(double screenWidth, double screenHeight) {
            if (Ball.X <= 0 || Ball.X + Ball.Width >= screenWidth)
            {
                Ball.VX *= -1;
            }
            else if (Ball.Y <= 0)
            {
                Ball.VY *= -1;
            }
            else if (Ball.Y + Ball.Height >= screenHeight)
            {
                State = GameState.GameOver;
                Ball.VX = 0;
                Ball.VY = 0;
            }
        }

        //ボールとパドルの当たり判定
        public void BallPaddleCollision() {
            if (Ball.X <= Paddle.X + Paddle.Width &&
                Ball.X + Ball.Width >= Paddle.X &&
                Ball.Y <= Paddle.Y + Paddle.Height &&
                Ball.Y + Ball.Height >= Paddle.Y )
            {
                //速度
                double speed = 5;

                double ballCenter = Ball.X + Ball.Width / 2;

                //パドル上のどこかを割合で出す＝　/ Paddle.Width
                //ballCenter - Paddle.X = パドルの左端から見てどれだけボールの中心が右にあるか
                //hitPos= 0～１
                double hitPos = (ballCenter - Paddle.X) / Paddle.Width;

                //パドルの左端から見て　direction=左端-1　真ん中0　右端1　＊以降はスピード調整
                double direction = (hitPos - 0.5) * 2;

                //真横すぎる角度を防ぐ
                direction = Math.Max(-0.8, Math.Min(0.8, direction));

                //方向ベクトル（ベクトルの長さが異なる）
                double dirx = direction;
                double diry = -1;

                //スカラ（速さ）異なる方向ベクトルで速さが一定になる
                double length = Math.Sqrt(dirx * dirx + diry * diry);

                //方向ベクトルをスカラで割って正規化した後（理論上は１になる）、speedを掛けて一定速度の速度ベクトルを算出する
                Ball.VX = (dirx / length) * speed;
                Ball.VY = (diry / length) * speed;
            }
        }

        //ボールとブロックの当たり判定とスコア返却
        public int BallBlockCollision()
        {
            for (int i = Blocks.Count - 1; i >= 0; i--)
            {
                Block b = Blocks[i];

                if (Ball.X + Ball.Width >= b.X &&
                   Ball.X <= b.X + b.Width &&
                   Ball.Y + Ball.Height >= b.Y &&
                   Ball.Y <= b.Y + b.Height)
                {
                    //指定した位置にある項目を削除
                    Blocks.RemoveAt(i);

                    Ball.VY *= -1;

                    score += 10;

                    break;
                }
            }
            if (Blocks.Count == 0) {
                State = GameState.GameClear;

                Ball.VX = 0;
                Ball.VY = 0;
            }

            return score;
        }

        //スコア表示
        public void DrawScore() {
            TotalScore = $"SCORE:{score}/{scoreMax}";
        }

        //ゲームオーバー、ゲームクリア処理
        public void CheckGameState()
        {
            if (State == GameState.GameClear)
            {
                GameSet = "GameClear\nSpace Key Pressed to Retart";
            }
            else if (State == GameState.GameOver)
            {
                GameSet = "GameOver\nSpace Key Pressed to Retart";
            }
        }

        //ゲームの初期化
        public void GameInit()
        {
            //Blocks.Clear();

            //ブロックの生成
            for (int row = 0; row < rows; row++)
            {
                for (int col = 0; col < cols; col++)
                {
                    Blocks.Add(new Block
                    {
                        //座標の開始位置と次のブロックとの間の空白の合計値
                        X = col * 117,
                        Y = row * 30,
                        Width = 100,
                        Height = 20
                    });
                }
            }

            Ball.X = 600;
            Ball.Y = 200;
            Ball.VX = 3;
            Ball.VY = 3;

            score = 0;

            State = GameState.Playing;

            GameSet = "";
        }

        //更新処理
        public void Update(double screenWidth, double screenHeight)
        {
            if (State != GameState.Playing) return;
            
            Ball.Move();

            BallPaddleCollision();

            BallBlockCollision();

            BallWallCollision(screenWidth, screenHeight);

            CheckGameState();

            DrawScore();
        }
    }
}
