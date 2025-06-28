namespace Chess
{
    public class PieceTag
    {
        public int Row { get; set; }
        public int Col { get; set; }
        public string PieceName { get; set; } // e.g., "WhitePawn", "BlackRook"
        public string Color { get; set; }     // "White" or "Black"
        public bool HasMoved { get; set; }    // for pawn/castling
        public bool ShowDot { get; set; }
    }
}
