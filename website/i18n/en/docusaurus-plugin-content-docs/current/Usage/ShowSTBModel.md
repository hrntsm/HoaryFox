---
id: ShowSTBModel
title: Show ST-Bridge model
---

This section explains how to load and display the STB model in Grasshopper. Please refer to the model in ShowStbModel.gh in the Samples folder for this content.

## Load the STB file

When you open the file, it should look like this: Right-click on the path component and select Select one existing file, then select the stb file you want to visualize.  
If there are no errors in loading, the Load STB data component next to it will convert the data. Connect this output to the visualization components to visualize the information in the Rhino viewport.

![](../../images/ShowStbModel/input.png)

## Visualization

### Display geometry.

[Geometry](../Component/Geometry) category components.  
Use the **"Stb to Line"** component if you want to display the frame data as a line, or the **"Stb to Brep"** component if you want to represent it in Brep, including the size.

### Display Name tag.

[NameTag](../Component/NameTag) category of components.  
There are visualization components for columns, spacers, beams, beams, and braces, so you can connect the output of the Load STB data component to the component you want to visualize. The size of the sign can be changed by entering the Size.

### Display section tag.

[SectionTag](../Component/SectionTag) category of components.  
There are visualization components for columns, spans, beams, beams, and braces, so connect the output of the Load STB data component to the component you want to visualize. The size of the sign can be changed by entering the Size.  
The cross-sectional size of the part will be output as text on the viewport.
