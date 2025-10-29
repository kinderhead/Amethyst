import { readFileSync } from "fs";
import { createHighlighterCoreSync } from "shiki/core";
import darkPlus from "@shikijs/themes/dark-plus"
import { transformerColorizedBrackets } from '@shikijs/colorized-brackets';
import amethyst from "@site/amethyst.tmLanguage.json";
import { createJavaScriptRegexEngine } from "shiki";

const highlighter = createHighlighterCoreSync({
    langs: [amethyst],
    themes: [darkPlus],
    engine: createJavaScriptRegexEngine()
});

interface HighlightCodeProps extends React.HTMLAttributes<HTMLElement> {
    code: string;
}

export default function HighlightCode({ code, ...props }: HighlightCodeProps) {
    return (
        <div {...props} dangerouslySetInnerHTML={{ __html: highlighter.codeToHtml(code, { lang: "Amethyst", theme: "dark-plus", transformers: [transformerColorizedBrackets()] }) }}></div>
    );
}
