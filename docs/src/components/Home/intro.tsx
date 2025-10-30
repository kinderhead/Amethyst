import Heading from '@theme/Heading';
import HighlightCode from '../highlight';
import styles from './home.module.css';

export default function Intro() {
    return (
        <>
            <Heading className={styles.heading} as="h1">Create powerful data packs with ease</Heading>
            <div className={styles.codeHolder}>
                <HighlightCode className={styles.extraCodeBubble} code={`int default_height = 6;

void move-to(int x, int z) {
    @/tp @s $(x) $(default_height + 2) $(z)
}`} />
                <HighlightCode className={styles.codeBubble} code={`#load
void main() {
    var entity = "sheep";
    var targets = @e[type=entity,limit=10];
    var arr = [1, 2, 3, 4];

    if (sum(arr) > 3) as (targets) {
        move-to(7, 7);
    }
}`} />
                <HighlightCode className={styles.extraCodeBubble} code={`int sum(int[] arr) {
    int sum = 0;

    for (int i = 0; i < arr.size(); ++i) {
        sum += arr[i];
    }

    return sum;
}`} />
            </div>
        </>
    );
}