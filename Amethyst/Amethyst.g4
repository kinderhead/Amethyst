grammar Amethyst;

// Parser

root
    : (namespace | function)* EOF
    ;

namespace
    : Namespace Identifier Semi
    ;

function
    : type Identifier paramList block
    ;

block
    : LBrak statement* RBrak
    ;

statement
    : (
        initAssignmentStatement
        | block
    ) Semi+
    ;

initAssignmentStatement
    : type Identifier Eq expression
    ;

expression
    : primaryExpression
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

Identifier: ([a-z] | [A-Z]) ([a-z] | [A-Z] | [0-9] | '_')*;
String: '"' ( ~[\\"\n\r] | '\\' [\\"] )* '"';
Integer: [0-9]+;

Whitespace: (' '|'\t'|'\n'|'\r')+ -> skip;
