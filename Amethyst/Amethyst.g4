grammar Amethyst;

// Parser

root
    : (namespace | function)* EOF
    ;

namespace
    : Namespace Identifier Semi
    ;

function
    : functionTag* type name=Identifier paramList block
    ;

functionTag
    : Hash Identifier
    ;

block
    : LBrak statement* RBrak
    ;

statement
    : (
        initAssignmentStatement
        | expressionStatement
        | returnStatement
    ) Semi+
    | commandStatement
    | block
    | ifStatement
    ;

initAssignmentStatement
    : type Identifier Eq expression
    ;

expressionStatement
    : expression
    ;

commandStatement
    : Command
    ;

ifStatement
    : If LParen expression RParen statement (Else statement)?
    ;

returnStatement
    : Return (expression)?
    ;

expression
    : assignmentExpression
    ;

assignmentExpression
    : logicalExpression
    | Identifier Eq expression
    ;

logicalExpression
    : equalityExpression ((AndAnd | OrOr) equalityExpression)*
    ;

equalityExpression
    : relationalExpression ((EqEq | Neq) relationalExpression)*
    ;

relationalExpression
    : additiveExpression ((Gt | Gte | Lt | Lte) additiveExpression)*
    ;

additiveExpression
    : multiplicativeExpression ((Plus | Minus) multiplicativeExpression)*
    ;

multiplicativeExpression
    : postfixExpression ((Star | Slash) postfixExpression)*
    ;

postfixExpression
    : primaryExpression (
        expressionList
    )*
    ;

primaryExpression
    : Identifier
    | String
    | Integer
    | LParen expression RParen
    ;

paramList
    : LParen (paramPair (Comma paramPair)*)? RParen
    ;

paramPair
    : type Identifier
    ;

expressionList
    : LParen (expression (Comma expression)*)? RParen
    ;

type
    : Identifier
    ;

// Lexer

Namespace: 'namespace';
If: 'if';
Else: 'else';
Return: 'return';

Semi: ';';
Comma: ',';
LParen: '(';
RParen: ')';
LBrak: '{';
RBrak: '}';
Eq: '=';
Plus: '+';
Minus: '-';
Star: '*';
Slash: '/';
Hash: '#';
EqEq: '==';
Neq: '!=';
Gt: '>';
Gte: '>=';
Lt: '<';
Lte: '<=';
AndAnd: '&&';
OrOr: '||';

Identifier: ([a-z] | [A-Z]) ([a-z] | [A-Z] | [0-9] | '_' | '-' | ':')*;
String: '"' ( ~[\\"\n\r] | '\\' [\\"] )* '"';
Command: '@/' ( ~[\n\r] )* ('\r' | '\n');
Integer: '-'? [0-9]+;

Whitespace: (' '|'\t'|'\n'|'\r')+ -> skip;
Comment: '/*' .*? '*/' -> skip;
LineComment: '//' ~[\n\r]* -> skip;
