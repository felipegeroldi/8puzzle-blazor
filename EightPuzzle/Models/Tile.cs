namespace EightPuzzle.Models
{
    public class Tile
    {
        public Tile()
        {
        }

        public Tile(int value, string color)
        {
            Value = value;
            Color = color;
        }

        public int Value { get; set; }
        public string Color { get; set; }
    }
}
