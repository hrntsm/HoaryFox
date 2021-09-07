module.exports = {
  title: 'HoaryFox',
  tagline: 'Grasshopper component for handling ST-Bridge data',
  url: 'https://hiron.dev',
  baseUrl: '/HoaryFox/',
  onBrokenLinks: 'throw',
  onBrokenMarkdownLinks: 'warn',
  favicon: 'img/HFicon.png',
  organizationName: 'hrntsm', // Usually your GitHub org/user name.
  projectName: 'HoaryFox', // Usually your repo name.
  i18n: {
    defaultLocale: 'ja',
    locales: ['ja', 'en'],
    localeConfigs: {
      ja: {
        label: '日本語',
      },
      en: {
        label: 'English',
      }
    }
  },
  themeConfig: {
    hideableSidebar: true,
    image: `img/HFicon.png`,
    metadates: [
      {
        name: `twitter:card`,
        content: `summary`
      },
      {
        name: `og:image`,
        content: `img/HFicon.png`
      }
    ],
    navbar: {
      title: 'HoaryFox',
      logo: {
        alt: 'My Site Logo',
        src: 'img/HFicon.png',
      },
      items: [
        {
          to: 'docs/Usage/HowToInstall',
          activeBasePath: 'docs/Usage/HowToInstall',
          label: 'Docs',
          position: 'left',
        },
        {
          to: 'blog',
          label: 'Blog',
          position: 'left'
        },
        {
          href: 'https://github.com/hrntsm/HoaryFox/',
          label: 'GitHub',
          position: 'right',
        },
        {
          href: 'https://hiron.dev/about/',
          label: 'AboutMe',
          position: 'right'
        },
        {
          to: 'docs/contact',
          label: 'Contact',
          position: 'right'
        },
        {
          type: 'localeDropdown',
          position: 'right',
        }
      ],
    },
    footer: {
      style: 'dark',
      links: [
        {
          title: 'Docs',
          items: [
            {
              label: 'License',
              to: 'docs/License',
            },
            {
              label: 'Component',
              to: 'docs/Component/Geometry',
            },
            {
              label: 'Usage',
              to: 'docs/Usage/ShowSTBModel',
            },
          ],
        },
        {
          title: 'Community',
          items: [
            {
              label: 'Food4Rhino',
              href: 'https://www.food4rhino.com/app/hoaryfox',
            },
            {
              label: 'Twitter',
              href: 'https://twitter.com/hiron_rgkr',
            },
          ],
        },
        {
          title: 'More',
          items: [
            {
              label: 'Blog',
              to: 'blog',
            },
            {
              label: 'Donation',
              to: 'docs/donation',
            },
          ],
        },
      ],
      copyright: `Copyright © ${new Date().getFullYear()} hrntsm, Inc. Built with Docusaurus.`,
    },
  },
  presets: [
    [
      '@docusaurus/preset-classic',
      {
        docs: {
          sidebarPath: require.resolve('./sidebars.js'),
          editUrl:
            'https://github.com/hrntsm/HoaryFox/edit/develop/website/',
        },
        blog: {
          showReadingTime: true,
          editUrl:
            'https://github.com/hrntsm/HoaryFox/edit/develop/website/',
        },
        theme: {
          customCss: require.resolve('./src/css/custom.css'),
        },
      },
    ],
  ],
};
