import React from 'react';
import clsx from 'clsx';
import Layout from '@theme/Layout';
import Link from '@docusaurus/Link';
import useDocusaurusContext from '@docusaurus/useDocusaurusContext';
import useBaseUrl from '@docusaurus/useBaseUrl';
import styles from './styles.module.css';
import Translate, {translate} from '@docusaurus/Translate';

const features = [
  {
    title: 'Easy to use',
    imageUrl: 'img/undraw_docusaurus_mountain.svg',
    description: (
      <>
        HoaryFox は簡単に建築構造向け BIM データ "ST-Bridge" を Grasshopper で扱えるように設計しています。
      </>
    ),
  },
  {
    title: 'Focus on improvement of efficiency',
    imageUrl: 'img/undraw_docusaurus_tree.svg',
    description: (
      <>
        Grasshopper の可視化や形状最適化機能を使った構造設計の効率化を目指しています。
      </>
    ),
  },
  {
    title: 'Enhance Karamba3D',
    imageUrl: 'img/undraw_docusaurus_react.svg',
    description: (
      <>
        Grasshopper の構造解析ツール Karamba3D への ST-Bridge データの入出力をサポートします。
      </>
    ),
  },
];

function Feature({imageUrl, title, description}) {
  const imgUrl = useBaseUrl(imageUrl);
  return (
    <div className={clsx('col col--4', styles.feature)}>
      {imgUrl && (
        <div className="text--center">
          <img className={styles.featureImage} src={imgUrl} alt={title} />
        </div>
      )}
      <h3>{title}</h3>
      <p>{description}</p>
    </div>
  );
}

function Home() {
  const context = useDocusaurusContext();
  const {siteConfig = {}} = context;
  return (
    <Layout
      title={`Hello from ${siteConfig.title}`}
      description="HoaryFoxは、ST-Bridgeモデルの可視化やKaramba3DへのST-Bridgeファイルの入出力をサポートします。">
      <header className={clsx('hero hero--primary', styles.heroBanner)}>
        <div className="container">
          <h1 className="hero__title">{siteConfig.title}</h1>
          <p className="hero__subtitle">{siteConfig.tagline}</p>
          <div className={styles.buttons}>
            <Link
              className={clsx(
                'button button--outline button--secondary button--lg',
                styles.getStarted,
              )}
              to={useBaseUrl('docs/Usage/HowToInstall')}>
              Get Started
            </Link>
          </div>
        </div>
      </header>
      <main>
        {features && features.length > 0 && (
          <section className={styles.features}>
            <div className="container">
              <div className="row">
                {features.map((props, idx) => (
                  <Feature key={idx} {...props} />
                ))}
              </div>
            </div>
          </section>
        )}
      </main>
    </Layout>
  );
}

export default Home;
