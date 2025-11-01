import Link from '@docusaurus/Link';
import Heading from '@theme/Heading';
import styles from './home.module.css';

export default function Start() {
    return (
        <div style={{ paddingTop: "3rem", paddingBottom: "3rem", display: "flex", alignItems: "center", flexDirection: "column", width: "100%" }}>
            <Heading className={styles.heading} as="h1">Get started</Heading>
            <div className={styles.buttonGroup}>
                <Link className="button button--success button--lg" to="/docs/tutorial/installing/">Tutorial</Link>
                <Link className="button button--primary button--lg" to="/docs/category/language-features/">Handbook</Link>
            </div>
        </div>
    );
}