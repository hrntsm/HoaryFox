---
id: Changelog
title: Changelog
---

---

## [v1.3.1 - 2021-03-28](https://github.com/hrntsm/HoaryFox/releases/tag/v1.3.1)

### ADD

- Bake function for Brep and Line

### UPDATE

- Brep to be exported by Stb2Brep, merged into a single Brep

---

## [v1.2.2 - 2021-02-17](https://github.com/hrntsm/HoaryFox/releases/tag/v1.2.2)

### ADD

- Create a documentation site
- SetCroSecFamilyName component to specify the family name of the cross-section

### FIX

- Fixed wrong units in material properties

### UPDATE

- RC cross-section names have been changed from "Id + number" to BD- or CD- to indicate the cross-section size.
  - For example, BD-300x600.
- The FrameBuilder using Karamba3D component has been separated into two parts: frame conversion and nodal conversion.
  - Framebuilder by angle and NodeBuilderFrameBuilder 
  - Allowed input of the angle for determining the column and beam when converting
  - Added text output to Rhino viewport to show results of the above decisions

---

## [v1.2.1 - 2020-12-31](https://github.com/hrntsm/HoaryFox/releases/tag/v1.2.1)

### FIX

- Change the karamba3D reference path, since the builder component wouldn't load to GH.

---

## [v1.2.0 - 2020-12-30](https://github.com/hrntsm/HoaryFox/releases/tag/v1.2.0)

### ADD

- ST-Bridge export feature.

### その他

- The part that handles STB was made independent as STBDotNet.
  - The reading part is still the same as the previous STBReader and will be updated in the next fix.

---

## [v0.9.0 - v1.1.3](https://github.com/hrntsm/HoaryFox/releases)

Please check the GitHub release page for the above versions.
