public class Parser {
    public class ParseError : Exception {
        public Token token;

        public ParseError(Token token, String message) : base(message) {
            this.token = token;
        }
    }

    private List<Token> tokens;
    private int current = 0;

    public Parser(List<Token> tokens)
    {
        this.tokens = tokens;
    }

    private bool match(params TokenType[] types) 
    {
        foreach (TokenType type in types) {
            if (check(type)) {
                advance();
                return true;
            }
        }

        return false;
    }
    public List<Stmt> parse() 
    {
        List<Stmt> statements = new List<Stmt>();

        while (!isAtEnd()){
            statements.Add(declaration());
        }

        return statements;
    }

    private Stmt declaration() {
        try {
            if (peekNext().type == TokenType.EQUAL) {
                return varDeclaration();
            }
            return statement();
        } catch (ParseError error) {
            synchronize();
            return null;
        }
    }

    private void synchronize() {
        advance();

        while (!isAtEnd()) {
            if (previous().line < peek().line) return;

            switch (peek().type) {
                case TokenType.PRINT: return;
            }

            advance();
        }
    }

    private Stmt varDeclaration() {
        Token name = consume(TokenType.IDENTIFIER, "Expect variable name.");

        Expr initializer = null;
        if (match(TokenType.EQUAL)) {
            initializer = expression();
        }

        return new Stmt.Var(name, initializer);
    }

    private Stmt statement()
    {
        if (match(TokenType.PRINT)) 
            return printStatement();

        return expressionStatement();
    }

    private Stmt expressionStatement()
    {
        Expr expr = expression();
        return new Stmt.Expression(expr);
    }

    private Stmt printStatement()
    {
        Expr value = expression();
        return new Stmt.Print(value);
    }

    public Expr expression() 
    {
        return _or();
    }

    private Expr _or()
    {
        Expr expr = _and();

        while (match(TokenType.OR)) {
            Token op = previous();
            Expr right = _and();

            expr = new Expr.Logical(expr, op, right);
        }

        return expr;
    }

    private Expr _and()
    {
        Expr expr = comparison();

        while (match(TokenType.AND)) {
            Token op = previous();
            Expr right = _and();

            expr = new Expr.Logical(expr, op, right);
        }

        return expr;
    }

    private Expr comparison() {
        Expr expr = term();

        while (match(TokenType.GREATER, TokenType.GREATER_EQUAL, TokenType.LESS, TokenType.LESS_EQUAL, TokenType.EQUAL_EQUAL, TokenType.BANG_EQUAL)) {
            Token op = previous();
            Expr right = term();
            expr = new Expr.Binary(expr, op, right);
        }

        return expr;
    }

    private Expr term() {
        Expr expr = factor();

        while (match(TokenType.MINUS, TokenType.PLUS)) {
            Token op = previous();
            Expr right = factor();
            expr = new Expr.Binary(expr, op, right);
        }

        return expr;
    }

    private Expr factor() {
        Expr expr = unary();

        while (match(TokenType.DIV, TokenType.MULT)) {
            Token op = previous();
            Expr right = unary();
            expr = new Expr.Binary(expr, op, right);
        }

        return expr;
    }

    private Expr unary() {
        if (match(TokenType.BANG, TokenType.MINUS)) {
            Token op = previous();
            Expr right = unary();
            return new Expr.Unary(op, right);
        }

        return primary();
    }

    private Expr primary() {
        if (match(TokenType.NUMBER)) {
            return new Expr.Literal(previous().literal);
        }

        if (match(TokenType.IDENTIFIER)) {
            return new Expr.Variable(previous());
        }

        if (match(TokenType.LPAREN)) {
            Expr expr = expression();
            consume(TokenType.RPAREN, "Expect ')' after expression.");
            return new Expr.Grouping(expr);
        }

        throw error(peek(), "Expect expression.");
    }

    private bool isAtEnd() {
        return peek().type == TokenType.EOF;
    }

    private Token peek() {
        return tokens[current];
    }

    private Token peekNext() {
        if (current + 1 >= tokens.Count) {
            return tokens[tokens.Count];
        }
        return tokens[current + 1];
    }

    private Token previous() {
        return tokens[current - 1];
    }

    private Token advance() {
        if (!isAtEnd()) current++;
        return previous();
    }

    private bool check(TokenType type) {
        if (isAtEnd()) return false;
        return peek().type == type;
    }

    private Token consume(TokenType type, string message) {
        if (check(type)) return advance();

        throw error(peek(), message);
    }

    private ParseError error(Token token, String message) {
        Program.error(token, message);
        return new ParseError(token, message);
    }
}