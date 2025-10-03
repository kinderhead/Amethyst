grammar Amethyst;

// Parser

root
    : (namespace | function | interface | (initAssignmentStatement Semi) | Semi)* EOF
    ;

namespace
    : Namespace id Semi
    ;

function
    : functionTag* functionModifier* type name=id paramList block
    ;

functionTag
    : Hash id
    ;

functionModifier
    : NoStack | Inline
    ;

block
    : LBrak statement* RBrak
    ;

interface
    : Interface id LBrak declaration* RBrak
    ;

declaration
    : type RawIdentifier Semi+
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
    | forStatement
    ;

initAssignmentStatement
    : type id (Eq expression)?
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

forStatement
    : For LParen initAssignmentStatement? Semi cond=expression Semi it=expression RParen statement
    ;

returnStatement
    : Return (expression)?
    ;

expression
    : assignmentExpression
    ;

assignmentExpression
    : logicalExpression ((Eq | PlusEq | MinusEq | StarEq | SlashEq) expression)?
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
    : castExpression ((Star | Slash) castExpression)*
    ;

castExpression
    : unaryExpression
    ;

unaryExpression
    : (PlusPlus | MinusMinus | Not | Minus)* postfixExpression
    ;

postfixExpression
    : primaryExpression (
        expressionList | indexExpression | propertyExpression
    )*
    ;

indexExpression
    : LSquareBrak expression RSquareBrak
    ;

propertyExpression
    : Dot RawIdentifier
    ;

primaryExpression
    : id
    | String
    | Integer
    | listLiteral
    | compoundLiteral
    | LParen expression RParen
    ;

listLiteral
    : LSquareBrak (expression (Comma expression)*)? RSquareBrak
    ;

compoundLiteral
    : LBrak (compoundKeyPair (Comma compoundKeyPair)*)? RBrak
    ;

compoundKeyPair
    : RawIdentifier Colon expression
    ;

paramList
    : LParen (paramPair (Comma paramPair)*)? RParen
    ;

paramPair
    : (paramModifier)* type id
    ;

paramModifier
    : Macro
    ;

expressionList
    : LParen (expression (Comma expression)*)? RParen
    ;

type
    : id
    | type LSquareBrak RSquareBrak
    | type And
    | type WeakRef
    ;

id
    : RawIdentifier (Colon RawIdentifier)?
    ;

// Lexer

Namespace: 'namespace';
If: 'if';
Else: 'else';
For: 'for';
Return: 'return';

Interface: 'interface';

Macro: 'macro';
NoStack: 'nostack';
Inline: 'inline';

Semi: ';';
Colon: ':';
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
Not: '!';
PlusEq: '+=';
MinusEq: '-=';
StarEq: '*=';
SlashEq: '/=';
Hash: '#';
EqEq: '==';
Neq: '!=';
Gt: '>';
Gte: '>=';
Lt: '<';
Lte: '<=';
And: '&';
AndAnd: '&&';
OrOr: '||';
PlusPlus: '++';
MinusMinus: '--';
Dot: '.';
WeakRef: '^';

RawIdentifier: ([a-z] | [A-Z] | '_' ) ([a-z] | [A-Z] | [0-9] | '_' | '-' | '/')*;
//Identifier: ([a-z] | [A-Z] | [0-9] | '_' | '-')* ':' ([a-z] | [A-Z] | [0-9] | '_' | '-' | '/')*;
String: '"' ( ~[\\"\n\r] | '\\' [\\"] )* '"';
Command: '@/' ( ~[\n\r] )* ('\r' | '\n');
Integer: '-'? (([0-9]+ [bsilBSIL]?) | ([0-9] [0-9.]* [fdFD]?));

Whitespace: (' '|'\t'|'\n'|'\r')+ -> skip;
Comment: '/*' .*? '*/' -> skip;
LineComment: '//' ~[\n\r]* -> skip;
