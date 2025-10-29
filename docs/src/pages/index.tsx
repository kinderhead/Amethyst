import Link from '@docusaurus/Link';
import useDocusaurusContext from '@docusaurus/useDocusaurusContext';
import Heading from '@theme/Heading';
import Layout from '@theme/Layout';
import clsx from 'clsx';
import type { ReactNode } from 'react';

import HighlightCode from '@site/src/components/highlight';
import styles from './index.module.css';

function HomepageHeader() {
    const {siteConfig} = useDocusaurusContext();
    return (
        <header className={clsx('hero', styles.heroBanner, styles.heroDark)}>
            <div className={clsx("container", styles.headerContainer)}>
                <img src={require("@site/static/img/logo.png").default} className={styles.logo} />
                <div>
                    <Heading as="h1" className="hero__title">
                        {siteConfig.title}
                    </Heading>
                    <p className="hero__subtitle">{siteConfig.tagline}</p>
                    <div className={styles.buttons}>
                        <Link className="button button--secondary button--lg" to="https://github.com/kinderhead/Amethyst/releases">
                            Download
                        </Link>
                    </div>
                </div>
            </div>
        </header>
    );
}

export default function Home(): ReactNode {
    const {siteConfig} = useDocusaurusContext();
    return (
        <Layout title={siteConfig.title} description={siteConfig.tagline}>
            <HomepageHeader />
            <main>
                <div className={styles.main}>
                    <HighlightCode className={styles.codeBubble} code={`#load
void main() {
    var entity = "sheep";
    var targets = @e[type=entity];

    as (targets) at (@r) {
        @/say This site is WIP!
    }
}`} />
                </div>
            </main>
        </Layout>
    );
}
