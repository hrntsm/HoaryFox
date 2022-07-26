"use strict";(self.webpackChunkhoaryfox_website=self.webpackChunkhoaryfox_website||[]).push([[7934],{3905:(e,t,a)=>{a.d(t,{Zo:()=>c,kt:()=>h});var n=a(7294);function i(e,t,a){return t in e?Object.defineProperty(e,t,{value:a,enumerable:!0,configurable:!0,writable:!0}):e[t]=a,e}function o(e,t){var a=Object.keys(e);if(Object.getOwnPropertySymbols){var n=Object.getOwnPropertySymbols(e);t&&(n=n.filter((function(t){return Object.getOwnPropertyDescriptor(e,t).enumerable}))),a.push.apply(a,n)}return a}function r(e){for(var t=1;t<arguments.length;t++){var a=null!=arguments[t]?arguments[t]:{};t%2?o(Object(a),!0).forEach((function(t){i(e,t,a[t])})):Object.getOwnPropertyDescriptors?Object.defineProperties(e,Object.getOwnPropertyDescriptors(a)):o(Object(a)).forEach((function(t){Object.defineProperty(e,t,Object.getOwnPropertyDescriptor(a,t))}))}return e}function l(e,t){if(null==e)return{};var a,n,i=function(e,t){if(null==e)return{};var a,n,i={},o=Object.keys(e);for(n=0;n<o.length;n++)a=o[n],t.indexOf(a)>=0||(i[a]=e[a]);return i}(e,t);if(Object.getOwnPropertySymbols){var o=Object.getOwnPropertySymbols(e);for(n=0;n<o.length;n++)a=o[n],t.indexOf(a)>=0||Object.prototype.propertyIsEnumerable.call(e,a)&&(i[a]=e[a])}return i}var s=n.createContext({}),m=function(e){var t=n.useContext(s),a=t;return e&&(a="function"==typeof e?e(t):r(r({},t),e)),a},c=function(e){var t=m(e.components);return n.createElement(s.Provider,{value:t},e.children)},d={inlineCode:"code",wrapper:function(e){var t=e.children;return n.createElement(n.Fragment,{},t)}},p=n.forwardRef((function(e,t){var a=e.components,i=e.mdxType,o=e.originalType,s=e.parentName,c=l(e,["components","mdxType","originalType","parentName"]),p=m(a),h=i,u=p["".concat(s,".").concat(h)]||p[h]||d[h]||o;return a?n.createElement(u,r(r({ref:t},c),{},{components:a})):n.createElement(u,r({ref:t},c))}));function h(e,t){var a=arguments,i=t&&t.mdxType;if("string"==typeof e||i){var o=a.length,r=new Array(o);r[0]=p;var l={};for(var s in t)hasOwnProperty.call(t,s)&&(l[s]=t[s]);l.originalType=e,l.mdxType="string"==typeof e?e:i,r[1]=l;for(var m=2;m<o;m++)r[m]=a[m];return n.createElement.apply(null,r)}return n.createElement.apply(null,a)}p.displayName="MDXCreateElement"},1592:(e,t,a)=>{a.r(t),a.d(t,{assets:()=>s,contentTitle:()=>r,default:()=>d,frontMatter:()=>o,metadata:()=>l,toc:()=>m});var n=a(7462),i=(a(7294),a(3905));const o={id:"ExportSTB",title:"Export ST-Bridge file"},r=void 0,l={unversionedId:"Usage/ExportSTB",id:"Usage/ExportSTB",title:"Export ST-Bridge file",description:"This section explains how to create ST-Bridge data from Karamba3D data. Please refer to the ExportSTB.gh model in the Samples folder for the details.",source:"@site/i18n/en/docusaurus-plugin-content-docs/current/Usage/ExportSTB.md",sourceDirName:"Usage",slug:"/Usage/ExportSTB",permalink:"/HoaryFox/en/docs/Usage/ExportSTB",editUrl:"https://github.com/hrntsm/HoaryFox/edit/develop/website/docs/Usage/ExportSTB.md",tags:[],version:"current",frontMatter:{id:"ExportSTB",title:"Export ST-Bridge file"},sidebar:"Sidebar",previous:{title:"Convert to Karamba3D",permalink:"/HoaryFox/en/docs/Usage/ConvertToKaramba"},next:{title:"Bake Geometry",permalink:"/HoaryFox/en/docs/Usage/BakeGeometry"}},s={},m=[{value:"Convert Karamba3D model",id:"convert-karamba3d-model",level:2},{value:"Creating Axis Information",id:"creating-axis-information",level:2},{value:"Create the floor information",id:"create-the-floor-information",level:2},{value:"Output the data",id:"output-the-data",level:2},{value:"Conversion specifications",id:"conversion-specifications",level:2},{value:"Target",id:"target",level:3},{value:"Part Identification",id:"part-identification",level:3},{value:"Material Identification",id:"material-identification",level:3},{value:"Material name",id:"material-name",level:3},{value:"Cross Section Name",id:"cross-section-name",level:3},{value:"Cross section shape",id:"cross-section-shape",level:3}],c={toc:m};function d(e){let{components:t,...o}=e;return(0,i.kt)("wrapper",(0,n.Z)({},c,o,{components:t,mdxType:"MDXLayout"}),(0,i.kt)("p",null,"This section explains how to create ST-Bridge data from Karamba3D data. Please refer to the ExportSTB.gh model in the Samples folder for the details.",(0,i.kt)("br",{parentName:"p"}),"\n","The output will be in ST-Bridge version 1.4 format."),(0,i.kt)("h2",{id:"convert-karamba3d-model"},"Convert Karamba3D model"),(0,i.kt)("p",null,"From the model created by Karamba3D, the following will be created"),(0,i.kt)("ul",null,(0,i.kt)("li",{parentName:"ul"},"Nodes (StbNodes)"),(0,i.kt)("li",{parentName:"ul"},"Part information (StbMembers)"),(0,i.kt)("li",{parentName:"ul"},"Section information (StbSections)")),(0,i.kt)("p",null,"The model data output from the AssembleModel component of Karamba3D will be converted by inputting the data into the FrameBuilder by angle component and NodeBuilder as follows.",(0,i.kt)("br",{parentName:"p"}),"\n","The Rhino viewport will display text to indicate whether the converted member is a column, beam, or brace. Columns and beams are classified based on the angle of the member, so if this is not the expected classification, enter the appropriate angle in the Angle input."),(0,i.kt)("p",null,(0,i.kt)("img",{loading:"lazy",src:a(3275).Z,width:"1079",height:"346"})),(0,i.kt)("div",{className:"admonition admonition-note alert alert--secondary"},(0,i.kt)("div",{parentName:"div",className:"admonition-heading"},(0,i.kt)("h5",{parentName:"div"},(0,i.kt)("span",{parentName:"h5",className:"admonition-icon"},(0,i.kt)("svg",{parentName:"span",xmlns:"http://www.w3.org/2000/svg",width:"14",height:"16",viewBox:"0 0 14 16"},(0,i.kt)("path",{parentName:"svg",fillRule:"evenodd",d:"M6.3 5.69a.942.942 0 0 1-.28-.7c0-.28.09-.52.28-.7.19-.18.42-.28.7-.28.28 0 .52.09.7.28.18.19.28.42.28.7 0 .28-.09.52-.28.7a1 1 0 0 1-.7.3c-.28 0-.52-.11-.7-.3zM8 7.99c-.02-.25-.11-.48-.31-.69-.2-.19-.42-.3-.69-.31H6c-.27.02-.48.13-.69.31-.2.2-.3.44-.31.69h1v3c.02.27.11.5.31.69.2.2.42.31.69.31h1c.27 0 .48-.11.69-.31.2-.19.3-.42.31-.69H8V7.98v.01zM7 2.3c-3.14 0-5.7 2.54-5.7 5.68 0 3.14 2.56 5.7 5.7 5.7s5.7-2.55 5.7-5.7c0-3.15-2.56-5.69-5.7-5.69v.01zM7 .98c3.86 0 7 3.14 7 7s-3.14 7-7 7-7-3.12-7-7 3.14-7 7-7z"}))),"note")),(0,i.kt)("div",{parentName:"div",className:"admonition-content"},(0,i.kt)("p",{parentName:"div"},"Please see the conversion specification at the bottom of this page for more information on how to model the output from Karamba3D."))),(0,i.kt)("h2",{id:"creating-axis-information"},"Creating Axis Information"),(0,i.kt)("p",null,"The AxisBuilder component is used to create data for the axes (StbAxes).",(0,i.kt)("br",{parentName:"p"}),"\n","The basic idea of the behavior is that nodes that are within the Range width (Dist \xb1 Range) from the axis specified by Distance are treated as nodes belonging to the axis.",(0,i.kt)("br",{parentName:"p"}),"\n","By looking at the output of the component, you can check the direction of the axis, the distance from the origin, and the number of nodes belonging to the axis."),(0,i.kt)("p",null,"The specification of the input is as follows. It is not easy to understand, so it will be improved in the future. The input is a list, and inputs with the same index are matched and converted."),(0,i.kt)("ul",null,(0,i.kt)("li",{parentName:"ul"},"Node: nodal information, please enter the Node from the NodeBuilder output"),(0,i.kt)("li",{parentName:"ul"},"Distance: Specify the distance of the axis from the origin.",(0,i.kt)("ul",{parentName:"li"},(0,i.kt)("li",{parentName:"ul"},"The direction of the axis will be parallel to the X or Y axis of the overall coordinate system."),(0,i.kt)("li",{parentName:"ul"},"The direction of the axis is parallel to the X-axis or Y-axis of the overall coordinate system. Whether the specified Distance is in the X-direction or Y-direction is specified by the Direction input."))),(0,i.kt)("li",{parentName:"ul"},"Range: Specify the width of the nodal point belonging to the axis set in Distance.",(0,i.kt)("ul",{parentName:"li"},(0,i.kt)("li",{parentName:"ul"},"Due to the fact that nodal coordinates are in floating point format, it is recommended to set a value greater than 0, even if all nodes are on the axis."))),(0,i.kt)("li",{parentName:"ul"},"Name: The name of the axis."),(0,i.kt)("li",{parentName:"ul"},"Direction: Specifies the direction of the axis.",(0,i.kt)("ul",{parentName:"li"},(0,i.kt)("li",{parentName:"ul"},"A value of 0 means that the axis will be treated as X-directed, and a value of 1 means that the axis will be treated as Y-directed.")))),(0,i.kt)("p",null,(0,i.kt)("img",{loading:"lazy",src:a(6200).Z,width:"765",height:"512"})),(0,i.kt)("h2",{id:"create-the-floor-information"},"Create the floor information"),(0,i.kt)("p",null,"The StoryBuilder component is used to create data for floors (StbStories).",(0,i.kt)("br",{parentName:"p"}),"\n","The basic behavior is the same as the AxisBuilder component.",(0,i.kt)("br",{parentName:"p"}),"\n","By looking at the output of the component, you can check the name of the floor, the height of the floor, and the number of nodes it belongs to."),(0,i.kt)("ul",null,(0,i.kt)("li",{parentName:"ul"},"Node: Nodal information, enter the Node in the NodeBuilder output."),(0,i.kt)("li",{parentName:"ul"},"Height: Floor height information. Please enter the height from the origin."),(0,i.kt)("li",{parentName:"ul"},"Range: Specify the width of the nodal point for the floor specified by Height."),(0,i.kt)("li",{parentName:"ul"},"Name: This is the name of the floor.")),(0,i.kt)("p",null,(0,i.kt)("img",{loading:"lazy",src:a(1982).Z,width:"860",height:"320"})),(0,i.kt)("h2",{id:"output-the-data"},"Output the data"),(0,i.kt)("p",null,"By inputting all the data converted by the above three into the Export STB file component, the data will be combined to create a single ST-Bridge file.",(0,i.kt)("br",{parentName:"p"}),"\n","The STB file will be output to the path specified in Path. If not specified, the file will be output to the desktop as model.stb.",(0,i.kt)("br",{parentName:"p"}),"\n","If you set the value of Out? to True, the file will be output. By checking the output, you can get an overview of the data conversion status."),(0,i.kt)("p",null,(0,i.kt)("img",{loading:"lazy",src:a(7415).Z,width:"1075",height:"331"})),(0,i.kt)("hr",null),(0,i.kt)("h2",{id:"conversion-specifications"},"Conversion specifications"),(0,i.kt)("p",null,"ST-Bridge data is created according to the following specifications."),(0,i.kt)("h3",{id:"target"},"Target"),(0,i.kt)("ul",null,(0,i.kt)("li",{parentName:"ul"},"Output in ST-Bridge version 1.4 format."),(0,i.kt)("li",{parentName:"ul"},"Convert columns, beams, and braces, but not surface materials such as floor walls.")),(0,i.kt)("div",{className:"admonition admonition-note alert alert--secondary"},(0,i.kt)("div",{parentName:"div",className:"admonition-heading"},(0,i.kt)("h5",{parentName:"div"},(0,i.kt)("span",{parentName:"h5",className:"admonition-icon"},(0,i.kt)("svg",{parentName:"span",xmlns:"http://www.w3.org/2000/svg",width:"14",height:"16",viewBox:"0 0 14 16"},(0,i.kt)("path",{parentName:"svg",fillRule:"evenodd",d:"M6.3 5.69a.942.942 0 0 1-.28-.7c0-.28.09-.52.28-.7.19-.18.42-.28.7-.28.28 0 .52.09.7.28.18.19.28.42.28.7 0 .28-.09.52-.28.7a1 1 0 0 1-.7.3c-.28 0-.52-.11-.7-.3zM8 7.99c-.02-.25-.11-.48-.31-.69-.2-.19-.42-.3-.69-.31H6c-.27.02-.48.13-.69.31-.2.2-.3.44-.31.69h1v3c.02.27.11.5.31.69.2.2.42.31.69.31h1c.27 0 .48-.11.69-.31.2-.19.3-.42.31-.69H8V7.98v.01zM7 2.3c-3.14 0-5.7 2.54-5.7 5.68 0 3.14 2.56 5.7 5.7 5.7s5.7-2.55 5.7-5.7c0-3.15-2.56-5.69-5.7-5.69v.01zM7 .98c3.86 0 7 3.14 7 7s-3.14 7-7 7-7-3.12-7-7 3.14-7 7-7z"}))),"note")),(0,i.kt)("div",{parentName:"div",className:"admonition-content"},(0,i.kt)("p",{parentName:"div"},"Output in ST-Bridge version 2 format will be supported in the future."))),(0,i.kt)("h3",{id:"part-identification"},"Part Identification"),(0,i.kt)("ul",null,(0,i.kt)("li",{parentName:"ul"},"The Karamba3D model does not distinguish between column and beam braces, so the following specifications are used",(0,i.kt)("ul",{parentName:"li"},(0,i.kt)("li",{parentName:"ul"},"If a part is treated as a truss element in Karamba3D, it is converted to a brace."),(0,i.kt)("li",{parentName:"ul"},"If the angle of a member is less than the angle entered in FrameBuilder's Angle to the Z-axis of the overall coordinate system, the member is considered to be a column; if the angle is greater than the angle, the member is considered to be a beam."))),(0,i.kt)("li",{parentName:"ul"},"The identification of the member corresponds to the respective representation in StbMember in ST-Bridge (e.g. StbColumn)."),(0,i.kt)("li",{parentName:"ul"},"STB beam members have a flag (isFoundation) to indicate whether the member is a foundation member, but all are output as False.")),(0,i.kt)("h3",{id:"material-identification"},"Material Identification"),(0,i.kt)("ul",null,(0,i.kt)("li",{parentName:"ul"},'In Karamba3D, if the Family name is "Steel", the material is assumed to be S. If the Family name is "Concrete", the material is assumed to be RC.'),(0,i.kt)("li",{parentName:"ul"},"The material identification corresponds to the respective expression in StbSections in ST-Bridge (e.g. StbSecColumn_RC).")),(0,i.kt)("div",{className:"admonition admonition-important alert alert--info"},(0,i.kt)("div",{parentName:"div",className:"admonition-heading"},(0,i.kt)("h5",{parentName:"div"},(0,i.kt)("span",{parentName:"h5",className:"admonition-icon"},(0,i.kt)("svg",{parentName:"span",xmlns:"http://www.w3.org/2000/svg",width:"14",height:"16",viewBox:"0 0 14 16"},(0,i.kt)("path",{parentName:"svg",fillRule:"evenodd",d:"M7 2.3c3.14 0 5.7 2.56 5.7 5.7s-2.56 5.7-5.7 5.7A5.71 5.71 0 0 1 1.3 8c0-3.14 2.56-5.7 5.7-5.7zM7 1C3.14 1 0 4.14 0 8s3.14 7 7 7 7-3.14 7-7-3.14-7-7-7zm1 3H6v5h2V4zm0 6H6v2h2v-2z"}))),"important")),(0,i.kt)("div",{parentName:"div",className:"admonition-content"},(0,i.kt)("p",{parentName:"div"},"If the family name of the material is other than the above, an error occurs."))),(0,i.kt)("h3",{id:"material-name"},"Material name"),(0,i.kt)("ul",null,(0,i.kt)("li",{parentName:"ul"},"Do not use Karamba3D's own names."),(0,i.kt)("li",{parentName:"ul"},"Karamba3D has an internal order of material names, which is a combination of numbers: C for columns, G for beams, V for braces. (C1, G1, etc.)")),(0,i.kt)("h3",{id:"cross-section-name"},"Cross Section Name"),(0,i.kt)("ul",null,(0,i.kt)("li",{parentName:"ul"},"Create a cross section with the name set in the Name field of the Cross Section component in Karamba3D.")),(0,i.kt)("div",{className:"admonition admonition-important alert alert--info"},(0,i.kt)("div",{parentName:"div",className:"admonition-heading"},(0,i.kt)("h5",{parentName:"div"},(0,i.kt)("span",{parentName:"h5",className:"admonition-icon"},(0,i.kt)("svg",{parentName:"span",xmlns:"http://www.w3.org/2000/svg",width:"14",height:"16",viewBox:"0 0 14 16"},(0,i.kt)("path",{parentName:"svg",fillRule:"evenodd",d:"M7 2.3c3.14 0 5.7 2.56 5.7 5.7s-2.56 5.7-5.7 5.7A5.71 5.71 0 0 1 1.3 8c0-3.14 2.56-5.7 5.7-5.7zM7 1C3.14 1 0 4.14 0 8s3.14 7 7 7 7-3.14 7-7-3.14-7-7-7zm1 3H6v5h2V4zm0 6H6v2h2v-2z"}))),"important")),(0,i.kt)("div",{parentName:"div",className:"admonition-content"},(0,i.kt)("p",{parentName:"div"},"Since each name is output to the STB file, please make sure to set the name.",(0,i.kt)("br",{parentName:"p"}),"\n","If there is a duplicate Name, HoaryFox Converter will treat it as the same section, so only one section will be output."))),(0,i.kt)("h3",{id:"cross-section-shape"},"Cross section shape"),(0,i.kt)("ul",null,(0,i.kt)("li",{parentName:"ul"},"The cross section will be output to STB according to the cross section shape set in Karamba3D."),(0,i.kt)("li",{parentName:"ul"},"If there is an error in the conversion, the cross section will be output as a 10 mm square."),(0,i.kt)("li",{parentName:"ul"},"RC cross-section does not have reinforcement information in Karamba3D, so it will be output as follows",(0,i.kt)("ul",{parentName:"li"},(0,i.kt)("li",{parentName:"ul"},"Main reinforcement: 3-D22 at top and bottom for beams, 8-D22 for columns"),(0,i.kt)("li",{parentName:"ul"},"Shear reinforcement: 2-D10@100"))),(0,i.kt)("li",{parentName:"ul"},"For S-section, the material is SN400 for both flange and web.")),(0,i.kt)("div",{className:"admonition admonition-note alert alert--secondary"},(0,i.kt)("div",{parentName:"div",className:"admonition-heading"},(0,i.kt)("h5",{parentName:"div"},(0,i.kt)("span",{parentName:"h5",className:"admonition-icon"},(0,i.kt)("svg",{parentName:"span",xmlns:"http://www.w3.org/2000/svg",width:"14",height:"16",viewBox:"0 0 14 16"},(0,i.kt)("path",{parentName:"svg",fillRule:"evenodd",d:"M6.3 5.69a.942.942 0 0 1-.28-.7c0-.28.09-.52.28-.7.19-.18.42-.28.7-.28.28 0 .52.09.7.28.18.19.28.42.28.7 0 .28-.09.52-.28.7a1 1 0 0 1-.7.3c-.28 0-.52-.11-.7-.3zM8 7.99c-.02-.25-.11-.48-.31-.69-.2-.19-.42-.3-.69-.31H6c-.27.02-.48.13-.69.31-.2.2-.3.44-.31.69h1v3c.02.27.11.5.31.69.2.2.42.31.69.31h1c.27 0 .48-.11.69-.31.2-.19.3-.42.31-.69H8V7.98v.01zM7 2.3c-3.14 0-5.7 2.54-5.7 5.68 0 3.14 2.56 5.7 5.7 5.7s5.7-2.55 5.7-5.7c0-3.15-2.56-5.69-5.7-5.69v.01zM7 .98c3.86 0 7 3.14 7 7s-3.14 7-7 7-7-3.12-7-7 3.14-7 7-7z"}))),"note")),(0,i.kt)("div",{parentName:"div",className:"admonition-content"},(0,i.kt)("p",{parentName:"div"},"If you have any requests for unsupported functions, please contact us."))))}d.isMDXComponent=!0},6200:(e,t,a)=>{a.d(t,{Z:()=>n});const n=a.p+"assets/images/AxisBuilder-029863f8992e1179e1a957654ac72cba.png"},7415:(e,t,a)=>{a.d(t,{Z:()=>n});const n=a.p+"assets/images/ExportStbfile-cffbd25b03fcaccc2cbbafdebed5f643.png"},3275:(e,t,a)=>{a.d(t,{Z:()=>n});const n=a.p+"assets/images/FrameBuilder-1061b496adb0fcdf1f1dcddbcebaf119.png"},1982:(e,t,a)=>{a.d(t,{Z:()=>n});const n=a.p+"assets/images/StoryBuilder-9770c78060c1bb559a9fd049ac234f74.png"}}]);