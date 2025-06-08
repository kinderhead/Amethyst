grammar Amethyst;

// Parser

root
    : (namespace | function)* EOF
    ;

namespace
    : Namespace Identifier Semi
    ;

function
    : functionTag* functionModifier* type name=Identifier paramList block
    ;

functionTag
    : Hash Identifier
    ;

functionModifier
    : NoStack
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
        expressionList | indexExpression
    )*
    ;

indexExpression
    : LSquareBrak expression RSquareBrak
    ;

primaryExpression
    : Identifier
    | String
    | Integer
    | listLiteral
    | LParen expression RParen
    ;

listLiteral
    : LSquareBrak (expression (Comma expression)*)? RSquareBrak
    ;

paramList
    : LParen (paramPair (Comma paramPair)*)? RParen
    ;

paramPair
    : (paramModifier)* type Identifier
    ;

paramModifier
    : Macro
    ;

expressionList
    : LParen (expression (Comma expression)*)? RParen
    ;

type
    : Identifier
    | type LSquareBrak RSquareBrak
    ;

// Lexer

Namespace: 'namespace';
If: 'if';
Else: 'else';
Return: 'return';

Macro: 'macro';
NoStack: 'nostack';

Semi: ';';
Comma: ',';
LParen: '(';
RParen: ')';
LSquareBrak: '[';
RSquareBrak: ']';
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

Identifier: ([a-z] | [A-Z] | '_' ) ([a-z] | [A-Z] | [0-9] | '_' | '-' | ':' | '/')*;
String: '"' ( ~[\\"\n\r] | '\\' [\\"] )* '"';
Command: '@/' ( ~[\n\r] )* ('\r' | '\n');
Integer: '-'? [0-9]+;

Whitespace: (' '|'\t'|'\n'|'\r')+ -> skip;
Comment: '/*' .*? '*/' -> skip;
LineComment: '//' ~[\n\r]* -> skip;
