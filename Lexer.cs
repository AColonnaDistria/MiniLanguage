
using System.Collections.Generic;

public class Lexer {
    private string source;
    public List<Token> tokens = new List<Token>();
    private int start = 0;
    private int current = 0;
    private int line = 1;
    private static Dictionary<String, TokenType> keywords = new Dictionary<String, TokenType>
    {
        {"print",  TokenType.PRINT},
        {"and",    TokenType.AND},
        {"or",     TokenType.OR},
    };

    public Lexer(string source) {
        this.source = source;
    }

    public List<Token> scanTokens()
    {
        while (!isAtEnd()) {
            start = current;
            scanToken();
        }

        tokens.Add(new Token(TokenType.EOF, "", null, line));
        return tokens;
    }

    private void scanToken()
    {
        char c = advance();

        switch (c) {
            case '(': addToken(TokenType.LPAREN); break;
            case ')': addToken(TokenType.RPAREN); break;
            case '+': addToken(TokenType.PLUS); break;
            case '*': addToken(TokenType.MULT); break;
            case '-': 
                if (match('>')) {
                    addToken(TokenType.RARROW);
                }
                else {
                    addToken(TokenType.MINUS);
                }
                break;
            case '/': addToken(TokenType.DIV); break;
            case '!':
                addToken(match('=') ? TokenType.BANG_EQUAL : TokenType.BANG);
                break;
            case '=':
                addToken(match('=') ? TokenType.EQUAL_EQUAL : TokenType.EQUAL);
                break;
            case '<':
                addToken(match('=') ? TokenType.LESS_EQUAL : TokenType.LESS);
                break;
            case '>':
                addToken(match('=') ? TokenType.GREATER_EQUAL : TokenType.GREATER);
                break;
            case '&':
                addToken(match('&') ? TokenType.AND : TokenType.AMPERSAND);
                break;
            case '|':
                addToken(match('|') ? TokenType.OR : TokenType.BAR);
                break;
            case ':':
                addToken(match(':') ? TokenType.COLON_COLON : TokenType.COLON);
                break;
            case ' ':
            case '\r':
            case '\t':
                // Ignore whitespace.
                break;
            case '\n':
                line++;
                break;
            default:        
                if (isDigit(c)) {
                    number();
                } 
                else if (isAlpha(c)) {
                    identifier();
                }
                else {
                    //error
                    Program.error(line, "Unexpected character.");
                }
                break;
        }
    }

    private void number() 
    {
        while (isDigit(peek())) advance();

        // Look for a fractional part.
        if (peek() == '.' && isDigit(peekNext())) {
            // Consume the "."
            advance();

            while (isDigit(peek())) advance();
        }
        
        addToken(TokenType.NUMBER, Double.Parse(source.Substring(start, current - start)));
    }

    private void identifier() 
    {
        while (isAlphaNumeric(peek())) advance();

        string text = source.Substring(start, current - start);
        TokenType type;
        if (!keywords.TryGetValue(text, out type)) {
            type = TokenType.IDENTIFIER;
        }

        addToken(type);
    }

    private bool isAlpha(char c) 
    {
        return (c >= 'a' && c <= 'z') ||
                (c >= 'A' && c <= 'Z') ||
                c == '_';
    }

    private bool isAlphaNumeric(char c) 
    {
        return isAlpha(c) || isDigit(c);
    }

    private bool isDigit(char c) 
    {
        return c >= '0' && c <= '9';
    } 

    private bool match(char expected) {
        if (isAtEnd()) return false;
        if (source[current] != expected) return false;

        current++;
        return true;
    }

    private char peek() {
        if (isAtEnd()) return '\0';
        return source[current];
    }

    private char peekNext() {
        if (current + 1 >= source.Length) return '\0';
        return source[current + 1];
    } 

    private char advance()
    {
        return source[current++];
    }

    private void addToken(TokenType type)
    {
        addToken(type, null);
    }

    private void addToken(TokenType type, Object literal) {
        string text = source.Substring(start, current - start);
        tokens.Add(new Token(type, text, literal, line));
    }

    private bool isAtEnd()
    {
        return current >= source.Length;
    }
}