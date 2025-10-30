import { transformerColorizedBrackets } from '@shikijs/colorized-brackets';
import darkPlus from "@shikijs/themes/dark-plus";
import amethyst from "@site/amethyst.tmLanguage.json";
import React from 'react';
import { createJavaScriptRegexEngine } from "shiki";
import { createHighlighterCoreSync } from "shiki/core";

const highlighter = createHighlighterCoreSync({
    langs: [amethyst],
    themes: [darkPlus],
    engine: createJavaScriptRegexEngine()
});

interface HighlightCodeProps extends React.HTMLAttributes<HTMLElement> {
    code: string;
}

export default function HighlightCode({ code, ...props }: HighlightCodeProps) {
    var lines = code.split("\n");
    
    if (lines.length > 0) {
        var spaces = 0;

        for (let i = 0; i < lines[0].length; i++) {
            if (lines[0][i] !== " ") {
                break;
            }

            spaces = i;
        }

        for (let i = 0; i < lines.length; i++) {
            if (lines[i].substring(0, spaces + 1).trim() === "") {
                lines[i] = lines[i].substring(spaces + 1);
            }
        }
    }

    code = lines.join("\n");

    return (
        <div {...props} dangerouslySetInnerHTML={{ __html: highlighter.codeToHtml(code, { lang: "Amethyst", theme: "dark-plus", transformers: [transformerColorizedBrackets()] }) }}></div>
    );
}
