module.exports = {
  title: 'HoaryFox',
  tagline: 'Grasshopper component for handling ST-Bridge data',
  url: 'https://hrntsm.github.io',
  baseUrl: '/HoaryFox/',
  onBrokenLinks: 'throw',
  onBrokenMarkdownLinks: 'warn',
  favicon: 'img/HFicon.png',
  organizationName: 'hrntsm', // Usually your GitHub org/user name.
  projectName: 'HoaryFox', // Usually your repo name.
  themeConfig: {
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
        {to: 'blog', label: 'Blog', position: 'left'},
        {
          href: 'https://github.com/hrntsm/HoaryFox/',
          label: 'GitHub',
          position: 'right',
        },
        {
          to: 'docs/contact',
          label: 'Contact',
          position: 'right'
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
          // Please change this to your repo.
          editUrl:
            'https://github.com/facebook/docusaurus/edit/master/website/',
        },
        blog: {
          showReadingTime: true,
          // Please change this to your repo.
          editUrl:
            'https://github.com/facebook/docusaurus/edit/master/website/blog/',
        },
        theme: {
          customCss: require.resolve('./src/css/custom.css'),
        },
      },
    ],
  ],
};
