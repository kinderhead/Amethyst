grammar Amethyst;

// Parser

root
    : (namespace | function | struct | (initAssignmentStatement Semi) | Semi)* EOF
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
    : Inline
    ;

block
    : LBrak statement* RBrak
    ;

struct
    : Struct id (Implements type)? LBrak (declaration | method)* RBrak
    ;

declaration
    : type RawIdentifier (Eq expression)? Semi+
    ;

method
    : methodModifier* type RawIdentifier paramList block
    | functionModifier* RawIdentifier paramList (Colon expression)? block
    ;

methodModifier
    : functionModifier | Virtual
    ;

statement
    : (
        initAssignmentStatement
        | expressionStatement
        | returnStatement
    ) Semi+
    | commandStatement
    | block
    | executeStatement
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

executeStatement
    : executeSubcommand+ statement (Else statement)?
    ;

executeSubcommand
    : (If | As | At) LParen expression RParen
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
    | LParen type RParen castExpression
    ;

unaryExpression
    : (PlusPlus | MinusMinus | Not | Minus | And | WeakRef | Star)* postfixExpression
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
    | Number
    | listLiteral
    | compoundLiteral
    | targetSelector
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

targetSelector
    : TargetSelectorVariable (LSquareBrak (targetSelectorArgument (Comma targetSelectorArgument)*)? RSquareBrak)?
    ;

targetSelectorArgument
    : RawIdentifier Eq expression
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
As: 'as';
At: 'at';
Else: 'else';
For: 'for';
Return: 'return';

Struct: 'struct';
Implements: 'implements';

Macro: 'macro';
Inline: 'inline';
Virtual: 'virtual';

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
TargetSelectorVariable: '@' [praesn];
Number: '-'? (([0-9]+ [bsilBSIL]?) | ([0-9] [0-9.]* [fdFD]?));

Whitespace: (' '|'\t'|'\n'|'\r')+ -> skip;
Comment: '/*' .*? '*/' -> skip;
LineComment: '//' ~[\n\r]* -> skip;
