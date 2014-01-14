## BtsMsiTask ##
*BtsMsiTask* expose a task for generating a MSI package directly from or many BizTalk project, without having to first install the resources into BizTalk Server. Read more about how and why to use the package in this blog post (soon).

Download the project source and check the included sample for further information.

## Getting started ##
Download the [latest release](http://blogblob.blob.core.windows.net/btsmsitask/BtsMsiTask-0.1.exe) and run the installer.

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
	  </PropertyGroup>
	  <ItemGroup>
		<!-- Add all resources to include in the MSI -->
	    <Resource Include="..\BtsSample.Transforms\bin\Debug\BtsSample.Transforms.dll" />
	  </ItemGroup>
	  <ItemGroup>
		<!-- Add possible referenced BizTalk Applications. Optional. -->
	    <ReferenceApplication Include="BtsSample.SharedSchemas" />
	  </ItemGroup>
	  <Target Name="GenerateMsi">
	    <MsiTask  ApplicationDescription="$(ApplicationDescription)"
	              Version="$(Version)"
	              DestinationPath="$(DestinationPath)"
	              ApplicationName="$(ApplicationName)"
	              Resources="@(Resource)" />
	  </Target>
	</Project>

	 
The `Import` node references the BtsMsiTask MSBuild targets file from the installation folder.

`DestinationPath` sets the location to were the final MSI should be generated. `ApplicationName` sets the name of the BizTalk Application. 

The `Resource` ItemGroup list all the resources to include in the final MSI.

    <ItemGroup>
		<Resource Include="..\BtsSample.Transforms\bin\Debug\BtsSample.Transforms.dll" />
 		<Resource Include="..\BtsSample.Schemas\bin\Debug\BtsSample.Schemas.dll" />
	</ItemGroup>

Finally call the `MsiTask` with the declared parameters.

    <MsiTask  ApplicationDescription="$(ApplicationDescription)"
      Version="$(Version)"
      DestinationPath="$(DestinationPath)" 
      ApplicationName="$(ApplicationName)" 
      Resources="@(Resource)" />
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
		<td>**Required**</td>
		<td>The end distanation to where the MSI will be published. By default the MSI file will be named as: `ApplicationName-yyyyMMddHHmmss`.</td>
	</tr>
    <tr>
		<td>ApplicationName</td>
		<td>Required</td>
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
		<td>A possible version number added to the MSI. Uses a `1.0.0.0` format.</td>
	</tr>
    <tr>
		<td>Resource (ItemGroup)</td>
		<td>Required</td>
		<td>A list of resources that should be added to the MSI.</td>
	</tr>
    <tr>
		<td>ReferenceApplication (ItemGroup)</td>
		<td>Optional</td>
		<td>List of BizTalk application that should be refernces as the MIS is imported. By default the "System.BizTalk" is added.</td>
	</tr>
</table>
 
## Limitations  ##
- BtsMsiTask is currently only tested on BizTalk Server 2013.
- BizTalk MSI packages allows for several different resources to be added (bindings, pre and post scripts, web service definitions, non BizTalk dll etc). BtsMsiTask currently however only support BizTalk Server dlls.  

## Releases ##
- **0.1** - [download](http://blogblob.blob.core.windows.net/btsmsitask/BtsMsiTask-0.1.exe)
	- Initial release with a number of known limitations. 

