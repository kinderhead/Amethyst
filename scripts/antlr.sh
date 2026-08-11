#!/usr/bin/bash

cd "$(dirname "$0")"
cd ../Amethyst

antlr4 -no-listener -visitor -package Amethyst.Antlr -Dlanguage=CSharp -o Antlr Amethyst.g4
