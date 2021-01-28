namespace Grid
{
    /// <summary>
    /// Source https://github.com/lordjesus/Packt-Introduction-to-graph-algorithms-for-game-developers
    /// Altered by David Schmidt
    /// TODO: Move to C++
    /// </summary>
    public class Point
    {
        public int X { get; }
        public int Y { get; }

        public Point(int x, int y)
        {
            X = x;
            Y = y;
        }

        public override bool Equals(object obj)
        {
            switch (obj)
            {
                case null:
                    return false;
                case Point point:
                {
                    var p = point;
                    return X == p.X && Y == p.Y;
                }
                default:
                    return false;
            }
        }
    
        public override int GetHashCode()
        {
            unchecked
            {
                var hash = 6949;
                hash = hash * 7907 + X.GetHashCode();
                hash = hash * 7907 + Y.GetHashCode();
                return hash;
            }
        }

        public override string ToString() => $"P({X}|{Y})";
    }
}
