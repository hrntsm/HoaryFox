"use strict";(self.webpackChunkwebsite=self.webpackChunkwebsite||[]).push([[4843],{3905:function(t,e,r){r.d(e,{Zo:function(){return c},kt:function(){return k}});var n=r(7294);function a(t,e,r){return e in t?Object.defineProperty(t,e,{value:r,enumerable:!0,configurable:!0,writable:!0}):t[e]=r,t}function l(t,e){var r=Object.keys(t);if(Object.getOwnPropertySymbols){var n=Object.getOwnPropertySymbols(t);e&&(n=n.filter((function(e){return Object.getOwnPropertyDescriptor(t,e).enumerable}))),r.push.apply(r,n)}return r}function o(t){for(var e=1;e<arguments.length;e++){var r=null!=arguments[e]?arguments[e]:{};e%2?l(Object(r),!0).forEach((function(e){a(t,e,r[e])})):Object.getOwnPropertyDescriptors?Object.defineProperties(t,Object.getOwnPropertyDescriptors(r)):l(Object(r)).forEach((function(e){Object.defineProperty(t,e,Object.getOwnPropertyDescriptor(r,e))}))}return t}function p(t,e){if(null==t)return{};var r,n,a=function(t,e){if(null==t)return{};var r,n,a={},l=Object.keys(t);for(n=0;n<l.length;n++)r=l[n],e.indexOf(r)>=0||(a[r]=t[r]);return a}(t,e);if(Object.getOwnPropertySymbols){var l=Object.getOwnPropertySymbols(t);for(n=0;n<l.length;n++)r=l[n],e.indexOf(r)>=0||Object.prototype.propertyIsEnumerable.call(t,r)&&(a[r]=t[r])}return a}var i=n.createContext({}),m=function(t){var e=n.useContext(i),r=e;return t&&(r="function"==typeof t?t(e):o(o({},e),t)),r},c=function(t){var e=m(t.components);return n.createElement(i.Provider,{value:e},t.children)},d={inlineCode:"code",wrapper:function(t){var e=t.children;return n.createElement(n.Fragment,{},e)}},u=n.forwardRef((function(t,e){var r=t.components,a=t.mdxType,l=t.originalType,i=t.parentName,c=p(t,["components","mdxType","originalType","parentName"]),u=m(r),k=a,b=u["".concat(i,".").concat(k)]||u[k]||d[k]||l;return r?n.createElement(b,o(o({ref:e},c),{},{components:r})):n.createElement(b,o({ref:e},c))}));function k(t,e){var r=arguments,a=e&&e.mdxType;if("string"==typeof t||a){var l=r.length,o=new Array(l);o[0]=u;var p={};for(var i in e)hasOwnProperty.call(e,i)&&(p[i]=e[i]);p.originalType=t,p.mdxType="string"==typeof t?t:a,o[1]=p;for(var m=2;m<l;m++)o[m]=r[m];return n.createElement.apply(null,o)}return n.createElement.apply(null,r)}u.displayName="MDXCreateElement"},1673:function(t,e,r){r.r(e),r.d(e,{frontMatter:function(){return p},contentTitle:function(){return i},metadata:function(){return m},toc:function(){return c},default:function(){return u}});var n=r(7462),a=r(3366),l=(r(7294),r(3905)),o=["components"],p={id:"Geometry",title:"Geometry"},i=void 0,m={unversionedId:"Component/Geometry",id:"Component/Geometry",isDocsHomePage:!1,title:"Geometry",description:"\u8aad\u307f\u8fbc\u3093\u3060 STB \u30c7\u30fc\u30bf\u304b\u3089\u90e8\u6750\u3092\u53ef\u8996\u5316\u3001Bake \u3059\u308b\u30b3\u30f3\u30dd\u30fc\u30cd\u30f3\u30c8\u306e\u30ab\u30c6\u30b4\u30ea",source:"@site/docs/Component/Geometry.md",sourceDirName:"Component",slug:"/Component/Geometry",permalink:"/HoaryFox/docs/Component/Geometry",editUrl:"https://github.com/hrntsm/HoaryFox/edit/develop/website/docs/Component/Geometry.md",tags:[],version:"current",frontMatter:{id:"Geometry",title:"Geometry"},sidebar:"Sidebar",previous:{title:"Bake Geometry",permalink:"/HoaryFox/docs/Usage/BakeGeometry"},next:{title:"IO",permalink:"/HoaryFox/docs/Component/IO"}},c=[{value:"Stb to Line",id:"stb-to-line",children:[]},{value:"Stb to Brep",id:"stb-to-brep",children:[{value:"\u8868\u793a\u4ed5\u69d8",id:"\u8868\u793a\u4ed5\u69d8",children:[]}]}],d={toc:c};function u(t){var e=t.components,p=(0,a.Z)(t,o);return(0,l.kt)("wrapper",(0,n.Z)({},d,p,{components:e,mdxType:"MDXLayout"}),(0,l.kt)("p",null,"\u8aad\u307f\u8fbc\u3093\u3060 STB \u30c7\u30fc\u30bf\u304b\u3089\u90e8\u6750\u3092\u53ef\u8996\u5316\u3001Bake \u3059\u308b\u30b3\u30f3\u30dd\u30fc\u30cd\u30f3\u30c8\u306e\u30ab\u30c6\u30b4\u30ea"),(0,l.kt)("hr",null),(0,l.kt)("h2",{id:"stb-to-line"},"Stb to Line"),(0,l.kt)("p",null,(0,l.kt)("img",{src:r(5352).Z})),(0,l.kt)("p",null,"\u90e8\u6750\u3092 Line \u3067\u8868\u793a\u3059\u308b"),(0,l.kt)("table",null,(0,l.kt)("thead",{parentName:"table"},(0,l.kt)("tr",{parentName:"thead"},(0,l.kt)("th",{parentName:"tr",align:null},"\u5165\u529b"),(0,l.kt)("th",{parentName:"tr",align:"center"},"\u8aac\u660e"))),(0,l.kt)("tbody",{parentName:"table"},(0,l.kt)("tr",{parentName:"tbody"},(0,l.kt)("td",{parentName:"tr",align:null},"Data"),(0,l.kt)("td",{parentName:"tr",align:"center"},"Load STB file \u30b3\u30f3\u30dd\u30fc\u30cd\u30f3\u30c8\u306e Data \u51fa\u529b\u3092\u5165\u529b")),(0,l.kt)("tr",{parentName:"tbody"},(0,l.kt)("td",{parentName:"tr",align:null},"Bake"),(0,l.kt)("td",{parentName:"tr",align:"center"},"\u5404 Line \u3092\u65ad\u9762\u7b26\u53f7\u3054\u3068\u306b\u30ec\u30a4\u30e4\u30fc\u5206\u3051\u3057\u3066 Bake \u3059\u308b")))),(0,l.kt)("table",null,(0,l.kt)("thead",{parentName:"table"},(0,l.kt)("tr",{parentName:"thead"},(0,l.kt)("th",{parentName:"tr",align:null},"\u51fa\u529b"),(0,l.kt)("th",{parentName:"tr",align:"center"},"\u8aac\u660e"))),(0,l.kt)("tbody",{parentName:"table"},(0,l.kt)("tr",{parentName:"tbody"},(0,l.kt)("td",{parentName:"tr",align:null},"Nodes"),(0,l.kt)("td",{parentName:"tr",align:"center"},"\u7bc0\u70b9\u306e Point3d \u306e\u30ea\u30b9\u30c8\u3092\u51fa\u529b")),(0,l.kt)("tr",{parentName:"tbody"},(0,l.kt)("td",{parentName:"tr",align:null},"Columns"),(0,l.kt)("td",{parentName:"tr",align:"center"},"\u67f1\u306e Line \u306e\u30ea\u30b9\u30c8\u3092\u51fa\u529b")),(0,l.kt)("tr",{parentName:"tbody"},(0,l.kt)("td",{parentName:"tr",align:null},"Girders"),(0,l.kt)("td",{parentName:"tr",align:"center"},"\u5927\u6881\u306e Line \u306e\u30ea\u30b9\u30c8\u3092\u51fa\u529b")),(0,l.kt)("tr",{parentName:"tbody"},(0,l.kt)("td",{parentName:"tr",align:null},"Posts"),(0,l.kt)("td",{parentName:"tr",align:"center"},"\u9593\u67f1\u306e Line \u306e\u30ea\u30b9\u30c8\u3092\u51fa\u529b")),(0,l.kt)("tr",{parentName:"tbody"},(0,l.kt)("td",{parentName:"tr",align:null},"Beams"),(0,l.kt)("td",{parentName:"tr",align:"center"},"\u5c0f\u6881\u306e Line \u306e\u30ea\u30b9\u30c8\u3092\u51fa\u529b")),(0,l.kt)("tr",{parentName:"tbody"},(0,l.kt)("td",{parentName:"tr",align:null},"Braces"),(0,l.kt)("td",{parentName:"tr",align:"center"},"\u30d6\u30ec\u30fc\u30b9\u306e Line \u306e\u30ea\u30b9\u30c8\u3092\u51fa\u529b")))),(0,l.kt)("hr",null),(0,l.kt)("h2",{id:"stb-to-brep"},"Stb to Brep"),(0,l.kt)("p",null,(0,l.kt)("img",{src:r(3950).Z})),(0,l.kt)("p",null,"\u90e8\u6750\u3092 Brep \u3067\u8868\u793a\u3059\u308b"),(0,l.kt)("table",null,(0,l.kt)("thead",{parentName:"table"},(0,l.kt)("tr",{parentName:"thead"},(0,l.kt)("th",{parentName:"tr",align:null},"\u5165\u529b"),(0,l.kt)("th",{parentName:"tr",align:"center"},"\u8aac\u660e"))),(0,l.kt)("tbody",{parentName:"table"},(0,l.kt)("tr",{parentName:"tbody"},(0,l.kt)("td",{parentName:"tr",align:null},"Data"),(0,l.kt)("td",{parentName:"tr",align:"center"},"Load STB file \u30b3\u30f3\u30dd\u30fc\u30cd\u30f3\u30c8\u306e Data \u51fa\u529b\u3092\u5165\u529b")),(0,l.kt)("tr",{parentName:"tbody"},(0,l.kt)("td",{parentName:"tr",align:null},"Bake"),(0,l.kt)("td",{parentName:"tr",align:"center"},"\u5404 Brep \u3092\u65ad\u9762\u7b26\u53f7\u3054\u3068\u306b\u30ec\u30a4\u30e4\u30fc\u5206\u3051\u3057\u3066 Bake \u3059\u308b")))),(0,l.kt)("table",null,(0,l.kt)("thead",{parentName:"table"},(0,l.kt)("tr",{parentName:"thead"},(0,l.kt)("th",{parentName:"tr",align:null},"\u51fa\u529b"),(0,l.kt)("th",{parentName:"tr",align:"center"},"\u8aac\u660e"))),(0,l.kt)("tbody",{parentName:"table"},(0,l.kt)("tr",{parentName:"tbody"},(0,l.kt)("td",{parentName:"tr",align:null},"Columns"),(0,l.kt)("td",{parentName:"tr",align:"center"},"\u67f1\u5f62\u72b6\u3092\u8868\u3059 Brep \u306e\u30ea\u30b9\u30c8\u3092\u51fa\u529b")),(0,l.kt)("tr",{parentName:"tbody"},(0,l.kt)("td",{parentName:"tr",align:null},"Girders"),(0,l.kt)("td",{parentName:"tr",align:"center"},"\u5927\u6881\u5f62\u72b6\u3092\u8868\u3059 Brep \u306e\u30ea\u30b9\u30c8\u3092\u51fa\u529b")),(0,l.kt)("tr",{parentName:"tbody"},(0,l.kt)("td",{parentName:"tr",align:null},"Posts"),(0,l.kt)("td",{parentName:"tr",align:"center"},"\u9593\u67f1\u5f62\u72b6\u3092\u8868\u3059 Brep \u306e\u30ea\u30b9\u30c8\u3092\u51fa\u529b")),(0,l.kt)("tr",{parentName:"tbody"},(0,l.kt)("td",{parentName:"tr",align:null},"Beams"),(0,l.kt)("td",{parentName:"tr",align:"center"},"\u5c0f\u6881\u5f62\u72b6\u3092\u8868\u3059 Brep \u306e\u30ea\u30b9\u30c8\u3092\u51fa\u529b")),(0,l.kt)("tr",{parentName:"tbody"},(0,l.kt)("td",{parentName:"tr",align:null},"Braces"),(0,l.kt)("td",{parentName:"tr",align:"center"},"\u30d6\u30ec\u30fc\u30b9\u5f62\u72b6\u3092\u8868\u3059 Brep \u306e\u30ea\u30b9\u30c8\u3092\u51fa\u529b")),(0,l.kt)("tr",{parentName:"tbody"},(0,l.kt)("td",{parentName:"tr",align:null},"Slabs"),(0,l.kt)("td",{parentName:"tr",align:"center"},"\u30b9\u30e9\u30d6\u5f62\u72b6\u3092\u8868\u3059 Brep \u306e\u30ea\u30b9\u30c8\u3092\u51fa\u529b")),(0,l.kt)("tr",{parentName:"tbody"},(0,l.kt)("td",{parentName:"tr",align:null},"Walls"),(0,l.kt)("td",{parentName:"tr",align:"center"},"\u58c1\u5f62\u72b6\u3092\u8868\u3059 Brep \u306e\u30ea\u30b9\u30c8\u3092\u51fa\u529b")))),(0,l.kt)("h3",{id:"\u8868\u793a\u4ed5\u69d8"},"\u8868\u793a\u4ed5\u69d8"),(0,l.kt)("ul",null,(0,l.kt)("li",{parentName:"ul"},"\u58c1\u306f\u958b\u53e3\u3092\u542b\u3081\u3066\u51fa\u529b\u3057\u307e\u3059"),(0,l.kt)("li",{parentName:"ul"},"\u30b9\u30e9\u30d6\u306f\u51f9\u5f62\u72b6\u306e\u5834\u5408\u3046\u307e\u304f\u51fa\u529b\u3055\u308c\u306a\u3044\u3053\u3068\u304c\u3042\u308a\u307e\u3059")))}u.isMDXComponent=!0},3950:function(t,e,r){e.Z=r.p+"assets/images/StbToBrep-066fbc05516654f5b3853bc4a2b010ce.png"},5352:function(t,e,r){e.Z=r.p+"assets/images/StbToLine-ebbeaafca626dd9fec3c53e8adc5376f.png"}}]);