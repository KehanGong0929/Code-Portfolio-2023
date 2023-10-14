public class Main {
    public static void main(String[] args){
        // Set and initialise player's indentities.
        Player human = new Player("You", Piece.RED);
        Player computer = new Player("Computer", Piece.YELLOW);
        MyConnectFour game = new MyConnectFour(human,computer); // Put the players in game.
        game.playGame();
    }
}
