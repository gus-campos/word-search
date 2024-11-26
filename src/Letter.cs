
class Letter {

    /*
    Represents a WordSearch table letter
    */

    // Properties

    private char character;
    private IntDuple position;

    // Constructor

    public Letter(char character, IntDuple position) {

        this.character = character;
        this.position = position;
    }

    // Public methods - Getters

    public char GetCharacter() {
        return this.character;
    }

    public IntDuple GetPosition() {
        return this.position;
    }
}