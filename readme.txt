Othello Game in C#
Introduction
This is a console-based implementation of the classic board game Othello (also known as Reversi) written in C#. The game is played on an 8x8 grid, where two players take turns placing their discs on the board with the goal of capturing the opponent's discs.

Game Rules
The game is played on an 8x8 grid.
Two players take turns, one using black discs and the other using white discs.
Players must place their discs in such a way that captures the opponent's discs.
A disc is captured when it is sandwiched between two discs of the opposing color.
A player must make a move that captures at least one of the opponent's discs.
If a player cannot make a valid move, their turn is skipped.
The game ends when there are no more valid moves or the board is completely filled.
The player with the most discs of their color on the board at the end of the game wins.
Prerequisites
To run this game, you need to have the following installed on your system:

.NET Core SDK (version 3.1 or higher)
Getting Started
Clone this repository to your local machine or download the source code as a ZIP file.
Navigate to the project directory using the command line.
Build the project by running the following command:
Copy code
dotnet build
Run the game with the following command:
arduino
Copy code
dotnet run
Game Controls
Players take turns entering their moves by specifying the row and column of the desired position (e.g., "3 4" for the third row and fourth column).
Valid moves are indicated by an asterisk (*) on the board.
You can quit the game at any time by entering "quit" or "exit".
Implementation Details
The game logic is implemented in the OthelloGame class, which manages the state of the board, validates moves, and checks for game over conditions.
The Player class represents a player and holds their color and score.
The Board class represents the game board and provides methods for placing and flipping discs.
The Program class handles the console input/output and coordinates the game flow.
Future Enhancements
Here are some possible enhancements that could be made to the game:

Implement a graphical user interface (GUI) using a framework like Windows Forms or WPF.
Add an AI opponent with different difficulty levels using algorithms such as minimax or alpha-beta pruning.
Implement an online multiplayer mode to play against other human players over a network.
Allow customization of the board size and starting configuration.
License
This project is licensed under the MIT License.

Acknowledgments
The Othello game logic and rules were adapted from the official Othello/Reversi rules by the Othello Association.
This project was created for educational purposes and is not affiliated with or endorsed by the Othello Association.



