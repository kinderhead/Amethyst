grammar Amethyst;

// Parser

root
    : (namespace | function)* EOF
    ;

namespace
    : Namespace Identifier Semi
    ;

function
    : (Hash Identifier)* type name=Identifier paramList block
    ;

block
    : LBrak statement* RBrak
    ;

statement
    : (
        initAssignmentStatement
        | block
        | expressionStatement
    ) Semi+
    | commandStatement
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

expression
    : assignmentExpression
    ;

assignmentExpression
    : additiveExpression
    | Identifier Eq expression
    ;

additiveExpression
    : primaryExpression ((Plus | Minus) primaryExpression)*
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
    : LParen ((type Identifier) (Comma type Identifier)*)? RParen
    ;

expressionList
    : LParen ((Identifier) (Comma Identifier)*)? RParen
    ;

type
    : Identifier
    ;

// Lexer

Namespace: 'namespace';
Semi: ';';
Comma: ',';
LParen: '(';
RParen: ')';
LBrak: '{';
RBrak: '}';
Eq: '=';
Plus: '+';
Minus: '-';
Hash: '#';

Identifier: ([a-z] | [A-Z]) ([a-z] | [A-Z] | [0-9] | '_' | '-')*;
String: '"' ( ~[\\"\n\r] | '\\' [\\"] )* '"';
Command: '/' ( ~[\n\r] )* ('\r' | '\n');
Integer: [0-9]+;

Whitespace: (' '|'\t'|'\n'|'\r')+ -> skip;
Comment: '/*' .*? '*/' -> skip;
LineComment: '//' ~[\n\r]* -> skip;
