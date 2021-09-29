module.exports = {
  Sidebar: [
    'License',
    {
      type: 'category',
      label: 'Getting Start',
      collapsed: false,
      items: [
        'Usage/HowToInstall',
        'Usage/ShowSTBModel',
        'Usage/ConvertToKaramba',
        'Usage/ExportSTB',
        'Usage/BakeGeometry',
      ]
    },
    {
      type: 'category',
      label: 'Component Info',
      items: [
        'Component/Geometry',
        'Component/IO',
        'Component/NameTag',
        'Component/SectionTag',
        'Component/StbBuilder',
      ]
    },
    'Changelog',
  ],
};
