import Heading from '@theme/Heading';
import styles from './home.module.css';
import Link from '@docusaurus/Link';

export default function Start() {
    return (
        <div style={{ paddingTop: "3rem", paddingBottom: "3rem", display: "flex", alignItems: "center", flexDirection: "column", width: "100%" }}>
            <Heading className={styles.heading} as="h1">Get started</Heading>
            <div>
                <Link className="button button--primary button--lg" to="/docs/handbook">Handbook</Link>
            </div>
        </div>
    );
}