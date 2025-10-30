import Heading from '@theme/Heading';
import styles from './home.module.css';

export default function CLI() {
    return (
        <div style={{ marginTop: "4rem", paddingTop: "3rem", paddingBottom: "3rem", display: "flex", alignItems: "center", flexDirection: "column", backgroundColor: "var(--ifm-background-surface-color)", width: "100%" }}>
            <Heading className={styles.heading} as="h1">Quickly build and test projects all in the command line</Heading>
            <pre className={styles.terminalBubble}>
                {`$ amethyst build file.ame -o out.zip --run
Hello from Minecraft!`}
            </pre>
        </div>
    );
}