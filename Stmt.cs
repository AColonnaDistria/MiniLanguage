abstract public record Stmt {
    public interface Visitor<R> {        
        public R visitExpressionStmt(Expression stmt);
        public R visitVarStmt(Var stmt);
        public R visitPrintStmt(Print stmt);
    }
    abstract public R accept<R>(Visitor<R> visitor);

    public record Print(Expr expression) : Stmt {
        override public R accept<R>(Visitor<R> visitor)
        {
            return visitor.visitPrintStmt(this);
        }
    }

    public record Expression(Expr expr) : Stmt {

        override public R accept<R>(Visitor<R> visitor)
        {
            return visitor.visitExpressionStmt(this);
        }
    }
    public record Var(Token name, Expr initializer) : Stmt {
        override public R accept<R>(Visitor<R> visitor)
        {
            return visitor.visitVarStmt(this);
        }
    }
}