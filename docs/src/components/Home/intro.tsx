import HighlightCode from '../highlight';
import styles from './home.module.css';

export default function Intro() {
    return (
        <HighlightCode className={styles.codeBubble} code={`#load
void main() {
    var entity = "sheep";
    var targets = @e[type=entity];

    as (targets) at (@r) {
        @/say This site is WIP!
    }
}`} />
    );
}