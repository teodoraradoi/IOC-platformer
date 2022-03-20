using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SharpDX.XInput;
namespace IOC_platformer
{
    public partial class Form1 : Form
    {
        public class XInputController
        {
            public Controller controller;
            public Gamepad gamepad;
            public bool connected = false;
            public int deadband = 2500;
            public Point leftThumb, rightThumb = new Point(0, 0);
            public float leftTrigger, rightTrigger;
            
            public XInputController()
            {
                controller = new Controller(UserIndex.One);
                connected = controller.IsConnected;
            }

            // Call this method to update all class values
            public void Update()
            {
                if (!connected)
                    return;

                gamepad = controller.GetState().Gamepad;
                
                leftThumb.X = (int)((Math.Abs((float)gamepad.LeftThumbX) < deadband) ? 0 : (float)gamepad.LeftThumbX / short.MinValue * -100);
                leftThumb.Y = (int)((Math.Abs((float)gamepad.LeftThumbY) < deadband) ? 0 : (float)gamepad.LeftThumbY / short.MaxValue * 100);
                rightThumb.Y = (int)((Math.Abs((float)gamepad.RightThumbX) < deadband) ? 0 : (float)gamepad.RightThumbX / short.MaxValue * 100);
                rightThumb.X = (int)((Math.Abs((float)gamepad.RightThumbY) < deadband) ? 0 : (float)gamepad.RightThumbY / short.MaxValue * 100);

                leftTrigger = gamepad.LeftTrigger;
                rightTrigger = gamepad.RightTrigger;
            }
        }
        bool goLeft, goRight, jumping, isGameOver;

        int jumpSpeed;
        int force;
        int score = 0;
        int playerSpeed = 7;

        int horizontalSpeed = 5;
        int verticalSpeed = 3;

        int enemyOneSpeed = 5;
        int enemyTwoSpeed = 3;
        XInputController controller = new XInputController();
        

        public Form1()
        {
            InitializeComponent();
        }


        private void MainGameTimerEvent(object sender, EventArgs e)
        {
            txtScore.Text = "Score: " + score;         

            player.Top += jumpSpeed;
            controller.Update();
            if (goLeft == true || controller.gamepad.Buttons.HasFlag(GamepadButtonFlags.DPadLeft))
            {
                player.Left -= playerSpeed;
            }
            if (goRight == true || controller.gamepad.Buttons.HasFlag(GamepadButtonFlags.DPadRight))
            {
                player.Left += playerSpeed;
            }

            if (jumping == true && force < 0)
            {
                jumping = false;
            }

            if (jumping == true)
            {
                jumpSpeed = -8;
                force -= 1;
            }
            else
            {
                jumpSpeed = 10;
            }
            if (controller.gamepad.Buttons.HasFlag(GamepadButtonFlags.A) && jumping == false)
            {
                jumping = true;
            }
            if (!controller.gamepad.Buttons.HasFlag(GamepadButtonFlags.A) && jumping == true && controller.connected == true)
            {
                jumping = false;
            }
            foreach (Control x in this.Controls)
            {
                if (x is PictureBox)
                {
                    if ((string)x.Tag == "platform")
                    {
                        if (player.Bounds.IntersectsWith(x.Bounds))
                        {
                            force = 8;
                            player.Top = x.Top - player.Height;


                            if ((string)x.Name == "horizontalPlatform" && goLeft == false || (string)x.Name == "horizontalPlatform" && goRight == false)
                            {
                                player.Left -= horizontalSpeed;
                            }
                        }
                        x.BringToFront();
                    }

                    if ((string)x.Tag == "coin")
                    {
                        if (player.Bounds.IntersectsWith(x.Bounds) && x.Visible == true)
                        {
                            x.Visible = false;
                            score++;
                        }
                    }


                    if ((string)x.Tag == "enemy")
                    {
                        if (player.Bounds.IntersectsWith(x.Bounds))
                        {
                            gameTimer.Stop();
                            isGameOver = true;
                            txtScore.Text = "Score: " + score +Environment.NewLine + "You were killed!" + Environment.NewLine + "Press enter to try again";
                            timer1.Start();
                        }
                    }

                }
            }
            

            horizontalPlatform.Left -= horizontalSpeed;

            if (horizontalPlatform.Left < 0 || horizontalPlatform.Left + horizontalPlatform.Width > this.ClientSize.Width)
            {
                horizontalSpeed = -horizontalSpeed;
            }

            verticalPlatform.Top += verticalSpeed;

            if (verticalPlatform.Top < 195 || verticalPlatform.Top > 581)
            {
                verticalSpeed = -verticalSpeed;
            }


            enemyOne.Left -= enemyOneSpeed;

            if (enemyOne.Left < pictureBox5.Left || enemyOne.Left + enemyOne.Width > pictureBox5.Left + pictureBox5.Width)
            {
                enemyOneSpeed = -enemyOneSpeed;
            }

            enemyTwo.Left += enemyTwoSpeed;

            if (enemyTwo.Left < pictureBox2.Left || enemyTwo.Left + enemyTwo.Width > pictureBox2.Left + pictureBox2.Width)
            {
                enemyTwoSpeed = -enemyTwoSpeed;
            }


            if (player.Top + player.Height > this.ClientSize.Height + 50)
            {
                gameTimer.Stop();
                isGameOver = true;
                txtScore.Text = "Score: " + score + Environment.NewLine + "You fell to your death!" + Environment.NewLine + "Press enter to try again";
                timer1.Start();
            }

            if (player.Bounds.IntersectsWith(door.Bounds) && score == 26)
            {
                gameTimer.Stop();
                isGameOver = true;
                txtScore.Text = "Score: " + score + Environment.NewLine + "Your quest is complete!";
            }
            else if(!isGameOver)
            {
                txtScore.Text = "Score: " + score + Environment.NewLine + "Collect all the coins";
            }

        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            controller.Update();
            if (controller.gamepad.Buttons == GamepadButtonFlags.RightShoulder)
            { 
                RestartGame();
            }
        }

        private void KeyIsDown(object sender, KeyEventArgs e)
        {
             
            if (e.KeyCode == Keys.Left)
            {
                goLeft = true;
            }
            if (e.KeyCode == Keys.Right)
            {
                goRight = true;
            }
            if (e.KeyCode == Keys.Space && jumping == false)
            {
                jumping = true;
            }
        }

        private void KeyIsUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Left)
            {
                goLeft = false;
            }
            if (e.KeyCode == Keys.Right)
            {
                goRight = false;
            }
            if (jumping == true)
            {
                jumping = false;
            }


            if (e.KeyCode == Keys.Enter && isGameOver == true)
            {
                RestartGame();
            }
        }

        private void RestartGame()
        {

            jumping = false;
            goLeft = false;
            goRight = false;
            isGameOver = false;
            score = 0;

            txtScore.Text = "Score: " + score;

            // make all the coins visible again
            foreach (Control x in this.Controls)
            {
                if (x is PictureBox && x.Visible == false)
                {
                    x.Visible = true;
                }
            }


            // reset the position of player, moving platforms and enemies

            player.Left = 72;
            player.Top = 656;

            enemyOne.Left = 471;
            enemyTwo.Left = 360;

            horizontalPlatform.Left = 275;
            verticalPlatform.Top = 581;
            timer1.Stop();
            gameTimer.Start();
            

        }
    }
}
