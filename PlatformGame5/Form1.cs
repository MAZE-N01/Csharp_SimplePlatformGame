using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PlatformGame5
{
    public partial class Form1 : Form
    {
        // Player movement flags
        bool goLeft, goRight, jumping, haskey, isOnGround;
        // Player jump settings
        int jumpSpeed = 12;
        int force = 9;
        // Game score
        int score = 0;
        // Player and background movement speeds
        int playerSpeed = 5;
        int backgroundSpeed = 8;
        public Form1()
        {
            InitializeComponent();
        }
        private void MainTimerEvent(object sender, EventArgs e)
        {
            // Update the score display
            txtScore.Text = "Coins: " + score;
            // Apply vertical movement to the player
            player.Top += jumpSpeed;
            // Move the player left if the left arrow key is pressed and player is not at the left boundary
            if (goLeft == true && player.Left > 80)
            {
                player.Left -= playerSpeed;
            }
            // Move the player right if the right arrow key is pressed and player is not at the right boundary
            if (goRight == true && player.Left + (player.Width + 70) < this.ClientSize.Width)
            {
                player.Left += playerSpeed;
            }
            // Move the background and game elements based on player movement
            if (goLeft == true && background.Left < 0)
            {
                background.Left += backgroundSpeed;
                MoveGameElements("forward");
            }
            if (goRight == true && background.Left > -1374)
            {
                background.Left -= backgroundSpeed;
                MoveGameElements("back");
            }
            // Adjust jump settings based on jumping state
            if (jumping == true)
            {
                jumpSpeed = -12;
                force -= 1;
            }
            else
            {
                jumpSpeed = 12;
            }
            // Check if the player is on the ground before allowing another jump
            if (isOnGround)
            {
                if (jumping == true && force <= 0)
                {
                    jumping = false;
                    isOnGround = false;
                }
            }
            // Check collisions with platforms and handle player landing
            foreach (Control x in this.Controls)
            {
                if (x is PictureBox && (string)x.Tag == "platform")
                {
                    if (player.Bounds.IntersectsWith(x.Bounds) && jumping == false)
                    {
                        force = 8;
                        player.Top = x.Top - player.Height;
                        jumpSpeed = 0;
                        isOnGround = true;  // Set the flag when the player is on the ground
                    }
                }
                // Check collisions with coins and update score
                if (x is PictureBox && (string)x.Tag == "coin")
                {
                    if (player.Bounds.IntersectsWith(x.Bounds) && x.Visible == true)
                    {
                        x.Visible = false;
                        score += 1;
                    }
                }
            }
            // Check if the player collected the key
            if (player.Bounds.IntersectsWith(key.Bounds))
            {
                key.Visible = false;
                haskey = true;
            }
            // Check if the player reached the door with the key and end the game
            if (player.Bounds.IntersectsWith(door.Bounds) && haskey == true)
            {
                door.Image = Properties.Resources.door_open;
                GameTimer.Stop();
                MessageBox.Show("Well done!" + Environment.NewLine + "Click OK to play again");
                RestartGame();
            }
            // Check if the player fell off the screen and end the game
            if (player.Top + player.Height > this.ClientSize.Height)
            {
                GameTimer.Stop();
                MessageBox.Show("RIP You Died!" + Environment.NewLine + "Click OK to play again");
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
            if (!jumping)
            {
                if (e.KeyCode == Keys.Space && isOnGround)
                {
                    jumping = true;
                }
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
            if (jumping)
            {
                if (e.KeyCode == Keys.Space && isOnGround)
                {
                    jumping = false;
                }
            }
        }
        private void CloseGame(object sender, FormClosedEventArgs e)
        {
            Application.Exit();
        }
        // Restarts the game by creating a new instance of Form1, showing it, and hiding the current form
        private void RestartGame()
        {
            // Create a new instance of the game window
            Form1 newWindow = new Form1();
            // Show the new game window
            newWindow.Show();
            // Hide the current game window
            this.Hide();
        }
        // Move game elements based on direction (forward/backward)
        private void MoveGameElements(string direction)
        {
            foreach (Control x in this.Controls)
            {
                if (x is PictureBox && (string)x.Tag == "platform" || x is PictureBox && (string)x.Tag == "coin" || x is PictureBox && (string)x.Tag == "key" || x is PictureBox && (string)x.Tag == "door")
                {
                    if (direction == "back")
                    {
                        x.Left -= backgroundSpeed;
                    }
                    if (direction == "forward")
                    {
                        x.Left += backgroundSpeed;
                    }
                }
            }
        }
    }
}
