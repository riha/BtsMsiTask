---
layout: default
---

<img style="float: right;padding:30px 10px 10px 10px;" src="assets/direct.png" />

<p class="abstract"><strong>MSI files are great!</strong> MSI:s provides an easy way to package a set of files and adds extra goodiness to ease deployment - <em>especially</em> when using BizTalk Server.</p> 

<p class="abstract">Traditionally the way to package BizTalk artifacts to an MSI involved installing the artifacts to a BizTalk Server and exporting them.</p>
<p class="abstract">BtsMsiTask however <strong>provides a way to package compiled artifacts directly into a MSI, without having to first installing them to a BizTalk Server.</strong> <a href="https://github.com/riha/BtsMsiTask">Open source</a> and <a href="{{ site.latest_download }}">free of charge</a>.</p>

## How does it work?

BtsMsiTask uses the exact same techniques as BizTalk Server does when exporting an MSI from the server. By reverse engineering we extracted the functionality and wrapped it in a stand alone library, not tied to the BizTalk Server infrastructure in any way. Any complied dll can now directly be packages into a BizTalk specific MSI.

We choose to package the deliver the functionality in BtsMsiTask as a [MsBuild](http://en.wikipedia.org/wiki/MSBuild) task.

## Getting started

Follow these three simple steps to get started and generate your first MSI.

**1. Download and install**

BtsMsiTask is delived as a installer that can be [downloaded here]({{ site.latest_download }}). 
Running it will by default install the binaries in the default MsBuild extension folder on your machine: `C:\Program Files (x86)\MSBuild\BtsMsiTask`.

**2. Create a MsBuild project file**

As BtsMsiTask executes as a MsBuild task you need to setup a `.proj` file to act as input to MsBuild. The `.proj` file will tell MuBuild what files to include in the MSI, where to write the file to and so on. There are a number of properties that can be set and they are listed [in detail here](/available-parameters), but a simple proj file could look something like this.

    <Project DefaultTargets="GenerateMsi" xmlns="http://schemas.microsoft.com/developer/msbuild/2003">
    	<Import Project="$(MSBuildExtensionsPath)\BtsMsiTask.targets" />
    	<PropertyGroup>
    		<DestinationPath>C:\Temp\BtsSample</DestinationPath>
    		<ApplicationName>BtsSampleApp</ApplicationName>
    	</PropertyGroup>
    	<ItemGroup>
    		<BtsAssembly Include="..\BtsSample.Transforms\bin\Debug\BtsSample.Transforms.dll" />
    		<BtsAssembly Include="..\BtsSample.Schemas\bin\Debug\BtsSample.Schemas.dll" />
    	</ItemGroup>
    		
    	<Target Name="GenerateMsi">
    		<MsiTask  
    			DestinationPath="$(DestinationPath)"
    			ApplicationName="$(ApplicationName)"
    			BtsAssemblies="@(BtsAssembly)"
    	</Target>
    </Project>

Download the file [here](https://raw.githubusercontent.com/riha/BtsMsiTask/tree/gh-pages/assets/proj-simple.proj).

**3. Run MsbUild**

Run MsBuild and point it to the created project file: `msbuild mypropj.proj`


