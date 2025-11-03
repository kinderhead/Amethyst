import Link from '@docusaurus/Link';
import useDocusaurusContext from '@docusaurus/useDocusaurusContext';
import Heading from '@theme/Heading';
import Layout from '@theme/Layout';
import clsx from 'clsx';
import type { ReactNode } from 'react';

import Home from '@site/src/components/Home';
import styles from './index.module.css';

function HomepageHeader() {
    const {siteConfig} = useDocusaurusContext();
    return (
        <header className={clsx('hero', styles.heroBanner, styles.heroDark)}>
            <div className={clsx("container", styles.headerContainer)}>
                {/* <img src={require("@site/static/img/logo.png").default} className={styles.logo} /> */}
                <div>
                    <Heading as="h1" className="hero__title">
                        {siteConfig.title}
                    </Heading>
                    <p className="hero__subtitle">{siteConfig.tagline}</p>
                    <div className={styles.buttons}>
                        <Link className="button button--secondary button--lg" to="/docs/tutorial/installing/">
                            Download
                        </Link>
                    </div>
                </div>
            </div>
        </header>
    );
}

export default function Index(): ReactNode {
    const {siteConfig} = useDocusaurusContext();
    return (
        <Layout title={siteConfig.title} description={siteConfig.tagline}>
            <HomepageHeader />
            <main>
                <Home/>
            </main>
        </Layout>
    );
}
