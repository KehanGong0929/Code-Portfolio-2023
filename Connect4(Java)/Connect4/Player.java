public class Player {
    String name;
    Piece piece; 
    //A player has two attributes: name and piece color.
    public Player(String name, Piece piece) { 
        this.name = name;
        this.piece = piece;
    }

    public String getName() {
        return name;
    }

    public Piece getPiece() { 
        return piece; 
    }
}
