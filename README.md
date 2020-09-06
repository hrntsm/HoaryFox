# HoaryFox

Grasshopper Component which read ST-Bridge file(.stb) and display its model information.  
You can easily check frames and present structure frame!!  
If you need stand-alone ST-Bridge file viewer, Please see [STEVIA](https://github.com/hrntsm/STEVIA-Stb2U/wiki)  
As a test function, it creates an analytical model of Karamba from st-bridge data.

## Install

1. Download HoaryFox.gha file from [food4rhino](https://www.food4rhino.com/app/hoaryfox) or [release page](https://github.com/hrntsm/HoaryFox/releases)
2. In Grasshopper, choose File > Special Folders > Components folder. Save the gha file there.  
   + If you want to use Karamba convert, it is super recommended to save the file in the same directory as Karamba.gha
3. Right-click the file > Properties > make sure there is no "blocked" text
4. Restart Rhino and Grasshopper

## How to use

Input st-bridge file path, output some its tag data(StbColumn, StbGirder, StbPost, StbBeam, StbBrace, StbSlab).

It supports version 1.x, version 2.x is not supported.  
If you need more information, send direct message to my twitter account.

Please refer to Samples directory files.

## Convert to karamba

Conversion of data into Karamba supports only beam elements.  
Mesh elements such as floor and wall are not supported.  
L-type and C-type cross-sections are replaced by rectangular cross-sections with equivalent axial cross-sections because Karamba does not support them.

## What is ST-Bridge

Quote from [building SMART Japan Structural Design Subcommittee](https://en.building-smart.or.jp/meeting/buildall/structural-design/) doing making specifications of ST-Bridge.

> ST Bridge is the standardized format for data sharing in Japan’s structural engineering industry.
> + Simpler to use than IFC due to the clearly defined the range of use
> + Integrate Japanese original methods of drawing methodology (Grids, part placement and section annotations, reinforcement information)
> + Aim for coordination between domestic structural applications, building skeleton surveying applications, 3D Object CAD

## Contact information

+ Twitter : [@hiron_rgkr](https://twitter.com/hiron_rgkr)
+ URL : [https://rgkr-memo.blogspot.com/](https://rgkr-memo.blogspot.com/)
