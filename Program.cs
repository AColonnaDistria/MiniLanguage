public class Program {
    private static Interpreter interpreter = new Interpreter();
    static bool hadError = false;
    static bool hadRuntimeError = false;

    public static void Main(string[] args)
    {
        if (args.Length > 1) {
            Console.WriteLine("Usage: mini [script]");
            System.Environment.Exit(64);
        }
        else if (args.Length == 1) {
            runFile(args[0]);
        }
        else {
            runPrompt();
        }
    }

    private static void runFile(String path) 
    {
        run(File.ReadAllText(path));
        if (hadError) System.Environment.Exit(65);
        if (hadRuntimeError) System.Environment.Exit(70);
    }

    private static void runPrompt() 
    {
        while (true) {
            Console.Write("> ");

            string line = Console.ReadLine();
            if (line == null) break;
            run(line);
        }
    }


    private static void run(string source)
    {
        Lexer lexer = new Lexer(source);
        List<Token> tokens = lexer.scanTokens();

        Parser parser = new Parser(tokens);
        List<Stmt> statements = parser.parse();

        // Stop if there was a syntax error.
        if (hadError) {
            hadError = false;
            return;
        }
        interpreter.interpret(statements);
    }

    public static void runtimeError(RuntimeError error) {
        Console.Error.WriteLine(error.Message + "\n[line " + error.token.line + "]");
        hadRuntimeError = true;
    }

    public static void error(Token token, String message) 
    {
        if (token.type == TokenType.EOF) {
            report(token.line, " at end", message);
        } 
        else {
            report(token.line, " at '" + token.lexeme + "'", message);
        }
    }

    public static void error(int line, string message) {
        report(line, "", message);
    }

    private static void report(int line, string where, string message) {
        Console.Error.WriteLine("[line " + line + "] Error" + where + ": " + message);
        hadError = true;
    }
}