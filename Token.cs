public record Token(TokenType type, string lexeme, object literal, int line) {
    public override string ToString()
    {
        return type.ToString() + " " + lexeme + " " + literal;
    }
}