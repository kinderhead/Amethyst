#!/usr/bin/bash

mkdir tmp

inkscape=org.inkscape.Inkscape

$inkscape -w 16 -h 16 -o tmp/16.png icon.svg
$inkscape -w 32 -h 32 -o tmp/32.png icon.svg
$inkscape -w 48 -h 48 -o tmp/48.png icon.svg

convert tmp/16.png tmp/32.png tmp/48.png icon.ico
cp icon.ico docs/static/img/icon.ico

rm -rf tmp
