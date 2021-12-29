---
id: Filter
title: Filter
---

Categories of components for filtering data.

---

## MaterialType

![](../../images/Component/MaterialType.png)

Create a category for each material.

|Input|Explanation|
|---|:---:|
|Data|Load STB file Input the Data output of the component|

|Output|Explanation|
|---|:---:|
|Columns|Material information for columns|
|Girders|Material information for girders|
|Posts|Material information for posts|
|Beams|Material information for beams|
|Braces|Material information for braces|
|Slabs|Material information for slabs|
|Walls|Material information for walls|

---

## Story

![](../../images/Component/Story.png)

Creating Categories for Each Floor  
The floors to which columns, walls, and braces belong are output as the floors to which the upper nodes belong among the nodes to which they belong.

|Input|Explanation|
|---|:---:|
|Data|Load STB file Input the Data output of the component|

|Output|Explanation|
|---|:---:|
|Columns|Floor information of the columns|
|Girders|Floor information of the girders|
|Posts|Floor information of the posts|
|Beams|Floor information of the beams|
|Braces|Floor information of the braces|
|Slabs|Floor information of the slabs|
|Walls|Floor information of the walls|

---

## Filter by Materials

![](../../images/Component/FilterByMaterials.png)

Filter the brep created by Stb to Brep by material and floor information.
The floor information is implemented in the branch.

|Input|Explanation|
|---|:---:|
|Geometry|Brep created with Stb to Brep|
|Material|Material information created by MaterialType|
|Story|Floor information created by Story|

|Output|Explanation|
|---|:---:|
|RC|Brep with RC material|
|S|Brep with S material|
|SRC|Brep with SRC material|
|CFT|Brep with CFT material|
