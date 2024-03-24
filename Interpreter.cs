public class Interpreter : Expr.Visitor<object>, Stmt.Visitor<Interpreter._Void> {
    private Dictionary<string, object> globals = new Dictionary<string, object>();

    class _Void {
    };

    public Interpreter() {
    }

    public void interpret(List<Stmt> statements) { 
        try {
            foreach (Stmt statement in statements) {
                execute(statement);
            }
        } catch (RuntimeError error) {
            Program.runtimeError(error);
        }
    }

    public void execute(Stmt stmt)
    {
        stmt.accept(this);
    }
    public object evaluate(Expr expr)
    {
        return expr.accept(this);
    }

    private bool isEqual(object a, object b) {
        if (a == null && b == null) return true;
        if (a == null) return false;

        return a.Equals(b);
    }
    private bool isTruthy(object Object) {
        if (Object == null) return false;
        if (Object.GetType() == typeof(bool)) return (bool)Object;
        return true;
    }

    _Void Stmt.Visitor<_Void>.visitPrintStmt(Stmt.Print stmt)
    {
        object value = evaluate(stmt.expression);
        Console.WriteLine(value.ToString());
        return null;
    }
    _Void Stmt.Visitor<_Void>.visitVarStmt(Stmt.Var stmt)
    {
        object value = null;
        if (stmt.initializer != null) {
            value = evaluate(stmt.initializer);
        }

        globals.Add(stmt.name.lexeme, value);
        //environment.define(stmt.name.lexeme, value);
        return null;
    }
    _Void Stmt.Visitor<_Void>.visitExpressionStmt(Stmt.Expression stmt)
    {
        evaluate(stmt.expr);
        return null;
    }

    object Expr.Visitor<object>.visitVariableExpr(Expr.Variable expr)
    {
        object? variableValue;
        if (!globals.TryGetValue(expr.name.lexeme, out variableValue))
            throw new RuntimeError(expr.name, "Undefined variable '" + expr.name.lexeme + "'.");

        return variableValue;
    }

    object Expr.Visitor<object>.visitLogicalExpr(Expr.Logical expr) {
        object left = evaluate(expr.left);

        if (expr.op.type == TokenType.OR) {
            if (isTruthy(left)) return left;
        }
        else {
            if (!isTruthy(left)) return left;
        }

        return evaluate(expr.right);
    }

    private void checkNumberOperands(Token op, object left, object right) {
        if (left.GetType() == typeof(double) && right.GetType() == typeof(double)) return;

        throw new RuntimeError(op, "Operands must be numbers.");
    }

    private void checkNumberOperand(Token op, object left) {
        if (left.GetType() == typeof(double)) return;

        throw new RuntimeError(op, "Operands must be numbers.");
    }

    object Expr.Visitor<object>.visitBinaryExpr(Expr.Binary expr) {
        object left = evaluate(expr.left);
        object right = evaluate(expr.right); 

        switch (expr.op.type) {
            case TokenType.GREATER:
                checkNumberOperands(expr.op, left, right);
                return (double)left > (double)right;
            case TokenType.GREATER_EQUAL:
                checkNumberOperands(expr.op, left, right);
                return (double)left >= (double)right;
            case TokenType.LESS:
                checkNumberOperands(expr.op, left, right);
                return (double)left < (double)right;
            case TokenType.LESS_EQUAL:
                checkNumberOperands(expr.op, left, right);
                return (double)left <= (double)right;
            case TokenType.BANG_EQUAL: 
                return !isEqual(left, right);
            case TokenType.EQUAL_EQUAL: 
                return isEqual(left, right);
            case TokenType.MINUS:
                checkNumberOperands(expr.op, left, right);
                return (double)left - (double)right;
            case TokenType.DIV:
                checkNumberOperands(expr.op, left, right);
                return (double)left / (double)right;
            case TokenType.MULT:
                checkNumberOperands(expr.op, left, right);
                return (double)left * (double)right;
            case TokenType.PLUS:
                checkNumberOperands(expr.op, left, right);
                return (double)left + (double)right;
        }

        // Unreachable.
        return null;
    }

    
    object Expr.Visitor<object>.visitUnaryExpr(Expr.Unary expr) 
    {
        object right = evaluate(expr.right);

        switch (expr.op.type) {
            case TokenType.MINUS:        
                checkNumberOperand(expr.op, right);
                return -(double)right;
            case TokenType.BANG:        
                return !isTruthy(right);
        }

        return null;
    }

    object Expr.Visitor<object>.visitLiteralExpr(Expr.Literal expr)
    {
        return expr.value;
    }
    object Expr.Visitor<object>.visitGroupingExpr(Expr.Grouping expr)
    {
        return evaluate(expr.expression);
    }
}