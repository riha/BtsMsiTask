# BtsMsiTask #
*BtsMsiTask* expose a task for generating a MSI package directly from or many BizTalk project, without having to first install the resources into BizTalk Server. Read more about how and why to use the package in [this blog post](http://www.richardhallgren.com/export-biztalk-server-msi-packages-directly-from-visual-studio-using-btsmsitask/).

Download the project source and check the included sample for further information.

## Getting started ##
Download the [latest release](http://blogblob.blob.core.windows.net/btsmsitask/BtsMsiTask-0.3.exe) and run the installer.

Create a `Build.proj` file like below for the solution you like to generate a MSI package for.

	<Project DefaultTargets="GenerateMsi" xmlns="http://schemas.microsoft.com/developer/msbuild/2003">
	  <Import Project="$(MSBuildExtensionsPath)\BtsMsiTask\BtsMsiTask.targets" />
	  <PropertyGroup>
	    <DestinationPath>C:\Temp2\BtsSample</DestinationPath>
	    <ApplicationName>BtsSampleApp</ApplicationName>
	    <!--Optional-->
	    <ApplicationDescription>Test description ...</ApplicationDescription>
	    <!--Optional-->
	    <Version>1.0.0.1</Version>
 		<!--Optional-->
    	<FileName>Build 23456_2</FileName>
    	<!--Optional-->
    	<SourceLocation>\\acme.com\drops$\Build 23456_2</SourceLocation>
	  </PropertyGroup>
	  <ItemGroup>
		<!-- Add all non BizTalk resources to include in the MSI as Resource nodes -->
		<Resource Include="..\BtsSample.Utilities\bin\Debug\BtsSample.Utilities.dll" />
		<!-- Add all BizTalk resources to include in the MSI as BtsAssembly nodes -->
    	<BtsAssembly Include="..\BtsSample.Transforms\bin\Debug\BtsSample.Transforms.dll" />
	  </ItemGroup>
	  <ItemGroup>
		<!-- Add possible referenced BizTalk Applications. Optional. -->
	    <ReferenceApplication Include="BtsSample.SharedSchemas" />
	  </ItemGroup>
	  <Target Name="GenerateMsi">
	    <MsiTask  ApplicationDescription="$(ApplicationDescription)"
	              Version="$(Version)"
				  FileName="$(FileName)"
	              DestinationPath="$(DestinationPath)"
                  SourceLocation="$(SourceLocation)"
	              ApplicationName="$(ApplicationName)"
                  BtsAssemblies="@(BtsAssembly)"
	              Resources="@(Resource)" 
                  ReferenceApplications="@(ReferenceApplication)" />
	  </Target>
	</Project>

	 
The `Import` node references the BtsMsiTask MSBuild targets file from the installation folder.

`DestinationPath` sets the location to were the final MSI should be generated. `ApplicationName` sets the name of the BizTalk Application. 

The `Resource` ItemGroup list all the resources to include in the final MSI. Resource can be of two different types: BizTalk resources and non-BizTalk resources.

    <ItemGroup>
		<Resource Include="..\BtsSample.Utilities\bin\Debug\BtsSample.Utilities.dll" />
 		<BtsAssembly Include="..\BtsSample.Transforms\bin\Debug\BtsSample.Transforms.dll" />
	</ItemGroup>

Finally call the `MsiTask` with the declared parameters.

    <MsiTask  
      DestinationPath="$(DestinationPath)" 
      ApplicationName="$(ApplicationName)" 
      BtsAssemblies="@(BtsAssembly)" />
      </Target>
    </Project> 


## Parameters ##
<table border="0" cellpadding="3" cellspacing="0" width="90%" id="tasksTable">
	<tr>
		<th align="left" width="190">
        Paramter
        </th>
        <th align="left">
            Description
        </th>
        <th align="left">
            
        </th>
    </tr>
    <tr>
		<td>DestinationPath</td>
		<td><i>Required</i></td>
		<td>The end distanation to where the MSI will be published. By default the MSI file will be named as: <i>ApplicationName-yyyyMMddHHmmss</i>.</td>
	</tr>
    <tr>
		<td>ApplicationName</td>
		<td><i>Required</i></td>
		<td>The BizTalk Application that be created/updated by the MSI.</td>
	</tr>
    <tr>
		<td>ApplicationDescription</td>
		<td>Optional</td>
		<td>The BizTalk Description that will be added/updated to the BizTalk Application.</td>
	</tr>
    <tr>
		<td>Version</td>
		<td>Optional</td>
		<td>A possible version number added to the MSI. Uses a <i>1.0.0.0</i> format.</td>
	</tr>
 	<tr>
		<td>FileName</td>
		<td><i>Optional</i></td>
		<td>If set if will be used as the MSI file name.</td>
	</tr>
 	<tr>
		<td>SourceLocation</td>
		<td><i>Optional</i></td>
		<td>If set will be part of the MSI property for source location and visible in the BizTalk Administration console.</td>
	</tr>
	<tr>
		<td>BtsAssemblies (ItemGroup)</td>
		<td><i>Optional</i></td>
		<td>A list of <i>BizTalk</i> resources that should be added to the MSI.</td>
	</tr>
	<tr>
		<td>Resources (ItemGroup)</td>
		<td><i>Optional</i></td>
		<td>A list of <i>Non BizTalk</i> resouses that should be added to the MSI.</td>
	</tr>
    <tr>
		<td>ReferenceApplications (ItemGroup)</td>
		<td>Optional</td>
		<td>List of BizTalk application that should be refernces as the MIS is imported. By default the <i>System.BizTalk</i> is added.</td>
	</tr>
</table>
 
## Limitations  ##
- BtsMsiTask is currently only tested on BizTalk Server 2013 and BizTalk 2010.
- BizTalk MSI packages allows for several different resources to be added (bindings, pre and post scripts, web service definitions, non BizTalk dll etc). BtsMsiTask currently however only support dlls (BizTalk and non BizTalk dlls).  

## Releases ##
- **0.1** - [download](http://blogblob.blob.core.windows.net/btsmsitask/BtsMsiTask-0.1.exe)
	- Initial release with a number of known limitations.
- **0.3** - [download](http://blogblob.blob.core.windows.net/btsmsitask/BtsMsiTask-0.3.exe)
	- Added a few extra properties to set as MsBuild properties.
	- Added support for non BizTalk dll as part of MSI package.
- **0.5** - [download](http://blogblob.blob.core.windows.net/btsmsitask/BtsMsiTask-0.5.13.exe)
	- Changed a few build parameter names.
	- Added auomated build server.    

