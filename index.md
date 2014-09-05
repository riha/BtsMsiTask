---
layout: default
---

<p class="abstract">MSI files are great. MSI:s provides an easy way to package a set of files, add attributes, dependency information and even extra functionally to ease deployment. Traditionally the way to package BizTalk artifacts to an MSI has involved installing the artifacts to a BizTalk server and exporting them. With BtsMsiTask it however possible to package compiled artifacts directly into a MSI package without having to first installing them to a BizTalk Server.</p>

<h2>
    <a name="how-does-it-work" class="anchor" href="#how-does-it-work"><span class="octicon octicon-link"></span></a>How does it work?</h2>

<p>
    BtsMsiTask uses the exact same techniques as BizTalk Server does when exporting a MSI form the server. 
                By reverse engineering we extracted the functionality and wrapped it in a stand alone library, not tied to the BizTalk Server infrastructure in any way. 
                Any complied dll can now directly be packages into a BizTalk specific MSI without any further hassle.
</p>
<p>In BtsMsiTask project we choose to package the functionality as a MsBuild task. This reason for this is that it suited our needs to use in a build server scenarion.</p>


<h2>
    <a name="how-does-it-work" class="anchor" href="#how-does-it-work"><span class="octicon octicon-link"></span></a>Getting started</h2>


<p>
    BtsMsiTask is delived as a installer that can be downloaded here. Running it will by default try and install the binaries that MSBuild task in the default MsBuild extension folder on your machine (C:\Program Files (x86)\MSBuild\).

</p>
<p>
    As BtsMsiTask today runs as a MsBuild task you will next need to setup a .proj file to act as input to MsBuild. The .proj file will be reponsible to tell MuBuild what files to
                include in the MSI, where to write the file to, what BizTalk application to install to and so on. There are a number of properties that can be set and they are listed in detail here,
                 but a simple proj file could look something like this.

                <pre>
                <code>
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
                    <MsiTask  DestinationPath="$(DestinationPath)"
                              ApplicationName="$(ApplicationName)"
                              BtsAssemblies="@(BtsAssembly)"
                  </Target>
                </Project>
                </code>
                </pre>
</p>
<p>
    Run MsBuild and point it to the created .proj file: 
</p>
