import CLI from './cli';
import styles from './home.module.css';
import Intro from './intro';
import Start from './start';

export default function Home() {
    return (
        <div className={styles.main}>
            <Intro/>
            <CLI/>
            <Start/>
        </div>
    );
}