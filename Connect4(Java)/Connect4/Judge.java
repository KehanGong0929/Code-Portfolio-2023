public class Judge {  // A winning checker.
    
    //private static final int winningNumber = 4; // Will not use the solution of count pieces number so disable this integer.
    private Board board;
    private Player player;

    public Judge(){
        this.board = board;
        this.player = player;
    }

    public boolean checkWin(Board board, Player player){ // If anyone won, it will return true. If no one won, then return false.
        
        // The proach is to traversing the position from where cursor is.
        // check horizontal
        for(int i = 0; i<board.myBoard.length; i++){
			for (int j = 0;j < board.myBoard[0].length - 3; j++){
				if (board.myBoard[i][j] == player.getPiece()   && 
                    board.myBoard[i][j+1] == player.getPiece() &&
                    board.myBoard[i][j+2] == player.getPiece() &&
                    board.myBoard[i][j+3] == player.getPiece()){
					return true;
                }
            }
        }
       // check vertical
       for(int i = 0; i<board.myBoard.length - 3; i++){
        for (int j = 0;j < board.myBoard[0].length; j++){
            if (board.myBoard[i][j] == player.getPiece()   && 
                board.myBoard[i+1][j] == player.getPiece() &&
                board.myBoard[i+2][j] == player.getPiece() &&
                board.myBoard[i+3][j] == player.getPiece()){
                return true;
            }
        }
    }
		//check upward diagonal
		for(int i = 3; i < board.myBoard.length; i++){
			for(int j = 0; j < board.myBoard[0].length - 3; j++){
				if (board.myBoard[i][j] == player.getPiece() && 
                board.myBoard[i-1][j+1] == player.getPiece() &&
                board.myBoard[i-2][j+2] == player.getPiece() &&
                board.myBoard[i-3][j+3] == player.getPiece()){
					return true;
				}
			}
		}
		//check downward diagonal
		for(int i = 0; i < board.myBoard.length - 3; i++){
			for(int j = 0; j < board.myBoard[0].length - 3; j++){
				if (board.myBoard[i][j] == player.getPiece() && 
                board.myBoard[i+1][j+1] == player.getPiece() &&
                board.myBoard[i+2][j+2] == player.getPiece() &&
                board.myBoard[i+3][j+3] == player.getPiece()){
					return true;
				}
			}
		}
		return false;
    }

}
