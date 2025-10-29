import styles from './home.module.css';
import Intro from './intro';

export default function Home() {
    return (
        <div className={styles.main}>
            <Intro/>
        </div>
    );
}