---
id: Changelog
title: Changelog
---

---

## [v2.3.0 - 2022-10-15](https://github.com/hrntsm/HoaryFox/releases/tag/v2.3.0)

### ADD

- If the section setting (StbSection) does not have an appropriate value, an error is now output.

### CHANGE

- The output of the Axes component is now a tree.
  - The output is now divided by floor.
- When a stb2brep component has a hollow section, a hollow section brep is created.
  - Previously, a rectangular solid cross section with the external shape was output.
  - This change improves the accuracy of member volumes used in LCC linkage, etc.

### FIX

- Fixed an error when a part is null.

---

## [v2.2.1 - 2022-3-22](https://github.com/hrntsm/HoaryFox/releases/tag/v2.2.1)

### FIX

- Fixed wall generation failure with v2.2.0 changes.

---

## [v2.2.0 - 2022-3-21](https://github.com/hrntsm/HoaryFox/releases/tag/v2.2.0)

### ADD

- Support Wall opens.

---

## [v2.1.1 - 2022-1-6](https://github.com/hrntsm/HoaryFox/releases/tag/v2.1.1)

### ADD

- Add a sample page for coding with STBDotNet

### CHANGE

- In creating the above sample, a bug was found in STBDotNet, so it was fixed and the version referenced was changed to 0.2.3 after the fix.

---

## [v2.1.0 - 2021-12-29](https://github.com/hrntsm/HoaryFox/releases/tag/v2.1.0)

### ADD

- Added filtering for output to LCA analysis
  - MaterialType
  - Story
  - Filter by Material

### FIX

- Multi-targeted builds, as we found that only matching combinations of versions would work.
- Karamba3D connection error stopped when the cross-sectional shape was null.

### CHANGE

- The method of creating a brep with the Stb to Brep component has been changed.
  - In the previous method, the volume was sometimes negative, so we added a process to check the normal of the brep and flip the face if the volume is negative.
  - Slabs that are not Planer faces are now given a thickness.

---

## [v2.0.1 - 2021-10-10](https://github.com/hrntsm/HoaryFox/releases/tag/v2.0.1)

### ADD

- Added a search bar using Algolia to the website.

### FIX

- The Stb2Brep component to fail to create a Brep when a deck slab and a precast slab are included.
- Fixed an error when converting beams with two cross sections with Stb2Brep.
  - This makes it possible to create a Brep that reflects the one-sided haunch.

---

## [v2.0.0 - 2021-09-17](https://github.com/hrntsm/HoaryFox/releases/tag/v2.0.0)

### CHANGE

- Support for both ST-Bridge v1 and v2 was previously available, but from this version only v2 is available.
- Stb2Brep and Stb2Line components are able to output more information as UserText in Rhino when baked.
- Stb2Brep displays the brep corresponding to the haunch of the member
- The Tag component used to display only representative cross-sections such as the center section, but it displays all cross-section information.
  - In the case of SRC, both RC shape and steel shape are output.
- Wall openings are no longer supported.
- Stb2Brep component outputs slabs and walls as Closed Brep instead of as a single surface.
- In HoaryFox v1, when converting from Karamba3D to ST-Bridge, an overview of the conversion status could be seen in the panel, but this is no longer possible in v2.

### ADD

- Slab and wall tag output is available.
  - Addition of SlabNameTag, WallNameTag, SlabSectionTag, and WallSectionTag
- Added the ability to display axes.
  - Using the Axis component, floor and axis information can be output.
- When converting to Karamba3D, the material in Karamba3D is specified according to the RC material name in ST-Bridge.
  - For example, if the material name is "FC21", the material properties equivalent to FC21 will be input based on the standards of the Architectural Institute of Japan.
  - Young's modulus, shear modulus, and density change according to the strength.
  - Corresponding materials are fc18, 21, 24, 26, 30, 33, 36, 40, 42, 45, 50, 55, 60, and other materials are converted to fc21
  - All steel materials are treated as SN400 because there is no change between materials except for strength.
- The conversion output of Karamba3D data to ST-Bridge has been changed from ST-Bridge v1.4 compliant content to v2.0.2 compliant content.

### INFORMATION

- STBDotNet and STBReader libraries were used to handle ST-Bridge, but they are unified into STBDotNet.
- Use GitHub's CodeQL feature to check code for vulnerabilities.

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
