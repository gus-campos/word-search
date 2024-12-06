
class Letter {

    /*
    Represents a WordSearch table letter
    */

    // Properties

    private char character;
    private Coord position;

    // Constructor

    public Letter(char character, Coord position) {

        this.character = character;
        this.position = position;
    }

    // Public methods - Getters

    public char GetCharacter() {
        return this.character;
    }

    public Coord GetPosition() {
        return this.position;
    }
}