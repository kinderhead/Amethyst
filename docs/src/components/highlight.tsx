import { readFileSync } from "fs";
import { createHighlighter } from "shiki";
import { transformerColorizedBrackets } from '@shikijs/colorized-brackets';
import amethyst from "../../../AmethystLanguageServer/syntaxes/amethyst.tmLanguage.json";

export const highlighter = await createHighlighter({
    langs: [amethyst],
    themes: ["dark-plus"],
});

export interface HighlightCodeProps extends React.HTMLAttributes<HTMLElement> {
    code: string;
}

export default function HighlightCode({ code, ...props }: HighlightCodeProps) {
    return (
        <div {...props} dangerouslySetInnerHTML={{ __html: highlighter.codeToHtml(code, { lang: "Amethyst", theme: "dark-plus", transformers: [transformerColorizedBrackets()] }) }}></div>
    );
}
