abstract public record Expr {
    public interface Visitor<R> {
        public R visitBinaryExpr(Binary expr);            
        public R visitUnaryExpr(Unary expr);
        public R visitVariableExpr(Variable expr);
        public R visitGroupingExpr(Grouping expr);           
        public R visitLiteralExpr(Literal expr);      
        public R visitLogicalExpr(Logical expr);
    }

    abstract public R accept<R>(Visitor<R> visitor);

    public record Logical(Expr left, Token op, Expr right) : Expr {
        override public R accept<R>(Visitor<R> visitor)
        {
            return visitor.visitLogicalExpr(this);
        }
    }

    public record Binary(Expr left, Token op, Expr right) : Expr {
        override public R accept<R>(Visitor<R> visitor)
        {
            return visitor.visitBinaryExpr(this);
        }
    }

    public record Unary(Token op, Expr right) : Expr {

        override public R accept<R>(Visitor<R> visitor)
        {
            return visitor.visitUnaryExpr(this);
        }
    }

    public record Variable(Token name) : Expr {

        override public R accept<R>(Visitor<R> visitor)
        {
            return visitor.visitVariableExpr(this);
        }
    }


    public record Grouping(Expr expression) : Expr {
        override public R accept<R>(Visitor<R> visitor)
        {
            return visitor.visitGroupingExpr(this);
        }
    }

    public record Literal(object value) : Expr {
        override public R accept<R>(Visitor<R> visitor)
        {
            return visitor.visitLiteralExpr(this);
        }
    }
}