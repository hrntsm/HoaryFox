---
id: ConvertToKaramba
title: Convert to Karamba3D
---

This section explains how to create a Karamba3D model using the STB data. Please refer to the Convert_to_Karamba.gh model in the Samples folder for the content here.

## Data Conversion

Make sure that Grasshopper can read the STB data as described in Show ST-Bridge model.
The data will be converted by entering the imported results into the Convert to Karamba component as shown below.

The elements and sections will be converted. The converted data can be used as input for Karamba3d's Assemble Model (Karamba3D) component, so you only need to set the Load and Support settings to run the analysis in Karamba3d.
The material information is included in the CrossSection output, so there is no need to set it separately.

The family name of the cross section can be set arbitrarily by the SetFamilyName component for each shape as shown below. If not set, the family name will start with "HF-", such as "HF-Box".

![](../../images/ConvertToKaramba/gh.png)

## Conversion specifications

The following specifications are used to convert the data from STB to Karamba3D.

### Conversion target

- Convert columns, beams and braces, but not face materials such as floor walls

### Element name

- For simplicity, the IDs in STB are converted as names. (e.g. "Id15").
- Therefore, the member code will be lost.

### Cross-sectional shape

- Even if a member has multiple cross sections or tapers, it will be converted as a member with only the center section.
- RC Column
  - Rectangular cross section: Convert as Trapezoid
  - Circular cross section: Since there is no circular solid cross section in Karamba3D, convert to match the axial cross section in 0-Section.
- RC Beams
  - All converted as Trapezoid
- S section
  - H-section: Converted as I-Section
  - T-section: Converted as I-Section
  - Flat bars: Converted as Trapezoid
  - Box section: Converted as []-Section
  - L-section, C-section, round steel: Converted as square Trapezoid to match the axial cross-sectional area, since Karamba3D does not support these cross-sections and they are basically used as braces.
    - Since back-to-back and double bracing are not supported, the cross section will be converted as a single section even if it is set as such in STB.

### Material

- RC
  - StbSecColumn_RC and StbSecBeam_RC are converted as RC.
  - The material can be specified in STB, but it is a string of material names and not necessarily in a specific format, so it is assumed that all materials have FC21 equivalent material properties.
    - Young's modulus: 21860 N/mm2
    - Poisson's ratio: 0.2
    - Density: 24 kN/m3
    - Karamba3d fy: 14 N/mm2
- S
  - For cross sections other than the above, steel material is given.
  - For the same reason as the material specification for RC, all of them are assumed to have material properties equivalent to SN400.
    - Young's modulus: 205000 N/mm2
    - Poisson's ratio: 0.3
    - Density: 78.5 kN/m3
    - Karamba3d fy: 235 N/mm2

:::note

If you have any requests for unsupported functions, please contact us.

:::
