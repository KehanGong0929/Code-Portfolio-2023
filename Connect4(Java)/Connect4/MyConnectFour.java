import java.util.Scanner;
import java.util.Random;

public class MyConnectFour{
    
    private Board board;
    private Player player1;
    private Player player2;
    private Judge judge;

    private boolean isYourTurn; // Identify if it is the human player's turn.

    public MyConnectFour(Player player1, Player player2){
        this.board = new Board(); //Initialize the board and players in constructer.
        this.player1 = player1;
        this.player2 = player2;
        this.judge = new Judge();
    }
    
    public void playGame(){
        isYourTurn = true; 
        boolean playing = true; // Indentify if the game is running.
        Player player;
        System.out.println("Welcome to Connect 4");
        System.out.println("There are 2 players red and yellow");
        // Get two player's name and assign the color.
        System.out.println(player1.getName() + " : " + player1.getPiece() + " , " + player2.getName() + " : " + player2.getPiece() + " ."); 
        System.out.println("To play the game type in the number of the column you want to drop you counter in");
        System.out.println("A player wins by connecting 4 counters in a row - vertically, horizontally or diagonally");
        while(playing){
            board.printBoard();
            boolean success = false; //An identifiy to see whether the piece successfully set or not.
            
            if(isYourTurn){ // Always begin from the human player.
                player = player1;
                System.out.println("Type in the number of the column you want to drop in");
                System.out.print("your choice is: ");
                Scanner input = new Scanner(System.in);
                int col = input.nextInt() -1; //The column is begin from 1.
                board.setPiece(col,player);
                success = board.isSetPiece(col);              
            }else{
                player = player2;
                //Generate a random choice to stimulate an agent.
                Random random = new Random();
                int col = random.nextInt(6) +1;
                System.out.println(player.getName() + "'s choice is: " + col); //Print out what did the computer choose.
                board.setPiece(col,player);
                success = board.isSetPiece(col);
            }
            
            if(success){
                if(judge.checkWin(board, player)){ // Call the judge to validate who is winning per turn.
                    playing = false; // If anyone won the game, it will stop.
                    if(isYourTurn){ // Human player won?
                        System.out.println("Congratulations, you won!");
                        board.printBoard();
                    }else{ // Computer player won?
                        System.out.println("Sorry, the computer won!");
                        board.printBoard();
                    }
                }
                isYourTurn = !isYourTurn; // If one player finished the move, turn to another one.
            }
        }
    }
}
