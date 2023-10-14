

public class Board {
     
    private static final int row = 6;
    private static final int column = 7;
    //Set the size of a board by using pieces as the content.
    public Piece[][] myBoard = new Piece[row][column];
  
    public void setPiece(int columnPick, Player player){ // Get the piece from Player's class.
        boolean isSet = isSetPiece(columnPick);
        if(isSet){
            for(int i = row -1; i>=0; i--){ // Check the toppiest position of the column from top to bottom.
                if (myBoard[i][columnPick]==Piece.NONE){ // Verify if the target position has been occupied.
                    //System.out.println("success!");
                    myBoard[i][columnPick] = player.getPiece(); // Write the current player's piece in to the board.
                    break;
                }
            }  
        }
    }

    public boolean isSetPiece(int columnPick){
        //check if the piece can be set in the column.
        if(columnPick >=0 && columnPick<=row){
            if(myBoard[0][columnPick] == Piece.NONE){ // Check if the toppiest position of the column is empty.
                //System.out.println("success!");
                return true;
            }else{
                //Exception handling: The column is full.
                System.err.println("The column is full!");
                return false;
            }           
        }else{
            //Exception handling: Pieces not set in the board.
            System.err.println("Out of the range!");
            return false;
        }
    }

    public void printBoard(){  // Print the whole board with boundaries and column name.
        for(int i = 0; i<row; i++){
            System.out.print("|"); // Set the boundary of the left of lane.
            for(int j = 0; j<column; j++){
                System.out.print(myBoard[i][j].toString()); // Print the content for each block.
                System.out.print("|"); // Set the boundary of the right of lane.
            }
            System.out.println();
        }
        System.out.println("  1   2   3   4   5   6   7"); // set the column numbers.
    }

    public Board(){  //Construct a board, default pieces should be none.
        for(int i = 0; i<row; i++){
            for(int j = 0; j<column; j++){
                myBoard[i][j] = Piece.NONE;
            }
        }
    }
}
