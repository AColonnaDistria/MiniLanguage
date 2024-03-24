public class RuntimeError : Exception {
    public Token token;

    public RuntimeError(Token token, String message) : base(message) {
        this.token = token;
    }
}