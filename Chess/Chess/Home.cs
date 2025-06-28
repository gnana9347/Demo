using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Chess
{
    public partial class FrmHome : Form
    {
        private bool isLoading = false;  // 👈 flag to prevent re-clicks

        public FrmHome()
        {
            InitializeComponent();
            btnPlay.Click += async (sender, e) => await btnPlay_ClickAsync();
        }

        private async Task btnPlay_ClickAsync()
        {
            if (isLoading) return; // 👈 stop if already loading
            isLoading = true;

            try
            {
                btnPlay.Cursor = Cursors.WaitCursor;

                // Create minimal loading form
                var loadingForm = new Form
                {
                    Width = 280,
                    Height = 120,
                    FormBorderStyle = FormBorderStyle.None,
                    StartPosition = FormStartPosition.CenterScreen,
                    BackColor = Color.White,
                    TopMost = true
                };

                // Border drawing
                loadingForm.Paint += (s, e) =>
                {
                    using (var pen = new Pen(Color.FromArgb(200, 200, 200)))
                    {
                        e.Graphics.DrawRectangle(pen,
                            0, 0,
                            loadingForm.ClientSize.Width - 1,
                            loadingForm.ClientSize.Height - 1);
                    }
                };

                // Animated loading dots label
                var dotsLabel = new Label
                {
                    Text = "● ○ ○",
                    Font = new Font("Segoe UI", 14),
                    AutoSize = true,
                    Location = new Point(110, 40),
                    ForeColor = Color.FromArgb(80, 80, 80)
                };

                // Loading message
                var textLabel = new Label
                {
                    Text = "Preparing your chess board",
                    Font = new Font("Segoe UI", 9),
                    AutoSize = true,
                    Location = new Point(60, 70),
                    ForeColor = Color.FromArgb(120, 120, 120)
                };

                loadingForm.Controls.Add(dotsLabel);
                loadingForm.Controls.Add(textLabel);
                loadingForm.Show();

                // Dot animation task
                var dotAnimation = Task.Run(async () =>
                {
                    string[] dotStates = { "● ○ ○", "○ ● ○", "○ ○ ●" };
                    int state = 0;

                    while (!loadingForm.IsDisposed)
                    {
                        dotsLabel.Invoke(new MethodInvoker(() =>
                        {
                            dotsLabel.Text = dotStates[state];
                        }));

                        state = (state + 1) % dotStates.Length;
                        await Task.Delay(300);
                    }
                });

                await Task.Delay(5000); // Simulated loading

                loadingForm.Close();
                await dotAnimation;

                this.Hide();
                using (var chessForm = new FrmChess())
                {
                    chessForm.ShowDialog();
                }
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
                this.Show();
            }
            finally
            {
                isLoading = false;
                btnPlay.Cursor = Cursors.Default;
            }
        }
    }
}
