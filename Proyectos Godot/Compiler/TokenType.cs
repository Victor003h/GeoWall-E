using System;
using System.Collections.Generic;
using System.IO;

namespace Godot;
public enum TokenType
    {


        //  TOKENS
        NumberToken,
        PlusToken,
        MinusToken,
        MultiplicationToken,
        DivisionToken,
        ExponentToken,
        RestoToken,
        EqualEqualToken,
        AndOperatorToken,
        OrOperatorToken,
        NotOperatorToken,
        NotEqualToken,
        AssignmentToken,
        OpenParenthesesToken,
        CloseParenthesesToken,
        ReservedWordToken,
        ComparisonToken,
        SemiColonToken,
        ColonToken,
        StringToken,
        IdentifierToken,
        ArrobaToken,
        ReservedFunctionToken,
        AssignmentFunctionToken,
        EndLine,
        EndFile,


        BoolExpression,
        BinaryExpression,
        StringExpression,
        FunctionExpression,
        PrintExpression,
        Identifier,


        Function,
        FunctionCall,
        FunctionCallExpression,
        IdentifierArgs,
        NumberExpression,
        Error,
        ParenthesesExpression,
        Point,
        OpenKeyToken,
        CloseKeyToken,
        PointExpression,
        Line,
        Segment,
        Ray,
        Circunference,
        Arc,
        Measure,
        Color,
        Restore,
        Figure,
        UnderscoreToken,
        Sequence,
        DrawFunction,
        PuntoSequenceToken,
        WhiteSpaceToken,
        BadToken,
        NextLineToken,
        UndefinedToken,
        UndefinedExpression,
        NextLine2Token,
        MultipleIdentifiersExpression,

    }