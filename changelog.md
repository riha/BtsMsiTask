---
layout: default
title: 'Changelog'
---

## Versions ##
- **1.1** - [download]({{ site.latest_download }})
	- Fixed issue with temp folders not being removed.
	- Typo in installer.
	- Fixes issue with SourceLocation property not being set. 
- **1.0** - [download](http://blogblob.blob.core.windows.net/btsmsitask/BtsMsiTask-1.1.21.exe)
	- Updated documentation and and comments.
- **0.5** - [download](http://blogblob.blob.core.windows.net/btsmsitask/BtsMsiTask-0.5.13.exe)
	- Changed build parameter names.
	- Added [automated build server](https://ci.appveyor.com/project/riha/btsmsitask). 
- **0.3** - [download](http://blogblob.blob.core.windows.net/btsmsitask/BtsMsiTask-0.3.exe)
	- Added a few extra properties to set as MsBuild properties.
	- Added support for non BizTalk dll as part of MSI package.
- **0.1** - [download](http://blogblob.blob.core.windows.net/btsmsitask/BtsMsiTask-0.1.exe)
	- Initial release with a number of known limitations.

Latest builds can be downloaded directly from the [build server](https://ci.appveyor.com/project/riha/btsmsitask).

## Current limitations  ##
- BtsMsiTask is currently tested on BizTalk Server 2013 and BizTalk 2010.
- BizTalk MSI packages allows for several different resources to be added (bindings, pre and post scripts, web service definitions, non BizTalk dll etc). BtsMsiTask currently however only support dlls (BizTalk and non BizTalk dlls).  
