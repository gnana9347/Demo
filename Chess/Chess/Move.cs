namespace Chess
{
    public class Move
    {
        public int FromRow { get; set; }
        public int FromCol { get; set; }
        public int ToRow { get; set; }
        public int ToCol { get; set; }
        public PieceTag Piece { get; set; }
        public PieceTag Captured { get; set; }
        public bool IsCastling { get; set; }
        public bool IsEnPassant { get; set; }
        public bool IsPromotion { get; set; }
    }
}
