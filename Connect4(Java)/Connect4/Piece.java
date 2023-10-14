enum Piece {  // A special class that implemented in the Player class.
    NONE(" "),
    RED("r"),
    YELLOW("y");

    private String color;

    Piece(String color) {
        this.color = color;
    }

    public String toString() {  // Get the piece's color.
        return String.format("[%s]", color);
    }
}