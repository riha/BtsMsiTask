## BtsMsiTask ##
*BtsMsiTask* expose a task for generating a MSI package directly from or many BizTalk project, without having to first install the resources into BizTalk Server. Read more about how and why to use the package in this blog post (soon).

Download the project source and check the included sample for further information.

## Getting started ##
Download the latest release here (soon). Save the generated dlls in a suitable location on disk.

Create a `Build.proj` (see this [sample](https://github.com/riha/BtsMsiTask/blob/master/Sample/Build/Build.proj)) file for the solution you like to generate the MSI for. 

Reference the dll 
`<UsingTask AssemblyFile="..\..\Src\BtsMsiTask\bin\Debug\BtsMsiTask.dll" TaskName="BtsMsiTask.MsiTask" />`

Set the `DestinationPath` to a location were the MSI should be generated. Set the `AppllicationName` to the name of the BizTalk Application. 

List all the resources to include in the final MSI as `Resource` nodes within a `ItemGroup` collection.

    <ItemGroup>
		<Resource Include="..\BtsSample.Transforms\bin\Debug\BtsSample.Transforms.dll" />
 		<Resource Include="..\BtsSample.Schemas\bin\Debug\BtsSample.Schemas.dll" />
	</ItemGroup>

Finally call the task with the declared parameters.

    <MsiTask  ApplicationDescription="$(ApplicationDescription)"
      Version="$(Version)"
      DestinationPath="$(DestinationPath)" 
      ApplicationName="$(ApplicationName)" 
      Resources="@(Resource)" />
      </Target>
    </Project> 


## Parameters ##
- **DestinationPath**	*Required*
	-  The end distanation to where the MSI will be published. By default the MSI file will be named as: `<ApplicationName>-yyyyMMddHHmmss`. 
- **ApplicationName** *Required*
	- The BizTalk Application that be created/updated by the MSI. 
- **ApplicationDescription** *Optional*
	- The BizTalk Description that will be added/updated to the BizTalk Application
- **Version** *Optional*
	- A possible version number added to the MSI. Uses a `1.0.0.0` format.
- **Resource** (ItemGroup) *Required*
	- A list of resources that should be added to the MSI.
- **ReferenceApplication**  (ItemGroup) *Optional*
	- List of BizTalk application that should be refernces as the MIS is imported. By default the `System.BizTalk` is added.
 
## Limitations  ##
- BtsMsiTask is currently only tested on BizTalk Server 2013.
- BizTalk MSI packages allows for several different resources to be added (bindings, pre and post scripts, web service definitions, non BizTalk dll etc). BtsMsiTask currently however only support BizTalk Server dlls.  

## Releases ##
- **0.1** - download (soon)
	- Initial release with a number of limitations. 

