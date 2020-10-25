# HoaryFox

![Platform](https://img.shields.io/badge/platform-Rhino6%20%7C%20Grasshopper-orange)
[![Codacy Badge](https://app.codacy.com/project/badge/Grade/c0a462728dce4983802d447ed67d3e7c)](https://www.codacy.com/gh/hrntsm/HoaryFox/dashboard?utm_source=github.com&amp;utm_medium=referral&amp;utm_content=hrntsm/HoaryFox&amp;utm_campaign=Badge_Grade)
[![Release](https://img.shields.io/github/v/release/hrntsm/HoaryFox)](https://github.com/hrntsm/HoaryFox/releases)
[![License](https://img.shields.io/github/license/hrntsm/HoaryFox)](https://github.com/hrntsm/HoaryFox/blob/master/LICENSE)

Grasshopper Component which read ST-Bridge file(.stb) and display its model information.  
You can easily check frames and present structure frame!!  
If you need stand-alone ST-Bridge file viewer, Please see [STEVIA](https://github.com/hrntsm/STEVIA-Stb2U/wiki)  
As a experimental function, it creates an analytical model of Karamba from st-bridge data.

## Install

1. Download HoaryFox.gha file from [food4rhino](https://www.food4rhino.com/app/hoaryfox) or [release page](https://github.com/hrntsm/HoaryFox/releases)
2. In Grasshopper, choose File > Special Folders > Components folder. Save the gha file there.  
   + If you want to use Karamba convert, it is super recommended to save the file in the same directory as Karamba.gha
3. Right-click the file > Properties > make sure there is no "blocked" text
4. Restart Rhino and Grasshopper

## Usage

Input st-bridge file path, output some its tag data(StbColumn, StbGirder, StbPost, StbBeam, StbBrace, StbSlab).

Support for ST-bridge ver2.0 is now available from HoaryFox ver1.1.  
If you need more information, send direct message to my twitter account.

Please refer to Samples directory files.

## Convert to karamba

Conversion of data into Karamba supports only beam elements.  
Mesh elements such as floor and wall are not supported.  
L-type and C-type cross-sections are replaced by rectangular cross-sections with equivalent axial cross-sections because Karamba does not support them.
If you want to use Karamba convert, it is super recommended to save the file in the same directory as Karamba.gha

## What is ST-Bridge

Quote from [building SMART Japan Structural Design Subcommittee](https://en.building-smart.or.jp/meeting/buildall/structural-design/) doing making specifications of ST-Bridge.

> ST Bridge is the standardized format for data sharing in Japan’s structural engineering industry.
> + Simpler to use than IFC due to the clearly defined the range of use
> + Integrate Japanese original methods of drawing methodology (Grids, part placement and section annotations, reinforcement information)
> + Aim for coordination between domestic structural applications, building skeleton surveying applications, 3D Object CAD

## Contact information

[![Twitter](https://img.shields.io/twitter/follow/hiron_rgkr?style=social)](https://twitter.com/hiron_rgkr)
+ HP : [https://hrntsm.github.io/](https://hrntsm.github.io/)
+ Blog : [https://rgkr-memo.blogspot.com/](https://rgkr-memo.blogspot.com/)
+ Mail : support(at)hrntsm.com
  + change (at) to @
  
## License

HoaryFox is licensed under the [MIT](https://github.com/hrntsm/HoaryFox/blob/master/LICENSE) license.  
Copyright© 2019-2020, hrntsm
