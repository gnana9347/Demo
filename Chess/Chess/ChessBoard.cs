using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Chess
{
    public partial class FrmChess : Form
    {
        // Board and game state
        private PictureBox[,] squares = new PictureBox[8, 8];
        private PieceTag selectedPiece = null;
        private (int row, int col)? kingInCheckPosition = null;
        private (int row, int col)? lastMovedPosition = null; // Track last moved piece
        private bool gameEnded = false;
        private bool gameStarted = false;
        private int gameTimeMinutes = 5; // Default game time

        // Clock variables
        private Label lblTopClock;
        private Label lblBottomClock;
        private System.Timers.Timer clockTimer;
        private TimeSpan whiteTime;
        private TimeSpan blackTime;
        private bool isWhiteTurn = true;
        private bool clocksActive = false;

        // Game state tracking
        private (int fromRow, int fromCol, int toRow, int toCol)? lastMove = null;
        private string currentTurn = "White";
        private int whiteCapturesCount = 0;
        private int blackCapturesCount = 0;
        private Stack<(Move move, PieceTag captured)> moveHistory = new Stack<(Move, PieceTag)>();

        // Draw rule variables
        private Dictionary<string, int> positionCounts = new Dictionary<string, int>();
        private int halfMoveClock = 0;
        private bool drawOffered = false;

        public FrmChess()
        {
            InitializeComponent();
            InitializeClocks();

            btnReset.Enabled = false;
            btnStartGame.Enabled = false;

            // Time control buttons click handlers
            btn1Min.Click += (s, e) => SetClockTime(1);
            btn2Min.Click += (s, e) => SetClockTime(2);
            btn5Min.Click += (s, e) => SetClockTime(5);
            btn10Min.Click += (s, e) => SetClockTime(10);
            btn15Min.Click += (s, e) => SetClockTime(15);
            btn30Min.Click += (s, e) => SetClockTime(30);

            InitializeBoard();
            LoadPieces();
        }

        #region Initialization Methods

        private void InitializeClocks()
        {
            // White clock 
            lblTopClock = new Label();
            lblTopClock.Text = "00:00";
            lblTopClock.Font = new Font("Arial", 16, FontStyle.Bold);
            lblTopClock.ForeColor = Color.Black;
            lblTopClock.BackColor = Color.MintCream;
            lblTopClock.TextAlign = ContentAlignment.MiddleCenter;
            lblTopClock.Size = new Size(100, 40);
            lblTopClock.Location = new Point(pnlChessBoard.Right + 17, pnlChessBoard.Bottom - 49);
            this.Controls.Add(lblTopClock);
            lblTopClock.BringToFront();

            // Black clock 
            lblBottomClock = new Label();
            lblBottomClock.Text = "00:00";
            lblBottomClock.Font = new Font("Arial", 16, FontStyle.Bold);
            lblBottomClock.ForeColor = Color.White;
            lblBottomClock.BackColor = Color.Black;
            lblBottomClock.TextAlign = ContentAlignment.MiddleCenter;
            lblBottomClock.Size = new Size(100, 40);
            lblBottomClock.Location = new Point(pnlChessBoard.Right + 17, pnlChessBoard.Top + 10);
            this.Controls.Add(lblBottomClock);
            lblBottomClock.BringToFront();

            // Timer setup
            clockTimer = new System.Timers.Timer(1000);
            clockTimer.Elapsed += ClockTimer_Elapsed;
        }

        [System.Runtime.InteropServices.DllImport("Gdi32.dll", EntryPoint = "CreateRoundRectRgn")]
        private static extern IntPtr CreateRoundRectRgn(int nLeftRect, int nTopRect, int nRightRect, int nBottomRect, int nWidthEllipse, int nHeightEllipse);

        private void btnResign_Click(object sender, EventArgs e)
        {
            if (!gameStarted || gameEnded) return;

            string resigningPlayer = currentTurn;
            string winningPlayer = currentTurn == "White" ? "Black" : "White";

            // --- Chess.com style custom Form (confirmation dialog) ---
            Form resignDialog = new Form()
            {
                Size = new Size(400, 220),
                FormBorderStyle = FormBorderStyle.FixedDialog,
                StartPosition = FormStartPosition.CenterParent,
                BackColor = Color.White,
                Font = new Font("Segoe UI", 10),
                Text = "Confirm Resignation",
                MaximizeBox = false,
                MinimizeBox = false,
                ShowInTaskbar = false
            };

            Label lblTitle = new Label()
            {
                Text = $"♔ {resigningPlayer}, you are about to resign.",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = Color.Black,
                AutoSize = false,
                TextAlign = ContentAlignment.MiddleCenter,
                Dock = DockStyle.Top,
                Height = 50
            };

            Label lblInfo = new Label()
            {
                Text = $"♛ {winningPlayer} will win the game by resignation.\n\nDo you really want to resign?",
                AutoSize = false,
                TextAlign = ContentAlignment.MiddleCenter,
                Dock = DockStyle.Top,
                Height = 70
            };

            Button btnYes = new Button()
            {
                Text = "Resign",
                DialogResult = DialogResult.OK,
                BackColor = Color.FromArgb(220, 53, 69),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Width = 100,
                Height = 35,
                Margin = new Padding(10)
            };
            btnYes.FlatAppearance.BorderSize = 0;
            btnYes.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, btnYes.Width, btnYes.Height, 15, 15));

            Button btnNo = new Button()
            {
                Text = "Cancel",
                DialogResult = DialogResult.Cancel,
                BackColor = Color.LightGray,
                ForeColor = Color.Black,
                FlatStyle = FlatStyle.Flat,
                Width = 100,
                Height = 35,
                Margin = new Padding(10)
            };
            btnNo.FlatAppearance.BorderSize = 0;
            btnNo.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, btnNo.Width, btnNo.Height, 15, 15));

            FlowLayoutPanel panelButtons = new FlowLayoutPanel()
            {
                FlowDirection = FlowDirection.RightToLeft,
                Dock = DockStyle.Bottom,
                Height = 60,
                Padding = new Padding(10)
            };
            panelButtons.Controls.Add(btnNo);
            panelButtons.Controls.Add(btnYes);

            resignDialog.Controls.Add(panelButtons);
            resignDialog.Controls.Add(lblInfo);
            resignDialog.Controls.Add(lblTitle);

            DialogResult result = resignDialog.ShowDialog();

            if (result == DialogResult.OK)
            {
                gameEnded = true;
                clockTimer.Stop();
                clocksActive = false;

                btnResign.Enabled = false;
                btnOfferDraw.Enabled = false;
                pnlChessBoard.Enabled = false;

                // --- Custom Game Over Dialog ---
                Form resultDialog = new Form()
                {
                    Size = new Size(400, 220),
                    FormBorderStyle = FormBorderStyle.FixedDialog,
                    StartPosition = FormStartPosition.CenterParent,
                    BackColor = Color.White,
                    Font = new Font("Segoe UI", 10),
                    Text = "Game Over",
                    MaximizeBox = false,
                    MinimizeBox = false,
                    ShowInTaskbar = false
                };

                Label lblResult = new Label()
                {
                    Text = $"♔ {resigningPlayer} has resigned.\n\n♛ {winningPlayer} wins the game by resignation.",
                    Font = new Font("Segoe UI Semibold", 11, FontStyle.Bold),
                    AutoSize = false,
                    TextAlign = ContentAlignment.MiddleCenter,
                    Dock = DockStyle.Fill,
                    ForeColor = Color.Black
                };

                Button btnOK = new Button()
                {
                    Text = "OK",
                    DialogResult = DialogResult.OK,
                    BackColor = Color.FromArgb(0, 123, 255),
                    ForeColor = Color.White,
                    FlatStyle = FlatStyle.Flat,
                    Width = 100,
                    Height = 36,
                    Margin = new Padding(10)
                };
                btnOK.FlatAppearance.BorderSize = 0;
                btnOK.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, btnOK.Width, btnOK.Height, 15, 15));

                FlowLayoutPanel okPanel = new FlowLayoutPanel()
                {
                    FlowDirection = FlowDirection.RightToLeft,
                    Dock = DockStyle.Bottom,
                    Height = 60,
                    Padding = new Padding(10)
                };
                okPanel.Controls.Add(btnOK);

                resultDialog.Controls.Add(okPanel);
                resultDialog.Controls.Add(lblResult);
                resultDialog.ShowDialog();
            }
        }

        private void InitializeBoard()
        {
            foreach (Control ctrl in pnlChessBoard.Controls)
            {
                if (ctrl is PictureBox pb && pb.Name.StartsWith("pbSquare_"))
                {
                    string[] parts = pb.Name.Split('_');
                    int row = int.Parse(parts[1]);
                    int col = int.Parse(parts[2]);

                    squares[row, col] = pb;
                    pb.BackColor = (row + col) % 2 == 0 ? Color.White : Color.Gray;
                    pb.BorderStyle = BorderStyle.None;

                    // Initialize with no click handler (will be attached when game starts)
                    pb.Click -= Square_Click;
                }
            }
        }

        #endregion

        #region Game Control Methods
        private void btnReset_Click(object sender, EventArgs e)
        {
            // Clear last moved position
            lastMovedPosition = null;

            // Stop the clock
            clockTimer.Stop();
            clocksActive = false;

            // Reset game state variables
            gameStarted = false;
            gameEnded = false;
            selectedPiece = null;
            kingInCheckPosition = null;
            currentTurn = "White";
            isWhiteTurn = true;
            moveHistory.Clear();
            lastMove = null;
            whiteCapturesCount = 0;
            blackCapturesCount = 0;
            positionCounts.Clear();
            halfMoveClock = 0;
            drawOffered = false;

            // Clear captured pieces panels
            pnlWhiteCaptured.Controls.Clear();
            pnlBlackCaptured.Controls.Clear();

            // Reset the board
            ClearBoard();
            InitializeBoard();
            LoadPieces();

            // Reset clocks
            SetClockTime(gameTimeMinutes);

            // Enable/disable buttons correctly
            btnStartGame.Enabled = true;
            btnResign.Enabled = false;
            btnOfferDraw.Enabled = false;
            pnlChessBoard.Enabled = false;
            EnableTimeControls(true);
        }

        private void ClearBoard()
        {
            lastMovedPosition = null;

            for (int row = 0; row < 8; row++)
            {
                for (int col = 0; col < 8; col++)
                {
                    if (squares[row, col] != null)
                    {
                        squares[row, col].Image = null;
                        squares[row, col].Tag = null;
                        squares[row, col].BackColor = (row + col) % 2 == 0 ? Color.White : Color.Gray;
                        squares[row, col].BorderStyle = BorderStyle.None;
                    }
                }
            }
        }

        private void SetClockTime(int minutes)
        {
            gameTimeMinutes = minutes;

            if (clockTimer != null && clockTimer.Enabled)
                clockTimer.Stop();

            whiteTime = TimeSpan.FromMinutes(minutes);
            blackTime = TimeSpan.FromMinutes(minutes);

            lblTopClock.Text = string.Format("{0:mm\\:ss}", whiteTime);
            lblBottomClock.Text = string.Format("{0:mm\\:ss}", blackTime);

            btnStartGame.Enabled = true;
            btnReset.Enabled = true;
            EnableTimeControls(!gameStarted);
        }

        private void EnableTimeControls(bool enable)
        {
            btn1Min.Enabled = enable;
            btn2Min.Enabled = enable;
            btn5Min.Enabled = enable;
            btn10Min.Enabled = enable;
            btn15Min.Enabled = enable;
            btn30Min.Enabled = enable;
        }

        private void btnStartGame_Click(object sender, EventArgs e)
        {
            gameStarted = true;
            gameEnded = false;
            btnStartGame.Enabled = false;
            btnResign.Enabled = true;
            currentTurn = "White";
            isWhiteTurn = true;
            positionCounts.Clear();
            halfMoveClock = 0;
            drawOffered = false;

            EnableTimeControls(false);
            // Reattach click handlers to all squares
            AttachSquareClickHandlers();

            clocksActive = true;
            clockTimer.Start();
            pnlChessBoard.Enabled = true;
            btnOfferDraw.Enabled = true;
        }

        private void AttachSquareClickHandlers()
        {
            foreach (var square in squares)
            {
                if (square != null)
                {
                    // Remove existing handler first to avoid duplicates
                    square.Click -= Square_Click;
                    square.Click += Square_Click;
                }
            }
        }
        #endregion

        #region Piece Movement and Game Logic

        private void Square_Click(object sender, EventArgs e)
        {
            if (!gameStarted)
            {
                MessageBox.Show("Start the game first!");
                return;
            }

            PictureBox clicked = sender as PictureBox;
            if (clicked == null) return;

            int row = GetRow(clicked);
            int col = GetCol(clicked);

            if (selectedPiece == null)
            {
                if (clicked.Tag is PieceTag tag && tag.Color == currentTurn)
                {
                    selectedPiece = tag;
                    ShowLegalMoves(tag);
                }
            }
            else
            {
                // If clicking on another piece of the same color, select that piece instead
                if (clicked.Tag is PieceTag newTag && newTag.Color == currentTurn)
                {
                    selectedPiece = newTag;
                    ClearHighlights();
                    ShowLegalMoves(newTag);
                }
                else
                {
                    TryMovePiece(row, col);
                }
            }

            // Maintain check highlight when clicking on the king
            if (kingInCheckPosition.HasValue &&
                kingInCheckPosition.Value.row == row &&
                kingInCheckPosition.Value.col == col)
            {
                squares[row, col].BackColor = Color.Red;
                squares[row, col].BorderStyle = BorderStyle.Fixed3D;
                squares[row, col].Refresh();
            }
        }

        private void TryMovePiece(int toRow, int toCol)
        {
            if (selectedPiece == null) return;

            PictureBox fromBox = squares[selectedPiece.Row, selectedPiece.Col];
            PictureBox toBox = squares[toRow, toCol];

            int fromRow = selectedPiece.Row;
            int fromCol = selectedPiece.Col;
            string pieceName = selectedPiece.PieceName;
            string color = selectedPiece.Color;

            bool isLegalMove = false;
            PieceTag capturedPiece = null;
            bool isCastling = false;
            bool isEnPassant = false;
            bool isPromotion = false;

            PieceTag targetPiece = toBox.Tag as PieceTag;

            switch (pieceName)
            {
                case string p when p.EndsWith("Pawn"):
                    isLegalMove = ValidatePawnMove(fromRow, fromCol, toRow, toCol, color,
                                                ref targetPiece, out isEnPassant, out isPromotion);
                    if (isLegalMove && targetPiece != null) capturedPiece = targetPiece;
                    break;

                case string p when p.EndsWith("Knight"):
                    isLegalMove = ValidateKnightMove(fromRow, fromCol, toRow, toCol, color, ref targetPiece);
                    if (isLegalMove && targetPiece != null) capturedPiece = targetPiece;
                    break;

                case string p when p.EndsWith("Bishop"):
                    isLegalMove = ValidateBishopMove(fromRow, fromCol, toRow, toCol, color, ref targetPiece);
                    if (isLegalMove && targetPiece != null) capturedPiece = targetPiece;
                    break;

                case string p when p.EndsWith("Rook"):
                    isLegalMove = ValidateRookMove(fromRow, fromCol, toRow, toCol, color, ref targetPiece);
                    if (isLegalMove && targetPiece != null) capturedPiece = targetPiece;
                    break;

                case string p when p.EndsWith("Queen"):
                    isLegalMove = ValidateQueenMove(fromRow, fromCol, toRow, toCol, color, ref targetPiece);
                    if (isLegalMove && targetPiece != null) capturedPiece = targetPiece;
                    break;

                case string p when p.EndsWith("King"):
                    isLegalMove = ValidateKingMove(fromRow, fromCol, toRow, toCol, color, ref targetPiece, ref isCastling);
                    if (isLegalMove && targetPiece != null && !isCastling) capturedPiece = targetPiece;
                    break;
            }

            if (isLegalMove)
            {
                var originalFromTag = fromBox.Tag;
                var originalToTag = toBox.Tag;

                fromBox.Tag = null;
                toBox.Tag = originalFromTag;

                bool leavesKingInCheck = IsInCheck(color);

                fromBox.Tag = originalFromTag;
                toBox.Tag = originalToTag;

                if (leavesKingInCheck)
                {
                    isLegalMove = false;
                }
            }

            if (isLegalMove)
            {
                moveHistory.Push((new Move
                {
                    FromRow = fromRow,
                    FromCol = fromCol,
                    ToRow = toRow,
                    ToCol = toCol,
                    Piece = selectedPiece,
                    Captured = capturedPiece,
                    IsCastling = isCastling,
                    IsEnPassant = isEnPassant,
                    IsPromotion = isPromotion
                }, capturedPiece));

                ExecuteMove(fromRow, fromCol, toRow, toCol, selectedPiece, capturedPiece, isCastling, isEnPassant, isPromotion);
                CheckGameStatus();
            }

            selectedPiece = null;
            ClearHighlights();
        }

        private void ExecuteMove(int fromRow, int fromCol, int toRow, int toCol,
                              PieceTag piece, PieceTag capturedPiece,
                              bool isCastling, bool isEnPassant, bool isPromotion)
        {
            // Clear previous last move highlight
            if (lastMovedPosition.HasValue)
            {
                var (prevRow, prevCol) = lastMovedPosition.Value;
                squares[prevRow, prevCol].BackColor = (prevRow + prevCol) % 2 == 0 ? Color.White : Color.Gray;
            }

            // Set new last moved position
            lastMovedPosition = (toRow, toCol);
            squares[toRow, toCol].BackColor = Color.FromArgb(255, 255, 150); // Light yellow highlight

            bool isPawnMove = piece.PieceName.EndsWith("Pawn");
            UpdateHalfMoveClock(piece, capturedPiece != null, isPawnMove);

            PictureBox fromBox = squares[fromRow, fromCol];
            PictureBox toBox = squares[toRow, toCol];

            if (capturedPiece != null)
            {
                if (isEnPassant)
                {
                    int capturedRow = piece.Color == "White" ? toRow + 1 : toRow - 1;
                    squares[capturedRow, toCol].Image = null;
                    squares[capturedRow, toCol].Tag = null;
                }
                AddCapturedPiece(capturedPiece, piece.Color);
            }

            if (isCastling)
            {
                bool kingSide = (toCol > fromCol);
                int rookFromCol = kingSide ? 7 : 0;
                int rookToCol = kingSide ? 5 : 3;

                // Move the rook
                PictureBox rookFromBox = squares[fromRow, rookFromCol];
                PictureBox rookToBox = squares[fromRow, rookToCol];

                rookToBox.Image = rookFromBox.Image;
                rookToBox.Tag = new PieceTag
                {
                    Row = fromRow,
                    Col = rookToCol,
                    PieceName = ((PieceTag)rookFromBox.Tag).PieceName,
                    Color = piece.Color,
                    HasMoved = true
                };

                rookFromBox.Image = null;
                rookFromBox.Tag = null;
            }

            toBox.Image = fromBox.Image;
            toBox.Tag = new PieceTag
            {
                Row = toRow,
                Col = toCol,
                PieceName = piece.PieceName,
                Color = piece.Color,
                HasMoved = true
            };

            fromBox.Image = null;
            fromBox.Tag = null;

            if (isPromotion)
            {
                // Create styled promotion dialog
                Form promotionForm = new Form()
                {
                    Width = 400,
                    Height = 250,
                    Text = "Promote Your Pawn",
                    FormBorderStyle = FormBorderStyle.FixedDialog,
                    StartPosition = FormStartPosition.CenterScreen,
                    BackColor = Color.FromArgb(240, 240, 240),
                    MaximizeBox = false,
                    MinimizeBox = false
                };

                // Header label
                Label headerLabel = new Label()
                {
                    Text = "Choose promotion piece:",
                    Font = new Font("Segoe UI", 12, FontStyle.Bold),
                    ForeColor = Color.FromArgb(70, 70, 70),
                    AutoSize = true,
                    Top = 20,
                    Left = (promotionForm.Width - 180) / 2
                };

                // Piece selection panel
                FlowLayoutPanel piecePanel = new FlowLayoutPanel()
                {
                    FlowDirection = FlowDirection.LeftToRight,
                    AutoSize = true,
                    Top = 60,
                    Left = (promotionForm.Width - 320) / 2
                };

                // Create piece buttons with images
                string[] pieces = { "Queen", "Rook", "Bishop", "Knight" };
                List<Button> pieceButtons = new List<Button>();

                foreach (string pieceType in pieces)
                {
                    Button pieceButton = new Button()
                    {
                        Width = 70,
                        Height = 70,
                        Margin = new Padding(5),
                        FlatStyle = FlatStyle.Flat,
                        BackgroundImage = (Image)Properties.Resources.ResourceManager.GetObject(
                            $"{piece.Color.ToLower()}_{pieceType.ToLower()}"),
                        BackgroundImageLayout = ImageLayout.Zoom,
                        Tag = pieceType,
                        Cursor = Cursors.Hand
                    };

                    pieceButton.FlatAppearance.BorderSize = 0;
                    pieceButton.FlatAppearance.MouseOverBackColor = Color.FromArgb(220, 220, 220);
                    pieceButton.FlatAppearance.MouseDownBackColor = Color.FromArgb(200, 200, 200);

                    pieceButton.Click += (sender, e) =>
                    {
                        string selectedPiece = ((Button)sender).Tag.ToString();
                        string promotedPiece = piece.Color + selectedPiece;

                        toBox.Image = pieceButton.BackgroundImage;
                        toBox.Tag = new PieceTag
                        {
                            Row = toRow,
                            Col = toCol,
                            PieceName = promotedPiece,
                            Color = piece.Color,
                            HasMoved = true
                        };

                        promotionForm.Close();
                    };

                    pieceButtons.Add(pieceButton);
                    piecePanel.Controls.Add(pieceButton);
                }

                // Add controls to form
                promotionForm.Controls.Add(headerLabel);
                promotionForm.Controls.Add(piecePanel);

                // Show as dialog
                promotionForm.ShowDialog();
            }

            lastMove = (fromRow, fromCol, toRow, toCol);
            currentTurn = currentTurn == "White" ? "Black" : "White";
            isWhiteTurn = !isWhiteTurn;
            drawOffered = false;

            if (kingInCheckPosition.HasValue &&
                piece.PieceName.EndsWith("King") &&
                fromRow == kingInCheckPosition.Value.row &&
                fromCol == kingInCheckPosition.Value.col)
            {
                ClearCheckHighlight();
            }

            UpdatePositionHistory();
        }

        #endregion

        #region Move Validation Methods 

        private bool ValidatePawnMove(int fromRow, int fromCol, int toRow, int toCol,
                             string color, ref PieceTag capturedPiece,
                             out bool isEnPassant, out bool isPromotion)
        {
            isEnPassant = false;
            isPromotion = false;

            int direction = color == "White" ? -1 : 1;
            int startRow = color == "White" ? 6 : 1;

            int rowDiff = toRow - fromRow;
            int colDiff = Math.Abs(toCol - fromCol);

            isPromotion = (color == "White" && toRow == 0) || (color == "Black" && toRow == 7);

            if (colDiff == 0)
            {
                if (rowDiff == direction)
                {
                    return squares[toRow, toCol].Tag == null;
                }
                else if (rowDiff == 2 * direction && fromRow == startRow)
                {
                    return squares[fromRow + direction, fromCol].Tag == null &&
                           squares[toRow, toCol].Tag == null;
                }
            }
            else if (colDiff == 1 && rowDiff == direction)
            {
                if (squares[toRow, toCol].Tag is PieceTag target && target.Color != color)
                {
                    capturedPiece = target;
                    return true;
                }
                else if (lastMove.HasValue)
                {
                    var (lastFromRow, lastFromCol, lastToRow, lastToCol) = lastMove.Value;
                    if (Math.Abs(lastFromRow - lastToRow) == 2 &&
                        lastToRow == fromRow &&
                        lastToCol == toCol)
                    {
                        PieceTag lastMovedPiece = squares[lastToRow, lastToCol].Tag as PieceTag;
                        if (lastMovedPiece != null &&
                            lastMovedPiece.PieceName.EndsWith("Pawn") &&
                            lastMovedPiece.Color != color)
                        {
                            capturedPiece = lastMovedPiece;
                            isEnPassant = true;
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        private bool ValidateKnightMove(int fromRow, int fromCol, int toRow, int toCol, string color, ref PieceTag capturedPiece)
        {
            int dRow = Math.Abs(toRow - fromRow);
            int dCol = Math.Abs(toCol - fromCol);

            if ((dRow == 2 && dCol == 1) || (dRow == 1 && dCol == 2))
            {
                var toTag = squares[toRow, toCol].Tag as PieceTag;
                if (toTag == null || toTag.Color != color)
                {
                    capturedPiece = toTag;
                    return true;
                }
            }
            return false;
        }

        private bool ValidateBishopMove(int fromRow, int fromCol, int toRow, int toCol, string color, ref PieceTag capturedPiece)
        {
            int dRow = toRow - fromRow;
            int dCol = toCol - fromCol;

            if (Math.Abs(dRow) == Math.Abs(dCol) && IsPathClear(fromRow, fromCol, toRow, toCol))
            {
                var toTag = squares[toRow, toCol].Tag as PieceTag;
                if (toTag == null || toTag.Color != color)
                {
                    capturedPiece = toTag;
                    return true;
                }
            }
            return false;
        }

        private bool ValidateRookMove(int fromRow, int fromCol, int toRow, int toCol, string color, ref PieceTag capturedPiece)
        {
            int dRow = toRow - fromRow;
            int dCol = toCol - fromCol;

            if ((dRow == 0 || dCol == 0) && IsPathClear(fromRow, fromCol, toRow, toCol))
            {
                var toTag = squares[toRow, toCol].Tag as PieceTag;
                if (toTag == null || toTag.Color != color)
                {
                    capturedPiece = toTag;
                    return true;
                }
            }
            return false;
        }

        private bool ValidateQueenMove(int fromRow, int fromCol, int toRow, int toCol, string color, ref PieceTag capturedPiece)
        {
            int dRow = toRow - fromRow;
            int dCol = toCol - fromCol;

            if ((Math.Abs(dRow) == Math.Abs(dCol) || (dRow == 0 || dCol == 0)) &&
                IsPathClear(fromRow, fromCol, toRow, toCol))
            {
                var toTag = squares[toRow, toCol].Tag as PieceTag;
                if (toTag == null || toTag.Color != color)
                {
                    capturedPiece = toTag;
                    return true;
                }
            }
            return false;
        }

        private bool ValidateKingMove(int fromRow, int fromCol, int toRow, int toCol, string color,
                             ref PieceTag capturedPiece, ref bool isCastling)
        {
            int dRow = toRow - fromRow;
            int dCol = toCol - fromCol;

            // Normal king move (1 square in any direction)
            if (Math.Abs(dRow) <= 1 && Math.Abs(dCol) <= 1)
            {
                var toTag = squares[toRow, toCol].Tag as PieceTag;
                if (toTag == null || toTag.Color != color)
                {
                    // Temporarily move the king to check if the square is safe
                    var originalFromTag = squares[fromRow, fromCol].Tag;
                    var originalToTag = squares[toRow, toCol].Tag;

                    squares[fromRow, fromCol].Tag = null;
                    squares[toRow, toCol].Tag = originalFromTag;

                    bool isSafe = !IsSquareUnderAttack(toRow, toCol, color);

                    // Restore original positions
                    squares[fromRow, fromCol].Tag = originalFromTag;
                    squares[toRow, toCol].Tag = originalToTag;

                    if (isSafe)
                    {
                        capturedPiece = toTag;
                        return true;
                    }
                }
            }
            // Castling move
            else if (Math.Abs(dCol) == 2 && dRow == 0 && !selectedPiece.HasMoved)
            {
                bool kingSide = (toCol > fromCol);
                if (CanCastle(color, kingSide))
                {
                    isCastling = true;
                    return true;
                }
            }

            return false;
        }

        private bool CanCastle(string color, bool kingSide)
        {
            int row = (color == "White") ? 7 : 0;
            int rookCol = kingSide ? 7 : 0;
            int kingCol = 4;

            var kingTag = squares[row, kingCol].Tag as PieceTag;
            var rookTag = squares[row, rookCol].Tag as PieceTag;

            if (kingTag == null || rookTag == null) return false;
            if (kingTag.HasMoved || rookTag.HasMoved) return false;
            if (!kingTag.PieceName.EndsWith("King") || !rookTag.PieceName.EndsWith("Rook")) return false;

            if (kingSide)
            {
                if (squares[row, 5].Tag != null || squares[row, 6].Tag != null) return false;
            }
            else
            {
                if (squares[row, 1].Tag != null || squares[row, 2].Tag != null || squares[row, 3].Tag != null) return false;
            }

            int[] colsToCheck = kingSide ? new[] { 4, 5, 6 } : new[] { 2, 3, 4 };
            foreach (int col in colsToCheck)
            {
                if (IsSquareUnderAttack(row, col, color)) return false;
            }

            return true;
        }

        private bool IsPathClear(int fromRow, int fromCol, int toRow, int toCol)
        {
            int dRow = Math.Sign(toRow - fromRow);
            int dCol = Math.Sign(toCol - fromCol);

            int r = fromRow + dRow, c = fromCol + dCol;
            while (r != toRow || c != toCol)
            {
                if (squares[r, c].Tag != null) return false;
                r += dRow;
                c += dCol;
            }
            return true;
        }

        #endregion

        #region Check and Checkmate Detection

        private void CheckGameStatus()
        {
            string currentPlayer = currentTurn; // Player whose turn it is
            bool inCheck = IsInCheck(currentPlayer);
            bool hasLegalMoves = HasLegalMove(currentPlayer);

            if (IsInsufficientMaterial())
            {
                HandleInsufficientMaterial();
                return;
            }

            if (inCheck)
            {
                HighlightKingInCheck(currentPlayer);

                if (!hasLegalMoves)
                {
                    // Current player is checkmated - opponent wins
                    HandleCheckmate(currentPlayer);
                    return;
                }
            }
            else if (!hasLegalMoves)
            {
                HandleStalemate();
                return;
            }
            else if (kingInCheckPosition.HasValue)
            {
                // Only clear check highlight if not in check anymore
                ClearCheckHighlight();
            }

            btnOfferDraw.Enabled = gameStarted && !gameEnded;
        }

        private bool IsInCheck(string color)
        {
            try
            {
                var (kingRow, kingCol) = FindKingPosition(color);
                string opponentColor = color == "White" ? "Black" : "White";

                for (int row = 0; row < 8; row++)
                {
                    for (int col = 0; col < 8; col++)
                    {
                        var piece = squares[row, col].Tag as PieceTag;
                        if (piece != null && piece.Color == opponentColor)
                        {
                            // Skip checking if the piece is the opponent king (to prevent infinite recursion)
                            if (piece.PieceName.EndsWith("King"))
                            {
                                // Check if kings are adjacent
                                int rowDiff = Math.Abs(kingRow - row);
                                int colDiff = Math.Abs(kingCol - col);
                                if (rowDiff <= 1 && colDiff <= 1)
                                {
                                    return true;
                                }
                                continue;
                            }

                            if (ValidateRawMovement(piece, row, col, kingRow, kingCol))
                            {
                                return true;
                            }
                        }
                    }
                }
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in IsInCheck: {ex.Message}");
                return false;
            }
        }

        private bool HasLegalMove(string color)
        {
            for (int fromRow = 0; fromRow < 8; fromRow++)
            {
                for (int fromCol = 0; fromCol < 8; fromCol++)
                {
                    var piece = squares[fromRow, fromCol].Tag as PieceTag;
                    if (piece == null || piece.Color != color) continue;

                    for (int toRow = 0; toRow < 8; toRow++)
                    {
                        for (int toCol = 0; toCol < 8; toCol++)
                        {
                            if (IsMoveLegal(piece, fromRow, fromCol, toRow, toCol, false))
                            {
                                var originalFromTag = squares[fromRow, fromCol].Tag;
                                var originalToTag = squares[toRow, toCol].Tag;

                                squares[fromRow, fromCol].Tag = null;
                                squares[toRow, toCol].Tag = originalFromTag;

                                bool stillInCheck = IsInCheck(color);

                                squares[fromRow, fromCol].Tag = originalFromTag;
                                squares[toRow, toCol].Tag = originalToTag;

                                if (!stillInCheck)
                                {
                                    return true;
                                }
                            }
                        }
                    }
                }
            }
            return false;
        }

        private bool IsMoveLegal(PieceTag piece, int fromRow, int fromCol, int toRow, int toCol, bool checkOnly)
        {
            // 1. Basic validation
            if (fromRow == toRow && fromCol == toCol) return false;

            var targetPiece = squares[toRow, toCol].Tag as PieceTag;
            if (targetPiece != null && targetPiece.Color == piece.Color) return false;

            // 2. Skip check validation if we're already in check detection
            if (checkOnly)
            {
                return ValidateRawMovement(piece, fromRow, fromCol, toRow, toCol);
            }

            // 3. Normal move validation with check prevention
            var originalFromTag = squares[fromRow, fromCol].Tag;
            var originalToTag = squares[toRow, toCol].Tag;

            // Simulate move
            squares[fromRow, fromCol].Tag = null;
            squares[toRow, toCol].Tag = piece;

            bool leavesKingInCheck = IsInCheck(piece.Color);

            // Revert simulation
            squares[fromRow, fromCol].Tag = originalFromTag;
            squares[toRow, toCol].Tag = originalToTag;

            if (leavesKingInCheck) return false;

            // 4. Piece-specific movement rules
            return ValidateRawMovement(piece, fromRow, fromCol, toRow, toCol);
        }

        private bool ValidateRawMovement(PieceTag piece, int fromRow, int fromCol, int toRow, int toCol)
        {
            // Create dummy variables for out/ref parameters we don't need for basic validation
            PieceTag dummyCaptured = null;
            bool dummyEnPassant = false;
            bool dummyPromotion = false;
            bool dummyCastling = false;

            switch (piece.PieceName)
            {
                case "WhitePawn":
                case "BlackPawn":
                    return ValidatePawnMove(fromRow, fromCol, toRow, toCol, piece.Color,
                                          ref dummyCaptured, out dummyEnPassant, out dummyPromotion);

                case "WhiteKnight":
                case "BlackKnight":
                    return ValidateKnightMove(fromRow, fromCol, toRow, toCol, piece.Color, ref dummyCaptured);

                case "WhiteBishop":
                case "BlackBishop":
                    return ValidateBishopMove(fromRow, fromCol, toRow, toCol, piece.Color, ref dummyCaptured);

                case "WhiteRook":
                case "BlackRook":
                    return ValidateRookMove(fromRow, fromCol, toRow, toCol, piece.Color, ref dummyCaptured);

                case "WhiteQueen":
                case "BlackQueen":
                    return ValidateQueenMove(fromRow, fromCol, toRow, toCol, piece.Color, ref dummyCaptured);

                case "WhiteKing":
                case "BlackKing":
                    return ValidateKingMove(fromRow, fromCol, toRow, toCol, piece.Color,
                                          ref dummyCaptured, ref dummyCastling);

                default:
                    return false;
            }
        }

        private bool IsSquareUnderAttack(int row, int col, string defenderColor)
        {
            string attackerColor = defenderColor == "White" ? "Black" : "White";

            for (int r = 0; r < 8; r++)
            {
                for (int c = 0; c < 8; c++)
                {
                    var piece = squares[r, c].Tag as PieceTag;
                    if (piece != null && piece.Color == attackerColor)
                    {
                        // Skip checking if the piece is the king (to prevent infinite recursion)
                        if (piece.PieceName.EndsWith("King"))
                        {
                            // Check if kings are adjacent
                            int rowDiff = Math.Abs(row - r);
                            int colDiff = Math.Abs(col - c);
                            if (rowDiff <= 1 && colDiff <= 1)
                            {
                                return true;
                            }
                            continue;
                        }

                        if (ValidateRawMovement(piece, r, c, row, col))
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        #endregion

        #region Draw Rule Methods

        private void UpdatePositionHistory()
        {
            string positionString = GetBoardPositionString();

            if (positionCounts.ContainsKey(positionString))
            {
                positionCounts[positionString]++;
            }
            else
            {
                positionCounts[positionString] = 1;
            }

            if (positionCounts[positionString] >= 3)
            {
                HandleThreefoldRepetition();
            }
        }

        private string GetBoardPositionString()
        {
            string position = "";

            for (int row = 0; row < 8; row++)
            {
                for (int col = 0; col < 8; col++)
                {
                    var piece = squares[row, col].Tag as PieceTag;
                    position += piece == null ? "-" : $"{piece.Color[0]}{piece.PieceName[0]}";
                }
            }

            position += $"{CanCastle("White", true)}{CanCastle("White", false)}";
            position += $"{CanCastle("Black", true)}{CanCastle("Black", false)}";
            position += lastMove.HasValue ? $"{lastMove.Value}" : "null";

            return position;
        }

        private void UpdateHalfMoveClock(PieceTag movedPiece, bool isCapture, bool isPawnMove)
        {
            if (isCapture || isPawnMove)
            {
                halfMoveClock = 0;
            }
            else
            {
                halfMoveClock++;
            }

            if (halfMoveClock >= 50)
            {
                HandleFiftyMoveRule();
            }
        }

        private bool IsInsufficientMaterial()
        {
            var pieces = new List<PieceTag>();
            for (int row = 0; row < 8; row++)
            {
                for (int col = 0; col < 8; col++)
                {
                    if (squares[row, col].Tag is PieceTag piece)
                    {
                        pieces.Add(piece);
                    }
                }
            }

            if (pieces.Count == 2) return true;

            if (pieces.Count == 3)
            {
                if (pieces.Any(p => p.PieceName.EndsWith("Bishop")) ||
                    pieces.Any(p => p.PieceName.EndsWith("Knight")))
                {
                    return true;
                }
            }

            if (pieces.Count == 4)
            {
                var knights = pieces.Where(p => p.PieceName.EndsWith("Knight")).ToList();
                if (knights.Count == 2 &&
                    knights[0].Color == knights[1].Color &&
                    pieces.Count(p => p.Color == knights[0].Color) == 3)
                {
                    return true;
                }

                var bishops = pieces.Where(p => p.PieceName.EndsWith("Bishop")).ToList();
                if (bishops.Count == 2)
                {
                    bool whiteBishopOnWhite = (bishops[0].Row + bishops[0].Col) % 2 == 0;
                    bool blackBishopOnWhite = (bishops[1].Row + bishops[1].Col) % 2 == 0;

                    if (whiteBishopOnWhite == blackBishopOnWhite)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private void btnOfferDraw_Click(object sender, EventArgs e)
        {
            if (!gameStarted || gameEnded) return;

            drawOffered = true;
            btnOfferDraw.Enabled = false;

            string offeringPlayer = currentTurn;
            string otherPlayer = currentTurn == "White" ? "Black" : "White";

            // --- Custom Draw Offer Dialog ---
            Form drawDialog = new Form()
            {
                Size = new Size(400, 220),
                FormBorderStyle = FormBorderStyle.FixedDialog,
                StartPosition = FormStartPosition.CenterParent,
                BackColor = Color.White,
                Font = new Font("Segoe UI", 10),
                Text = "Draw Offer",
                MaximizeBox = false,
                MinimizeBox = false,
                ShowInTaskbar = false
            };

            Label lblTitle = new Label()
            {
                Text = $"♟ {offeringPlayer} has offered a draw.",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = Color.Black,
                AutoSize = false,
                TextAlign = ContentAlignment.MiddleCenter,
                Dock = DockStyle.Top,
                Height = 60
            };

            Label lblInfo = new Label()
            {
                Text = $"♟ {otherPlayer}, do you accept the draw offer?",
                AutoSize = false,
                TextAlign = ContentAlignment.MiddleCenter,
                Dock = DockStyle.Top,
                Height = 60
            };

            Button btnAccept = new Button()
            {
                Text = "Accept",
                DialogResult = DialogResult.Yes,
                BackColor = Color.FromArgb(40, 167, 69), // Bootstrap success green
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Width = 100,
                Height = 36,
                Margin = new Padding(10)
            };
            btnAccept.FlatAppearance.BorderSize = 0;
            btnAccept.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, btnAccept.Width, btnAccept.Height, 15, 15));

            Button btnDecline = new Button()
            {
                Text = "Decline",
                DialogResult = DialogResult.No,
                BackColor = Color.LightGray,
                ForeColor = Color.Black,
                FlatStyle = FlatStyle.Flat,
                Width = 100,
                Height = 36,
                Margin = new Padding(10)
            };
            btnDecline.FlatAppearance.BorderSize = 0;
            btnDecline.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, btnDecline.Width, btnDecline.Height, 15, 15));

            FlowLayoutPanel panelButtons = new FlowLayoutPanel()
            {
                FlowDirection = FlowDirection.RightToLeft,
                Dock = DockStyle.Bottom,
                Height = 60,
                Padding = new Padding(10)
            };
            panelButtons.Controls.Add(btnDecline);
            panelButtons.Controls.Add(btnAccept);

            drawDialog.Controls.Add(panelButtons);
            drawDialog.Controls.Add(lblInfo);
            drawDialog.Controls.Add(lblTitle);

            DialogResult result = drawDialog.ShowDialog();

            if (result == DialogResult.Yes)
            {
                gameEnded = true;
                clockTimer.Stop();
                clocksActive = false;

                // --- Custom Draw Result Dialog ---
                Form drawResultDialog = new Form()
                {
                    Size = new Size(400, 180),
                    FormBorderStyle = FormBorderStyle.FixedDialog,
                    StartPosition = FormStartPosition.CenterParent,
                    BackColor = Color.White,
                    Font = new Font("Segoe UI", 10),
                    Text = "Game Over - Draw",
                    MaximizeBox = false,
                    MinimizeBox = false,
                    ShowInTaskbar = false
                };

                Label lblResult = new Label()
                {
                    Text = "Game drawn by mutual agreement.",
                    Font = new Font("Segoe UI Semibold", 11, FontStyle.Bold),
                    AutoSize = false,
                    TextAlign = ContentAlignment.MiddleCenter,
                    Dock = DockStyle.Fill,
                    ForeColor = Color.Black
                };

                Button btnOK = new Button()
                {
                    Text = "OK",
                    DialogResult = DialogResult.OK,
                    BackColor = Color.FromArgb(0, 123, 255),
                    ForeColor = Color.White,
                    FlatStyle = FlatStyle.Flat,
                    Width = 100,
                    Height = 36,
                    Margin = new Padding(10)
                };
                btnOK.FlatAppearance.BorderSize = 0;
                btnOK.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, btnOK.Width, btnOK.Height, 15, 15));

                FlowLayoutPanel okPanel = new FlowLayoutPanel()
                {
                    FlowDirection = FlowDirection.RightToLeft,
                    Dock = DockStyle.Bottom,
                    Height = 60,
                    Padding = new Padding(10)
                };
                okPanel.Controls.Add(btnOK);

                drawResultDialog.Controls.Add(okPanel);
                drawResultDialog.Controls.Add(lblResult);
                drawResultDialog.ShowDialog();

                // Disable buttons
                btnResign.Enabled = false;
                btnOfferDraw.Enabled = false;
                pnlChessBoard.Enabled = false;
            }
            else
            {
                drawOffered = false;
                btnOfferDraw.Enabled = true;
            }
        }

        private void HandleThreefoldRepetition()
        {
            gameEnded = true;
            clockTimer?.Stop();
            clocksActive = false;
            pnlChessBoard.Enabled = false;

            // --- Custom Threefold Repetition Result Dialog ---
            Form drawResultDialog = new Form()
            {
                Size = new Size(400, 220),
                FormBorderStyle = FormBorderStyle.FixedDialog,
                StartPosition = FormStartPosition.CenterParent,
                BackColor = Color.White,
                Font = new Font("Segoe UI", 10),
                Text = "Game Over - Draw",
                MaximizeBox = false,
                MinimizeBox = false,
                ShowInTaskbar = false
            };

            Label lblResult = new Label()
            {
                Text = "The same position has occurred three times.\nDraw by threefold repetition!",
                Font = new Font("Segoe UI Semibold", 11, FontStyle.Bold),
                AutoSize = false,
                TextAlign = ContentAlignment.MiddleCenter,
                Dock = DockStyle.Fill,
                ForeColor = Color.Black
            };

            Button btnOK = new Button()
            {
                Text = "OK",
                DialogResult = DialogResult.OK,
                BackColor = Color.FromArgb(0, 123, 255),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Width = 100,
                Height = 36,
                Margin = new Padding(10)
            };
            btnOK.FlatAppearance.BorderSize = 0;
            btnOK.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, btnOK.Width, btnOK.Height, 15, 15));

            FlowLayoutPanel okPanel = new FlowLayoutPanel()
            {
                FlowDirection = FlowDirection.RightToLeft,
                Dock = DockStyle.Bottom,
                Height = 60,
                Padding = new Padding(10)
            };
            okPanel.Controls.Add(btnOK);

            drawResultDialog.Controls.Add(okPanel);
            drawResultDialog.Controls.Add(lblResult);
            drawResultDialog.ShowDialog();
        }

        private void HandleFiftyMoveRule()
        {
            gameEnded = true;
            clockTimer?.Stop();
            clocksActive = false;
            pnlChessBoard.Enabled = false;

            // --- Custom 50-Move Rule Draw Dialog ---
            Form drawResultDialog = new Form()
            {
                Size = new Size(400, 220),
                FormBorderStyle = FormBorderStyle.FixedDialog,
                StartPosition = FormStartPosition.CenterParent,
                BackColor = Color.White,
                Font = new Font("Segoe UI", 10),
                Text = "Game Over - Draw",
                MaximizeBox = false,
                MinimizeBox = false,
                ShowInTaskbar = false
            };

            Label lblResult = new Label()
            {
                Text = "Fifty moves have passed with no capture or pawn move.\nDraw by the 50-move rule!",
                Font = new Font("Segoe UI Semibold", 11, FontStyle.Bold),
                AutoSize = false,
                TextAlign = ContentAlignment.MiddleCenter,
                Dock = DockStyle.Fill,
                ForeColor = Color.Black
            };

            Button btnOK = new Button()
            {
                Text = "OK",
                DialogResult = DialogResult.OK,
                BackColor = Color.FromArgb(0, 123, 255),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Width = 100,
                Height = 36,
                Margin = new Padding(10)
            };
            btnOK.FlatAppearance.BorderSize = 0;
            btnOK.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, btnOK.Width, btnOK.Height, 15, 15));

            FlowLayoutPanel okPanel = new FlowLayoutPanel()
            {
                FlowDirection = FlowDirection.RightToLeft,
                Dock = DockStyle.Bottom,
                Height = 60,
                Padding = new Padding(10)
            };
            okPanel.Controls.Add(btnOK);

            drawResultDialog.Controls.Add(okPanel);
            drawResultDialog.Controls.Add(lblResult);
            drawResultDialog.ShowDialog();
        }

        private void HandleInsufficientMaterial()
        {
            gameEnded = true;
            clockTimer?.Stop();
            clocksActive = false;
            pnlChessBoard.Enabled = false;

            // --- Custom Insufficient Material Draw Dialog ---
            Form drawResultDialog = new Form()
            {
                Size = new Size(400, 220),
                FormBorderStyle = FormBorderStyle.FixedDialog,
                StartPosition = FormStartPosition.CenterParent,
                BackColor = Color.White,
                Font = new Font("Segoe UI", 10),
                Text = "Game Over - Draw",
                MaximizeBox = false,
                MinimizeBox = false,
                ShowInTaskbar = false
            };

            Label lblResult = new Label()
            {
                Text = "Neither player has sufficient material to checkmate.\nDraw by insufficient material!",
                Font = new Font("Segoe UI Semibold", 11, FontStyle.Bold),
                AutoSize = false,
                TextAlign = ContentAlignment.MiddleCenter,
                Dock = DockStyle.Fill,
                ForeColor = Color.Black
            };

            Button btnOK = new Button()
            {
                Text = "OK",
                DialogResult = DialogResult.OK,
                BackColor = Color.FromArgb(0, 123, 255),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Width = 100,
                Height = 36,
                Margin = new Padding(10)
            };
            btnOK.FlatAppearance.BorderSize = 0;
            btnOK.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, btnOK.Width, btnOK.Height, 15, 15));

            FlowLayoutPanel okPanel = new FlowLayoutPanel()
            {
                FlowDirection = FlowDirection.RightToLeft,
                Dock = DockStyle.Bottom,
                Height = 60,
                Padding = new Padding(10)
            };
            okPanel.Controls.Add(btnOK);

            drawResultDialog.Controls.Add(okPanel);
            drawResultDialog.Controls.Add(lblResult);
            drawResultDialog.ShowDialog();
        }

        #endregion

        #region Helper Methods

        private (int, int) FindKingPosition(string color)
        {
            for (int r = 0; r < 8; r++)
            {
                for (int c = 0; c < 8; c++)
                {
                    var tag = squares[r, c].Tag as PieceTag;
                    if (tag != null && tag.Color == color && tag.PieceName.EndsWith("King"))
                        return (r, c);
                }
            }
            throw new Exception("King not found - invalid board state");
        }

        private void HighlightKingInCheck(string color)
        {
            try
            {
                var (kingRow, kingCol) = FindKingPosition(color);

                this.Invoke((MethodInvoker)delegate {
                    // Only update if the king position has changed or isn't already highlighted
                    if (!kingInCheckPosition.HasValue ||
                        kingInCheckPosition.Value.row != kingRow ||
                        kingInCheckPosition.Value.col != kingCol)
                    {
                        // Clear previous highlight if exists
                        if (kingInCheckPosition.HasValue)
                        {
                            var (prevRow, prevCol) = kingInCheckPosition.Value;
                            squares[prevRow, prevCol].BackColor = (prevRow + prevCol) % 2 == 0 ? Color.White : Color.Gray;
                            squares[prevRow, prevCol].BorderStyle = BorderStyle.None;
                        }

                        // Apply new highlight
                        squares[kingRow, kingCol].BackColor = Color.Red;
                        squares[kingRow, kingCol].BorderStyle = BorderStyle.Fixed3D;
                        kingInCheckPosition = (kingRow, kingCol);
                    }
                    squares[kingRow, kingCol].Refresh();
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in HighlightKingInCheck: {ex.Message}");
            }
        }

        private void ClearCheckHighlight()
        {
            if (kingInCheckPosition.HasValue)
            {
                var (row, col) = kingInCheckPosition.Value;
                squares[row, col].BackColor = (row + col) % 2 == 0 ? Color.White : Color.Gray;
                squares[row, col].BorderStyle = BorderStyle.None;
                squares[row, col].Refresh();
                kingInCheckPosition = null;
            }
        }

        private void ClearHighlights()
        {
            for (int row = 0; row < 8; row++)
            {
                for (int col = 0; col < 8; col++)
                {
                    if (kingInCheckPosition.HasValue &&
                        kingInCheckPosition.Value.row == row &&
                        kingInCheckPosition.Value.col == col)
                    {
                        continue;
                    }

                    // Skip if this is the last moved position
                    if (lastMovedPosition.HasValue &&
                        lastMovedPosition.Value.row == row &&
                        lastMovedPosition.Value.col == col)
                    {
                        continue;
                    }

                    squares[row, col].BackColor = (row + col) % 2 == 0 ? Color.White : Color.Gray;
                    squares[row, col].BorderStyle = BorderStyle.None;
                }
            }
        }

        private void ShowLegalMoves(PieceTag tag)
        {
            ClearHighlights();
            squares[tag.Row, tag.Col].BackColor = Color.Yellow;

            int row = tag.Row;
            int col = tag.Col;
            int dir = tag.Color == "White" ? -1 : 1;
            string name = tag.PieceName;

            if (name.EndsWith("Pawn"))
            {
                ShowPawnMoves(tag, row, col, dir);
            }
            else if (name.EndsWith("Knight"))
            {
                ShowKnightMoves(tag, row, col);
            }
            else if (name.EndsWith("Bishop") || name.EndsWith("Rook") || name.EndsWith("Queen"))
            {
                ShowSlidingMoves(tag, row, col, name);
            }
            else if (name.EndsWith("King"))
            {
                ShowKingMoves(tag, row, col);
            }
        }

        private void ShowPawnMoves(PieceTag tag, int row, int col, int dir)
        {
            int oneStep = row + dir;
            int twoStep = row + 2 * dir;

            if (IsInsideBoard(oneStep, col) && squares[oneStep, col].Tag == null)
            {
                HighlightMove(oneStep, col, false);

                if (!tag.HasMoved && IsInsideBoard(twoStep, col) && squares[twoStep, col].Tag == null)
                {
                    HighlightMove(twoStep, col, false);
                }
            }

            for (int d = -1; d <= 1; d += 2)
            {
                int newCol = col + d;
                if (IsInsideBoard(oneStep, newCol))
                {
                    var target = squares[oneStep, newCol].Tag as PieceTag;
                    if (target != null && target.Color != tag.Color)
                    {
                        HighlightMove(oneStep, newCol, true);
                    }
                }
            }

            if (lastMove.HasValue)
            {
                var (fromRow, fromCol, toRow, toCol) = lastMove.Value;
                if (Math.Abs(fromRow - toRow) == 2 &&
                    toRow == row && Math.Abs(toCol - col) == 1)
                {
                    var adjPawn = squares[toRow, toCol].Tag as PieceTag;
                    if (adjPawn != null &&
                        adjPawn.PieceName.EndsWith("Pawn") &&
                        adjPawn.Color != tag.Color)
                    {
                        HighlightMove(toRow + dir, toCol, true, true);
                    }
                }
            }
        }

        private void ShowKnightMoves(PieceTag tag, int row, int col)
        {
            int[][] offsets = {
                new int[] { -2, -1 }, new int[] { -2, 1 },
                new int[] { -1, -2 }, new int[] { -1, 2 },
                new int[] { 1, -2 }, new int[] { 1, 2 },
                new int[] { 2, -1 }, new int[] { 2, 1 }
            };

            foreach (var offset in offsets)
            {
                int r = row + offset[0];
                int c = col + offset[1];

                if (IsInsideBoard(r, c))
                {
                    var target = squares[r, c].Tag as PieceTag;
                    if (target == null || target.Color != tag.Color)
                    {
                        HighlightMove(r, c, target != null);
                    }
                }
            }
        }

        private void ShowSlidingMoves(PieceTag tag, int row, int col, string name)
        {
            List<int[]> directions = new List<int[]>();

            if (name.EndsWith("Bishop") || name.EndsWith("Queen"))
            {
                directions.AddRange(new[] {
                    new int[] { -1, -1 }, new int[] { -1, 1 },
                    new int[] { 1, -1 },  new int[] { 1, 1 }
                });
            }

            if (name.EndsWith("Rook") || name.EndsWith("Queen"))
            {
                directions.AddRange(new[] {
                    new int[] { -1, 0 }, new int[] { 1, 0 },
                    new int[] { 0, -1 }, new int[] { 0, 1 }
                });
            }

            foreach (var dirPair in directions)
            {
                int dr = dirPair[0];
                int dc = dirPair[1];

                int r = row + dr, c = col + dc;
                while (IsInsideBoard(r, c))
                {
                    var target = squares[r, c].Tag as PieceTag;
                    if (target == null)
                    {
                        HighlightMove(r, c, false);
                    }
                    else
                    {
                        if (target.Color != tag.Color)
                            HighlightMove(r, c, true);
                        break;
                    }
                    r += dr;
                    c += dc;
                }
            }
        }

        private void ShowKingMoves(PieceTag tag, int row, int col)
        {
            // Normal king moves (1 square in any direction)
            for (int dr = -1; dr <= 1; dr++)
            {
                for (int dc = -1; dc <= 1; dc++)
                {
                    if (dr == 0 && dc == 0) continue; // Skip current position

                    int r = row + dr;
                    int c = col + dc;

                    if (IsInsideBoard(r, c))
                    {
                        var target = squares[r, c].Tag as PieceTag;

                        // Only highlight if:
                        // 1. Square is empty OR contains opponent's piece
                        // 2. Square is not under attack
                        if (target == null || target.Color != tag.Color)
                        {
                            // Simulate move to check if it would leave king in check
                            var originalFromTag = squares[row, col].Tag;
                            var originalToTag = squares[r, c].Tag;

                            squares[row, col].Tag = null;
                            squares[r, c].Tag = tag;

                            bool leavesInCheck = IsInCheck(tag.Color);

                            squares[row, col].Tag = originalFromTag;
                            squares[r, c].Tag = originalToTag;

                            if (!leavesInCheck)
                            {
                                HighlightMove(r, c, target != null);
                            }
                        }
                    }
                }
            }

            // Castling moves (only if king hasn't moved and not in check)
            if (!tag.HasMoved && !IsInCheck(tag.Color))
            {
                // King-side castling
                if (CanCastle(tag.Color, true))
                {
                    int castleCol = tag.Color == "White" ? 6 : 6;
                    HighlightMove(row, castleCol, false, false, true);
                }

                // Queen-side castling
                if (CanCastle(tag.Color, false))
                {
                    int castleCol = tag.Color == "White" ? 2 : 2;
                    HighlightMove(row, castleCol, false, false, true);
                }
            }
        }

        private void HighlightMove(int row, int col, bool isCapture, bool isEnPassant = false, bool isCastling = false)
        {
            if (isCastling)
                squares[row, col].BackColor = Color.CornflowerBlue;
            else if (isEnPassant)
                squares[row, col].BackColor = Color.Orange;
            else if (isCapture)
                squares[row, col].BackColor = Color.OrangeRed;
            else
                squares[row, col].BackColor = Color.LightGreen;
        }

        private bool IsInsideBoard(int row, int col)
        {
            return row >= 0 && row < 8 && col >= 0 && col < 8;
        }

        private int GetRow(PictureBox pb)
        {
            if (pb == null || !pb.Name.StartsWith("pbSquare_"))
                throw new ArgumentException("Invalid PictureBox");

            string[] parts = pb.Name.Split('_');
            if (parts.Length < 3)
                throw new ArgumentException("Invalid PictureBox name format");

            return int.Parse(parts[1]);
        }

        private int GetCol(PictureBox pb)
        {
            if (pb == null || !pb.Name.StartsWith("pbSquare_"))
                throw new ArgumentException("Invalid PictureBox");

            string[] parts = pb.Name.Split('_');
            if (parts.Length < 3)
                throw new ArgumentException("Invalid PictureBox name format");

            return int.Parse(parts[2]);
        }

        private void FlashKingSquare(int flashes = 3)
        {
            if (!kingInCheckPosition.HasValue) return;

            var (row, col) = kingInCheckPosition.Value;
            var square = squares[row, col];

            Color originalColor = (row + col) % 2 == 0 ? Color.White : Color.Gray;

            for (int i = 0; i < flashes; i++)
            {
                square.BackColor = Color.Red;
                square.Refresh();
                System.Threading.Thread.Sleep(150);
                square.BackColor = originalColor;
                square.Refresh();
                System.Threading.Thread.Sleep(150);
            }
        }

        #endregion

        #region Game End Handling

        private void HandleCheckmate(string checkmatedColor)
        {
            // Winner is always the opponent of the checkmated player
            string winner = (checkmatedColor == "White") ? "Black" : "White";
            gameEnded = true;

            // Flash the checkmated king's square
            var (kingRow, kingCol) = FindKingPosition(checkmatedColor);
            for (int i = 0; i < 5; i++)
            {
                this.Invoke((MethodInvoker)delegate {
                    squares[kingRow, kingCol].BackColor = i % 2 == 0 ? Color.DarkRed : Color.Red;
                    squares[kingRow, kingCol].Refresh();
                });
                System.Threading.Thread.Sleep(200);
            }

            // Stop game
            clockTimer?.Stop();
            clocksActive = false;
            pnlChessBoard.Enabled = false;

            // --- Modern Checkmate Dialog ---
            Form resultDialog = new Form()
            {
                Size = new Size(400, 220),
                FormBorderStyle = FormBorderStyle.FixedDialog,
                StartPosition = FormStartPosition.CenterParent,
                BackColor = Color.White,
                Font = new Font("Segoe UI", 10),
                Text = $"{winner} Wins!",
                MaximizeBox = false,
                MinimizeBox = false,
                ShowInTaskbar = false
            };

            Label lblResult = new Label()
            {
                Text = $"♛ CHECKMATE!\n{winner} wins against {checkmatedColor}.",
                Font = new Font("Segoe UI Semibold", 12, FontStyle.Bold),
                AutoSize = false,
                TextAlign = ContentAlignment.MiddleCenter,
                Dock = DockStyle.Fill,
                ForeColor = Color.Black
            };

            Button btnOK = new Button()
            {
                Text = "OK",
                DialogResult = DialogResult.OK,
                BackColor = Color.FromArgb(0, 123, 255),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Width = 100,
                Height = 36,
                Margin = new Padding(10)
            };
            btnOK.FlatAppearance.BorderSize = 0;
            btnOK.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, btnOK.Width, btnOK.Height, 15, 15));

            FlowLayoutPanel okPanel = new FlowLayoutPanel()
            {
                FlowDirection = FlowDirection.RightToLeft,
                Dock = DockStyle.Bottom,
                Height = 60,
                Padding = new Padding(10)
            };
            okPanel.Controls.Add(btnOK);

            resultDialog.Controls.Add(okPanel);
            resultDialog.Controls.Add(lblResult);
            resultDialog.ShowDialog();
        }

        private void HandleStalemate()
        {
            gameEnded = true;
            clockTimer?.Stop();
            clocksActive = false;
            pnlChessBoard.Enabled = false;

            // --- Modern Stalemate Dialog ---
            Form drawResultDialog = new Form()
            {
                Size = new Size(400, 220),
                FormBorderStyle = FormBorderStyle.FixedDialog,
                StartPosition = FormStartPosition.CenterParent,
                BackColor = Color.White,
                Font = new Font("Segoe UI", 10),
                Text = "Game Over - Draw",
                MaximizeBox = false,
                MinimizeBox = false,
                ShowInTaskbar = false
            };

            Label lblResult = new Label()
            {
                Text = "♟ STALEMATE!\nThe game ends in a draw by stalemate.",
                Font = new Font("Segoe UI Semibold", 11, FontStyle.Bold),
                AutoSize = false,
                TextAlign = ContentAlignment.MiddleCenter,
                Dock = DockStyle.Fill,
                ForeColor = Color.Black
            };

            Button btnOK = new Button()
            {
                Text = "OK",
                DialogResult = DialogResult.OK,
                BackColor = Color.FromArgb(0, 123, 255),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Width = 100,
                Height = 36,
                Margin = new Padding(10)
            };
            btnOK.FlatAppearance.BorderSize = 0;
            btnOK.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, btnOK.Width, btnOK.Height, 15, 15));

            FlowLayoutPanel okPanel = new FlowLayoutPanel()
            {
                FlowDirection = FlowDirection.RightToLeft,
                Dock = DockStyle.Bottom,
                Height = 60,
                Padding = new Padding(10)
            };
            okPanel.Controls.Add(btnOK);

            drawResultDialog.Controls.Add(okPanel);
            drawResultDialog.Controls.Add(lblResult);
            drawResultDialog.ShowDialog();

            // Optional: Highlight king's square in gray
            try
            {
                var currentColor = currentTurn;
                var (kingRow, kingCol) = FindKingPosition(currentColor);
                squares[kingRow, kingCol].BackColor = Color.LightGray;
                squares[kingRow, kingCol].Refresh();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error highlighting king in stalemate: {ex.Message}");
            }
        }

        #endregion

        #region Clock and Timer Methods

        private void ClockTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            this.Invoke((MethodInvoker)delegate {
                if (clocksActive)
                {
                    if (isWhiteTurn)
                    {
                        whiteTime = whiteTime.Subtract(TimeSpan.FromSeconds(1));
                        if (whiteTime.TotalSeconds <= 0)
                        {
                            whiteTime = TimeSpan.Zero;
                            clockTimer.Stop();
                            clocksActive = false;
                            pnlChessBoard.Enabled = false;
                            gameStarted = false;

                            // --- Custom Timeout Dialog for White ---
                            Form timeoutDialog = new Form()
                            {
                                Size = new Size(400, 220),
                                FormBorderStyle = FormBorderStyle.FixedDialog,
                                StartPosition = FormStartPosition.CenterParent,
                                BackColor = Color.White,
                                Font = new Font("Segoe UI", 10),
                                Text = "Time Out",
                                MaximizeBox = false,
                                MinimizeBox = false,
                                ShowInTaskbar = false
                            };

                            Label lblResult = new Label()
                            {
                                Text = "⏱ White's time has expired!\nBlack wins on time.",
                                Font = new Font("Segoe UI Semibold", 11, FontStyle.Bold),
                                AutoSize = false,
                                TextAlign = ContentAlignment.MiddleCenter,
                                Dock = DockStyle.Fill,
                                ForeColor = Color.Black
                            };

                            Button btnOK = new Button()
                            {
                                Text = "OK",
                                DialogResult = DialogResult.OK,
                                BackColor = Color.FromArgb(0, 123, 255),
                                ForeColor = Color.White,
                                FlatStyle = FlatStyle.Flat,
                                Width = 100,
                                Height = 36,
                                Margin = new Padding(10)
                            };
                            btnOK.FlatAppearance.BorderSize = 0;
                            btnOK.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, btnOK.Width, btnOK.Height, 15, 15));

                            FlowLayoutPanel okPanel = new FlowLayoutPanel()
                            {
                                FlowDirection = FlowDirection.RightToLeft,
                                Dock = DockStyle.Bottom,
                                Height = 60,
                                Padding = new Padding(10)
                            };
                            okPanel.Controls.Add(btnOK);

                            timeoutDialog.Controls.Add(okPanel);
                            timeoutDialog.Controls.Add(lblResult);
                            timeoutDialog.ShowDialog();
                        }
                        lblTopClock.Text = string.Format("{0:mm\\:ss}", whiteTime);
                    }
                    else
                    {
                        blackTime = blackTime.Subtract(TimeSpan.FromSeconds(1));
                        if (blackTime.TotalSeconds <= 0)
                        {
                            blackTime = TimeSpan.Zero;
                            clockTimer.Stop();
                            clocksActive = false;
                            pnlChessBoard.Enabled = false;
                            gameStarted = false;

                            // --- Custom Timeout Dialog for Black ---
                            Form timeoutDialog = new Form()
                            {
                                Size = new Size(400, 220),
                                FormBorderStyle = FormBorderStyle.FixedDialog,
                                StartPosition = FormStartPosition.CenterParent,
                                BackColor = Color.White,
                                Font = new Font("Segoe UI", 10),
                                Text = "Time Out",
                                MaximizeBox = false,
                                MinimizeBox = false,
                                ShowInTaskbar = false
                            };

                            Label lblResult = new Label()
                            {
                                Text = "⏱ Black's time has expired!\nWhite wins on time.",
                                Font = new Font("Segoe UI Semibold", 11, FontStyle.Bold),
                                AutoSize = false,
                                TextAlign = ContentAlignment.MiddleCenter,
                                Dock = DockStyle.Fill,
                                ForeColor = Color.Black
                            };

                            Button btnOK = new Button()
                            {
                                Text = "OK",
                                DialogResult = DialogResult.OK,
                                BackColor = Color.FromArgb(0, 123, 255),
                                ForeColor = Color.White,
                                FlatStyle = FlatStyle.Flat,
                                Width = 100,
                                Height = 36,
                                Margin = new Padding(10)
                            };
                            btnOK.FlatAppearance.BorderSize = 0;
                            btnOK.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, btnOK.Width, btnOK.Height, 15, 15));

                            FlowLayoutPanel okPanel = new FlowLayoutPanel()
                            {
                                FlowDirection = FlowDirection.RightToLeft,
                                Dock = DockStyle.Bottom,
                                Height = 60,
                                Padding = new Padding(10)
                            };
                            okPanel.Controls.Add(btnOK);

                            timeoutDialog.Controls.Add(okPanel);
                            timeoutDialog.Controls.Add(lblResult);
                            timeoutDialog.ShowDialog();
                        }
                        lblBottomClock.Text = string.Format("{0:mm\\:ss}", blackTime);
                    }
                }
            });
        }

        #endregion

        #region Piece Loading and Captures

        private void LoadPieces()
        {
            ClearBoard();

            foreach (var square in squares)
            {
                if (square != null)
                {
                    square.Click -= Square_Click;
                }
            }

            // Pawns
            for (int col = 0; col < 8; col++)
            {
                squares[6, col].Image = Properties.Resources.white_pawn;
                squares[6, col].Tag = new PieceTag { Row = 6, Col = col, PieceName = "WhitePawn", Color = "White", HasMoved = false };

                squares[1, col].Image = Properties.Resources.black_pawn;
                squares[1, col].Tag = new PieceTag { Row = 1, Col = col, PieceName = "BlackPawn", Color = "Black", HasMoved = false };
            }

            // White pieces
            squares[7, 0].Image = Properties.Resources.white_rook;
            squares[7, 0].Tag = new PieceTag { Row = 7, Col = 0, PieceName = "WhiteRook", Color = "White" };

            squares[7, 1].Image = Properties.Resources.white_knight;
            squares[7, 1].Tag = new PieceTag { Row = 7, Col = 1, PieceName = "WhiteKnight", Color = "White" };

            squares[7, 2].Image = Properties.Resources.white_bishop;
            squares[7, 2].Tag = new PieceTag { Row = 7, Col = 2, PieceName = "WhiteBishop", Color = "White" };

            squares[7, 3].Image = Properties.Resources.white_queen;
            squares[7, 3].Tag = new PieceTag { Row = 7, Col = 3, PieceName = "WhiteQueen", Color = "White" };

            squares[7, 4].Image = Properties.Resources.white_king;
            squares[7, 4].Tag = new PieceTag { Row = 7, Col = 4, PieceName = "WhiteKing", Color = "White" };

            squares[7, 5].Image = Properties.Resources.white_bishop;
            squares[7, 5].Tag = new PieceTag { Row = 7, Col = 5, PieceName = "WhiteBishop", Color = "White" };

            squares[7, 6].Image = Properties.Resources.white_knight;
            squares[7, 6].Tag = new PieceTag { Row = 7, Col = 6, PieceName = "WhiteKnight", Color = "White" };

            squares[7, 7].Image = Properties.Resources.white_rook;
            squares[7, 7].Tag = new PieceTag { Row = 7, Col = 7, PieceName = "WhiteRook", Color = "White" };

            // Black pieces
            squares[0, 0].Image = Properties.Resources.black_rook;
            squares[0, 0].Tag = new PieceTag { Row = 0, Col = 0, PieceName = "BlackRook", Color = "Black" };

            squares[0, 1].Image = Properties.Resources.black_knight;
            squares[0, 1].Tag = new PieceTag { Row = 0, Col = 1, PieceName = "BlackKnight", Color = "Black" };

            squares[0, 2].Image = Properties.Resources.black_bishop;
            squares[0, 2].Tag = new PieceTag { Row = 0, Col = 2, PieceName = "BlackBishop", Color = "Black" };

            squares[0, 3].Image = Properties.Resources.black_queen;
            squares[0, 3].Tag = new PieceTag { Row = 0, Col = 3, PieceName = "BlackQueen", Color = "Black" };

            squares[0, 4].Image = Properties.Resources.black_king;
            squares[0, 4].Tag = new PieceTag { Row = 0, Col = 4, PieceName = "BlackKing", Color = "Black" };

            squares[0, 5].Image = Properties.Resources.black_bishop;
            squares[0, 5].Tag = new PieceTag { Row = 0, Col = 5, PieceName = "BlackBishop", Color = "Black" };

            squares[0, 6].Image = Properties.Resources.black_knight;
            squares[0, 6].Tag = new PieceTag { Row = 0, Col = 6, PieceName = "BlackKnight", Color = "Black" };

            squares[0, 7].Image = Properties.Resources.black_rook;
            squares[0, 7].Tag = new PieceTag { Row = 0, Col = 7, PieceName = "BlackRook", Color = "Black" };

            if (gameStarted)
            {
                AttachSquareClickHandlers();
                foreach (var square in squares)
                {
                    if (square != null)
                    {
                        square.Click += Square_Click;
                    }
                }
            }
        }

        private void AddCapturedPiece(PieceTag captured, string byColor)
        {
            const int SPACING = 30; // Reduced from 42 to 30 (adjust as needed)
            const int PIECE_SIZE = 30; // Matches your PictureBox size

            PictureBox pb = new PictureBox
            {
                Size = new Size(PIECE_SIZE, PIECE_SIZE),
                SizeMode = PictureBoxSizeMode.StretchImage,
                BackColor = Color.Transparent
            };

            switch (captured.PieceName)
            {
                case "WhitePawn": pb.Image = Properties.Resources.white_pawn; break;
                case "WhiteRook": pb.Image = Properties.Resources.white_rook; break;
                case "WhiteQueen": pb.Image = Properties.Resources.white_queen; break;
                case "WhiteKnight": pb.Image = Properties.Resources.white_knight; break;
                case "WhiteBishop": pb.Image = Properties.Resources.white_bishop; break;
                case "WhiteKing": pb.Image = Properties.Resources.white_king; break;

                case "BlackPawn": pb.Image = Properties.Resources.black_pawn; break;
                case "BlackRook": pb.Image = Properties.Resources.black_rook; break;
                case "BlackQueen": pb.Image = Properties.Resources.black_queen; break;
                case "BlackKnight": pb.Image = Properties.Resources.black_knight; break;
                case "BlackBishop": pb.Image = Properties.Resources.black_bishop; break;
                case "BlackKing": pb.Image = Properties.Resources.black_king; break;
            }

            if (byColor == "White")
            {
                int x = blackCapturesCount * SPACING;
                pb.Location = new Point(x, 0);
                pnlBlackCaptured.Controls.Add(pb);
                blackCapturesCount++;
            }
            else
            {
                int x = whiteCapturesCount * SPACING;
                pb.Location = new Point(x, 0);
                pnlWhiteCaptured.Controls.Add(pb);
                whiteCapturesCount++;
            }
        }
        #endregion
    }
}