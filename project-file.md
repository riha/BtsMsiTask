---
layout: default
---
## Project file example ##

The following example show a advanced project file with all [optional parameters](/available-parameters) set. A more basic example can be found [here](https://raw.githubusercontent.com/riha/BtsMsiTask/tree/gh-pages/assets/simple-advanced.proj).

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

Download the file [here](https://raw.githubusercontent.com/riha/BtsMsiTask/tree/gh-pages/assets/proj-advanced.proj).

### Import  Node###
The `Import` node references the BtsMsiTask MSBuild targets file from the installation folder.

### "DestinationPath" property###
`DestinationPath` sets the location to were the final MSI should be generated. 

### "ApplicationName" property###
`ApplicationName` sets the name of the BizTalk Application. 

### Resource ItemGroup property###
The `Resource` ItemGroup list all the resources to include in the final MSI. Resource can be of two different types: *BizTalk resources* and *non-BizTalk resources*.

    <ItemGroup>
		<Resource Include="..\BtsSample.Utilities\bin\Debug\BtsSample.Utilities.dll" />
 		<BtsAssembly Include="..\BtsSample.Transforms\bin\Debug\BtsSample.Transforms.dll" />
	</ItemGroup>

### Import  Node###
Finally call the `MsiTask` target with the declared parameters.

    <MsiTask  
      DestinationPath="$(DestinationPath)" 
      ApplicationName="$(ApplicationName)" 
      Resources="@(Resource)
      BtsAssemblies="@(BtsAssembly)" />